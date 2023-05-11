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

    public class DefaultPrefabConfig<TControlComponent> : DefaultPrefabConfig
        where TControlComponent : DefaultSpawnedEnemyControl
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);
            p.prefab.AddComponent<TControlComponent>();
        }
    }

    public class DefaultSpawner : ISpawner
    {
        public virtual GameObject Spawn(PrefabObject p, ObjectMetadata source)
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
            return gameObject;
        }
    }

    public class DefaultSpawner<TControlComponent> : DefaultSpawner
        where TControlComponent : DefaultSpawnedEnemyControl
    {
        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            var newObject = base.Spawn(p, source);
            var control = newObject.GetOrAddComponent<TControlComponent>();
            if (source == null || !source.IsPogoLogic)
            {
                control.Setup(source);
            }
            else
            {
                GameObject.Destroy(control);
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
