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

        public Vector2? topLeft;
        public Vector2? botRight;

        protected HashSet<GameObject> children = new HashSet<GameObject>();
        public HashSet<GameObject> Children => children;

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

        public virtual GameObject SpawnAndTrackChild(string objectName, Vector3 spawnPoint, string originalEnemy = null, bool setActive = true, bool allowRandomization = false)
        {
            GameObject child = SpawnerExtensions.SpawnEntityAt(objectName, spawnPoint, originalEnemy, setActive, allowRandomization);

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
                foreach(var x in Children)
                {
                    if (x == null)
                        continue;

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
                }

                Children.Clear();
            }
        }

        IEnumerable<GameObject> GetChildrenOutsideColo()
        {
            try
            {
                var outside = children.Where(x => x != null).Where(x => IsOutsideColo(x));
            }
            catch (Exception e) { Dev.LogError($"Exception caught in GetChildrenOutsideColo when trying to get the children outside colo ERROR:{e.Message}  STACKTRACE: {e.StackTrace}"); }

            return null;
        }


        IEnumerable<GameObject> GetChildrenOutsideArena()
        {
            try
            {
                var outside = children.Where(x => x != null).Where(x => IsOutsideArena(x));
            }
            catch (Exception e) { Dev.LogError($"Exception caught in GetChildrenOutsideArena when trying to get the children outside colo ERROR:{e.Message}  STACKTRACE: {e.StackTrace}"); }

            return null;
        }

        public virtual void RemoveDeadChildren()
        {
            if (children == null)
                return;

            var outside = GetChildrenOutsideColo();
            var outsideArena = GetChildrenOutsideArena();

            try
            {
                var newKids = new HashSet<GameObject>();
                foreach(var c in children.Where(x => !IsOutsideColo(x)).Where(x => IsAlive(x)))
                {
                    newKids.Add(c);
                }
                children = newKids;
            }
            catch (Exception e) { Dev.LogError($"Exception caught in RemoveDeadChildren when trying to cull outside or dead game objects ERROR:{e.Message}  STACKTRACE: {e.StackTrace}"); }

            if (outside != null)
            {
                DestroyAllChildren(outside);
            }

            if(outsideArena != null)
            {
                DestroyAllChildren(outsideArena);
            }
        }

        protected virtual void DestroyAllChildren(IEnumerable<GameObject> childrenToDestroy)
        {
            foreach (var c in Children.Where(x => x != null))
            {
                try
                {
                    c.KillObjectNow();
                }
                catch (Exception e) { Dev.LogError($"Exception caught in DestroyAllChildren when trying to kill {c} ERROR:{e.Message}  STACKTRACE: {e.StackTrace}"); }
            }
        }

        protected virtual bool IsAlive(GameObject x)
        {
            if (x == null)
                return false;

            var hm = x.GetComponent<HealthManager>();

            return 
            (
               hm != null &&
               hm.hp > 0 &&
               hm.isDead == false
            )
            ||
            (
               hm == null &&
               x.activeInHierarchy != false
            );
        }

        protected virtual bool IsOutsideArena(GameObject g)
        {
            if (g == null)
                return true;

            if (topLeft == null)
                return false;

            if (botRight == null)
                return false;

            return !InBox(g, topLeft.Value, botRight.Value);
        }

        public bool InBox(GameObject g, Vector2 topleft, Vector2 bottomRight)
        {
            var point = g.transform.position.ToVec2();

            // Check if the point's X coordinate is within the box's X range
            bool withinXRange = point.x >= topleft.x && point.x <= bottomRight.x;

            // Check if the point's Y coordinate is within the box's Y range
            bool withinYRange = point.y <= topleft.y && point.y >= bottomRight.y;

            // Return true if the point is within both the X and Y ranges, indicating it is inside the box
            return withinXRange && withinYRange;
        }

        protected virtual bool IsOutsideColo(GameObject g)
        {
            if (g == null)
                return true;

            if (GameManager.instance.GetCurrentMapZone() != "COLOSSEUM")
                return false;

            if (!g.activeInHierarchy)
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

            if (!isOutside)
            {
                bool isColoBronze = BattleManager.StateMachine.Value is ColoBronze;
                if (isColoBronze)
                {
                    var bronze = BattleManager.StateMachine.Value as ColoBronze;
                    if (bronze.StateIndex >= 32 && bronze.StateIndex < 42)
                    {
                        if (pos.y > 15.5f)
                        {
                            isOutside = true;
                        }
                    }
                }
            }

            if (!isOutside)
            {
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
                    enemy = SpawnerExtensions.SpawnEntityAt(specificEnemy, pos, originalEnemy, false, false);
                }
                else
                {
                    enemyToSpawn = SpawnerExtensions.GetRandomPrefabNameForArenaEnemy(rng, originalEnemy);
                    enemy = SpawnAndTrackChild(enemyToSpawn, pos, originalEnemy, false, false);
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

            try
            {
                enemy = ActivateAndTrackSpawnedObject(enemy);

                if (enemy != null && !string.IsNullOrEmpty(originalEnemy))
                {
                    var soc2 = enemy.GetComponent<DefaultSpawnedEnemyControl>();
                    if (soc2 != null)
                    {
                        float orginalHP = 0f;
                        try
                        {
                            orginalHP = SpawnerExtensions.GetObjectPrefab(originalEnemy).prefab.GetEnemyHealthManager().hp;
                            Dev.Log($"Got original HP {orginalHP} from {originalEnemy}");
                        }
                        catch (Exception e) { Dev.LogError($"Exception caught in SpawnCustomArenaEnemy when trying to get orginalHP from GetObjectPrefab from {originalEnemy} to apply to {soc2.gameObject}  ERROR:{e.Message}  STACKTRACE: {e.StackTrace}"); }

                        try
                        {
                            if (orginalHP <= 1f)
                            {
                                orginalHP = SpawnerExtensions.GetObjectPrefab(enemy.name).prefab.GetEnemyHealthManager().hp;
                                Dev.Log($"Got HP {orginalHP} from {enemy} because the original was too low");
                            }
                        }
                        catch (Exception e) { Dev.LogError($"Exception caught in SpawnCustomArenaEnemy when trying to get orginalHP from GetObjectPrefab from {originalEnemy} to apply to {soc2.gameObject}  ERROR:{e.Message}  STACKTRACE: {e.StackTrace}"); }

                        if(orginalHP <= 1f)
                        {
                            orginalHP = 100f;
                            Dev.Log($"Setting HP to {orginalHP} because it's still zero or negative");
                        }

                        try
                        {
                            bool isColoSilver = BattleManager.StateMachine.Value is ColoSilver;
                            if (isColoSilver)
                            {
                                orginalHP = orginalHP * 1.5f;
                                Dev.Log($"Scaling HP to {orginalHP} because it's silver colo");

                                soc2.defaultScaledMaxHP = Mathf.FloorToInt(orginalHP);
                                soc2.CurrentHP = soc2.defaultScaledMaxHP;
                            }
                            else
                            {
                                bool isColoGold = BattleManager.StateMachine.Value is ColoGold;
                                if (isColoGold)
                                {
                                    //only adjust their HP in gold if they spawned with like 1hp for some reason
                                    if (soc2.defaultScaledMaxHP < 10f)
                                    {
                                        soc2.defaultScaledMaxHP = Mathf.FloorToInt(orginalHP);
                                        soc2.CurrentHP = soc2.defaultScaledMaxHP;
                                        Dev.Log($"Scaling HP to {soc2.CurrentHP} because it's gold colo");
                                    }
                                    else
                                    {
                                        Dev.Log($"Leaving their HP at {orginalHP} because it's gold colo and their hp is too low");
                                    }
                                }
                                else
                                {
                                    soc2.defaultScaledMaxHP = Mathf.FloorToInt(orginalHP);
                                    soc2.CurrentHP = soc2.defaultScaledMaxHP;
                                    Dev.Log($"Scaling HP to {orginalHP} because it's bronze colo");
                                }
                            }
                        }
                        catch (Exception e) { Dev.LogError($"Exception caught in SpawnCustomArenaEnemy when trying to apply hp to {soc2.gameObject}  ERROR:{e.Message}  STACKTRACE: {e.StackTrace}"); }

                    }
                }
            }
            catch (Exception e) { Dev.LogError($"Exception caught in SpawnCustomArenaEnemy in last part of arena enemy modification  ERROR:{e.Message}  STACKTRACE: {e.StackTrace}"); }

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