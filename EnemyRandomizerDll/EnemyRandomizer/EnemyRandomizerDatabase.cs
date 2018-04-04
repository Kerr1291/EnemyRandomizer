using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using nv;


/*
 * [DONE]DONT OPTIMIZE SKIP on _Scenery\Ruins Flying Sentry
 * 
 * 
 * 
 *
				if (GameManager.instance.GetCurrentMapZone() != "COLOSSEUM")
 *
 * Movement Components
 * ----
 * Walker
 * 
 * 
 * 
 * Enemy FSMs to convert to componets:
 * ----
 * Mender Bug Ctrl  --- edit Sign Broken?  state and  PlayerDataBoolTest (boolName) = menderSignBroken  && edit  Chance  && RandomInt/IntCompare
 * Mawlek Control
 * Zombie Guard
 * Centipede
 * Centipede Hatcher
 * Hopper - (Hopper & Giant Hopper)
 * "Control" - Hornet Boss 2  (also want Needle and Needle Tink)
 * Slash Spider
 * "Control" - Boss Control\Radiance
 * Mossy Control -- moss charger
 * "Control" - Grey Prince
 * "Control" -- Hornet Boss 1
 * Moss Knight Control
 * Mantis Lord
 * Shroom Turret
 * Mush Roller
 * Shroom Brawler  (wake up with WAKE on state Sleep)
 * "Movement" -- Ghost Warrior Hu  && FSM:Set Ghost PD Int
 * Mantis - Mantis Traitor Lord   (maybe investigate this Battle Scene\Wave 3\Mantis Traitor Lord\Above Range)
 * "Attack" -- on garden zombie
 * Plant Trap Control
 * Crazy Hopper
 * Jellyfish
 * Jellyfish Baby
 * Mega Jellyfish
 * "Control" -- Grimm Boss
 * constrain_x - grimm  
 * Constrain Y - grimm
 * Hive Zombie
 * Big Bee
 * Bee
 * Bee Stinger
 * "Control" - Hive Knight
 * Grub Mimic
 * Beam Miner
 * Roller
 * Crystal Flyer
 * Laser Bug
 * Mines Crawler
 * Miner FX
 * Big Buzzer
 * Electric Mage
 * Zombie Miner
 * fat fly bounce -- Mega Fat Bee
 * Mozzie -- mosquito
 * Zombie Leap
 * Mage Lord 2
 * Mage Lord
 * Zombie Swipe -- zombie runner
 * Mage
 * Mage Knight
 * Blob -- mage blob
 * Control -- "mage balloon"
 * Black Knight
 * ZombieShieldControl -- on Great Shield Zombie
 * Ruins Sentry
 * "Control" -- on Jar Collector
 * Coward Swipe -- Royal Zombie Coward
 * Dung Defender
 * "Attack" -- used by Flip Hopper * 
 * Flukeman
 * Fluke Fly
 * Fluke Mother
 * Ruins Sentry Fat
 * Flying Sentry Javelin
 * Ceiling Dropper
 * Flying Sentry Nail -- Ruins Flying Sentry
 * Inflater
 * Guard  -- white palace
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * Enemy-Related FSM Parts to convert to components:
 * ----
 * Destroy Self -- Battle Scene\Jar Collector\Slam Effect
 * Death -- on Jar Collector
 * Stun Control -- on Jar Collector
 * Damage Control -- on Jar Collector
 * Phase Control -- on Jar Collector
 * Hit Launch -- is on fluke fly
 * "FSM" -- from fluke fly -- plays an audio clip
 * Drowner -- used by flukeman
 * "FSM" -- Alert Range object -- used by many things
 * "FSM" -- Hero Range object -- used by flip hopper
 * Stun -- used by bosses
 * hp_scaler -- used by bosses (dung, jar)
 * "FSM" -- Dung Defender\Splash Out && Splash Out Erupt && Burst Effect
 * Splash Out Control -- used by dung defender
 * Deactivate -- used by dung defender "Slam Effect"
 * Corpse -- used by many things
 * "FSM" -- Evade Check -- used by dung defender and probably others
 * flyer_receive_direction_msg - on buzzer in tutorial
 * chaser -- on buzzer in tutorial
 * nail_clash_tink -- on Great Shield Zombie (2)\Slash1
 * Shadow Dash -- Great Shield Zombie (2)\Hero Blocker
 * Boss Deactivate
 * disable_special_death -- on a Ruins Flying Sentry??
 * Go Upper -- on a Ruins Flying Sentry??
 * Lost Hero Check -- on a Ruins Flying Sentry???
 * "FSM" -- on Black Knights, does "SetHP"
 * Corpse Black Knight
 * follow_hero -- used by mage knight
 * Summon Orbs -- used by mage lord
 * Tele Out -- mage lord
 * deactivate -- Mage Lord\Quake Hit
 * Destroy If Defeated -- mage lord
 * "FSM" -- on Mosquito, used to aim/rotate?
 * damages_enemy -- on plant traps
 * 
 * 
 * Dreamnail Reject -- hollow knight
 * "Control" -- hollow knight boss control -- Boss Control\Hollow Knight Boss
 * 
 * 
 * 
 * 
 * Non-Enemy FSMs to convert to components:
 * ----
 * Battle Control -- load waves 
 * "Control" -- on egg sac, controls the spitting out of the pick-up-able item
 * "Control" -- on Battle Scene in Jar Collector fight
 * "Battle Control" -- Ruins2_03_boss [Build index: 115]
 * BG Control - part of ^
 * deparent_and_follow -- on orb spinner
 * Lift Control
 * Shiny Control --- VERY IMPORTANT-- DO THIS  (part of shiny item)
 * Generate Wave -- also ^
 * Shiny Control -- Battle Scene v2\Completed\Shiny Item <-- LOOK AT THIS
 * 
 * 
 * 
 * Other things to export from scenes 
 * ----
 * Egg Sac -- spawns rancid eggs
 * Jelly Egg Bomb -- explosive fog canyon bubbles
 * Jelly Egg Empty -- non explosive fog canon bubbles
 * Gorgeous Husk\Shine -- shiney golden husk effect
 * Laser Turret Mega (1) -- from rematch scene, maybe remove fsm - Laser Bug Mega
 * beam stuff from  Mines_18_boss [Build index: 261]
 * Laser Turret from Printing full hierarchy for scene: Mines_17 [Build index: 259]
 * Ring Holder -- special attack for ghost warrior Hu
 * Shot Mantis Lord -- mantis lord attacks
 * Cave Spikes (13) -- a cave spike? i think
 * Big Centipede (3) -- giant deepnest cave centipede
 * Heart Piece -- brooding mawlek scene -- with FSM Heart Container Control
 * _Enemies\Fly Spawn  -- the fly spawns used by gruz mother
 * Worm -- needs to be added to effect/"enemies" list
 * 
 * Hatcher Cage (2)  -- used by fluke mother spawns
 * Orb Spinner -- used by mage lord
 * 
 * 
 * 
 * FSM replacements that need to happen using existing Components:
 * -----
 * Crawler FSM ---> Crawler component
 * Corpse FSM --> ???? Corpse component
 * 
 * 
 * 
 * 
 * 
 * Components to investigate:
 * ----
 * LineOfSightDetector -- on buzzer in tutorial?
 * AlertRange -- on Royal Zombie Coward?
 * DeactivateIfPlayerdataTrue
 * 
 * 
 * 
 * 
 * CREATE A REFLECTION GETTER FOR 
	public void SetBattleScene(GameObject newBattleScene)
	{
		this.battleScene = newBattleScene;
	}
    AND SET THIS ON REPLACEMENTS INSTEAD OF USING REPLACEMENT PAIRS
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * Knight is slightly shorter than 5 units, 3 or 4?
 * 
 * 
 * [DONE]Flamebearer Small/Med -- Try sending "START" event on wake
 * [TEST]Giant Hopper -- Colosseum version? dies when spawned? seems to project itself to 0,0,0???
 * [DONE]Bursting Zombie -- needs to drop geo
 * [TEST]Mega Fat Bee -- needs a modification to it's position/fly in animation/FSM
 * [DONE]Mantis Heavy  -- spawns outside of scene at 0,0,0
 * [DONE]Mage Knight -- spawned at 0,0,0
 * [TEST]Electric Mage - spawned at 0,0,0  -- force instantiate
 * [TEST]Lobster -- spawned in ground
 * [TEST]Mender Bug - needs an awake message of some sort -- modified the FSM
 * [NEED TO SAVE OFF EFFECT PREFAB]Giant Fly - needs to spawn as a ground enemy & needs a fix so it spawns enemies && DONT replace it
 * [TEST]Mawlek Body - needs the fix so it doesn't check to see if the player has killed one already
 * [TEST]Blocker - floating just a bit above ground, about 1 unit / never spawns enemies
 * [TEST]Hatcher - doesn't spawn enemies
 * [???]Hatcher Baby - maybe don't rando replace???
 * [TEST]Zombie Myla - spawned at zero and also has some persistant thing that needs to be fixed
 * [NEEDS REMOVE?]Ruins Sentry Fat - spawned at zero, but others were fine?
 * [TEMP REMOVED]Mage Lord Phase2 - spawned and then teleported far away, fix his teleport destination?/Spawning him twice is broken probably a persistant bool item
 * [TEST & NEEDS TO SAVE OFF EFFECT PREFAB]Mage Lord - same problem, NEEDS TO COPY OUT ORB SPINNER
 * [TEST]Black Knight - needs wake event
 * [TEST]Infected Knight - needs wake event
 * [TEST]Jar Collector - needs wake event
 * [TEST]Hornet Boss - needs wake event, NEEDS NEEDLE AND NEEDLE TINK EFFECT OBJECTS
 * [TEST]Moss Charger - index 88, needs to be looked at for nullref  (  EnemyRandomizerMod.EnemyRandomizerLogic.PositionRandomizedEnemy  threw a nullref  )
 * [TEST]Moss Knight - spawns a bit above the ground by around 1 unit
 * [ADDED GEO]Lazy Flyer Enemy - lake of unn fliers, either add geo or remove them from the list
 * [TEMP REMOVED]Mega Moss Charger - needs something fixed in the fsm so it works where it spawns
 * Ghost Warrior No Eyes - works, but spawns offset way to the left and up, needs lots of space
 * [TEST]Mushroom Turret - spawns a bit inside the wall, move out by about 0.5 units
 * [TEST]Mushroom Brawler - needs a wake area component
 * [TEST]Mantis Flyer Child - when spawning, use the crawler placement logic, when replacing, use the flyer
 * [TEST SCALING]Ghost Warrior Slug - works, just needs lots of space, or scaling
 * [TEST]Ghost Warrior Hu - needs space and room for his attacks, attacks don't work, need something to fix
 * [TEST]Garden Zombie - raycast that it does to wake up? seems to be broken. either needs wake event or a "fixed" fsm
 * [NEED MORE INFO]Mantis Traitor Lord - needs a fix, spawns about 20 units above his placement point
 * [NEED MORE INFO]Mega Jellyfish - spawns somewhere? and dies
 * [TEST - NEEDS MORE INFO]]Mines Crawler - upside down and about 1 unit far from the wall
 * [ASSEST NEED TO BE SAVED IN THE ASSET LIST]Mega Zombie Beam Miner - needs to have a fix in for his lasers, probably temporarily remove since version 2 works fine
   [TEST ADDED SPAWN TO OLD WAY](added)Baby Centipede - nullref issue (need to instantiate and keep a copy of this like the old way)
 * [TEST]Centipede Hatcher - doesn't spawn anything
 * [NEEDS TEST TO RULE OUT MEMORY LEAK]Mimic Spider - spawn position seems locked to a fixed point? nosk seems to jump up into the roof and get stuck?
 * Ghost Warrior Galien - needs lots of room and a fix for spawning his attack
 * Ghost Warrior Markoth - needs fix for his attack
 * [NEEDS TESTING]Abyss Crawler - spawning upside down?
 * [TEST]Shade Sibling - nullref issue, needs to instantiate like old way
 * [TEMP REMOVED]Dung Defender - needs to spawn in the ground? and needs a wake event and lots of space -- just remove for now
 * [TEST]Fluke Mother - needs fix for spawning enemies
 * [TEST]"Enemy" - needs to be move down 1/2 unit
 * [TEST]Zombie Hive - needs fix for spawning adds
 * [TEST]Hive Knight - needs wake event
 * [TEST?]Dream mage lord - fix to not dream spawn you
 * 
 * 
 * 
 * 
 * Other Notes:
 * 
 * Blow Fly - fat kingdom's edge fly
 * Zombie Runner Sp - on death randomizes into another enemy, kindof awesome
 * Zombie Hornhead Sp - on death randomizes into another enemy, kindof awesome
 * 
 * [DONE]Increase the raycast down length for positioning ground enemies
 * 
 * [REMOVED]Sending Force Kill didn't "properly" kill mage lord phase 2?
 * 
 * [TEST]Mage Knight/Mage seem to probably need my area wake fixes
 * 
 * [TEST]Rando enemies need to have their persistant bool things removed, else they will kill themselves when spawned
 * [TEST]Lnacer still does not despawn on death, careful of where we place her
 * 
 * 
 * Don't spawn:
 * 
 * Buzzer Col -- duplicate of Buzzer and does not drop geo
 * Maybe: Giant Buzzer Col - does not spawn adds
 * [DONE]False Knight New -- very broken
 * [DONE]Great Shield Zombie bottom - duplicate
 * [DONE]Ruins Sentry Fat - remove duplicates
 * [DONE]Roller R - very broken
 * [DONE]Spitter R - very broken
 * [DONE]Buzzer R -^^
 * [DONE]Shell - broken
 * [TODO: CHECK THEIR FSM TO SEE HOW IT'S DIFFERENT FROM UP]Plant Turret Right - the enemy ends up right and up by about 5? units. also duplicate of Plant Turret, so try removing
 * [DONE]Giant Buzzer - duplicate of the Buazzer Col and spawns holding zote, maybe keep buzzer col. for certain, don't rando replace this one
 * [DONE]Moss Knight B - duplicate of Moss Knight and doesn't spawn properly
 * [DONE]Moss Knight C - ^^
 * [DONE]Cap Hit - don't load, not an enemy
 * [DONE]Jellyfish Baby Inert - duplicate
 * [DONE]Hiveling Spawner - remove
 * 
 * */

//disable the unreachable code detected warning for this file
#pragma warning disable 0162

namespace EnemyRandomizerMod
{
    //TODO: refactor the randomizer to move all this into XML
    public class EnemyRandomizerDatabase
    {
        public static EnemyRandomizerDatabase Instance { get; private set; }

        CommunicationNode comms;

        public List<GameObject> loadedEnemyPrefabs = new List<GameObject>();
        public List<string> loadedEnemyPrefabNames = new List<string>();

        //a user-controllable list of enemies that may be placed when randomizing
        public List<bool> enabledEnemyPrefabs = new List<bool>();

        //hold references to certain effects used by enemies (like the beam parts used by crystal guardian)
        public Dictionary<string, GameObject> loadedEffectPrefabs = new Dictionary<string, GameObject>();

        public Dictionary<string, GameObject> levelParts = new Dictionary<string, GameObject>();

        public EnemyRandomizerDatabase()
        {
        }

        public void Setup()
        {
            Instance = this;
            comms = new CommunicationNode();
            comms.EnableNode( this );
        }

        public void Unload()
        {
            comms.DisableNode();
            Instance = null;
        }

        //enable this to only load scenes from the "testTypeScenes" list for faster debugging
        public const bool USE_TEST_SCENES = true;

        public static List<int> EnemyTypeScenes {
            get {
                if( USE_TEST_SCENES )
                    return testTypeScenes;

                else
                    return scenesToLoad;
            }
        }

        static List<int> testTypeScenes = new List<int>()
        {
            132,
            133
        };

        //tip: the scene we end the load on will determine the menu music
        public static List<int> scenesToLoad = new List<int>()
        {
              6
             ,27
             ,32
             ,33
             ,34
             ,37
             ,39
             ,40
             ,45
             ,48
             ,49
             ,50
             ,54
             ,57
             ,58
             ,59
             ,73
             ,76
             ,81
             ,83
             ,84
             ,88
             ,90
             ,92
             ,94
             ,102
             ,114
             ,115
             ,126
             ,128
             ,133
             ,138
             ,139
             ,140
             ,141
             ,147
             ,148
             ,149
             ,151
             ,154
             ,156
             ,159
             ,161
             ,167
             ,168
             ,169
             ,171
             ,174
             ,176
             ,178
             ,180
             ,192
             ,194
             ,197
             ,200
             ,203
             ,209
             ,219
             ,221
             ,228
             ,229
             ,238
             ,245
             ,249
             ,251
             ,252
             ,256
             ,258
             ,261
             ,264
             ,271
             ,278
             ,279
             ,280
             ,286
             ,290
             ,291
             ,292
             ,297
             ,298
             ,306
             ,314
             ,332
             ,334
             ,344
             ,349
             ,350
             ,355
             ,358
             ,361
             ,365
             ,367
             ,384
             ,386
             ,389
             ,392
             ,393
             ,395
             ,396
             ,398
             ,409 //hollow knight room
             ,407
             ,399
             ,397
             ,366 //white palace
             ,383 //path of pain
        };

        public List<int> scenesLoaded = new List<int>();

        /*
         * 
         * 
         * Other things to export from scenes 
         * ----
         * Egg Sac -- spawns rancid eggs
         * Jelly Egg Bomb -- explosive fog canyon bubbles
         * Jelly Egg Empty -- non explosive fog canon bubbles
         * Gorgeous Husk\Shine -- shiney golden husk effect
         * Laser Turret Mega (1) -- from rematch scene, maybe remove fsm - Laser Bug Mega
         * beam stuff from  Mines_18_boss [Build index: 261]
         * Laser Turret from Printing full hierarchy for scene: Mines_17 [Build index: 259]
         * Ring Holder -- special attack for ghost warrior Hu
         * Shot Mantis Lord -- mantis lord attacks
         * Cave Spikes (13) -- a cave spike? i think
         * Big Centipede (3) -- giant deepnest cave centipede
         * Heart Piece -- brooding mawlek scene -- with FSM Heart Container Control
         * _Enemies\Fly Spawn  -- the fly spawns used by gruz mother
         * Worm -- needs to be added to effect/"enemies" list
         * 
         * Hatcher Cage (2)  -- used by fluke mother spawns
         * Orb Spinner -- used by mage lord
         * 
         * 
         */




        //effects used by enemies, like crystal guardian, or just things we can use for fun
        public static List<string> effectObjectNames = new List<string>()
        {
            "Beam Impact",
            "Beam Ball",
            "Beam Point L",
            "Beam Point R",
            "Beam",
            "Crystal Rain",
            "Fly Spawn",
            "Egg Sac",
            "Jelly Egg Bomb",
            "Jelly Egg Empty",
            "Shine",
            "Laser Turret Mega",
            "Laser Turret",
            "Ring Holder",
            "Shot Mantis Lord",
            "Cave Spikes",
            "Big Centipede",
            "Heart Piece",
            "Fly Spawn",
            "Worm",
            "Hatcher Cage",
            "Orb Spinner",
            "Health Cocoon",
            "Geo Rock",
            "Spawn Jar(Clone)",
            "Spawn Jar",
            "Teleplanes",
            "Grimm Bats",
            "Grimm Spike Holder",
            "NONE"
        };

        //TODO: add this to the randomizing and shuffling
        public static List<string> otherRandomObjectNames = new List<string>()
        {
            "Egg Sac",
            "Jelly Egg Bomb",
            "Jelly Egg Empty",
            "Laser Turret Mega",
            "Laser Turret",
            "Cave Spikes",
            "Big Centipede",
            "Worm",
            "Health Cocoon",
            "Geo Rock",
            "NONE"
        };


        //misc objects we can mess with
        //TODO: place health cocoons around places
        //for most of these objects, set the "PersistentBoolItem" component to = that of the original
        public static List<string> miscObjectNames = new List<string>()
        {
            //"Cocoon Plant 1", //??? also part of health cocoon??? -- OH this is just the butterfly looking plants
            "Health Cocoon", //scene 7, default rotation is hanging from roof
            //check if it has the "HealthCocoon" component
            //does have PersistentBoolItem
        
            //"Chest" //scene 7, by default contains a charm -- check if it has a "Chest Control" FSM
            //mess with this after the content patch, right now it would use FSMs to modify... too much work :P

            "Geo Rock"//scene 7 -- basic crossroads geo rock
            //Example: "Geo Rock 3" is the rock just to the left of start
            //check if it has "Geo Rock" FSM
            //does not have? PersistentBoolItem
        };

        //==============================================================================================================
        //==============================================================================================================
        //==============================================================================================================


        public static List<string> flyerEnemyTypeNames = new List<string>()
        {
            "Mosquito",
            "Blobble",
            "Giant Buzzer Col",
            "Buzzer Col",
            "Super Spitter Col",
            "Mega Fat Bee",
            "Mage",
            "Giant Fly Col",
            "Mage Balloon",
            "Electric Mage",
            "Angry Buzzer",
            "Spitter",
            "Buzzer",
            "Fly",
            "Bursting Bouncer",
            "Hatcher",
            "Ruins Flying Sentry",
            "Ruins Flying SentryB",
            "Ruins Flying Sentry Javelin",
            "Mage Lord Phase2",
            "Mage Lord",
            "Acid Flyer",
            "Fat Fly",
            "Giant Buzzer",
            "Lazy Flyer Enemy",
            "Ghost Warrior No Eyes",
            "Fungus Flyer",
            "Fungoon Baby",
            "Ghost Warrior Hu",
            "Jellyfish Baby",
            "Jellyfish",
            "Mantis Heavy Flyer",
            "Moss Flyer",
            "Ghost Warrior Marmu",
            "Mega Jellyfish",
            "Ghost Warrior Slug",
            "Ghost Warrior Xero",
            "Centipede Hatcher",
            "Spider Flyer",
            "Ghost Warrior Galien",
            "Blow Fly",
            "Bee Hatchling Ambient",
            "Super Spitter",
            "Ghost Warrior Markoth",
            "Shade Sibling",
            "Parasite Balloon",
            "Inflater",
            "Fluke Fly",
            "Fluke Mother",
            "White Palace Fly",
            "Bee Stinger",
            "Big Bee",
            "Mosquito",
            "Zote Balloon",
            "Zoteling",
            "Dream Mage Lord",
            "Dream Mage Lord Phase2",
            "Colosseum_Flying_Sentry",
            "Colosseum_Armoured_Mosquito",
            "Radiance"
        };

        public static List<string> groundEnemyTypeNames = new List<string>()
        {
            "Colosseum_Miner",
            "Colosseum_Armoured_Roller",
            "Colosseum_Shield_Zombie",
            "Colosseum_Worm",
            "Giant Fly",
            "Lancer",
            "Lobster",
            "Mage Blob",
            "Mage Knight",
            "Zombie Runner",
            "Bursting Zombie",
            "Mender Bug",
            "Crawler",
            "Zombie Hornhead",
            "Zombie Barger",
            "Spitting Zombie",
            "Mawlek Body",
            "Prayer Slug",
            "Blocker",
            "Zombie Shield",
            "Zombie Leaper",
            "Zombie Guard",
            "Zombie Myla",
            "Roller",
            "Great Shield Zombie",
            "Royal Zombie Coward",
            "Royal Zombie",
            "Gorgeous Husk",
            "Royal Zombie Fat",
            "Ruins Sentry",
            "Ruins Sentry FatB",
            "Ruins SentryB",
            "Ruins Sentry Fat B",
            "Ruins Sentry Fat",
            "Great Shield Zombie bottom",
            "Black Knight",
            "Jar Collector",
            "Pigeon",
            "Mossman_Runner",
            "Moss Walker",
            "Mossman_Shaker",
            "Hornet Boss",
            "Moss Charger",
            "Shell",
            "Moss Knight",
            "Grass Hopper",
            "Mega Moss Charger",
            "Moss Knight C",
            "Moss Knight B",
            "Zombie Fungus",
            "Fung Crawler",
            "Mushroom Baby",
            "Mushroom Brawler",
            "Mushroom Roller",
            "Zombie Fungus A",
            "Mantis",
            "Mantis Flyer Child",
            "Jellyfish Baby",
            "Mantis Heavy",
            "Garden Zombie",
            "Mantis Traitor Lord",
            "Moss Knight Fat",
            "Mantis Heavy Spawn",
            "Grave Zombie",
            "Zombie Miner",
            "Crystal Crawler",
            "Crystal Flyer",
            "Grub Mimic",
            "Mega Zombie Beam Miner",
            "Zombie Beam Miner",
            "Zombie Beam Miner Rematch",
            "Baby Centipede",
            "Zombie Runner Sp",
            "Zombie Spider 1",
            "Zombie Hornhead Sp",
            "Zombie Spider 2",
            "Mimic Spider",
            "Slash Spider",
            "Tiny Spider",
            "Giant Hopper",
            "Hopper",
            "Abyss Crawler",
            "Lesser Mawlek",
            "Infected Knight",
            "Flip Hopper",
            "Flukeman",
            "Dung Defender",
            "Royal Gaurd",
            "Enemy",
            "Zombie Hive",
            "Hive Knight",
            "Grimm Boss",
            "Nightmare Grimm Boss",
            "False Knight Dream",
            "Lost Kin",
            "White Defender",
            "Grey Prince",
            "Hollow Knight Boss"
        };

        public static List<string> smallEnemyTypeNames = new List<string>()
        {
            "Mosquito",
            "Buzzer Col",
            "Blobble",
            "Colosseum_Armoured_Roller",
            "Colosseum_Armoured_Mosquito",
            "Colosseum_Flying_Sentry",
            "Colosseum_Shield_Zombie",
            "Super Spitter Col",
            "Colosseum_Worm",
            "Colosseum_Miner",
            "Mega Fat Bee",
            "Mage Knight",
            "Mage Blob",
            "Mage Balloon",
            "Zombie Runner",
            "Giant Fly Col",
            "Angry Buzzer",
            "Bursting Zombie",
            "Mender Bug",
            "Climber",
            "Spitter",
            "Buzzer",
            "Zombie Hornhead",
            "Fly",
            "Zombie Barger",
            "Spitting Zombie",
            "Climber",
            "Bursting Bouncer",
            "Prayer Slug",
            "Zombie Shield",
            "Mawlek Body",
            "Angry Buzzer",
            "Hatcher",
            "Zombie Leaper",
            "Zombie Myla",
            "Roller",
            "Great Shield Zombie",
            "Royal Zombie Coward",
            "Royal Zombie",
            "Gorgeous Husk",
            "Royal Zombie Fat",
            "Ruins Sentry",
            "Ceiling Dropper",
            "Ceiling Dropper Col",
            "Ruins Flying Sentry",
            "Ruins Sentry FatB",
            "Ruins SentryB",
            "Ruins Flying SentryB",
            "Ruins Sentry Fat B",
            "Ruins Flying Sentry Javelin",
            "Ruins Sentry Fat",
            "Black Knight",
            "Ceiling Dropper", //
            "Plant Trap", //
            "Plant Turret", //
            "Laser Turret Frames", //
            "Mawlek Turret Ceiling", //
            "Mawlek Turret", //
            "Worm", //
            "Mushroom Turret",
            "Mines Crawler", //
            "Abyss Crawler", //
            "Climber", //
            "Crystallised Lazer Bug",
            "Spider Mini",
            "Jar Collector",
            "Pigeon",
            "Mossman_Runner",
            "Moss Walker",
            "Mossman_Shaker",
            "Hornet Boss",
            "Acid Flyer",
            "Plant Turret Right",
            "Fat Fly",
            "Giant Buzzer",
            "Grass Hopper",
            "Lazy Flyer Enemy",
            "Fungus Flyer",
            "Fungoon Baby",
            "Zombie Fungus",
            "Fung Crawler",
            "Mushroom Baby",
            "Mushroom Brawler",
            "Mushroom Roller",
            "Zombie Fungus A",
            "Mantis",
            "Jellyfish",
            "Mantis Heavy Flyer",
            "Moss Flyer",
            "Mantis Heavy",
            "Garden Zombie",
            "Mantis Traitor Lord",
            "Moss Knight Fat",
            "Mantis Heavy Spawn",
            "Grave Zombie",
            "Zombie Miner",
            "Crystal Crawler",
            "Crystal Flyer",
            "Grub Mimic",
            "Mega Zombie Beam Miner",
            "Zombie Beam Miner",
            "Zombie Beam Miner Rematch",
            "Baby Centipede",
            "Zombie Runner Sp",
            "Zombie Spider 1",
            "Zombie Hornhead Sp",
            "Zombie Spider 2",
            "Slash Spider",
            "Tiny Spider",
            "Spider Flyer",
            "Bee Hatchling Ambient",
            "Super Spitter",
            "Giant Hopper",
            "Hopper",
            "Shade Sibling",
            "Lesser Mawlek",
            "Parasite Balloon",
            "Flip Hopper",
            "Inflater",
            "Flukeman",
            "Fluke Fly",
            "White Palace Fly",
            "Royal Gaurd",
            "Enemy",
            "Giant Buzzer Col",
            "Zombie Hive",
            "Bee Stinger",
            "Big Bee",
            "Zote Balloon",
            "Zoteling",
            "Crawler"
        };

        public static List<string> mediumEnemyTypeNames = new List<string>()
        {
            "Mosquito",
            "Colosseum_Worm",
            "Colosseum_Flying_Sentry",
            "Colosseum_Miner",
            "Colosseum_Shield_Zombie",
            "Mega Fat Bee",
            "Mage Knight",
            "Mage",
            "Electric Mage",
            "Angry Buzzer",
            "Blocker",
            "Mawlek Body",
            "Great Shield Zombie",
            "Gorgeous Husk",
            "Giant Fly Col",
            "Royal Zombie Fat",
            "Ruins Sentry",
            "Ruins Flying Sentry",
            "Ruins Sentry FatB",
            "Ruins SentryB",
            "Ruins Flying SentryB",
            "Ruins Sentry Fat B",
            "Ruins Flying Sentry Javelin",
            "Ruins Sentry Fat",
            "Black Knight",
            "Jar Collector",
            "Hornet Boss",
            "Shell",
            "Fat Fly",
            "Giant Buzzer",
            "Lazy Flyer Enemy",
            "Fungus Flyer",
            "Mushroom Brawler",
            "Mushroom Roller",
            "Mantis",
            "Mantis Flyer Child",
            "Jellyfish",
            "Mantis Heavy Flyer",
            "Mantis Heavy",
            "Mantis Traitor Lord",
            "Moss Knight Fat",
            "Mantis Heavy Spawn",
            "Grave Zombie",
            "Crystal Crawler",
            "Grub Mimic",
            "Mega Zombie Beam Miner",
            "Zombie Beam Miner",
            "Zombie Beam Miner Rematch",
            "Centipede Hatcher",
            "Slash Spider",
            "Spider Flyer",
            "Giant Hopper",
            "Shade Sibling",
            "Lesser Mawlek",
            "Flip Hopper",
            "Inflater",
            "Flukeman",
            "Royal Gaurd",
            "Zombie Hive",
            "Bee Stinger",
            "Big Bee"
        };

        public static List<string> bigEnemyTypeNames = new List<string>()
        {
            "Mosquito",
            "Colosseum_Worm",
            "Giant Buzzer Col",
            "Mega Fat Bee",
            "Lancer",
            "Lobster",
            "Mage Knight",
            "Giant Fly",
            "Mawlek Body",
            "Zombie Guard",
            "Great Shield Zombie",
            "Gorgeous Husk",
            "Mage Lord Phase2",
            "Mage Lord",
            "Great Shield Zombie bottom",
            "Black Knight",
            "Jar Collector",
            "Moss Charger",
            "Fat Fly",
            "Giant Buzzer",
            "Moss Knight",
            "Lazy Flyer Enemy",
            "Mega Moss Charger",
            "Moss Knight C",
            "Giant Fly Col",
            "Moss Knight B",
            "Ghost Warrior No Eyes",
            "Fungus Flyer",
            "Mushroom Brawler",
            "Ghost Warrior Hu",
            "Jellyfish",
            "Mantis Traitor Lord",
            "Moss Knight Fat",
            "Ghost Warrior Marmu",
            "Mega Jellyfish",
            "Ghost Warrior Slug",
            "Ghost Warrior Xero",
            "Grave Zombie",
            "Centipede Hatcher",
            "Mimic Spider",
            "Ghost Warrior Galien",
            "Blow Fly",
            "Giant Hopper",
            "Ghost Warrior Markoth",
            "Shade Sibling",
            "Infected Knight",
            "Inflater",
            "Dung Defender",
            "Fluke Mother",
            "Royal Gaurd",
            "Zombie Hive",
            "Bee Stinger",
            "Big Bee",
            "Hive Knight",
            "Grimm Boss",
            "Nightmare Grimm Boss",
            "False Knight Dream",
            "Dream Mage Lord",
            "Dream Mage Lord Phase2",
            "Lost Kin",
            "White Defender",
            "Grey Prince",
            "Radiance",
            "Hollow Knight Boss"
        };

        //doubled the occurence of some types in here to even out the replacements
        public static List<string> wallEnemyTypeNames = new List<string>()
        {
            "Ceiling Dropper Col",//34
            "Ceiling Dropper", //??(forgot)
            "Plant Trap", //117
            "Plant Trap", //117
            "Plant Turret", //127
            "Plant Turret", //127
            "Laser Turret Frames", //234
            "Laser Turret Frames", //234
            "Mawlek Turret Ceiling", //321
            "Mawlek Turret", //321
            "Worm", //49
            "Worm", //49
            "Mushroom Turret",
            "Plant Turret Right",
            "Plant Turret Right",
            "Mushroom Turret"
        };


        public static List<string> crawlerEnemyTypeNames = new List<string>()
        {
            "Mines Crawler", //243
            "Abyss Crawler", //320
            "Climber", //40
            "Crystallised Lazer Bug", //243 (this was removed from the list for some reason???)
            "Spider Mini" //271
        };


        public static List<string> excludeFromBattleArenaZones = new List<string>()
        {
            "Mage",
            "Electric Mage",
            "Zombie Myla",
            "Mega Moss Charger",
            "White Palace Fly",
            "Laser Turret",
            "Laser Turret Mega",
            ////until mage enemies are fixed, do not spawn them in battle areas
            "Electric Mage",//35 //TODO: needs to be moved down? (by 20?) -- broken can't put most places
            "Mage",//35 //TODO: still broken
            "Mantis Traitor Lord",
            //"Flamebearer Spawn",
            //"Centipede Hatcher",
            //"Mossman_Shaker",
            //"Mage Blob",//35
            //"Mage Balloon", //102
            "Zombie Spider 1", //271
            "Zombie Spider 2", //271
            "Parasite Balloon",
            ////"Zombie Beam Miner Rematch", //241 should be fixed now
            ////"Mage Knight",//35 //he's fixed now
            "Lancer",//35
            "Lobster",//35
            //"Colosseum_Worm",
            "Worm",
            "Baby Centipede", //259
            "Zombie Spider 2",
            "Zombie Spider 1",
            "Mender Bug",
            "Abyss Crawler",
            "White Defender",
            "Grey Prince",
            "Radiance",
            "Pigeon",
            "Hollow Knight Boss",
            "Zote Boss"//33 (BOSS???)
        };


        public static List<string> excludeFromColosseum = new List<string>()
        {
            "Zombie Myla",
            "Mega Moss Charger",
            "White Palace Fly",
            "Laser Turret",
            "Laser Turret Mega",
            "Zombie Spider 1", //271
            "Zombie Spider 2", //271
            "Parasite Balloon",
            "Lancer",//35 //TODO: find a way to check if she is dead
            "Lobster",//35
            "Worm",
            "Baby Centipede", //259
            "Mender Bug",
            "Abyss Crawler",
            "White Defender",
            "Grey Prince",
            "Radiance",
            "Mantis Traitor Lord",
            "Hollow Knight Boss",
            "Pigeon",
            "Zote Boss"//33 (BOSS???) //TODO: find a way to check if zote is dead
        };

        /*
         * 
 * 
 * 
 * None of the colosseum enemies project themselves onto the ground properly?
 * 
 * Colosseum List: 
 * 
 * Colosseum_Armoured_Roller
 * Super Spitter Col
 * Colosseum_Worm
 * Hopper
 * Ceiling Dropper Col (now works properly)
 * Super Spitter
 * Mega Fat Bee
 * Mantis Heavy Flier
 * Mantis Heavy 
 * Colosseum Grass Hopper
 * Mawlek Col
 * Lesser Mawlek
 * Angry Buzzer
 * Lancer
 * Lobster
 * Mage Knight
 * Mage Blob
 * Mage
 * Mage Balloon
 * Electric Mage
         * 
         */
        public static List<string> colloseumEnemyTypes = new List<string>()
        {
            "Mosquito",
            "Giant Buzzer Col",
            "Colosseum_Miner",
            "Mega Fat Bee",
            "Lancer",
            "Lobster",
            "Mage",
            "Mage Balloon",
            "Electric Mage",

            "Mage Blob",
            "Mage Knight",
            "Giant Hopper",


            "Colosseum_Armoured_Roller",
            "Super Spitter Col",
            "Colosseum_Worm",
            "Ceiling Dropper Col",
            "Super Spitter",
            "Mantis Heavy Flier",
            "Mantis Heavy",
            "Colosseum Grass Hopper",
            "Mawlek Col",
            "Buzzer Col",
            "Lesser Mawlek",
            "Angry Buzzer",
            "Mage Knight",
            "Mage Blob",
            "Giant Fly Col",
            "Blobble",
            "Hopper",
            "Colosseum_Flying_Sentry",
            "Colosseum_Armoured_Mosquito",
            "Colosseum_Shield_Zombie",
            "Grub Mimic",
            "Blobble",
            "Bursting Bouncer",


            "Dung Defender"
        };


        public static List<string> hardEnemyTypeNames = new List<string>()
        {
            "Mage Knight",
            "Electric Mage",
            "Giant Buzzer Col",
            "Mega Fat Bee",
            "Giant Buzzer",
            "Lancer",
            "Lobster",
            "Mawlek Body",
            "Great Shield Zombie",
            "Gorgeous Husk",
            "Mage Lord Phase2",
            "Mage Lord",
            "Great Shield Zombie bottom",
            "Jar Collector",
            "Hornet Boss",
            "Moss Knight",
            "Mega Moss Charger",
            "Giant Fly Col",
            "Moss Knight C",
            "Moss Knight B",
            "Ghost Warrior No Eyes",
            "Mushroom Brawler",
            "Ghost Warrior Hu",
            "Big Bee",
            "Mantis Traitor Lord",
            "Moss Knight Fat",
            "Ghost Warrior Marmu",
            "Mega Jellyfish",
            "Ghost Warrior Slug",
            "Ghost Warrior Xero",
            "Mega Zombie Beam Miner",
            "Zombie Beam Miner Rematch",
            "Centipede Hatcher",
            "Mimic Spider",
            "Ghost Warrior Galien",
            "Ghost Warrior Markoth",
            "Infected Knight",
            "Fluke Mother",
            "Royal Gaurd",
            "Hive Knight",
            "Grimm Boss",
            "Nightmare Grimm Boss",
            "False Knight Dream",
            "Dream Mage Lord",
            "Dream Mage Lord Phase2",
            "Lost Kin",
            "White Defender",
            "Grey Prince",
            "Radiance",
            "Hollow Knight Boss"

            //"Centipede Hatcher",
            //"Gorgeous Husk", //82 (for fun)
            //"Electric Mage",//35
            //"Mage Knight",//35
            //"Giant Buzzer Col",//33
            //"Giant Fly Col",//34 
            //"Lancer",//35
            //"Lobster",//35
            //"Mawlek Body", //46 (mini boss)
            //"Moss Knight", //137            
            //"Mushroom Brawler", //
            //"Royal Gaurd", //358
            //"Mimic Spider", //269 (Nosk, BOSS)
            //"Zombie Beam Miner Rematch", //241
            //"Mantis Traitor Lord" //194 (BOSS)          needs lots of room or will fall through the floor  
        };

        //"Giant Fly",//(gruz mother?)
        //"Great Shield Zombie", //106
        //"Moss Knight Fat", //204
        //public static List<string> bossTypeNames = new List<string>()
        //{
        //    "Jar Collector", //115 (boss)
        //    //"Mushroom Turret", //
        //    "Mantis Traitor Lord", //194 (BOSS)          needs lots of room or will fall through the floor  
        //    "Mantis Lord Temp", //167 (BOSSES)
        //    "Mantis Lord", //
        //    "Zombie Beam Miner Rematch", //241
        //    //"Zote Boss",//33 (BOSS???)
        //    "Infected Knight", //320 (boss)
        //    "Hornet Boss 1",//121     (BOSS)       
        //    "Hornet Boss 2", //302 (BOSS)
        //    "Mimic Spider", //269 (Nosk, BOSS)
        //    "Dung Defender", //331  (BOSS)
        //    "Fluke Mother" //336 (BOSS)

        //    //"PLACEHOLDER"
        //};
    }
}


#pragma warning restore 0162
