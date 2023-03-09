using UnityEngine;
using System.Collections;
using System;

namespace EnemyRandomizerMod
{
    internal class DefaultPrefabConfig : IPrefabConfig
    {
        public virtual void SetupPrefab(PrefabObject p)
        {
            p.prefabName = EnemyRandomizerDatabase.ToDatabaseKey(p.prefab.name);
        }
    }

    internal class DefaultSpawner : ISpawner
    {
        public virtual GameObject Spawn(PrefabObject p)
        {
            var go = GameObject.Instantiate(p.prefab);
            go.name = go.name + "(" + Guid.NewGuid().ToString() + ")"; //name values in parenthesis will be trimmed out when converting to a database key'd name
            return go;
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
