using UnityEngine;
using System.Collections;
using System;

namespace EnemyRandomizerMod
{
    public class DefaultSpawnedEnemyControl : MonoBehaviour
    {
        public ObjectMetadata thisMetadata;
        public ObjectMetadata originialMetadata;

        public virtual void Setup(ObjectMetadata other)
        {
            thisMetadata = new ObjectMetadata();
            thisMetadata.Setup(gameObject, EnemyRandomizerDatabase.GetDatabase());
            originialMetadata = other;
        }
    }

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
            var go = GameObject.Instantiate(p.prefab);
            if(source == null)
                go.name = go.name + "(" + Guid.NewGuid().ToString() + ")"; //name values in parenthesis will be trimmed out when converting to a database key'd name
            else
                go.name = go.name + $" ([{source.ObjectPosition.GetHashCode()}][{source.ScenePath.GetHashCode()}])"; //name values in parenthesis will be trimmed out when converting to a database key'd name
            return go;
        }
    }

    public class DefaultSpawner<TControlComponent> : DefaultSpawner
        where TControlComponent : DefaultSpawnedEnemyControl
    {
        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            var newObject = base.Spawn(p, source);
            var control = newObject.GetComponent<TControlComponent>();
            control.Setup(source);
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

    //TODO: provide a component that will properly re-scale an enemy
}
