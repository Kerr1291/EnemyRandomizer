<# : install_mod.bat
:: Base script taken from:
:: https://stackoverflow.com/a/15885133/1683264
::
:: Helper script to make "installing" mods easier for users
:: 
:: To have the script use its current location to copy the files from use the following line:
:: $current = $pwd
:: Otherwise, change $current to whatever path you want
::
:: Feel free to change this to work with other mods
::$modname = "EnemyRandomizer.dll" <-mod dll
::$modres = "mainui" <-mod resources, can be file or folder of files
::$modpath = "\hollow_knight_Data\Managed\Mods" <-where to copy the mod dll
::$modrespath = "\hollow_knight_Data\Managed\Mods" <-where to copy the mod's resources
::
:: Hope this helps...
:: -Kerr1291

@echo off
echo Please navigate to your game folder and select "hollow_knight.exe"
setlocal

for /f "delims=" %%I in ('powershell -noprofile "iex (${%~f0} | out-string)"') do (
    @echo %%I
)
pause
goto :EOF

: end Batch portion / begin PowerShell hybrid chimera #>

Add-Type -AssemblyName System.Windows.Forms
$f = new-object Windows.Forms.OpenFileDialog
$f.InitialDirectory = pwd
$f.AutoUpgradeEnabled = $true
$f.Title = "Select your hollow_knight.exe"
$f.Filter = "hollow_knight.exe)|*.exe|All Files (*.*)|*.*"
$f.ShowHelp = $true
$f.Multiselect = $false
[void]$f.ShowDialog()
if ($f.Multiselect) { $f.FileNames } else { $f.FileName }
if($f.FileName)
{
$current = $pwd
$modname = "EnemyRandomizer.dll"
$modres = "mainui"
$modpath = "\hollow_knight_Data\Managed\Mods"
$modrespath = "\hollow_knight_Data\Managed\Mods"
$finalmodpath = (Join-Path (Split-Path -parent $f.FileName) $modpath)
$finalrespath = (Join-Path (Split-Path -parent $f.FileName) $modrespath)
If(!(test-path $finalmodpath))
{
  "Creating mod directory"
      New-Item -ItemType Directory -Force -Path $finalmodpath
}
else
{
  "Mod directory exists, no need to create"
}
If(!(test-path $finalrespath))
{
  "Creating mod resource directory"
      New-Item -ItemType Directory -Force -Path $finalrespath
}
else
{
  "Mod resource directory exists, no need to create"
}
Copy-Item -Path (Join-Path $current $modname)  -Destination $finalmodpath
Copy-Item -Path (Join-Path $current $modres)  -Destination $finalrespath -Force -Recurse
"Copied EnemyRandomizer into mod directory."
}