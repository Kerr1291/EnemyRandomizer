
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
 */