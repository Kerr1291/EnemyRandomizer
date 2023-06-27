using UnityEngine;
using System;

namespace EnemyRandomizerMod
{
    public abstract class SpawnEffect : MonoBehaviour
    {
        public const string NONE = "NONE";

        public string effectToSpawn = "Gas Explosion Recycle L";
        public virtual bool destroyGameObject => false;
        public virtual bool allowRandomizationOfSpawn => false;
        public bool isSpawnerEnemy = false;
        public bool activateOnSpawn = true;
        protected bool isUnloading = false;

        public Action<SpawnEffect, GameObject> onSpawn;

        protected virtual void Awake()
        {
            GameManager.instance.UnloadingLevel -= SetUnloading;
            GameManager.instance.UnloadingLevel += SetUnloading;
        }

        protected virtual void OnDestroy()
        {
            GameManager.instance.UnloadingLevel -= SetUnloading;
        }

        void SetUnloading()
        {
            isUnloading = true;
        }

        protected virtual void Spawn(GameObject target, Vector2 pos)
        {
            if (isUnloading)
                return;

            if (target == null)
                target = gameObject;

            try
            {
#if DEBUG
                if (SpawnedObjectControl.VERBOSE_DEBUG)
                {
                    var metaInfo = ObjectMetadata.Get(target);
                    //var soc = gameObject.GetComponent<SpawnedObjectControl>();
                    if (metaInfo == null)
                        metaInfo = new ObjectMetadata(target);

                    Dev.Log($"A SpawnEffect attached to {metaInfo} is being invoked");
                }
#endif

                if (!target.IsInAValidScene())
                    return;

                if ((effectToSpawn != NONE && !string.IsNullOrEmpty(effectToSpawn)) || (isSpawnerEnemy && allowRandomizationOfSpawn))
                {
                    GameObject spawned = null;
                    if (isSpawnerEnemy && allowRandomizationOfSpawn)
                    {
                        spawned = SpawnerExtensions.SpawnEnemyForEnemySpawner(pos, activateOnSpawn, null);
                    }
                    else
                    {
                        spawned = SpawnerExtensions.SpawnEntityAt(effectToSpawn, pos, null, activateOnSpawn, allowRandomizationOfSpawn);
                    }

                    onSpawn?.Invoke(this, spawned);
                }
            }
            catch (Exception e) { Dev.Log($"Caught exception in spawn effect \n{e.Message}\n{e.StackTrace} "); }

            EnemyRandomizerDatabase.ClearBypass();

            if (destroyGameObject)
                GameObject.Destroy(target);

            GameObject.Destroy(gameObject);
        }
    }
}
