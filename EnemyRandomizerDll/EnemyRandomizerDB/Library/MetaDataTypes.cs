using System.Collections.Generic;

namespace EnemyRandomizerMod
{
    public static class MetaDataTypes
    {
        public static List<string> Bosses = new List<string>() {
            "Giant Fly",
            "Mega Moss Charger",
            "Mawlek Body",

            "Black Knight",
            "Fluke Mother",
            "Mantis Traitor Lord",

            "Hornet Boss 1",
            "Hornet Boss 2",

            "Giant Buzzer",
            "Giant Buzzer Col",
            "Mega Fat Bee", //obblobble boss
            "Lobster",
            "Lancer",

            "Dung Defender",
            "White Defender",

            "Mega Zombie Beam Miner",
            "Zombie Beam Miner Rematch",

            "Grimm Boss",
            "Nightmare Grimm Boss",

            "Infected Knight",
            "Lost Kin",

            "False Knight New",
            "False Knight Dream",

            "Mage Lord",
            "Mage Lord Phase2",
            "Dream Mage Lord",
            "Dream Mage Lord Phase2",

            "Jar Collector",
            "Hornet Nosk",
            "Mimic Spider",
            "Mega Jellyfish",
            "Jellyfish GG",

            "Hive Knight",

            "Pale Lurker",
            "Oro",
            "Mato",
            "Sheo Boss",
            "Sly Boss",

            "Grey Prince",
            "Zote Boss",

            "Hollow Knight Boss",
            "HK Prime",

            "Radiance",
            "Absolute Radiance",

            "Ghost Warrior Xero",
            "Ghost Warrior Marmu",
            "Ghost Warrior Galien",

            "Ghost Warrior Slug",
            "Ghost Warrior No Eyes",
            "Ghost Warrior Hu",
            "Ghost Warrior Markoth",
            };

        public static List<string> IsWallMounted = new List<string>() {
            "Ceiling Dropper",
            "Ceiling Dropper Col",
            "Mantis Flyer Child",
            "Plant Trap",
            "Plant Turret",
            "Plant Turret Right",
            "Mushroom Turret",
            "Laser Turret Frames",
            "Mawlek Turret",
            "Mawlek Turret Ceiling",
            };

        public static List<string> Flying = new List<string>() {
            "Acid Flyer",
            "Blobble",
            "Flamebearer Small",
            "Flamebearer Med",
            "Flamebearer Large",
            "Mosquito",
            "Bursting Bouncer",
            "Buzzer Col",
            "Giant Fly",
            "Colosseum_Armoured_Mosquito",
            "Colosseum_Flying_Sentry",
            "Angry Buzzer",
            "Mantis Heavy Flyer",
            "Fly",
            "Shade Sibling",
            "Colosseum_Armoured_Mosquito R",
            "Buzzer",
            "Mega Fat Bee",
            "Giant Buzzer Col",
            "Hatcher",
            "Ruins Flying Sentry",
            "Ruins Flying Sentry Javelin",
            "Acid Flyer",
            "Fat Fly",
            "Lazy Flyer Enemy",
            "Moss Flyer",
            "Crystal Flyer",
            "Centipede Hatcher",
            "Blow Fly",
            "Bee Hatchling Ambient",
            "Spider Flyer",
            "Parasite Balloon",
            "Inflater",
            "Fluke Fly",
            "White Palace Fly",
            "Bee Stinger",
            "Big Bee",
            "Zote Balloon",
            "Zote Balloon Ordeal",
            "Zote Salubra",
            "Zote Fluke",
            "Radiance",
            "Mage",
            "Electric Mage",
            "Mage Balloon",
            "Mage Lord",
            "Mage Lord Phase2",
            "Dream Mage Lord",
            "Dream Mage Lord Phase2",
            "Hornet Nosk",
            "Fungus Flyer",
            "Hatcher Baby",
            "Jellyfish",
            "Lil Jellyfish",
            "Jellyfish Baby",
            "Super Spitter",
            "Spitter R",
            "Spitter",
            "Super Spitter R",
            "Mega Jellyfish",
            "Jellyfish GG",
            "Ghost Warrior Xero",
            "Ghost Warrior Marmu",
            "Ghost Warrior Galien",
            "Ghost Warrior Slug",
            "Ghost Warrior No Eyes",
            "Ghost Warrior Hu",
            "Ghost Warrior Markoth",
            "Electro Zap",
            "Bee Dropper",
            };

        public static List<string> Crawling = new List<string>() {
            "Crawler",
            "Crystal Crawler",
            "Tiny Spider",
            };

        public static List<string> Climbing = new List<string>() {
            "Spider Mini",
            "Climber",
            "Mines Crawler",
            "Crystallised Lazer Bug",
            "Abyss Crawler",
            };

        public static List<string> Static = new List<string>() {
            "Blocker",
            "Egg Sac",
            "Plant Trap",
            "Mawlek Turret",
            "Mawlek Turret Ceiling",
            "fluke_baby_01",
            "fluke_baby_02",
            "fluke_baby_03",
            "Zote Turret",
            "White Palace Fly",
            "Electro Zap",
            "Zote Balloon",
            "Zote Balloon Ordeal",
            "Plant Turret",
            "Plant Turret Right",
            "Mushroom Turret",
            "Laser Turret Frames",
            "Fluke Mother",
            };

        //public static List<string> IsSummonedByEvent = new List<string>() {
        //    "Giant Fly Col",
        //    "Buzzer Col",
        //    "Colosseum_Armoured_Roller",
        //    "Colosseum_Miner",
        //    "Colosseum_Armoured_Mosquito",
        //    "Colosseum_Flying_Sentry",
        //    "Ceiling Dropper Col",
        //    "Colosseum_Worm",
        //    "Mawlek Col",
        //    "Colosseum Grass Hopper",
        //    "Hatcher Baby",
        //    "Zombie Spider 2",
        //    "Zombie Spider 1",
        //    "Flukeman Top",
        //    "Flukeman Bot",
        //    "Colosseum_Armoured_Roller R",
        //    "Colosseum_Armoured_Mosquito R",
        //    "Giant Buzzer Col",
        //    "Mega Fat Bee",
        //    "Lobster",
        //    "Mage Knight",
        //    "Mage",
        //    "Electric Mage",
        //    "Mender Bug",
        //    "Mawlek Body",
        //    "False Knight New",
        //    "Mage Lord",
        //    "Mage Lord Phase2",
        //    "Black Knight",
        //    "Moss Knight",
        //    "Jar Collector",
        //    "Giant Buzzer",
        //    "Mega Moss Charger",
        //    "Mantis Traitor Lord",
        //    "Mega Zombie Beam Miner",
        //    "Zombie Beam Miner",
        //    "Zombie Beam Miner Rematch",
        //    "Mimic Spider",
        //    "Hornet Boss 2",
        //    "Infected Knight",
        //    "Dung Defender",
        //    "Fluke Mother",
        //    "Hive Knight",
        //    "Grimm Boss",
        //    "Nightmare Grimm Boss",
        //    "False Knight Dream",
        //    "Dream Mage Lord",
        //    "Dream Mage Lord Phase2",
        //    "Lost Kin",
        //    "Grey Prince",
        //    "Radiance",
        //    "Hollow Knight Boss",
        //    "HK Prime",
        //    "Pale Lurker",
        //    "Oro",
        //    "Mato",
        //    "Sheo Boss",
        //    "Absolute Radiance",
        //    "Sly Boss",
        //    "Hornet Nosk",
        //    "Mega Jellyfish",
        //    "Jellyfish GG",
        //    "Ghost Warrior Xero",
        //    "Ghost Warrior Marmu",
        //    "Ghost Warrior Galien",
        //    "Ghost Warrior Slug",
        //    "Ghost Warrior No Eyes",
        //    "Ghost Warrior Hu",
        //    "Ghost Warrior Markoth",
        //    };

        public static List<string> AlwaysDeleteObject = new List<string>() {
            "Fly Spawn",
            "Hatcher Spawn",
            "Hatcher Baby Spawner",
            "Parasite Balloon Spawner",
            };

        public static List<string> SpawnerEnemies = new List<string>() {
            "Zombie Hornhead Sp",
            "Zombie Runner Sp",
            "Centipede Hatcher",
            "Blocker",
            "Flukeman",
            "Zombie Hive",
            "fluke_baby_01",
            "fluke_baby_02",
            "fluke_baby_03",
            "Fluke Mother",
            "Hatcher",
            "Giant Fly",
            };

        public static List<string> TinkerEnemies = new List<string>() {
            "Acid Flyer",
            "Acid Walker",
            };

        public static Dictionary<string, List<string>> BattleEnemies = new Dictionary<string, List<string>>()
        {
            {"ANY",
                new List<string>()
                {
                    "Giant Fly Col",
                    "Buzzer Col",
                    "Colosseum_Armoured_Roller",
                    "Colosseum_Armoured_Mosquito",
                    "Colosseum_Flying_Sentry",
                    "Colosseum_Miner",
                    "Ceiling Dropper Col",
                    "Colosseum_Worm",
                    "Mawlek Col",
                    "Colosseum Grass Hopper",
                    "Colosseum_Armoured_Roller R",
                    "Colosseum_Armoured_Mosquito R",
                    "Giant Buzzer Col",
                    "Mega Fat Bee",
                    "Mage Knight",
                    "Electric Mage",
                    "Giant Buzzer",
                    "Mawlek Body",
                    "False Knight New",
                    "Jar Collector",
                    "Black Knight",
                }
            },


            {"Abyss_17",
                new List<string>()
                {
                    "Lesser Mawlek",
                }
            },
            {"Abyss_19",
                new List<string>()
                {
                    "Infected Knight",
                }
            },
            {"Crossroads_04",
                new List<string>()
                {
                    "Giant Fly",
                    "Fly",
                }
            },
            {"Crossroads_08",
                new List<string>()
                {
                    "Spitter",
                }
            },
            {"Crossroads_22",
                new List<string>()
                {
                    "Hatcher",
                    "Spitter",
                }
            },
            {"Deepnest_33",
                new List<string>()
                {
                    "Zombie Spider 1",
                    "Zombie Spider 2",
                }
            },
            {"Fungus1_21",
                new List<string>()
                {
                    "Moss Knight",
                }
            },
            {"Fungus1_32",
                new List<string>()
                {
                    "Moss Knight B",
                    "Moss Knight C",
                    "Moss Knight",
                }
            },
            {"Fungus2_05",
                new List<string>()
                {
                    "Mushroom Baby",
                    "Mushroom Brawler",
                }
            },
            {"Fungus3_archive_02_boss",
                new List<string>()
                {
                    "Mega Jellyfish",
                }
            },
            {"Mines_18_boss",
                new List<string>()
                {
                    "Mega Zombie Beam Miner",
                }
            },
            {"Ruins1_23",
                new List<string>()
                {
                    "Mage Blob",
                    "Mage Knight",
                    "Mage",
                }
            },
            {"Ruins2_01_b",
                new List<string>()
                {
                    "Royal Zombie Fat",
                    "Royal Zombie",
                    "Royal Zombie Coward",
                }
            },
            {"Ruins2_03",
                new List<string>()
                {
                    "Ruins Flying Sentry Javelin",
                    "Great Shield Zombie",
                }
            },
        };

        public static List<string> PogoLogicEnemies = new List<string>() 
        {
            "Acid Flyer",
            "Acid Walker",
            "White Palace Fly",
        };

        public static List<string> HasUniqueSizeEnemies = new List<string>()
        {
            "Acid Flyer",
        };

        public static List<string> SmasherNeedsCustomSmashBehaviour = new List<string>()
        {
            "Inflater",
            "Buzzer",
            "Angry Buzzer",
            "Bursting Bouncer",
            "Colosseum_Flying_Sentry",
            "Fly",
            "Blobble",
            "Bee Hatchling Ambient",
            "Moss Flyer",
            "Spitter",
        };

        public static Dictionary<string,float> GoodSmasherReplacement = new Dictionary<string, float>()
        {
            {"Mosquito", 0.5f},
            {"Colosseum_Armoured_Mosquito", 0.5f},
            {"Spitter", 0.5f},
            {"Moss Flyer", 1f},
            {"Zoteling Buzzer", 0.25f},
            {"Hornet Boss 1", 0.5f},
            {"Fluke Fly", 0.5f},
            {"Bee Hatchling Ambient", 0.5f},
            {"Bee Stinger", 0.5f},
            {"Big Bee", 0.5f},           
            {"Colosseum_Flying_Sentry", 0.7f},
            {"Mega Fat Bee", 1f},
            {"Bursting Bouncer", 1f},
            {"Blobble", 1f},
            {"Angry Buzzer", 1f},
            {"Fly", 1f},
            {"Buzzer", 1f},
            {"Giant Fly", 1f}, //spawn error, needs fix
            {"Inflater", 1f},

            {"Grass Hopper", 1f},
            {"Zoteling", 0.25f},


            //{"Hopper", 0.5f},
            //{"Giant Hopper", 0.5f},
            {"Zote Boss", 0.3f},//spawns in floor
            {"Zote Crew Normal", 0.3f},//spawns in floor

            //{"Bee Dropper", 1f} //needs some starting velocity, a collider so it can be hit?, and POOB component
        };

        public static List<string> BadPogoReplacement = new List<string>()
        {
            "Mantis Traitor Lord",
            "Oro",
            "Mato",
            "Sheo Boss",
            "Sly Boss",
            "Dream Mage Lord",
            "Dream Mage Lord Phase2",
            "Hive Knight",
            "Grimm Boss",
            "Nightmare Grimm Boss",
            "Fluke Mother",
            "Mawlek Body",
            "Mage Lord",
            "Zote Turret",
            "Mage Lord Phase2",
            "Dung Defender",
            "White Defender",
            "False Knight New",
            "False Knight Dream",
            "Ceiling Dropper",
            "Ceiling Dropper Col",
            "Plant Trap",
            "Plant Turret",
            "Plant Turret Right",
            "Mage",
            "Electric Mage",
            "Ruins Flying Sentry Javelin",
            "Ghost Warrior Xero",
            "Ghost Warrior Markoth",
            "Moss Charger",
            "Mimic Spider",
            "Mega Moss Charger"
        };

        public static List<string> InGroundEnemy = new List<string>()
        {
            "Pigeon",
            "Dung Defender",
            "White Defender",
            "Plant Trap",
            "Moss Charger",
            "Zote Turret",
            "Laser Turret Frames",
            "Mega Moss Charger"
        };

        public static List<string> RandoControlledPooling = new List<string>()
        {
            "Radiant Nail",
            "Dust Trail",
        };

        public static List<string> ReplacementEnemiesToSkip = new List<string>()
        {
            "Zote Balloon Ordeal",
            "Dream Mage Lord Phase2",
            "Mage Lord Phase2",
            "Mega Jellyfish",
            "Mega Jellyfish GG",
            //"Radiance",
            "Giant Buzzer",//temporarily broken
            "Giant Buzzer Col",//temporarily broken
            "Corpse Garden Zombie", //don't spawn this, it's just a corpse
            "Super Spitter Col"                    ,
            //"Giant Fly Col"                        ,
            "Buzzer Col"                           ,
            "Ceiling Dropper Col"                  ,
            "Roller R"                             ,
            "Spitter R"                            ,
            //"Lil Jellyfish"                        ,
            "Buzzer R"                             ,
            "Flukeman Top"                         ,
            "Flukeman Bot"                         ,
            "Colosseum_Armoured_Roller R"          ,
            "Colosseum_Armoured_Mosquito R"        ,
            "Super Spitter R"                      ,
            "Giant Buzzer Col"                     ,
            "Plant Turret Right"                   ,
            //"Colosseum_Worm"                       ,
            //"Colosseum Grass Hopper"               ,
            //"Mage Lord"               , //until he's fixed
            //"Mage Lord Phase2"               , //until he's fixed
            "Zombie Beam Miner Rematch",
            "White Defender"               , //until he's fixed
            "Ghost Warrior Markoth",
            "Infected Knight",
            "Nightmare Grimm Boss",
            "Ghost Warrior Xero",
            "Hive Knight",
            "Mato",
            "Sly Boss",
            "False Knight Dream",
            "Bee Dropper",
        };

        public static List<string> ReplacementHazardsToSkip = new List<string>()
        {
            "Cave Spikes tile", //small bunch of upward pointing cave spikes
            "Cave Spikes tile(Clone)"
        };

        //TODO: temp solution for "bad effects"
        public static List<string> ReplacementEffectsToSkip = new List<string>()
        {
            "tank_fill",
            "tank_full",
            "Bugs Idle",
            "bee_fg_swarm",
            "spider sil left",
            "Butterflies FG",
            "Butterflies BG",
            "wind system",
            "water component",
            "BG_swarm_01",
            "Ruins_Rain",
            "Snore",
            "BG_swarm_02",
            "FG_swarm_02",
            "FG_swarm_01",
            "Particle System FG",
            "Particle System BG",
            "bg_dream",
            "fung_immediate_BG",  //
            "tank_full", //large rect of acid, LOOPING
            "bg_dream",  //big dream circles that float in the background, LOOPING
            "Bugs Idle", //floaty puffs of glowy moth things, LOOPING
            "Shade Particles", //sprays out shade stuff in an wide upward spray, LOOPING
            "Fire Particles", //emits burning fire particles, LOOPING
            "spawn particles b", //has a serialization error? -- may need fixing/don't destroy on load or something
            "Acid Steam", //has a serialization error?
            "Spre Fizzle", //emits a little upward spray of green particles, LOOPING
            "Dust Land", //emits a crescent of steamy puffs that spray upward a bit originating a bit under the given origin, LOOPING
            "Slash Ball", //emits the huge pale lurker slash AoE, LOOPING
            "Bone", //serialzation error?
            "Particle System", //serialization error?
            "Dust Land Small", //??? seems to be making lots of issues
            "Infected Grass A", //??? seems to be making lots of issues
            //"Dust Trail", // --- likely needs mod to control pooling
        };


        public static List<string> BurstEffects = new List<string>()
        {
            "Roar Feathers",
        };


        /// <summary>
        /// Typically we're going to do this because altering the enemy or arena in other ways is more interesting
        /// or we're keeping it to preserve some other logic.
        /// </summary>
        public static List<string> SkipReplacementOfEnemyForLogicReasons = new List<string>()
        {
            "Giant Fly",
            "Giant Buzzer",
            "Blocker",
        };


        public static Dictionary<string, int> GeoZoneScale = new Dictionary<string, int>()
            {
                { "NONE", 1 },
                { "TEST_AREA", 1 },
                { "KINGS_PASS", 1 },
                { "CLIFFS", 1 },
                { "TOWN", 1 },
                { "CROSSROADS", 1 },
                { "GREEN_PATH", 2 },
                { "ROYAL_GARDENS", 5 },
                { "FOG_CANYON", 3 },
                { "WASTES", 3 },
                { "DEEPNEST", 4 },
                { "HIVE", 3 },
                { "BONE_FOREST", 1 },
                { "PALACE_GROUNDS", 8 },
                { "MINES", 3 },
                { "RESTING_GROUNDS", 1 },
                { "CITY", 3 },
                { "DREAM_WORLD", 5 },
                { "COLOSSEUM", 10 },
                { "ABYSS", 5 },
                { "ROYAL_QUARTER", 2 },
                { "WHITE_PALACE", 6 },
                { "SHAMAN_TEMPLE", 2 },
                { "WATERWAYS", 5 },
                { "QUEENS_STATION", 2 },
                { "OUTSKIRTS", 2 },
                { "KINGS_STATION", 4 },
                { "MAGE_TOWER", 3 },
                { "TRAM_UPPER", 1 },
                { "TRAM_LOWER", 1 },
                { "FINAL_BOSS", 11 },
                { "SOUL_SOCIETY", 2 },
                { "ACID_LAKE", 3 },
                { "NOEYES_TEMPLE", 2 },
                { "MONOMON_ARCHIVE", 7 },
                { "MANTIS_VILLAGE", 3 },
                { "RUINED_TRAMWAY", 1 },
                { "DISTANT_VILLAGE", 1 },
                { "ABYSS_DEEP", 6 },
                { "ISMAS_GROVE", 3 },
                { "WYRMSKIN", 4 },
                { "LURIENS_TOWER", 5 },
                { "LOVE_TOWER", 9 },
                { "GLADE", 2 },
                { "BLUE_LAKE", 1 },
                { "PEAK", 1 },
                { "JONI_GRAVE", 1 },
                { "OVERGROWN_MOUND", 2 },
                { "CRYSTAL_MOUND", 2 },
                { "BEASTS_DEN", 2 },
                { "GODS_GLORY", 5 },
                { "GODSEEKER_WASTE", 11 }
            };



        //main game zones go 0 - 49
        public static Dictionary<string, int> ProgressionZoneScale = new Dictionary<string, int>()
            {
                { "GODS_GLORY",         -5 },
                { "BONE_FOREST",        -3 },
                { "NONE",               -2 },
                { "TEST_AREA",          -1 },

                { "TOWN",               0 },
                { "DREAM_WORLD",        1 },
                { "KINGS_PASS",         2 },
                { "CROSSROADS",         3 },
                { "SHAMAN_TEMPLE",      4 },
                { "GREEN_PATH",         5 },
                { "GLADE",              6 },
                { "WASTES",             7 },
                { "MANTIS_VILLAGE",     8 },
                { "CLIFFS",             9 },
                { "JONI_GRAVE",        10 },
                { "OVERGROWN_MOUND",   11 },
                { "MINES",             12 },
                { "RESTING_GROUNDS",   13 },
                { "NOEYES_TEMPLE",     14 },
                { "BLUE_LAKE",         15 },
                { "CITY",              16 },
                { "QUEENS_STATION",    17 },
                { "MAGE_TOWER",        18 },
                { "SOUL_SOCIETY",      19 },
                { "ROYAL_QUARTER",     20 },
                { "KINGS_STATION",     21 },
                { "WATERWAYS",         22 },
                { "ISMAS_GROVE",       23 },
                { "ACID_LAKE",         24 },
                { "DEEPNEST",          25 },
                { "RUINED_TRAMWAY",    26 },
                { "DISTANT_VILLAGE",   27 },
                { "BEASTS_DEN",        28 },
                { "TRAM_UPPER",        29 },
                { "HIVE",              31 },
                { "TRAM_LOWER",        32 },
                { "OUTSKIRTS",         33 },
                { "WYRMSKIN",          34 },
                { "COLOSSEUM",         35 },
                { "CRYSTAL_MOUND",     36 },
                { "PEAK",              37 },
                { "FOG_CANYON",        38 },
                { "MONOMON_ARCHIVE",   39 },
                { "ROYAL_GARDENS",     41 },
                { "LOVE_TOWER",        42 },
                { "LURIENS_TOWER",     43 },
                { "ABYSS",             44 },
                { "PALACE_GROUNDS",    45 },
                { "WHITE_PALACE",      46 },
                { "GODSEEKER_WASTE",   47 },
                { "ABYSS_DEEP",        48 },
                { "FINAL_BOSS",        49 },
            };

        public static Dictionary<string, string> CustomReplacementName = new Dictionary<string, string>()
        {
            {"Health Cocoon", "HealthScuttler"},
            {"Health Scuttler", "HealthScuttler"},
            {"Orange Scuttler", "OrangeScuttler"},
            {"Abyss Tendril", "AbyssTendril"},
            {"Flamebearer Small", "FlameBearerSmall"},
            {"Flamebearer Med", "FlameBearerMed"},
            {"Flamebearer Large", "FlameBearerLarge"},
            {"Mosquito", "Mosquito"},
            {"Colosseum_Armoured_Roller", "ColRoller"},
            {"Colosseum_Miner", "ColMiner"},
            {"Zote Boss", "ZoteBoss"},
            {"Bursting Bouncer", "BurstingBouncer"},
            {"Super Spitter", "SuperSpitter"},
            {"Super Spitter Col", "SuperSpitterCol"},
            {"Giant Fly Col", "GiantFlyCol"},
            {"Colosseum_Shield_Zombie", "ColShield"},
            {"Buzzer Col", "BuzzerCol"},
            {"Blobble", "Blobble"},
            {"Colosseum_Armoured_Mosquito", "ColMosquito"},
            {"Colosseum_Flying_Sentry", "ColFlyingSentry"},
            {"Hopper", "Hopper"},
            {"Giant Hopper", "GiantHopper"},
            {"Ceiling Dropper Col", "CeilingDropperCol"},
            {"Colosseum_Worm", "ColWorm"},
            {"Spitting Zombie", "SpittingZombie"},
            {"Bursting Zombie", "BurstingZombie"},
            {"Angry Buzzer", "AngryBuzzer"},
            {"Mawlek Col", "MawlekCol"},
            {"Mantis Heavy", "MantisHeavy"},
            {"Lesser Mawlek", "LesserMawlek"},
            {"Mantis Heavy Flyer", "MantisHeavyFlyer"},
            {"Colosseum Grass Hopper", "ColHopper"},
            {"Fly", "Fly"},
            {"Roller", "Roller"},
            {"Hatcher Baby", "HatcherBaby"},
            {"Roller R", "RollerR"},
            {"Spitter R", "SpitterR"},
            {"Buzzer R", "Buzzer"},
            {"Mossman_Runner", "MossmanRunner"},
            {"Jellyfish", "Jellyfish"},
            {"Lil Jellyfish", "LilJellyfish"},
            {"Mantis Flyer Child", "MantisFlyerChild"},
            {"Ghost Warrior Slug", "GhostAladar"},
            {"Corpse Garden Zombie", "CorpseGardenZombie"},
            {"Baby Centipede", "BabyCentipede"},
            {"Zombie Spider 2", "ZombieSpider2"},
            {"Zombie Spider 1", "ZombieSpider1"},
            {"Tiny Spider", "ShootSpider"},
            {"Shade Sibling", "ShadeSibling"},
            {"Flukeman Top", "FlukemanTop"},
            {"Flukeman Bot", "FlukemanBot"},
            {"White Defender", "WhiteDefender"},
           

            {"Jellyfish GG", "JellyfishGG"},
            {"Colosseum_Armoured_Roller R", "ColosseumArmouredRollerR"},
            {"Colosseum_Armoured_Mosquito R", "ColosseumArmouredMosquitoR"},
            {"Super Spitter R", "SuperSpitterR"},
            {"Crawler", "Crawler"},
            {"Buzzer", "Buzzer"},
            {"Giant Buzzer Col", "GiantBuzzerCol"},
            {"Mega Fat Bee", "Oblobble"},
            {"Lobster", "LobsterLancer"},
            {"Mage Knight", "MageKnight"},
            {"Mage", "Mage"},
            {"Electric Mage", "ElectricMage"},
            {"Mage Blob", "MageBlob"},
            {"Lancer", "LobsterLancer"},
            {"Climber", "Climber"},
            {"Zombie Runner", "ZombieRunner"},
            {"Mender Bug", "MenderBug"},
            {"Spitter", "Spitter"},
            {"Zombie Hornhead", "ZombieHornhead"},
            {"Giant Fly", "GiantFly"},
            {"Zombie Barger", "ZombieBarger"},
            {"Mawlek Body", "Mawlek"},
            {"False Knight New", "FalseKnightNew"},
            {"Prayer Slug", "PrayerSlug"},
            {"Blocker", "Blocker"},
            {"Zombie Shield", "ZombieShield"},
            {"Hatcher", "Hatcher"},
            {"Zombie Leaper", "ZombieLeaper"},
            {"Zombie Guard", "ZombieGuard"},
            {"Zombie Myla", "ZombieMyla"},
            {"Egg Sac", "ZapBug"},
            {"Royal Zombie Fat", "RoyalPlumper"},
            {"Royal Zombie", "RoyalDandy"},
            {"Royal Zombie Coward", "RoyalCoward"},
            {"Gorgeous Husk", "GorgeousHusk"},
            {"Ceiling Dropper", "CeilingDropper"},
            {"Ruins Sentry", "Sentry"},
            {"Ruins Flying Sentry", "FlyingSentrySword"},
            {"Ruins Flying Sentry Javelin", "FlyingSentryJavelin"},
            {"Ruins Sentry Fat", "SentryFat"},
            {"Mage Balloon", "MageBalloon"},
            {"Mage Lord", "MageLord"},
            {"Mage Lord Phase2", "MageLordPhase2"},
            {"Great Shield Zombie", "GreatShieldZombie"},
            {"Great Shield Zombie bottom", "GreatShieldZombie"},
            {"Black Knight", "BlackKnight"},
            {"Jar Collector", "JarCollector"},
            {"Moss Walker", "MossWalker"},
            {"Plant Trap", "SnapperTrap"},
            {"Mossman_Shaker", "MossmanShaker"},
            {"Pigeon", "Pigeon"},
            {"Hornet Boss 1", "Hornet"},
            {"Acid Flyer", "AcidFlyer"},
            {"Moss Charger", "MossCharger"},
            {"Acid Walker", "AcidWalker"},
            {"Plant Turret", "PlantShooter"},
            {"Plant Turret Right", "PlantShooter"},
            {"Fat Fly", "FatFly"},
            {"Giant Buzzer", "BigBuzzer"},
            {"Moss Knight", "MossKnight"},
            {"Grass Hopper", "GrassHopper"},
            {"Lazy Flyer Enemy", "LazyFlyerEnemy"},
            {"Mega Moss Charger", "MegaMossCharger"},



            {"Ghost Warrior No Eyes", "GhostNoEyes"},
            {"Fungoon Baby", "FungoonBaby"},
            {"Mushroom Turret", "MushroomTurret"},
            {"Fungus Flyer", "FungusFlyer"},
            {"Zombie Fungus B", "FungifiedZombie"},
            {"Fung Crawler", "FungCrawler"},
            {"Mushroom Brawler", "MushroomBrawler"},
            {"Mushroom Baby", "MushroomBaby"},
            {"Mushroom Roller", "MushroomRoller"},
            {"Zombie Fungus A", "FungifiedZombie"},
            {"Mantis", "Mantis"},
            {"Ghost Warrior Hu", "GhostHu"},
            {"Jellyfish Baby", "JellyfishBaby"},
            {"Moss Flyer", "MossFlyer"},
            {"Garden Zombie", "GardenZombie"},
            {"Mantis Traitor Lord", "MantisTraitorLord"},
            {"Moss Knight Fat", "MossKnightFat"},
            {"Mantis Heavy Spawn", "HeavyMantis"},
            {"Ghost Warrior Marmu", "GhostMarmu"},
            {"Mega Jellyfish", "MegaJellyfish"},
            {"Ghost Warrior Xero", "GhostXero"},
            {"Grave Zombie", "GraveZombie"},
            {"Crystal Crawler", "CrystalCrawler"},
            {"Zombie Miner", "ZombieMiner"},
            {"Crystal Flyer", "CrystalFlyer"},
            {"Crystallised Lazer Bug", "LaserBug"},
            {"Mines Crawler", "MinesCrawler"},
            {"Mega Zombie Beam Miner", "MegaBeamMiner"},
            {"Zombie Beam Miner", "BeamMiner"},
            {"Zombie Beam Miner Rematch", "MegaBeamMiner"},
            {"Spider Mini", "SpiderMini"},
            {"Zombie Hornhead Sp", "SpiderCorpse"},
            {"Zombie Runner Sp", "SpiderCorpse"},
            {"Centipede Hatcher", "CentipedeHatcher"},
            {"Mimic Spider", "MimicSpider"},
            {"Slash Spider", "SlashSpider"},
            {"Spider Flyer", "SpiderFlyer"},
            {"Ghost Warrior Galien", "GhostGalien"},
            {"Blow Fly", "BlowFly"},
            {"Bee Hatchling Ambient", "BeeHatchling"},
            {"Ghost Warrior Markoth", "GhostMarkoth"},
            {"Hornet Boss 2", "DreamGuard"},
            {"Abyss Crawler", "AbyssCrawler"},
            {"Infected Knight", "InfectedKnight"},
            {"Parasite Balloon", "ParasiteBalloon"},
            {"Mawlek Turret", "MawlekTurret"},
            {"Mawlek Turret Ceiling", "MawlekTurret"},
            {"Flip Hopper", "FlipHopper"},
            {"Inflater", "Inflater"},
            {"Fluke Fly", "FlukeFly"},
            {"Flukeman", "Flukeman"},
            {"Dung Defender", "DungDefender"},
            {"fluke_baby_02", "JellyCrawler"},
            {"fluke_baby_01", "BlobFlyer"},
            {"fluke_baby_03", "MenderBug"},
            {"Fluke Mother", "FlukeMother"},
            {"White Palace Fly", "PalaceFly"},
            {"Enemy", "WhiteRoyal"},
            {"Royal Gaurd", "RoyalGaurd"},
            {"Zombie Hive", "ZombieHive"},
            {"Bee Stinger", "BeeStinger"},
            {"Big Bee", "BigBee"},
            {"Hive Knight", "HiveKnight"},
            {"Grimm Boss", "Grimm"},
            {"Nightmare Grimm Boss", "NightmareGrimm"},
            {"False Knight Dream", "FalseKnightDream"},
            {"Dream Mage Lord", "DreamMageLord"},
            {"Dream Mage Lord Phase2", "DreamMageLordPhase2"},
            {"Lost Kin", "LostKin"},
            {"Zoteling", "Zoteling"},
            {"Zoteling Buzzer", "ZotelingBuzzer"},
            {"Zoteling Hopper", "ZotelingHopper"},
            {"Zote Balloon", "ZoteBalloon"},
            {"Grey Prince", "GreyPrince"},
            {"Radiance", "FinalBoss"},
            {"Hollow Knight Boss", "HollowKnight"},
            {"HK Prime", "HollowKnightPrime"},
            {"Pale Lurker", "PaleLurker"},
            {"Oro", "NailBros"},
            {"Mato", "NailBros"},
            {"Sheo Boss", "Paintmaster"},
            {"Fat Fluke", "FatFluke"},
            {"Absolute Radiance", "AbsoluteRadiance"},
            {"Sly Boss", "Nailsage"},
            {"Zote Turret", "ZoteTurret"},
            {"Zote Balloon Ordeal", "ZotelingBalloon"},
            {"Ordeal Zoteling", "OrdealZoteling"},
            {"Zote Salubra", "EggSac"},
            {"Zote Thwomp", "ZoteThwomp"},
            {"Zote Fluke", "Mummy"},
            {"Zote Crew Normal", "ZoteCrewNormal"},
            {"Zote Crew Fat", "ZoteCrewFat"},
            {"Zote Crew Tall", "ZoteCrewTall"},
            {"Hornet Nosk", "HornetNosk"},
            {"Mace Head Bug", "MaceHeadBug"},
            {"Big Centipede Col", "BigCentipede"},
            {"Laser Turret Frames", "LaserBug"},
            {"Jelly Egg Bomb", "JellyEggBomb"},
            {"Worm", "ColWorm"},
            {"Bee Dropper", "Pigeon"}
        };

        public static Dictionary<string, bool> GoodForArenaBoss = new Dictionary<string, bool>()
        {
            {"Zote Boss", true},
            {"Super Spitter", true},
            {"Giant Hopper", true},
            {"Mantis Heavy", true},
            {"Lesser Mawlek", true},
            {"Mantis Heavy Flyer", true},
            {"Ghost Warrior Slug", true},
            {"Mega Fat Bee", true},
            {"Lobster", true},
            {"Mage Knight", true},
            {"Lancer", true},
            {"Giant Fly", false},
            {"Mawlek Body", true},
            {"False Knight New", true},
            {"Gorgeous Husk", true},
            {"Mage Lord", true},
            {"Mage Lord Phase2", true},
            {"Great Shield Zombie", true},
            {"Black Knight", true},
            {"Jar Collector", true},
            {"Hornet Boss 1", true},
            {"Hornet Boss 2", true},
            {"Moss Knight", true},
            {"Ghost Warrior No Eyes", false},
            {"Mantis Traitor Lord", false},
            {"Moss Knight Fat", true},
            {"Ghost Warrior Marmu", true},
            {"Mega Zombie Beam Miner", false},
            {"Mimic Spider", true},
            {"Ghost Warrior Galien", true},
            {"Fluke Mother", true},
            {"Royal Gaurd", true},
            {"False Knight Dream", false},
            {"Lost Kin", true},
            {"Grey Prince", true},
            {"Zote Crew Fat", true},
            {"Hornet Nosk", true},
        };


        public static Dictionary<string, bool> SafeForArenas = new Dictionary<string, bool>()
        {
            {"Mosquito", true},
            {"Colosseum_Armoured_Roller", true},
            {"Colosseum_Miner", true},
            {"Zote Boss", true},
            {"Bursting Bouncer", true},
            {"Super Spitter", true},
            {"Colosseum_Shield_Zombie", true},
            {"Blobble", true},
            {"Colosseum_Armoured_Mosquito", true},
            {"Colosseum_Flying_Sentry", true},
            {"Hopper", true},
            {"Giant Hopper", true},
            {"Spitting Zombie", true},
            {"Bursting Zombie", true},
            {"Angry Buzzer", true},
            {"Mantis Heavy", true},
            {"Lesser Mawlek", true},
            {"Mantis Heavy Flyer", true},
            {"Fly", true},
            {"Roller", true},
            {"Mossman_Runner", true},
            {"Jellyfish", true},
            {"Ghost Warrior Slug", true},
            {"Baby Centipede", true},
            {"Tiny Spider", true},
            {"Shade Sibling", true},
            {"Crawler", true},
            {"Buzzer", true},
            {"Mega Fat Bee", true},
            {"Lobster", true},
            {"Mage Knight", true},
            {"Mage", true},
            {"Electric Mage", true},
            {"Mage Blob", true},
            {"Lancer", true},
            {"Climber", true},
            {"Zombie Runner", true},
            {"Spitter", true},
            {"Zombie Hornhead", true},
            {"Giant Fly", true}, 
            {"Zombie Barger", true},
            {"Mawlek Body", true},
            {"False Knight New", true}, 
            {"Prayer Slug", true},
            {"Blocker", true},
            {"Zombie Shield", true},
            {"Hatcher", true},
            {"Zombie Leaper", true},
            {"Zombie Guard", true},
            {"Zombie Myla", true},
            {"Royal Zombie Fat", true},
            {"Royal Zombie", true},
            {"Royal Zombie Coward", true},
            {"Gorgeous Husk", true},
            {"Ceiling Dropper", true},
            {"Ruins Sentry", true},
            {"Ruins Flying Sentry", true},
            {"Ruins Flying Sentry Javelin", true},
            {"Ruins Sentry Fat", true},
            {"Mage Balloon", true},
            {"Mage Lord", true},
            {"Mage Lord Phase2", true},
            {"Great Shield Zombie", true},
            {"Black Knight", true},
            {"Jar Collector", true},
            {"Moss Walker", true},
            {"Mossman_Shaker", true},
            {"Hornet Boss 1", true},
            {"Fat Fly", true},
            {"Giant Buzzer", true},
            {"Moss Knight", true},
            {"Grass Hopper", true},
            {"Ghost Warrior No Eyes", true},
            {"Fungoon Baby", true},
            {"Mushroom Turret", true},
            {"Fungus Flyer", true},
            {"Zombie Fungus B", true},
            {"Fung Crawler", true},
            {"Mushroom Brawler", true},
            {"Mushroom Baby", true},
            {"Mushroom Roller", true},
            {"Zombie Fungus A", true},
            {"Mantis", true},
            {"Moss Flyer", true},
            {"Garden Zombie", true},
            {"Mantis Traitor Lord", true},
            {"Moss Knight Fat", true},
            {"Mantis Heavy Spawn", true},
            {"Ghost Warrior Marmu", true},
            {"Grave Zombie", true},
            {"Crystal Crawler", true},
            {"Zombie Miner", true},
            {"Crystal Flyer", true},
            {"Mines Crawler", true},
            {"Mega Zombie Beam Miner", false},
            {"Zombie Beam Miner", true},
            {"Spider Mini", true},
            {"Zombie Hornhead Sp", true},
            {"Zombie Runner Sp", true},
            {"Centipede Hatcher", true},
            {"Mimic Spider", true},
            {"Slash Spider", true},
            {"Ghost Warrior Galien", true},
            {"Blow Fly", true},
            {"Bee Hatchling Ambient", true},
            {"Hornet Boss 2", true},
            {"Abyss Crawler", true},
            {"Mawlek Turret", true},
            {"Flip Hopper", true},
            {"Inflater", true},
            {"Fluke Fly", true},
            {"Flukeman", true},
            {"Fluke Mother", true}, 
            {"White Palace Fly", true},
            {"Enemy", true},
            {"Royal Gaurd", true},
            {"Zombie Hive", true},
            {"Bee Stinger", true},
            {"Big Bee", true},
            {"False Knight Dream", false},
            {"Lost Kin", true},
            {"Zoteling", true},
            {"Zoteling Buzzer", true},
            {"Grey Prince", true},
            {"Pale Lurker", true},
            {"Fat Fluke", true},
            {"Zote Turret", true},
            {"Zote Salubra", true},
            {"Zote Thwomp", true},
            {"Zote Crew Normal", true},
            {"Zote Crew Fat", true},
            {"Zote Crew Tall", true},
            {"Hornet Nosk", true},
            {"Jellyfish Baby", true},
            {"Acid Walker", true},
            {"Colosseum_Worm", true},
            {"Colosseum Grass Hopper", true},
            //{"Grub Mimic", true},
            
            //disabled after testing
            {"Zoteling Hopper", false},
            {"Spider Flyer", false},//same problem as parasite balloon
            {"Parasite Balloon", false},//doesn't feel good, takes too long to show up and feels like the arena broke
            {"Zombie Spider 2", false},//sometimes spawns inside spikes or floors where it won't wake up
            {"Zombie Spider 1", false},//sometimes spawns inside spikes or floors where it won't wake up
            {"Plant Trap", false}, //spawns floating too much, can be hard to find

            //these could go into the arena pool after they're fixed
            {"Hive Knight", false},//not spawning
            {"Grimm Boss", false},//teleports into walls
            {"Nightmare Grimm Boss", false},//stuck on spawn -- still steals HUD -- death doesn't delete
            {"Dream Mage Lord", false},
            {"Dream Mage Lord Phase2", false},
            {"Radiance", false},
            {"Hollow Knight Boss", false},//fell through world
            {"HK Prime", false},//didn't spawn
            {"Oro", false},//didn't delete
            {"Mato", false},//didn't activate or die
            {"Sheo Boss", false},//didn't die
            {"Absolute Radiance", false},//nullref
            {"Sly Boss", false},//didn't die
            {"Zote Fluke", false},//add poob
            {"Ordeal Zoteling", false},
            {"Bee Dropper", false}, //needs some starting velocity, a collider so it can be hit?, and POOB component
            {"Ghost Warrior Hu", false},
            {"Mega Jellyfish", false},
            {"Ghost Warrior Xero", false},//killing caused nullrefs
            {"Zombie Beam Miner Rematch", false},
            {"Ghost Warrior Markoth", false},
            {"Infected Knight", false},//didn't spawn
            {"Dung Defender", false},
            {"Jellyfish GG", false},
            {"White Defender", false},//error on spawn

            //these are just not fit for an arena, ever
            {"Acid Flyer", false},
            {"Lazy Flyer Enemy", false},
            {"Moss Charger", false},
            {"Mega Moss Charger", false},
            {"Plant Turret", false},
            {"Flamebearer Small", false},//these work, but they're annoying and don't stay in the arenas
            {"Flamebearer Med", false},  //these work, but they're annoying and don't stay in the arenas
            {"Flamebearer Large", false},//these work, but they're annoying and don't stay in the arenas
            {"Mace Head Bug", false},
            {"Crystallised Lazer Bug", false},
            {"fluke_baby_02", false},
            {"fluke_baby_01", false},
            {"fluke_baby_03", false},
            {"Big Centipede Col", false},
            {"Jelly Egg Bomb", false},//not loaded?
            {"Worm", false}, //need flipped
            {"Laser Turret Frames", false},//nullref
            {"Health Scuttler", false},
            {"Orange Scuttler", false},
            {"Lil Jellyfish", false},//bomb, worked
            {"Abyss Tendril", false},
            {"Mender Bug", false},
            {"Egg Sac", false},//didnt transfer item
            {"Pigeon", false},
            {"Zote Balloon", false},
            {"Mantis Flyer Child", false},//spawned in ground
            
            //{"Grub Mimic Bottle", false},
            {"Mage Orb", false },
            
            //these are junk enemies, duplicates, or not enemies; never spawn these
            {"Hatcher Baby", false},
            {"Mawlek Turret Ceiling", false},//didn't spawn
            {"Colosseum_Armoured_Roller R", false},  //dont spawn
            {"Colosseum_Armoured_Mosquito R", false},//dont spawn
            {"Super Spitter R", false}, //dont spawn
            {"Giant Buzzer Col", false},
            {"Great Shield Zombie bottom", false},//skip, doesn't spawn
            {"Mawlek Col", false},//?? didn't spawn
            {"Plant Turret Right", false},
            {"Health Cocoon", false},
            {"Super Spitter Col", false},//didnt spawn
            {"Ceiling Dropper Col", false},//didn't spawn
            {"Giant Fly Col", false},//didn't spawn
            {"Buzzer Col", false},//didn't spawn
            {"Roller R", false},
            {"Spitter R", false},
            {"Buzzer R", false},
            {"Flukeman Top", false},//dont spawn
            {"Flukeman Bot", false},//don't spawn
            {"Zote Balloon Ordeal", false},//skip
            {"Corpse Garden Zombie", false},//literally just a corpse
        };



        public static Dictionary<string, float> RNGWeights = new Dictionary<string, float>()
        {
            {"Health Cocoon", 0.0001f},
            {"Health Scuttler", 0.0001f},
            {"Abyss Tendril", 0.00001f},
            {"Orange Scuttler", 0.01f},

            {"Flamebearer Small", 0.05f},//these work, but they're annoying and don't stay in the arenas
            {"Flamebearer Med", 0.05f},  //these work, but they're annoying and don't stay in the arenas
            {"Flamebearer Large", 0.05f},//these work, but they're annoying and don't stay in the arenas

            {"Mosquito", 0.6f},
            {"Colosseum_Armoured_Mosquito", 0.4f},

            {"Colosseum_Armoured_Roller", 0.5f},
            {"Roller", 0.6f},
            {"Roller R", 0.001f},

            {"Super Spitter", 1f},
            {"Super Spitter Col", 0.0001f},//didnt spawn
            
            {"Spitter R", 0.001f},
            {"Spitter", 1f},

            {"White Defender", 0.05f},//error on spawn
            {"Dung Defender", 0.1f},

            {"Giant Buzzer Col", 0.001f},//coun't hurt
            {"Giant Buzzer", 0.2f},
            
            {"Zombie Shield", 0.25f},
            {"Zombie Leaper", 0.25f},
            {"Zombie Guard", 0.25f},
            {"Zombie Runner", 0.25f},
            {"Zombie Hornhead", 0.25f},
            {"Zombie Barger", 0.25f},

            {"False Knight Dream", 0.0001f},
            {"False Knight New", 0.4f}, 

            {"Royal Zombie Fat", 0.4f},
            {"Royal Zombie", 0.4f},
            {"Royal Zombie Coward", 0.4f},

            {"Gorgeous Husk", 0.5f},
            {"Ceiling Dropper", 1f},

            {"Ruins Sentry", 0.2f},
            {"Ruins Flying Sentry", 0.2f},
            {"Ruins Flying Sentry Javelin", 0.05f},
            {"Ruins Sentry Fat", 0.3f},

            {"Great Shield Zombie", 0.25f},
            {"Great Shield Zombie bottom", 0.0001f},//skip, doesn't spawn
            
            {"Plant Trap", 1f},
            {"Pigeon", 0.0001f},

            {"Acid Flyer", 0.05f},
            {"Acid Walker", 0.25f},

            {"Plant Turret", 0.4f},
            {"Plant Turret Right", 0.0001f},

            {"Fungoon Baby", 0.3f},
            {"Fungus Flyer", 0.4f},

            {"Zombie Fungus A", 0.5f},
            {"Zombie Fungus B", 0.5f},

            {"Moss Flyer", 0.7f},
            {"Moss Knight Fat", 0.5f},
            {"Moss Knight", 0.6f},
            {"Moss Walker", 0.5f},
            {"Mossman_Shaker", 0.7f},
            {"Mossman_Runner", 0.7f},

            {"Moss Charger", 0.3f},
            {"Mega Moss Charger", 0.1f},

            {"Mantis Heavy", 0.2f},
            {"Mantis Heavy Flyer", 0.2f},
            {"Mantis Flyer Child", 0.05f},
            {"Mantis", 0.2f},
            {"Mantis Traitor Lord", 0.05f},
            {"Mantis Heavy Spawn", 0.1f},

            {"Ghost Warrior Markoth", 0.05f},
            {"Ghost Warrior Galien", 0.05f},
            {"Ghost Warrior Xero", 0.05f},
            {"Ghost Warrior Marmu", 0.05f},
            {"Ghost Warrior No Eyes", 0.05f},
            {"Ghost Warrior Hu", 0.05f},
            {"Ghost Warrior Slug", 0.05f},
            
            {"Zote Boss", 0.4f},

            {"Zoteling", 0.25f},
            {"Zoteling Buzzer", 0.15f},
            {"Zoteling Hopper", 0.15f},
            {"Ordeal Zoteling", 0.1f},

            {"Zote Balloon Ordeal", 0.0001f},//skip
            {"Zote Balloon", 0.1f},

            {"Zote Turret", 0.35f},
            {"Zote Salubra", 0.15f},
            {"Zote Thwomp", 0.15f},
            {"Zote Fluke", 0.15f},

            {"Zote Crew Normal", 0.3f},
            {"Zote Crew Fat", 0.3f},
            {"Zote Crew Tall", 0.3f},
            
            {"Jellyfish", 1f},
            {"Lil Jellyfish", 0.02f},//will just plow into player and explode, not fair but the surprise results in a lot of twitch clips when it's rare
            {"Jellyfish GG", 0.1f},
            {"Mega Jellyfish", 0.1f},
            {"Jellyfish Baby", 1f},

            {"Mega Zombie Beam Miner", 0.2f},
            {"Zombie Beam Miner", 0.7f},
            {"Zombie Beam Miner Rematch", 0.1f},//nullrefs on spawn
            
            {"fluke_baby_02", 0.3f},
            {"fluke_baby_01", 0.3f},//no blue mask
            {"fluke_baby_03", 0.3f},//no explode
            
            {"Mushroom Turret", 1f},//spawned in floor
            {"Mushroom Brawler", 1f},
            {"Mushroom Baby", 1f},
            {"Mushroom Roller", 1f},

            {"Oro", 0.1f},//didn't delete
            {"Mato", 0.1f},//didn't activate or die
            {"Sheo Boss", 0.1f},//didn't die

            {"Hollow Knight Boss", 0.05f},//fell through world
            {"HK Prime", 0.01f},//didn't spawn
            
            {"Radiance", 0.0001f},
            {"Absolute Radiance", 0.0001f},//nullref
            
            {"Mawlek Turret", 0.4f},
            {"Mawlek Turret Ceiling", 0.001f},//didn't spawn
            
            //{"Grub Mimic", 0.05f},
            //{"Grub Mimic Bottle", 0.02f},

            {"Mage Knight", 0.5f},
            {"Mage", 0.5f},//spawned outside arena
            {"Electric Mage", 0.2f},//teleported away
            {"Mage Blob", 0.7f},
            {"Mage Balloon", 0.7f},
            {"Mage Lord", 0.15f},//error? fix?
            {"Mage Lord Phase2", 0.1f},//works -- cant find his orb idle spot
            {"Dream Mage Lord", 0.15f},
            {"Dream Mage Lord Phase2", 0.1f},

            {"Hornet Boss 1", 0.7f},
            {"Hornet Boss 2", 0.3f},

            {"Zombie Myla", 0.5f},
            {"Zombie Miner", 0.5f},

            {"Zombie Hornhead Sp", 0.5f},
            {"Zombie Runner Sp", 0.5f},

            {"Fluke Fly", 0.4f},
            {"Flukeman", 0.3f},
            {"Fluke Mother", 0.1f}, //nullref
            
            {"Zombie Hive", 0.3f},
            {"Bee Hatchling Ambient", 0.5f},
            {"Bee Stinger", 0.4f},
            {"Big Bee", 0.3f},

            {"Infected Knight", 0.01f},//didn't spawn
            {"Lost Kin", 0.5f},

            {"Grimm Boss", 0.1f},//teleports into walls
            {"Nightmare Grimm Boss", 0.05f},//stuck on spawn -- still steals HUD -- death doesn't delete
            
            {"Hopper", 0.4f},
            {"Giant Hopper", 0.2f},

            {"Colosseum_Miner", 0.7f},
            {"Colosseum_Shield_Zombie", 0.7f},
            {"Colosseum_Flying_Sentry", 0.7f},

            {"Mega Fat Bee", 0.5f}, //???? didn't see it spawn
            {"Bursting Bouncer", 0.9f},//corpse didnt explode
            {"Blobble", 0.8f},
            {"Spitting Zombie", 0.7f},
            {"Bursting Zombie", 0.7f},
            {"Angry Buzzer", 1f},
            {"Lesser Mawlek", 1f},
            {"Fly", 1f},
            {"Buzzer R", 0.0001f},
            {"Baby Centipede", 0.3f},
            {"Zombie Spider 2", 0.5f},//nullref
            {"Tiny Spider", 0.8f},
            {"Shade Sibling", 0.5f},
            {"Crawler", 0.7f},
            {"Buzzer", 0.7f},
            {"Lobster", 0.5f},//was placed inside floor
            {"Lancer", 0.5f},//error, spawned stuck and couldn't die as well
            {"Climber", 0.7f},
            {"Mender Bug", 0.2f},
            {"Giant Fly", 0.7f},
            {"Mawlek Body", 1f},          
            {"Black Knight", 1f},
            {"Jar Collector", 0.4f},//jars are spawning inactive enemies -- spawned in floor
            {"Prayer Slug", 1f},
            {"Blocker", 0.2f},
            {"Hatcher", 0.7f},
            {"Egg Sac", 0.001f},//didnt transfer item
            {"Fat Fly", 0.7f},
            {"Grass Hopper", 0.5f},
            {"Colosseum Grass Hopper", 0.5f},
            {"Lazy Flyer Enemy", 0.25f},
            {"Fung Crawler", 1f},
            {"Garden Zombie", 1f},
            {"Grave Zombie", 1f},
            {"Crystal Crawler", 1f},
            {"Crystal Flyer", 1f},
            {"Crystallised Lazer Bug", 1f},
            {"Mines Crawler", 1f},
            {"Spider Mini", 1f},
            {"Centipede Hatcher", 1f},
            {"Mimic Spider", 0.5f},//nullref
            {"Slash Spider", 1f},
            {"Spider Flyer", 1f},
            {"Blow Fly", 1f},
            {"Abyss Crawler", 1f},
            {"Parasite Balloon", 0.7f},
            {"Flip Hopper", 1f},
            {"Inflater", 1f},
            {"White Palace Fly", 1f},
            {"Enemy", 0.5f},
            {"Royal Gaurd", 0.5f},//fix boomerang -- elite not spawning
            {"Hive Knight", 0.5f},//not spawning
            {"Grey Prince", 0.1f},
            {"Pale Lurker", 0.2f},
            {"Fat Fluke", 1f},
            {"Sly Boss", 0.05f},//didn't die
            {"Hornet Nosk", 0.1f},
            {"Mace Head Bug", 1f},
            {"Big Centipede Col", 1f},
            {"Laser Turret Frames", 1f},//nullref
            {"Jelly Egg Bomb", 0.5f},//not loaded?
            {"Worm", 1f}, //need flipped
            {"Zombie Spider 1", 0.5f},//didn't spawn
            {"Colosseum_Worm", 0.4f},

            {"Hatcher Baby", 0.0001f},
            {"Corpse Garden Zombie", 0.0001f},
            {"Colosseum_Armoured_Roller R", 0.0001f},  //dont spawn
            {"Flukeman Top", 0.0001f},//dont spawn
            {"Flukeman Bot", 0.0001f},//don't spawn
            {"Mawlek Col", 0.0001f},//?? didn't spawn
            {"Ceiling Dropper Col", 0.0001f},//didn't spawn
            {"Giant Fly Col", 0.0001f},//didn't spawn
            {"Buzzer Col", 0.0001f},//didn't spawn
            {"Colosseum_Armoured_Mosquito R", 0.0001f},//dont spawn
            {"Super Spitter R", 0.0001f}, //dont spawn

            {"Bee Dropper", 0.0001f} //needs some starting velocity, a collider so it can be hit?, and POOB component
        };














        public static Dictionary<string, float> PossibleEnemiesFromSpawner = new Dictionary<string, float>()
        {
            {"Orange Scuttler", 0.1f},
            {"Mosquito", 0.5f},
            {"Colosseum_Armoured_Mosquito", 0.5f},

            {"Colosseum_Armoured_Roller", 0.5f},
            {"Roller", 0.5f},
            {"Roller R", 0.5f},

            {"Super Spitter", 1f},
            {"Super Spitter Col", 0.5f},//didnt spawn
            
            {"Spitter R", 0.5f},
            {"Spitter", 0.5f},

            {"Royal Zombie Fat", 0.4f},
            {"Royal Zombie", 0.4f},
            {"Royal Zombie Coward", 0.4f},

            {"Gorgeous Husk", 1f},

            {"Ruins Sentry Fat", 0.25f},

            {"Plant Turret", 0.5f},
            {"Plant Turret Right", 0.01f},

            {"Fungoon Baby", 0.5f},
            {"Fungus Flyer", 0.5f},

            {"Zombie Fungus A", 0.5f},
            {"Zombie Fungus B", 0.5f},

            {"Moss Flyer", 1f},
            {"Moss Knight Fat", 1f},
            {"Moss Knight", 0.5f},
            {"Mossman_Shaker", 1f},
            {"Mossman_Runner", 1f},
                        
            {"Zote Boss", 0.5f},// still has white screen issue?

            {"Zoteling", 0.25f},
            {"Zoteling Buzzer", 0.25f},
            {"Zoteling Hopper", 0.25f},
            {"Ordeal Zoteling", 0.25f},

            {"Zote Crew Normal", 0.3f},//spawns in floor
            {"Zote Crew Fat", 0.3f},
            
            {"Lil Jellyfish", 0.05f},//bomb, worked            
            {"Jellyfish Baby", 1f},

            {"Zombie Beam Miner", 0.7f},
                        
            {"Mushroom Turret", 0.5f},
            {"Mushroom Baby", 0.7f},
            {"Mushroom Roller", 1f},
            
            {"Mage Balloon", 0.8f},
            
            {"Hornet Boss 1", 0.25f},
            {"Hornet Boss 2", 0.25f},

            {"Zombie Myla", 0.5f},
            {"Zombie Miner", 0.5f},

            {"Zombie Hornhead Sp", 0.25f},
            {"Zombie Runner Sp", 0.25f},

            {"Fluke Fly", 0.5f},
            
            {"Bee Hatchling Ambient", 0.4f},
            {"Bee Stinger", 0.35f},
            {"Big Bee", 0.25f},

            {"Lost Kin", 0.05f},//didn't spawn
            {"Mage Orb", 0.01f},
            {"Colosseum Grass Hopper", 0.1f },
                        
            {"Hopper", 0.5f},

            {"Colosseum_Miner", 0.5f},
            {"Colosseum_Shield_Zombie", 0.6f},
            {"Colosseum_Flying_Sentry", 0.4f},

            {"Mega Fat Bee", 0.7f}, 
            {"Bursting Bouncer", 1f},
            {"Blobble", 1f},
            {"Spitting Zombie", 1f},
            {"Bursting Zombie", 1f},
            {"Angry Buzzer", 0.8f},
            {"Lesser Mawlek", 0.7f},
            {"Fly", 0.9f},
            {"Baby Centipede", 0.7f},
            //{"Zombie Spider 2", 0.5f},
            {"Shade Sibling", 0.5f},
            {"Buzzer", 1f},
            //{"Giant Fly", 1f},
            {"Prayer Slug", 1f},
            {"Hatcher", 0.1f},
            {"Fat Fly", 1f},
            {"Grass Hopper", 1f},
            {"Lazy Flyer Enemy", 0.25f},
            {"Garden Zombie", 1f},
            {"Grave Zombie", 0.8f},
            {"Crystal Flyer", 1f},
            {"Spider Mini", 1f},
            {"Centipede Hatcher", 0.1f},
            {"Blow Fly", 0.7f},
            {"Flip Hopper", 1f},
            {"Inflater", 1f},
            {"White Palace Fly", 1f},
            {"Fat Fluke", 0.1f},
            //{"Sly Boss", 0.001f},//didn't die
            {"Colosseum_Worm", 0.01f},

            {"Buzzer R", 0.001f},
            //{"Bee Dropper", 1f} //needs some starting velocity, a collider so it can be hit?, and POOB component
        };





        /*
         add these to the perma skip list
            {"Super Spitter Col", "SuperSpitterCol"},
            {"Giant Fly Col", "GiantFlyCol"},
            {"Buzzer Col", "BuzzerCol"},
            {"Ceiling Dropper Col", "CeilingDropperCol"},
            {"Roller R", "RollerR"},
            {"Spitter R", "SpitterR"},
            {"Lil Jellyfish", "LilJellyfish"},//bomb
            {"Buzzer R", "Buzzer"},
            {"Flukeman Top", "FlukemanTop"},
            {"Flukeman Bot", "FlukemanBot"},
            {"Colosseum_Armoured_Roller R", "ColosseumArmouredRollerR"},
            {"Colosseum_Armoured_Mosquito R", "ColosseumArmouredMosquitoR"},
            {"Super Spitter R", "SuperSpitterR"},
            {"Giant Buzzer Col", "GiantBuzzerCol"},
            {"Plant Turret Right", "PlantShooter"},
            {"Mawlek Turret Ceiling", "MawlekTurret"},
            {"Colosseum_Worm", "ColWorm"},          //look into where these two went?
            {"Colosseum Grass Hopper", "ColHopper"},//look into where these two went?
         */



        //TODO
        public static Dictionary<string, string> DebugTestEnemies = new Dictionary<string, string>()
        {


            //{"Moss Charger", "MossCharger"},
            //{"Mega Moss Charger", "MegaMossCharger"},
            //{"Ghost Warrior Markoth", "GhostMarkoth"},
            //{"Ghost Warrior Xero", "GhostXero"},


            //{"Giant Buzzer", "BigBuzzer"},
            
            //{"Grey Prince", "GreyPrince"},
            //{"Mantis Traitor Lord", "MantisTraitorLord"},
            //{"Grub Mimic", "---"},
            //{"Grub Mimic Bottle", "---"},
            {"Colosseum_Worm", "---"},
            {"Colosseum Grass Hopper", "---"},
            {"Blobble", "---"},
            {"Ghost Warrior Hu", "GhostHu"},

            //{"Zombie Beam Miner Rematch", "MegaBeamMiner"},//busted?

            {"Dung Defender", "DungDefender"},
            {"fluke_baby_02", "JellyCrawler"},
            {"fluke_baby_01", "BlobFlyer"},
            {"fluke_baby_03", "MenderBug"},
            {"Fluke Mother", "FlukeMother"},
            {"Enemy", "WhiteRoyal"},
            {"Royal Gaurd", "RoyalGaurd"},
            {"Zombie Hive", "ZombieHive"},
            //{"Hive Knight", "HiveKnight"},
            {"Grimm Boss", "Grimm"},
            //{"Nightmare Grimm Boss", "NightmareGrimm"},
            //{"False Knight Dream", "FalseKnightDream"},
            {"Dream Mage Lord", "DreamMageLord"},
            //{"Dream Mage Lord Phase2", "DreamMageLordPhase2"},
            {"Lost Kin", "LostKin"},
            {"Zoteling", "Zoteling"},
            {"Zoteling Buzzer", "ZotelingBuzzer"},
            {"Zoteling Hopper", "ZotelingHopper"},
            {"Zote Balloon", "ZoteBalloon"},
            //{"Radiance", "FinalBoss"},
            {"Hollow Knight Boss", "HollowKnight"},
            {"HK Prime", "HollowKnightPrime"},
            {"Pale Lurker", "PaleLurker"},
            {"Oro", "NailBros"},
            {"Mato", "NailBros"},
            {"Sheo Boss", "Paintmaster"},
            {"Fat Fluke", "FatFluke"},
            {"Absolute Radiance", "AbsoluteRadiance"},
            {"Sly Boss", "Nailsage"},
            {"Zote Balloon Ordeal", "ZotelingBalloon"},
            {"Ordeal Zoteling", "OrdealZoteling"},
            {"Zote Salubra", "EggSac"},
            {"Zote Thwomp", "ZoteThwomp"},
            {"Zote Fluke", "Mummy"},
            {"Zote Crew Normal", "ZoteCrewNormal"},
            {"Zote Crew Fat", "ZoteCrewFat"},
            {"Zote Crew Tall", "ZoteCrewTall"},
            {"Hornet Nosk", "HornetNosk"},
            {"Mace Head Bug", "MaceHeadBug"},
            {"Big Centipede Col", "BigCentipede"},
            {"Jelly Egg Bomb", "JellyEggBomb"},
            {"Worm", "ColWorm"},
            {"Bee Dropper", "Pigeon"},


            {"Health Cocoon", "HealthScuttler"},
            {"Abyss Tendril", "AbyssTendril"},
            {"Health Scuttler", "HealthScuttler"},
            {"Orange Scuttler", "OrangeScuttler"},


            {"Laser Turret Frames", "LaserBug"},
            
            //{"Plant Turret", "PlantShooter"}, minor issues, fix later
            //{"Mage Lord", "MageLord"},             //good for now, some placements aren't great but overall doesn't get stuck as much or at all
            //{"Mage Lord Phase2", "MageLordPhase2"},//good for now, some placements aren't great but overall doesn't get stuck as much or at all

            //{"False Knight New", "FalseKnightNew"},//mostly good- shockwave could use a tune up
            //{"Giant Fly", "GiantFly"},//test the boss

            //{"Infected Knight", "InfectedKnight"},//nullrefs -- fix before trying again
            //{"White Defender", "WhiteDefender"},//fix first
            //{"Jellyfish GG", "JellyfishGG"},//needs implementing
            //{"Mega Jellyfish", "MegaJellyfish"},//needs implementing
            
            //{"Mimic Spider", "MimicSpider"}, //needs a timeout on charge -- basically fine
            //{"Jar Collector", "JarCollector"},//final hp test for spawns, but works great
        };


























        public static Dictionary<string, float> GoodForArenaBossWeights = new Dictionary<string, float>()
        {
            {"Zote Boss", 1.0f},
            {"Super Spitter", 1.0f},
            {"Giant Hopper", 1.0f},
            {"Mantis Heavy", 1.0f},
            {"Lesser Mawlek", 1.0f},
            {"Mantis Heavy Flyer", 1.0f},
            {"Ghost Warrior Slug", 1.0f},
            {"Mega Fat Bee", 1.0f},
            {"Lobster", 1.0f},
            {"Mage Knight", 1.0f},
            {"Lancer", 1.0f},
            {"Giant Fly", 1.0f},
            {"Mawlek Body", 1.0f},
            {"False Knight New", 1.0f},
            {"Gorgeous Husk", 1.0f},
            {"Mage Lord", 1.0f},
            {"Mage Lord Phase2", 1.0f},
            {"Great Shield Zombie", 1.0f},
            {"Black Knight", 1.0f},
            {"Jar Collector", 1.0f},
            {"Hornet Boss 1", 1.0f},
            {"Hornet Boss 2", 1.0f},
            {"Moss Knight", 1.0f},
            {"Ghost Warrior No Eyes", 1.0f},
            {"Mantis Traitor Lord", 1.0f},
            {"Moss Knight Fat", 1.0f},
            {"Ghost Warrior Marmu", 1.0f},
            {"Mega Zombie Beam Miner", 1.0f},
            {"Mimic Spider", 1.0f},
            {"Ghost Warrior Galien", 1.0f},
            {"Fluke Mother", 1.0f},
            {"Royal Gaurd", 1.0f},
            {"False Knight Dream", 1.0f},
            {"Lost Kin", 1.0f},
            {"Grey Prince", 1.0f},
            {"Zote Crew Fat", 1.0f},
            {"Hornet Nosk", 1.0f},
            {"Dung Defender", 1.0f},
            {"Hollow Knight Boss", 1.0f},
            {"HK Prime", 1.0f},
            {"Grimm Boss", 1.0f},
        };

        

























        public static Dictionary<string, float> DifficultyWeights = new Dictionary<string, float>()
        {
            {"Health Cocoon", 0.0f},
            {"Health Scuttler", 0.0f},
            {"Orange Scuttler", 0.0f},
            {"Abyss Tendril", 0.0f},


            {"Mosquito", 0.5f},
            {"Colosseum_Armoured_Mosquito", 0.5f},

            {"Colosseum_Armoured_Roller", 0.5f},
            {"Roller", 0.5f},
            {"Roller R", 0.5f},

            {"Super Spitter", 1f},
            {"Super Spitter Col", 0.5f},//didnt spawn
            
            {"Spitter R", 0.5f},
            {"Spitter", 0.5f},

            {"White Defender", 0.5f},//error on spawn
            {"Dung Defender", 0.5f},

            {"Giant Buzzer Col", 0.5f},//coun't hurt
            {"Giant Buzzer", 0.5f},

            {"Zombie Shield", 0.25f},
            {"Zombie Leaper", 0.25f},
            {"Zombie Guard", 0.25f},
            {"Zombie Runner", 0.25f},
            {"Zombie Hornhead", 0.25f},
            {"Zombie Barger", 0.25f},

            {"False Knight Dream", 0.2f},
            {"False Knight New", 0.4f}, ///???? some errors but killable

            {"Royal Zombie Fat", 0.4f},
            {"Royal Zombie", 0.4f},
            {"Royal Zombie Coward", 0.4f},

            {"Gorgeous Husk", 1f},
            {"Ceiling Dropper", 1f},

            {"Ruins Sentry", 0.25f},
            {"Ruins Flying Sentry", 0.25f},
            {"Ruins Flying Sentry Javelin", 0.25f},
            {"Ruins Sentry Fat", 0.25f},

            {"Great Shield Zombie", 0.4f},
            {"Great Shield Zombie bottom", 0.4f},//skip, doesn't spawn
            
            {"Plant Trap", 1f},
            {"Pigeon", 0.1f},

            {"Acid Flyer", 0.25f},
            {"Acid Walker", 0.3f},

            {"Plant Turret", 0.5f},
            {"Plant Turret Right", 0.01f},

            {"Fungoon Baby", 0.5f},
            {"Fungus Flyer", 0.5f},

            {"Zombie Fungus A", 0.5f},
            {"Zombie Fungus B", 0.5f},

            {"Moss Flyer", 1f},
            {"Moss Knight Fat", 1f},
            {"Moss Knight", 1f},
            {"Moss Walker", 1f},
            {"Mossman_Shaker", 1f},
            {"Mossman_Runner", 1f},

            {"Moss Charger", 0.5f},
            {"Mega Moss Charger", 0.25f},

            {"Mantis Heavy", 0.2f},
            {"Mantis Heavy Flyer", 0.2f},
            {"Mantis Flyer Child", 0.2f},//spawned in ground
            {"Mantis", 0.2f},
            {"Mantis Traitor Lord", 0.2f},
            {"Mantis Heavy Spawn", 0.2f},

            {"Ghost Warrior Markoth", 0.15f},
            {"Ghost Warrior Galien", 0.15f},
            {"Ghost Warrior Xero", 0.15f},//killing caused nullrefs
            {"Ghost Warrior Marmu", 0.15f},
            {"Ghost Warrior No Eyes", 0.15f},
            {"Ghost Warrior Hu", 0.15f},
            {"Ghost Warrior Slug", 0.15f},//isn't attacking and drifts left -- movement broken like markoth
            
            {"Zote Boss", 0.5f},// still has white screen issue?

            {"Zoteling", 0.25f},
            {"Zoteling Buzzer", 0.25f},
            {"Zoteling Hopper", 0.25f},
            {"Ordeal Zoteling", 0.25f},

            {"Zote Balloon Ordeal", 0.25f},//skip
            {"Zote Balloon", 0.25f},

            {"Zote Turret", 0.25f},
            {"Zote Salubra", 0.25f},
            {"Zote Thwomp", 0.25f},
            {"Zote Fluke", 0.25f},//add poob

            {"Zote Crew Normal", 0.3f},//spawns in floor
            {"Zote Crew Fat", 0.3f},
            {"Zote Crew Tall", 0.3f},//spawns in floor
            
            {"Jellyfish", 1f},
            {"Lil Jellyfish", 1f},//bomb, worked
            {"Jellyfish GG", 0.5f},
            {"Mega Jellyfish", 0.5f},
            {"Jellyfish Baby", 1f},

            {"Mega Zombie Beam Miner", 0.2f},//nullrefs on spawn
            {"Zombie Beam Miner", 0.7f},
            {"Zombie Beam Miner Rematch", 0.1f},//nullrefs on spawn
            
            {"fluke_baby_02", 0.3f},
            {"fluke_baby_01", 0.3f},//no blue mask
            {"fluke_baby_03", 0.3f},//no explode
            
            {"Mushroom Turret", 1f},//spawned in floor
            {"Mushroom Brawler", 1f},
            {"Mushroom Baby", 1f},
            {"Mushroom Roller", 1f},

            {"Oro", 0.3f},//didn't delete
            {"Mato", 0.3f},//didn't activate or die
            {"Sheo Boss", 0.3f},//didn't die

            {"Hollow Knight Boss", 0.5f},//fell through world
            {"HK Prime", 0.5f},//didn't spawn
            
            {"Radiance", 0.5f},
            {"Absolute Radiance", 0.5f},//nullref
            
            {"Mawlek Turret", 0.5f},
            {"Mawlek Turret Ceiling", 0.5f},//didn't spawn
            
            {"Mage Knight", 1f},
            {"Mage", 1f},//spawned outside arena
            {"Electric Mage", 0.5f},//teleported away
            {"Mage Blob", 1f},
            {"Mage Balloon", 1f},
            {"Mage Lord", 0.25f},//error? fix?
            {"Mage Lord Phase2", 0.25f},//works -- cant find his orb idle spot
            {"Dream Mage Lord", 0.25f},
            {"Dream Mage Lord Phase2", 0.25f},

            {"Hornet Boss 1", 0.5f},
            {"Hornet Boss 2", 0.5f},

            {"Zombie Myla", 0.5f},
            {"Zombie Miner", 0.5f},

            {"Zombie Hornhead Sp", 0.5f},
            {"Zombie Runner Sp", 0.5f},

            {"Fluke Fly", 0.5f},
            {"Flukeman", 0.5f},
            {"Fluke Mother", 0.5f}, //nullref
            
            {"Zombie Hive", 0.5f},
            {"Bee Hatchling Ambient", 0.5f},
            {"Bee Stinger", 0.5f},
            {"Big Bee", 0.5f},

            {"Infected Knight", 0.5f},//didn't spawn
            {"Lost Kin", 0.5f},

            {"Grimm Boss", 0.5f},//teleports into walls
            {"Nightmare Grimm Boss", 0.5f},//stuck on spawn -- still steals HUD -- death doesn't delete
            
            {"Hopper", 0.5f},
            {"Giant Hopper", 0.5f},

            {"Colosseum_Miner", 0.7f},
            {"Colosseum_Shield_Zombie", 0.7f},
            {"Colosseum_Flying_Sentry", 0.7f},

            {"Mega Fat Bee", 1f}, //???? didn't see it spawn
            {"Bursting Bouncer", 1f},//corpse didnt explode
            {"Blobble", 1f},
            {"Spitting Zombie", 1f},
            {"Bursting Zombie", 1f},
            {"Angry Buzzer", 1f},
            {"Lesser Mawlek", 1f},
            {"Fly", 1f},
            {"Buzzer R", 1f},
            {"Baby Centipede", 0.5f},
            {"Zombie Spider 2", 0.5f},//nullref
            {"Tiny Spider", 1f},
            {"Shade Sibling", 1f},
            {"Crawler", 1f},
            {"Buzzer", 1f},
            {"Lobster", 1f},//was placed inside floor
            {"Lancer", 1f},//error, spawned stuck and couldn't die as well
            {"Climber", 1f},
            {"Mender Bug", 0.25f},
            {"Giant Fly", 1f}, //spawn error, needs fix
            {"Mawlek Body", 1f},//spawn errror            
            {"Black Knight", 1f},//??? still yeets into space
            {"Jar Collector", 1f},//jars are spawning inactive enemies -- spawned in floor
            {"Prayer Slug", 1f},
            {"Blocker", 1f},
            {"Hatcher", 1f},
            {"Egg Sac", 1f},//didnt transfer item
            {"Fat Fly", 1f},
            {"Grass Hopper", 1f},
            {"Lazy Flyer Enemy", 0.25f},
            {"Fung Crawler", 1f},
            {"Garden Zombie", 1f},
            {"Grave Zombie", 1f},
            {"Crystal Crawler", 1f},
            {"Crystal Flyer", 1f},
            {"Crystallised Lazer Bug", 1f},
            {"Mines Crawler", 1f},
            {"Spider Mini", 1f},
            {"Centipede Hatcher", 1f},
            {"Mimic Spider", 1f},//nullref
            {"Slash Spider", 1f},
            {"Spider Flyer", 1f},
            {"Blow Fly", 1f},
            {"Abyss Crawler", 1f},
            {"Parasite Balloon", 1f},
            {"Flip Hopper", 1f},
            {"Inflater", 1f},
            {"White Palace Fly", 1f},
            {"Enemy", 0.5f},
            {"Royal Gaurd", 1f},//fix boomerang -- elite not spawning
            {"Hive Knight", 1f},//not spawning
            {"Grey Prince", 1f},
            {"Pale Lurker", 1f},
            {"Fat Fluke", 1f},
            {"Sly Boss", 0.7f},//didn't die
            {"Hornet Nosk", 0.3f},
            {"Mace Head Bug", 1f},
            {"Big Centipede Col", 1f},
            {"Laser Turret Frames", 1f},//nullref
            {"Jelly Egg Bomb", 1f},//not loaded?
            {"Worm", 1f}, //need flipped
            {"Colosseum_Worm", 1f},
            {"Zombie Spider 1", 0.5f},

            {"Hatcher Baby", 0.01f},
            {"Corpse Garden Zombie", 0.01f},
            {"Colosseum_Armoured_Roller R", 0.5f},  //dont spawn
            {"Flukeman Top", 0.01f},//dont spawn
            {"Flukeman Bot", 0.01f},//don't spawn
            {"Colosseum Grass Hopper", 0.5f},//??? didn't spawn
            {"Mawlek Col", 0.5f},//?? didn't spawn
            {"Ceiling Dropper Col", 0.5f},//didn't spawn
            {"Giant Fly Col", 0.5f},//didn't spawn
            {"Buzzer Col", 0.5f},//didn't spawn
            {"Colosseum_Armoured_Mosquito R", 1f},//dont spawn
            {"Super Spitter R", 1f}, //dont spawn

            {"Bee Dropper", 1f}, //needs some starting velocity, a collider so it can be hit?, and POOB component


            {"Flamebearer Small", 0.1f},
            {"Flamebearer Med", 0.1f},
            {"Flamebearer Large", 0.1f},
        };

    }
}





















//protected virtual void CheckIfIsBattleEnemy(GameObject sceneObject)
//{
//    if (EnemyHealthManager == null)
//        return;

//    if (!IsBattleEnemy)
//    {
//        if (sceneObject.GetComponent<BattleManagedObject>())
//            IsBattleEnemy = true;

//        if (!IsBattleEnemy)
//        {
//            IsBattleEnemy = IsBoss;

//            if (!IsBattleEnemy)
//            {
//                IsBattleEnemy = ScenePath.Split('/').Any(x => BattleManager.battleControllers.Any(y => x.Contains(y)));
//            }

//            if (!IsBattleEnemy)
//            {
//                bool hasScene = MetaDataTypes.BattleEnemies.TryGetValue("ANY", out var enemies);
//                if (hasScene)
//                {
//                    IsBattleEnemy = enemies.Contains(DatabaseName);
//                }

//                if (!IsBattleEnemy)
//                {
//                    hasScene = MetaDataTypes.BattleEnemies.TryGetValue(SceneName, out var enemies2);
//                    if (hasScene)
//                    {
//                        IsBattleEnemy = enemies2.Contains(ScenePath);
//                    }
//                }
//            }
//        }
//    }
//}




//protected virtual bool SetupDatabaseRefs(GameObject sceneObject, EnemyRandomizerDatabase database)
//{
//    if (!EnemyRandomizerDatabase.IsDatabaseObject(sceneObject))
//    {
//        HasData = false;
//    }
//    else
//    {
//        DatabaseName = EnemyRandomizerDatabase.ToDatabaseKey(sceneObject.name);
//        if (!string.IsNullOrEmpty(DatabaseName))
//        {
//            HasData = IsObjectInDatabase(database);
//            if (HasData)
//            {
//                SetupObjectType(DatabaseName, database);
//            }
//        }
//    }

//    return HasData;
//}




//protected virtual void SetupRandoProperties(GameObject sceneObject)
//{
//    IsTinker = sceneObject.GetComponentInChildren<TinkEffect>() != null;

//    if (EnemyHealthManager == null)
//    {
//        if (IsTinker)
//            IsInvincible = true;
//        return;
//    }

//    bool isFlyingFromComponents =
//        (sceneObject.GetComponent<Rigidbody2D>() != null && sceneObject.GetComponent<Rigidbody2D>().gravityScale == 0) &&
//        (sceneObject.GetComponent<Climber>() == null);

//    IsBoss = MetaDataTypes.Bosses.Contains(DatabaseName);
//    IsFlying = isFlyingFromComponents || MetaDataTypes.Flying.Contains(DatabaseName);
//    IsCrawling = sceneObject.GetComponent<Crawler>() != null || MetaDataTypes.Crawling.Contains(DatabaseName);
//    IsClimbing = sceneObject.GetComponent<Climber>() != null || MetaDataTypes.Climbing.Contains(DatabaseName);
//    IsMobile = !MetaDataTypes.Static.Contains(DatabaseName);
//    IsSmasher = sceneObject.GetDirectChildren().Any(x => x.name == "Smasher");//can use this child object layer to enable smashing of platforms/walls
//    IsSummonedByEvent = MetaDataTypes.IsSummonedByEvent.Contains(DatabaseName);
//    IsEnemySpawner = MetaDataTypes.SpawnerEnemies.Contains(DatabaseName);
//    CheckIfIsBattleEnemy(sceneObject);
//}



//protected virtual void SetupComponentRefs(GameObject sceneObject)
//{

//    //RandoObject = sceneObject.GetComponent<ManagedObject>();
//    //BattleRandoObject = sceneObject.GetComponent<BattleManagedObject>();

//    EnemyHealthManager = sceneObject.GetComponent<HealthManager>();
//    if (EnemyHealthManager != null)
//    {
//        DefaultHP = EnemyHealthManager.hp;
//        IsInvincible = EnemyHealthManager.IsInvincible;

//        var battleScene = EnemyHealthManager.GetBattleScene();
//        if (battleScene != null)
//        {
//            IsBattleEnemy = true;
//        }

//        GeoManager = new Geo(this);
//    }

//    HeroDamage = sceneObject.GetComponent<DamageHero>();
//    if (HeroDamage != null)
//    {
//        DamageDealt = HeroDamage.damageDealt;
//    }

//    EnemyDamage = sceneObject.GetComponent<DamageEnemies>();
//    if (EnemyDamage != null)
//    {
//        EnemyDamageDealt = EnemyDamage.damageDealt;
//    }

//    Walker = sceneObject.GetComponent<Walker>();
//    IsWalker = Walker != null;

//    PhysicsBody = sceneObject.GetComponent<Rigidbody2D>();

//    //check if it's disabled/completed already
//    var pbi = sceneObject.GetComponent<PersistentBoolItem>();
//    if (pbi != null)
//    {
//        SceneSaveData = pbi;

//        if (pbi.isActiveAndEnabled)
//        {
//            IsDisabledBySavedGameState = pbi.persistentBoolData.activated;
//        }
//        else
//        {
//            var data = global::SceneData.instance.FindMyState(pbi.persistentBoolData);
//            if (data != null)
//                IsDisabledBySavedGameState = data.activated;
//            else
//                IsDisabledBySavedGameState = false;
//        }
//    }

//    Collider = sceneObject.GetComponent<Collider2D>();
//    Sprite = sceneObject.GetComponent<tk2dSprite>();

//    DeathEffects = sceneObject.GetComponent<EnemyDeathEffects>();
//}






//protected virtual void SetupTransformValues(GameObject sceneObject)
//{
//    SizeScale = 1f;

//    if (MetaDataTypes.HasUniqueSizeEnemies.Contains(DatabaseName))
//    {
//        SetSizeFromUniqueObject(sceneObject);
//    }
//    else
//    {
//        SetSizeFromComponents(sceneObject);
//    }

//    UpdateTransformValues();
//}



//public virtual void UpdateTransformValues()
//{
//    if (Source != null)
//    {
//        ObjectPosition = Source.transform.position;
//        ObjectScale = Source.transform.localScale;
//        Rotation = Source.transform.localEulerAngles.z;
//    }
//}


//protected virtual void SetupItemValues(GameObject sceneObject)
//{
//    //check and see if this enemy would have dropped an item
//    HasAvailableItem = sceneObject.GetComponent<PreInstantiateGameObject>();
//    if (!HasAvailableItem)
//    {
//        var ede = sceneObject.GetComponent<EnemyDeathEffects>();
//        if (ede != null)
//        {
//            var corpse = ede.GetCorpseFromDeathEffects();
//            if (corpse != null)
//            {
//                if (corpse.GetComponent<PreInstantiateGameObject>())
//                {
//                    var go = corpse.GetComponent<PreInstantiateGameObject>().InstantiatedGameObject;
//                    if (go != null && go.name.Contains("Shiny Item"))
//                    {
//                        var go_pbi = go.GetComponent<PersistentBoolItem>();
//                        if (go_pbi != null && !go_pbi.persistentBoolData.activated)
//                        {
//                            HasAvailableItem = true;
//                            AvailableItem = go;
//                        }
//                    }
//                }
//            }
//            else
//            {
//                if (EnemyHealthManager != null)
//                    Debug.LogError(ScenePath + " has no corpse!");
//            }
//        }
//    }
//}



//protected virtual bool IsVisibleNow()
//{
//    bool isVisible = false;
//    GameObject sceneObject = Source;
//    if (Source == null)
//    {
//        //Dev.Log($"{ScenePath} source has been deleted so this object can never be active!");
//        return false;
//    }

//    if (sceneObject.gameObject.activeSelf)
//    {
//        isVisible = sceneObject.gameObject.activeSelf;
//        //Dev.Log($"{ScenePath} has self active state: {isVisible}");

//        //might not actually be...
//        if (isVisible)
//        {
//            //count "out of bounds" enemies as not visible

//            if (BlackBorders.Value != null && BlackBorders.Value.Count > 0)
//            {
//                var leftRight = BlackBorders.Value.Where(x => x.transform.localScale.x == 20);
//                var topBot = BlackBorders.Value.Where(x => x.transform.localScale.y == 20);

//                var xmin = leftRight.Min(o => (o.transform.position.x - 10f));
//                var xmax = leftRight.Max(o => (o.transform.position.x + 10f));
//                var ymin = topBot.Min(o => (o.transform.position.y - 10f));
//                var ymax = topBot.Max(o => (o.transform.position.y + 10f));

//                //Dev.Log($"{ScenePath} pos:{sceneObject.transform.position}");
//                //Dev.Log($"{ScenePath} BOUNDS[ xmin:{xmin} xmax:{xmax} ymin:{ymin} ymax:{ymax}]");

//                if (sceneObject.transform.position.x < xmin)
//                    isVisible = false;
//                else if (sceneObject.transform.position.x > xmax)
//                    isVisible = false;
//                else if (sceneObject.transform.position.y < ymin)
//                    isVisible = false;
//                else if (sceneObject.transform.position.y > ymax)
//                    isVisible = false;

//                //if(isVisible)
//                //    Dev.Log($"{ScenePath} is in bounds!");
//                //else
//                //    Dev.Log($"{ScenePath} is out of bounds!");
//            }
//            else
//            {
//                //Dev.Log($"{ScenePath} BOUNDS NOT READY YET! Cannot check if visible");
//                //not visible yet
//                return false;
//            }
//        }

//        //still might not actually be...
//        if (isVisible)
//        {
//            Collider2D collider = sceneObject.GetComponent<Collider2D>();
//            MeshRenderer renderer = sceneObject.GetComponent<MeshRenderer>();
//            if (collider != null || renderer != null)
//            {
//                if (collider != null && renderer == null)
//                    isVisible = collider.enabled;
//                else if (collider == null && renderer != null)
//                    isVisible = renderer.enabled;
//                else //if (collider != null && renderer != null)
//                    isVisible = collider.enabled && renderer.enabled;
//            }

//            //if(!isVisible)
//            //    Dev.Log($"{ScenePath} has disabled colliders or renderers and is considered invisible!");
//        }
//    }
//    return isVisible;
//}




//public virtual bool CheckIfIsActiveAndVisible()
//{
//    if (Source == null)
//        return false;

//    bool IsVisible = IsVisibleNow();
//    IsActive = IsVisible && !IsDisabledBySavedGameState;
//    return IsVisible && IsActive;
//}





//public bool IsTemporarilyInactive()
//{
//    return HasData && !IsAReplacementObject && Source.activeInHierarchy && EnemyHealthManager != null && !IsDisabledBySavedGameState && !CheckIfIsActiveAndVisible();
//}





//public bool IsBattleInactive()
//{
//    return HasData && !IsAReplacementObject && IsBattleEnemy && !Source.activeInHierarchy && EnemyHealthManager != null && !IsDisabledBySavedGameState && !CheckIfIsActiveAndVisible();
//}





//public virtual bool CanProcessObject()
//{
//    if (!HasData)
//        return false;

//    if (IsInvalidObject)
//        return false;

//    if (!IsActive)
//    {
//        if (ObjectType != PrefabType.Effect)
//            return false;
//    }

//    if (IsAReplacementObject)
//        return false;

//    if (!CheckIfIsActiveAndVisible())
//    {
//        return false;
//    }

//    return true;
//}




//public virtual void MarkObjectAsReplacement(ObjectMetadata oldObject)
//{
//    ObjectThisReplaced = oldObject;

//    if (RandoObject == null)
//    {
//        if (oldObject.IsBattleEnemy)
//        {
//            BattleRandoObject = Source.AddComponent<BattleManagedObject>();
//            RandoObject = BattleRandoObject;
//        }
//        else
//        {
//            RandoObject = Source.AddComponent<ManagedObject>();
//        }

//        RandoObject.Setup(oldObject);
//    }

//    RandoObject.replaced = true;
//}