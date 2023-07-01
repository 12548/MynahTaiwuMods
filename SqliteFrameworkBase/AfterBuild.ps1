Set-Location $PSScriptRoot

$ModName = "SqliteFramework"

$TargetDir = "";
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

$Dll = "$TargetDir${ModName}Base.dll"
$Pdb = "$TargetDir${ModName}Base.pdb"

Copy-Item $Dll "$ModPath\Plugins" -Verbose
Copy-Item $Pdb "$ModPath\Plugins" -Verbose

Write-Output "copy over"
