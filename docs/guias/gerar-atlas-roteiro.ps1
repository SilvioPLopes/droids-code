# Executar na raiz do repositorio; gera apenas imagens documentais, sem Unity.
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$inventory = Get-Content -Raw -Encoding UTF8 'output/apresentacao-jogavel/2026-11-12/circulacao/inventario-colisores-ativos/inventario.json' | ConvertFrom-Json
$rooms = @($inventory.cenas | Where-Object nome -ne 'City')
$font = [Drawing.Font]::new('Segoe UI',17,[Drawing.FontStyle]::Bold)
$small = [Drawing.Font]::new('Segoe UI',13)
$center = [Drawing.StringFormat]::new()
$center.Alignment = [Drawing.StringAlignment]::Center
$center.LineAlignment = [Drawing.StringAlignment]::Center
function Draw-Room($scene, $graphics, [int]$left, [int]$top, [int]$width, [int]$height) {
    $f = $scene.fundo
    $src = [Drawing.Bitmap]::new((Join-Path (Get-Location) $f.textureAssetPath))
    $ratio = [Math]::Min($width / $src.Width, ($height - 72) / $src.Height)
    $w = [int]($src.Width * $ratio); $h = [int]($src.Height * $ratio)
    $dx = $left + [int](($width-$w)/2); $dy = $top + 43
    $graphics.DrawString($scene.nome,$font,[Drawing.Brushes]::White,$left+12,$top+8)
    $graphics.DrawImage($src,[Drawing.Rectangle]::new($dx,$dy,$w,$h))
    $n=0
    foreach($portal in $scene.portais) {
        $n++
        $collider = @($scene.triggers + $scene.solidos | Where-Object { $portal.colliderPaths -contains $_.colliderPath }) | Select-Object -First 1
        if($null -eq $collider) { continue }
        $px = $f.rectXPixels + ($collider.x-$f.minX)/($f.maxX-$f.minX)*$f.rectWidthPixels
        $py = $f.textureHeight-$f.rectYPixels-($collider.y-$f.minY)/($f.maxY-$f.minY)*$f.rectHeightPixels
        $x=[single]($dx+[Math]::Max(14,[Math]::Min($w-14,$px*$ratio)))
        $y=[single]($dy+[Math]::Max(14,[Math]::Min($h-14,$py*$ratio)))
        $graphics.FillEllipse([Drawing.Brushes]::Yellow,$x-13,$y-13,26,26)
        $graphics.DrawString([string]$n,$small,[Drawing.Brushes]::Black,[Drawing.RectangleF]::new($x-13,$y-13,26,26),$center)
    }
    $graphics.DrawString('Portas numeradas: legenda no guia; fundo sem NPCs.',$small,[Drawing.Brushes]::White,$left+12,$top+$height-25)
    $src.Dispose()
}
for($page=0; $page -lt 8; $page++) {
    $bitmap=[Drawing.Bitmap]::new(1600,1440)
    $graphics=[Drawing.Graphics]::FromImage($bitmap)
    $graphics.Clear([Drawing.Color]::FromArgb(24,29,35))
    for($tile=0;$tile -lt 4;$tile++) {
        $i=$page*4+$tile
        if($i -lt $rooms.Count) { Draw-Room $rooms[$i] $graphics (($tile%2)*800) ([int][Math]::Floor($tile/2)*720) 800 720 }
    }
    $bitmap.Save((Join-Path (Get-Location) ('docs/guias/imagens/atlas-interiores-{0}.jpg' -f ($page+1))),[Drawing.Imaging.ImageFormat]::Jpeg)
    $graphics.Dispose();$bitmap.Dispose()
}
$city=$inventory.cenas | Where-Object nome -eq 'City'
$bitmap=[Drawing.Bitmap]::new(1920,1200);$graphics=[Drawing.Graphics]::FromImage($bitmap)
$graphics.Clear([Drawing.Color]::FromArgb(24,29,35))
Draw-Room $city $graphics 0 0 1920 1200
$bitmap.Save((Join-Path (Get-Location) 'docs/guias/imagens/atlas-portas-cidade.jpg'),[Drawing.Imaging.ImageFormat]::Jpeg)
$graphics.Dispose();$bitmap.Dispose();$font.Dispose();$small.Dispose();$center.Dispose()
$src=[Drawing.Bitmap]::new((Join-Path (Get-Location) 'Assets/sprites/Mapas/ferrovale-original-upscale-4x.png'))
$bitmap=[Drawing.Bitmap]::new(1600,914);$graphics=[Drawing.Graphics]::FromImage($bitmap)
$graphics.DrawImage($src,[Drawing.Rectangle]::new(0,0,1600,914))
$bitmap.Save((Join-Path (Get-Location) 'docs/guias/imagens/atlas-cratera.jpg'),[Drawing.Imaging.ImageFormat]::Jpeg)
$src.Dispose();$graphics.Dispose();$bitmap.Dispose()
