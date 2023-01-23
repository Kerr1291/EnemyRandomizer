using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyRandomizerMod.Behaviours
{
    class GrimmkinNightmare : Grimmkin
    {
        private new void Awake()
        {
            grimmchildLevel = 3;
            base.Awake();
        }
    }
}
