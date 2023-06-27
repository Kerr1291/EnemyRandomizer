using System;

namespace EnemyRandomizerMod
{
    public class MusicPrefabConfig : IPrefabConfig
    {
        public virtual void SetupPrefab(PrefabObject p)
        {
            if (p.source.Scene.name == "GG_Mighty_Zote")
            {
                try
                {
                    var prefab = p.prefab;
                    prefab.name = "Zote Music";
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
