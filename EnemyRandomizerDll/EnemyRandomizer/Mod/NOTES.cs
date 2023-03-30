
/* CURRENT TODO
 * 
 * IF AN ENEMY REPLACES THE SAME TYPE, SKIP REPLACEMENT? (add an option) 
 *
 * Remove the grimmkin's screen space existance abilities....
 * 
 * NERF the electric mage HP when replaces weakish enemies (based on count of them imo)
 * reduce their aggro range too
 * 
 * 
 * 
 * BUGS TO FIX:
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
 * WISHLIST
 * 
 * 
 * RANDOMIZE DREAMERS
 * RANDOMIZE END BOSS/ENDING
 * 
 * Change the city of tears' statues of dreamers/THK to be statues of the replacements
 * 
 * 
//grab "Health Scuttler" and add it to the list of valid enemies to rando -- TODO
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
    <string>Bee Hatchling Ambient</string>                 -   ""
    <string>Flip Hopper</string>                           -   ""
    <string>Inflater</string>                              -   ""
    <string>Fluke Fly</string>                             -   ""
    <string>Flukeman</string>                              -   ""
    <string>Bee Stinger</string>                           -   ""
    <string>Fat Fluke</string>                             -   ""
    <string>Super Spitter</string>                         -   ""
    <string>Mawlek Col</string>                            -   ""
    <string>Colosseum Grass Hopper</string>                -   ""


    <string>Super Spitter Col</string>                     -  Shouldn't be their own enemies....
    <string>Giant Fly Col</string>                         -  Shouldn't be their own enemies....
    <string>Buzzer Col</string>                            -  Shouldn't be their own enemies....
    <string>Ceiling Dropper Col</string>                   -  Shouldn't be their own enemies....
    <string>Colosseum_Armoured_Roller R</string>           -  ""
    <string>Colosseum_Armoured_Mosquito R</string>         -  ""
    <string>Super Spitter R</string>                       -  ""
    <string>Hatcher Baby</string>                          -   -- ??? don't think this will be needed
    <string>Roller R</string>                              -   -- (don't have a specific spawner with this string)
    <string>Spitter R</string>                             -   -- (it's converted to the non-R type)
    <string>Buzzer R</string>                              -   --
    <string>Great Shield Zombie bottom</string>            -   --
    <string>Flamebearer Spawn</string>                     -   used to extract the grimmkin
    <string>Corpse Garden Zombie</string>                  -   ADDED TO BAN LIST



    <string>Giant Fly</string>                             -   FIXED
    <string>Hatcher</string>                               -   FIXED
    <string>Crystallised Lazer Bug</string>                -   FIXED
    <string>Mawlek Turret</string>                         -   FIXED
    <string>Mawlek Turret Ceiling</string>                 -   FIXED
    <string>White Palace Fly</string>                      -   FIXED
    <string>Zote Boss</string>                             -   FIXED
 *  
    <string>Flamebearer Small</string>                     -   FIX SCREENSPACE - CHECK AGGRO
    <string>Flamebearer Med</string>                       -   FIX SCREENSPACE - CHECK AGGRO
    <string>Flamebearer Large</string>                     -   FIX SCREENSPACE - CHECK AGGRO

    <string>Zombie Spider 2</string>                       -   WORKING? - TEST
    <string>Zombie Spider 1</string>                       -   WORKING? - TEST
    <string>Zoteling</string>                              -   WORKING? - TEST
    <string>Mawlek Body</string>                           -   NEEDS TESTING -- PROJECTILES DON'T WORK
    <string>False Knight New</string>                      -   NEEDS TESTING
    <string>Mage Lord</string>                             -   NEEDS TESTING
    <string>Mage Lord Phase2</string>                      -   NEEDS TESTING
    <string>Infected Knight</string>                       -   NEEDS TESTING
    <string>False Knight Dream</string>                    -   NEEDS TESTING
    <string>Lost Kin</string>                              -   NEEDS TESTING
    <string>Radiance</string>                              -   NEEDS TESTING
    <string>Absolute Radiance</string>                     -   NEEDS TESTING
    <string>Zote Turret</string>                           -   NEEDS TESTING -- ?????
    <string>Zote Balloon Ordeal</string>                   -   NEEDS TESTING -- ?????
    <string>Zote Salubra</string>                          -   NEEDS TESTING -- DOESN'T DRAIN
    <string>Zote Thwomp</string>                           -   NEEDS TESTING -- STILL BROKEN
    <string>Zote Fluke</string>                            -   NEEDS TESTING -- DOESN'T WORK?
    <string>Zote Crew Normal</string>                      -   NEEDS TESTING -- ?????
    <string>Zote Crew Fat</string>                         -   NEEDS TESTING -- SEEMS TO ALMOST WORK???
    <string>Zote Crew Tall</string>                        -   NEEDS TESTING -- ?????
    <string>Zote Balloon</string>                          -   NEEDS TESTING -- FIX EXPLOSION NOT HAPPENING && RESPAWN STILL HAPPENING
    <string>Ordeal Zoteling</string>                       -   BROKEN - NEEDS ZOTELING FIX???
    <string>Baby Centipede</string>                        -   NEEDS TESTING

    <string>Grey Prince</string>                           -   STILL BROKEN
    <string>Tiny Spider</string>                           -   USUALLY SPAWNS STUCK
    <string>Shade Sibling</string>                         -   (ADD AN OPTION TO) REMOVE THE CHARM FRIENDLY EFFECT / GIVE IT TO THE REPLACED ENEMIES
    <string>Lancer</string>                                -   +++++++++++++++++NEEDS TESTING
    <string>Mage Blob</string>                             -   DONT SPAWN HIDDEN IN ARENAS
    <string>Mage</string>                                  -   +++++++++++++++++NEEDS TESTING
    <string>Electric Mage</string>                         -   +++++++++++++++++NEEDS TESTING
    <string>Climber</string>                               -   CAN SPAWN FLOATING?
    <string>Mender Bug</string>                            -   +++++++++++++++++NEEDS TESTING
    <string>Blocker</string>                               -   +++++++++++++++++NEEDS TESTING
    <string>Egg Sac</string>                               -   NEEDS ITEM TRANSFER/SPAWN SCRIPT
    <string>Gorgeous Husk</string>                         -   NEEDS MEME SCRIPT
    <string>Ceiling Dropper</string>                       -   KEEPS SPAWNING FLOATING
    <string>Mage Balloon</string>                          -   DONT SPAWN HIDDEN IN ARENAS
    <string>Plant Trap</string>                            -   OFTEN SPAWNS FLOATING / FACING INCORRECTLY
    <string>Acid Flyer</string>                            -   IF SOMETHING REPLACES THIS IT SHOULD HAVE TINKER ADDED!!
    <string>Acid Walker</string>                           -   IF SOMETHING REPLACES THIS IT SHOULD HAVE TINKER ADDED!!
    <string>Plant Turret</string>                          -   OFTEN SPAWNS FLOATING / FACING INCORRECTLY / DOESNT SHOOT CORRECTLY
    <string>Plant Turret Right</string>                    -   OFTEN SPAWNS FLOATING / FACING INCORRECTLY / DOESNT SHOOT CORRECTLY
    <string>Mushroom Turret</string>                       -   OFTEN SPAWNS FLOATING / FACING INCORRECTLY / DOESNT SHOOT CORRECTLY
    <string>Moss Knight</string>                           -   NEEDS TESTING
    <string>Moss Charger</string>                          -   SCRIPT HAS NULLREF ERRORS
    <string>Ghost Warrior Marmu</string>                   -   NEEDS CORPSE TESTING FOR CRASH
    <string>Zombie Beam Miner</string>                     -   NEEDS LASER RANGE EXTENDED
    <string>Spider Mini</string>                           -   KEEPS SPAWNING STUCK IN AIR
    <string>Centipede Hatcher</string>                     -   NEEDS HATCHER FIX
    <string>Mantis Heavy Spawn</string>                    -   NEEDS TESTING/REMOVAL
    <string>Zombie Hornhead Sp</string>                    -   NEEDS TESTING
    <string>Zombie Runner Sp</string>                      -   NEEDS TESTING
    <string>Fat Fly</string>                               -   TEST? MIGHT NEED BOSS FIX
    <string>Parasite Balloon</string>                      -   MAKE SPAWN FAST IN ARENA?
    <string>Royal Gaurd</string>                           -   NEEDS A FIX TO WORK RANDOMIZED EFFECTS
    <string>Zombie Hive</string>                           -   DOESNT PROPERLY SPAWN ADDS
    <string>Big Bee</string>                               -   WHEN SOMETHING REPLACES THIS IT SHOULD HAVE A SMASHER
    <string>Grave Zombie</string>                          -   MAYBE NERF TO 1 DMG WHEN REPLACING A WEAK ENEMY
    <string>Crystal Crawler</string>                       -   OFTEN SPAWNS FLOATING
    <string>Mines Crawler</string>                         -   OFTEN SPAWNS FLOATING?
    <string>Abyss Crawler</string>                         -   OFTEN SPAWN FLOATING
    <string>Mantis Flyer Child</string>                    -   NEEDS FIX FOR SPAWNING ON SURFACES CORRECTLY
    <string>Lil Jellyfish</string>                         -   IS A PROJECTILE??? LOOK INTO THIS 
    <string>Jellyfish</string>                             -   NEEDS SCRIPT FOR SPAWNING BABY CORRECTLY

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

    <string>Mega Fat Bee</string>                          -   +++++++++++++++++NEEDS TESTING
    <string>Lobster</string>                               -   +++++++++++++++++NEEDS TESTING
    <string>Mage Knight</string>                           -   +++++++++++++++++NEEDS TESTING
    <string>Black Knight</string>                          -   +++++++++++++++++NEEDS TESTING
    <string>Jar Collector</string>                         -   +++++++++++++++++NEEDS TESTING
    <string>Hornet Boss 1</string>                         -   NEEDS BOSS SCRIPT
    <string>Giant Buzzer</string>                          -   +++++++++++++++++NEEDS TESTING
    <string>Giant Buzzer Col</string>                      -   BROKEN HITBOX (not enabled) (this version spawns when zote chewing flag is done)
    <string>Mega Moss Charger</string>                     -   +++++++++++++++++NEEDS TESTING
    <string>Ghost Warrior No Eyes</string>                 -   NEEDS BOSS SCRIPT
    <string>Ghost Warrior Hu</string>                      -   NEEDS BOSS SCRIPT
    <string>Mantis Traitor Lord</string>                   -   NEEDS BOSS SCRIPT
    <string>Mega Jellyfish</string>                        -   NEEDS BOSS SCRIPT
    <string>Ghost Warrior Xero</string>                    -   NEEDS BOSS SCRIPT
    <string>Mega Zombie Beam Miner</string>                -   +++++++++++++++++NEEDS TESTING
    <string>Zombie Beam Miner Rematch</string>             -   +++++++++++++++++NEEDS TESTING
    <string>Mimic Spider</string>                          -   NEEDS BOSS SCRIPT
    <string>Ghost Warrior Galien</string>                  -   NEEDS BOSS SCRIPT
    <string>Hornet Boss 2</string>                         -   NEEDS BOSS SCRIPT
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
    <string>Ghost Warrior Slug</string>                    -   NEEDS BOSS SCRIPT
    <string>White Defender</string>                        -   NEEDS BOSS SCRIPT
    <string>Ghost Warrior Markoth</string>                 -   NEEDS BOSS SCRIPT
    <string>Jellyfish GG</string>                          -   NEEDS BOSS SCRIPT
 */