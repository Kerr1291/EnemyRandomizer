﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using nv;
using UniRx;

namespace EnemyRandomizerMod
{
    public class ManagedObject : MonoBehaviour
    {
        public GameObject sourceEnemy;
        public string originalGameObjectPath;
        public string sourceDatabaseKey;
        public EnemyReplacer replacer;
        public string myDatabaseKey;
        public bool replaced;

        /// <summary>
        /// True if this has not been replaced
        /// </summary>
        public bool ThisIsSourceObject { get; protected set; }

        /// <summary>
        /// Pass "null" for the source if setting up a custom enemy
        /// </summary>
        public virtual void Setup(EnemyReplacer replacer, GameObject source)
        {
            ThisIsSourceObject = (source == gameObject);

            this.replacer = replacer;

            if (source != null)
            {
                this.originalGameObjectPath = source.GetSceneHierarchyPath();
                sourceDatabaseKey = EnemyRandomizerDatabase.ToDatabaseKey(source.name);
            }

            myDatabaseKey = EnemyRandomizerDatabase.ToDatabaseKey(name);
            sourceEnemy = source;

            if (!ThisIsSourceObject)
                replaced = true;
        }
    }

    public class BattleManagedObject : ManagedObject
    {
        public BattleWave myWave;
        int customWave = -1;

        public override void Setup(EnemyReplacer replacer, GameObject source)
        {
            base.Setup(replacer, source);

            RegisterWithBattleManager();
        }

        public virtual void SetCustomWave(int newWave)
        {
            int oldWave = GetMyWave();
            customWave = newWave;
            RegisterWithBattleManager();
            UnregisterWithBattleManager(oldWave);
        }

        public virtual void RegisterWithBattleManager()
        {
            BattleManager.StateMachine.Value.RegisterEnemy(this);
        }

        public virtual void UnregisterWithBattleManager()
        {
            BattleManager.StateMachine.Value.UnregisterEnemy(this);
        }

        public virtual void UnregisterWithBattleManager(int oldWave)
        {
            BattleManager.StateMachine.Value.UnregisterEnemy(this, oldWave);
        }

        public bool IsWaveInName()
        {
            if (string.IsNullOrEmpty(originalGameObjectPath))
                return false;

            return originalGameObjectPath.Contains("Battle Scene/Wave");
        }

        public int GetBattleWaveFromName()
        {
            if (string.IsNullOrEmpty(originalGameObjectPath))
                return customWave;

            var wavePart = originalGameObjectPath.Split('/').FirstOrDefault(x => x.Contains("Wave"));
            var endPart = wavePart.Split(' ').Last();
            int numberPart = int.Parse(endPart);
            return numberPart;
        }

        public virtual int GetMyWave()
        {
            if (string.IsNullOrEmpty(originalGameObjectPath))
                return customWave;

            var fullPath = originalGameObjectPath.Split('/');
            if(fullPath.Length > 1)
            {
                if(IsWaveInName())
                {
                    return GetBattleWaveFromName();
                }

                List<string> pathRemaining = fullPath.ToList();

                string myPath = originalGameObjectPath;
                int wave = -1;
                //try partial match if full match doesn't work
                while (!StateMachines.BattleWaveMap.TryGetValue(myPath, out wave))
                {
                    wave = -1;
                    pathRemaining.RemoveAt(pathRemaining.Count - 1);
                    if (pathRemaining.Count <= 0)
                        break;

                    myPath = string.Join(@"/", pathRemaining);
                }

                if (wave >= 0)
                    return wave;

                bool hasWave = StateMachines.BattleWaveMap.ContainsKey(myPath);

                if (hasWave && wave < 0)
                {
                    return wave;
                }
            }
            else
            {
                string myPath = originalGameObjectPath;
                if (StateMachines.BattleWaveMap.TryGetValue(myPath, out int wave))
                {
                    return wave;
                }
            }

            return 0;
        }
    }
}
