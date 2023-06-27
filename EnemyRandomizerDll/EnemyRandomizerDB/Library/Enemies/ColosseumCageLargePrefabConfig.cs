using UnityEngine;
using System;

namespace EnemyRandomizerMod
{
    //this converts the colo cages into prefabs of the enemies that exist inside them
    public class ColosseumCageLargePrefabConfig : IPrefabConfig
    {
        public virtual void SetupPrefab(PrefabObject p)
        {
            if (p.source.Scene.name == "Room_Colosseum_Bronze" && p.source.path.Contains("(1)"))
            {
                try
                {
                    var prefab = p.prefab;
                    prefab.name = "Arena Cage Large";
                    string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);

                    Dev.Log("Loaded = " + p.prefab.name);

                    p.prefabName = prefab.name;
                    p.prefab = prefab;
                }
                catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
            }
            else
            {
                try
                {
                    var prefab = p.prefab.LocateMyFSM("Spawn").Fsm.GetFsmGameObject("Corpse to Instantiate").Value;

                    string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);

                    //get actual enemy prefab from the fsm
                    p.prefabName = keyName;
                    p.prefab = prefab;
                }
                catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
            }
        }

        public static string GetEnemyPrefabNameInsideCage(GameObject largeCage)
        {
            try
            {
                var prefab = largeCage.LocateMyFSM("Spawn").Fsm.GetFsmGameObject("Corpse to Instantiate").Value;

                string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);

                //get actual enemy prefab from the fsm
                return keyName;
            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
            return string.Empty;
        }
    }




}
