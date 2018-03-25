using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using nv;

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
        //public const bool USE_TEST_SCENES = false;

        //public static List<int> EnemyTypeScenes {
        //    get {
        //        //if( USE_TEST_SCENES )
        //            return testTypeScenes;

        //        //else
        //        //    return enemyTypeScenes;
        //    }
        //}

        //static List<int> testTypeScenes = new List<int>()
        //{
        //    //244, //test crystal guardian and Zombie Beam Miner(244)
        //    34, //test super spitter (colosseum enemies)
        //    7 //test flame spawn
        //    //276, //testing tiny spider
        //    //189,//testing garden zombie

        //    //test these for mage??
        //    //96  //testing mage?
        //};


        /* 
         * Zones by build index__
         * 
         * Tutorial: 7 
         * Crossroads: Scenes 38-80         
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

        //static List<int> enemyTypeScenes = new List<int>()
        //{
        //    241,  //LOAD THIS SCENE FIRST -- has laser impact special objects that we want
        //    244,

        //    96,  //LOAD SECOND -- has mage/mage knight/mage blob and related enemies AND Teleplanes that the mage requires

        //    7,//tutorial scene

        //    28,
        //    33,
        //    34,
        //    35,
        //    36,

        //    //38,//mender bug
        //    40,
        //    41,
        //    46,
        //    47,
        //    49,
        //    52,
        //    55,
        //    58,
        //    71,
        //    74,
        //    76,
        //    82,
        //    88,//Ruins Flying Sentry
        //    90,//(should have Ruins Flying Sentry Javelin (2) )
        //    97,
        //    102,
        //    106,
        //    115,
        //    117,
        //    121,
        //    126,//(should have Acid Flyer???)
        //    127,
        //    135,//should have Fat Fly???
        //    137,
        //    142,//lazy flyer enemy
        //    144,
        //    146,
        //    155,
        //    158,
        //    164,//(Mantis Flyer Child)
        //    166,
        //    167,
        //    177,
        //    181,//(fungus flyer)
        //    183,
        //    186,//(moss flyer)
        //    189,//garden zombie
        //    194,
        //    204,
        //    208,
        //    226,
        //    //232,//(crystal flyer) -- TEST, don't seem to need this!
        //    234,

        //    243,
        //    259,
        //    269,
        //    271,//seems to have loading problem?
        //    276,//(supposed to have spider flyer)
        //    285,
        //    302,
        //    313,
        //    320,
        //    321,
        //    326,//(Inflater)
        //    327,
        //    331,
        //    336,
        //    340,
        //    358,
        //    362
        //};

        public List<int> emptyScenesToSkipOnLoad = new List<int>()
        {
            0,1,2,3,4,5
            ,7
            ,8
            ,9
            ,10
            ,11
            ,12
            ,13
            ,14
            ,15
            ,16
            ,17
            ,18
            ,19
            ,20
            ,21
            ,22
            ,23
            ,24
            ,25
            ,26
            ,28
            ,29
            ,30
            ,31
            ,35
            ,36
            ,38
            ,41
            ,42
            ,44
            ,46
            ,47
            ,51
            ,52
            ,53
            ,55
            ,56
            ,60
            ,61
            ,62
            ,63
            ,64
            ,65
            ,66
            ,67
            ,68
            ,69
            ,70
            ,71
            ,72
            ,74
            ,75
            ,77
            ,78
            ,79
            ,80
            ,82
            ,85
            ,86
            ,87
            ,89
            ,91
            ,93
            ,95
            ,96
            ,97
            ,98
            ,99
            ,100
            ,101
            ,103
            ,104
            ,105
            ,106
            ,107
            ,108
            ,109
            ,110
            ,111
            ,112
            ,113
            ,116
            ,117
            ,118
            ,119
            ,120
            ,121
            ,122
            ,123
            ,124
            ,125
            ,127
            ,129
            ,130
            ,131
            ,132
            ,134
            ,135
            ,136
            ,137
            ,142
            ,143
            ,144
            ,145
            ,146
            ,150
            ,152
            ,153
            ,155
            ,157
            ,158
            ,160
            ,162
            ,163
            ,164
            ,165
            ,166
            ,170
            ,172
            ,173
            ,175
            ,177
            ,179
            ,181
            ,182
            ,183
            ,184
            ,185
            ,186
            ,187
            ,188
            ,189
            ,190
            ,191
            ,192
            ,193
            ,195
            ,196
            ,198
            ,199
            ,201
            ,202
            ,204
            ,205
            ,206
            ,207
            ,208
            ,210
            ,211
            ,212
            ,213
            ,214
            ,215
            ,216
            ,217
            ,218
            ,220
            ,222
            ,223
            ,224
            ,225
            ,226
            ,227
            ,230
            ,231
            ,233
            ,234
            ,235
            ,236
            ,237
            ,239
            ,240
            ,241
            ,242
            ,243
            ,244
            ,246
            ,247
            ,248
            ,250
            ,253
            ,254
            ,255
            ,257
            ,259
            ,260
            ,262
            ,263
            ,265
            ,266
            ,267
            ,268
            ,269
            ,270
            ,272
            ,273
            ,274
            ,275
            ,276
            ,277
            ,281
            ,282
            ,283
            ,284
            ,285
            ,287
            ,288
            ,289
            ,291
            ,293
            ,294
            ,295
            ,296
            ,299
            ,300
            ,301
            ,302
            ,303
            ,304
            ,305
            ,307
            ,308
            ,311
            ,312
            ,313
            ,315
            ,316
            ,317
            ,318
            ,319
            ,320
            ,321
            ,322
            ,323
            ,324
            ,325
            ,326
            ,327
            ,328
            ,329
            ,330
            ,331
            ,333
            ,334
            ,335
            ,337
            ,338
            ,339
            ,340
            ,342
            ,345
            ,346
            ,347
            ,348
            ,351
            ,352
            ,353
            ,354
            ,356
            ,357
            ,359
            ,360
            ,362
            ,363
            ,364
            ,368
            ,369
            ,370
            ,371
            ,372
            ,373
            ,374
            ,375
            ,376
            ,377
            ,378
            ,379
            ,380
            ,381
            ,382
            ,383
            ,385
            ,387
            ,388
            ,390
            ,391
            ,394
            ,400
            ,401
            ,402
            ,403
            ,404
            ,405
            ,406
            ,408
            ,410
            ,411
            ,412
            ,413
            ,414
            ,415
            ,416
            ,417
            ,418
            ,419
            ,420
        };

        public List<int> scenesLoaded = new List<int>();

        //effects used by enemies, like crystal guardian, or just things we can use for fun
        public static List<string> effectObjectNames = new List<string>()
        {
            //load from crystal guardian scene 241
            "Beam Impact",
            "Beam Ball",
            "Beam Point L",
            "Beam Point R",
            "Beam",
            "Crystal Rain"
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

        //all the enemy types used in the randomizer
        //public static List<string> enemyTypeNames = new List<string>()
        //{
        //    //Fungus shaman?
        //    "Flamebearer Spawn",//7 -- NEEDS TESTING -- probably doesn't work right now

        //    "Colosseum_Armoured_Mosquito",//34
        //    "Mosquito",//28
            
        //    "Colosseum_Miner",//35

        //    //Colosseum
        //    "Giant Buzzer Col",//33
        //    "Angry Buzzer",//35
        //    "Buzzer",//74

        //    "Colosseum_Shield_Zombie",//33
        //    "Super Spitter Col",//33
        //    "Super Spitter",//34
        //    "Spitter", //40

        //    "Bursting Bouncer",//33
            
        //    "Ceiling Dropper Col",//34
        //    "Ceiling Dropper", //??(forgot)
            
        //    "Giant Fly Col",//34 
        //    //"Giant Fly",// (gruz mother, it starts sleeping and looks weird...)
        //    //"Fly Spawn",
        //    "Fat Fly", //146 --doesn't seem to be here? --try scene 135???
        //    "Blow Fly", //285
        //    "Fluke Fly Spawner", // NEEDS TESTING
        //    "Fluke Fly", //327
        //    //"White Palace Fly", //340 (may have to remove randomization for this too)
        //    "Fly",

        //    "Grub Mimic",//34
        //    "Mega Fat Bee",//34
        //    "Colosseum_Flying_Sentry",//34
        //    "Spitting Zombie",//34 and 41

        //    "Colosseum Grass Hopper",//35
        //    "Giant Hopper",//34
        //    "Grass Hopper", //208
        //    "Flip Hopper", //
        //    "Hopper",//34
        //    "Blobble",//34

        //    "Lancer",//35
        //    "Lobster",//35
            
        //    //"Mage Balloon Spawner",//35
        //    "Mage Knight",//35(96)  
        //    "Mage Blob",//35(96)  
        //    "Mage Balloon", //102  
        //    //"Mage Lord Phase2", //97 (BOSS)
        //    //"Mage Lord", //97 (BOSS)
        //    "Electric Mage",//35  
        //    "Mage",//35(96)  

        //    "Lesser Mawlek",//35
        //    "Mawlek Col",//35 ??? might be same as lesser mawlek
        //    "Mawlek Body", //46 (BOSS)

        //    //Crossroads
        //    "Bursting Zombie",//36
        //    //"Mender Bug",//38?(not 36?)
            
        //    "Fung Crawler", //158
        //    "Mines Crawler", //243
        //    "Crystal Crawler", //243  
        //    "Abyss Crawler", //320
        //    "Crawler", //40

        //    "Climber", //40
            
        //    "Zombie Hornhead Sp", //271
        //    "Zombie Hornhead", //41 (Giant Fly = boss)


        //    // "Head",//turns out this is just a head....
        //    "Prayer Slug", //47 prayer slugs = maggots
                        
        //    "Colosseum_Worm",//34
        //    "Worm", //49
            
        //    //"Hatcher Baby Spawner", //55
        //    "Hatcher",//52

        //    "Zombie Shield", //52
        //    "Zombie Leaper",//52
        //    "Zombie Myla", //71 -- seems to be killing enemies it replaces?? maybe remove from replacement list
        //    "Blocker",//74 (baulder shell)

        //    "Zombie Guard", //76 (big enemy)
            
        //    "Colosseum_Armoured_Roller",//33
        //    "Mushroom Roller", //
        //    "Roller", //74

        //    //City of tears
        //    "Gorgeous Husk", //82
            
        //    "Zombie Runner Sp", //271 ???
        //    "Zombie Runner", //90

        //    "Zombie Barger", //90
            
        //    "Ruins Sentry Fat", //90
        //    "Ruins Sentry", //90


        //    "Great Shield Zombie", //106

        //    "Royal Zombie Coward", //106
        //    "Royal Zombie Fat", //106
        //    "Royal Zombie", //106

        //    "Ruins Flying Sentry Javelin", //106
        //    "Ruins Flying Sentry", //106
            
        //    //"Jar Collector", //115 (boss)

        //    //Greenapth
        //    "Moss Walker", //117
        //    "Mossman_Shaker", //117
        //    "Pigeon", //117 (don't randomize, while technically enemies, kinda ends up being lame)
        //    "Plant Trap", //117
        //    "Mossman_Runner",//117

        //    "Acid Flyer", //126
        //    "Shell", //126 (don't randomize this, since it's key to a couple encounters)
            
        //    "Plant Turret", //127
        //    //"Acid Walker", //127 (don't randomize this, since it's fairly key to getting around a few places)
            

        //    "Moss Knight Fat", //204
        //    "Moss Knight", //137            

        //    //"Mega Moss Charger", //144 (subboss thing, doesn't work when spawned somewhere else)
            
            
        //    //Fungal Waste
        //    "Zombie Fungus", //155
        //    "Fungus Flyer", //
        //    "Mushroom Turret", //
        //    "Fungoon Baby", //



            
        //    "Mantis Traitor Lord", //194 (BOSS)          needs lots of room or will fall through the floor  
        //    "Mantis Heavy Flyer",//35  
        //    "Mantis Heavy",//35  
        //    //"Mantis Lord Temp", //167 (BOSSES)
        //    //"Mantis Lord S1", // (static clinging mantis bosses, don't load these)
        //    //"Mantis Lord S2", // (static clinging mantis bosses, don't load these)
        //    //"Mantis Lord", // (boss)
        //    "Mantis Flyer Child", //166
        //    "Gate Mantis", // --kinda boring, let's not randomize him, also might break something in mantis village
        //    "Mantis", //

        //    "Mushroom Baby", //177
        //    "Mushroom Brawler", //

        //    //Queens Garden
        //    "Jellyfish Baby", //183  
        //    "Jellyfish", //  

        //    "Moss Flyer", //  
        //    "Garden Zombie", //183 -- has idle issues, does not wake up like it should

            
        //    "Lazy Flyer Enemy", //  

        //    //Resting Grounds
        //    "Grave Zombie", //226

        //    //Crystal Peak
        //    "Laser Turret Frames", //234

        //    "Zombie Beam Miner Rematch", //241    

        //    "Zombie Miner", //243  
        //    "Crystallised Lazer Bug", //243  
        //    "Crystal Flyer", //243  

        //    "Zombie Beam Miner", //244 -- has nullref issues, don't load for now until we work on a fix

        //    //Deepnest
        //    "Baby Centipede", //259
        //    "Centipede Hatcher", //???? (find scene for this)

        //    "Slash Spider", //271  
        //    "Spider Mini", //271
        //    "Zombie Spider 1", //271   -- TODO: put a filter in to NOT use him a replacement
        //    "Zombie Spider 2", //271  

        //    "Tiny Spider", //276
        //    "Spider Flyer", // 
        //    //"Deep Spikes", //
            
        //    //Kingdom's Edge
        //    "Bee Hatchling Ambient", //

        //    //Abyss
        //    "Shade Sibling", //313
            
        //    "Parasite Balloon", //320
            
        //    "Mawlek Turret Ceiling", //321
        //    "Mawlek Turret", //321

        //    //Waterways
        //    "Inflater", //
        //    "Flukeman", //
            
        //    //White Palace
        //    "Royal Gaurd", //358

        //    //Hive
        //    "Big Bee", //362
        //    "Bee Stinger", //   
        //    "Zombie Hive", //   
        //    //"Hiveling Spawner", // don't want this

            
        //    //"Mimic Spider", //269 (Nosk, BOSS) -- has a memory leak, don't load
            
            
        //    "Zote Boss"//33 (BOSS???)
        //};


        //==============================================================================================================
        //==============================================================================================================
        //==============================================================================================================


        public static List<string> flyerEnemyTypeNames = new List<string>()
        {
            "Mosquito",
            "Giant Buzzer Col",
            "Mega Fat Bee",
            "Mage",
            "Mage Balloon",
            "Electric Mage",
            "Angry Buzzer",
            "Spitter",
            "Buzzer",
            "Fly",
            "Giant Fly",
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
            "Radiance"
        };

        public static List<string> groundEnemyTypeNames = new List<string>()
        {
            "Colosseum_Miner",
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
            "Mega Fat Bee",
            "Mage Knight",
            "Mage Blob",
            "Mage Balloon",
            "Zombie Runner",
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
            "Mega Fat Bee",
            "Mage Knight",
            "Mage",
            "Electric Mage",
            "Angry Buzzer",
            "Blocker",
            "Mawlek Body",
            "Great Shield Zombie",
            "Gorgeous Husk",
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
            "White Palace Fly"
            ////until mage enemies are fixed, do not spawn them in battle areas
            //"Electric Mage",//35 //TODO: needs to be moved down? (by 20?) -- broken can't put most places
            //"Mage",//35 //TODO: still broken
            //"Mantis Traitor Lord",
            //"Flamebearer Spawn",
            //"Centipede Hatcher",
            //"Mossman_Shaker",
            //"Mage Blob",//35
            //"Mage Balloon", //102
            //"Zombie Spider 1", //271
            //"Zombie Spider 2", //271
            //"Parasite Balloon",
            ////"Zombie Beam Miner Rematch", //241 should be fixed now
            ////"Mage Knight",//35 //he's fixed now
            //"Lancer",//35
            //"Lobster",//35
            //"Colosseum_Worm",//34
            //"Laser Turret Frames", //234
            //"Worm", //49
            //"Baby Centipede", //259
            //"Zote Boss"//33 (BOSS???)
        };


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

            "Dung Defender"
            //"Colosseum_Armoured_Mosquito",//34
            
            //"Colosseum_Miner",//35

            //"Giant Buzzer Col",//33
            
            //"Colosseum_Shield_Zombie",//33
            //"Super Spitter Col",//33

            //"Bursting Bouncer",//33
            
            //"Ceiling Dropper Col",//34
            //"Giant Fly Col",//34 
            
            //"Grub Mimic",//34
            //"Mega Fat Bee",//34
            //"Colosseum_Flying_Sentry",//34

            //"Colosseum Grass Hopper",//35
            //"Giant Hopper",//34
            //"Hopper",//34
            //"Blobble",//34

            //"Lancer",//35
            //"Lobster",//35
            
            //"Mage Knight",//35
            //"Mage Blob",//35
            //"Electric Mage",//35
            //"Mage",//35
            //"Mantis Heavy Flyer",//35
            //"Mantis Heavy",//35
            
            //"Mawlek Col"//35 ??? might be same as lesser mawlek
        };


        public static List<string> hardEnemyTypeNames = new List<string>()
        {
            "Mage Knight",
            "Electric Mage",
            "Mega Fat Bee",
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
            "Moss Knight C",
            "Moss Knight B",
            "Ghost Warrior No Eyes",
            "Mushroom Brawler",
            "Ghost Warrior Hu",
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
