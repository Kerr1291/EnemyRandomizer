
/* CURRENT TODO
 * 
 * NEEDS TEST: KILLING WHITE PALACE ENEMY GUARD DIDN'T TRIGGER GATE OPENING
 * NEEDS TEST: Ground enemy spawned in first arena and walked outside, gate closed... try teleporting them back in when battle starts if they're outside
 * 
 * MAKE USER SETTABLE SEED
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
 * Slash (don't add this)
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
ADD "Health Scuttler" and add it to the list of valid enemies to rando -- TODO
ADD "Mace Head Bug" -- fsm "Mace Control" (need to add a health manager to it) and a DamagesHero component
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
 *  
    <string>Mosquito</string>                              -   IS BASIC ENEMY (see BasicEnemies.cs) && SHOULD WORK FINE
    <string>Hopper</string>                                -   ""
    <string>Bursting Bouncer</string>                      -   ""
    <string>Blobble</string>                               -   ""
    <string>Giant Hopper</string>                          -   ""
    <string>Spitting Zombie</string>                       -   ""
    <string>Bursting Zombie</string>                       -   ""
    <string>Angry Buzzer</string>                          -   ""
    <string>Mantis Heavy</string>                          -   ""
    <string>Lesser Mawlek</string>                         -   ""
    <string>Mantis Heavy Flyer</string>                    -   ""
    <string>Fly</string>                                   -   ""
    <string>Roller</string>                                -   ""
    <string>Mossman_Runner</string>                        -   ""
    <string>Flukeman Top</string>                          -   ""
    <string>Flukeman Bot</string>                          -   ""
    <string>Crawler</string>                               -   ""
    <string>Buzzer</string>                                -   ""
    <string>Zombie Runner</string>                         -   ""
    <string>Spitter</string>                               -   ""
    <string>Zombie Hornhead</string>                       -   ""
    <string>Zombie Barger</string>                         -   ""
    <string>Prayer Slug</string>                           -   ""
    <string>Zombie Shield</string>                         -   ""
    <string>Zombie Leaper</string>                         -   ""
    <string>Zombie Guard</string>                          -   ""
    <string>Zombie Myla</string>                           -   ""
    <string>Royal Zombie Fat</string>                      -   ""
    <string>Royal Zombie</string>                          -   ""
    <string>Royal Zombie Coward</string>                   -   ""
    <string>Ruins Sentry</string>                          -   ""
    <string>Ruins Flying Sentry</string>                   -   ""
    <string>Ruins Flying Sentry Javelin</string>           -   ""
    <string>Ruins Sentry Fat</string>                      -   ""
    <string>Great Shield Zombie</string>                   -   ""
    <string>Moss Walker</string>                           -   ""
    <string>Moss Flyer</string>                            -   ""
    <string>Mossman_Shaker</string>                        -   ""
    <string>Grass Hopper</string>                          -   ""
    <string>Lazy Flyer Enemy</string>                      -   ""
    <string>Fungoon Baby</string>                          -   ""
    <string>Fungus Flyer</string>                          -   ""
    <string>Zombie Fungus B</string>                       -   ""
    <string>Fung Crawler</string>                          -   ""
    <string>Mushroom Brawler</string>                      -   ""
    <string>Mushroom Baby</string>                         -   ""
    <string>Mushroom Roller</string>                       -   ""
    <string>Zombie Fungus A</string>                       -   ""
    <string>Mantis</string>                                -   ""
    <string>Jellyfish Baby</string>                        -   ""
    <string>Garden Zombie</string>                         -   ""
    <string>Moss Knight Fat</string>                       -   ""
    <string>Zombie Miner</string>                          -   ""
    <string>Crystal Flyer</string>                         -   ""
    <string>Slash Spider</string>                          -   ""
    <string>Spider Flyer</string>                          -   ""
    <string>Blow Fly</string>                              -   ""
    <string>Bee Hatchling Ambient</string>                 -   ""   ADD GEO TO THIS
    <string>Flip Hopper</string>                           -   ""
    <string>Inflater</string>                              -   ""
    <string>Fluke Fly</string>                             -   ""
    <string>Flukeman</string>                              -   ""
    <string>Bee Stinger</string>                           -   ""
    <string>Fat Fluke</string>                             -   ""
    <string>Super Spitter</string>                         -   ""
    <string>Mawlek Col</string>                            -   ""   NOT A VALID ENEMY STRING (fix this?)
    <string>Colosseum Grass Hopper</string>                -   ""   NO LONGER A VALID ENEMY STRING

    <string>Crystal Crawler</string>                       -   TODO: TEST POSITION FIX
    <string>Mines Crawler</string>                         -   TODO: TEST POSITION FIX
    <string>Abyss Crawler</string>                         -   TODO: TEST POSITION FIX
    <string>Tiny Spider</string>                           -   TODO: TEST POSITION FIX
    <string>Climber</string>                               -   TODO: TEST POSITION FIX
    <string>Crystallised Lazer Bug</string>                -   TODO: NEEDS TO BE POSITIONED HALF ITS HITBOX SIZE DOWNWARD
    

    <string>Hatcher</string>                               -   FIXED -- ADDED DIE CHILDREN METHOD (test it) and changed child count to spawn more consistantly
    <string>Centipede Hatcher</string>                     -   NEEDS TESTING
    <string>Zombie Hive</string>                           -   NEEDS TESTING
    <string>Blocker</string>                               -   NEEDS TESTING
    

    <string>Giant Fly</string>                             -   WORKS BUT  --- FIX THIS ENEMY TO NOT START SLEEPING OR PLACE ON THE GROUND
                                                                          ALSO, REMOVE THE TITLE POPUP
    <string>Mawlek Turret</string>                         -   FIXED
    <string>Mawlek Turret Ceiling</string>                 -   FIXED
    <string>White Palace Fly</string>                      -   FIXED
    <string>Zote Boss</string>                             -   FIXED -- TODO: remove the white screen flash
 *  
    <string>Flamebearer Small</string>                     -   ERROR: DOES NOT SPAWN?
    <string>Flamebearer Med</string>                       -   ERROR: HAS A NULLREF
    <string>Flamebearer Large</string>                     -   ERROR: SPAWNS IN A WEIRD SPOT

    <string>Zombie Spider 2</string>                       -   ERROR: NULLREF TRYING TO SPAWN -- CORRECTLY SPAWNS WHEN USED AS REPLACEMENT BUT DID NOT COME ALIVE
    <string>Zombie Spider 1</string>                       -   SPAWNS DISABLED? -- CORRECTLY SPAWNS WHEN USED AS REPLACEMENT
    <string>Zoteling</string>                              -   SPAWNS DISABLED? -- CORRECTLY SPAWNS WHEN USED AS REPLACEMENT -- NEEDS SOUND EFFECTS
    <string>Mawlek Body</string>                           -   NEED TO FIX HIM STARTING IN STEALTH STATE  -- MawlekBodyControl+<Start>  HAS A NULLREF
    <string>False Knight New</string>                      -   SEEMS TOTALLY BUSTED -- ALSO CAN'T SEEM TO KILL -- ONCE ENGAGED HE FIGHTS OK (BUT HIS BASIC COMBAT BODY IS INVISIBLE)
    <string>Mage Lord</string>                             -   SPAWNS INCORRECTLY AND IN FLOOR? --- GETS RANDOMIZED WHEN HE TELEPORTS????????
    <string>Mage Lord Phase2</string>                      -   GETS RANDOMIZED WHEN HE TELEPORTS --- ALMOST WORKS WHEN REPLACING
    <string>Infected Knight</string>                       -   DOESN'T SPAWN -- SPAWNS WHEN REPLACING, DOESN'T HANDLE MAX HEIGHT ROOFS - YEETS TO SPACE INDOORS
    <string>False Knight Dream</string>                    -   NULLREF ISSUES
    <string>Lost Kin</string>                              -   SPAWNS INCORRECTLY WHEN REPLACING
    <string>Radiance</string>                              -   SPAWNS CORRECTLY - NULLREF WHEN REPLACING
    <string>Absolute Radiance</string>                     -   SPAWNS CORRECTLY - REPLACES INCORRECTLY (SAME POSITION ISSUE AS OTHERS)


    <string>Zote Turret</string>                           -   SPAWNS INCORRECTLY - REPLACES CORRECTLY BUT POSITION IS WRONG
    <string>Zote Balloon Ordeal</string>                   -   NEEDS TESTING -- ?????
    <string>Zote Salubra</string>                          -   NEEDS TESTING -- DOESN'T DRAIN
    <string>Zote Thwomp</string>                           -   NEEDS TESTING -- STILL BROKEN
    <string>Zote Fluke</string>                            -   NEEDS TESTING -- DOESN'T WORK?
    <string>Zote Crew Normal</string>                      -   NEEDS TESTING -- ?????
    <string>Zote Crew Fat</string>                         -   NEEDS TESTING -- SEEMS TO ALMOST WORK???
    <string>Zote Crew Tall</string>                        -   NEEDS TESTING -- ?????
    <string>Zote Balloon</string>                          -   NEEDS TESTING -- FIX EXPLOSION NOT HAPPENING && RESPAWN STILL HAPPENING
    <string>Ordeal Zoteling</string>                       -   BROKEN - NEEDS ZOTELING FIX???


    <string>Baby Centipede</string>                        -   NEED TO LOOK UP ITS FSM IN GAME

    <string>Grey Prince</string>                           -   STILL BROKEN
    <string>Shade Sibling</string>                         -   (ADD AN OPTION TO) REMOVE THE CHARM FRIENDLY EFFECT / GIVE IT TO THE REPLACED ENEMIES
    <string>Lancer</string>                                -   +++++++++++++++++NEEDS TESTING
    <string>Mage Blob</string>                             -   DONT SPAWN HIDDEN IN ARENAS
    <string>Mage</string>                                  -   +++++++++++++++++NEEDS TESTING
    <string>Electric Mage</string>                         -   +++++++++++++++++NEEDS TESTING
    <string>Mender Bug</string>                            -   +++++++++++++++++NEEDS TESTING
    <string>Egg Sac</string>                               -   NEEDS ITEM TRANSFER/SPAWN SCRIPT
    <string>Gorgeous Husk</string>                         -   NEEDS MEME SCRIPT
    <string>Ceiling Dropper</string>                       -   NEEDS TESTING
    <string>Mage Balloon</string>                          -   DONT SPAWN HIDDEN IN ARENAS
    <string>Plant Trap</string>                            -   NEEDS TESTING
    <string>Plant Turret</string>                          -   NEEDS TESTING
    <string>Plant Turret Right</string>                    -   NEEDS TESTING
    <string>Mushroom Turret</string>                       -   NEEDS TESTING
    <string>Moss Knight</string>                           -   NEEDS TESTING
    <string>Moss Charger</string>                          -   SCRIPT HAS NULLREF ERRORS
    <string>Ghost Warrior Marmu</string>                   -   NEEDS MORE TESTING -- SEEMS FIXED?
    <string>Zombie Beam Miner</string>                     -   NEEDS LASER RANGE EXTENDED
    <string>Spider Mini</string>                           -   NEEDS TESTING
    <string>Mantis Heavy Spawn</string>                    -   NEEDS TESTING/REMOVAL
    <string>Zombie Hornhead Sp</string>                    -   NEEDS TESTING
    <string>Zombie Runner Sp</string>                      -   NEEDS TESTING
    <string>Fat Fly</string>                               -   TEST? MIGHT NEED BOSS FIX
    <string>Parasite Balloon</string>                      -   MAKE SPAWN FAST IN ARENA?
    <string>Royal Gaurd</string>                           -   MAYBE NERF TO 1 DMG WHEN REPLACING A WEAK ENEMY -- NEEDS A FIX TO WORK RANDOMIZED EFFECTS
    <string>Grave Zombie</string>                          -   MAYBE NERF TO 1 DMG WHEN REPLACING A WEAK ENEMY
    <string>Mantis Flyer Child</string>                    -   NEEDS FIX FOR SPAWNING ON SURFACES CORRECTLY
    <string>Lil Jellyfish</string>                         -   IS A PROJECTILE??? LOOK INTO THIS 

    <string>Jellyfish</string>                             -   NEEDS SCRIPT FOR SPAWNING BABY CORRECTLY ?
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

    <string>Mega Fat Bee</string>                          -   NEEDS TESTING
    <string>Lobster</string>                               -   NEEDS TESTING
    <string>Mage Knight</string>                           -   WORKS WHEN REPLACING?
    <string>Black Knight</string>                          -   NEEDS TESTING - YEETING WAS A RESULT OF DISABLED FSM WHEN OUT OF RANGED
    <string>Jar Collector</string>                         -   ADDED SOME CODE TO CATCH THE NULLREF IN SETUP -- TEST!
    <string>Hornet Boss 1</string>                         -   DOESN'T SPAWN CORRECTLY
    <string>Giant Buzzer</string>                          -   +++++++++++++++++NEEDS TESTING -- SPAMS NULLREFS? (it changed to a giant buzzer col and spammed nullrefs)
    <string>Giant Buzzer Col</string>                      -   +++++++++++++++++NEEDS TESTING -- hitbox fixed? BROKEN HITBOX (not enabled) (this version spawns when zote chewing flag is done)
    <string>Mega Moss Charger</string>                     -   NULLREF
    <string>Mega Zombie Beam Miner</string>                -   TEST IF CAMERA FIGHTING IS FIXED
    <string>Zombie Beam Miner Rematch</string>             -   LASER ISN'T WORKING
    <string>Hornet Boss 2</string>                         -   DOESN'T SPAWN
    <string>Mimic Spider</string>                          -   SPAWNS TOO FAR TO THE RIGHT
    <string>Mantis Traitor Lord</string>                   -   NEEDS BOSS SCRIPT
    <string>Dung Defender</string>                         -   NEEDS BOSS SCRIPT
    <string>Fluke Mother</string>                          -   NEEDS BOSS SCRIPT
    <string>Hive Knight</string>                           -   NEEDS BOSS SCRIPT
    <string>Grimm Boss</string>                            -   NEEDS BOSS SCRIPT
    <string>Nightmare Grimm Boss</string>                  -   NEEDS BOSS SCRIPT
    <string>Dream Mage Lord</string>                       -   NEEDS BOSS SCRIPT
    <string>Dream Mage Lord Phase2</string>                -   NEEDS BOSS SCRIPT
    <string>Hollow Knight Boss</string>                    -   NEEDS BOSS SCRIPT
    <string>HK Prime</string>                              -   NEEDS BOSS SCRIPT
    <string>Pale Lurker</string>                           -   NEEDS BOSS SCRIPT
    <string>Oro</string>                                   -   NEEDS BOSS SCRIPT
    <string>Mato</string>                                  -   NEEDS BOSS SCRIPT
    <string>Sheo Boss</string>                             -   NEEDS BOSS SCRIPT
    <string>Sly Boss</string>                              -   NEEDS BOSS SCRIPT
    <string>Hornet Nosk</string>                           -   NEEDS BOSS SCRIPT
    <string>White Defender</string>                        -   NEEDS BOSS SCRIPT
    <string>Jellyfish GG</string>                          -   NEEDS BOSS SCRIPT
    <string>Mega Jellyfish</string>                        -   NEEDS BOSS SCRIPT
    <string>Ghost Warrior Galien</string>                  -   NEEDS BOSS SCRIPT
    <string>Ghost Warrior Xero</string>                    -   NEEDS BOSS SCRIPT
    <string>Ghost Warrior Slug</string>                    -   NEEDS BOSS SCRIPT
    <string>Ghost Warrior Markoth</string>                 -   NEEDS BOSS SCRIPT
    <string>Ghost Warrior No Eyes</string>                 -   NEEDS BOSS SCRIPT
    <string>Ghost Warrior Hu</string>                      -   NEEDS BOSS SCRIPT






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