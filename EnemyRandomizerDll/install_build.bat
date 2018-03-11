@echo off
SET DLL_SOURCE="..\EnemyRandomizer\bin\Debug\EnemyRandomizer.dll"
SET RES_SOURCE="..\..\AssetBundleProject\Assets\AssetBundles\mainui"
SET MOD_DEST="C:\Program Files (x86)\Steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods"
echo Copying build from
echo %DLL_SOURCE%
echo to
echo %MOD_DEST%
copy %DLL_SOURCE% %MOD_DEST%
echo Copying reources from
echo %RES_SOURCE%
echo to
echo %MOD_DEST%
copy %RES_SOURCE% %MOD_DEST%
PAUSE