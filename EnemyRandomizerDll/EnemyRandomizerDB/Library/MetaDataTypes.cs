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
            "Mega Moss Charger"
        };

        public static List<string> InGroundEnemy = new List<string>()
        {
            "Pigeon",
            "Dung Defender",
            "White Defender",
            "Plant Trap",
            "Moss Charger",
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
            "Radiance",
            "Giant Buzzer",//temporarily broken
            "Giant Buzzer Col",//temporarily broken
            "Corpse Garden Zombie", //don't spawn this, it's just a corpse
            "Super Spitter Col"                    ,
            "Giant Fly Col"                        ,
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
            "Colosseum_Worm"                       ,
            "Colosseum Grass Hopper"               ,
            "White Defender"               , //until he's fixed
            //"Mage Lord"               , //until he's fixed
            //"Mage Lord Phase2"               , //until he's fixed
            "Black Knight",
            "Ghost Warrior No Eyes",
            "Ghost Warrior Markoth",
            "Infected Knight",
            "Fluke Mother",
            "Nightmare Grimm Boss",
            "Mato",
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



        //make markoth shield start rotating
        public static Dictionary<string, bool> SafeForArenas = new Dictionary<string, bool>()
        {
            {"Health Cocoon", false},
            {"Health Scuttler", false},
            {"Orange Scuttler", false},
            {"Abyss Tendril", false},
            {"Flamebearer Small", false},//these work, but they're annoying and don't stay in the arenas
            {"Flamebearer Med", false},  //these work, but they're annoying and don't stay in the arenas
            {"Flamebearer Large", false},//these work, but they're annoying and don't stay in the arenas
            {"Mosquito", true},
            {"Colosseum_Armoured_Roller", true},
            {"Colosseum_Miner", true},
            {"Zote Boss", true},// TEST FIXES
            {"Bursting Bouncer", true},//corpse didnt explode
            {"Super Spitter", true},
            {"Super Spitter Col", false},//didnt spawn
            {"Giant Fly Col", false},//didn't spawn
            {"Colosseum_Shield_Zombie", true},
            {"Buzzer Col", false},//didn't spawn
            {"Blobble", true},
            {"Colosseum_Armoured_Mosquito", true},
            {"Colosseum_Flying_Sentry", true},
            {"Hopper", true},
            {"Giant Hopper", true},
            {"Ceiling Dropper Col", false},//didn't spawn
            {"Colosseum_Worm", false},//?? didn't spawn
            {"Spitting Zombie", true},
            {"Bursting Zombie", true},
            {"Angry Buzzer", true},
            {"Mawlek Col", false},//?? didn't spawn
            {"Mantis Heavy", true},
            {"Lesser Mawlek", true},
            {"Mantis Heavy Flyer", true},
            {"Colosseum Grass Hopper", false},//??? didn't spawn
            {"Fly", true},
            {"Roller", true},
            {"Hatcher Baby", false},
            {"Roller R", false},
            {"Spitter R", true},
            {"Buzzer R", false},
            {"Mossman_Runner", true},
            {"Jellyfish", true},
            {"Lil Jellyfish", false},//bomb, worked
            {"Mantis Flyer Child", false},//spawned in ground
            {"Ghost Warrior Slug", false},//isn't attacking and drifts left -- movement broken like markoth
            {"Corpse Garden Zombie", false},
            {"Baby Centipede", true},
            {"Zombie Spider 2", false},//nullref
            {"Zombie Spider 1", false},//didn't spawn
            {"Tiny Spider", true},
            {"Shade Sibling", true},
            {"Flukeman Top", false},//dont spawn
            {"Flukeman Bot", false},//don't spawn
            {"White Defender", false},//error on spawn


            {"Jellyfish GG", false},
            {"Colosseum_Armoured_Roller R", false},  //dont spawn
            {"Colosseum_Armoured_Mosquito R", false},//dont spawn
            {"Super Spitter R", false}, //dont spawn
            {"Crawler", true},
            {"Buzzer", true},
            {"Giant Buzzer Col", true},//coun't hurt
            {"Mega Fat Bee", true}, //???? didn't see it spawn
            {"Lobster", true},//was placed inside floor
            {"Mage Knight", true},
            {"Mage", false},//spawned outside arena
            {"Electric Mage", false},//teleported away
            {"Mage Blob", true},
            {"Lancer", false},//error, spawned stuck and couldn't die as well
            {"Climber", true},
            {"Zombie Runner", true},
            {"Mender Bug", false},
            {"Spitter", true},
            {"Zombie Hornhead", true},
            {"Giant Fly", false}, //spawn error, needs fix
            {"Zombie Barger", true},
            {"Mawlek Body", false},//spawn errror
            {"False Knight New", true}, ///???? some errors but killable
            {"Prayer Slug", true},
            {"Blocker", true},
            {"Zombie Shield", true},
            {"Hatcher", true},
            {"Zombie Leaper", true},
            {"Zombie Guard", true},
            {"Zombie Myla", true},
            {"Egg Sac", false},//didnt transfer item
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
            {"Mage Lord", false},//error? fix?
            {"Mage Lord Phase2", true},//works -- cant find his orb idle spot
            {"Great Shield Zombie", true},
            {"Great Shield Zombie bottom", false},//skip, doesn't spawn
            {"Black Knight", false},//??? still yeets into space
            {"Jar Collector", false},//jars are spawning inactive enemies -- spawned in floor
            {"Moss Walker", true},
            {"Plant Trap", true},
            {"Mossman_Shaker", true},
            {"Pigeon", false},
            {"Hornet Boss 1", true},
            {"Acid Flyer", false},
            {"Moss Charger", false},
            {"Acid Walker", false},
            {"Plant Turret", false},
            {"Plant Turret Right", false},
            {"Fat Fly", true},
            {"Giant Buzzer", false},
            {"Moss Knight", true},
            {"Grass Hopper", true},
            {"Lazy Flyer Enemy", false},
            {"Mega Moss Charger", false},



            {"Ghost Warrior No Eyes", false},
            {"Fungoon Baby", true},
            {"Mushroom Turret", false},//spawned in floor
            {"Fungus Flyer", true},
            {"Zombie Fungus B", true},
            {"Fung Crawler", true},
            {"Mushroom Brawler", true},
            {"Mushroom Baby", true},
            {"Mushroom Roller", true},
            {"Zombie Fungus A", true},
            {"Mantis", true},
            {"Ghost Warrior Hu", false},
            {"Jellyfish Baby", false},
            {"Moss Flyer", true},
            {"Garden Zombie", true},
            {"Mantis Traitor Lord", true},
            {"Moss Knight Fat", true},
            {"Mantis Heavy Spawn", true},
            {"Ghost Warrior Marmu", true},
            {"Mega Jellyfish", false},
            {"Ghost Warrior Xero", false},//killing caused nullrefs
            {"Grave Zombie", true},
            {"Crystal Crawler", true},
            {"Zombie Miner", true},
            {"Crystal Flyer", true},
            {"Crystallised Lazer Bug", false},
            {"Mines Crawler", true},
            {"Mega Zombie Beam Miner", true},//nullrefs on spawn
            {"Zombie Beam Miner", true},
            {"Zombie Beam Miner Rematch", true},//nullrefs on spawn
            {"Spider Mini", true},
            {"Zombie Hornhead Sp", true},
            {"Zombie Runner Sp", true},
            {"Centipede Hatcher", true},
            {"Mimic Spider", true},//nullref
            {"Slash Spider", true},
            {"Spider Flyer", true},
            {"Ghost Warrior Galien", false},
            {"Blow Fly", true},
            {"Bee Hatchling Ambient", true},
            {"Ghost Warrior Markoth", false},
            {"Hornet Boss 2", true},
            {"Abyss Crawler", true},
            {"Infected Knight", true},//didn't spawn
            {"Parasite Balloon", true},
            {"Mawlek Turret", true},
            {"Mawlek Turret Ceiling", false},//didn't spawn
            {"Flip Hopper", true},
            {"Inflater", true},
            {"Fluke Fly", true},
            {"Flukeman", true},
            {"Dung Defender", false},
            {"fluke_baby_02", false},
            {"fluke_baby_01", false},//no blue mask
            {"fluke_baby_03", false},//no explode
            {"Fluke Mother", false}, //nullref
            {"White Palace Fly", true},
            {"Enemy", true},
            {"Royal Gaurd", true},//fix boomerang -- elite not spawning
            {"Zombie Hive", true},
            {"Bee Stinger", true},
            {"Big Bee", true},
            {"Hive Knight", false},//not spawning
            {"Grimm Boss", false},//teleports into walls
            {"Nightmare Grimm Boss", false},//stuck on spawn -- still steals HUD -- death doesn't delete
            {"False Knight Dream", true},
            {"Dream Mage Lord", false},
            {"Dream Mage Lord Phase2", false},
            {"Lost Kin", true},
            {"Zoteling", true},
            {"Zoteling Buzzer", true},
            {"Zoteling Hopper", true},
            {"Zote Balloon", false},
            {"Grey Prince", true},
            {"Radiance", false},
            {"Hollow Knight Boss", false},//fell through world
            {"HK Prime", false},//didn't spawn
            {"Pale Lurker", true},
            {"Oro", false},//didn't delete
            {"Mato", false},//didn't activate or die
            {"Sheo Boss", false},//didn't die
            {"Fat Fluke", true},
            {"Absolute Radiance", false},//nullref
            {"Sly Boss", false},//didn't die
            {"Zote Turret", true},
            {"Zote Balloon Ordeal", false},//skip
            {"Ordeal Zoteling", false},
            {"Zote Salubra", false},
            {"Zote Thwomp", true},
            {"Zote Fluke", false},//add poob
            {"Zote Crew Normal", false},//spawns in floor
            {"Zote Crew Fat", true},
            {"Zote Crew Tall", false},//spawns in floor
            {"Hornet Nosk", true},
            {"Mace Head Bug", false},
            {"Big Centipede Col", false},
            {"Laser Turret Frames", false},//nullref
            {"Jelly Egg Bomb", false},//not loaded?
            {"Worm", false}, //need flipped
            {"Bee Dropper", false} //needs some starting velocity, a collider so it can be hit?, and POOB component
        };



        public static Dictionary<string, float> RNGWeights = new Dictionary<string, float>()
        {
            {"Health Cocoon", 0.01f},
            {"Health Scuttler", 0.01f},
            {"Orange Scuttler", 0.01f},
            {"Abyss Tendril", 0.01f},

            {"Flamebearer Small", 0.1f},//these work, but they're annoying and don't stay in the arenas
            {"Flamebearer Med", 0.1f},  //these work, but they're annoying and don't stay in the arenas
            {"Flamebearer Large", 0.1f},//these work, but they're annoying and don't stay in the arenas

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

            {"Hatcher Baby", 0.01f},
            {"Corpse Garden Zombie", 0.01f},
            {"Colosseum_Armoured_Roller R", 0.5f},  //dont spawn
            {"Flukeman Top", 0.01f},//dont spawn
            {"Flukeman Bot", 0.01f},//don't spawn
            {"Zombie Spider 1", 0.5f},//didn't spawn
            {"Colosseum Grass Hopper", 0.5f},//??? didn't spawn
            {"Mawlek Col", 0.5f},//?? didn't spawn
            {"Ceiling Dropper Col", 0.5f},//didn't spawn
            {"Colosseum_Worm", 0.5f},//?? didn't spawn
            {"Giant Fly Col", 0.5f},//didn't spawn
            {"Buzzer Col", 0.5f},//didn't spawn
            {"Colosseum_Armoured_Mosquito R", 1f},//dont spawn
            {"Super Spitter R", 1f}, //dont spawn

            {"Bee Dropper", 1f} //needs some starting velocity, a collider so it can be hit?, and POOB component
        };














        public static Dictionary<string, float> PossibleEnemiesFromSpawner = new Dictionary<string, float>()
        {
            {"Orange Scuttler", 0.5f},
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
            {"Moss Knight", 1f},
            {"Mossman_Shaker", 1f},
            {"Mossman_Runner", 1f},
                        
            {"Zote Boss", 0.5f},// still has white screen issue?

            {"Zoteling", 0.25f},
            {"Zoteling Buzzer", 0.25f},
            {"Zoteling Hopper", 0.25f},
            {"Ordeal Zoteling", 0.25f},

            {"Zote Crew Normal", 0.3f},//spawns in floor
            {"Zote Crew Fat", 0.3f},
            
            {"Lil Jellyfish", 1f},//bomb, worked            
            {"Jellyfish Baby", 1f},

            {"Zombie Beam Miner", 0.7f},
                        
            {"Mushroom Turret", 1f},
            {"Mushroom Baby", 1f},
            {"Mushroom Roller", 1f},
            
            {"Mage Balloon", 1f},
            
            {"Hornet Boss 1", 0.5f},

            {"Zombie Myla", 0.5f},
            {"Zombie Miner", 0.5f},

            {"Zombie Hornhead Sp", 0.5f},
            {"Zombie Runner Sp", 0.5f},

            {"Fluke Fly", 0.5f},
            
            {"Bee Hatchling Ambient", 0.5f},
            {"Bee Stinger", 0.5f},
            {"Big Bee", 0.5f},

            {"Infected Knight", 0.5f},//didn't spawn
                        
            {"Hopper", 0.5f},

            {"Colosseum_Miner", 0.7f},
            {"Colosseum_Shield_Zombie", 0.7f},
            {"Colosseum_Flying_Sentry", 0.7f},

            {"Mega Fat Bee", 1f}, 
            {"Bursting Bouncer", 1f},
            {"Blobble", 1f},
            {"Spitting Zombie", 1f},
            {"Bursting Zombie", 1f},
            {"Angry Buzzer", 1f},
            {"Lesser Mawlek", 1f},
            {"Fly", 1f},
            {"Buzzer R", 1f},
            {"Baby Centipede", 10.0f},
            {"Zombie Spider 2", 0.5f},//nullref
            {"Tiny Spider", 1f},
            {"Shade Sibling", 1f},
            {"Buzzer", 1f},
            {"Giant Fly", 1f}, //spawn error, needs fix
            {"Prayer Slug", 1f},
            {"Hatcher", 0.1f},
            {"Fat Fly", 1f},
            {"Grass Hopper", 1f},
            {"Lazy Flyer Enemy", 0.25f},
            {"Garden Zombie", 1f},
            {"Grave Zombie", 1f},
            {"Crystal Flyer", 1f},
            {"Spider Mini", 1f},
            {"Centipede Hatcher", 0.1f},
            {"Blow Fly", 1f},
            {"Flip Hopper", 1f},
            {"Inflater", 1f},
            {"White Palace Fly", 1f},
            {"Fat Fluke", 0.1f},
            {"Sly Boss", 0.01f},//didn't die

            {"Bee Dropper", 1f} //needs some starting velocity, a collider so it can be hit?, and POOB component
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




        public static Dictionary<string, string> DebugTestEnemies = new Dictionary<string, string>()
        {
            {"Zombie Spider 2", "ZombieSpider2"},
            {"Zombie Spider 1", "ZombieSpider1"},

            //{"White Defender", "WhiteDefender"},//fix first
            //{"Jellyfish GG", "JellyfishGG"},//needs implementing

            {"Mage Knight", "MageKnight"},

            {"Mage", "Mage"},
            {"Electric Mage", "ElectricMage"},

            //{"Giant Fly", "GiantFly"},//test the boss
            {"Mawlek Body", "Mawlek"}, //test -- should be ok now
            {"False Knight New", "FalseKnightNew"},
            {"Blocker", "Blocker"},
            {"Mage Lord", "MageLord"},
            {"Mage Lord Phase2", "MageLordPhase2"},
            {"Black Knight", "BlackKnight"},
            {"Jar Collector", "JarCollector"},
            {"Plant Trap", "SnapperTrap"},
            {"Moss Charger", "MossCharger"},
            {"Plant Turret", "PlantShooter"},
            {"Giant Buzzer", "BigBuzzer"},
            {"Mega Moss Charger", "MegaMossCharger"},



            {"Ghost Warrior No Eyes", "GhostNoEyes"},
            {"Mushroom Turret", "MushroomTurret"},
            {"Mushroom Roller", "MushroomRoller"},
            {"Ghost Warrior Hu", "GhostHu"},
            {"Mantis Traitor Lord", "MantisTraitorLord"},
            {"Ghost Warrior Marmu", "GhostMarmu"},
            //{"Mega Jellyfish", "MegaJellyfish"},//needs implementing
            {"Ghost Warrior Xero", "GhostXero"},
            {"Crystallised Lazer Bug", "LaserBug"},
            {"Mega Zombie Beam Miner", "MegaBeamMiner"},
            {"Zombie Beam Miner Rematch", "MegaBeamMiner"},
            {"Zombie Hornhead Sp", "SpiderCorpse"},
            {"Zombie Runner Sp", "SpiderCorpse"},
            {"Centipede Hatcher", "CentipedeHatcher"},
            {"Mimic Spider", "MimicSpider"},
            {"Ghost Warrior Galien", "GhostGalien"},
            {"Ghost Warrior Markoth", "GhostMarkoth"},
            {"Infected Knight", "InfectedKnight"},
            {"Mawlek Turret", "MawlekTurret"},
            {"Dung Defender", "DungDefender"},
            {"fluke_baby_02", "JellyCrawler"},
            {"fluke_baby_01", "BlobFlyer"},
            {"fluke_baby_03", "MenderBug"},
            {"Fluke Mother", "FlukeMother"},
            {"White Palace Fly", "PalaceFly"},
            {"Enemy", "WhiteRoyal"},
            {"Royal Gaurd", "RoyalGaurd"},
            {"Zombie Hive", "ZombieHive"},
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
            {"Bee Dropper", "Pigeon"},


            {"Health Cocoon", "HealthScuttler"},
            {"Abyss Tendril", "AbyssTendril"},
            {"Health Scuttler", "HealthScuttler"},
            {"Orange Scuttler", "OrangeScuttler"},
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