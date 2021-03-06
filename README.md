# EnemyRandomizer

https://github.com/Kerr1291/EnemyRandomizer

This is a mod for [Hollow Knight](http://hollowknight.com/) that randomizes the Enemies.

This mod is still in the early stages of development! Use at your own risk!

## Other mods that pair well with this

RandomizerMod ( https://github.com/MyEyes/RandomizerMod ) - Randomizes Spells and Charms in the game

Debug Mod ( https://github.com/seanpr96/DebugMod ) - A life saver in case anything breaks, also fun to have around.

## Example of what this mod does

https://clips.twitch.tv/HeadstrongThoughtfulEmuSoonerLater

https://clips.twitch.tv/CrazyFamousSheepShadyLulu

https://clips.twitch.tv/SingleObliqueMomNotLikeThis

https://clips.twitch.tv/PreciousCleverPeanutGOWSkull

https://clips.twitch.tv/RichDeterminedCakeMingLee

Twitch clips courtesy of 56 from #modding.

## Release Installation

Copy the "EnemyRandomizer.dll" to Folder: `<Path-To-Hollow Knight>\Hollow Knight\hollow_knight_Data\Managed\Mods`

OR 

Go to https://github.com/Kerr1291/Crossroads and download and run the installer :)

## Dependencies

This mod depends on the Modding API by Seanpr and Firzen, which is a modified `Assembly-CSharp.dll`.
There is currently no public download link for the Modding API.
For now, check the pinned messages in the #modding channel in the Hollow Knight discord.

## Development setup

After installing the Modding API, open this solution in Visual Studio.
You will get many errors for missing assembly references.
Here's how to resolve them:

1. Right click the **EnemyRandomizer** project in the Solution Explorer.
2. Properties
3. Referenced Paths
4. Folder: `<Path-To-Hollow Knight>\Hollow Knight\hollow_knight_Data\Managed\`
5. Add Folder

Now if you open up EnemyRandomizer.cs, you should see no errors.

Note: install_built.bat is currently set to run as a post-build step. Open the file and configure "MOD_DEST" to automatically copy in the dll to your game's mod path after building.

### Debugging

Printf-style debugging is done through calls to `Modding.ModHooks.ModLog()`,
which outputs text to `%APPDATA%\..\LocalLow\Team Cherry\Hollow Knight\ModLog.txt`.

There is currently no easy way to use the Visual Studio debugger while Hollow Knight is running.
Please let someone know if you manage to get this working, and we'll update these instructions.

#### Thanks

Special thanks to 56 ( https://www.twitch.tv/5fiftysix6 ) for doing a TON of play testing and bug reporting for me. The help has been huge and really accelerated the development.

Big thanks to the RandomizerMod ( https://github.com/MyEyes/RandomizerMod ) which the code of which I used as a starting point/inspiration to make this mod.
Additional thanks to everyone at #modding in hollow knight discord for their hard work. The modding API is fantastic.
