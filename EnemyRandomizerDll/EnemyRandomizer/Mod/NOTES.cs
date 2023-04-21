
/* CURRENT TODO
 * 
 * NEEDS TEST: KILLING WHITE PALACE ENEMY GUARD DIDN'T TRIGGER GATE OPENING
 * NEEDS TEST: killing the greenpath moss knight replacement didn't work
 * NEEDS TEST: Ground enemy spawned in first arena and walked outside, gate closed... try teleporting them back in when battle starts if they're outside
 * add lumaflies to the randomize hazard list
 * re-balance the random scale probabilities
 * 
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
 * Fix toggling the mod off/on
 * Fix toggling randomize bosses off/on
 * 
 * The lancer dies but does not despawn-- look up some corpses/death effects to use when replacing the lancer's
 * Replacements in an arena in queen's gardens did not work
 * Replacements in an arena in crossroads did not work (the one for glowing womb)
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
 * add Jelly Egg Bomb 
 * add Worm (gorm?)
 * Missing hazards to possibly add:
 * ruind_bridge_roof_04_spikes
 * Dung Pillar (1-6)
 * RoofSpikes
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
 * IF AN ENEMY REPLACES THE SAME TYPE, SKIP REPLACEMENT? (add an option) 
 * 
 * Remove the grimmkin's screen space existance abilities....
 * 
 * 
 * 
 * DESIGN ITERATIONS:
 * 
 * Check on the electric mage's hp, it was nerfed to 1/2- see how it feels.
 * Reduced Electric mage aggro range to a 30 radius circle- see how it feels.
 * 
 * 
 * 
 * WISHLIST
 * 
 * 
 * RANDOMIZE DREAMERS
 * RANDOMIZE END BOSS/ENDING
 * 
 * Change the city of tears' statues of dreamers/THK to be statues of the replacements
 * 
 * 
 * WISHLIST MODULES
 * 
 * Custom Scale (slider) in the scaling module -- will allow for everything to be (small/big)
 * 
 * Module/options to randomize only bosses
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
    BASIC FLYING ENEMIES
    <string>Mosquito</string>                              -   
    <string>Blobble</string>                               -   ""
    <string>Fly</string>                                   -   ""
    <string>Bursting Bouncer</string>                      -   ""
    <string>Angry Buzzer</string>                          -   ""
    <string>Mantis Heavy Flyer</string>                    -   ""
    <string>Buzzer</string>                                -   ""
    <string>Spitter</string>                               -   ""
    <string>Spitter R</string>                             -   "" ADDED GEO?
    <string>Ruins Flying Sentry</string>                   -   ""
    <string>Ruins Flying Sentry Javelin</string>           -   ""
    <string>Moss Flyer</string>                            -   ""
    <string>Lazy Flyer Enemy</string>                      -   ""
    <string>Fungoon Baby</string>                          -   ""
    <string>Fungus Flyer</string>                          -   ""
    <string>Jellyfish Baby</string>                        -   ""
    <string>Spider Flyer</string>                          -   ""
    <string>Crystal Flyer</string>                         -   ""  TODO: get their projectile
    <string>Blow Fly</string>                              -   ""
    <string>Bee Hatchling Ambient</string>                 -   ""   ADD GEO TO THIS (test)
    <string>Inflater</string>                              -   ""
    <string>Fluke Fly</string>                             -   ""
    <string>Bee Stinger</string>                           -   ""
    <string>Super Spitter</string>                         -   ""
    
    BASIC NONFLYING -- DOES NOT STICK TO GROUND
    <string>Hopper</string>                                -   "" 
    <string>Giant Hopper</string>                          -   ""
    <string>Roller</string>                                -   ""
    
    BASIC NONFLYING
    <string>Spitting Zombie</string>                       -   ""
    <string>Bursting Zombie</string>                       -   "" 
    <string>Mantis Heavy</string>                          -   "" 
    <string>Lesser Mawlek</string>                         -   "" 
    <string>Mossman_Runner</string>                        -   "" 
    <string>Crawler</string>                               -   "" 
    <string>Zombie Runner</string>                         -   "" fell through the floor?
    <string>Zombie Hornhead</string>                       -   "" 
    <string>Zombie Barger</string>                         -   "" 
    <string>Prayer Slug</string>                           -   "" (maggot)
    <string>Zombie Shield</string>                         -   ""
    <string>Zombie Leaper</string>                         -   ""
    <string>Zombie Guard</string>                          -   ""
    <string>Zombie Myla</string>                           -   "" WORKS (had to removed superdash check)
    <string>Royal Zombie Fat</string>                      -   ""
    <string>Royal Zombie</string>                          -   ""
    <string>Royal Zombie Coward</string>                   -   ""
    <string>Ruins Sentry</string>                          -   ""
    <string>Ruins Sentry Fat</string>                      -   ""
    <string>Great Shield Zombie</string>                   -   ""
    <string>Moss Walker</string>                           -   "" 
    <string>Mossman_Shaker</string>                        -   ""
    <string>Grass Hopper</string>                          -   ""                      
    <string>Zombie Fungus B</string>                       -   ""
    <string>Fung Crawler</string>                          -   ""
    <string>Mushroom Brawler</string>                      -   "" NEEDS TO BE UP MORE -- NEEDS FIX SO IT ISN'T RANDOMIZED AND STARTS AWAKE
    <string>Mushroom Baby</string>                         -   "" NEEDS TO BE UP MORE
    <string>Mushroom Roller</string>                       -   "" Added some spice
    <string>Zombie Fungus A</string>                       -   "" NEEDS TO BE UP SOME MORE
    <string>Mantis</string>                                -   ""
    <string>Garden Zombie</string>                         -   ""
    <string>Moss Knight Fat</string>                       -   "" Bouncing useless fat moss guy
    <string>Zombie Miner</string>                          -   ""
    <string>Slash Spider</string>                          -   ""
    <string>Flip Hopper</string>                           -   ""
    <string>Flukeman</string>                              -   ""
    <string>Fat Fluke</string>                             -   "" -- tiny ones walk out of arenas -- one spawned in the roof?
    <string>Crystal Crawler</string>                       -   WORKS - giant crystal crawler is fine

    <string>Mines Crawler</string>                         -   SPAWNS IN GROUND -- TEST, adjusted up slightly
    <string>Abyss Crawler</string>                         -   FIXED
    <string>Tiny Spider</string>                           -   FIXED
    <string>Climber</string>                               -   FIXED
    <string>Crystallised Lazer Bug</string>                -   POSITION FIXED, NEEDS FLIPPING ON TRANSITION IMPLEMENTED
                                                               ALSO, TRY NOT TO PICK WALLS WITH SLOPES WHEN THERE ARE ALTERNATIVES

    <string>Hatcher</string>                               -   FIXED -- ADDED DIE CHILDREN METHOD (test it) and changed child count to spawn more consistantly
    <string>Centipede Hatcher</string>                     -   FIXED
    <string>Zombie Hive</string>                           -   FIXED
    <string>Blocker</string>                               -   NEEDS TESTING
    

    <string>Giant Fly</string>                             -   NEEDS TESTING -- NEED FIX FOR WHEN VANILLA BOSS IS USED
    <string>Mawlek Turret</string>                         -   FIXED
    <string>Mawlek Turret Ceiling</string>                 -   FIXED
    <string>White Palace Fly</string>                      -   FIXED
    <string>Zote Boss</string>                             -   ERROR: AUDIO BUG, reduce the max enemy scale size for zote (and maybe others) -- TODO: remove the white screen flash
 *  
    <string>Flamebearer Small</string>                     -   ERROR: ADDED MINES 10
    <string>Flamebearer Med</string>                       -   ERROR: HAS A NULLREF -- TEST NEXT BUILD
    <string>Flamebearer Large</string>                     -   ERROR: SPAWNS IN A WEIRD SPOT

    <string>Zombie Spider 2</string>                       -   MAYBE FIXED? TEST
    <string>Zombie Spider 1</string>                       -   MAYBE FIXED? TEST
    <string>Zoteling</string>                              -   SPAWNS DISABLED? -- CORRECTLY SPAWNS WHEN USED AS REPLACEMENT -- NEEDS SOUND EFFECTS
    <string>Mawlek Body</string>                           -   NEED TO FIX HIM STARTING IN STEALTH STATE  -- MawlekBodyControl+<Start>  HAS A NULLREF
    <string>False Knight New</string>                      -   SEEMS TOTALLY BUSTED -- ALSO CAN'T SEEM TO KILL -- ONCE ENGAGED HE FIGHTS OK (BUT HIS BASIC COMBAT BODY IS INVISIBLE)
    <string>Mage Lord</string>                             -   SPAWNS INCORRECTLY AND IN FLOOR? --- GETS RANDOMIZED WHEN HE TELEPORTS????????
    <string>Mage Lord Phase2</string>                      -   GETS RANDOMIZED WHEN HE TELEPORTS --- ALMOST WORKS WHEN REPLACING
    <string>Infected Knight</string>                       -   DOESN'T SPAWN -- SPAWNS WHEN REPLACING, DOESN'T HANDLE MAX HEIGHT ROOFS - YEETS TO SPACE INDOORS
    <string>False Knight Dream</string>                    -   NULLREF ISSUES
    <string>Lost Kin</string>                              -   SPAWNS INCORRECTLY WHEN REPLACING
    <string>Radiance</string>                              -   SPAWNS CORRECTLY - NULLREF WHEN REPLACING
    <string>Absolute Radiance</string>                     -   HAS MANY ISSUES


    <string>Zote Turret</string>                           -   SPAWNS INCORRECTLY - REPLACES CORRECTLY BUT POSITION IS WRONG
    <string>Zote Balloon Ordeal</string>                   -   SPAWNS INCORRECTLY
    <string>Zote Salubra</string>                          -   NEEDS TESTING -- DOESN'T DRAIN
    <string>Zote Thwomp</string>                           -   NEEDS TESTING -- STILL BROKEN
    <string>Zote Fluke</string>                            -   NEEDS TESTING -- DOESN'T WORK?
    <string>Zote Crew Normal</string>                      -   NEEDS TESTING -- ?????
    <string>Zote Crew Fat</string>                         -   NEEDS TESTING -- SEEMS TO ALMOST WORK???
    <string>Zote Crew Tall</string>                        -   NEEDS TESTING -- ?????
    <string>Zote Balloon</string>                          -   NEEDS TESTING -- FIX EXPLOSION NOT HAPPENING && RESPAWN STILL HAPPENING
    <string>Ordeal Zoteling</string>                       -   BROKEN - NEEDS ZOTELING FIX???


    <string>Baby Centipede</string>                        -   GET LOTS OF NULLREFS -- SPAWNS UNDER SCENE

    <string>Grey Prince</string>                           -   STILL BROKEN -- MOSTLY WORKING -- THE SLAME EFFECTS ARE USING THE WRONG HEIGHT -- FIX THE CEILING DROP POSITIONING
    <string>Shade Sibling</string>                         -   (ADD AN OPTION TO) REMOVE THE CHARM FRIENDLY EFFECT / GIVE IT TO THE REPLACED ENEMIES
    <string>Lancer</string>                                -   +++++++++++++++++NEEDS TESTING
    <string>Mage Blob</string>                             -   DONT SPAWN HIDDEN IN ARENAS
    <string>Mage</string>                                  -   DOES NOT SPAWN CORRECTLY
    <string>Electric Mage</string>                         -   DOES NOT SPAWN CORRECTLY
    <string>Mender Bug</string>                            -   +++++++++++++++++NEEDS TESTING
    <string>Egg Sac</string>                               -   NEEDS ITEM TRANSFER/SPAWN SCRIPT
    <string>Gorgeous Husk</string>                         -   Made more exciting
    <string>Ceiling Dropper</string>                       -   NEEDS TESTING -- FACING THE WRONG WAY
    <string>Mage Balloon</string>                          -   DONT SPAWN HIDDEN IN ARENAS
    <string>Plant Trap</string>                            -   NEEDS TESTING -- wasnnt placed on a surface (was floating high up)
    <string>Plant Turret</string>                          -   NEEDS TESTING -- spawning in ground, wrong direction
    <string>Plant Turret Right</string>                    -   NEEDS TESTING -- spawning in ground, wrong direction
    <string>Mushroom Turret</string>                       -   NEEDS TESTING -- spawning in ground/floating, wrong direction
    <string>Moss Knight</string>                           -   NEEDS TESTING
    <string>Moss Charger</string>                          -   SCRIPT HAS NULLREF ERRORS
    <string>Zombie Beam Miner</string>                     -   NEEDS LASER RANGE EXTENDED
    <string>Spider Mini</string>                           -   FIXED
    <string>Mantis Heavy Spawn</string>                    -   NEEDS TESTING/REMOVAL
    <string>Zombie Hornhead Sp</string>                    -   NEEDS TESTING
    <string>Zombie Runner Sp</string>                      -   NEEDS TESTING
    <string>Fat Fly</string>                               -   WORKS
    <string>Parasite Balloon</string>                      -   SEEMS OK
    <string>Royal Gaurd</string>                           -   MAYBE NERF TO 1 DMG WHEN REPLACING A WEAK ENEMY -- NEEDS A FIX TO WORK RANDOMIZED EFFECTS
    <string>Grave Zombie</string>                          -   MAYBE NERF TO 1 DMG WHEN REPLACING A WEAK ENEMY
    <string>Mantis Flyer Child</string>                    -   SPAWNS ON WALLS/FLOOR BACKWARDS/INSIDE
    <string>Lil Jellyfish</string>                         -   THIS IS THE JELLYFISH PROJECTILE -- DON'T RANDOMLY REPLACE AN ENEMY WITH THIS

    <string>Jellyfish</string>                             -   NEEDS SCRIPT FOR SPAWNING BABY CORRECTLY
    <string>Acid Flyer</string>                            -   IF SOMETHING REPLACES THIS IT SHOULD HAVE TINKER ADDED!!
    <string>Acid Walker</string>                           -   IF SOMETHING REPLACES THIS IT SHOULD HAVE TINKER ADDED!!
    <string>Big Bee</string>                               -   WHEN SOMETHING REPLACES THIS IT SHOULD HAVE A SMASHER

    <string>Pigeon</string>                                -   COULD USE SOMETHING TO MAKE THEM MORE INTERESTING
    <string>fluke_baby_02</string>                         -   COULD USE SOMETHING TO MAKE THEM MORE INTERESTING
    <string>fluke_baby_01</string>                         -   COULD USE SOMETHING TO MAKE THEM MORE INTERESTING
    <string>fluke_baby_03</string>                         -   COULD USE SOMETHING TO MAKE THEM MORE INTERESTING
    <string>Enemy</string>                                 -   "" 


    <string>Colosseum_Armoured_Roller</string>             -   CHECK COLO SCRIPT?
    <string>Colosseum_Miner</string>                       -   CHECK COLO SCRIPT?
    <string>Colosseum_Shield_Zombie</string>               -   CHECK COLO SCRIPT?
    <string>Colosseum_Armoured_Mosquito</string>           -   CHECK COLO SCRIPT?
    <string>Colosseum_Flying_Sentry</string>               -   CHECK COLO SCRIPT?
    <string>Colosseum_Worm</string>                        -   CHECK COLO SCRIPT?

    <string>Mega Fat Bee</string>                          -   DID NOT SPAWN IN THE CORRECT LOCATION
    <string>Lobster</string>                               -   SPAWNED IN THE GROUND
    <string>Mage Knight</string>                           -   WORKS WHEN REPLACING?
    <string>Black Knight</string>                          -   NEEDS TESTING - YEETING WAS A RESULT OF DISABLED FSM WHEN OUT OF RANGED
    <string>Jar Collector</string>                         -   DOESN'T PROPERLY THROW JARS
    <string>Hornet Boss 1</string>                         -   REMOVE 'DEACTIVATE IF PLAYER DATA TRUE'
    <string>Giant Buzzer</string>                          -   NEEDS TESTING (seems to work)
    <string>Giant Buzzer Col</string>                      -   NEEDS TESTING (seems to work)
    <string>Mega Moss Charger</string>                     -   NULLREF
    <string>Mega Zombie Beam Miner</string>                -   TEST IF CAMERA FIGHTING IS FIXED
    <string>Zombie Beam Miner Rematch</string>             -   LASER ISN'T WORKING
    <string>Hornet Boss 2</string>                         -   DOESN'T SPAWN
    <string>Mimic Spider</string>                          -   SPAWNS TOO FAR TO THE RIGHT
    <string>Mantis Traitor Lord</string>                   -   NEEDS TESTING
    <string>Dung Defender</string>                         -   NEEDS BOSS SCRIPT
    <string>Fluke Mother</string>                          -   NEEDS TESTING
    <string>Hive Knight</string>                           -   NEEDS TESTING
    <string>Grimm Boss</string>                            -   NEEDS TESTING
    <string>Nightmare Grimm Boss</string>                  -   NEEDS TESTING
    <string>Dream Mage Lord</string>                       -   NEEDS TESTING
    <string>Dream Mage Lord Phase2</string>                -   NEEDS TESTING
    <string>Hollow Knight Boss</string>                    -   NEEDS BOSS SCRIPT -- WOULDN'T ACTIVATE AND COULDN'T DIE
    <string>HK Prime</string>                              -   NEEDS TESTING
    <string>Pale Lurker</string>                           -   WOULD NOT ACTIVATE
    <string>Oro</string>                                   -   WOULD NOT ACTIVATE
    <string>Mato</string>                                  -   WOULD NOT ACTIVATE
    <string>Sheo Boss</string>                             -   WOULD NOT ACTIVATE
    <string>Sly Boss</string>                              -   NEEDS BOSS SCRIPT
    <string>Hornet Nosk</string>                           -   WOULDN'T ACTIVATE
    <string>White Defender</string>                        -   NEEDS BOSS SCRIPT
    <string>Jellyfish GG</string>                          -   NEEDS BOSS SCRIPT
    <string>Mega Jellyfish</string>                        -   NEEDS BOSS SCRIPT
    <string>Ghost Warrior Marmu</string>                   -   NEEDS MORE TESTING -- SEEMS FIXED?
    <string>Ghost Warrior Galien</string>                  -   NEEDS BOSS SCRIPT
    <string>Ghost Warrior Xero</string>                    -   NEEDS BOSS SCRIPT
    <string>Ghost Warrior Slug</string>                    -   NEEDS BOSS SCRIPT
    <string>Ghost Warrior Markoth</string>                 -   NEEDS BOSS SCRIPT -- NEEDS HIS GHOST WEAPON 
    <string>Ghost Warrior No Eyes</string>                 -   NEEDS BOSS SCRIPT
    <string>Ghost Warrior Hu</string>                      -   NEEDS BOSS SCRIPT






    <string>Mawlek Col</string>                            -   ""   NOT A VALID ENEMY STRING (fix this?)
    <string>Colosseum Grass Hopper</string>                -   ""   NO LONGER A VALID ENEMY STRING
    <string>Flukeman Top</string>                          -   "" NO LONGER A VALID ENEMY STRING
    <string>Flukeman Bot</string>                          -   "" NO LONGER A VALID ENEMY STRING
    <string>Super Spitter Col</string>                     -  NO LONGER A VALID ENEMY STRING
    <string>Giant Fly Col</string>                         -  NO LONGER A VALID ENEMY STRING
    <string>Buzzer Col</string>                            -  NO LONGER A VALID ENEMY STRING
    <string>Ceiling Dropper Col</string>                   -  NO LONGER A VALID ENEMY STRING
    <string>Colosseum_Armoured_Roller R</string>           -  NO LONGER A VALID ENEMY STRING
    <string>Colosseum_Armoured_Mosquito R</string>         -  NO LONGER A VALID ENEMY STRING
    <string>Super Spitter R</string>                       -  NO LONGER A VALID ENEMY STRING
    <string>Hatcher Baby</string>                          -  NO LONGER A VALID ENEMY STRING
    <string>Roller R</string>                              -  NO LONGER A VALID ENEMY STRING
    <string>Spitter R</string>                             -  VALID - CONVERTS TO NON-R TYPE (fails to drop geo)
    <string>Buzzer R</string>                              -  NO LONGER A VALID ENEMY STRING
    <string>Great Shield Zombie bottom</string>            -  NO LONGER A VALID ENEMY STRING
    <string>Flamebearer Spawn</string>                     -  NO LONGER A VALID ENEMY STRING - used to extract the grimmkin types on load
    <string>Corpse Garden Zombie</string>                  -  ADDED TO BAN LIST
 */