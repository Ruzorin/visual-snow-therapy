Add-Type -AssemblyName System.Drawing

$baseDir = $PSScriptRoot
$assetsDir = Join-Path $baseDir "Assets"
if (-not (Test-Path $assetsDir)) { New-Item -ItemType Directory -Path $assetsDir | Out-Null }

$bmp = New-Object System.Drawing.Bitmap 256, 256
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = 'HighQuality'
$g.Clear([System.Drawing.Color]::Transparent)

# Rose-amber circle background
$brush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(224, 169, 175))
$g.FillEllipse($brush, 8, 8, 240, 240)

# Dark border
$pen = New-Object System.Drawing.Pen ([System.Drawing.Color]::FromArgb(180, 100, 110)), 6
$g.DrawEllipse($pen, 8, 8, 240, 240)

# Eye shape
$eyePath = New-Object System.Drawing.Drawing2D.GraphicsPath
$eyePath.AddArc(48, 88, 160, 100, 180, 180)
$eyePath.AddArc(48, 68, 160, 100, 0, 180)
$eyeBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::White)
$g.FillPath($eyeBrush, $eyePath)

# Iris
$irisBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(180, 100, 110))
$g.FillEllipse($irisBrush, 98, 98, 60, 60)

# Pupil
$pupilBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(40, 20, 25))
$g.FillEllipse($pupilBrush, 113, 113, 30, 30)

# Reflection
$reflBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(200, 255, 255, 255))
$g.FillEllipse($reflBrush, 120, 108, 10, 10)

$g.Dispose()

# Export as PNG bytes
$ms = New-Object System.IO.MemoryStream
$bmp.Save($ms, [System.Drawing.Imaging.ImageFormat]::Png)
$pngBytes = $ms.ToArray()
$ms.Dispose()

# Build ICO file
$icoPath = Join-Path $assetsDir "app.ico"
$fs = [System.IO.File]::Create($icoPath)
$w = New-Object System.IO.BinaryWriter($fs)
# ICO header
$w.Write([UInt16]0)     # reserved
$w.Write([UInt16]1)     # type ICO
$w.Write([UInt16]1)     # 1 image
# Directory entry (16 bytes)
$w.Write([byte]0)       # width 256
$w.Write([byte]0)       # height 256
$w.Write([byte]0)       # colors
$w.Write([byte]0)       # reserved
$w.Write([UInt16]1)     # planes
$w.Write([UInt16]32)    # bpp
$w.Write([UInt32]$pngBytes.Length)
$w.Write([UInt32]22)    # offset = 6 + 16
# Image data
$w.Write($pngBytes)
$w.Flush()
$w.Close()
$fs.Close()
$bmp.Dispose()

Write-Host "Icon created: $icoPath ($((Get-Item $icoPath).Length) bytes)"
