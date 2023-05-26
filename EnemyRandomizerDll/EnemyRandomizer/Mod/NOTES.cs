
/* CURRENT TODO
 * 
 * 
 * MAKE SMASHER REPLACEMENT LIST
 * 
 * Fix HK prime taking UI    
 * Fix NKG death removing UI   
 * Uumuu isn't attacking   --- WILL DO LATER
 * Fix enemies that replace in-ground/on-ground enemies to  -- SORTA?
 * Fix? zote dying removing UI (and white screen)  -- FIXED?
 * Scale projectile size too 
 * Fix white palace guard getting stuck after throwing his weapon  -- FIXED?
 * Fix watcher knights ascending into space  -- FIXED?
 * NKG dying stole UI?  -- FIXED?
 * THK spawned in the ground in the white palace gate scene
 * Some explosive bubble eggs didn't get randomized
 * Add a check on traitor lord so he doesn't ground slam w/e shade cloak  -- FIXED?
 * Fix radiance -- WILL DO LATER, DISABLED FOR NOW
 * Fix THK credits  -- FIXED?
 * Check if HK Prime makes UI go away  -- FIXED?
 * Sheo was put in the floor outside colo, check on that  -- FIXED?
 * 
 * Fix bird replacements from being put in the floor-- FIXED?
 * Fix infection dropping fly that didn't explode on death-- FIXED?
 * Edit marmu's teleport to not per marmu inside walls-- FIXED?
 * Fix plant turret not shooting projectiles
 * Fix the zote vengefly boss room -- add to logical skip, allow the venge fly summons to get randomized
 * Finish adding enemy pogo replacements--- TODO: add tween to walking ones
 * Fix pale lurker so it doesn't get stuck on corners--LATER
 * Fix pale lurker so it the attacks are kill-able by the knight--LATER
 * Fix grimm spawn so it doesn't trigger boss music-- FIXED?
 * Get the fart sound from fly corpse death--LATER
 * Fix nuke fart scaling issue from mushroom farting bug--LATER
 * Add some kind of pre-check in for randomization to see if the enemy has a valid, non-insta-death placement before trying--LATER
 * Add a more sophisticated check for wall/ceiling enemies to search for placements not on slopes (or use the slope normals)--LATER
 * 
 * 
 * Make custom colo logic -- TODO !!!!!!!!!!!!!!!!!!!!!
 * 
 * Fix ghost boss replacements, and in general boss replacements, so they trigger their bools
 *   
 * add lumaflies, bomb eggs, and more to the randomize hazard list -- SOME MISSING?
 * re-balance the random scale probabilities -- LATER
 * fix traitor lord from jumping into ceilings (test?)-- FIXED?
 * fix mender bug not being killable --?????
 * fix hornet 2 from not spawning correctly-- FIXED?
 * collector stuck in floor -- need some kind of fix --????
 * fix blugg items-- FIXED?
 * fix geo randomizer golden husk not dropping item --NO IDEA. TEST THIS
 * fix bee/breaking walls in hive-- FIXED?
 * fix hazard randos not doing all wp saws + making pits in deepnest --LATER
 * fix boss-geo replacement and other rando check interactions--???????????? NO IDEA
 * 
 * 
 * 
 * "scale down" the difficulty of enemise that replace the pidgeons/fluke eggs -- NEED TO DO THIS
 * 
 * 
 * Add blue/orange scuttlers to the rando list-- FIXED?
 * Add mace bug to the rando list--LATER
 * Replace mace bug on false knight's staff with another enemy for luls--LATER
 * DISABLE CG TINKERS-- FIXED?
 * DISABLE WATCHER KNIGHT TINKERS (make their load states much faster)
 * NERF ORDEAL ZOTELING DAMAGE-- FIXED?
 * add poob to gruz mother-- FIXED?
 * make zote balloons spawn an explosion on death-- FIXED?
 * 
 * ********since I'm loading pretty much every song anyway.... add music randomizer module
 *
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * BUGS TO FIX:
 * 
 * Fix corpses so their death souths are also scaled
 * FIX the "teleport into arena" function
 * Fix toggling the mod off/on -- is broken -- LOL
 * 
 * 
 * 
 * 
 *
 * Hazards to fix:
 * Shot Orange LG 0.7:  needs velocity added? -- also look into a name correction....
 * ^delete the fsm, give some velocity, enable gravity, add a disable on collision action
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
 * 
 * 
 * 
 * 
 * ENEMIES TO ADD
 * 
ADD "Health Scuttler" and add it to the list of valid enemies to rando -- TODO
ADD "Mace Head Bug" -- fsm "Mace Control" (need to add a health manager to it) and a DamagesHero component
 * orange skitterer bugs
 * blue health cocoon bugs
 * fk's mace
 * 
 * 
 * 
 * HAZARDS TO ADD
 * 
 * 
 * 
 * 
 * 
 * EFFECTS TO ADD
 * 
 * 
 * 
 * 
 * 
 * 
 * LATER TODO:
 * 
 * Remove the grimmkin's screen space existance abilities....
 * 
 * 
 * 
 * 
 * WISHLIST
 * 
 * 
 * RANDOMIZE DREAMERS
 * RANDOMIZE END BOSS/ENDING
 * 
 * 
 * WISHLIST MODULES
 * 
 * Custom Scale (slider) in the scaling module -- will allow for everything to be (small/big)
 * 
 * Enemies must appear once every/logical module
 * 
 * 
 */

/*
 * Ruins1_27
 * root: _Scenery/ruind_fountain/fountain_new
 *   parts: _0083_fountain (center piece)
 *   parts: _0082_fountain (back dreamer)
 *   parts: _0092_fountain (right dreamer)
 *   parts: _0092_fountain (1) (left dreamer)
 * 
 * 
 * GG_Blue_Room
 * root: gg_blue_core
 *  remove: DeactivateIfPlayerDataTrue
 *  FSM: Dream React
 *      State: Take Control  <--probably just remove this state, at least all the actions on it should go
 *          Action: SetPlayerDataBool
 *          Action: CallMethodProper
 *          Action: SetFsmBool
 *      State: Regain Control <-- and probably just remove this too
 *      
 *      
 * GG_Workshop
 * 
 * dream_beam_animation   :cool floor aoe particle effect
 * 
 * GG_Statue_Gorb <-maybe copy them all?
 * (remove the BossStatue component)
 * 
 * make spawners for these
    <string>GG_Statue_Gorb</string><!--scene: GG_Workshop -->
    <string>GG_Statue_Hornet</string><!--scene: GG_Workshop -->
    <string>GG_Statue_GreyPrince</string><!--scene: GG_Workshop -->
    <string>Knight_v01</string><!--scene: GG_Workshop -->
    <string>Knight_v02</string><!--scene: GG_Workshop -->
    <string>GG_Statue_Grimm</string><!--scene: GG_Workshop -->
    <string>GG_Statue_Zote</string><!--scene: GG_Workshop -->
    <string>dream_beam_animation</string><!--scene: GG_Workshop - cool floor aoe particle effect-->
    <string>gg_blue_core</string><!-- scene: GG_Blue_Room - lifeblood core from godhome-->
    <string>_0083_fountain</string><!--scene: Ruins1_27 - fountain (center piece)-->
    <string>_0082_fountain</string><!--scene: Ruins1_27 - fountain (back dreamer)-->
    <string>_0092_fountain</string><!--scene: Ruins1_27 - fountain (right dreamer)-->
    <string>_0092_fountain</string><!--scene: Ruins1_27 - fountain (1) (left dreamer)-->


    <string>Health Scuttler</string>
    <string>Mace Head Bug</string>
 * 
 * 

//fix Zombie Swipe state machine -- no fix?
  //I think the scaling issue in the zombie swipe is in the animation clip of the zombie runner enemy itself

 *  
 * Broken Enemies to fix
 * big bee -- can set bounces in Charge Antic - SetIntValue(Bounces)
 * --might actually be fine
 * 
 * 
 * 
 * 
 *  ENEMY STATUS
 *  
 *  
 *  EnemyRandomizerMod.EnemyRandomizer.DebugSpawnEnemy("Lazy Flyer Enemy",null);
    EnemyRandomizerMod.EnemyRandomizer.DebugSpawnEnemy("Hatcher",null);
    EnemyRandomizerMod.EnemyRandomizer.DebugSpawnEnemy("Bursting Bouncer",null);
    EnemyRandomizerMod.EnemyRandomizer.DebugSpawnEnemy("Hornet Boss 1",null);
    EnemyRandomizerMod.EnemyRandomizer.DebugSpawnEnemy("Mage",null);
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
 */