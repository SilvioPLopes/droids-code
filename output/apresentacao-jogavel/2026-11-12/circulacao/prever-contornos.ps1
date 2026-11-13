$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing
$basePath='output/apresentacao-jogavel/2026-11-12/circulacao'
$inv=Get-Content -Raw -Encoding UTF8 "$basePath/inventario-inicial/inventario.json"|ConvertFrom-Json
$ajustes=Get-Content -Raw -Encoding UTF8 "$basePath/contornos-corrigidos.json"|ConvertFrom-Json
$outPath="$basePath/contornos-preview"
New-Item -ItemType Directory -Force -Path $outPath|Out-Null
foreach($grupo in ($ajustes.alvos|Group-Object cena)){
  $scene=$inv.cenas|Where-Object nome -eq $grupo.Name
  $img=[Drawing.Bitmap]::new((Join-Path (Get-Location) $scene.fundo.textureAssetPath))
  $g=[Drawing.Graphics]::FromImage($img)
  $font=[Drawing.Font]::new('Segoe UI',16,[Drawing.FontStyle]::Bold)
  $oldPen=[Drawing.Pen]::new([Drawing.Color]::Red,3);$oldPen.DashStyle=[Drawing.Drawing2D.DashStyle]::Dash
  $newPen=[Drawing.Pen]::new([Drawing.Color]::Lime,3)
  foreach($a in $grupo.Group){
    $c=$scene.solidos|Where-Object nome -eq $a.nome
    $f=$scene.fundo
    $x=[single](($c.minX-$f.minX)/($f.maxX-$f.minX)*$f.textureWidth)
    $y=[single](($f.maxY-$c.maxY)/($f.maxY-$f.minY)*$f.textureHeight)
    $w=[single]($c.width/($f.maxX-$f.minX)*$f.textureWidth)
    $h=[single]($c.height/($f.maxY-$f.minY)*$f.textureHeight)
    $g.DrawRectangle($oldPen,$x,$y,$w,$h)
    $p=$a.pixels
    $g.DrawRectangle($newPen,[single]$p[0],[single]$p[1],[single]($p[2]-$p[0]),[single]($p[3]-$p[1]))
    $g.DrawString("$($a.nome) [$($p -join ',')]",$font,[Drawing.Brushes]::Lime,[single]$p[0],[single]($p[1]-24))
  }
  $g.DrawString('VERMELHO: antes | VERDE: contorno proposto em pixels; colisor ATIVO',$font,[Drawing.Brushes]::White,55,1018)
  $img.Save((Join-Path (Get-Location) "$outPath/$($grupo.Name).png"),[Drawing.Imaging.ImageFormat]::Png)
  $oldPen.Dispose();$newPen.Dispose();$font.Dispose();$g.Dispose();$img.Dispose()
}
Write-Output '5 comparacoes geradas; nenhuma alteracao de Assets'
