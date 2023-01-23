using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyRandomizerMod.Behaviours
{
    class GrimmkinNovice : Grimmkin
    {
        private new void Awake()
        {
            grimmchildLevel = 1;
            base.Awake();
        }
    }
}
