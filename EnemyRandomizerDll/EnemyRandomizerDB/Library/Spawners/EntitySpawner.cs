using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UniRx;

namespace EnemyRandomizerMod
{
    public class EntitySpawner : MonoBehaviour
    {
        public int maxChildren = 5;
        public bool removeChildrenOnDeath = true;
        public string spawnEntityOnChildDeath;

        protected List<GameObject> children = new List<GameObject>();
        public List<GameObject> Children => children;

        public IEnumerable<T> GetChildrenControllers<T>()
            where T : SpawnedObjectControl
        {
            return Children.Where(x => x != null).Select(x => x.GetComponent<T>());
        }

        public int RemainingSpawns => maxChildren - children.Count;

        public bool AtMaxChildren => Children.Count >= maxChildren;
        public virtual GameObject ActivateAndTrackSpawnedObject(GameObject objectThatWillBeReplaced)
        {
            GameObject spawnedObject = objectThatWillBeReplaced;
            var handle = EnemyRandomizerDatabase.OnObjectReplaced.AsObservable().Subscribe(x =>
            {
                if (Children == null)
                    return;

                if (Children.Contains(x.oldObject))
                    Children.Remove(x.oldObject);
                if (!Children.Contains(x.newObject))
                    Children.Add(x.newObject);

                spawnedObject = x.newObject;
            });

            var child = objectThatWillBeReplaced;
            Children.Add(child);
            child.SafeSetActive(true);

            handle.Dispose();

            return spawnedObject;
        }

        public void UntrackObject(GameObject child)
        {
            if (Children == null)
                return;

            if (Children.Contains(child))
                Children.Remove(child);
        }

        public void TrackObject(GameObject child)
        {
            if (Children == null)
                return;

            if (!Children.Contains(child))
                Children.Add(child);
        }

        public virtual GameObject SpawnAndTrackChild(string objectName, Vector3 spawnPoint, bool setActive = true, bool allowRandomization = false)
        {
            GameObject child = SpawnerExtensions.SpawnEntityAt(objectName, spawnPoint, setActive, allowRandomization);

            Children.Add(child);

            return child;
        }

        protected virtual void OnDestroy()
        {
            try
            {
                if (removeChildrenOnDeath)
                    RemoveChildren();
            }
            catch (Exception e) { Dev.LogError($"Exception caught on RemoveChildren for {gameObject.GetSceneHierarchyPath()} ERROR:{e.Message}  STACKTRACE: {e.Message}"); }
        }

        protected virtual void Update()
        {
            try
            {
                RemoveDeadChildren();
            }
            catch (Exception e)
            {
                Dev.Log($"{this}:{this}: Caught exception in RemoveDeadChildren for {gameObject.GetSceneHierarchyPath()} ERROR:{e.Message} STACKTRACE{e.StackTrace}");
            }
        }

        public virtual void RemoveChildren()
        {
            if (Children != null)
            {
                Children.ForEach(x =>
                {
                    if (x == null)
                        return;

                    if (x.IsInAValidScene())
                    {
                        if(!string.IsNullOrEmpty(spawnEntityOnChildDeath))
                        {
                            var boom = EnemyRandomizerDatabase.CustomSpawnWithLogic(x.transform.position, spawnEntityOnChildDeath, null, false);
                            boom.ScaleObject(0.5f);
                            boom.SetActive(true);
                        }

                        var hm = x.GetComponent<HealthManager>();
                        if(hm != null)
                            x.KillObjectNow();
                        else
                            GameObject.Destroy(x);
                    }
                    else
                    {
                        GameObject.Destroy(x);
                    }
                });

                Children.Clear();
            }
        }

        public virtual void RemoveDeadChildren()
        {
            if (Children == null)
                return;

            for (int i = 0; i < Children.Count;)
            {
                if (i >= Children.Count)
                    break;

                if (Children[i] == null)
                {
                    Children.RemoveAt(i);
                    continue;
                }
                else if(IsOutsideColo(Children[i]))
                {
                    Children[i].KillObjectNow();
                    Children.RemoveAt(i);
                    continue;
                }
                else
                {
                    var hm = Children[i].GetComponent<HealthManager>();
                    if (hm == null || hm.hp <= 0 || hm.isDead)
                    {
                        Children.RemoveAt(i);
                        continue;
                    }
                }

                ++i;
            }
        }

        protected virtual bool IsOutsideColo(GameObject g)
        {
            if (GameManager.instance.GetCurrentMapZone() != "COLOSSEUM")
                return false;

            bool isOutside = false;
            var pos = g.transform.position.ToVec2();
            if(pos.y < 3.5f || pos.y > 30f)
            {
                isOutside = true;
            }
            if (pos.x < 83f || pos.x > 122f)
            {
                isOutside = true;
            }

            bool isColoBronze = BattleManager.StateMachine.Value is ColoBronze;
            if (isColoBronze)
            {
                var bronze = BattleManager.StateMachine.Value as ColoBronze;
                if(bronze.StateIndex >= 32 && bronze.StateIndex < 42)
                {
                    if(pos.y > 15.5f)
                    {
                        isOutside = true;
                    }
                }
            }


            bool isColoSilver = BattleManager.StateMachine.Value is ColoSilver;
            if (isColoSilver)
            {
                var silver = BattleManager.StateMachine.Value as ColoSilver;
                if (silver.StateIndex >= 60 && silver.StateIndex < 63)
                {
                    if (pos.y > 15.1f)
                    {
                        isOutside = true;
                    }
                }
                if (silver.StateIndex >= 64)
                {
                    if (pos.y > 18.1f)
                    {
                        isOutside = true;
                    }
                }
            }


            return isOutside;
        }

        /// <summary>
        /// Warning: only use with custom arenas! This will not randomize any enemies that it spawns
        /// and activate them immediately
        /// </summary>
        public virtual GameObject SpawnCustomArenaEnemy(Vector2 pos, string specificEnemy = null, string originalEnemy = null, RNG rng = null)
        {
            GameObject enemy = null;
            string enemyToSpawn = null;
            try
            {
                if(!string.IsNullOrEmpty(specificEnemy))
                {
                    enemyToSpawn = specificEnemy;
                    enemy = SpawnerExtensions.SpawnEntityAt(specificEnemy, pos, false, false);
                }
                else
                {
                    enemyToSpawn = SpawnerExtensions.GetRandomPrefabNameForArenaEnemy(rng, originalEnemy);
                    enemy = SpawnAndTrackChild(enemyToSpawn, pos, false, false);
                }

                if (enemy != null)
                {
                    var soc = enemy.GetComponent<SpawnedObjectControl>();
                    if (soc != null)
                    {
                        soc.placeGroundSpawnOnGround = false;
                    }
                }
            }
            catch (Exception e) { Dev.LogError($"Exception caught in SpawnCustomArenaEnemy when trying to spawn {enemyToSpawn} ERROR:{e.Message}  STACKTRACE: {e.StackTrace}"); }

            try
            {
                if (enemy != null && !string.IsNullOrEmpty(originalEnemy) && !string.IsNullOrEmpty(enemyToSpawn))
                {
                    float sizeScale = SpawnerExtensions.GetRelativeScale(enemyToSpawn, originalEnemy);
                    if (!Mathnv.FastApproximately(sizeScale, 1f, 0.01f))
                    {
                        enemy.ScaleObject(sizeScale);
                        enemy.ScaleAudio(sizeScale);//might not need this....
                    }
                }
            }
            catch (Exception e) { Dev.LogError($"Exception caught in SpawnCustomArenaEnemy when trying to scale {enemyToSpawn} to match {originalEnemy} ERROR:{e.Message}  STACKTRACE: {e.StackTrace}"); }

            enemy = ActivateAndTrackSpawnedObject(enemy);

            if (enemy != null && !string.IsNullOrEmpty(originalEnemy))
            {
                var soc2 = enemy.GetComponent<DefaultSpawnedEnemyControl>();
                if (soc2 != null)
                {
                    float orginalHP = SpawnerExtensions.GetObjectPrefab(originalEnemy).prefab.GetEnemyHealthManager().hp;

                    bool isColoSilver = BattleManager.StateMachine.Value is ColoSilver;
                    if (isColoSilver)
                    {
                        orginalHP = orginalHP * 1.5f;

                        soc2.defaultScaledMaxHP = Mathf.FloorToInt(orginalHP);
                        soc2.CurrentHP = soc2.defaultScaledMaxHP;
                    }
                    else
                    {
                        bool isColoGold = BattleManager.StateMachine.Value is ColoGold;
                        if (isColoGold)
                        {
                            //only adjust their HP in gold if they spawned with like 1hp for some reason
                            if(soc2.defaultScaledMaxHP < 10f)
                            {
                                soc2.defaultScaledMaxHP = Mathf.FloorToInt(orginalHP);
                                soc2.CurrentHP = soc2.defaultScaledMaxHP;
                            }
                        }
                        else
                        {
                            soc2.defaultScaledMaxHP = Mathf.FloorToInt(orginalHP);
                            soc2.CurrentHP = soc2.defaultScaledMaxHP;
                        }
                    }
                }
            }

            return enemy;
        }
    }
}





























//protected virtual void AddResetToStateOnHide(PlayMakerFSM fsm, string resetToState)
//{
//    if (FSMsWithResetToStateOnHide == null)
//        FSMsWithResetToStateOnHide = new Dictionary<PlayMakerFSM, string>();

//    FSMsWithResetToStateOnHide.Add(fsm, resetToState);
//}

//protected virtual void SpawnAndFlingItem()
//{
//    Dev.Where();
//    //if (thisMetadata != null && !thisMetadata.IsValidScene)
//    //    return;

//    //if (thisMetadata.AvailableItem != null)
//    //{
//    //    FlingUtils.SelfConfig fling = new FlingUtils.SelfConfig()
//    //    {
//    //        Object = thisMetadata.AvailableItem.Spawn(transform.position),
//    //        SpeedMin = 5f,
//    //        SpeedMax = 10f,
//    //        AngleMin = 0f,
//    //        AngleMax = 180f
//    //    };
//    //    FlingUtils.FlingObject(fling, null, Vector3.zero);
//    //}
//}



//protected virtual void ForceUpdateJournal()
//{
//    var pdName = thisMetadata.PlayerDataName;
//    RecordCustomJournalOnDeath(pdName);
//}

//protected virtual void RecordCustomJournalOnDeath(string pdName)
//{
//    PlayerData playerData = GameManager.instance.playerData;
//    string text = "killed" + pdName;
//    string text2 = "kills" + pdName;
//    string text3 = "newData" + pdName;
//    bool flag = false;
//    if (!playerData.GetBool(text))
//    {
//        flag = true;
//        playerData.SetBool(text, true);
//        playerData.SetBool(text3, true);
//    }
//    bool flag2 = false;
//    int num = playerData.GetInt(text2);
//    if (num > 0)
//    {
//        num--;
//        playerData.SetInt(text2, num);
//        if (num <= 0)
//        {
//            flag2 = true;
//        }
//    }
//    if (playerData.hasJournal)
//    {
//        bool flag3 = false;
//        if (flag2)
//        {
//            flag3 = true;
//            playerData.journalEntriesCompleted++;
//        }
//        else if (flag)
//        {
//            flag3 = true;
//            playerData.journalNotesCompleted++;
//        }
//        if (flag3)
//        {
//            //in lieu of the proper journal unlock effect, just do something
//            EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Item Get Effect R", null, true);
//        }
//    }
//}









//var handle = EnemyRandomizerDatabase.OnObjectReplaced.AsObservable().Subscribe(x =>
//{
//    if (Children == null)
//        return;

//    if (Children.Contains(x.oldObject))
//        Children.Remove(x.oldObject);
//    if (!Children.Contains(x.newObject))
//        Children.Add(x.newObject);

//    spawnedObject = x.newObject;
//});


//handle.Dispose();