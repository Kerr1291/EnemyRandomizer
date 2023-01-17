@echo off
SET DLL_SOURCE="..\EnemyRandomizer\EnemyRandomizer.dll"
SET MOD_DEST="K:\SteamLibrary\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods\EnemyRandomizer"
echo Copying build from
echo %DLL_SOURCE%
echo to
echo %MOD_DEST%
copy %DLL_SOURCE% %MOD_DEST%
PAUSE