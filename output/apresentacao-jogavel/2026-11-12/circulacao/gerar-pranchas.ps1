$Pasta='output/apresentacao-jogavel/2026-11-12/circulacao/inventario-inicial'
$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing
$base=(Get-Location).Path
$dados=Get-Content -Raw -Encoding UTF8 -LiteralPath (Join-Path $Pasta 'inventario.json')|ConvertFrom-Json
$cenas=@($dados.cenas|Where-Object nome -ne 'City'|Sort-Object nome)
$largura=1650; $altura=1470; $tileW=550; $tileH=470
$fonte=[Drawing.Font]::new('Segoe UI',14,[Drawing.FontStyle]::Bold)
$fontePequena=[Drawing.Font]::new('Segoe UI',9)
$branco=[Drawing.Brushes]::White
for($pagina=0;$pagina -lt [math]::Ceiling($cenas.Count/9);$pagina++){
  $bitmap=[Drawing.Bitmap]::new($largura,$altura)
  $g=[Drawing.Graphics]::FromImage($bitmap)
  $g.Clear([Drawing.Color]::FromArgb(25,25,29))
  $g.SmoothingMode=[Drawing.Drawing2D.SmoothingMode]::AntiAlias
  $g.DrawString('VERMELHO: blocos decorativos | LARANJA: demais solidos | CIANO: chegadas + corpo | AMARELO: portais',$fontePequena,$branco,10,5)
  for($n=0;$n -lt 9 -and ($pagina*9+$n) -lt $cenas.Count;$n++){
    $s=$cenas[$pagina*9+$n]; $f=$s.fundo
    $tx=($n%3)*$tileW; $ty=[math]::Floor($n/3)*$tileH+30
    $g.DrawString($s.nome,$fonte,$branco,$tx+8,$ty)
    if(!$f -or !$f.textureAssetPath){$g.DrawString('Fundo nao identificado: inspecao separada necessaria',$fontePequena,$branco,$tx+8,$ty+35);continue}
    $img=[Drawing.Image]::FromFile((Join-Path $base $f.textureAssetPath))
    $escala=[math]::Min(530/$f.rectWidthPixels,410/$f.rectHeightPixels)
    $dw=[single]($f.rectWidthPixels*$escala); $dh=[single]($f.rectHeightPixels*$escala)
    $ix=[single]($tx+($tileW-$dw)/2); $iy=[single]($ty+28)
    $dest=[Drawing.RectangleF]::new($ix,$iy,$dw,$dh)
    $src=[Drawing.RectangleF]::new($f.rectXPixels,($f.textureHeight-$f.rectYPixels-$f.rectHeightPixels),$f.rectWidthPixels,$f.rectHeightPixels)
    $g.DrawImage($img,$dest,$src,[Drawing.GraphicsUnit]::Pixel)
    $img.Dispose()
    function PX([double]$x){[single]($ix+($x-$f.minX)/($f.maxX-$f.minX)*$dw)}
    function PY([double]$y){[single]($iy+($f.maxY-$y)/($f.maxY-$f.minY)*$dh)}
    foreach($c in $s.solidos){
      if(!$c.active -or !$c.enabled -or $c.eDoPlayer){continue}
      $cor=if($c.obstaculoNumerado){[Drawing.Color]::FromArgb(110,255,30,30)}else{[Drawing.Color]::FromArgb(50,255,175,0)}
      $pincel=[Drawing.SolidBrush]::new($cor)
      $caneta=[Drawing.Pen]::new([Drawing.Color]::FromArgb(240,$cor.R,$cor.G,$cor.B),1.5)
      if($c.cornersWorld.Count -ge 3){
        $pts=[Drawing.PointF[]]@($c.cornersWorld|ForEach-Object{[Drawing.PointF]::new((PX $_.x),(PY $_.y))})
        $g.FillPolygon($pincel,$pts); $g.DrawPolygon($caneta,$pts)
      }elseif($c.tipo -eq 'CircleCollider2D'){
        $x=PX ($c.x-$c.raioWorld); $y=PY ($c.y+$c.raioWorld)
        $w=[single](2*$c.raioWorld/($f.maxX-$f.minX)*$dw);$h=[single](2*$c.raioWorld/($f.maxY-$f.minY)*$dh)
        $g.FillEllipse($pincel,$x,$y,$w,$h);$g.DrawEllipse($caneta,$x,$y,$w,$h)
      }elseif($c.pointsWorld.Count -ge 2){
        $pts=[Drawing.PointF[]]@($c.pointsWorld|ForEach-Object{[Drawing.PointF]::new((PX $_.x),(PY $_.y))})
        $g.DrawLines($caneta,$pts)
      }
      $pincel.Dispose();$caneta.Dispose()
    }
    $amarela=[Drawing.Pen]::new([Drawing.Color]::Yellow,2)
    foreach($p in $s.portais){foreach($id in $p.colliderPaths){
      $c=$s.triggers|Where-Object colliderPath -eq $id|Select-Object -First 1
      if($c){$g.DrawRectangle($amarela,(PX $c.minX),(PY $c.maxY),[single](($c.maxX-$c.minX)/($f.maxX-$f.minX)*$dw),[single](($c.maxY-$c.minY)/($f.maxY-$f.minY)*$dh))}
    }}
    $amarela.Dispose()
    $player=$s.players|Select-Object -First 1
    $chegadas=@()
    foreach($origem in $dados.cenas){
      foreach($p in $origem.portais|Where-Object destino -eq $s.nome){$chegadas+=@{x=$p.pontoChegadaX;y=$p.pontoChegadaY}}
      foreach($p in $origem.npcs|Where-Object destinoDeVisita -eq $s.nome){$chegadas+=@{x=$p.chegadaDaVisitaX;y=$p.chegadaDaVisitaY}}
    }
    $ciano=[Drawing.Pen]::new([Drawing.Color]::Cyan,2)
    foreach($p in $chegadas){
      $x=$p.x+$player.colliderCenterOffsetX; $y=$p.y+$player.colliderCenterOffsetY; $r=$player.raioWorld
      $g.DrawEllipse($ciano,(PX ($x-$r)),(PY ($y+$r)),[single](2*$r/($f.maxX-$f.minX)*$dw),[single](2*$r/($f.maxY-$f.minY)*$dh))
      $g.FillEllipse([Drawing.Brushes]::Cyan,(PX $p.x)-2,(PY $p.y)-2,4,4)
    }
    $ciano.Dispose()
    $blocos=@($s.solidos|Where-Object {$_.obstaculoNumerado -and $_.enabled -and $_.active}).Count
    $g.DrawString("blocos=$blocos | chegadas=$($chegadas.Count) | raio=$($player.raioWorld.ToString('0.00'))m",$fontePequena,$branco,$tx+8,$ty+440)
  }
  $saida=Join-Path $Pasta ('prancha-'+($pagina+1).ToString('D2')+'.png')
  $bitmap.Save((Join-Path $base $saida),[Drawing.Imaging.ImageFormat]::Png)
  $g.Dispose();$bitmap.Dispose()
  Write-Output $saida
}
$fonte.Dispose();$fontePequena.Dispose()
