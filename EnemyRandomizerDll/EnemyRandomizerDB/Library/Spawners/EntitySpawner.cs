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

        public virtual GameObject SpawnAndTrackChild(string objectName, Vector3 spawnPoint, bool setActive = true, bool allowRandomization = false)
        {
            GameObject spawnedObject = null;
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

            GameObject child = null;
            if(allowRandomization)
            {
                child = EnemyRandomizerDatabase.GetDatabase().Spawn(objectName);
                child.transform.position = spawnPoint;
            }
            else
            {
                child = SpawnerExtensions.SpawnEntityAt(objectName, spawnPoint, setActive);
            }
            Children.Add(child);
            if (setActive)
            {
                child.SafeSetActive(true);
            }
            handle.Dispose();
            return spawnedObject;
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