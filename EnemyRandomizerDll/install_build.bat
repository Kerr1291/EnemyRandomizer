@echo off
SET DLL_SOURCE="..\EnemyRandomizer\bin\Debug\EnemyRandomizer.dll"
SET MOD_DEST="K:\Games\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods"
echo Copying build from
echo %DLL_SOURCE%
echo to
echo %MOD_DEST%
copy %DLL_SOURCE% %MOD_DEST%
PAUSE