Set-Location $PSScriptRoot

$ModName = "MakeItemPreview"

$TargetDir = $args[1];
$FileId = $args[0]

Write-Output "TargetDir: $TargetDir"
Write-Output "FileId: $FileId"

if($FileId) {
    $ModPath = "$env:TAIWU_WORKSHOP_PATH\$FileId"
} else {
    $ModPath = "$env:TAIWU_PATH\Mod\$ModName"
}

New-Item $ModPath -ItemType "directory"
New-Item "$ModPath\Plugins" -ItemType "directory"

$ConfigLuaPath = "$TargetDir"+"Config.lua"

Write-Output "ConfigLuaPath: $ConfigLuaPath"

& "../../../MynahModConfigGenerator/bin/Debug/net6.0/MynahModConfigGenerator.exe" $ConfigLuaPath "$TargetDir${ModName}Frontend.dll"

Copy-Item *.dll "$ModPath\Plugins" -Verbose
Copy-Item Config.lua $ModPath -Verbose

Write-Output "copy over"
