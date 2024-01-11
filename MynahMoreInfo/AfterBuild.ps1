Set-Location $PSScriptRoot

$ModName = "MynahMoreInfo"

$TargetDir = $args[1];
$FileId = $args[0]

Write-Output "TargetDir: $TargetDir"
Write-Output "FileId: $FileId"

$ModPath = "$env:TAIWU_PATH\Mod\$ModName"

New-Item $ModPath -ItemType "directory"
New-Item "$ModPath\Plugins" -ItemType "directory"

$ConfigLuaPath = "$ModPath"+"\Config.lua"

Write-Output "ConfigLuaPath: $ConfigLuaPath"

$Dll = "$TargetDir${ModName}Frontend.dll"
$Pdb = "$TargetDir${ModName}Frontend.pdb"

& "../../../MynahModConfigGenerator/bin/Debug/net6.0/MynahModConfigGenerator.exe" $ConfigLuaPath $Dll

Copy-Item $Dll "$ModPath\Plugins" -Verbose
Copy-Item $Pdb "$ModPath\Plugins" -Verbose
#Copy-Item Config.lua $ModPath -Verbose

Write-Output "copy over"
