Set-Location $PSScriptRoot

& "../../../MynahModConfigGenerator/bin/Debug/net6.0/MynahModConfigGenerator.exe" Config.lua MynahMoreInfo.dll

Copy-Item *.dll "C:\SteamLibrary\steamapps\common\The Scroll Of Taiwu\Mod\MynahMoreInfo\Plugins" -Verbose
Copy-Item Config.lua "C:\SteamLibrary\steamapps\common\The Scroll Of Taiwu\Mod\MynahMoreInfo" -Verbose

Write-Output "copy over"
