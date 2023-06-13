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
                {
                    Dev.Log(gameObject + " HP WAS " + EnemyHealthManager.hp);
                    Dev.Log(Dev.FunctionHeader(0));
                    Dev.Log(Dev.FunctionHeader(1));
                    Dev.Log(Dev.FunctionHeader(2));
                    Dev.Log(Dev.FunctionHeader(3));
                    EnemyHealthManager.hp = value;
                    Dev.Log(gameObject + " HP IS " + EnemyHealthManager.hp);
                    lastSetHP = CurrentHP;
                }
                else
                { }
            }
        }

        public int counter = 5;
        public bool didSetHP = false;
        public int lastSetHP = -1;

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
            
            if (gameObject.ObjectType() != PrefabObject.PrefabType.Enemy)
                return;

            //just no -- don't let the game set the hp ever
            var hpscaler = gameObject.LocateMyFSM("hp_scaler");
            if (hpscaler != null)
                Destroy(hpscaler);

            try
            {
                SetupEnemyHP(objectThatWillBeReplaced);
            }
            catch (Exception e)
            {
                Dev.Log($"{this}:{this.thisMetadata}: Caught exception in SetupEnemyHP ERROR:{e.Message} STACKTRACE{e.StackTrace}");
            }

            try
            {
                SetupEnemyGeo();
            }
            catch (Exception e)
            {
                Dev.Log($"{this}:{this.thisMetadata}: Caught exception in SetupEnemyGeo ERROR:{e.Message} STACKTRACE{e.StackTrace}");
            }

            try
            {
                SetupForSpecialGameLogic(objectThatWillBeReplaced);
            }
            catch (Exception e)
            {
                Dev.Log($"{this}:{this.thisMetadata}: Caught exception in SetupForSpecialGameLogic ERROR:{e.Message} STACKTRACE{e.StackTrace}");
            }

        }

        protected virtual void SetupForSpecialGameLogic(GameObject objectThatWillBeReplaced)
        {
            if (objectThatWillBeReplaced != gameObject && objectThatWillBeReplaced != null)
            {
                if (objectThatWillBeReplaced.CheckIfIsPogoLogicType())
                {
                    defaultScaledMaxHP = 69;
                    CurrentHP = 69;
                }
                else if (objectThatWillBeReplaced.IsSmasher())
                {
                    defaultScaledMaxHP = 100;
                    CurrentHP = 100;
                    gameObject.AddParticleEffect_WhiteSoulEmissions(Color.yellow);

                    //TODO: needs testing
                    if (gameObject.IsFlying() && MetaDataTypes.SmasherNeedsCustomSmashBehaviour.Contains(gameObject.GetDatabaseKey()))
                    {
                        AddCustomSmashBehaviour();
                    }
                }
                else
                {
                    if (EnemyHealthManager != null)
                    {
                        var ignoreAcid = EnemyHealthManager.GetType().GetField("ignoreAcid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        ignoreAcid.SetValue(EnemyHealthManager, false);
                    }
                }
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

        protected virtual int GetStartingMaxHP(string prefabName)
        {
            if (string.IsNullOrEmpty(prefabName))
            {
                return gameObject.OriginalPrefabHP();
            }

            if (!string.IsNullOrEmpty(prefabName) && SpawnerExtensions.IsBoss(prefabName))
            {
                return ScaleHPToBoss(gameObject.OriginalPrefabHP(), SpawnerExtensions.OriginalPrefabHP(prefabName));
            }
            else
            {
                return ScaleHPToNormal(gameObject.OriginalPrefabHP(), SpawnerExtensions.OriginalPrefabHP(prefabName));
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

            CheckHackySceneFixes();

            if (counter > 0)
            {
                counter--;
                if (counter <= 0 && lastSetHP > 0)
                    MaxHP = lastSetHP;
            }

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
                if (cams != null && cams.Count() > 0)
                {
                    cams.UnlockCameras();
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
            float origClamped = Mathf.Max(originalEnemyHP, 1f);
            float ratio = (float)defaultNewEnemyHP / (float)origClamped;
            string zone = GameManager.instance.GetCurrentMapZone();
            int zoneScale = MetaDataTypes.ProgressionZoneScale[zone];
            bool infectionIsActive = GameManager.instance.playerData.crossroadsInfected;

            if (ratio > 2f)
            {
                //example provided below using PV's HP of 1600 and an enemy that might have 1 hp (worst case scenario)
                //ratio at 1600, goes to about 8.5
                //ratio at 800,  goes to about 8
                //ratio at 400,  goes to about 6 or 7
                //ratio at 160,  goes to about 6
                //ratio at 80,  goes to about 5.9
                //ratio at 20,  goes to about 4.5
                //ratio at 10,  goes to about 3.75
                //ratio at 4,  goes to about 2.9
                //ratio at 2 goes to about 2
                float logRatio = Mathf.Log(ratio) + 1.45f;

                //for the early areas, if infection is not active, scale it down more
                if (!infectionIsActive)
                {
                    if (zoneScale > 0 && zoneScale <= 4)
                    {
                        logRatio = logRatio * 0.25f;
                    }
                    else if (zoneScale > 4 && zoneScale <= 10)
                    {
                        logRatio = logRatio * 0.5f;
                    }
                    else if (zoneScale > 10 && zoneScale <= 20)
                    {
                        logRatio = logRatio * 0.7f;
                    }
                }

                //but keep it from going lower than 2
                if (logRatio < 2f)
                    logRatio = 2f;

                //then lets floor the value into a multiplier, so that 8.5 above goes to 8, for example
                int multiplier = Mathf.FloorToInt(logRatio);

                //and send it back
                return originalEnemyHP * multiplier;

            }
            else if(ratio < 0.5f)
            {
                return originalEnemyHP;
            }
            else
            {
                return defaultNewEnemyHP;
            }
        }

        protected virtual int ScaleHPToBoss(int defaultNewEnemyHP, int originalEnemyHP)
        {
            string zone = GameManager.instance.GetCurrentMapZone();
            int zoneScale = MetaDataTypes.ProgressionZoneScale[zone];

            //don't scale the hp in godhome
            if (zoneScale < 0)
                return originalEnemyHP;

            //use their own hp in town
            if (zoneScale == 0)
                return defaultNewEnemyHP;

            //in dream world (dream boss, so let's use that hp)
            if(zoneScale == MetaDataTypes.ProgressionZoneScale["DREAM_WORLD"] || 
               zoneScale == MetaDataTypes.ProgressionZoneScale["FINAL_BOSS"] )
            {
                return originalEnemyHP;
            }
            else
            {
                //just try using the original hp for now, just to see how it feels
                return originalEnemyHP;
            }
        }

        protected virtual void SetupEnemyHP(GameObject objectThatWillBeReplaced)
        {
            bool isColo = IsInColo();

            //don't setup HP if we're in colo, let the event do that 
            if (isColo)
                return;

            if(SpawnedObjectControl.VERBOSE_DEBUG)
                Dev.Log($"{this} hp was {CurrentHP}");

            if (objectThatWillBeReplaced == null && originialMetadata != null)
            {
                defaultScaledMaxHP = GetStartingMaxHP(originialMetadata.GetDatabaseKey());
                CurrentHP = defaultScaledMaxHP;
            }
            else if(objectThatWillBeReplaced != null)
            {
                defaultScaledMaxHP = GetStartingMaxHP(objectThatWillBeReplaced);
                CurrentHP = defaultScaledMaxHP;
            }
            else
            {
                //use default hp
            }

            if (SpawnedObjectControl.VERBOSE_DEBUG)
                Dev.Log($"Setting {this} hp to {CurrentHP}");
        }

        protected virtual void SetupEnemyGeo()
        {
            bool isBoss = false;
            int originalGeo = SpawnerExtensions.GetOriginalGeo(thisMetadata.ObjectName);
            if (originialMetadata != null && thisMetadata != originialMetadata)
            {
                originalGeo = SpawnerExtensions.GetOriginalGeo(originialMetadata.ObjectName);
                isBoss = SpawnerExtensions.IsBoss(originialMetadata.GetDatabaseKey());
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
                    if(isBoss)
                    {
                        Geo = SpawnerExtensions.GetRandomValueBetween(geoScale * 50, geoScale * 100);
                    }
                    else
                    {
                        Geo = SpawnerExtensions.GetRandomValueBetween(1, originalGeo * geoScale);
                    }
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

        protected virtual void CheckHackySceneFixes()
        {
            string currentScene = SpawnerExtensions.SceneName(gameObject);

            var poob = gameObject.GetComponent<PreventOutOfBounds>();
            bool needUnstuck = false;
            Vector2 fixedPos = Vector2.zero;

            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (currentScene == "Crossroads_01")
            {
                Vector2 putHere = new Vector2(52.5f, 18f);

                if(transform.position.y > 20)
                {
                    needUnstuck = true;
                }

                if(transform.position.x < 37f)
                {
                    if (transform.position.x > 35f)
                    {
                        if (transform.position.y < 4.7f)
                        {
                            needUnstuck = true;
                        }
                    }
                    else if (transform.position.x > 17f)
                    {
                        if (transform.position.y < 8.5f)
                        {
                            needUnstuck = true;
                        }
                    }

                    if (transform.position.y < 2.5f)
                    {
                        needUnstuck = true;
                    }

                }
                else
                {
                    if (transform.position.y < 0f)
                    {
                        needUnstuck = true;
                    }


                    if (transform.position.x > 48f)
                    {
                        if (transform.position.y < 3.5f)
                        {
                            needUnstuck = true;
                        }
                    }
                }

                if (needUnstuck)
                    fixedPos = putHere;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////



            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (currentScene == "Crossroads_08")
            {
                Vector2 putHere = new Vector2(31f, 15.5f);

                if (transform.position.y < 4f || transform.position.y > 29.5f)
                {
                    needUnstuck = true;
                }

                if (transform.position.x < 18.5f || transform.position.x > 43f)
                {
                    needUnstuck = true;
                }

                if (needUnstuck)
                    fixedPos = putHere;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////


            if (needUnstuck)
            {
                if(poob != null)
                {
                    poob.ForcePosition(fixedPos);
                    transform.position = fixedPos;
                }
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