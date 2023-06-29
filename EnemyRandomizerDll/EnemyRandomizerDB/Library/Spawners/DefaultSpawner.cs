using UnityEngine;
using System;
using On.Language;

namespace EnemyRandomizerMod
{
    public class DefaultSpawner<TControlComponent> : DefaultSpawner
        where TControlComponent : DefaultSpawnedEnemyControl
    {
        public override DefaultSpawnedEnemyControl AddController(GameObject newlySpawnedObject)
        {
            if (SpawnedObjectControl.VERBOSE_DEBUG)
                Dev.Log($"starting setup of control component of type {typeof(TControlComponent).Name} for {newlySpawnedObject.GetSceneHierarchyPath()}");
            return newlySpawnedObject.GetOrAddComponent<TControlComponent>();
        }
    }

    public class DefaultSpawner : ISpawner
    {
        public virtual bool corpseRemovedByEffect => false;
        public virtual string corpseRemoveEffectName => null;

        public virtual bool spawnEffectOnCorpseRemoved => false;
        public virtual string spawnEffectOnCorpseRemovedEffectName => null;

        public virtual EnemyRandomizerDatabase defaultDatabase => EnemyRandomizerDatabase.GetDatabase();

        public virtual GameObject Spawn(PrefabObject prefabToSpawn, GameObject objectToReplace)
        {
            GameObject gameObject = null;

            if (prefabToSpawn.prefab == null)
            {
                Dev.LogError($"Cannot Instantiate a null object {prefabToSpawn}!");
                return null;
            }    

            try
            {
                gameObject = GameObject.Instantiate(prefabToSpawn.prefab);
            }
            catch(Exception e)
            {
                Dev.LogError($"Error when trying to instantiate {prefabToSpawn}");
            }

            if (gameObject == null)
                return null;

            ConfigureObject(gameObject, objectToReplace);

            return gameObject;
        }

        public void ConfigureObject(GameObject gameObject, GameObject objectToReplace)
        {
            //format the name cleanly (remove the (clone) part)
            gameObject.name = EnemyRandomizerDatabase.ToDatabaseKey(gameObject.name);

            //apply a unique identifier
            if (objectToReplace == null)
                gameObject.name = gameObject.name + "(" + System.Guid.NewGuid().ToString() + ")"; //name values in parenthesis will be trimmed out when converting to a database key'd name
            else
                gameObject.name = gameObject.name + $" ([{objectToReplace.GetObjectPrefab().prefab.transform.position.GetHashCode()}][{objectToReplace.GetSceneHierarchyPath().GetHashCode()}])"; //name values in parenthesis will be trimmed out when converting to a database key'd name

            //parent it properly
            if (objectToReplace != null)
            {
                gameObject.SetParentToOthersParent(objectToReplace);
                gameObject.transform.position = objectToReplace.transform.position;
            }

            //don't do any more setup if it's replacing a pogo logic enemy
            if (objectToReplace != null && objectToReplace.CheckIfIsPogoLogicType())
            {
                if (gameObject.GetComponent<SpawnedObjectControl>() != null)
                {
                    throw new InvalidOperationException("This object was already setup (somehow)");
                }

                var spawnedControl = gameObject.AddComponent<SpawnedObjectControl>();

                spawnedControl.thisMetadata = new ObjectMetadata(gameObject);
                spawnedControl.Setup(objectToReplace);
            }
            else if (gameObject.CheckIfIsCustomArenaCageType())
            {
                if (gameObject.GetComponent<SpawnedObjectControl>() != null)
                {
                    throw new InvalidOperationException("This object was already setup (somehow)");
                }

                var spawnedControl = gameObject.AddComponent<SpawnedObjectControl>();

                spawnedControl.thisMetadata = new ObjectMetadata(gameObject);
                spawnedControl.Setup(objectToReplace);
            }
            else
            {
                var controller = AddController(gameObject);

                controller.thisMetadata = new ObjectMetadata(gameObject);
                controller.Setup(objectToReplace);

                SetupSpawnedObject(controller);
            }
        }

        /// <summary>
        /// Override this to add a different controller than the default
        /// </summary>
        public virtual DefaultSpawnedEnemyControl AddController(GameObject newlySpawnedObject)
        {
            if(SpawnedObjectControl.VERBOSE_DEBUG)
                Dev.Log($"starting setup of control component of type {typeof(DefaultSpawnedEnemyControl).Name} for {newlySpawnedObject.GetSceneHierarchyPath()}");
            return newlySpawnedObject.GetOrAddComponent<DefaultSpawnedEnemyControl>();
        }

        public virtual void SetupSpawnedObject(DefaultSpawnedEnemyControl newlySpawnedObject)
        {
            var gameObject = newlySpawnedObject.gameObject;

            if (corpseRemovedByEffect)
            {
                if (GameManager.instance.GetCurrentMapZone() != "FINAL_BOSS")
                {
                    //default effect = death explode boss
                    string removeEffect = string.IsNullOrEmpty(corpseRemoveEffectName) ? "Death Explode Boss" : corpseRemoveEffectName;

                    GameObject corpse = gameObject.GetCorpseObject();
                    if (corpse != null)
                    {
                        corpse.AddCorpseRemoverWithEffect(gameObject, removeEffect);
                    }
                    else
                    {
                        if (SpawnedObjectControl.VERBOSE_DEBUG)
                            Dev.LogWarning($"{gameObject} has no corpse to apply an effect to!");
                    }
                }
            }

            if (spawnEffectOnCorpseRemoved)
            {
                if (GameManager.instance.GetCurrentMapZone() != "FINAL_BOSS")
                {
                    //default effect = death explode boss
                    string removeEffect = string.IsNullOrEmpty(corpseRemoveEffectName) ? "Death Explode Boss" : corpseRemoveEffectName;

                    GameObject corpse = gameObject.GetCorpseObject();
                    if (corpse != null)
                    {
                        corpse.AddEffectSpawnerOnCorpseRemoved(gameObject, removeEffect);
                    }
                    else
                    {
                        if (SpawnedObjectControl.VERBOSE_DEBUG)
                            Dev.LogWarning($"{gameObject} has no corpse to apply an effect to!");
                    }
                }
            }
        }
    }
}
