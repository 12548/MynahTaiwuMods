Set-Location $PSScriptRoot

$ModName = "MynahBaseMod"
$TargetDir = $args[0]

New-Item "$env:TAIWU_PATH\Mod\$ModName" -ItemType "directory"
New-Item "$env:TAIWU_PATH\Mod\$ModName\Plugins" -ItemType "directory"

$ConfigLuaPath = "$TargetDir"+"Config.lua"

Write-Output "ConfigLuaPath: $ConfigLuaPath"

& "../../../MynahModConfigGenerator/bin/Debug/net6.0/MynahModConfigGenerator.exe" $ConfigLuaPath "$TargetDir${ModName}Frontend.dll"

Copy-Item "$TargetDir*.dll" "$env:TAIWU_PATH\Mod\$ModName\Plugins" -Verbose
Copy-Item $ConfigLuaPath "$env:TAIWU_PATH\Mod\$ModName" -Verbose

Write-Output "copy over"
