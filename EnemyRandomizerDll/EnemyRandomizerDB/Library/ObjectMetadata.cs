using System.Collections;
using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using System.Linq;
using System.Reflection;
using static EnemyRandomizerMod.PrefabObject;
using UniRx;
using UniRx.Triggers;
using UniRx.Operators;
using HutongGames.PlayMaker.Actions;
using Satchel;
using Satchel.Futils;
using HutongGames.PlayMaker;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using System;

namespace EnemyRandomizerMod
{
    public class ObjectMetadata
    {
        public string SceneName { get; protected set; }

        public string ObjectName { get; protected set; }

        public string ScenePath { get; protected set; }

        public bool IsCustomPlayerDataName { get; protected set; }
        public string playerDataName { get; set; }
        public string PlayerDataName
        {
            get
            {
                return playerDataName;
            }
            set
            {
                IsCustomPlayerDataName = true;
                playerDataName = value;
            }
        }

        public override string ToString()
        {
            return $"[{SpawnerExtensions.ObjectType(ObjectName)}, [{SceneName}]://{ScenePath}]";
        }

        public void Dump()
        {
            var self = this;
            var props = self.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            props.ToList().ForEach(x => Dev.Log($"{x.Name}: {x.GetValue(self)}"));
        }

        public ObjectMetadata(GameObject sceneObject = null)
        {
            if (sceneObject != null)
            {
                ObjectName = sceneObject.name;
                ScenePath = sceneObject.GetSceneHierarchyPath();
                SceneName = (sceneObject.scene.IsValid() ? sceneObject.scene.name : null);
            }
        }

        public static ObjectMetadata Get(GameObject owner)
        {
            if (owner == null)
                return null;

            var defaultSpawnedEnemy = owner.GetComponent<SpawnedObjectControl>();
            if (defaultSpawnedEnemy != null)
            {
                return defaultSpawnedEnemy.thisMetadata;
            }
            else
            {
                return null;
            }
        }

        public static ObjectMetadata GetOriginal(GameObject owner)
        {
            if (owner == null)
                return null;

            var defaultSpawnedEnemy = owner.GetComponent<SpawnedObjectControl>();
            if (defaultSpawnedEnemy != null)
            {
                return defaultSpawnedEnemy.originialMetadata;
            }
            else
            {
                return null;
            }
        }
    }
}




