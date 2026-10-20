"""Compositor LPC (Python) que replica a lógica do renderer oficial do
Universal LPC Spritesheet Character Generator (sources/canvas/renderer.ts),
usando as definições do repositório clonado (sheet_definitions/ + spritesheets/)."""
import json, os
from PIL import Image

ROOT = '/home/claude/lpc'
META = '/home/claude/meta'
IT  = json.load(open(f'{META}/itemMetadata.json'))
LAY = json.load(open(f'{META}/itemLayers.json'))
PM  = json.load(open(f'{META}/paletteMetadata.json'))
CRED = json.load(open(f'{META}/itemCredits.json'))

FRAME = 64
SHEET_W, SHEET_H = 13 * FRAME, 3456
OFFSETS = {  # ANIMATION_OFFSETS (constants.ts)
    'spellcast': 0, 'thrust': 4*FRAME, 'walk': 8*FRAME, 'slash': 12*FRAME, 'shoot': 16*FRAME,
    'hurt': 20*FRAME, 'climb': 21*FRAME, 'idle': 22*FRAME, 'jump': 26*FRAME, 'sit': 30*FRAME,
    'emote': 34*FRAME, 'run': 38*FRAME, 'combat_idle': 42*FRAME, 'backslash': 46*FRAME,
    'halfslash': 50*FRAME,
}
DIRS = ['up', 'left', 'down', 'right']  # ordem das linhas

def _hex(h):
    h = h.lstrip('#'); return tuple(int(h[i:i+2], 16) for i in (0, 2, 4))

def palette(material, key):
    """key: 'cor' | 'versao.cor' | 'material.versao.cor' -> lista de cores hex"""
    mm = PM['materials'][material]
    parts = key.split('.')
    if len(parts) == 3: material, ver, col = parts; mm = PM['materials'][material]
    elif len(parts) == 2: ver, col = parts
    else: ver, col = mm['default'], parts[0]
    return mm['palettes'][ver][col]

def _supports(meta, anim):
    a = meta.get('animations') or []
    if not a: return False
    if anim == 'combat_idle': return 'combat' in a
    if anim == 'thrust': return 'thrust' in a or 'watering' in a
    if anim == 'backslash': return '1h_slash' in a or '1h_backslash' in a
    if anim == 'halfslash': return '1h_halfslash' in a
    return anim in a

class Char:
    """items: lista ordenada de dicts {id, variant?, colors?{chave:cor}}"""
    def __init__(self, name, body_type, skin, items):
        self.name, self.body_type, self.skin, self.items = name, body_type, skin, items

    # ---- resolução de caminho / recolor
    def _name_key(self, item_id): return IT[item_id]['name'].replace(' ', '_')
    def _selected(self, type_name):
        for s in self.items:
            if IT[s['id']]['type_name'] == type_name: return s
        return None

    def _base_path(self, item_id, layer):
        p = layer[self.body_type]
        if '${' in p:
            meta = IT[item_id]
            for tn, table in meta['replace_in_path'].items():
                sel = self._selected(tn)
                if sel:
                    rep = table.get(self._name_key(sel['id']))
                    p = p.replace('${%s}' % tn, rep if rep else '')
        return p

    def _mappings(self, sel):
        meta = IT[sel['id']]
        maps = []
        colors = sel.get('colors', {})
        for idx, rc in enumerate(meta.get('recolors', [])):
            mat = rc['material']
            target = colors.get(idx) or colors.get(rc.get('type_name') or '_') or colors.get(mat)
            if rc.get('type_name') is None and not target and mat == 'body' and meta['matchBodyColor']:
                target = self.skin
            if not target: continue
            src = palette(mat, rc['base'] if '.' in rc['base'] else f"{rc['default']}.{rc['base']}")
            maps.append((src, palette(mat, target)))
        return maps

    def draw_calls(self):
        calls = []
        for sel in self.items:
            meta = IT[sel['id']]
            if self.body_type not in meta['required']: continue
            for n in range(1, 10):
                layer = LAY[sel['id']].get(f'layer_{n}')
                if not layer: break
                if layer.get('custom_animation'): continue  # armas oversize etc. (não usado)
                calls.append((layer.get('zPos', 100), sel, n, layer))
        calls.sort(key=lambda c: c[0])  # estável
        return calls

    def render(self, anims=None):
        import numpy as np
        anims = anims or list(OFFSETS)
        sheet = Image.new('RGBA', (SHEET_W, SHEET_H), (0, 0, 0, 0))
        missing = []
        calls = self.draw_calls()
        order = sorted(OFFSETS.items(), key=lambda kv: kv[1])
        heights = {a: ((order[i+1][1] if i+1 < len(order) else SHEET_H) - y) for i, (a, y) in enumerate(order)}
        for anim in anims:
            y = OFFSETS[anim]
            block = Image.new('RGBA', (SHEET_W, heights[anim]), (0, 0, 0, 0))
            for z, sel, n, layer in calls:
                meta = IT[sel['id']]
                if not _supports(meta, anim): continue
                base = self._base_path(sel['id'], layer)
                if sel.get('variant'):
                    rel = f"spritesheets/{base}{anim}/{sel['variant'].replace(' ', '_')}.png"
                else:
                    rel = f"spritesheets/{base}{anim}.png"
                path = os.path.join(ROOT, rel)
                if not os.path.exists(path):
                    missing.append((sel['id'], anim, rel)); continue
                img = Image.open(path).convert('RGBA')
                maps = self._mappings(sel) if not sel.get('variant') else []
                if maps:
                    a = np.array(img); flat = a.reshape(-1, 4)
                    pairs = [(_hex(s), _hex(t)) for src_, tgt in maps for s, t in zip(src_, tgt)]
                    keys = (flat[:, 0].astype(np.int64) << 16) | (flat[:, 1].astype(np.int64) << 8) | flat[:, 2].astype(np.int64)
                    uniq, inv = np.unique(keys, return_inverse=True)
                    newcols = np.zeros((len(uniq), 3), dtype=np.uint8); has = np.zeros(len(uniq), dtype=bool)
                    for ui, k in enumerate(uniq):
                        r, g, b = (k >> 16) & 255, (k >> 8) & 255, k & 255
                        t = next((t for s, t in pairs if abs(r-s[0]) <= 1 and abs(g-s[1]) <= 1 and abs(b-s[2]) <= 1), None)
                        if t: newcols[ui] = t; has[ui] = True
                    sel_mask = has[inv] & (flat[:, 3] > 0)
                    flat[sel_mask, :3] = newcols[inv][sel_mask]
                    img = Image.fromarray(flat.reshape(a.shape), 'RGBA')
                layer_img = Image.new('RGBA', block.size, (0, 0, 0, 0))
                layer_img.paste(img, (0, 0))
                block = Image.alpha_composite(block, layer_img)
            # mascara magenta -> transparente (mask.ts)
            b = np.array(block); m = (b[..., 3] > 0) & (b[..., 0] == 255) & (b[..., 1] == 44) & (b[..., 2] == 230)
            b[m] = (0, 0, 0, 0); block = Image.fromarray(b, 'RGBA')
            sheet.paste(block, (0, y))
        return sheet, missing

    def coverage(self):
        """animações em que TODAS as peças (exceto expressão facial) têm arte."""
        its = [s for s in self.items if self.body_type in IT[s['id']]['required']]
        return [a for a in OFFSETS if all(_supports(IT[s['id']], a) for s in its if IT[s['id']]['type_name'] != 'expression')]

    def hash_params(self):
        """parâmetros de hash no formato do gerador (hash.ts) — NÃO testado no navegador."""
        p = [('sex', self.body_type)]
        for s in self.items:
            meta = IT[s['id']]; name = meta['name'].replace(' ', '_')
            if s.get('variant'):
                p.append((meta['type_name'], f"{name}_{s['variant'].replace(' ', '_')}")); continue
            colors = s.get('colors', {})
            for idx, rc in enumerate(meta.get('recolors', [])):
                mat = rc['material']; tn = rc.get('type_name')
                target = colors.get(idx) or colors.get(tn or '_') or colors.get(mat)
                if rc.get('type_name') is None:
                    if not target and mat == 'body' and meta['matchBodyColor']: target = self.skin
                    p.append((meta['type_name'], f"{name}_{target}" if target else name))
                elif target:
                    p.append((tn, f"{rc['label'].replace(' ', '_')}_{target}"))
        out = {}
        for k, v in p: out[k] = v
        return out

    def url(self):
        base = 'https://liberatedpixelcup.github.io/Universal-LPC-Spritesheet-Character-Generator/#'
        return base + '&'.join(f'{k}={v}' for k, v in self.hash_params().items())

SINGLE_ROW = {'hurt', 'climb'}
def frame(sheet, anim, direction, col):
    row = 0 if anim in SINGLE_ROW else DIRS.index(direction)
    y = OFFSETS[anim] + row * FRAME
    return sheet.crop((col*FRAME, y, col*FRAME+FRAME, y+FRAME))
