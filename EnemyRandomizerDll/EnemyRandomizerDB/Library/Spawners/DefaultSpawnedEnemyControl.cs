using UnityEngine;
using System;
using System.Linq;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Satchel;
using Satchel.Futils;
using System.Collections.Generic;
using System.Collections;

namespace EnemyRandomizerMod
{
    public class DefaultSpawnedEnemyControl : SpawnedObjectControl
    {
#if DEBUG
        public DebugColliders debugColliders;
#endif

        protected Geo geoManager { get; set; }
        public Geo GeoManager
        {
            get
            {
                if (geoManager == null)
                    geoManager = new Geo(gameObject);

                return geoManager;
            }
        }

        public override int Geo
        {
            get => GeoManager.Value;
            set => GeoManager.Value = value;
        }

        public int previousHP = -1;
        public int CurrentHP
        {
            get => EnemyHealthManager == null ? 0 : EnemyHealthManager.hp;
            set
            {
                if (EnemyHealthManager != null)
                    EnemyHealthManager.hp = value;
                else
                { }
            }
        }

        public float CurrentHPf
        {
            get => (float)CurrentHP;
            set => CurrentHP = Mathf.FloorToInt(value);
        }

        /// <summary>
        /// set this to change the max hp of an enemy
        /// </summary>
        public int defaultScaledMaxHP = -1;
        public virtual int MaxHP
        {
            get => defaultScaledMaxHP;
            set
            {
                defaultScaledMaxHP = value;
                if (CurrentHP > defaultScaledMaxHP)
                    CurrentHP = defaultScaledMaxHP;
            }
        }

        public float MaxHPf
        {
            get => (float)MaxHP;
            set => MaxHP = Mathf.FloorToInt(value);
        }


        public virtual bool takesSpecialCharmDamage => gameObject.GetComponent<TinkEffect>() != null ? true : false;
        public virtual bool takesSpecialSpellDamage => gameObject.GetComponent<TinkEffect>() != null ? true : false;
        public virtual bool doBlueHealHeroOnDeath => didOriginalDoBlueHealHeroOnDeath;
        public virtual bool didOriginalDoBlueHealHeroOnDeath => originialMetadata != null && originialMetadata.GetDatabaseKey() != null && originialMetadata.GetDatabaseKey().Contains("Health");
        public virtual bool hasSeenPlayer { get; protected set; }
        public virtual bool didShowWhenHeroWasInAggroRange { get; protected set; }
        public virtual float aggroRange => 40f;
        public virtual bool didDisableCameraLocks { get; protected set; }
        protected virtual bool DisableCameraLocks => false;
        protected virtual IEnumerable<CameraLockArea> cameraLocks => DisableCameraLocks ? gameObject.GetCameraLocksFromScene() : null;

#if DEBUG
        public virtual bool showDebugColliders => true;
#endif

        public override void Setup(GameObject objectThatWillBeReplaced = null)
        {
            base.Setup(objectThatWillBeReplaced);

            try
            {
                if (gameObject.ObjectType() == PrefabObject.PrefabType.Enemy)
                {
                    defaultScaledMaxHP = GetStartingMaxHP(objectThatWillBeReplaced);
                    CurrentHP = defaultScaledMaxHP;

                    if (objectThatWillBeReplaced != gameObject && objectThatWillBeReplaced != null)
                    {
                        if (gameObject.CheckIfIsPogoLogicType())
                        {
                            defaultScaledMaxHP = 69;
                            CurrentHP = 69;
                        }
                        else if (gameObject.IsSmasher())
                        {
                            defaultScaledMaxHP = 100;
                            CurrentHP = 100;
                            gameObject.AddParticleEffect_WhiteSoulEmissions(Color.yellow);

                            if (gameObject.IsFlying() && MetaDataTypes.SmasherNeedsCustomSmashBehaviour.Contains(gameObject.GetDatabaseKey()))
                            {
                                AddCustomSmashBehaviour();
                            }
                        }
                    }

                    SetupEnemyGeo();
                }
            }
            catch (Exception e)
            {
                Dev.Log($"{this}:{this.thisMetadata}: Caught exception in ConfigureRelativeToReplacement ERROR:{e.Message} STACKTRACE{e.StackTrace}");
            }
        }

        protected virtual void AddCustomSmashBehaviour()
        {
            var charge = gameObject.GetBigBeeChargeAttack(HeroController.instance.gameObject, null, null);
            gameObject.DistanceFlyChase(HeroController.instance.gameObject, 7f, 0.08f, 5f, 7f, charge, 8f);
        }

        protected virtual int GetStartingMaxHP(GameObject objectThatWillBeReplaced)
        {
            if(objectThatWillBeReplaced == null)
            {
                return gameObject.OriginalPrefabHP();
            }

            if (objectThatWillBeReplaced != null && objectThatWillBeReplaced.IsBoss())
            {
                return ScaleHPToBoss(gameObject.OriginalPrefabHP(), objectThatWillBeReplaced.OriginalPrefabHP());
            }
            else
            {
                return ScaleHPToNormal(gameObject.OriginalPrefabHP(), objectThatWillBeReplaced.OriginalPrefabHP());
            }
        }

        protected override void DoValidSceneDestroyEvents()
        {
            base.DoValidSceneDestroyEvents();
            try
            {
                if (doBlueHealHeroOnDeath)
                    gameObject.DoBlueHealHero();

                //if (gameObject.GetAvailableItem() != null)
                //    SpawnAndFlingItem();
            }
            catch (Exception e) { Dev.LogError($"Exception caught on OnDestroy callback for {this}:{thisMetadata} ERROR:{e.Message}  STACKTRACE: {e.StackTrace}"); }
        }

        protected virtual bool HeroInAggroRange()
        {
            if (hasSeenPlayer)
            {
                return gameObject.IsNearPlayer(aggroRange);
            }
            else
            {
                if (gameObject.CanSeePlayer())
                {
                    hasSeenPlayer = true;
                    return true;
                }

                return false;
            }
        }

        protected virtual void Update()
        {
            if (!loaded)
                return;

            if (gameObject.ObjectType() != PrefabObject.PrefabType.Enemy)
                return;

#if DEBUG
            if (showDebugColliders && debugColliders == null)
            {
                debugColliders = gameObject.GetOrAddComponent<DebugColliders>();
            }
            else if(showDebugColliders && !debugColliders.enabled)
            {
                debugColliders.enabled = true;
            }
            else if (!showDebugColliders && debugColliders.enabled)
            {
                debugColliders.enabled = false;
            }
#endif

            try
            {
                CheckDisableCameras();
            }
            catch (Exception e)
            {
                Dev.Log($"{this}:{this.thisMetadata}: Caught exception in CheckDisableCameras ERROR:{e.Message} STACKTRACE{e.StackTrace}");
            }

            try
            {
                CheckUpdateHeroInAggroRange();
            }
            catch (Exception e)
            {
                Dev.Log($"{this}:{this.thisMetadata}: Caught exception in CheckUpdateHeroInAggroRange ERROR:{e.Message} STACKTRACE{e.StackTrace}");
            }

            try
            {
                CheckControlInCustomHiddenState();
            }
            catch (Exception e)
            {
                Dev.Log($"{this}:{this.thisMetadata}: Caught exception in CheckControlInCustomHiddenState ERROR:{e.Message} STACKTRACE{e.StackTrace}");
            }
        }

        protected virtual void CheckDisableCameras()
        {
            if (!didDisableCameraLocks && DisableCameraLocks)
            {
                var cams = cameraLocks;
                if (cams.Count() > 0)
                {
                    cameraLocks.UnlockCameras();
                    didDisableCameraLocks = true;
                }
            }
        }


        protected virtual void CheckUpdateHeroInAggroRange()
        {
            if (!didShowWhenHeroWasInAggroRange)
            {
                if (HeroInAggroRange())
                {
                    didShowWhenHeroWasInAggroRange = true;
                    OnHeroInAggroRangeTheFirstTime();
                }
            }
        }

        protected virtual void CheckControlInCustomHiddenState()
        {
            if (control == null)
                return;

            if (control.ActiveStateName == FSMHiddenStateName)
            {
                control.SendEvent("SHOW");
            }
        }

        protected virtual int ScaleHPToNormal(int defaultNewEnemyHP, int originalEnemyHP)
        {
            if (originalEnemyHP * 2 < defaultNewEnemyHP)
                return Mathf.FloorToInt(originalEnemyHP * 2f);
            else if (originalEnemyHP < defaultNewEnemyHP)
                return originalEnemyHP;
            else
                return defaultNewEnemyHP;
        }

        protected virtual int ScaleHPToBoss(int defaultNewEnemyHP, int originalEnemyHP)
        {
            int minBarBossHP = 225;
            if(originalEnemyHP < minBarBossHP)
            {
                int newBossHP = originalEnemyHP * 2;
                minBarBossHP = minBarBossHP + 100;
                for (int i = 0; i < 16; ++i)
                {
                    if (newBossHP > minBarBossHP)
                    {
                        return newBossHP;
                    }
                    newBossHP = originalEnemyHP * 2;
                    minBarBossHP = minBarBossHP + 100;
                }

                return defaultNewEnemyHP;
            }
            else
            {
                if (defaultNewEnemyHP * 2 < defaultNewEnemyHP)
                    return Mathf.FloorToInt(originalEnemyHP * 2f);
                return originalEnemyHP;
            }
        }

        protected virtual void SetupEnemyGeo()
        {
            int originalGeo = SpawnerExtensions.GetOriginalGeo(thisMetadata.ObjectName);
            if (originialMetadata != null && thisMetadata != originialMetadata)
            {
                originalGeo = SpawnerExtensions.GetOriginalGeo(originialMetadata.ObjectName);
            }

            var zone = GameManager.instance.GetCurrentMapZone();
            int geoScale = 1;
            if (MetaDataTypes.GeoZoneScale.TryGetValue(zone, out geoScale))
            {
                if (originalGeo <= 0)
                {
                    Geo = SpawnerExtensions.GetRandomValueBetween(geoScale, geoScale * 5);
                }
                else
                {
                    Geo = SpawnerExtensions.GetRandomValueBetween(originalGeo, originalGeo * geoScale);
                }
            }
            else
            {
                Geo = SpawnerExtensions.GetRandomValueBetween(1 + originalGeo, originalGeo * 2);
            }
        }

        public override void RecieveExtraDamage(ExtraDamageTypes extraDamageType)
        {
            base.RecieveExtraDamage(extraDamageType);
            if (takesSpecialCharmDamage && EnemyHealthManager != null)
            {
                int dmgAmount = ExtraDamageable.GetDamageOfType(extraDamageType);
                EnemyHealthManager.ApplyExtraDamage(dmgAmount);
            }
        }

        public override void Hit(HitInstance damageInstance)
        {
            base.Hit(damageInstance);
            if (takesSpecialSpellDamage && EnemyHealthManager != null && damageInstance.AttackType == AttackTypes.Spell)
            {
                int dmgAmount = damageInstance.DamageDealt;
                EnemyHealthManager.ApplyExtraDamage(dmgAmount);
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