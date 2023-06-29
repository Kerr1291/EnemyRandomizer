using UnityEngine;
using Satchel.Futils;

namespace EnemyRandomizerMod
{
    public interface IPrefabConfig
    {
        void SetupPrefab(PrefabObject p);
    }

    public interface ISpawner
    {
        GameObject Spawn(PrefabObject prefabToSpawn, GameObject objectToReplace);

        void ConfigureObject(GameObject newlySpawnedObject, GameObject objectToReplace);

        DefaultSpawnedEnemyControl AddController(GameObject newlySpawnedObject);

        void SetupSpawnedObject(DefaultSpawnedEnemyControl newlySpawnedObject);
    }
}
