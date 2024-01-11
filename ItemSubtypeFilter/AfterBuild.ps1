Set-Location $PSScriptRoot

$ModName = "ItemSubtypeFilter"

$TargetDir = $args[1];

Write-Output "TargetDir: $TargetDir"

$ModPath = "$env:TAIWU_PATH\Mod\$ModName"

New-Item $ModPath -ItemType "directory"
New-Item "$ModPath\Plugins" -ItemType "directory"

$ConfigLuaPath = "$TargetDir"+"Config.lua"

Write-Output "ConfigLuaPath: $ConfigLuaPath"

$Dll = "$TargetDir${ModName}.dll"

& "../../../MynahModConfigGenerator/bin/Debug/net6.0/MynahModConfigGenerator.exe" $ConfigLuaPath $Dll

Copy-Item $Dll "$ModPath\Plugins" -Verbose
Copy-Item Config.lua $ModPath -Verbose

Write-Output "copy over"
