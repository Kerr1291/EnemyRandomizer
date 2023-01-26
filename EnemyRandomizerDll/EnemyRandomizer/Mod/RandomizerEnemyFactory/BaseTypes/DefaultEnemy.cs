using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using nv;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using System.Reflection;
using System.Collections;

namespace EnemyRandomizerMod
{
    public class DefaultEnemy : IRandomizerEnemy
    {
        public virtual EnemyData Data { get; protected set; }
        public virtual GameObject Prefab { get; protected set; }

        public virtual bool IsBoss { get => Data.isBoss; }

        public virtual bool IsFlyer { get { return Data.enemyType.Contains("FLYER"); } }

        public virtual int Difficulty { get => CalculateDifficulty(); }

        public virtual void LinkDataObjects(EnemyData enemy, List<EnemyData> knownEnemyTypes, GameObject prefabObject)
        {
            Data = enemy;
            Prefab = prefabObject;
        }

        public virtual void SetupPrefab()
        {
            Dev.Log($"Setting up prefab for {Data.name}");

            //string typeName = typeof(HutongGames.PlayMaker.Actions.GetColliderRange).Name;
            //try
            //{
            //    var result = Prefab.FindFSMAction(typeName);

            //    {
            //        if (result.state != null)
            //        {
            //            try
            //            {
            //                if(result.state.Fsm.FsmComponent == null)
            //                {
            //                    result.state.Fsm.Owner = Prefab.GetComponentInChildren<PlayMakerFSM>(true);
            //                }

            //                Dev.Log($"state {result.state == null}");
            //                Dev.Log($"fsm {result.state.Fsm == null}");
            //                Dev.Log($"fsm c {result.state.Fsm.FsmComponent == null}");

            //                Dev.Log($"Found GetColliderRange on {result.state} in {result.state.Fsm} in {Prefab.name}");
            //            }
            //            catch (Exception e)
            //            {
            //                Dev.LogError("failed to print found log");
            //            }
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    Dev.LogError("failed search for fsm action " + typeName + " in " + Prefab.name);
            //}

            //Dev.Log($"Invoking AddComponent DefaultEnemyController");
            Prefab.AddComponent<DefaultEnemyController>();
        }

        public virtual void DebugPrefab()
        {
            //string typeName = typeof(HutongGames.PlayMaker.Actions.GetColliderRange).Name;

            //try
            //{
            //    var result = Prefab.FindFSMAction(typeName);

            //    {
            //        if (result.state != null)
            //        {
            //            try
            //            {
            //                Dev.Log($"state {result.state == null}");
            //                Dev.Log($"fsm {result.state.Fsm == null}");
            //                Dev.Log($"fsm c {result.state.Fsm.FsmComponent == null}");

            //                Dev.Log($"Found GetColliderRange on {result.state} in {result.state.Fsm} in {Prefab.name}");
            //            }
            //            catch (Exception e)
            //            {
            //                Dev.LogError("failed to print found log");
            //            }
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    Dev.LogError("failed search for fsm action " + typeName + " in " + Prefab.name);
            //}
        }

        public virtual bool IsValidDifficultyReplacement(EnemyData dataOfEnemyToReplace)
        {
            //TODO: work out how I want this logic to go
            return true;
        }

        public virtual bool IsValidTypeReplacement(EnemyData dataOfEnemyToReplace)
        {
            if (Data.isBoss != dataOfEnemyToReplace.isBoss)
                return false;

            if (Data.isHard != dataOfEnemyToReplace.isHard)
                return false;

            //TODO: change these to bools
            if (Data.enemyType.Contains("CRAWLER") && dataOfEnemyToReplace.enemyType.Contains("CRAWLER"))
            {
                return true;
            }
            if (Data.enemyType.Contains("GROUND") && dataOfEnemyToReplace.enemyType.Contains("GROUND"))
            {
                return true;
            }
            if (Data.enemyType.Contains("FLYER") && dataOfEnemyToReplace.enemyType.Contains("FLYER"))
            {
                return true;
            }
            if (Data.enemyType.Contains("WALL") && dataOfEnemyToReplace.enemyType.Contains("WALL"))
            {
                return true;
            }

            return false;
        }

        public virtual GameObject Instantiate()
        {
            GameObject gameObject = null;

            try
            {
                gameObject = GameObject.Instantiate(Prefab);
                if (gameObject == null)
                    throw new NullReferenceException("Error: Instantiate somehow returned null without throwing");
            }
            catch(Exception e)
            {
                Dev.LogError($"Error creating instance of {Prefab.name}: {e.Message} >> {e.StackTrace}");
            }

            return gameObject;
        }

        public virtual int CalculateDifficulty()
        {
            var hm = Prefab.GetComponent<HealthManager>();
            if (hm == null)
                return 1;
            return hm.hp * (IsBoss ? 10 : 1);
        }
    }
}
