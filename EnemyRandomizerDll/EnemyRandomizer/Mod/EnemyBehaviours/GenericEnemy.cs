using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyRandomizerMod.Behaviours
{
    public class GenericEnemy : EnemyBehaviour
    {
        private void Start()
        {
            foreach (FsmState state in _fsm.FsmStates)
            {
                if (state.Name == "Init")
                {
                    _fsm.SetState("Init");
                    return;
                }
                if (state.Name == "Initialise")
                {
                    _fsm.SetState("Initialise");
                    return;
                }
            }

            //else if (goName.Contains("Grimm Boss") && !goName.Contains("Nightmare"))
            //{
            //    //GameObject spikeHolder = Instantiate(CustomTrial.GameObjects["grimmspikeholder"], new Vector2(ArenaInfo.CenterX, ArenaInfo.BottomY - 3), Quaternion.identity);
            //    //spikeHolder.SetActive(true);
            //}

            //else if (goName.Contains("Nightmare Grimm Boss"))
            //{
            //    //GameObject spikeHolder = Instantiate(CustomTrial.GameObjects["nightmaregrimmspikeholder"], new Vector2(ArenaInfo.CenterX, ArenaInfo.BottomY - 3), Quaternion.identity);
            //    //spikeHolder.SetActive(true);
            //}

            //else if (goName.Contains("Mega Jellyfish GG"))
            //{
            //    //GameObject jellyfishSpawner = Instantiate(CustomTrial.GameObjects["jellyfishspawner"], new Vector2(ArenaInfo.CenterX, ArenaInfo.CenterY), Quaternion.identity);
            //    //jellyfishSpawner.SetActive(true);
            //    //jellyfishSpawner.AddComponent<JellyfishSpawner>();
            //    //GameObject multizaps = Instantiate(CustomTrial.GameObjects["megajellyfishmultizaps"], new Vector2(ArenaInfo.CenterX, ArenaInfo.CenterY), Quaternion.identity);
            //    //multizaps.SetActive(true);
            //}
        }
    }
}
