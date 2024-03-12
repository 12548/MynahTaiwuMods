Set-Location $PSScriptRoot

$ModName = $args[0]
$DllName = $args[1]
$TargetDir = $args[2];

Write-Output "TargetDir: $TargetDir"

$ModPath = "$env:TAIWU_PATH\Mod\$ModName"

New-Item $ModPath -ItemType "directory"
New-Item "$ModPath\Plugins" -ItemType "directory"

$Dll = "$TargetDir${DllName}"

Copy-Item $Dll "$ModPath\Plugins" -Verbose

Write-Output "copy over"
