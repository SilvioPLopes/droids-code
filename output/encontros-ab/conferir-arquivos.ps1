$ErrorActionPreference = 'Stop'
$pastaProjeto = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
function Ler([string]$caminho) { [IO.File]::ReadAllText((Join-Path $pastaProjeto $caminho)) }
function Blocos([string]$texto) {
    $resultado = @{}
    foreach ($m in [regex]::Matches($texto, '(?ms)^--- !u!(\d+) &(\d+)\r?\n.*?(?=^--- !u!|\z)')) {
        $chave = $m.Groups[1].Value + ':' + $m.Groups[2].Value
        if ($resultado.ContainsKey($chave)) { throw "fileID duplicado: $chave" }
        $resultado[$chave] = $m.Value.Replace("`r`n", "`n")
    }
    $resultado
}
$resumo = [Collections.Generic.List[string]]::new()
foreach ($cena in @('Game','Battle')) {
    $anterior = Blocos (Ler "output/backups/2026-10-29-encontros-ab/aplicacao-offline/$cena.unity")
    $atual = Blocos (Ler "Assets/Scenes/$cena.unity")
    if ($anterior.Count -ne $atual.Count) { throw "Contagem de blocos mudou: $cena" }
    $mudou = @($anterior.Keys | Where-Object { $anterior[$_] -cne $atual[$_] } | Sort-Object)
    $permitidos = if ($cena -eq 'Game') { @('114:450786053') } else { @('114:164412323','114:211052730') }
    if (($mudou -join ',') -ne (($permitidos | Sort-Object) -join ',')) { throw "Diff fora de escopo: $cena $mudou" }
    $resumo.Add("$cena : somente blocos permitidos $($mudou -join ', '); geometria e demais objetos preservados.")
}
foreach ($arquivo in @('Assets/Prefabs/VisualInimigoBattle.prefab','Assets/Prefabs/ZonaEncontro.prefab')) {
    $texto = Ler $arquivo
    $blocos = Blocos $texto
    $ids = @($blocos.Keys | ForEach-Object { ($_ -split ':')[1] })
    foreach ($referencia in [regex]::Matches($texto, '\{fileID: (-?\d+)\}')) {
        $id = $referencia.Groups[1].Value
        if ($id -ne '0' -and $id -notin $ids) { throw "Referência local ausente em $arquivo : $id" }
    }
    if ($arquivo.Contains('Visual') -and $texto -notmatch 'batalha: \{fileID: 0\}') { throw 'Prefab com gestor externo.' }
    $resumo.Add("$arquivo : referências locais fechadas e meta presente: $(Test-Path (Join-Path $pastaProjeto ($arquivo+'.meta'))).")
}
$meta = Ler 'Assets/sprites/Entities/Enemies/Battle/SucataReforcada-v2.png.meta'
$guidB = [regex]::Match($meta,'(?m)^guid: ([a-f0-9]+)').Groups[1].Value
foreach ($arquivo in @('Assets/Scenes/Battle.unity','Assets/Prefabs/VisualInimigoBattle.prefab')) {
    if ([regex]::Matches((Ler $arquivo), "guid: $guidB,").Count -ne 2) { throw "B-v2 não está referenciada em ambos os campos: $arquivo" }
}
foreach ($marcador in @('(?m)^  spriteMode: 1$', '(?m)^  nPOTScale: 0$', '(?m)^    filterMode: 0$', '(?m)^    enableMipMap: 0$')) {
    if ($meta.Replace("`r`n","`n") -notmatch $marcador) { throw "Importação sem campo esperado: $marcador" }
}
$resumo.Add('B-v2 referenciada por GUID na cena/prefab, com meta Single/Point/sem mipmap/NPOT.')
$base = Ler 'output/backups/2026-10-29-encontros-ab/baseline.json' | ConvertFrom-Json
$protegidos = @('City.unity','CasaLip.unity','Canvas.prefab','PainelStatus.prefab','PainelBag.prefab','CatalogoDeInimigos.cs','TerminalDroidApi.cs')
foreach ($registro in $base) {
    if ([IO.Path]::GetFileName($registro.Path) -notin $protegidos) { continue }
    if ((Get-FileHash -LiteralPath $registro.Path).Hash -ne $registro.Hash) { throw "Dependência protegida mudou: $($registro.Path)" }
}
$resumo.Add('Hashes protegidos de City/CasaLip/Canvas/Bag/Status/catálogo/terminal preservados.')
$resumo.Add('Somente conferência estrutural de arquivos; NÃO compila C#, importa assets ou testa comportamento no Unity.')
$resumo | Set-Content (Join-Path $PSScriptRoot 'conferencia-arquivos.txt')
$resumo
