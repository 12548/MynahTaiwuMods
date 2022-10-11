Set-Location $PSScriptRoot

$ModName = "MynahMoreInfo"
$TargetDir = $args[0]

New-Item "$env:TAIWU_PATH\Mod\$ModName" -ItemType "directory"
New-Item "$env:TAIWU_PATH\Mod\$ModName\Plugins" -ItemType "directory"

Copy-Item "$TargetDir*.dll" "$env:TAIWU_PATH\Mod\$ModName\Plugins" -Verbose

Write-Output "copy over"
