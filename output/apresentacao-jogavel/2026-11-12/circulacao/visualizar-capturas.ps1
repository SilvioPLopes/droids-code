param([string]$Execucao)
$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing
$pasta="output/apresentacao-jogavel/2026-11-12/circulacao/execucoes/$Execucao/visual"
foreach($arquivo in (Get-ChildItem -LiteralPath $pasta -Filter '*.json')){
 $f=Get-Content -Raw -Encoding UTF8 -LiteralPath $arquivo.FullName|ConvertFrom-Json
 if(!(Test-Path -LiteralPath $f.imagem)){continue}
 $b=[Drawing.Bitmap]::new($f.imagem);$g=[Drawing.Graphics]::FromImage($b)
 $p=[Drawing.Pen]::new([Drawing.Color]::FromArgb(180,255,190,0),3)
 $pp=[Drawing.Pen]::new([Drawing.Color]::Lime,5)
 foreach($o in $f.colisores){
  if($o.trigger){continue}
  $pen=if($o.caminho -match '(^|/)Player$'){$pp}else{$p}
  if($o.raioPixel -gt 0){$r=[single]$o.raioPixel;$g.DrawEllipse($pen,[single]($o.centroPixel.x-$r),[single]($f.altura-$o.centroPixel.y-$r),[single](2*$r),[single](2*$r))}
  elseif($o.contornoPixel.Count -ge 3){
   $pts=[Drawing.PointF[]]@($o.contornoPixel|ForEach-Object{[Drawing.PointF]::new([single]$_.x,[single]($f.altura-$_.y))})
   if($o.tipo -eq 'EdgeCollider2D'){$g.DrawLines($p,$pts)}else{$g.DrawPolygon($p,$pts)}
  }
 }
 $saida=Join-Path $arquivo.DirectoryName ($arquivo.BaseName+'-colisores.png')
 $b.Save($saida,[Drawing.Imaging.ImageFormat]::Png);$p.Dispose();$pp.Dispose();$g.Dispose();$b.Dispose()
}
