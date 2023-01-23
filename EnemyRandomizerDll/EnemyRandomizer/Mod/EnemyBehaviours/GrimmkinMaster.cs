using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyRandomizerMod.Behaviours
{
    class GrimmkinMaster : Grimmkin
    {
        private new void Awake()
        {
            grimmchildLevel = 2;
            base.Awake();
        }
    }
}
