using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EnemyRandomizerMod
{
    public class RandomizeEachEnemyToZote : BaseRandomizerLogic
    {
        public override string Name => "Zote Mode";

        public override string Info => "Precept Forty-Five: 'One Thing Is Not Another'. This one should be obvious, but I've had others try to argue that one thing, which is clearly what it is and not something else, is actually some other thing, which it isn't. Stay on your guard!";

        public override bool EnableByDefault => false;

        public override List<PrefabObject> GetValidReplacements(GameObject sourceData, List<PrefabObject> validReplacementObjects)
        {
            if (sourceData.ObjectType() == PrefabObject.PrefabType.Enemy)
            {
                return GetValidEnemyReplacements(validReplacementObjects);
            }

            return validReplacementObjects;
        }

        public virtual List<PrefabObject> GetValidEnemyReplacements(List<PrefabObject> validReplacements)
        {
            return validReplacements.Where(x => x.prefabName.ToLower().Contains("zote") || x.prefabName.ToLower().Contains("prince")).ToList();
        }

        (string,string,bool) CustomOption = ("Allow Custom Zotes", "[NOT YET IMPLEMENTED] Precept Thirty-Two: 'Names Have Power'. Names have power, and so to name something is to grant it power. I myself named my nail 'Life Ender'. Do not steal the name I came up with! Invent your own!", false);

        protected override List<(string Name, string Info, bool DefaultState)> ModOptions
        {
            get => new List<(string, string, bool)>()
            {
                CustomOption,
            };
        }
    }
}
