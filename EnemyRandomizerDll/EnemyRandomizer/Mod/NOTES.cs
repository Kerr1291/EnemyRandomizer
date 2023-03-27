
/* CURRENT TODO
 * 
 * 
 *
 * Remove the grimmkin's screen space existance abilities....
 * 
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
    <string>Giant Fly</string>                             -   FIXED
    <string>Hatcher</string>                               -   FIXED
    <string>Crystallised Lazer Bug</string>                -   FIXED
    <string>Mawlek Turret</string>                         -   FIXED
    <string>Mawlek Turret Ceiling</string>                 -   FIXED
    <string>White Palace Fly</string>                      -   FIXED
    <string>Zote Boss</string>                             -   FIXED

 *  <string>Flamebearer Spawn</string>                     -   DONE
 *  
    <string>Flamebearer Small</string>                     -   FIX SCREENSPACE - CHECK AGGRO
    <string>Flamebearer Med</string>                       -   FIX SCREENSPACE - CHECK AGGRO
    <string>Flamebearer Large</string>                     -   FIX SCREENSPACE - CHECK AGGRO

    <string>Zombie Spider 2</string>                       -   WORKING? - TEST
    <string>Zombie Spider 1</string>                       -   WORKING? - TEST
    <string>Zoteling</string>                              -   WORKING? - TEST
    <string>Mawlek Body</string>                           -   NEEDS TESTING
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
    <string>Ordeal Zoteling</string>                       -   ???????

    <string>Grey Prince</string>                           -   STILL BROKEN

    <string>Mosquito</string>                              -   IN BASIC ENEMY LIST (see BasicEnemies.cs)
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
    <string>Hatcher Baby</string>                          -   -- ??? don't think this will be needed
    <string>Roller R</string>                              -   -- (don't have a specific spawner with this string)
    <string>Spitter R</string>                             -   -- (it's converted to the non-R type)
    <string>Buzzer R</string>                              -   --
    <string>Mossman_Runner</string>                        -   ""
    <string>Corpse Garden Zombie</string>                  -   "" ??? check if this is a corpse, remove from enemy pool if it is
    <string>Tiny Spider</string>                           -   ""
    <string>Shade Sibling</string>                         -   ""
    <string>Baby Centipede</string>                        -   "" 
    <string>Flukeman Top</string>                          -   ""
    <string>Flukeman Bot</string>                          -   ""
    <string>White Defender</string>                        -   ""
    <string>Jellyfish GG</string>                          -   ""
    <string>Crawler</string>                               -   ""
    <string>Buzzer</string>                                -   ""
    <string>Mega Fat Bee</string>                          -   ""
    <string>Lobster</string>                               -   ""
    <string>Mage Knight</string>                           -   ""
    <string>Mage</string>                                  -   ""
    <string>Electric Mage</string>                         -   ""
    <string>Mage Blob</string>                             -   ""
    <string>Lancer</string>                                -   ""
    <string>Climber</string>                               -   ""
    <string>Zombie Runner</string>                         -   ""
    <string>Mender Bug</string>                            -   ""
    <string>Spitter</string>                               -   ""
    <string>Zombie Hornhead</string>                       -   ""
    <string>Zombie Barger</string>                         -   ""
    <string>Prayer Slug</string>                           -   ""
    <string>Blocker</string>                               -   ""
    <string>Zombie Shield</string>                         -   ""
    <string>Zombie Leaper</string>                         -   ""
    <string>Zombie Guard</string>                          -   ""
    <string>Zombie Myla</string>                           -   ""
    <string>Egg Sac</string>                               -   ""
    <string>Royal Zombie Fat</string>                      -   ""
    <string>Royal Zombie</string>                          -   ""
    <string>Royal Zombie Coward</string>                   -   ""
    <string>Gorgeous Husk</string>                         -   ""
    <string>Ceiling Dropper</string>                       -   ""
    <string>Ruins Sentry</string>                          -   ""
    <string>Ruins Flying Sentry</string>                   -   ""
    <string>Ruins Flying Sentry Javelin</string>           -   ""
    <string>Ruins Sentry Fat</string>                      -   ""
    <string>Mage Balloon</string>                          -   ""
    <string>Great Shield Zombie</string>                   -   ""
    <string>Great Shield Zombie bottom</string>            -   --
    <string>Black Knight</string>                          -   ""
    <string>Jar Collector</string>                         -   ""
    <string>Moss Walker</string>                           -   ""
    <string>Plant Trap</string>                            -   ""
    <string>Mossman_Shaker</string>                        -   ""
    <string>Pigeon</string>                                -   ""
    <string>Hornet Boss 1</string>                         -   ""
    <string>Acid Flyer</string>                            -   ""
    <string>Moss Charger</string>                          -   ""
    <string>Acid Walker</string>                           -   ""
    <string>Plant Turret</string>                          -   ""
    <string>Plant Turret Right</string>                    -   ""
    <string>Fat Fly</string>                               -   ""
    <string>Giant Buzzer</string>                          -   ""
    <string>Moss Knight</string>                           -   ""
    <string>Grass Hopper</string>                          -   ""
    <string>Lazy Flyer Enemy</string>                      -   ""
    <string>Mega Moss Charger</string>                     -   ""
    <string>Ghost Warrior No Eyes</string>                 -   ""
    <string>Fungoon Baby</string>                          -   ""
    <string>Mushroom Turret</string>                       -   ""
    <string>Fungus Flyer</string>                          -   ""
    <string>Zombie Fungus B</string>                       -   ""
    <string>Fung Crawler</string>                          -   ""
    <string>Mushroom Brawler</string>                      -   ""
    <string>Mushroom Baby</string>                         -   ""
    <string>Mushroom Roller</string>                       -   ""
    <string>Zombie Fungus A</string>                       -   ""
    <string>Mantis</string>                                -   ""
    <string>Ghost Warrior Hu</string>                      -   ""
    <string>Jellyfish Baby</string>                        -   ""
    <string>Moss Flyer</string>                            -   ""
    <string>Garden Zombie</string>                         -   ""
    <string>Mantis Traitor Lord</string>                   -   ""
    <string>Moss Knight Fat</string>                       -   ""
    <string>Mantis Heavy Spawn</string>                    -   ""
    <string>Ghost Warrior Marmu</string>                   -   ""
    <string>Mega Jellyfish</string>                        -   ""
    <string>Ghost Warrior Xero</string>                    -   ""
    <string>Grave Zombie</string>                          -   ""
    <string>Crystal Crawler</string>                       -   ""
    <string>Zombie Miner</string>                          -   ""
    <string>Crystal Flyer</string>                         -   ""
    <string>Mines Crawler</string>                         -   ""
    <string>Mega Zombie Beam Miner</string>                -   ""
    <string>Zombie Beam Miner</string>                     -   ""
    <string>Zombie Beam Miner Rematch</string>             -   ""
    <string>Spider Mini</string>                           -   ""
    <string>Zombie Hornhead Sp</string>                    -   ""
    <string>Zombie Runner Sp</string>                      -   ""
    <string>Centipede Hatcher</string>                     -   ""
    <string>Mimic Spider</string>                          -   ""
    <string>Slash Spider</string>                          -   ""
    <string>Spider Flyer</string>                          -   ""
    <string>Ghost Warrior Galien</string>                  -   ""
    <string>Blow Fly</string>                              -   ""
    <string>Bee Hatchling Ambient</string>                 -   ""
    <string>Ghost Warrior Markoth</string>                 -   ""
    <string>Hornet Boss 2</string>                         -   ""
    <string>Abyss Crawler</string>                         -   ""
    <string>Parasite Balloon</string>                      -   ""
    <string>Flip Hopper</string>                           -   ""
    <string>Inflater</string>                              -   ""
    <string>Fluke Fly</string>                             -   ""
    <string>Flukeman</string>                              -   ""
    <string>Dung Defender</string>                         -   ""
    <string>fluke_baby_02</string>                         -   ""
    <string>fluke_baby_01</string>                         -   ""
    <string>fluke_baby_03</string>                         -   ""
    <string>Fluke Mother</string>                          -   ""
    <string>Enemy</string>                                 -   ""
    <string>Royal Gaurd</string>                           -   ""
    <string>Zombie Hive</string>                           -   ""
    <string>Bee Stinger</string>                           -   ""
    <string>Big Bee</string>                               -   ""
    <string>Hive Knight</string>                           -   ""
    <string>Grimm Boss</string>                            -   ""
    <string>Nightmare Grimm Boss</string>                  -   ""
    <string>Dream Mage Lord</string>                       -   ""
    <string>Dream Mage Lord Phase2</string>                -   ""
    <string>Hollow Knight Boss</string>                    -   ""
    <string>HK Prime</string>                              -   ""
    <string>Pale Lurker</string>                           -   ""
    <string>Oro</string>                                   -   ""
    <string>Mato</string>                                  -   ""
    <string>Sheo Boss</string>                             -   ""
    <string>Fat Fluke</string>                             -   ""
    <string>Sly Boss</string>                              -   ""
    <string>Hornet Nosk</string>                           -   ""
    <string>Super Spitter</string>                         -   ""
    <string>Colosseum_Armoured_Roller</string>             -   ""
    <string>Colosseum_Miner</string>                       -   ""
    <string>Colosseum_Shield_Zombie</string>               -   ""
    <string>Colosseum_Armoured_Mosquito</string>           -   ""
    <string>Colosseum_Flying_Sentry</string>               -   ""
    <string>Colosseum_Worm</string>                        -   ""
    <string>Mawlek Col</string>                            -   ""
    <string>Colosseum Grass Hopper</string>                -   "" 
    <string>Jellyfish</string>                             -   ""
    <string>Mantis Flyer Child</string>                    -   ""
    <string>Ghost Warrior Slug</string>                    -   ""
    <string>Lil Jellyfish</string>                         -   ""

    <string>Super Spitter Col</string>                     -  Shouldn't be their own enemies....
    <string>Giant Fly Col</string>                         -  Shouldn't be their own enemies....
    <string>Buzzer Col</string>                            -  Shouldn't be their own enemies....
    <string>Ceiling Dropper Col</string>                   -  Shouldn't be their own enemies....
    <string>Colosseum_Armoured_Roller R</string>           -  ""
    <string>Colosseum_Armoured_Mosquito R</string>         -  ""
    <string>Giant Buzzer Col</string>                      -  ""
    <string>Super Spitter R</string>                       -  ""
 */