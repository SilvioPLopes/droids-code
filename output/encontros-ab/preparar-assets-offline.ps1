$ErrorActionPreference = 'Stop'
# Preparação única de assets autorizados; não executa o Unity nem acessa saves.
$pastaProjeto = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$codificacao = [Text.UTF8Encoding]::new($false)
function Ler([string]$relativo) { [IO.File]::ReadAllText((Join-Path $pastaProjeto $relativo)) }
function Gravar([string]$relativo, [string]$texto) { [IO.File]::WriteAllText((Join-Path $pastaProjeto $relativo), $texto, $codificacao) }
function Bloco([string]$texto, [string]$id) {
    $padrao = '(?ms)^--- !u!\d+ &' + [regex]::Escape($id) + '\r?\n.*?(?=^--- !u!|\z)'
    $achados = [regex]::Matches($texto, $padrao)
    if ($achados.Count -ne 1) { throw "Exige bloco único: $id" }
    $achados[0].Value
}
function MetaPrefab([string]$relativo) {
    $guidNovo = [guid]::NewGuid().ToString('N')
    Gravar ($relativo + '.meta') ("fileFormatVersion: 2`nguid: $guidNovo`nPrefabImporter:`n  externalObjects: {}`n  userData: `n  assetBundleName: `n  assetBundleVariant: `n")
}
$sprite = 'Assets/sprites/Entities/Enemies/Battle/SucataReforcada-v2.png'
$prefabVisual = 'Assets/Prefabs/VisualInimigoBattle.prefab'
$prefabZona = 'Assets/Prefabs/ZonaEncontro.prefab'
foreach ($novo in @($sprite + '.meta', $prefabVisual, $prefabVisual + '.meta', $prefabZona, $prefabZona + '.meta')) {
    if (Test-Path -LiteralPath (Join-Path $pastaProjeto $novo)) { throw "Asset já existe; não repetir preparação: $novo" }
}
if (!(Test-Path -LiteralPath (Join-Path $pastaProjeto $sprite))) { throw 'PNG v2 ausente.' }
$guidB = [guid]::NewGuid().ToString('N')
$meta = Ler 'Assets/sprites/Entities/Enemies/Battle/SucataReforcada-v1.png.meta'
$meta = [regex]::Replace($meta, '(?m)^guid: [a-f0-9]+', "guid: $guidB")
$meta = [regex]::Replace($meta, '(?ms)^  internalIDToNameTable:.*?(?=^  externalObjects:)', "  internalIDToNameTable: []`n")
$spriteId = [guid]::NewGuid().ToString('N')
$sheet = @"
  spriteSheet:
    serializedVersion: 2
    sprites: []
    outline: []
    customData:
    physicsShape: []
    bones: []
    spriteID: $spriteId
    internalID: 0
    vertices: []
    indices:
    edges: []
    weights: []
    secondaryTextures: []
    spriteCustomMetadata:
      entries: []
    nameFileIdTable: {}
"@
$meta = [regex]::Replace($meta, '(?ms)^  spriteSheet:.*?(?=^  mipmapLimitGroupName:)', ($sheet + "`n"))
Gravar ($sprite + '.meta') $meta

$battle = Ler 'Assets/Scenes/Battle.unity'
$anterior = '  droidB: {fileID: 21300000, guid: 5ec105dfe4bc74b41b647dc095410121, type: 3}'
if ([regex]::Matches($battle, [regex]::Escape($anterior)).Count -ne 1) { throw 'Referência B anterior divergente.' }
$visuais = @"
  droidB: {fileID: 21300000, guid: $guidB, type: 3}
  visuais:
  - id: sucata_captura
    sprite: {fileID: 21300000, guid: 4c07a0f708b15674a9123d67ee3f152c, type: 3}
  - id: sucata_reforcada
    sprite: {fileID: 21300000, guid: $guidB, type: 3}
"@
$battle = $battle.Replace($anterior, $visuais.TrimEnd("`r", "`n"))
Gravar 'Assets/Scenes/Battle.unity' $battle

$cabecalho = "%YAML 1.1`n%TAG !u! tag:unity3d.com,2011:`n"
$blocos = foreach ($id in @('211052726', '211052727', '211052728', '211052729', '211052730')) { Bloco $battle $id }
$visual = $cabecalho + ($blocos -join '')
$visual = $visual.Replace('  m_Name: SpriteInimigo', '  m_Name: VisualInimigoBattle')
$visual = $visual.Replace('  m_Father: {fileID: 1232528688}', '  m_Father: {fileID: 0}')
$visual = $visual.Replace('  batalha: {fileID: 164412323}', '  batalha: {fileID: 0}')
Gravar $prefabVisual $visual
MetaPrefab $prefabVisual

$game = Ler 'Assets/Scenes/Game.unity'
$blocos = foreach ($id in @('450786052', '450786055', '450786053', '450786054')) { Bloco $game $id }
$zona = $cabecalho + ($blocos -join '')
$zona = $zona.Replace('  m_Name: ZonaBatalha', '  m_Name: ZonaEncontro')
$zona = $zona.Replace('  m_LocalPosition: {x: -2.5, y: 0, z: 0}', '  m_LocalPosition: {x: 0, y: 0, z: 0}')
$zona = $zona.Replace('  m_LocalScale: {x: 6.5, y: 6.5, z: 1}', '  m_LocalScale: {x: 1, y: 1, z: 1}')
Gravar $prefabZona $zona
MetaPrefab $prefabZona
"Assets preparados em disco; importação e execução no Unity pendentes. GUID B-v2: $guidB"
