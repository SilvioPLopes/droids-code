$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$tiles = @(Get-Content -LiteralPath (Join-Path $PSScriptRoot 'manifesto.json') -Raw | ConvertFrom-Json)
$destination = Join-Path $PSScriptRoot 'previa-unificada-3840.png'
if (Test-Path -LiteralPath $destination) { throw 'A previa ja existe; nao sobrescrever.' }
if ($tiles.Count -ne 13) { throw 'Esperadas 13 secoes.' }
$area = 0
foreach ($tile in $tiles) {
    if ($tile.x -lt 0 -or $tile.y -lt 0 -or $tile.width -le 0 -or $tile.height -le 0 -or ($tile.x + $tile.width) -gt 1920 -or ($tile.y + $tile.height) -gt 1080) { throw 'Retangulo fora do mapa.' }
    $area += $tile.width * $tile.height
}
for ($i = 0; $i -lt $tiles.Count; $i++) {
    for ($j = $i + 1; $j -lt $tiles.Count; $j++) {
        $a = $tiles[$i]; $b = $tiles[$j]
        if ($a.x -lt ($b.x + $b.width) -and $b.x -lt ($a.x + $a.width) -and $a.y -lt ($b.y + $b.height) -and $b.y -lt ($a.y + $a.height)) { throw 'Retangulos sobrepostos.' }
    }
}
if ($area -ne (1920 * 1080)) { throw 'Cobertura incompleta.' }
$bitmap = [System.Drawing.Bitmap]::new(3840, 2160)
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)
try {
    $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $graphics.CompositingMode = [System.Drawing.Drawing2D.CompositingMode]::SourceCopy
    foreach ($tile in $tiles) {
        $source = [System.Drawing.Image]::FromFile((Join-Path $PSScriptRoot ($tile.id + '.png')))
        try {
            if ($source.Width -ne $tile.outputWidth -or $source.Height -ne $tile.outputHeight) { throw ('Dimensoes divergentes: ' + $tile.id) }
            $rect = [System.Drawing.Rectangle]::new(($tile.x * 2), ($tile.y * 2), ($tile.width * 2), ($tile.height * 2))
            $graphics.DrawImage($source, $rect, 0, 0, $source.Width, $source.Height, [System.Drawing.GraphicsUnit]::Pixel)
        } finally { $source.Dispose() }
    }
    $bitmap.Save($destination, [System.Drawing.Imaging.ImageFormat]::Png)
} finally { $graphics.Dispose(); $bitmap.Dispose() }
Write-Output 'Previa salva: 3840x2160; 13 secoes; cobertura integral sem sobreposicao. Reamostragem por retangulo, sem retoque ou integracao Unity.'
