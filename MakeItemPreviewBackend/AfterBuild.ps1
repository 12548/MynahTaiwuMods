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

Copy-Item "$TargetDir*.dll" "$ModPath\Plugins" -Verbose

Write-Output "copy over"
