# Configuração visual do elenco — itens do LPC por personagem.
# cor de peça com variantes -> {'variant': 'nome'}; peça recolorível -> colors={material: cor}
def B(skin_eyes=None): return {'id': 'body'}
def H(head, eyes): return {'id': head, 'colors': {'eyes': eyes}}
def F(eyes, face='face_neutral'): return {'id': face, 'colors': {'eyes': eyes}}
def hair(i, c): return {'id': i, 'colors': {'hair': c}}
def cl(i, c): return {'id': i, 'colors': {'cloth': c}}
def mt(i, c): return {'id': i, 'colors': {'metal': c}}
def vr(i, v): return {'id': i, 'variant': v}

APOLLO_WHITE, APOLLO_YELLOW, APOLLO_DARK, APOLLO_VISOR = 'all.lpcr.white', 'all.lpcr.tumeric', 'all.lpcr.shadow', 'all.lpcr.azure'

CAST = {
 'lip': dict(display='Felipe "Lip" Andrade', folder='Protagonist', body='male', skin='amber', items=[
    B(), H('heads_human_male','brown'), F('brown'), hair('hair_messy1','black'),
    cl('torso_clothes_longsleeve','slate'), cl('legs_pants','charcoal'), cl('feet_boots_basic','brown')]),
 'lip_mochila': dict(display='Lip (variante com mochila)', folder='Protagonist/variants/com_mochila', body='male', skin='amber', items=[
    B(), H('heads_human_male','brown'), F('brown'), hair('hair_messy1','black'),
    cl('torso_clothes_longsleeve','slate'), cl('legs_pants','charcoal'), cl('feet_boots_basic','brown'),
    vr('backpack','leather'), vr('backpack_straps','leather')]),
 'apollo': dict(display='Apollo (placeholder LPC)', folder='Companion/Apollo', body='male', skin='light', items=[
    B(), H('heads_human_male','blue'),
    mt('hat_helmet_close',APOLLO_WHITE), mt('hat_visor_slit',APOLLO_VISOR),
    cl('torso_clothes_longsleeve','yellow'), mt('torso_armour_plate',APOLLO_WHITE),
    cl('legs_pants','charcoal'), cl('feet_boots_basic','charcoal'), mt('arms_gloves',APOLLO_DARK)]),
 'ivo': dict(display='Ivo Rangel', folder='Antagonists/Ivo_Rangel', body='male', skin='olive', items=[
    B(), H('heads_human_male_elderly','gray'), F('gray'), {'id':'head_wrinkles'},
    hair('hair_balding','gray'), hair('beards_5oclock_shadow','gray'),
    cl('torso_clothes_longsleeve','gray'), vr('torso_aprons_overalls','slate'), cl('feet_boots_basic','charcoal')]),
 'nadir': dict(display='Nadir Quintela', folder='Antagonists/Nadir_Quintela', body='female', skin='taupe', items=[
    B(), H('heads_human_female','brown'), F('brown'), hair('hair_bob_side_part','black'),
    vr('torso_clothes_blouse_longsleeve','white'), cl('legs_formal','charcoal'), cl('feet_shoes_basic','black')]),
 'carregador': dict(display='Carregador da Apex (figurante)', folder='Antagonists/Carregador_Apex', body='male', skin='olive', items=[
    B(), H('heads_human_male','brown'), F('brown'), hair('hair_buzzcut','black'),
    cl('torso_clothes_longsleeve2','white'), cl('legs_pants2','gray'), cl('feet_boots_basic','gray'), vr('backpack','gray')]),
 'dara': dict(display='Dara', folder='NPCs/Dara', body='female', skin='brown', items=[
    B(), H('heads_human_female','brown'), F('brown'), hair('hair_ponytail','black'),
    cl('torso_clothes_longsleeve','bluegray'), cl('legs_pants','brown'), cl('feet_boots_basic','charcoal')]),
 'rasha': dict(display='Rasha', folder='NPCs/Rasha', body='female', skin='bronze', items=[
    B(), H('heads_human_female','brown'), F('brown'), hair('hair_pixie','black'),
    cl('torso_clothes_longsleeve','charcoal'),
    {'id':'torso_armour_leather','colors':{0:'maroon',1:'iron'}}, mt('shoulders_legion','all.lpcr.wine'),
    cl('legs_pants','charcoal'), cl('feet_boots_basic','black')]),
 'teodoro': dict(display='Teodoro Sampaio', folder='NPCs/Teodoro_Sampaio', body='male', skin='taupe', items=[
    B(), H('heads_human_male','gray'), F('gray'), hair('hair_parted','gray'), hair('beards_trimmed','gray'),
    cl('torso_clothes_longsleeve2_vneck','white'), vr('torso_jacket_trench','gray'), cl('legs_pants2','gray'), cl('feet_boots_basic','tan')]),
 'tacio': dict(display='Senhor Tácio', folder='NPCs/Tacio', body='male', skin='brown', items=[
    B(), H('heads_human_male','brown'), F('brown'), hair('hair_balding','gray'), hair('beards_mustache','gray'),
    cl('torso_clothes_shortsleeve','tan'), vr('torso_aprons_apron','leather'), cl('legs_pants','brown'), cl('feet_boots_basic','walnut'),
    {'id':'neck_necklace_chain','variant':'iron'}]),
 'bruna': dict(display='Mestra Bruna', folder='NPCs/Mestra_Bruna', body='female', skin='olive', items=[
    B(), H('heads_human_female','brown'), F('brown'), hair('hair_ponytail2','dark_gray'),
    cl('torso_clothes_tshirt','charcoal'), vr('torso_aprons_apron_full','leather'), cl('legs_pants','charcoal'), cl('feet_boots_basic','black'),
    mt('arms_gloves','iron'), mt('arms_bracers','iron')]),
 'nilce': dict(display='Nilce', folder='NPCs/Nilce', body='female', skin='light', items=[
    B(), H('heads_human_female','green'), F('green'), hair('hair_bangs_bun','gray'),
    vr('torso_clothes_blouse','tan'), cl('legs_skirts_plain','brown'), vr('torso_aprons_apron_half','gray'), cl('feet_shoes_basic','brown')]),
 'quirino': dict(display='Quirino', folder='NPCs/Quirino', body='male', skin='bronze', items=[
    B(), H('heads_human_male','brown'), F('brown'), hair('hair_buzzcut','black'), hair('beards_handlebar','black'),
    cl('torso_clothes_shortsleeve','gray'), vr('torso_aprons_apron','leather'), cl('legs_pants','charcoal'), cl('feet_boots_basic','black'),
    mt('arms_gloves','iron')]),
 'sula': dict(display='Sula', folder='NPCs/Sula', body='female', skin='taupe', items=[
    B(), H('heads_human_female','brown'), F('brown'), hair('hair_bob','black'),
    vr('torso_clothes_tunic','slate'), cl('legs_pants','bluegray'), cl('neck_scarf','maroon'), cl('feet_boots_basic','brown')]),
 'pipo': dict(display='Pipo', folder='NPCs/Pipo', body='child', skin='bronze', items=[
    B(), H('heads_human_child','brown'), hair('hair_relm_short','black'),
    vr('torso_clothes_child_shirt','gray'), vr('legs_childpants','brown')]),
 'onofre': dict(display='Seu Onofre', folder='NPCs/Seu_Onofre', body='male', skin='brown', items=[
    B(), H('heads_human_male_elderly','brown'), F('brown'), {'id':'head_wrinkles'},
    hair('hair_balding','white'), hair('beards_winter','white'),
    cl('torso_clothes_longsleeve','tan'), vr('torso_clothes_vest','brown'), cl('legs_pants','brown'), cl('feet_boots_basic','walnut')]),
 'homem_fila': dict(display='Homem da Fila (figurante)', folder='NPCs/Homem_da_Fila', body='male', skin='light', items=[
    B(), H('heads_human_male','brown'), F('brown'), hair('hair_plain','dark_brown'),
    cl('torso_clothes_longsleeve','forest'), cl('legs_pants','charcoal'), cl('feet_boots_basic','brown')]),
 'celia': dict(display='Célia Ferraz Rangel (referência de foto)', folder='NPCs/Celia_Ferraz_Rangel', body='female', skin='light', items=[
    B(), H('heads_human_female','brown'), F('brown'), hair('hair_ponytail','dark_brown'),
    cl('torso_clothes_longsleeve','gray'), vr('torso_aprons_overalls','slate'), cl('feet_boots_basic','charcoal')]),
}

def check_unique(key):
    import lpc
    seen = {}
    for s in CAST[key]['items']:
        t = lpc.IT[s['id']]['type_name']
        if t in seen: raise ValueError(f"{key}: type_name duplicado {t}: {seen[t]} / {s['id']}")
        seen[t] = s['id']
