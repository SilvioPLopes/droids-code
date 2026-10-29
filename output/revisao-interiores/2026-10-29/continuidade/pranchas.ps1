# Pranchas diagnósticas; somente lê imagens e grava cópias anotadas em output.
Add-Type -AssemblyName System.Drawing
$raizProjeto = (Get-Location).Path
$dirAuditoria = Join-Path $raizProjeto 'output/revisao-interiores/2026-10-29/continuidade'
$config = Get-Content -Raw -Encoding UTF8 'output/montagem-ferrovale/2026-10-29/config-montagem.json' | ConvertFrom-Json
$nomes = @($config.rooms.sceneName) + @('CasaLip')
$font = [System.Drawing.Font]::new('Arial',14)
$pen = [System.Drawing.Pen]::new([System.Drawing.Color]::Yellow,3)
for ($inicio = 0; $inicio -lt $nomes.Count; $inicio += 4) {
    $bmp = [System.Drawing.Bitmap]::new(1448,1220)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    try {
        $g.Clear([System.Drawing.Color]::FromArgb(25,25,25))
        for ($n = 0; $n -lt 4 -and ($inicio+$n) -lt $nomes.Count; $n++) {
            $dados = Get-Content -Raw -Encoding UTF8 (Join-Path $dirAuditoria ($nomes[$inicio+$n]+'.json')) | ConvertFrom-Json
            $dx = ($n % 2)*724; $dy = [math]::Floor($n/2)*610
            $g.DrawString($dados.scene,$font,[System.Drawing.Brushes]::White,$dx+8,$dy+3)
            $img = [System.Drawing.Image]::FromFile((Join-Path $raizProjeto $dados.background.path))
            try {
                $fator = [math]::Min(724.0/$img.Width,543.0/$img.Height)
                $largura = [int]($img.Width*$fator); $altura = [int]($img.Height*$fator)
                $imgX = $dx+[int]((724-$largura)/2); $imgY = $dy+28
                $g.DrawImage($img,[System.Drawing.Rectangle]::new($imgX,$imgY,$largura,$altura))
            } finally { $img.Dispose() }
            $num = 0
            foreach ($portal in $dados.portals) {
                $num++
                $x = $imgX+$largura*$portal.pixel.x/$dados.background.width
                $y = $imgY+$altura*$portal.pixel.y/$dados.background.height
                $g.DrawEllipse($pen,[single]($x-9),[single]($y-9),18,18)
                $g.DrawString([string]$num,$font,[System.Drawing.Brushes]::Yellow,[single]($x+9),[single]($y-9))
            }
            $legenda = @($dados.portals | ForEach-Object {$_.target -replace 'Interior_',''}) -join ' | '
            $g.DrawString($legenda,$font,[System.Drawing.Brushes]::White,$dx+8,$dy+575)
        }
        $bmp.Save((Join-Path $dirAuditoria ('prancha-'+(1+$inicio/4)+'.jpg')),[System.Drawing.Imaging.ImageFormat]::Jpeg)
    } finally { $g.Dispose(); $bmp.Dispose() }
}
$font.Dispose(); $pen.Dispose()
Write-Output '8 pranchas diagnósticas; nenhuma imagem de Assets alterada.'
