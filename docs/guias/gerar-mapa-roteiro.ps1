# Executar na raiz do projeto. Apenas gera a imagem documental.
$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing
$r=Get-Content -Raw -Encoding UTF8 docs/guias/imagens/roteiro-100-pontos.json|ConvertFrom-Json
$f=$r.fundo
$src=[Drawing.Bitmap]::new((Join-Path (Get-Location) $f.textureAssetPath))
$b=[Drawing.Bitmap]::new(1920,1080);$g=[Drawing.Graphics]::FromImage($b)
$g.DrawImage($src,[Drawing.Rectangle]::new(0,0,1920,1080))
$font=[Drawing.Font]::new('Segoe UI',16,[Drawing.FontStyle]::Bold)
$pen=[Drawing.Pen]::new([Drawing.Color]::Black,3)
foreach($p in $r.pontos){
 $x=[single](($f.rectXPixels+($p.x-$f.minX)/($f.maxX-$f.minX)*$f.rectWidthPixels)/$src.Width*1920)
 $y=[single](($f.textureHeight-$f.rectYPixels-($p.y-$f.minY)/($f.maxY-$f.minY)*$f.rectHeightPixels)/$src.Height*1080)
 $x=[single][Math]::Max(20,[Math]::Min(1900,$x));$y=[single][Math]::Max(20,[Math]::Min(1060,$y))
 $g.FillEllipse([Drawing.Brushes]::Yellow,$x-17,$y-17,34,34)
 $g.DrawEllipse($pen,$x-17,$y-17,34,34)
 $fmt=[Drawing.StringFormat]::new();$fmt.Alignment=[Drawing.StringAlignment]::Center;$fmt.LineAlignment=[Drawing.StringAlignment]::Center
 $g.DrawString([string]$p.n,$font,[Drawing.Brushes]::Black,[Drawing.RectangleF]::new($x-17,$y-17,34,34),$fmt);$fmt.Dispose()
}
$b.Save((Join-Path (Get-Location) 'docs/guias/imagens/roteiro-100-cidade.png'),[Drawing.Imaging.ImageFormat]::Png)
$src.Dispose();$g.Dispose();$b.Dispose();$font.Dispose();$pen.Dispose()
