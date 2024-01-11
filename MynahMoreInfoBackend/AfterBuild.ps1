Set-Location $PSScriptRoot

$ModName = "MynahMoreInfo"

$OutDir = $args[1];
$FileId = $args[0]

Write-Output "TargetDir: $OutDir"
$ModPath = "$env:TAIWU_PATH\Mod\$ModName"

New-Item $ModPath -ItemType "directory"
New-Item "$ModPath\Plugins" -ItemType "directory"

Copy-Item "$OutDir\${ModName}Backend.dll" "$ModPath\Plugins" -Verbose
Copy-Item "$OutDir\${ModName}Backend.pdb" "$ModPath\Plugins" -Verbose

Write-Output "copy over"
