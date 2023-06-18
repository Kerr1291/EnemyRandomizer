using UnityEngine;
using System.Collections;
using System;
using On.Language;

namespace EnemyRandomizerMod
{
    public class DefaultPrefabConfig : IPrefabConfig
    {
        public virtual void SetupPrefab(PrefabObject p)
        {
            p.prefabName = EnemyRandomizerDatabase.ToDatabaseKey(p.prefab.name);
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
            else if(gameObject.CheckIfIsCustomArenaCageType())
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

            return gameObject;
        }

        /// <summary>
        /// Override this to add a different controller than the default
        /// </summary>
        public virtual DefaultSpawnedEnemyControl AddController(GameObject newlySpawnedObject)
        {
            Dev.Log($"starting setup of control component of type {typeof(DefaultSpawnedEnemyControl).Name} for {newlySpawnedObject.GetSceneHierarchyPath()}");
            return newlySpawnedObject.GetOrAddComponent<DefaultSpawnedEnemyControl>();
        }

        public virtual void SetupSpawnedObject(DefaultSpawnedEnemyControl newlySpawnedObject)
        {
            var gameObject = newlySpawnedObject.gameObject;

            if (corpseRemovedByEffect)
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
                    Dev.LogWarning($"{gameObject} has no corpse to apply an effect to!");
                }
            }

            if (spawnEffectOnCorpseRemoved)
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
                    Dev.LogWarning($"{gameObject} has no corpse to apply an effect to!");
                }
            }
        }
    }

    public class DefaultSpawner<TControlComponent> : DefaultSpawner
        where TControlComponent : DefaultSpawnedEnemyControl
    {
        public override DefaultSpawnedEnemyControl AddController(GameObject newlySpawnedObject)
        {
            Dev.Log($"starting setup of control component of type {typeof(TControlComponent).Name} for {newlySpawnedObject.GetSceneHierarchyPath()}");
            return newlySpawnedObject.GetOrAddComponent<TControlComponent>();
        }
    }

    public abstract class SpawnEffect : MonoBehaviour
    {
        public const string NONE = "NONE";

        public string effectToSpawn = "Gas Explosion Recycle L";
        public virtual bool destroyGameObject => false;
        public virtual bool allowRandomizationOfSpawn => false;
        public bool isSpawnerEnemy = false;
        public bool activateOnSpawn = true;

        public Action<SpawnEffect, GameObject> onSpawn;

        protected virtual void Spawn(GameObject target, Vector2 pos)
        {
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

    public class CorpseRemover : SpawnEffect
    {
        public bool useOwner = false;
        public GameObject owner;

        public GameObject corpseTarget;
        public Vector2 lastPos;

        public override bool destroyGameObject => true;

        public bool didSpawn = false;
        public bool preRemoveCorpse = true;

        protected virtual void Update()
        {
            if (owner != null)
                lastPos = owner.transform.position;
            CheckRemoveAndSpawn();
        }

        protected virtual void CheckRemoveAndSpawn()
        {
            if (didSpawn)
                return;

            if (preRemoveCorpse)
            {
                if (corpseTarget != null)
                {
                    SpawnerExtensions.DestroyObject(corpseTarget);
                    corpseTarget = null;
                }

                if (!useOwner)
                {
                    Spawn(owner, lastPos);
                }
            }

            if (useOwner)
            {
                bool hasHm = owner != null && owner.GetComponent<HealthManager>() != null;
                bool isDead = false;
                if (owner != null &&
                    owner.GetComponent<HealthManager>() != null &&
                    (owner.GetComponent<HealthManager>().hp <= 0 || owner.GetComponent<HealthManager>().isDead))
                    isDead = true;

                if (owner == null || 
                   (!hasHm && !owner.activeInHierarchy) || 
                    isDead)
                {
                    Spawn(owner, lastPos);
                    didSpawn = true;
                }

                //var parent = transform.parent;
                //if (parent == null)
                //{
                //    Spawn(owner, lastPos);
                //    didSpawn = true;
                //}
                //else
                //{
                    //var parentName = parent.name;
                    //var thisName = gameObject.name;

                    //parentName = EnemyRandomizerDatabase.ToDatabaseKey(parentName);
                    //thisName = thisName.Remove("Corpse");
                    //thisName = EnemyRandomizerDatabase.ToDatabaseKey(thisName);

                    //if (!parentName.Contains(thisName))
                    //{
                    //    Spawn(owner, lastPos);
                    //    didSpawn = true;
                    //}
                //}
            }
        }
    }

    public class SpawnEffectOnDestroy : SpawnEffect
    {
        public override bool allowRandomizationOfSpawn => allowRandomization;

        public bool allowRandomization = false;
        public bool forceDestroyIfRendererBecomesDisabled = true;
        MeshRenderer mRenderer;
        bool wasEnabled = false;

        protected virtual void OnEnable()
        {
            mRenderer = GetComponent<MeshRenderer>();
        }

        protected virtual void OnDestroy() { Spawn(gameObject, transform.position); }

        protected virtual void Update()
        {
            if (mRenderer == null)
                return;

            if(!wasEnabled)
                wasEnabled = mRenderer.enabled;

            if (wasEnabled && mRenderer.enabled == false)
            {
                GameObject.Destroy(gameObject);
            }
        }
    }

    public class CorpseOrientationFixer : MonoBehaviour
    {
        public float corpseAngle;
        public float timeout = 5f;

        IEnumerator Start()
        {
            while (timeout > 0f)
            {
                var angles = transform.localEulerAngles;
                angles.z = corpseAngle;
                transform.localEulerAngles = angles;
                yield return null;
                timeout -= Time.deltaTime;
            }

            yield break;
        }
    }

    public class PositionLocker : MonoBehaviour
    {
        public Vector2? positionLock;

        protected virtual void Update()
        {
            if(positionLock != null)
                transform.position = positionLock.Value;
        }
    }
}
