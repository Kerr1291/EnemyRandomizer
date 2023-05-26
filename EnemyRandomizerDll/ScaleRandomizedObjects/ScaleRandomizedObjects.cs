using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using HutongGames.PlayMaker.Actions;
using Satchel.BetterMenus;

namespace EnemyRandomizerMod
{
    public class ScaleRandomizedObjects : BaseRandomizerLogic
    {
        public override string Name => "Enemy Size Changer";

        public override string Info => "Changes the size of enemies";

        public override bool EnableByDefault => true;

        List<(string Name, string Info, bool Default)> CustomOptions = new List<(string, string, bool)>()
        {
            ("Match Audio to Scaling", "Should enemies have their sounds changed too?", true),
            //("Scale Enemies", "Should this effect randomized enemies?", true),
            //("Scale Hazards", "Should this effect randomized hazards?", false),
            //("Scale Effects", "Should this effect randomized effects?", false),
        };

        List<(string Name, string Info, bool Default)> ScalingTypes = new List<(string, string, bool)>()
        {
            ("Match", "New enemies match the size of the original enemy. Best for stability.", true),
            ("Random", "Scale each enemy randomly", false),
            ("All Big", "Giant Enemies Everywhere (WARNING Unstable: Many enemies will not fit places...)", false),
            ("All Tiny", "Tiny Enemies Everywhere", false),
        };

        protected override List<(string Name, string Info, bool DefaultState)> ModOptions
        {
            get => CustomOptions;
        }

        public override void Setup(EnemyRandomizerDatabase database)
        {
            base.Setup(database);
            EnemyRandomizer.Instance.enemyReplacer.loadedLogics.Add(this);
        }


        public override void ModifyObject(GameObject objectToModify, GameObject originalObject)
        {
            if (objectToModify.ObjectType() == PrefabObject.PrefabType.Enemy)
            {
                ScaleObject(objectToModify, originalObject);
            }
        }

        public override List<Element> GetEntries()
        {
            var result = base.GetEntries();


            var setting = ScalingTypes.FirstOrDefault(x => Settings.GetOption(x.Name).value).Name;

            string subpageID = "Niice";
            var elems = new List<Element>();
            var types = new string[] { "Match","Random","All Big","All Tiny" };
            var desc = new string[] {
                "New enemies match the size of the original enemy. Best for stability",
                "Scale each enemy randomly",
                "Giant Enemies Everywhere",
                "Tiny Enemies Everywhere",
            };
            elems.Add(new TextPanel("Choose 1"));
            elems.Add(new HorizontalOption("Enemy Scaling Type", "",
                    types, i =>
                    {
                        setting = types[i];
                        (EnemyRandomizer.SubPages[Name].Find(subpageID) as HorizontalOption).Description = $"{desc[i]}";
                        EnemyRandomizer.SubPages[Name].Update();

                        ScalingTypes.ForEach(x => Settings.GetOption(x.Name).value = false);
                        Settings.GetOption(setting).value = true;
                    },
                    () =>
                    {
                        ScalingTypes.ForEach(x => Settings.GetOption(x.Name).value = false);
                        Settings.GetOption(setting).value = true;

                        return types.ToList().IndexOf(setting);
                    },
                    Id: subpageID
                    )
                );

            return result.Concat(elems).ToList();
        }

        public virtual void ScaleObject(GameObject objectToModify, GameObject originalObject)
        {
            float scale = 1f;
            if (Settings.GetOption(ScalingTypes[0].Name).value)
            {
                scale = ApplyRelativeSizeScale(objectToModify, originalObject);
            }
            else
            if (Settings.GetOption(ScalingTypes[1].Name).value)
            {
                scale = DoRandomScale(objectToModify);
            }
            else
            if (Settings.GetOption(ScalingTypes[2].Name).value)
            {
                scale = DoBigScale(objectToModify);
            }
            else
            if (Settings.GetOption(ScalingTypes[3].Name).value)
            {
                scale = DoTinyScale(objectToModify);
            }

            //scale audio?
            if (Settings.GetOption(CustomOptions[0].Name).value)
            {
                objectToModify.SetAudioToMatchScale(scale);
            }
        }

        private static float DoBigScale(GameObject objectToModify)
        {
            float scale;
            RNG prng = new RNG();
            prng.Seed = objectToModify.ObjectName().GetHashCode() + objectToModify.SceneName().GetHashCode();

            scale = prng.Rand(2.2f, 3f);

            objectToModify.ScaleObject(scale);
            return scale;
        }

        private static float DoTinyScale(GameObject objectToModify)
        {
            float scale;
            RNG prng = new RNG();
            prng.Seed = objectToModify.ObjectName().GetHashCode() + objectToModify.SceneName().GetHashCode();

            scale = prng.Rand(.2f, .4f);

            objectToModify.ScaleObject(scale);
            return scale;
        }

        private static float DoRandomScale(GameObject objectToModify)
        {
            float scale;
            RNG prng = new RNG();
            prng.Seed = objectToModify.ObjectName().GetHashCode() + objectToModify.SceneName().GetHashCode();

            //50/50 chance to be randomly big or small
            bool big = prng.CoinToss();
            if (big)
            {

                scale = prng.Rand(1f, 3f);
            }
            else//smol
            {

                scale = prng.Rand(.2f, 1f);
            }

            objectToModify.ScaleObject(scale);
            return scale;
        }

        public virtual float ApplyRelativeSizeScale(GameObject objectToModify, GameObject originalObject)
        {
            float scale = 1f;
            if (originalObject != null)
            {
                scale = objectToModify.GetRelativeScale(originalObject, .2f);
                if(SpawnedObjectControl.VERBOSE_DEBUG)
                    Dev.Log($"{Name}:ApplyRelativeSizeScale()// Relative scale of [{objectToModify.GetDatabaseKey()} / {originalObject.GetDatabaseKey()} = {scale}]");
                objectToModify.ScaleObject(scale);
            }
            return scale;
        }

        /// <summary>
        /// Used to optimize the randomizer, does this logic, if enabled, replace a given type?
        /// </summary>
        public override bool WillReplaceType(PrefabObject.PrefabType prefabType)
        {
            return false;
        }

        /// <summary>
        /// Used to optimize the randomizer, does this logic, if enabled, modify a given type?
        /// </summary>
        public override bool WillModifyType(PrefabObject.PrefabType prefabType)
        {
            return prefabType == PrefabObject.PrefabType.Enemy;
            //return prefabType == PrefabObject.PrefabType.Enemy && Settings.GetOption(CustomOptions[0].Name).value ||
            //       prefabType == PrefabObject.PrefabType.Hazard && Settings.GetOption(CustomOptions[1].Name).value ||
            //       prefabType == PrefabObject.PrefabType.Effect && Settings.GetOption(CustomOptions[2].Name).value;
        }
    }
}




//public override void ModifyObject(GameObject objectToModify, GameObject originalObject)
//{
//    if (objectToModify.ObjectType() == PrefabObject.PrefabType.Enemy && Settings.GetOption(CustomOptions[0].Name).value)
//    {
//        ScaleObject(objectToModify, originalObject);
//    }

//    else if (objectToModify.ObjectType() == PrefabObject.PrefabType.Hazard && Settings.GetOption(CustomOptions[1].Name).value)
//    {
//        ScaleObject(objectToModify, originalObject);
//    }

//    else if (objectToModify.ObjectType() == PrefabObject.PrefabType.Effect && Settings.GetOption(CustomOptions[2].Name).value)
//    {
//        ScaleObject(objectToModify, originalObject);
//    }
//}