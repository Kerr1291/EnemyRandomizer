using System;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using UnityEngine;
using nv;

namespace EnemyRandomizerMod
{    
    public static class FSMUtils
    {
        public static bool RemoveState(this HutongGames.PlayMaker.Fsm fsm, string stateName)
        {
            if (fsm.States.Any(x => x.Name == stateName))
            {
                fsm.States = fsm.States.Where(x => x.Name != stateName).ToArray();
                return true;
            }

            return false;
        }
    }
}
