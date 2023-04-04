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

        public override List<PrefabObject> GetValidReplacements(ObjectMetadata sourceData, List<PrefabObject> validReplacementObjects)
        {
            if (sourceData.ObjectType == PrefabObject.PrefabType.Enemy)
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






        //public override GameObject ReplaceEnemy(GameObject other, List<PrefabObject> allowedReplacements)
        //{
        //    SetupRNGForReplacement(other.name, other.scene.name);
        //    var prefab = allowedEnemyReplacements.GetRandomElementFromList(rng);
        //    ObjectMetadata otherData = new ObjectMetadata();
        //    otherData.Setup(other, Database);
        //    GameObject newEnemy = Database.Spawn(prefab, otherData);
        //    newEnemy.SetParentToOthersParent(otherData);
        //    newEnemy.PositionNewEnemy(otherData);
        //    return newEnemy;
        //}

        //List<PrefabObject> allowedEffectReplacements;

        //public override GameObject ReplaceHazardObject(GameObject other)
        //{
        //    if (allowedEffectReplacements == null)
        //        allowedEffectReplacements = Database.hazardPrefabs.Where(x => !EnemyReplacer.ReplacementEffectsToSkip.Contains(x.prefabName)).ToList();

        //    SetupRNGForReplacement(other.name, other.scene.name);
        //    var prefab = allowedEffectReplacements.GetRandomElementFromList(rng);
        //    ObjectMetadata otherData = new ObjectMetadata();
        //    otherData.Setup(other, Database);
        //    GameObject newHazard = Database.Spawn(prefab, otherData);
        //    newHazard.SetParentToOthersParent(otherData);
        //    newHazard.PositionNewEnemy(otherData);
        //    return newHazard;
        //}

        //public override GameObject ReplacePooledObject(GameObject other)
        //{
        //    SetupRNGForReplacement(other.name, other.scene.name);
        //    var prefab = Database.effectPrefabs.GetRandomElementFromList(rng);
        //    ObjectMetadata otherData = new ObjectMetadata();
        //    otherData.Setup(other, Database);
        //    GameObject newEffect = Database.Spawn(prefab, otherData);
        //    newEffect.SetParentToOthersParent(otherData);
        //    newEffect.PositionNewEnemy(otherData);
        //    return newEffect;
        //}
    }
}
