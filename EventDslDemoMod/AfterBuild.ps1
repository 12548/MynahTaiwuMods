Set-Location $PSScriptRoot

$ModName = "EventDslDemoMod"

$TargetDir = $args[0];
$FileId = $args[1]

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

$Dll = "$TargetDir${ModName}.dll"
$pdb = "$TargetDir${ModName}.pdb"

& "../../../MynahModConfigGenerator/bin/Debug/net6.0/MynahModConfigGenerator.exe" $ConfigLuaPath $Dll

Copy-Item $Dll "$ModPath\Plugins" -Verbose
Copy-Item $pdb "$ModPath\Plugins" -Verbose
Copy-Item Config.lua $ModPath -Verbose

Write-Output "copy over"
