using System;

namespace EnemyRandomizerMod
{
    public class ColosseumCageZotePrefabConfig : IPrefabConfig
    {
        public virtual void SetupPrefab(PrefabObject p)
        {
            if (p.source.Scene.name == "Room_Colosseum_Bronze")
            {
                try
                {
                    var prefab = p.prefab;
                    prefab.name = "Arena Cage Zote";
                    string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);

                    Dev.Log("Loaded = " + p.prefab.name);

                    p.prefabName = prefab.name;
                    p.prefab = prefab;
                }
                catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
            }
            else
            {
            }
        }
    }




}
