"""Previa padrao; aplicacao explicita altera somente 98 m_Enabled de 30 prefabs.

Python 3.12, biblioteca padrao. Nao reserializa YAML, nao modifica .meta,
nao controla Unity e nao oferece reversao automatica de uma falha parcial.
"""
from __future__ import annotations

import argparse
import hashlib
import json
import os
from pathlib import Path, PurePosixPath
import re
import sys
from dataclasses import dataclass
from typing import Any


SCRIPT = Path(__file__).resolve()
PROJECT = SCRIPT.parents[4]
EVIDENCE = SCRIPT.parent
INVENTORY = EVIDENCE / "inventario-inicial/inventario.json"
MANIFEST = EVIDENCE / "mobiliario-preview.json"
APPLICATION = EVIDENCE / "aplicacao-mobiliario.json"
BASELINE = EVIDENCE / "baseline/mobiliario"
PREFABS = PROJECT / "Assets/Prefabs/Interiores"
SCENES = PROJECT / "Assets/Scenes/Interiores"
EXPECTED_PREFABS = 30
EXPECTED_COMPONENTS = 98
EXPECTED_SCENES = 31

HEADER = re.compile(rb"(?m)^--- !u!(\d+) &(-?\d+)(?: stripped)?[ \t]*\r?$")
NAME = re.compile(r"Obstaculo_\d+\Z", re.ASCII)
GUID = re.compile(rb"(?m)^guid: ([0-9a-fA-F]{32})[ \t]*\r?$")
REFERENCE_GUID = re.compile(rb"\bguid: ([0-9a-fA-F]{32})\b")


class Refusal(RuntimeError):
    pass


def require(condition: bool, message: str) -> None:
    if not condition:
        raise Refusal(message)


def sha(data: bytes) -> str:
    return hashlib.sha256(data).hexdigest().upper()


def relative(path: Path) -> str:
    return path.relative_to(PROJECT).as_posix()


def evidence_path(path: Path) -> Path:
    require(path.resolve().is_relative_to(EVIDENCE), f"Destino externo de evidencia: {path}")
    return path


def prefab_path(value: str) -> Path:
    require(isinstance(value, str) and "\\" not in value, "Prefab path invalido.")
    logical = PurePosixPath(value)
    require(not logical.is_absolute() and ".." not in logical.parts, f"Prefab path invalido: {value}")
    path = PROJECT.joinpath(*logical.parts)
    resolved = path.resolve(strict=True)
    require(resolved.is_relative_to(PREFABS) and path.suffix == ".prefab",
            f"Prefab fora de Assets/Prefabs/Interiores/: {value}")
    require(relative(path) == value, f"Prefab path nao canonico: {value}")
    return path


@dataclass(frozen=True)
class Block:
    kind: int
    identifier: int
    start: int
    end: int
    data: bytes


@dataclass
class Prepared:
    path: Path
    meta: Path
    before: bytes
    after: bytes
    meta_before: bytes
    record: dict[str, Any]


def blocks(data: bytes) -> list[Block]:
    data.decode("utf-8-sig", errors="strict")
    headers = list(HEADER.finditer(data))
    require(bool(headers), "YAML Unity sem blocos identificaveis.")
    result = []
    ids: set[int] = set()
    for index, header in enumerate(headers):
        identifier = int(header[2])
        require(identifier not in ids, f"fileID duplicado no YAML: {identifier}")
        ids.add(identifier)
        end = headers[index + 1].start() if index + 1 < len(headers) else len(data)
        result.append(Block(int(header[1]), identifier, header.start(), end, data[header.start():end]))
    return result


def scalar(block: Block, key: str) -> tuple[bytes, re.Match[bytes]]:
    pattern = re.compile(rb"(?m)^  " + re.escape(key.encode("ascii")) + rb": ([^\r\n]*?)[ \t]*\r?$")
    matches = list(pattern.finditer(block.data))
    require(len(matches) == 1, f"Campo unico {key} ausente/duplicado no bloco {block.identifier}.")
    return matches[0][1], matches[0]


def object_link(block: Block) -> int:
    value, _ = scalar(block, "m_GameObject")
    match = re.fullmatch(rb"\{fileID: (-?\d+)\}", value)
    require(match is not None, f"m_GameObject invalido em {block.identifier}.")
    return int(match[1])


def has_field(block: Block, key: str) -> bool:
    return re.search(rb"(?m)^  " + re.escape(key.encode("ascii")) + rb": ", block.data) is not None


def game_name(block: Block) -> str:
    value, _ = scalar(block, "m_Name")
    name = value.decode("utf-8")
    # Os nomes alvo sao ASCII. Suporta a forma simples e strings YAML entre aspas.
    if name.startswith('"'):
        name = json.loads(name)
    elif len(name) >= 2 and name[0] == name[-1] == "'":
        name = name[1:-1].replace("''", "'")
    require(isinstance(name, str), f"Nome invalido em {block.identifier}.")
    return name


def prove_only_enabled(before: bytes, after: bytes, positions: list[int], ids: set[int]) -> None:
    require(len(before) == len(after), "Edicao mudou o tamanho em bytes do prefab.")
    changed = {index for index, (a, b) in enumerate(zip(before, after)) if a != b}
    require(changed == set(positions) and len(changed) == len(ids),
            "Diff nao corresponde exclusivamente aos m_Enabled previstos.")
    require(all(before[index:index + 1] == b"1" and after[index:index + 1] == b"0" for index in changed),
            "Diff contem valor diferente de m_Enabled 1 -> 0.")
    old_blocks, new_blocks = blocks(before), blocks(after)
    require([(b.kind, b.identifier) for b in old_blocks] == [(b.kind, b.identifier) for b in new_blocks],
            "Edicao alterou identidade/ordem dos blocos.")
    for old, new in zip(old_blocks, new_blocks):
        if old.identifier not in ids:
            require(old.data == new.data, f"Bloco fora do alvo foi alterado: {old.identifier}")
        else:
            require(old.kind == new.kind == 61, "Alvo nao e BoxCollider2D.")
            old_value, old_match = scalar(old, "m_Enabled")
            new_value, new_match = scalar(new, "m_Enabled")
            require(old_value == b"1" and new_value == b"0", "Valor m_Enabled divergente.")
            require(old.data[:old_match.start(1)] + b"?" + old.data[old_match.end(1):]
                    == new.data[:new_match.start(1)] + b"?" + new.data[new_match.end(1):],
                    f"Geometria/outro campo mudou no bloco {old.identifier}.")


def prepare_prefab(path: Path, candidates: list[dict[str, Any]]) -> Prepared:
    before = path.read_bytes()
    meta = Path(str(path) + ".meta")
    require(meta.resolve(strict=True).is_relative_to(PREFABS), f"Meta externo: {meta}")
    meta_bytes = meta.read_bytes()
    meta_bytes.decode("utf-8-sig", errors="strict")
    guid_matches = GUID.findall(meta_bytes)
    require(len(guid_matches) == 1, f"GUID ausente/duplicado: {relative(meta)}")
    parsed = blocks(before)
    # Referencias "stripped" de prefabs aninhados nao possuem estes campos e ficam byte a byte intactas.
    objects = {block.identifier: block for block in parsed if block.kind == 1 and has_field(block, "m_Name")}
    names = {identifier: game_name(block) for identifier, block in objects.items()}
    named_objects = {identifier: name for identifier, name in names.items() if NAME.fullmatch(name)}
    expected = {candidate["nome"]: candidate for candidate in candidates}
    require(len(expected) == len(candidates), f"Nome alvo duplicado no inventario: {relative(path)}")
    require(set(expected) == set(named_objects.values()) and len(named_objects) == len(expected),
            f"GameObjects Obstaculo_N divergem do inventario: {relative(path)}")
    transforms = {object_link(block): block for block in parsed if block.kind == 4 and has_field(block, "m_GameObject")}
    targets = [block for block in parsed if block.kind == 61 and has_field(block, "m_GameObject")
               and object_link(block) in named_objects]
    require(len(targets) == len(expected), f"Quantidade de BoxCollider2D alvo divergente: {relative(path)}")
    require(len({object_link(block) for block in targets}) == len(targets),
            f"Mais de um BoxCollider2D por objeto alvo: {relative(path)}")
    after = bytearray(before)
    positions: list[int] = []
    components = []
    for block in targets:
        object_id = object_link(block)
        name = named_objects[object_id]
        go = objects[object_id]
        require(scalar(go, "m_IsActive")[0] == b"1", f"GameObject alvo inativo: {name}")
        linked_components = re.findall(rb"(?m)^  - component: \{fileID: (-?\d+)\}[ \t]*\r?$", go.data)
        require(sum(int(value) == block.identifier for value in linked_components) == 1,
                f"BoxCollider2D nao esta associado ao m_Component do GameObject: {name}")
        require(scalar(block, "m_IsTrigger")[0] == b"0", f"Alvo trigger recusado: {name}")
        enabled, enabled_match = scalar(block, "m_Enabled")
        require(enabled == b"1", f"Alvo ja desativado/divergente: {relative(path)}:{name}")
        require(object_id in transforms, f"Transform do alvo ausente: {name}")
        position = block.start + enabled_match.start(1)
        require(before[position:position + 1] == b"1", "Offset m_Enabled invalido.")
        after[position] = ord("0")
        positions.append(position)
        candidate = expected[name]
        require(candidate["caminho"].split("/")[-1] == name
                and candidate["colliderPath"] == candidate["caminho"] + "/BoxCollider2D[0]",
                f"Caminho/componente do inventario nao corresponde ao alvo: {name}")
        changed_block = block.data[:enabled_match.start(1)] + b"0" + block.data[enabled_match.end(1):]
        components.append({
            "gameObjectFileID": str(object_id), "componentFileID": str(block.identifier), "nome": name,
            "propriedade": "m_Enabled", "antes": 1, "depois": 0, "offsetByte": position,
            "blocoSHA256Antes": sha(block.data), "blocoSHA256DepoisPrevisto": sha(changed_block),
            "geometriaOriginalYaml": {key: scalar(block, key)[0].decode("utf-8")
                                      for key in ("m_Offset", "m_Size", "m_EdgeRadius")},
            "transformOriginalYaml": transforms[object_id].data.decode("utf-8"),
            "inventarioOriginal": candidate,
        })
    result = bytes(after)
    prove_only_enabled(before, result, positions, {block.identifier for block in targets})
    record = {
        "path": relative(path), "metaPath": relative(meta), "guid": guid_matches[0].decode("ascii").lower(),
        "sha256Antes": sha(before), "sha256DepoisPrevisto": sha(result), "metaSHA256Antes": sha(meta_bytes),
        "backupPath": relative(BASELINE / relative(path)),
        "backupMetaPath": relative(BASELINE / relative(meta)),
        "bytesAntes": len(before), "bytesDepois": len(result), "utf8BOM": before.startswith(b"\xef\xbb\xbf"),
        "crlf": before.count(b"\r\n"), "lfSemCR": before.count(b"\n") - before.count(b"\r\n"),
        "componentes": sorted(components, key=lambda item: int(item["componentFileID"])),
        "consumidores": [], "validacaoDiff": "Somente um byte 1->0 de m_Enabled por componente; demais bytes iguais.",
    }
    return Prepared(path, meta, before, result, meta_bytes, record)


def prepare() -> tuple[dict[str, Any], list[Prepared]]:
    inventory_bytes = INVENTORY.read_bytes()
    inventory = json.loads(inventory_bytes.decode("utf-8-sig"))
    scenes = [scene for scene in inventory["cenas"] if scene["path"].startswith("Assets/Scenes/Interiores/")]
    scene_paths = sorted(SCENES.glob("*.unity"))
    require(len(scene_paths) == EXPECTED_SCENES and len(scenes) == EXPECTED_SCENES
            and {relative(path) for path in scene_paths} == {scene["path"] for scene in scenes},
            "Escopo de 31 cenas de interiores diverge do inventario/disco.")
    grouped: dict[str, list[dict[str, Any]]] = {}
    for scene in scenes:
        for collider in scene["solidos"]:
            if not (collider.get("enabled") is True and collider.get("active") is True
                    and collider.get("obstaculoNumerado") is True and collider.get("tipo") == "BoxCollider2D"
                    and collider.get("isTrigger") is False):
                continue
            require(NAME.fullmatch(collider["nome"]) is not None and not collider.get("eDoPlayer"),
                    "Nome/tipo de alvo nao autorizado no inventario.")
            source = collider["prefabSourcePath"]
            prefab_path(source)
            grouped.setdefault(source, []).append({"cena": scene["path"], **collider})
    count = sum(map(len, grouped.values()))
    require(len(grouped) == EXPECTED_PREFABS and count == EXPECTED_COMPONENTS,
            f"Esperados 30 prefabs/98 componentes; encontrados {len(grouped)}/{count}.")
    prepared = [prepare_prefab(prefab_path(path), grouped[path]) for path in sorted(grouped)]
    require(sum(len(item.record["componentes"]) for item in prepared) == EXPECTED_COMPONENTS,
            "Contagem YAML nao corresponde aos 98 componentes.")
    guid_to_record = {item.record["guid"]: item.record for item in prepared}
    require(len(guid_to_record) == EXPECTED_PREFABS, "GUID duplicado entre prefabs alvo.")
    consumers = scene_paths + [PROJECT / "Assets/Scenes/City.unity", PROJECT / "Assets/Scenes/CasaLip.unity"]
    consumers += sorted(PREFABS.glob("*.prefab"))
    scanned = []
    for consumer in sorted(set(consumers)):
        resolved = consumer.resolve(strict=True)
        require(resolved.is_relative_to(PROJECT / "Assets"), f"Consumidor fora de Assets: {consumer}")
        data = consumer.read_bytes()
        scanned.append({"path": relative(consumer), "sha256": sha(data)})
        found = [value.decode("ascii").lower() for value in REFERENCE_GUID.findall(data)]
        for guid in sorted(set(found).intersection(guid_to_record)):
            guid_to_record[guid]["consumidores"].append({"path": relative(consumer), "referencias": found.count(guid)})
    require(all(item.record["consumidores"] for item in prepared), "Prefab alvo sem consumidor por GUID.")
    manifest = {
        "versao": 1, "modo": "preview", "inventarioPath": relative(INVENTORY),
        "inventarioSHA256": sha(inventory_bytes), "prefabs": EXPECTED_PREFABS,
        "componentes": EXPECTED_COMPONENTS, "cenasInteriores": EXPECTED_SCENES,
        "propriedadePermitida": "BoxCollider2D.m_Enabled: 1 -> 0, apenas fileIDs listados",
        "consumidoresAnalisados": scanned, "arquivos": [item.record for item in prepared],
    }
    return manifest, prepared


def json_bytes(value: Any) -> bytes:
    return (json.dumps(value, ensure_ascii=False, sort_keys=True, indent=2) + "\n").encode("utf-8")


def same_or_new(path: Path, data: bytes, write: bool) -> None:
    evidence_path(path)
    if path.exists():
        require(path.read_bytes() == data, f"Evidencia/backup existente diferente; nao sobrescrito: {relative(path)}")
    elif write:
        path.parent.mkdir(parents=True, exist_ok=True)
        with path.open("xb") as stream:
            stream.write(data)
            stream.flush()
            os.fsync(stream.fileno())


def backup_check(prepared: list[Prepared], write: bool, require_existing: bool = False) -> None:
    for item in prepared:
        for source, data in ((item.path, item.before), (item.meta, item.meta_before)):
            backup = BASELINE / relative(source)
            if require_existing:
                require(backup.is_file(), f"Backup obrigatorio ausente: {relative(backup)}")
            same_or_new(backup, data, write)


def preview() -> dict[str, Any]:
    manifest, prepared = prepare()
    output = json_bytes(manifest)
    same_or_new(MANIFEST, output, write=False)
    backup_check(prepared, write=False)
    # Toda validacao de alvos/hash/diff precede a gravacao de qualquer evidencia.
    backup_check(prepared, write=True)
    same_or_new(MANIFEST, output, write=True)
    return {"modo": "preview", "status": "VALIDADO", "prefabs": len(prepared),
            "componentes": sum(len(item.record["componentes"]) for item in prepared),
            "backups": len(prepared) * 2, "consumidoresAnalisados": len(manifest["consumidoresAnalisados"]),
            "manifesto": relative(MANIFEST), "assetsAlterados": 0}


def journal(value: dict[str, Any]) -> None:
    evidence_path(APPLICATION)
    temporary = evidence_path(APPLICATION.with_suffix(".json.tmp"))
    # Temporario somente na pasta de evidencias, nunca dentro de Assets.
    with temporary.open("xb") as stream:
        stream.write(json_bytes(value))
        stream.flush()
        os.fsync(stream.fileno())
    os.replace(temporary, APPLICATION)


def apply() -> dict[str, Any]:
    require(MANIFEST.is_file(), "Execute --preview antes de --aplicar.")
    require(not APPLICATION.exists(), "Aplicacao anterior/ parcial registrada; inspecione antes de qualquer nova mutacao.")
    require(not APPLICATION.with_suffix(".json.tmp").exists(), "Journal temporario anterior existe; inspecione-o.")
    manifest, prepared = prepare()
    require(MANIFEST.read_bytes() == json_bytes(manifest),
            "Previa diverge dos hashes/IDs/consumidores atuais; nenhum asset alterado.")
    backup_check(prepared, write=False, require_existing=True)
    # Preflight de TODOS os 30 arquivos e seus metas antes da primeira mutacao de asset.
    for item in prepared:
        require(item.path.read_bytes() == item.before and item.meta.read_bytes() == item.meta_before,
                f"Arquivo/meta mudou durante preflight: {relative(item.path)}")
    require(sha(INVENTORY.read_bytes()) == manifest["inventarioSHA256"], "Inventario mudou durante preflight.")
    report: dict[str, Any] = {
        "versao": 1, "status": "PREPARADO", "manifesto": relative(MANIFEST),
        "manifestoSHA256": sha(MANIFEST.read_bytes()), "prefabsPrevistos": EXPECTED_PREFABS,
        "componentesPrevistos": EXPECTED_COMPONENTS, "escritos": [], "tentados": [], "emGravacao": None,
        "rollbackAutomatico": False,
        "arquivos": [{"path": item.record["path"], "sha256Antes": sha(item.before),
                      "sha256DepoisPrevisto": sha(item.after), "metaSHA256": sha(item.meta_before),
                      "componentes": len(item.record["componentes"])} for item in prepared],
    }
    journal(report)
    try:
        for item in prepared:
            require(item.path.read_bytes() == item.before and item.meta.read_bytes() == item.meta_before,
                    f"Arquivo/meta mudou antes da gravacao: {relative(item.path)}")
            report["status"], report["emGravacao"] = "EM_GRAVACAO", relative(item.path)
            report["tentados"].append(relative(item.path))
            journal(report)
            # Comprimento igual: escreve somente o arquivo autorizado, sem reserializar ou tocar .meta.
            with item.path.open("r+b") as stream:
                require(stream.read() == item.before, f"Arquivo mudou ao abrir: {relative(item.path)}")
                stream.seek(0)
                written = stream.write(item.after)
                require(written == len(item.after), f"Gravacao incompleta: {relative(item.path)}")
                stream.flush()
                os.fsync(stream.fileno())
            report["escritos"].append(relative(item.path))
            require(item.path.read_bytes() == item.after, f"Hash apos gravacao divergente: {relative(item.path)}")
            require(item.meta.read_bytes() == item.meta_before, f"Meta mudou durante aplicacao: {relative(item.meta)}")
            report["emGravacao"] = None
            journal(report)
        for item in prepared:
            require(item.path.read_bytes() == item.after and item.meta.read_bytes() == item.meta_before,
                    f"Verificacao final de bytes/meta falhou: {relative(item.path)}")
            for record in report["arquivos"]:
                if record["path"] == relative(item.path):
                    record["sha256Depois"] = sha(item.path.read_bytes())
        report["status"] = "APLICADO"
        report["componentesAlterados"] = EXPECTED_COMPONENTS
        report["validacaoDiff"] = "98 bytes m_Enabled 1->0; todos demais bytes e todos metas preservados."
        journal(report)
    except BaseException as error:
        report["status"], report["erro"] = "FALHA_PARCIAL", str(error)
        report["estadoAtual"] = []
        for item in prepared:
            try:
                current = item.path.read_bytes()
                state = {"path": relative(item.path), "sha256Atual": sha(current),
                         "igualAntes": current == item.before, "igualDepois": current == item.after,
                         "metaPreservado": item.meta.read_bytes() == item.meta_before}
            except OSError as inspection_error:
                state = {"path": relative(item.path), "erroLeitura": str(inspection_error)}
            report["estadoAtual"].append(state)
        try:
            journal(report)
        except OSError as journal_error:
            report["erroJournal"] = str(journal_error)
        print(json.dumps({"modo": "aplicar", "status": report["status"], "erro": str(error),
                          "escritos": report["escritos"], "tentados": report["tentados"],
                          "emGravacao": report["emGravacao"],
                          "relatorio": relative(APPLICATION), "rollbackAutomatico": False}, ensure_ascii=False))
        return {"exitCode": 1}
    return {"modo": "aplicar", "status": "APLICADO", "prefabs": len(prepared),
            "componentes": EXPECTED_COMPONENTS, "relatorio": relative(APPLICATION)}


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    mode = parser.add_mutually_exclusive_group()
    mode.add_argument("--preview", action="store_true", help="Valida e guarda previa/backups; padrao.")
    mode.add_argument("--aplicar", action="store_true", help="Aplica exclusivamente a previa validada.")
    args = parser.parse_args()
    try:
        result = apply() if args.aplicar else preview()
        if "exitCode" in result:
            return result["exitCode"]
        print(json.dumps(result, ensure_ascii=False))
        return 0
    except (Refusal, OSError, UnicodeError, ValueError, KeyError, TypeError) as error:
        print(json.dumps({"modo": "aplicar" if args.aplicar else "preview", "status": "RECUSADO",
                          "erro": str(error),
                          "assetsAlterados": None if args.aplicar and APPLICATION.exists() else 0,
                          "relatorio": relative(APPLICATION) if args.aplicar and APPLICATION.exists() else None},
                         ensure_ascii=False))
        return 1


if __name__ == "__main__":
    sys.exit(main())
