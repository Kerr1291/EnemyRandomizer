using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using nv;

namespace EnemyRandomizerMod.Behaviours
{
    public class ArenaEnemy : EnemyBehaviour
    {
        bool hasDied = false;
        int hasLeftArena = 0;
        private void Update()
        {
            if (!bounds.Contains(transform.position) && !hasDied)
            {
                if(hasLeftArena < 5)
                {
                    transform.position = bounds.center;
                    hasLeftArena++;
                }
                Dev.Log(gameObject.name + " has left the arena bounds");
                _hm.hp = 1;
                _hm.ApplyExtraDamage(1000);
                hasDied = true;
            }
        }
    }
}
