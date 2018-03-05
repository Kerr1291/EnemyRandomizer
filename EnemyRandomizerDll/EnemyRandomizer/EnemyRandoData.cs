using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnemyRandomizerMod
{
    public class EnemyRandoData
    {
        /*
         * 
         *  NOTES
         * 
         * 
         * --needs more space (above it) when it spawns
         * Royal Gaurd needs more space, stuck in floor when spawned
         * giant moss crawler needs more space?, fell through the world?
         * mimic grub
         * great zombie guard (2 damage sword one)
         * Lobster
         * mega fat bee (flying but needs space)
         * moss knight fat
         * 
         * --rotated mobs like worms need to be un-rotated
         * 
         * --Laser Turret Frames needs to be properly rotated and placed on a surface (worms work pretty well actually)
         * --plant turret needs to be placed on ground or other surface
         * laser crawler
         * 
         * 
         * --mawlek turret (3) disappeared?
         * --huge grass charger fell down elevator shaft and vanished?
         * --infected knight (lost kin) replaced a miner in crystal peak and vanished
         * --zombie beam miner and and zombie shield dropped through floor after they replaced some miners in crystal peak
         * 
         * 
         * --baulder shell enemy had every enemy he shot randomized
         * 
         * --enemies that replace shade siblings should be placed into the air/on the ground (they spawn in the ground)
         * 
         * --maybe add Abyss Tendrils?
         * 
         * --brooding malwerk spazzes out of spawned in a tight hallway
         * 
         * --spawning door mantis is kinda lame
         * 
         * --"lil jellyfish" (the exploding one) is getting rando replaced
         * 
         * --add Centipede Hatcher to rando monsters?
         * they're extra dangerous because theirs spawns are randomized
         * 
         * --health scuttler is the blue health bug
         * 
         * --"Steep Slope" to keep player from climbing something
         * 
         * --flukemarm spawning in certain spots just causes infinite yelling effect
         * 
         * --things replacing baby centipede need to be adjusted to be sure they're in a safe spot like the siblings since they can spawn inside the ground
         * 
         * --add? Tentacle Box monster
         * 
         * 
         * --fun thing, small mushrooms rando when they "wake up"
         * 
         * --having nosk spawn while exploring is annoying
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * ----
         * --prevent wall type enemies from replacing flyers
         * --spread out the post load searching for randomized enemies over multiple frames, it takes too long and causes the game to hang for a moment
         * --moss knight fat fell through the world? OH i think this is the "sub boss" one... let's not randomize it
         * --mushroom brawler needs to be lowered to the floor? and flipped/rotated the proper direction
         * --mage night was invisible? not sure what adjustment it needs
         * --brooding mawlek could be adjusted to the bottom/ground, but it's also kinda ok floating above things
         * --wall enemies need logic to be placed on walls correctly
         * --Ruins Flying Sentry Javelin wasn't randomized in one scene
         * --bug: big bee replaced mage knight
         * --spider flyer didn't get randomized
         * --bug? mantis heavy flyer is a ground thing?
         * --crystal guardian will have to be removed or have its camera effect fixed, it causes the camera to pan way off to the side on activation
         * 
         * 
         * --mage blob replaced the zombie guard by grub?
         * --mage knight seems to have spawning issue, need to experiement with it more
         * 
         * --removed mender bug as he was always showing up as disabled
         * 
         * 
         * 
         * //TRY THIS to enable enemy hitboxes:
         *   https://github.com/AllanBishop/UnityPhysicsDebugDraw2D
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
         */


        /*
         * Tutorial: 7 
         * Crossroads: Scenes 38-80         * 
         * Ruins_House (city of tears internal): 81-85  
         * Ruins (City of tears): 86 - 116
         * Greenpath/Fungal Wastes/Queens Gardens: 117 - 212
         * Howling Cliffs: 213 - 218
         * Resting Grounds: 219 - 228
         * Crystal Peak Mines: 229 - 256
         * Deepnest: 257 - 302
         * Abyss: 304 - 323
         * Waterways: 326 - 339
         * White Palace: 340 - 358
         * Hive: 359 - 363
         * 
         */


        public static List<int> enemyTypeScenes = new List<int>()
        {
            7,//tutorial scene
            28,
            33,
            34,
            35,
            36,
            //38,//mender bug
            40,
            41,
            46,
            47,
            49,
            52,
            55,
            58,
            71,
            74,
            76,
            82,
            88,//Ruins Flying Sentry
            90,//(should have Ruins Flying Sentry Javelin (2) )
            97,
            102,
            106,
            115,
            117,
            121,
            126,//(should have Acid Flyer???)
            127,
            135,//should have Fat Fly???
            137,
            142,//lazy flyer enemy
            144,
            146,
            155,
            158,
            164,//(Mantis Flyer Child)
            166,
            167,
            177,
            181,//(fungus flyer)
            183,
            186,//(moss flyer)
            189,
            194,
            204,
            208,
            226,
            232,//(crystal flyer)
            234,
            241,
            243,
            244,
            259,
            269,
            271,
            276,//(supposed to have spider flyer)
            285,
            302,
            313,
            320,
            321,
            326,//(Inflater)
            327,
            331,
            336,
            340,
            358,
            362
        };

        public static List<string> enemyTypeNames = new List<string>()
        {
            //Fungus shaman?
            "Colosseum_Armoured_Mosquito",//34
            "Mosquito",//28
            
            "Colosseum_Miner",//35

            //Colosseum
            "Giant Buzzer Col",//33
            "Angry Buzzer",//35
            "Buzzer",//74

            "Colosseum_Shield_Zombie",//33
            "Super Spitter Col",//33
            "Super Spitter",//34
            "Spitter", //40

            "Bursting Bouncer",//33
            
            "Ceiling Dropper Col",//34
            "Ceiling Dropper", //??(forgot)
            
            "Giant Fly Col",//34 
            //"Giant Fly",// (gruz mother, it starts sleeping and looks weird...)
            //"Fly Spawn",
            "Fat Fly", //146 --doesn't seem to be here? --try scene 135???
            "Blow Fly", //285
            //"Fluke Fly Spawner", //
            "Fluke Fly", //327
            //"White Palace Fly", //340 (may have to remove randomization for this too)
            "Fly",

            "Grub Mimic",//34
            "Mega Fat Bee",//34
            "Colosseum_Flying_Sentry",//34
            "Spitting Zombie",//34 and 41

            "Colosseum Grass Hopper",//35
            "Giant Hopper",//34
            "Grass Hopper", //208
            "Flip Hopper", //
            "Hopper",//34
            "Blobble",//34

            "Lancer",//35
            "Lobster",//35
            
            //"Mage Balloon Spawner",//35
            "Mage Knight",//35
            "Mage Blob",//35
            "Mage Balloon", //102
            //"Mage Lord Phase2", //97 (BOSS)
            //"Mage Lord", //97 (BOSS)
            "Electric Mage",//35
            "Mage",//35

            "Lesser Mawlek",//35
            "Mawlek Col",//35 ??? might be same as lesser mawlek
            "Mawlek Body", //46 (BOSS)

            //Crossroads
            "Bursting Zombie",//36
            //"Mender Bug",//38?(not 36?)
            
            "Fung Crawler", //158
            "Mines Crawler", //243
            "Crystal Crawler", //243
            //"Crystal Crawler", //243?
            "Abyss Crawler", //320
            //"Abyss Crawler", //321
            "Crawler", //40

            "Climber", //40
            
            "Zombie Hornhead Sp", //271
            "Zombie Hornhead", //41 (Giant Fly = boss)


            // "Head",//turns out this is just a head....
            "Prayer Slug", //47 prayer slugs = maggots
                        
            "Colosseum_Worm",//34
            "Worm", //49
            
            //"Hatcher Baby Spawner", //55
            "Hatcher",//52

            "Zombie Shield", //52
            "Zombie Leaper",//52
            "Zombie Myla", //71
            //"Blocker",//74 (baulder shell)

            "Zombie Guard", //76 (big enemy)
            
            "Colosseum_Armoured_Roller",//33
            "Mushroom Roller", //
            "Roller", //74

            //City of tears
            "Gorgeous Husk", //82
            
            "Zombie Runner Sp", //271
            "Zombie Runner", //90

            "Zombie Barger", //90
            
            "Ruins Sentry Fat", //90
            "Ruins Sentry", //90


            "Great Shield Zombie", //106

            "Royal Zombie Coward", //106
            "Royal Zombie Fat", //106
            "Royal Zombie", //106

            "Ruins Flying Sentry Javelin", //106
            "Ruins Flying Sentry", //106
            
            //"Jar Collector", //115 (boss)

            //Greenapth
            "Moss Walker", //117
            "Mossman_Shaker", //117
            //"Pigeon", //117 (don't randomize, while technically enemies, kinda ends up being lame)
            "Plant Trap", //117
            "Mossman_Runner",//117

            "Acid Flyer", //126
            //"Shell", //126 (don't randomize this, since it's key to a couple encounters)
            
            "Plant Turret", //127
            //"Acid Walker", //127 (don't randomize this, since it's fairly key to getting around a few places)
            

            "Moss Knight Fat", //204
            "Moss Knight", //137            

            //"Mega Moss Charger", //144 (subboss thing, doesn't work when spawned somewhere else)
            
            
            //Fungal Waste
            "Zombie Fungus", //155
            "Fungus Flyer", //
            "Mushroom Turret", //
            "Fungoon Baby", //



            
            "Mantis Traitor Lord", //194 (BOSS)          needs lots of room or will fall through the floor  
            "Mantis Heavy Flyer",//35
            "Mantis Heavy",//35
            //"Mantis Lord Temp", //167 (BOSSES)
            //"Mantis Lord S1", // (static clinging mantis bosses, don't load these)
            //"Mantis Lord S2", // (static clinging mantis bosses, don't load these)
            //"Mantis Lord", // (boss)
            "Mantis Flyer Child", //166
            "Gate Mantis", // --kinda boring, let's not randomize him, also might break something in mantis village
            "Mantis", //

            "Mushroom Baby", //177
            "Mushroom Brawler", //

            //Queens Garden
            "Jellyfish Baby", //183
            "Jellyfish", //

            "Moss Flyer", //
            "Garden Zombie", //

            
            "Lazy Flyer Enemy", //

            //Resting Grounds
            "Grave Zombie", //226

            //Crystal Peak
            "Laser Turret Frames", //234

            //"Mega Zombie Beam Miner", //241 -- has camera issues, don't load for now until we work on a fix

            "Zombie Miner", //243
            //"Crystallised Lazer Bug", //243 -- has nullref issues, don't load for now until we work on a fix
            "Crystal Flyer", //235

            "Zombie Beam Miner", //244

            //Deepnest
            "Baby Centipede", //259

            "Slash Spider", //271
            "Spider Mini", //271
            "Zombie Spider 1", //271 -- TODO: put a filter in to NOT use him a replacement
            "Zombie Spider 2", //271

            "Tiny Spider", //276
            "Spider Flyer", //
            //"Deep Spikes", //
            
            //Kingdom's Edge
            "Bee Hatchling Ambient", //

            //Abyss
            "Shade Sibling", //313
            
            "Parasite Balloon", //320
            
            "Mawlek Turret Ceiling", //321
            "Mawlek Turret", //321

            //Waterways
            "Inflater", //
            "Flukeman", //
            
            //White Palace
            "Royal Gaurd", //358

            //Hive
            "Big Bee", //362
            "Bee Stinger", //   
            "Zombie Hive", //   
            //"Hiveling Spawner", // don't want this

            
            //"Mimic Spider", //269 (Nosk, BOSS) -- has a memory leak, don't load
            
            
            "Zote Boss"//33 (BOSS???)
        };


        //==============================================================================================================
        //==============================================================================================================
        //==============================================================================================================


        public static List<string> flyerEnemyTypeNames = new List<string>()
        {
            "Colosseum_Armoured_Mosquito",//34
            "Mosquito",//28
            "Giant Buzzer Col",//33
            "Angry Buzzer",//35
            "Buzzer",//74
            "Super Spitter Col",//33
            "Super Spitter",//34
            "Spitter", //40
            "Bursting Bouncer",//33
            "Giant Fly Col",//34 
            //"Giant Fly",//(gruz mother?)
            //"Fly Spawn",
            "Fat Fly", //146 
            "Blow Fly", //285
            //"Fluke Fly Spawner", //
            "Fluke Fly", //327
            //"White Palace Fly", //340
            "Fly",
            "Mega Fat Bee",//34
            "Colosseum_Flying_Sentry",//34
            "Blobble",//34
            "Mage Balloon", //102
            "Electric Mage",//35
            "Mage",//35
            "Hatcher",//52
            "Ruins Flying Sentry Javelin", //106
            "Ruins Flying Sentry", //106
            "Acid Flyer", //126
            "Fungus Flyer", //
            "Fungoon Baby", //
            "Mantis Heavy Flyer",//35
            "Mantis Flyer Child", //166
            "Jellyfish Baby", //183
            "Jellyfish", //
            "Moss Flyer", //
            "Lazy Flyer Enemy", //
            "Crystal Flyer", //235
            "Spider Flyer", //
            "Bee Hatchling Ambient", //
            "Inflater", //
            "Bee Stinger", // 
            "Shade Sibling",
            "Big Bee", //362 
            "Parasite Balloon"
        };

        public static List<string> groundEnemyTypeNames = new List<string>()
        {
            "Colosseum_Miner",//35
            "Colosseum_Worm",//34
            "Colosseum_Shield_Zombie",//33
            "Grub Mimic",//34
            "Spitting Zombie",//34 and 41
            "Colosseum Grass Hopper",//35
            "Giant Hopper",//34
            "Grass Hopper", //208
            "Flip Hopper", //
            "Hopper",//34
            "Lancer",//35
            "Lobster",//35
            "Mage Blob",//35 (the "mistake" enemy)
            "Mage Knight",//35
            "Lesser Mawlek",//35
            "Mawlek Col",//35 ??? might be same as lesser mawlek
            "Crystal Crawler", //243
            "Mawlek Body", //46 (BOSS)
            "Bursting Zombie",//36
            //"Mender Bug",//38?(not 36?)
            "Zombie Hornhead Sp", //271
            "Zombie Hornhead", //41 (Giant Fly = boss)
            "Prayer Slug", //47 prayer slugs = maggots
            "Zombie Shield", //52
            "Zombie Leaper",//52
            "Zombie Myla", //71
            //"Blocker",//74
            "Zombie Guard", //76 (big enemy)
            "Colosseum_Armoured_Roller",//33
            "Mushroom Roller", //
            "Roller", //74
            "Gorgeous Husk", //82
            "Zombie Runner Sp", //271
            "Zombie Runner", //90
            "Zombie Barger", //90
            "Ruins Sentry Fat", //90
            "Ruins Sentry", //90
            "Great Shield Zombie", //106
            "Royal Zombie Coward", //106
            "Royal Zombie Fat", //106
            "Royal Zombie", //106
            "Moss Walker", //117
            "Mossman_Shaker", //117
            "Mossman_Runner",//117
            //"Moss Knight Fat", //204
            "Moss Knight", //137            
            "Zombie Fungus", //155
            "Mantis Heavy",//35
            "Mantis", //
            "Mushroom Baby", //177
            "Mushroom Brawler", //
            "Garden Zombie", //
            "Grave Zombie", //226
            "Zombie Miner", //243
            "Zombie Beam Miner", //244
            "Baby Centipede", //259
            "Slash Spider", //271
            "Spider Mini", //271
            "Zombie Spider 1", //271
            "Zombie Spider 2", //271
            "Tiny Spider", //276
            "Flukeman", //
            "Royal Gaurd", //358
            "Zombie Hive", //   
            "Gate Mantis",
            "Mega Zombie Beam Miner", //241
            "Mimic Spider", //269 (Nosk, BOSS)
            "Mantis Traitor Lord", //194 (BOSS)          needs lots of room or will fall through the floor  
            "Zote Boss"//33 (BOSS???)
        };


        public static List<string> crawlerEnemyTypeNames = new List<string>()
        {
            "Fung Crawler", //158
            "Mines Crawler", //243
            "Abyss Crawler", //320
            "Crawler", //40
            "Climber", //40
            "Crystallised Lazer Bug", //243 (this was removed from the list for some reason???)
        };

        public static List<string> smallEnemyTypeNames = new List<string>()
        {
            "Colosseum_Armoured_Mosquito",//34
            "Mosquito",//28
            "Buzzer",//74
            "Super Spitter Col",//33
            "Super Spitter",//34
            "Spitter", //40
            "Bursting Bouncer",//33
            "Ceiling Dropper Col",//34
            "Ceiling Dropper", //??(forgot)
            //"Fluke Fly Spawner", //
            "Moss Flyer", //
            "Fluke Fly", //327
            //"White Palace Fly", //340
            "Fly",
            "Blobble",//34
            "Mage Blob",//35
            "Mage Balloon", //102
            "Fung Crawler", //158
            "Mines Crawler", //243
            "Abyss Crawler", //320
            "Crawler", //40
            "Climber", //40
            "Colosseum_Armoured_Roller",//33
            "Roller", //74
            "Mossman_Shaker", //117
            "Plant Turret", //127
            "Zombie Fungus", //155
            "Mushroom Turret", //
            "Fungoon Baby", //
            "Mushroom Baby", //177
            "Jellyfish Baby", //183
            "Garden Zombie", //
            "Laser Turret Frames", //234
            "Crystallised Lazer Bug", //243
            "Crystal Flyer", //235
            "Spider Mini", //271
            "Tiny Spider", //276
            "Bee Hatchling Ambient", //
            "Inflater", //
            "Parasite Balloon",
            "Mawlek Turret Ceiling", //321
            "Mawlek Turret", //321
            "Worm", //49
            "Worm", //49
            "Plant Trap", //117
            "Plant Trap", //117
            "Zote Boss"//33 (BOSS???)
        };

        public static List<string> mediumEnemyTypeNames = new List<string>()
        {
            "Colosseum_Miner",//35
            "Giant Buzzer Col",//33
            "Angry Buzzer",//35
            "Colosseum_Shield_Zombie",//33
            "Grub Mimic",//34
            "Colosseum_Flying_Sentry",//34
            "Spitting Zombie",//34 and 41
            "Colosseum Grass Hopper",//35
            "Giant Hopper",//34
            "Grass Hopper", //208
            "Flip Hopper", //
            "Hopper",//34
            "Lesser Mawlek",//35
            "Mawlek Col",//35 ??? might be same as lesser mawlek
            "Bursting Zombie",//36
            //"Mender Bug",//38?(not 36?)
            "Crystal Crawler", //243
            "Zombie Hornhead Sp", //271
            "Zombie Hornhead", //41 (Giant Fly = boss)
            "Prayer Slug", //47 prayer slugs = maggots
            "Hatcher",//52
            "Zombie Shield", //52
            "Zombie Leaper",//52
            "Zombie Myla", //71
            //"Blocker",//74
            "Mushroom Roller", //
            "Gorgeous Husk", //82
            "Zombie Runner Sp", //271
            "Zombie Runner", //90
            "Zombie Barger", //90
            "Ruins Sentry", //90
            "Royal Zombie Coward", //106
            "Royal Zombie Fat", //106
            "Royal Zombie", //106
            "Ruins Flying Sentry Javelin", //106
            "Ruins Flying Sentry", //106
            "Moss Walker", //117
            "Mossman_Runner",//117
            "Acid Flyer", //126
            "Gate Mantis",
            "Mantis Heavy Flyer",//35
            "Mantis Heavy",//35
            "Mantis Flyer Child", //166
            "Mantis", //
            "Zombie Miner", //243
            "Zombie Beam Miner", //244
            "Baby Centipede", //259
            "Zombie Spider 1", //271
            "Zombie Spider 2", //271
            "Spider Flyer", //
            "Flukeman", //
            "Zombie Hive", //   
            "Bee Stinger", //   
            "Shade Sibling",

            "Mega Zombie Beam Miner", //241


            //original "big enemy" list
            "Blow Fly", //285
            "Ruins Sentry Fat", //90
            "Giant Fly Col",//34 
            //"Giant Fly",//(gruz mother?)
            //"Fly Spawn",
            "Fat Fly", //146 
            "Mega Fat Bee",//34
            "Lancer",//35
            "Lobster",//35
            "Mage Knight",//35
            "Electric Mage",//35
            "Mage",//35
            "Mawlek Body", //46 (BOSS)
            "Colosseum_Worm",//34
            "Worm", //49
            "Zombie Guard", //76 (big enemy)
            "Great Shield Zombie", //106
            "Moss Knight Fat", //204
            "Moss Knight", //137            
            "Fungus Flyer", //
            "Mushroom Brawler", //
            "Jellyfish", //
            "Lazy Flyer Enemy", //
            "Grave Zombie", //226
            "Slash Spider", //271   -- doesn't seem to randomize well, needs more research
            "Royal Gaurd", //358
            "Big Bee", //362
            "Colosseum_Worm",//34
            "Mimic Spider", //269 (Nosk, BOSS)
            "Mantis Traitor Lord" //194 (BOSS)          needs lots of room or will fall through the floor  



        };

        //TODO: testing....
        public static List<string> bigEnemyTypeNames = new List<string>()
        {
            "NONE",
            "NONE"
            //"Blow Fly", //285
            //"Ruins Sentry Fat", //90
            //"Giant Fly Col",//34 
            ////"Giant Fly",//(gruz mother?)
            ////"Fly Spawn",
            //"Fat Fly", //146 
            //"Mega Fat Bee",//34
            //"Lancer",//35
            //"Lobster",//35
            //"Mage Knight",//35
            //"Electric Mage",//35
            //"Mage",//35
            //"Mawlek Body", //46 (BOSS)
            //"Colosseum_Worm",//34
            //"Worm", //49
            //"Zombie Guard", //76 (big enemy)
            //"Great Shield Zombie", //106
            //"Moss Knight Fat", //204
            //"Moss Knight", //137            
            //"Fungus Flyer", //
            //"Mushroom Brawler", //
            //"Jellyfish", //
            //"Lazy Flyer Enemy", //
            //"Grave Zombie", //226
            //"Slash Spider", //271   -- doesn't seem to randomize well, needs more research
            //"Royal Gaurd", //358
            //"Big Bee", //362
            //"Colosseum_Worm",//34
            //"Mimic Spider", //269 (Nosk, BOSS)
            //"Mantis Traitor Lord" //194 (BOSS)          needs lots of room or will fall through the floor  
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
            "Mushroom Turret"
        };

        //public static List<string> hardEnemyTypeNames = new List<string>()
        //{
        //    "Gorgeous Husk", //82 (for fun)
        //    "Electric Mage",//35
        //    "Mage Knight",//35
        //    "Giant Buzzer Col",//33
        //    "Giant Fly Col",//34 
        //    "Lancer",//35
        //    "Lobster",//35
        //    "Mawlek Body", //46 (mini boss)
        //    "Moss Knight", //137            
        //    "Mushroom Brawler", //
        //    "Royal Gaurd", //358
        //    "Mimic Spider", //269 (Nosk, BOSS)
        //    "Mantis Traitor Lord" //194 (BOSS)          needs lots of room or will fall through the floor  
        //};

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
        //    "Mega Zombie Beam Miner", //241
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
