using UnityEngine;

namespace EnemyRandomizerMod
{
    public class InGroundEnemyControl : DefaultSpawnedEnemyControl
    {
        public override bool preventInsideWallsAfterPositioning => false;
        public override bool preventOutOfBoundsAfterPositioning => false;

        public override float spawnPositionOffset => 0.3f;

        public override Vector2 pos2dWithOffset => pos2d + Vector2.up * 2f;

        public RaycastHit2D floorSpawn => SpawnerExtensions.GetRayOn(pos2dWithOffset, Vector2.down, 200f);
        public RaycastHit2D floorLeft => SpawnerExtensions.GetRayOn(floorSpawn.point - Vector2.one * 0.2f, Vector2.left, 50f);
        public RaycastHit2D floorRight => SpawnerExtensions.GetRayOn(floorSpawn.point - Vector2.one * 0.2f, Vector2.right, 50f);
        public RaycastHit2D wallLeft => SpawnerExtensions.GetRayOn(pos2dWithOffset, Vector2.left, 50f);
        public RaycastHit2D wallRight => SpawnerExtensions.GetRayOn(pos2dWithOffset, Vector2.right, 50f);
        public float floorsize => floorRight.distance + floorLeft.distance;
        public float floorCenter => floorLeft.point.x + floorsize * .5f;

        public Vector3 emergePoint;
        public Vector3 emergeVelocity;

        protected override bool HeroInAggroRange()
        {
            if (heroPos2d.y < floorSpawn.point.y)
                return false;

            if (heroPos2d.y > floorSpawn.point.y + 5f)
                return false;

            if (heroPos2d.x < floorLeft.point.x)
                return false;

            if (heroPos2d.x > floorRight.point.x)
                return false;

            hasSeenPlayer = true;
            return true;
        }

        public override void Setup(GameObject objectThatWillBeReplaced = null)
        {
            base.Setup(objectThatWillBeReplaced);

            var fsm = gameObject.LocateMyFSM("FSM");
            if (fsm != null)
                Destroy(fsm);

            FitToFloor();
        }

        protected virtual void FitToFloor()
        {
            if (floorsize < (gameObject.GetOriginalObjectSize().x * SizeScale))
            {
                float ratio = (floorsize) / (gameObject.GetOriginalObjectSize().x * SizeScale);
                gameObject.ScaleObject(ratio * .5f);
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