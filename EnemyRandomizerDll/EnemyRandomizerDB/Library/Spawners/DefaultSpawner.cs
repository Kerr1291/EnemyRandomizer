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
        public virtual ObjectMetadata Spawn(PrefabObject p, ObjectMetadata source, EnemyRandomizerDatabase database)
        {
            GameObject gameObject = null;

            if(p.prefab == null)
            {
                Dev.LogError("Cannot Instantiate a null object!");
            }    

            try
            {
                gameObject = GameObject.Instantiate(p.prefab);
            }
            catch(Exception e)
            {
                Dev.LogError($"Error when trying to instantiate {p.prefab} from {p.prefabType} at {p.source.path} in {p.source.Name}");
            }

            if (gameObject == null)
                return null;

            if(source == null)
                gameObject.name = gameObject.name + "(" + System.Guid.NewGuid().ToString() + ")"; //name values in parenthesis will be trimmed out when converting to a database key'd name
            else
                gameObject.name = gameObject.name + $" ([{source.ObjectPosition.GetHashCode()}][{source.ScenePath.GetHashCode()}])"; //name values in parenthesis will be trimmed out when converting to a database key'd name

            return new ObjectMetadata(gameObject, database);
        }
    }

    public class DefaultSpawner<TControlComponent> : DefaultSpawner
        where TControlComponent : DefaultSpawnedEnemyControl
    {
        public override ObjectMetadata Spawn(PrefabObject p, ObjectMetadata source, EnemyRandomizerDatabase database)
        {
            var newObject = base.Spawn(p, source, database);
            var control = newObject.Source.GetOrAddComponent<TControlComponent>();
            if (source != null && source.IsPogoLogic)
            {
                GameObject.Destroy(control);
            }
            else
            {
                Dev.Log("enabling newly spawned object for setup");
                newObject.Source.SetActive(true);

                Dev.Log("starting setup of control component");
                control.Setup(source);

                Dev.Log("disabling newly setup object");
                newObject.Source.SetActive(false);

                Dev.Log("object disabled");
            }
            return newObject;
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

    public class CorpseRemover : MonoBehaviour
    {
        public string replacementEffect = "Pt Feather Burst";
        protected virtual void OnEnable()
        {
            EnemyRandomizerDatabase.CustomSpawnWithLogic(gameObject.transform.position, replacementEffect, null, true);
            GameObject.Destroy(gameObject);
        }
    }

    public class SpawnOnDestroy : MonoBehaviour
    {
        public string spawnEntity = "Fly";
        public bool didEnable = false;
        public bool didSpawn = false;

        public int? setHealthOnSpawn;

        protected virtual void OnEnable()
        {
            didEnable = true;
        }

        protected virtual void OnDestroy()
        {
            if (!didEnable)
                return;
            if (didSpawn)
                return;
            if (gameObject == null)
                return;
            if (!gameObject.scene.IsValid())
                return;
            if (!gameObject.scene.isLoaded)
                return;

            var active = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (!active.IsValid() || active.name != gameObject.scene.name)
                return;

            didSpawn = true;
            var thing = EnemyRandomizerDatabase.GetDatabase().Spawn(spawnEntity);
            thing.ObjectPosition = gameObject.transform.position;
            if (thing != null)
            {
                var thinghm = thing.EnemyHealthManager;
                if (setHealthOnSpawn != null && thinghm != null)
                {
                    thinghm.hp = setHealthOnSpawn.Value;
                }
            }
            thing.ActivateSource();
        }
    }

    public class ExplodeOnCorpseRemoved : MonoBehaviour
    {
        protected virtual void OnDestroy()
        {
            EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Gas Explosion Recycle L", null, true);
        }
    }

    public class PositionFixer : MonoBehaviour
    {
        public Vector2 positionLock;

        protected virtual void Update()
        {
            transform.position = positionLock;
        }
    }

    public class CustomTweener : MonoBehaviour
    {
        public float travelTime;
        public Vector3 from;
        public Vector3 to;

        float t;
        bool flipped;

        protected virtual void Update()
        {
            if(flipped)
            {
                gameObject.transform.position = Vector3.Slerp(to, from, t / travelTime);
            }
            else
            {
                gameObject.transform.position = Vector3.Slerp(from, to, t / travelTime);
            }

            t += Time.deltaTime;
            if (t >= travelTime)
            {
                t = 0f;
                flipped = !flipped;
            }
        }
    }
}
