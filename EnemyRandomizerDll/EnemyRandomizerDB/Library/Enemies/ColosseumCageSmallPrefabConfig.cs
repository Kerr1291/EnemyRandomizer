using UnityEngine;
using System;

namespace EnemyRandomizerMod
{
    //this converts the colo cages into prefabs of the enemies that exist inside them
    public class ColosseumCageSmallPrefabConfig : IPrefabConfig
    {
        public virtual void SetupPrefab(PrefabObject p)
        {

            if (p.source.Scene.name == "Room_Colosseum_Bronze" && p.source.path.Contains("(1)"))
            {
                try
                {
                    var prefab = p.prefab;
                    prefab.name = "Arena Cage Small";
                    string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);

                    Dev.Log("Loaded = " + p.prefab.name);

                    p.prefabName = prefab.name;
                    p.prefab = prefab;
                }
                catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
            }
            else
            {
                GameObject prefab = null;

                try
                {
                    if (prefab == null)
                        prefab = p.prefab.LocateMyFSM("Spawn").Fsm.GetFsmGameObject("Enemy Type").Value;
                }
                catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }

                try
                {
                    if (prefab == null)
                        prefab = p.prefab.LocateMyFSM("Spawn").Fsm.GetFsmGameObject("z Corpse to Instantiate").Value;
                }
                catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }


                string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);

                //get actual enemy prefab from the fsm
                p.prefabName = keyName;
                p.prefab = prefab;
            }
        }

        public static string GetEnemyPrefabNameInsideCage(GameObject smallCage)
        {
            GameObject prefab = null;
            try
            {
                prefab = smallCage.LocateMyFSM("Spawn").Fsm.GetFsmGameObject("Enemy Type").Value;
            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }

            try
            {
                if (prefab == null)
                    prefab = smallCage.LocateMyFSM("Spawn").Fsm.GetFsmGameObject("z Corpse to Instantiate").Value;
            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }

            if (prefab != null)
            {
                string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);

                //get actual enemy prefab from the fsm
                return keyName;
            }
            return string.Empty;
        }
    }




}
