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
                    EnemyHealthManager.hp = value;
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
        public float AggroRange => specialAggroRange == null ? aggroRange : specialAggroRange.Value;
        public float? specialAggroRange;
        public bool doSpecialBossAggro;
        public virtual bool didDisableCameraLocks { get; protected set; }
        protected virtual bool DisableCameraLocks => false;
        protected virtual IEnumerable<CameraLockArea> cameraLocks => DisableCameraLocks ? gameObject.GetCameraLocksFromScene() : null;

#if DEBUG
        public virtual bool showDebugColliders => true;
#endif

        public override void Setup(GameObject objectThatWillBeReplaced = null)
        {
            base.Setup(objectThatWillBeReplaced);

            if (isUnloading)
                return;

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
                else if (objectThatWillBeReplaced.name.Contains("fluke_baby"))
                {
                    MaxHP = 1;
                    gameObject.ScaleObject(0.25f);
                    gameObject.ScaleAudio(0.25f);
                    bool canDamageHero = SpawnerExtensions.RollProbability(out _, 10, 100);
                    if (!canDamageHero)
                    {
                        var damageHero = gameObject.GetComponent<DamageHero>();
                        if (damageHero != null)
                        {
                            damageHero.damageDealt = 0;
                        }
                    }
                }
                else if (objectThatWillBeReplaced.IsBoss())
                {
                    CheckHackyBossSetup(objectThatWillBeReplaced);
                }
                else
                {
                    CheckHackyFixesOnSetup(objectThatWillBeReplaced);

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

        public virtual int GetStartingMaxHP(GameObject objectThatWillBeReplaced)
        {
            if (isUnloading)
                return 0;

            if (objectThatWillBeReplaced == null)
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

        public virtual int GetStartingMaxHP(string prefabName)
        {
            if (isUnloading)
                return 0;

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

            if (!CanSpawnInScene)
                return;

            try
            {
                CheckHackySceneFixesOnDeath();

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
                if (doSpecialBossAggro)
                    return true;

                return gameObject.IsNearPlayer(AggroRange);
            }
            else
            {
                if (doSpecialBossAggro)
                {
                    if (gameObject.IsNearPlayer(AggroRange) && gameObject.CanSeePlayer())
                    {
                        hasSeenPlayer = true;
                        return true;
                    }
                }
                else
                {
                    if (gameObject.CanSeePlayer())
                    {
                        hasSeenPlayer = true;
                        return true;
                    }
                }

                return false;
            }
        }

        float grimmTimeout = 0f;
        protected virtual void Update()
        {
            if (!loaded)
                return;

            if (gameObject.ObjectType() != PrefabObject.PrefabType.Enemy)
                return;

            if (gameObject != null && gameObject.GetDatabaseKey() == "Grimm Boss")
            {
                var corpse = gameObject.GetCorpseObject();
                if (corpse != null)
                    GameObject.Destroy(corpse);

                if (control.ActiveStateName == "AD Fire")
                {
                    grimmTimeout += Time.deltaTime;
                    if (grimmTimeout > 1f)
                    {
                        control.SendEvent("LAND");
                        grimmTimeout = 0f;
                    }
                }
                else if (control.ActiveStateName == "AD Edge")
                {
                    grimmTimeout += Time.deltaTime;
                    if (grimmTimeout > 1f)
                    {
                        control.SendEvent("LAND");
                        grimmTimeout = 0f;
                    }
                }
                else
                {
                    grimmTimeout = 0f;
                }
            }

            try
            {
                CheckHackySceneFixesOnUpdate();
            }
            catch (Exception e) { Dev.Log($"Caught exception in CheckHackySceneFixesOnUpdate :  \n{e.Message}\n{e.StackTrace}"); }

            if (counter > 0)
            {
                counter--;
                if (counter <= 0 && lastSetHP > 0)
                {
                    MaxHP = lastSetHP;

                    var orig = ObjectMetadata.GetOriginal(gameObject);

                    if (specialAggroRange != null && orig != null && !IsInColo())
                    {
                        var poob = gameObject.GetComponent<PreventOutOfBounds>();
                        string currentScene = SpawnerExtensions.SceneName(gameObject);



                        if (orig.ObjectName.Contains("False Knight Dream"))
                        {
                            if (currentScene == "Dream_01_False_Knight")//false knight dream / failed champ
                            {
                                UnFreeze();
                                transform.position = new Vector3(60f, 35f);
                                if (poob != null)
                                    poob.ForcePosition(new Vector3(60f, 35f));
                            }
                        }
                        if (orig.ObjectName.Contains("False Knight"))
                        {
                            if (currentScene == "Crossroads_10_boss" || currentScene == "Crossroads_10")//false knight
                            {
                                UnFreeze();
                                transform.position = new Vector3(29f, 38f);
                                if (poob != null)
                                    poob.ForcePosition(new Vector3(29f, 38f));
                            }
                        }
                        else if (orig.ObjectName.Contains("Mage Lord") && !orig.ObjectName.Contains("Phase2"))
                        {
                            PlayMakerFSM.BroadcastEvent("CANCEL SHAKE");
                            UnFreeze();
                            transform.position = new Vector3(16f, 34f);
                            if (poob != null)
                                poob.ForcePosition(new Vector3(16f, 34f));
                        }
                        else if (orig.ObjectName.Contains("Mage Lord") && orig.ObjectName.Contains("Phase2"))
                        {
                            PlayMakerFSM.BroadcastEvent("CANCEL SHAKE");
                            UnFreeze();
                            transform.position = new Vector3(16f, 13f);
                            if (poob != null)
                                poob.ForcePosition(new Vector3(16f, 13f));
                        }
                        else if (orig.ObjectName.Contains("Dream Mage Lord") && !orig.ObjectName.Contains("Phase2"))
                        {
                            PlayMakerFSM.BroadcastEvent("CANCEL SHAKE");
                            UnFreeze();
                            transform.position = new Vector3(20f, 35f);
                            if (poob != null)
                                poob.ForcePosition(new Vector3(20f, 35f));
                        }
                        else if (orig.ObjectName.Contains("Dung Defender"))
                        {
                            UnFreeze();
                            transform.position = new Vector3(75f, 13f);
                            if (poob != null)
                                poob.ForcePosition(new Vector3(75f, 13f));
                        }
                        else if (orig.ObjectName.Contains("Fluke Mother"))
                        {
                            UnFreeze();
                            transform.position = new Vector3(18f, 17f);
                            if (poob != null)
                                poob.ForcePosition(new Vector3(18f, 17f));
                        }
                        else if (orig.ObjectName.Contains("Hornet Boss 2"))
                        {
                            UnFreeze();
                            transform.position = new Vector3(34f, 33f);
                            if (poob != null)
                                poob.ForcePosition(new Vector3(34f, 33f));
                        }
                        else if (orig.ObjectName.Contains("Hornet Boss 1"))
                        {
                            UnFreeze();
                            transform.position = new Vector3(27f, 35f);
                            if (poob != null)
                                poob.ForcePosition(new Vector3(27f, 35f));
                        }
                        else if (orig.ObjectName.Contains("Traitor Lord"))
                        {
                            UnFreeze();
                            transform.position = new Vector3(44.0f, 35.9f);
                            if (poob != null)
                                poob.ForcePosition(new Vector3(44.0f, 35.9f));
                        }
                        else if (orig.ObjectName.Contains("Jar Collector"))
                        {
                            UnFreeze();
                            transform.position = new Vector3(55f, 100f);
                            if (poob != null)
                                poob.ForcePosition(new Vector3(55f, 100f));
                        }
                        else if (orig.ObjectName.Contains("Lost Kin"))
                        {
                            UnFreeze();
                            transform.position = new Vector3(27f, 34f);
                            if (poob != null)
                                poob.ForcePosition(new Vector3(27f, 34f));
                        }
                        else if (orig.ObjectName.Contains("Mawlek Body"))
                        {
                            UnFreeze();
                            transform.position = new Vector3(62f, 8f, 0f);
                            if (poob != null)
                                poob.ForcePosition(new Vector3(62f, 8f, 0f));

                            var hitbox = gameObject.GetComponent<Collider2D>();
                            if (hitbox != null)
                                hitbox.enabled = true;

                            if (MRenderer != null)
                                MRenderer.enabled = true;

                            //make fluke mother big if she spawns in mawlek's room
                            if (thisMetadata.GetDatabaseKey() == "Fluke Mother")
                            {
                                var pos = new Vector3(61.8909f, 12.1455f, 0);
                                transform.position = pos;
                                if (poob != null)
                                    poob.ForcePosition(pos);
                                gameObject.ScaleObject(1f);
                                gameObject.ScaleAudio(0.8f);
                                SizeScale = 1f;
                            }
                        }
                        else
                        {

                        }
                    }
                }
            }

#if DEBUG
            if (showDebugColliders && debugColliders == null)
            {
                debugColliders = gameObject.GetOrAddComponent<DebugColliders>();
            }
            else if (showDebugColliders && !debugColliders.enabled)
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
                bool didAggro = false;
                string currentScene = SpawnerExtensions.SceneName(gameObject);
                if (currentScene == "Crossroads_09")//mawlek
                {
                    if (BattleManager.StateMachine.Value.battleStarted)
                        didAggro = true;
                }
                else if (HeroInAggroRange() || BattleManager.AggroAllBossesNow)
                {
                    didAggro = true;
                }


                if (didAggro)
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
            else if (ratio < 0.5f)
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
            if (zoneScale == MetaDataTypes.ProgressionZoneScale["DREAM_WORLD"] ||
               zoneScale == MetaDataTypes.ProgressionZoneScale["FINAL_BOSS"])
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

            if (!EnemyRandomizerDatabase.GetGlobalSettings().balanceReplacementHP)
                return;

            if (SpawnedObjectControl.VERBOSE_DEBUG)
                Dev.Log($"{this} hp was {CurrentHP}");

            if (objectThatWillBeReplaced == null && originialMetadata != null)
            {
                defaultScaledMaxHP = GetStartingMaxHP(originialMetadata.GetDatabaseKey());
                CurrentHP = defaultScaledMaxHP;
            }
            else if (objectThatWillBeReplaced != null)
            {
                defaultScaledMaxHP = GetStartingMaxHP(objectThatWillBeReplaced);
                CurrentHP = defaultScaledMaxHP;
            }
            else
            {
                //use default hp
            }

            //failsafe
            if (CurrentHP <= 0)
            {
                MaxHP = 1 + MetaDataTypes.ProgressionZoneScale[GameManager.instance.GetCurrentMapZone()] * 2;
            }

            if (SpawnedObjectControl.VERBOSE_DEBUG)
                Dev.Log($"Setting {this} hp to {CurrentHP}");
        }

        protected virtual void SetupEnemyGeo()
        {
            if (!EnemyRandomizerDatabase.GetGlobalSettings().randomizeReplacementGeo)
            {
                int basicGeo = SpawnerExtensions.GetOriginalGeo(thisMetadata.ObjectName);
                if (originialMetadata != null && thisMetadata != originialMetadata)
                {
                    basicGeo = SpawnerExtensions.GetOriginalGeo(originialMetadata.ObjectName);
                }
                Geo = basicGeo;
                return;
            }

            bool isBoss = false;
            int originalGeo = SpawnerExtensions.GetOriginalGeo(thisMetadata.ObjectName);
            if (originialMetadata != null && thisMetadata != originialMetadata)
            {
                originalGeo = SpawnerExtensions.GetOriginalGeo(originialMetadata.ObjectName);
                isBoss = SpawnerExtensions.IsBoss(originialMetadata.GetDatabaseKey());
            }

            var zone = GameManager.instance.GetCurrentMapZone();
            if (MetaDataTypes.GeoZoneScale.TryGetValue(zone, out int geoScale))
            {
                if (originalGeo <= 0)
                {
                    Geo = SpawnerExtensions.GetRandomValueBetween(geoScale, geoScale * 5 + 1);
                }
                else
                {
                    if (isBoss)
                    {
                        Geo = SpawnerExtensions.GetRandomValueBetween(geoScale * 50, geoScale * 100 + 1);
                    }
                    else
                    {
                        Geo = SpawnerExtensions.GetRandomValueBetween(1, originalGeo * geoScale + 1);
                    }
                }
            }
            else
            {
                Geo = SpawnerExtensions.GetRandomValueBetween(1 + originalGeo, originalGeo * 2 + 1);
            }
        }

        public override void RecieveExtraDamage(ExtraDamageTypes extraDamageType)
        {
            if (isUnloading)
                return;

            base.RecieveExtraDamage(extraDamageType);
            if (takesSpecialCharmDamage && EnemyHealthManager != null)
            {
                int dmgAmount = ExtraDamageable.GetDamageOfType(extraDamageType);
                EnemyHealthManager.ApplyExtraDamage(dmgAmount);
            }
        }

        public override void Hit(HitInstance damageInstance)
        {
            if (isUnloading)
                return;

            base.Hit(damageInstance);
            if (takesSpecialSpellDamage && EnemyHealthManager != null && damageInstance.AttackType == AttackTypes.Spell)
            {
                int dmgAmount = damageInstance.DamageDealt;
                EnemyHealthManager.ApplyExtraDamage(dmgAmount);
            }
        }

        float hackyClimberFixResetTimer = 0f;
        float hackyClimberFixTimer = 0f;
        float previousClimberY = 0f;
        bool hackyClimberFixOnce = false;
        protected virtual void CheckHackySceneFixesOnUpdate()
        {
            DisableCollidersForBackgroundThings();

            if (gameObject.GetDatabaseKey() == "Ceiling Dropper")
            {
                var alertRange = gameObject.FindGameObjectInDirectChildren("Alert Range");
                if (alertRange != null)
                {
                    var aBox = alertRange.GetComponent<BoxCollider2D>();
                    if (aBox != null)
                    {
                        aBox.size = new Vector2(aBox.size.x, 50f);
                    }
                }
            }

            if (!gameObject.IsBattleEnemy() && gameObject.IsClimbing() && !hackyClimberFixOnce)
            {
                var rayUp = Physics2D.Raycast(transform.position, Vector2.up);
                var rayDown = Physics2D.Raycast(transform.position, Vector2.down);
                var rayLeft = Physics2D.Raycast(transform.position, Vector2.left);
                var rayRight = Physics2D.Raycast(transform.position, Vector2.right);
                if (rayUp.collider is PolygonCollider2D || rayUp.collider is EdgeCollider2D)
                {
                    bool leftIsPoly = rayLeft.collider is PolygonCollider2D || rayLeft.collider is EdgeCollider2D;
                    bool rightIsPoly = rayRight.collider is PolygonCollider2D || rayRight.collider is EdgeCollider2D;

                    if (rayDown.distance > 1f)
                    {
                        gameObject.SetActive(false);
                        gameObject.SetRotation(0f);
                        gameObject.StickToGroundX(spawnPositionOffset * .2f);

                        var climber = gameObject.GetComponent<Climber>();
                        climber.startRight = false;
                        var f = climber.GetType().GetField("previousPos", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        f.SetValue(climber, transform.position.ToVec2());

                        var f2 = climber.GetType().GetField("previousTurnPos", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        f.SetValue(climber, transform.position.ToVec2());

                        try
                        {
                            var f3 = climber.GetType().GetField("currentDirection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            f.SetValue(climber, 2);
                        }
                        catch (Exception) { }

                        gameObject.SetActive(true);

                        try
                        {
                            var s = climber.GetType().GetMethod("Start", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            s.Invoke(climber, null);
                        }
                        catch (Exception) { }
                        hackyClimberFixOnce = true;
                    }
                }
            }

            if (hackyClimberFixOnce && gameObject.IsClimbing())
            {
                var up = gameObject.GetUpFromSelfAngle(false);
                if (Mathf.Abs((up.y - previousClimberY)) > 0.01f)
                {
                    hackyClimberFixTimer += Time.deltaTime;
                    hackyClimberFixResetTimer = 0f;

                    if (hackyClimberFixTimer > 1.0f)
                    {
                        hackyClimberFixOnce = false;

                        hackyClimberFixTimer = 0f;
                    }
                }
                else
                {
                    hackyClimberFixResetTimer += Time.deltaTime;
                    if (hackyClimberFixResetTimer > 1f)
                    {
                        hackyClimberFixTimer = 0f;
                    }
                }

                up.y = previousClimberY;
            }

            string currentScene = SpawnerExtensions.SceneName(gameObject);

            if (gameObject == null)
                return;

            var poob = gameObject.GetComponent<PreventOutOfBounds>();
            bool needUnstuck = false;
            Vector2 fixedPos = Vector2.zero;

            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (currentScene == "Crossroads_01")
            {
                Vector2 putHere = new Vector2(52.5f, 18f);

                if (IsInBox(new Vector2(6f, 10f), new Vector2(12f, 8f)))
                {
                    putHere = new Vector2(10f, 12f);
                    needUnstuck = true;
                }

                if (transform.position.y > 20)
                {
                    needUnstuck = true;
                }

                if (transform.position.x < 37f)
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


            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (currentScene == "Crossroads_07")
            {
                Vector2 putHere = new Vector2(transform.position.x, 83f);

                if (transform.position.y > 93f)
                {
                    needUnstuck = true;
                }

                if (transform.position.x < 0f || transform.position.x > 42f)
                {
                    putHere = new Vector2(21f, 83f);
                }


                if (transform.position.x < 18f && transform.position.y < 2.5f)
                {
                    putHere = new Vector2(10.5f, 10f);
                }

                if ((transform.position.x > 22f && transform.position.y < 2.5f) || transform.position.y < 2.5f)
                {
                    putHere = new Vector2(33f, 10f);
                }

                if (needUnstuck)
                    fixedPos = putHere;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////


            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (currentScene == "Crossroads_13")
            {
                Vector2 putHere = new Vector2(19.5f, 13.5f);

                if (transform.position.y < 11f && transform.position.x < 8f)
                {
                    needUnstuck = true;
                }
                else if (transform.position.y < 1.5f)
                {
                    putHere = new Vector2(transform.position.x, 12.5f);
                    needUnstuck = true;
                }

                if (needUnstuck)
                    fixedPos = putHere;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////


            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (currentScene == "Crossroads_03")
            {
                Vector2 putHere = new Vector2(22.5f, 35.5f);

                if (IsInBox(new Vector2(26f, 53f), new Vector2(45f, 37f)))
                {
                    needUnstuck = true;
                }
                else if (transform.position.x > 25f)
                {
                    needUnstuck = true;
                }

                if (needUnstuck)
                    fixedPos = putHere;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////




            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (currentScene == "Ruins1_09")
            {
                Vector2 putHere = new Vector2(19f, 20f);

                if (transform.position.y > 28f)
                {
                    needUnstuck = true;
                }
                else if (transform.position.x < 3f)
                {
                    needUnstuck = true;
                }
                else if (transform.position.y < 11.5f)
                {
                    needUnstuck = true;
                }
                else if (transform.position.x > 39f)
                {
                    needUnstuck = true;
                }

                if (needUnstuck)
                    fixedPos = putHere;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////




            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (currentScene == "Ruins1_17")
            {
                Vector2 putHere = new Vector2(transform.position.x, transform.position.y);

                if (transform.position.x < 2f)
                {
                    putHere.x = 27.8f;
                    needUnstuck = true;
                }
                else if (transform.position.x > 85f)
                {
                    putHere.x = 48f;
                    needUnstuck = true;
                }
                else if (transform.position.y < 1.5f)
                {
                    putHere.y = 4.5f;
                    needUnstuck = true;
                }
                else if (transform.position.y > 69f)
                {
                    putHere.y = 50f;
                    needUnstuck = true;
                }

                if (needUnstuck)
                    fixedPos = putHere;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////



            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (currentScene == "Ruins1_28")
            {
                Vector2 putHere = new Vector2(73f, 15f);

                if (transform.position.x < 1f || transform.position.y < 1f)
                {
                    needUnstuck = true;
                }

                if (needUnstuck)
                    fixedPos = putHere;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////




            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (currentScene == "Deepnest_16")
            {
                Vector2 putHere = new Vector2(55f, 19f);

                if (transform.position.x < 1f || transform.position.y < 1f)
                {
                    needUnstuck = true;
                }

                if (needUnstuck)
                    fixedPos = putHere;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////




            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (currentScene == "Deepnest_32") //nosk
            {
                Vector2 putHere = new Vector2(100f, 11f);

                if (transform.position.y > 18f)
                {
                    putHere = new Vector2(transform.position.x, 18f);
                    needUnstuck = true;
                }

                if (transform.position.x > 119f || transform.position.y < 2.5f || transform.position.x < 70.5f)
                {
                    needUnstuck = true;
                }

                if (needUnstuck)
                    fixedPos = putHere;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////







            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (currentScene == "Deepnest_17")
            {
                Vector2 putHere = new Vector2(23f, 17f);

                if (transform.position.x < 1f || transform.position.y < 1f)
                {
                    needUnstuck = true;
                }
                else if (transform.position.x > 29f || transform.position.y > 60f)
                {
                    putHere = new Vector2(20f, 49f);
                    needUnstuck = true;
                }
                else if (IsInBox(new Vector2(0f, 62f), new Vector2(14f, 60f)))
                {
                    putHere = new Vector2(20f, 49f);
                    needUnstuck = true;
                }
                else if (IsInBox(new Vector2(7f, 46f), new Vector2(23f, 44f)))
                {
                    putHere = new Vector2(20f, 49f);
                    needUnstuck = true;
                }
                else if (IsInBox(new Vector2(16f, 23f), new Vector2(30f, 21f)))
                {
                    putHere = new Vector2(20f, 49f);
                    needUnstuck = true;
                }
                else if (IsInBox(new Vector2(8f, 18f), new Vector2(20f, 16f)))
                {
                    putHere = new Vector2(20f, 49f);
                    needUnstuck = true;
                }

                if (needUnstuck)
                    fixedPos = putHere;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////





            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (currentScene == "Ruins1_24" || currentScene == "Ruins1_24_boss")
            {
                bool hero_inPhase1Area = HeroController.instance.gameObject.IsInBox(new Vector2(5f, 41f), new Vector2(37f, 28f));
                bool hero_inPhase2Area = HeroController.instance.gameObject.IsInBox(new Vector2(8f, 25f), new Vector2(24f, 8.5f));

                bool boss_inPhase1Area = IsInBox(new Vector2(5f, 41f), new Vector2(37f, 28f));
                bool boss_inPhase2Area = IsInBox(new Vector2(8f, 25f), new Vector2(42f, 8.5f));

                Vector2 putHere_phase1 = new Vector2(10f, 34f);
                Vector2 putHere_phase2 = new Vector2(32f, 16f);

                Vector2 putHere = putHere_phase1;

                if (hero_inPhase1Area && !boss_inPhase1Area)
                {
                    putHere = putHere_phase1;
                    needUnstuck = true;
                }
                else if (hero_inPhase2Area && !boss_inPhase2Area)
                {
                    putHere = putHere_phase2;
                    needUnstuck = true;
                }

                if (needUnstuck)
                    fixedPos = putHere;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////



            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (currentScene == "Crossroads_04")
            {
                Vector2 putHere = new Vector2(80f, 5.0f);

                if (IsInBox(new Vector2(79f, 15.7f), new Vector2(85f, 10f)))
                {
                    needUnstuck = true;
                }
                else if (transform.position.y < 0f)
                {
                    needUnstuck = true;
                }

                if (needUnstuck)
                    fixedPos = putHere;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////







            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (currentScene == "Fungus1_11")
            {
                Vector2 putHere = new Vector2(12f, 62f);

                if (transform.position.y > 68f)
                {
                    needUnstuck = true;
                }
                else if (IsInBox(new Vector2(23f, 68f), new Vector2(34f, 60f)))
                {
                    needUnstuck = true;
                }
                else if (IsInBox(new Vector2(22f, 58f), new Vector2(60f, 54f)))
                {
                    needUnstuck = true;
                }
                else if (IsInBox(new Vector2(47.5f, 59f), new Vector2(55f, 45f)))
                {
                    needUnstuck = true;
                }
                else if (IsInBox(new Vector2(10.5f, 55.5f), new Vector2(20f, 52.5f)))
                {
                    putHere = new Vector2(20f, 48f);
                    needUnstuck = true;
                }

                if (needUnstuck)
                    fixedPos = putHere;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////







            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (currentScene == "Fungus3_01")
            {
                Vector2 putHere = new Vector2(8f, 75f);

                if (transform.position.y > 80f)
                {
                    needUnstuck = true;
                }
                else if (transform.position.x < 1f)
                {
                    putHere = new Vector2(5f, 45f);
                    needUnstuck = true;
                }
                else if (transform.position.x > 32f)
                {
                    putHere = new Vector2(27f, 45f);
                    needUnstuck = true;
                }
                else if (IsInBox(new Vector2(9f, 73f), new Vector2(24f, 69f)))
                {
                    needUnstuck = true;
                }

                if (needUnstuck)
                    fixedPos = putHere;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////




            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (currentScene == "Fungus2_04")
            {
                Vector2 putHere = new Vector2(4.5f, 66f);

                if (transform.position.y > 74.5f)
                {
                    needUnstuck = true;
                }
                else if (transform.position.x < 1f)
                {
                    putHere = new Vector2(8.5f, 40f);
                    needUnstuck = true;
                }
                else if (transform.position.x > 33f)
                {
                    putHere = new Vector2(15f, 14f);
                    needUnstuck = true;
                }
                else if (IsInBox(new Vector2(9f, 73f), new Vector2(24f, 69f)))
                {
                    needUnstuck = true;
                }

                if (needUnstuck)
                    fixedPos = putHere;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////
            ///






            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (currentScene == "Fungus2_05")
            {
                Vector2 putHere = new Vector2(transform.position.x, 15f);

                if (transform.position.y < 12f)
                {
                    needUnstuck = true;
                }
                if (transform.position.y > 23.2f)
                {
                    needUnstuck = true;
                }
                else if (transform.position.x < 14f)
                {
                    putHere = new Vector2(17.5f, 14.5f);
                    needUnstuck = true;
                }
                else if (transform.position.x > 45.5f)
                {
                    putHere = new Vector2(42f, 15f);
                    needUnstuck = true;
                }
                //else if (IsInBox(new Vector2(9f, 73f), new Vector2(24f, 69f)))
                //{
                //    needUnstuck = true;
                //}

                if (needUnstuck)
                    fixedPos = putHere;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////
            ///







            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (currentScene == "Fungus2_08")
            {
                Vector2 putHere = new Vector2(5f, transform.position.y);

                if (transform.position.x < 1f)
                {
                    needUnstuck = true;
                }
                else if (transform.position.x > 28f)
                {
                    putHere = new Vector2(20f, transform.position.y);
                    needUnstuck = true;
                }
                else if (transform.position.x < 14f)
                {
                    putHere = new Vector2(17.5f, 14.5f);
                    needUnstuck = true;
                }
                else if (transform.position.x > 45.5f)
                {
                    putHere = new Vector2(42f, 15f);
                    needUnstuck = true;
                }
                //else if (IsInBox(new Vector2(9f, 73f), new Vector2(24f, 69f)))
                //{
                //    needUnstuck = true;
                //}

                if (needUnstuck)
                    fixedPos = putHere;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////
            ///






            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (currentScene == "Fungus2_11")
            {
                Vector2 putHere = new Vector2(13f, 36f);

                if (IsInBox(new Vector2(8f, 31f), new Vector2(19f, 29f)))
                {
                    needUnstuck = true;
                }

                if (needUnstuck)
                    fixedPos = putHere;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////










            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (currentScene == "Crossroads_21")
            {
                Vector2 putHere = new Vector2(transform.position.x, 3.3f);

                if (transform.position.y < 1.5f)
                {
                    needUnstuck = true;
                }

                if (needUnstuck)
                    fixedPos = putHere;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////



            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (currentScene == "Fungus1_23")
            {
                Vector2 putHere = new Vector2(transform.position.x, 9f);

                if (transform.position.y < 4f)
                {
                    needUnstuck = true;
                }

                if (needUnstuck)
                    fixedPos = putHere;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////





            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (currentScene == "Waterways_08")
            {
                Vector2 putHere = new Vector2(transform.position.x, 7.5f);

                //lower regions
                if (pos2d.y < 7.5f)
                {
                    putHere = new Vector2(transform.position.x, 7.5f);
                    if (pos2d.x > 63 && pos2d.x < 67f)
                    {
                        putHere.y = 4.5f;
                    }
                    else if (pos2d.x < 30f)
                    {
                        putHere.y = 6f;
                    }

                    if (pos2d.y < 1f)
                    {
                        needUnstuck = true;
                    }
                    else if (IsInBox(new Vector2(1f, 1f), new Vector2(100f, -10f)))
                    {
                        needUnstuck = true;
                    }
                    else if (IsInBox(new Vector2(105f, 5.8f), new Vector2(140f, -10f)))
                    {
                        needUnstuck = true;
                    }
                    else if (IsInBox(new Vector2(88f, 5.5f), new Vector2(96f, -10f)))
                    {
                        needUnstuck = true;
                    }
                    else if (IsInBox(new Vector2(70f, 3.5f), new Vector2(78f, -10f)))
                    {
                        needUnstuck = true;
                    }
                    else if (IsInBox(new Vector2(0, 10f), new Vector2(28f, 7f)))
                    {
                        needUnstuck = true;
                    }
                }

                if (pos2d.y > 7.5f)
                {
                    putHere = new Vector2(transform.position.x, 7.5f);
                    if (pos2d.x > 63 && pos2d.x < 67f)
                    {
                        putHere.y = 4.5f;
                    }
                    else if (pos2d.x < 30f)
                    {
                        putHere.y = 6f;
                    }

                    if (IsInBox(new Vector2(90f, 21.8f), new Vector2(93f, 10.1f)))
                    {
                        needUnstuck = true;
                    }
                    else if (IsInBox(new Vector2(93f, 21.8f), new Vector2(107f, 13f)))
                    {
                        needUnstuck = true;
                    }
                    else if (IsInBox(new Vector2(103f, 13f), new Vector2(107f, 10f)))
                    {
                        needUnstuck = true;
                    }
                    else if (IsInBox(new Vector2(47f, 21.8f), new Vector2(84f, 11f)))
                    {
                        needUnstuck = true;
                    }
                    else if (IsInBox(new Vector2(34f, 31f), new Vector2(48f, 13.2f)))
                    {
                        needUnstuck = true;
                    }
                    else if (IsInBox(new Vector2(0, 10f), new Vector2(28f, 7f)))
                    {
                        needUnstuck = true;
                    }
                }

                if (needUnstuck)
                    fixedPos = putHere;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////






            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (currentScene == "Dream_02_Mage_Lord")
            {
                bool hero_inPhase1Area = HeroController.instance.gameObject.IsInBox(new Vector2(4f, 41f), new Vector2(37f, 27f));
                bool hero_inPhase2Area = HeroController.instance.gameObject.IsInBox(new Vector2(8f, 25f), new Vector2(24f, 8.5f));

                bool boss_inPhase1Area = IsInBox(new Vector2(4f, 41f), new Vector2(37f, 27f));
                bool boss_inPhase2Area = IsInBox(new Vector2(8f, 25f), new Vector2(42f, 8.5f));

                Vector2 putHere_phase1 = new Vector2(10f, 34f);
                Vector2 putHere_phase2 = new Vector2(32f, 16f);

                Vector2 putHere = putHere_phase1;

                if (hero_inPhase1Area && !boss_inPhase1Area)
                {
                    putHere = putHere_phase1;
                    needUnstuck = true;
                }
                else if (hero_inPhase2Area && !boss_inPhase2Area)
                {
                    putHere = putHere_phase2;
                    needUnstuck = true;
                }

                if (needUnstuck)
                    fixedPos = putHere;






                if (HeroController.instance.transform.position.y > 23f)
                {
                    {
                        var gate = GameObject.Find("Dream Gate Phase 2");
                        if (gate != null)
                            gate.SafeSetActive(false);
                    }

                    {
                        var gate = GameObject.Find("Dream Gate Phase 2 (1)");
                        if (gate != null)
                            gate.SafeSetActive(false);
                    }

                    {
                        var gate = GameObject.Find("Dream Gate Phase 2 (2)");
                        if (gate != null)
                            gate.SafeSetActive(false);
                    }

                    {
                        var gate = GameObject.Find("Dream Gate Phase 2 (3)");
                        if (gate != null)
                            gate.SafeSetActive(false);
                    }
                }
                else
                {
                    {
                        var gate = GameObject.Find("Dream Gate Phase 2");
                        if (gate != null)
                            gate.SafeSetActive(true);
                    }

                    {
                        var gate = GameObject.Find("Dream Gate Phase 2 (1)");
                        if (gate != null)
                            gate.SafeSetActive(true);
                    }

                    {
                        var gate = GameObject.Find("Dream Gate Phase 2 (2)");
                        if (gate != null)
                            gate.SafeSetActive(true);
                    }

                    {
                        var gate = GameObject.Find("Dream Gate Phase 2 (3)");
                        if (gate != null)
                            gate.SafeSetActive(true);
                    }
                }
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////


















            //trigger lasers
            if (gameObject.ObjectType() == PrefabObject.PrefabType.Enemy && (currentScene == "Mines_18_boss" || currentScene == "Mines_18" || currentScene == "Mines_32"))
            {
                hackyLaserCDTimer += Time.deltaTime;
                if ((currentScene == "Mines_32" && hackyLaserCDTimer > 4f) || (hackyLaserCDTimer > 6f))
                {
                    foreach (var t in GameObjectExtensions.EnumerateRootObjects().Where(x => x.name.Contains("Turret Mega")))
                    {
                        if (!t.activeInHierarchy)
                        {
                            t.SafeSetActive(true);
                        }

                        if (t.LocateMyFSM("Laser Bug Mega") != null && !t.LocateMyFSM("Laser Bug Mega").enabled)
                        {
                            t.LocateMyFSM("Laser Bug Mega").enabled = true;
                        }
                    }
                    PlayMakerFSM.BroadcastEvent("LASER SHOOT");
                    hackyLaserCDTimer = SpawnerExtensions.GetRandomValueBetween(-2, 2);
                }
            }

            //trigger grim spikes
            if (gameObject.ObjectType() == PrefabObject.PrefabType.Enemy && (currentScene.Contains("Grimm_")))
            {
                hackyLaserCDTimer += Time.deltaTime;
                if ((currentScene.Contains("Grimm_Nightmare") && hackyLaserCDTimer > 4f) || (hackyLaserCDTimer > 6f))
                {
                    foreach (var t in GameObjectExtensions.EnumerateRootObjects().Where(x => x.name.Contains("Grimm Spike Holder")))
                    {
                        if (!t.activeInHierarchy)
                        {
                            t.SafeSetActive(true);
                        }

                        if (t.LocateMyFSM("Spike Control") != null && !t.LocateMyFSM("Spike Control").enabled)
                        {
                            t.LocateMyFSM("Spike Control").enabled = true;
                        }
                    }
                    PlayMakerFSM.BroadcastEvent("SPIKE ATTACK");
                    hackyLaserCDTimer = SpawnerExtensions.GetRandomValueBetween(-2, 2);
                }
            }



            //trigger hive knight attacks
            if (gameObject.ObjectType() == PrefabObject.PrefabType.Enemy && (currentScene.Contains("Hive_05")))
            {
                int attack = SpawnerExtensions.GetRandomValueBetween(0, 2);
                hackyLaserCDTimer += Time.deltaTime;
                if (hackyLaserCDTimer > 10f)
                {
                    if (attack == 0)
                    {
                        foreach (var t in BattleManager.Instance.Value.gameObject.FindGameObjectInDirectChildren("Droppers").EnumerateChildren().Where(x => x.name.Contains("Bee Dropper")))
                        {
                            if (!t.activeInHierarchy)
                            {
                                t.SafeSetActive(true);
                            }

                            if (t.LocateMyFSM("Control") != null && !t.LocateMyFSM("Control").enabled)
                            {
                                t.LocateMyFSM("Control").enabled = true;
                            }
                        }
                        PlayMakerFSM.BroadcastEvent("SWARM");
                        hackyLaserCDTimer = SpawnerExtensions.GetRandomValueBetween(-5, 5);
                    }
                    else if (attack == 1)
                    {
                        var t = BattleManager.Instance.Value.gameObject.FindGameObjectInDirectChildren("Globs");
                        {
                            if (!t.activeInHierarchy)
                            {
                                t.SafeSetActive(true);
                            }

                            if (t.LocateMyFSM("Control") != null && !t.LocateMyFSM("Control").enabled)
                            {
                                t.LocateMyFSM("Control").enabled = true;
                            }
                        }
                        PlayMakerFSM.BroadcastEvent("FIRE");
                        hackyLaserCDTimer = SpawnerExtensions.GetRandomValueBetween(-5, 5);
                    }
                    else
                    {
                        hackyLaserCDTimer = SpawnerExtensions.GetRandomValueBetween(-1, 1);
                    }
                }
            }

            if (needUnstuck)
            {
                if (poob != null)
                {
                    poob.ForcePosition(fixedPos);
                    transform.position = fixedPos;
                }
                else
                {
                    transform.position = fixedPos;
                }
            }
        }

        public bool IsInBox(Vector2 topleft, Vector2 bottomRight)
        {
            var point = pos2d;

            // Check if the point's X coordinate is within the box's X range
            bool withinXRange = point.x >= topleft.x && point.x <= bottomRight.x;

            // Check if the point's Y coordinate is within the box's Y range
            bool withinYRange = point.y <= topleft.y && point.y >= bottomRight.y;

            // Return true if the point is within both the X and Y ranges, indicating it is inside the box
            return withinXRange && withinYRange;
        }

        float hackyLaserCDTimer = 0f;

        protected override void OnHeroInAggroRangeTheFirstTime()
        {
            base.OnHeroInAggroRangeTheFirstTime();

            CheckHackyOnAggroFixes();
        }

        protected override void PreSetSpawnPosition(GameObject objectThatWillBeReplaced)
        {
            base.PreSetSpawnPosition(objectThatWillBeReplaced);

            if (originialMetadata == null)
                return;

            //????? should never happen
            if (thisMetadata == null)
                return;

            if (objectThatWillBeReplaced != null)
            {
                var up = objectThatWillBeReplaced.GetUpFromSelfAngle(false);
                Vector2 upwardShift = transform.position.ToVec2() + up * 2f;
                Vector2 nearestWall = Vector2.zero;
                Vector2 farthestWall = Vector2.zero;
                Vector2 roofPoint = Vector2.zero;
                Vector2 roofToDownLeftPoint = Vector2.zero;
                Vector2 roofToDownRightPoint = Vector2.zero;
                bool isRoofFlat = false;

                var leftRay = SpawnerExtensions.GetRayOn(transform.position, Vector2.left, 200f);
                var rightRay = SpawnerExtensions.GetRayOn(transform.position, Vector2.right, 200f);
                var roofRay = SpawnerExtensions.GetRayOn(transform.position, Vector2.up, 200f);
                var roofToGroundLeft = SpawnerExtensions.GetRayOn(roofRay.point + Vector2.down * 8f, (Vector2.down + Vector2.left).normalized, 200f);
                var roofToGroundRight = SpawnerExtensions.GetRayOn(roofRay.point + Vector2.down * 8f, (Vector2.down + Vector2.right).normalized, 200f);

                nearestWall = leftRay.distance < rightRay.distance ? leftRay.point + Vector2.right * 2f : rightRay.point + Vector2.left * 2f;
                farthestWall = leftRay.distance < rightRay.distance ? rightRay.point + Vector2.left * 2f : leftRay.point + Vector2.right * 2f;
                roofPoint = roofRay.point + Vector2.down * 2f;
                isRoofFlat = Mathnv.FastApproximately(roofRay.normal.x, 0f, 0.1f);
                roofToDownLeftPoint = roofToGroundLeft.point + (Vector2.up + Vector2.right) * 2f;
                roofToDownRightPoint = roofToGroundRight.point + (Vector2.up + Vector2.left) * 2f;

                if (originialMetadata.GetDatabaseKey() == "Plant Trap")
                {
                    transform.position = upwardShift;
                }
                else if (originialMetadata.GetDatabaseKey() == "Mushroom Turret")
                {
                    transform.position = upwardShift;
                }
                else if (originialMetadata.GetDatabaseKey() == "Plant Turret")
                {
                    transform.position = upwardShift;
                }
                else if (originialMetadata.GetDatabaseKey() == "Crawler")
                {
                    transform.position = upwardShift;
                }
                else if (originialMetadata.GetDatabaseKey() == "Climber")
                {
                    transform.position = upwardShift * 2f;
                }
                else if (originialMetadata.GetDatabaseKey() == "Mantis Flyer Child")
                {
                    transform.position = farthestWall;
                }
                else if (originialMetadata.GetDatabaseKey() == "Ruins Flying Sentry Javelin")
                {
                    transform.position = farthestWall;
                }
                else if (originialMetadata.GetDatabaseKey() == "Ceiling Dropper")
                {
                    transform.position = transform.position.ToVec2() + Vector2.down * 2f;
                }
                else if (originialMetadata.GetDatabaseKey() == "Baby Centipede")
                {
                    transform.position = upwardShift;
                }
                else if (originialMetadata.GetDatabaseKey() == "Mega Moss Charger")
                {
                    transform.position = upwardShift;
                }
                else if (originialMetadata.GetDatabaseKey() == "Pigeon")
                {
                    transform.position = upwardShift;
                }
                else if (originialMetadata.GetDatabaseKey() == "Big Centipede")
                {
                    transform.position = upwardShift * 2f;
                }

                if (gameObject.IsClimbing())
                {
                    if (!isRoofFlat)
                    {
                        RNG rng = new RNG();
                        bool heads = rng.CoinToss();
                        if (heads)
                        {
                            transform.position = roofToDownLeftPoint;
                            SpawnerExtensions.SetRotationToRayCollisionNormal(gameObject, roofToGroundLeft, false);
                        }
                        else
                        {
                            transform.position = roofToDownRightPoint;
                            SpawnerExtensions.SetRotationToRayCollisionNormal(gameObject, roofToGroundRight, false);
                        }
                    }
                }
            }

            if (doSpecialBossAggro)
            {
                //for now just don't
                //if (MRenderer != null)
                //    MRenderer.enabled = false;

                string currentScene = SpawnerExtensions.SceneName(gameObject);

                if (currentScene == "Crossroads_04")//giant fly
                {
                    if (MRenderer != null)
                        MRenderer.enabled = true;
                }
                else if (currentScene == "Fungus1_29")//mega moss charger
                {
                    transform.position = transform.position + Vector3.up * 5f;
                }
                else if (currentScene == "Ruins2_03_boss" || currentScene == "Ruins2_03")//black knights
                {
                }
                else if (currentScene == "Waterways_12_boss" || currentScene == "Waterways_12")//fluke mother
                {
                    if (MRenderer != null)
                        MRenderer.enabled = true;
                }
                else if (currentScene == "Fungus3_23_boss")//Mantis Traitor Lord
                {
                }
                else if (currentScene == "Fungus1_04_boss" || currentScene == "Fungus1_04")//hornet 1
                {
                    if (MRenderer != null)
                        MRenderer.enabled = true;
                }
                else if (currentScene == "Deepnest_East_Hornet_boss" || currentScene == "Deepnest_East_Hornet")//
                {
                    if (MRenderer != null)
                        MRenderer.enabled = true;
                }
                else if (currentScene == "Fungus1_20_v02")//giant buzzer (save zote)
                {
                    if (MRenderer != null)
                        MRenderer.enabled = true;
                }
                else if (currentScene == "Waterways_05_boss" || currentScene == "Waterways_05")//dung
                {
                    transform.position = transform.position + Vector3.up * 2f;
                }
                else if (currentScene == "Dream_04_White_Defender")//
                {
                }
                else if (currentScene == "Mines_18_boss" || currentScene == "Mines_18")//mega beam miner
                {
                    if (MRenderer != null)
                        MRenderer.enabled = true;
                }
                else if (currentScene == "Mines_32")//mega beam miner 2
                {
                    if (MRenderer != null)
                        MRenderer.enabled = true;
                }
                else if (currentScene == "Grimm_Main_Tent_boss" || currentScene == "Grimm_Main_Tent")//
                {
                }
                else if (currentScene == "Grimm_Nightmare")//
                {
                }
                else if (currentScene == "Abyss_19")//Infected Knight
                {
                    if (MRenderer != null)
                        MRenderer.enabled = true;
                }
                else if (currentScene == "Dream_03_Infected_Knight")//lost kin
                {
                }
                else if (currentScene == "Crossroads_10_boss" || currentScene == "Crossroads_10")//false knight
                {
                    placeGroundSpawnOnGround = false;
                    transform.position = new Vector3(29f, 38f);
                }
                else if (currentScene == "Dream_01_False_Knight")//dream fk
                {
                    transform.position = transform.position + Vector3.down * 15f;
                }
                else if (currentScene == "Ruins1_24_boss" && originialMetadata.GetDatabaseKey().Contains("Phase2"))//mage lord2
                {
                }
                else if (currentScene == "Ruins1_24_boss")//mage lord
                {
                }
                else if (currentScene == "Dream_02_Mage_Lord" && originialMetadata.GetDatabaseKey().Contains("Phase2"))//
                {
                }
                else if (currentScene == "Dream_02_Mage_Lord")//
                {
                }
                else if (currentScene == "Ruins2_11_boss" || currentScene == "Ruins2_11")//jar collector
                {
                    transform.position = transform.position + Vector3.down * 10f;
                }
                else if (currentScene == "Deepnest_32")//nosk / mimic spider
                {
                    if (MRenderer != null)
                        MRenderer.enabled = true;
                }
                else if (currentScene == "Fungus3_archive_02")//mega jellyfish
                {
                }
                else if (currentScene == "Hive_05")//hive knight
                {
                }
                else if (currentScene == "GG_Lurker")//
                {
                    if (MRenderer != null)
                        MRenderer.enabled = true;
                }
                else if (currentScene == "Dream_Mighty_Zote" && originialMetadata.GetDatabaseKey().Contains("Prince"))//zote
                {
                }
                else if (currentScene == "RestingGrounds_02_boss" || currentScene == "RestingGrounds_02")//xero
                {
                }
                else if (currentScene == "Fungus3_40_boss")//marmu
                {
                }
                else if (currentScene == "Deepnest_40")//galien
                {
                }
                else if (currentScene == "Cliffs_02_boss" || currentScene == "Cliffs_02")//gorb
                {
                }
                else if (currentScene == "Fungus1_35")//no eyes
                {
                }
                else if (currentScene == "Fungus2_32")//hu
                {
                }
                else if (currentScene == "Deepnest_East_10")//markoth
                {
                }
            }

        }

        protected override void OnSetSpawnPosition(GameObject objectThatWillBeReplaced)
        {
            base.OnSetSpawnPosition(objectThatWillBeReplaced);

            if (originialMetadata == null)
                return;

            string currentScene = SpawnerExtensions.SceneName(gameObject);
            var poob = gameObject.GetComponent<PreventOutOfBounds>();

            if (currentScene == "Crossroads_50")//blue lake
            {
                var newPos = transform.position.ToVec2();
                if (newPos.y < 28f)
                {
                    newPos.y = 28f;
                    if (poob != null)
                    {
                        poob.ForcePosition(newPos);
                    }
                    else
                    {
                        transform.position = newPos;
                    }
                }
            }


            if (doSpecialBossAggro)
            {
                if (currentScene == "Crossroads_04")//giant fly
                {
                    if (MaxHP < 90)
                        MaxHP = 90;
                }
                else if (currentScene == "Fungus1_29")//mega moss charger
                {
                }
                else if (currentScene == "Ruins2_03_boss" || currentScene == "Ruins2_03")//black knights
                {
                    if (this.Collider != null)
                        this.Collider.enabled = false;
                    EnemyHealthManager.IsInvincible = true;
                    hackyBlackKnightPreviousDamage = DamageDealt;
                    DamageDealt = 0;
                }
                else if (currentScene == "Waterways_12_boss" || currentScene == "Waterways_12")//fluke mother
                {
                }
                else if (currentScene == "Fungus3_23_boss")//Mantis Traitor Lord
                {
                }
                else if (currentScene == "Fungus1_04_boss" || currentScene == "Fungus1_04")//hornet 1
                {
                }
                else if (currentScene == "Deepnest_East_Hornet_boss" || currentScene == "Deepnest_East_Hornet")//
                {
                }
                else if (currentScene == "Fungus1_20_v02")//giant buzzer (save zote)
                {
                }
                else if (currentScene == "Waterways_05_boss" || currentScene == "Waterways_05")//dung
                {
                }
                else if (currentScene == "Dream_04_White_Defender")//
                {
                }
                else if (currentScene == "Mines_18_boss")//mega beam miner
                {
                }
                else if (currentScene == "Mines_32")//mega beam miner 2
                {
                }
                else if (currentScene == "Grimm_Main_Tent_boss")//
                {
                }
                else if (currentScene == "Grimm_Nightmare")//
                {
                }
                else if (currentScene == "Abyss_19")//Infected Knight
                {
                }
                else if (currentScene == "Dream_03_Infected_Knight")//lost kin
                {
                }
                else if (currentScene == "Crossroads_10_boss" || currentScene == "Crossroads_10")//false knight
                {
                    transform.position = new Vector3(29f, 38f);
                    if (poob != null)
                        poob.ForcePosition(new Vector3(29f, 38f));
                }
                else if (currentScene == "Dream_01_False_Knight")//dream fk
                {
                }
                else if (currentScene == "Ruins1_24_boss" && originialMetadata.GetDatabaseKey().Contains("Phase2"))//mage lord2
                {
                }
                else if (currentScene == "Ruins1_24_boss")//mage lord
                {
                }
                else if (currentScene == "Dream_02_Mage_Lord" && originialMetadata.GetDatabaseKey().Contains("Phase2"))//
                {
                }
                else if (currentScene == "Dream_02_Mage_Lord")//
                {
                }
                else if (currentScene == "Ruins2_11_boss")//jar collector
                {
                }
                else if (currentScene == "Deepnest_32")//nosk / mimic spider
                {
                }
                else if (currentScene == "Fungus3_archive_02")//mega jellyfish
                {
                }
                else if (currentScene == "Hive_05")//hive knight
                {
                }
                else if (currentScene == "GG_Lurker")//
                {
                }
                else if (currentScene == "Dream_Mighty_Zote" && originialMetadata.GetDatabaseKey().Contains("Prince"))//zote
                {
                }
                else if (currentScene == "RestingGrounds_02_boss")//xero
                {
                }
                else if (currentScene == "Fungus3_40_boss")//marmu
                {
                }
                else if (currentScene == "Deepnest_40")//galien
                {
                }
                else if (currentScene == "Cliffs_02_boss" || currentScene == "Cliffs_02")//gorb
                {
                }
                else if (currentScene == "Fungus1_35")//no eyes
                {
                }
                else if (currentScene == "Fungus2_32")//hu
                {
                }
                else if (currentScene == "Deepnest_East_10")//markoth
                {
                }
                Freeze();
            }
        }

        public bool awardGrimmFlameOnDeath;

        protected virtual void CheckHackyFixesOnSetup(GameObject objectThatWillBeReplaced)
        {
            if (IsInColo())
                return;

            if (gameObject.GetDatabaseKey() == "Ceiling Dropper")
            {
                var alertRange = gameObject.FindGameObjectInDirectChildren("Alert Range");
                var aBox = alertRange.GetComponent<BoxCollider2D>();
                aBox.size = new Vector2(aBox.size.x, 50f);
            }

            try
            {
                if (objectThatWillBeReplaced != null)
                {
                    var key = objectThatWillBeReplaced.GetDatabaseKey();

                    if ((key.Contains("Flamebearer Small")) &&
                        GameManager.instance.playerData.grimmChildLevel == 1 &&
                        GameManager.instance.playerData.equippedCharm_40 &&
                        (GameManager.instance.playerData.flamesCollected < GameManager.instance.playerData.flamesRequired))
                    {
                        SpawnerExtensions.AddParticleEffect_TorchFire(gameObject, 8);
                        SpawnerExtensions.AddParticleEffect_WhiteSoulEmissions(gameObject, Color.red);

                        awardGrimmFlameOnDeath = true;

                        gameObject.GetComponents<PlayMakerFSM>().ToList().ForEach(x => x.enabled = true);

                        var spawnPosition = GameObjectExtensions.EnumerateRootObjects(true).FirstOrDefault(x => x.name.Contains("Flamebearer Spawn")).transform.position;

                        var poob = gameObject.GetComponent<PreventOutOfBounds>();
                        if (poob != null)
                        {
                            poob.ForcePosition(spawnPosition);
                        }
                        else
                        {
                            gameObject.transform.position = spawnPosition;
                        }
                    }

                    else if ((key.Contains("Flamebearer Med")) &&
                        GameManager.instance.playerData.grimmChildLevel == 2 &&
                        GameManager.instance.playerData.equippedCharm_40 &&
                        (GameManager.instance.playerData.flamesCollected < GameManager.instance.playerData.flamesRequired))
                    {
                        SpawnerExtensions.AddParticleEffect_TorchFire(gameObject, 8);
                        SpawnerExtensions.AddParticleEffect_WhiteSoulEmissions(gameObject, Color.red);

                        awardGrimmFlameOnDeath = true;

                        gameObject.GetComponents<PlayMakerFSM>().ToList().ForEach(x => x.enabled = true);

                        var spawnPosition = GameObjectExtensions.EnumerateRootObjects(true).FirstOrDefault(x => x.name.Contains("Flamebearer Spawn")).transform.position;

                        var poob = gameObject.GetComponent<PreventOutOfBounds>();
                        if (poob != null)
                        {
                            poob.ForcePosition(spawnPosition);
                        }
                        else
                        {
                            gameObject.transform.position = spawnPosition;
                        }
                    }

                    else if ((key.Contains("Flamebearer Large")) &&
                        GameManager.instance.playerData.grimmChildLevel == 3 &&
                        GameManager.instance.playerData.equippedCharm_40 &&
                        (GameManager.instance.playerData.flamesCollected < GameManager.instance.playerData.flamesRequired))
                    {
                        SpawnerExtensions.AddParticleEffect_TorchFire(gameObject, 8);
                        SpawnerExtensions.AddParticleEffect_WhiteSoulEmissions(gameObject, Color.red);

                        awardGrimmFlameOnDeath = true;

                        gameObject.GetComponents<PlayMakerFSM>().ToList().ForEach(x => x.enabled = true);

                        var spawnPosition = GameObjectExtensions.EnumerateRootObjects(true).FirstOrDefault(x => x.name.Contains("Flamebearer Spawn")).transform.position;

                        var poob = gameObject.GetComponent<PreventOutOfBounds>();
                        if (poob != null)
                        {
                            poob.ForcePosition(spawnPosition);
                        }
                        else
                        {
                            gameObject.transform.position = spawnPosition;
                        }
                    }


                    else if (key.Contains("Jelly Egg Bomb"))
                    {
                        SpawnerExtensions.AddParticleEffect_TorchFire(gameObject, 4);
                    }
                }
            }
            catch (Exception e) { } //nom any errors in ehre for now TODO: actually handle and check this stuff in next release
        }

        IEnumerator ForceSuperBossHack(SuperSpitterControl ssc)
        {
            yield return null;
            while (gameObject != null && gameObject.activeInHierarchy)
            {
                if (gameObject.GetDatabaseKey() == "Super Spitter")
                {
                    if (ssc != null && !ssc.isSuperBoss)
                    {
                        ssc.MakeSuperBoss();
                    }
                }
                yield return null;
            }
        }

        protected virtual void CheckHackyBossSetup(GameObject objectThatWillBeReplaced)
        {
            string currentScene = SpawnerExtensions.SceneName(gameObject);
            bool isBoss = SpawnerExtensions.IsBoss(objectThatWillBeReplaced.GetDatabaseKey());
            if (isBoss)
            {
                doSpecialBossAggro = true;

                if (gameObject.GetDatabaseKey() == "Super Spitter")
                {
                    var ssc = gameObject.GetComponent<SuperSpitterControl>();
                    if (ssc != null)
                    {
                        ssc.MakeSuperBoss();
                        StartCoroutine(ForceSuperBossHack(ssc));
                    }
                }

                //make the enemy into its boss type
                {
                    var bossType = gameObject.GetComponent<ZoteBossControl>();
                    if (bossType != null)
                    {
                        PhysicsBody.gravityScale = 0.5f;
                    }
                }
                {
                    var bossType = gameObject.GetComponent<MageKnightControl>();
                    if (bossType != null)
                    {
                        bossType.canFakeout = true;
                    }
                }
                {
                    var bossType = gameObject.GetComponent<GorgeousHuskControl>();
                    if (bossType != null)
                    {
                        bossType.MakeSuperHusk();
                    }
                }
                {
                    var bossType = gameObject.GetComponent<RoyalGaurdControl>();
                    if (bossType != null)
                    {
                        bossType.MakeElite();
                    }
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////
                if (currentScene == "Crossroads_09")//mawlek
                {
                    specialAggroRange = 1f;
                }
                else
                if (currentScene == "Crossroads_04")//giant fly
                {
                    specialAggroRange = 5f;
                }
                else if (currentScene == "Fungus1_29")//mega moss charger
                {
                    specialAggroRange = 20f;
                }
                else if (currentScene == "Ruins2_03_boss" || currentScene == "Ruins2_03")//black knights
                {
                    specialAggroRange = 4f; //TODO: start the encounter for all after one is triggered
                    hackyBlackKnightIndex = BattleManager.BlackKnightCounter;
                    BattleManager.BlackKnightCounter = BattleManager.BlackKnightCounter + 1;
                }
                else if (currentScene == "Waterways_12_boss" || currentScene == "Waterways_12")//fluke mother
                {
                    specialAggroRange = 10f;
                }
                else if (currentScene == "Fungus3_23_boss" || currentScene == "Fungus3_23")//Mantis Traitor Lord
                {
                    specialAggroRange = 5f;
                }
                else if (currentScene == "Fungus1_04_boss" || currentScene == "Fungus1_04")//hornet 1
                {
                    specialAggroRange = 8f;
                }
                else if (currentScene == "Deepnest_East_Hornet_boss" || currentScene == "Deepnest_East_Hornet")//
                {
                    specialAggroRange = 15f;
                }
                else if (currentScene == "Fungus1_20_v02")//giant buzzer (save zote)
                {
                    specialAggroRange = 8f;
                }
                else if (currentScene == "Waterways_05_boss" || currentScene == "Waterways_05")//dung
                {
                    specialAggroRange = 10f;
                }
                else if (currentScene == "Dream_04_White_Defender")//
                {
                    specialAggroRange = 5f;
                }
                else if (currentScene == "Mines_18_boss" || currentScene == "Mines_18")//mega beam miner
                {
                    specialAggroRange = 5f;
                }
                else if (currentScene == "Mines_32")//mega beam miner 2
                {
                    specialAggroRange = 10f;
                }
                else if (currentScene == "Grimm_Main_Tent_boss" || currentScene == "Grimm_Main_Tent")//
                {
                    specialAggroRange = 10f;
                }
                else if (currentScene == "Grimm_Nightmare")//
                {
                    specialAggroRange = 10f;
                }
                else if (currentScene == "Abyss_19")//Infected Knight
                {
                    specialAggroRange = 5f;
                }
                else if (currentScene == "Dream_03_Infected_Knight")//lost kin
                {
                    specialAggroRange = 10f;
                }
                else if (currentScene == "Crossroads_10_boss" || currentScene == "Crossroads_10")//false knight
                {
                    specialAggroRange = 18f;
                }
                else if (currentScene == "Dream_01_False_Knight")//dream fk
                {
                    specialAggroRange = 12f;
                }
                else if ((currentScene == "Ruins1_24" || currentScene == "Ruins1_24_boss") && objectThatWillBeReplaced.GetDatabaseKey().Contains("Phase2"))//mage lord2
                {
                    specialAggroRange = 10f;
                }
                else if (currentScene == "Ruins1_24" || currentScene == "Ruins1_24_boss")//mage lord
                {
                    specialAggroRange = 10f;
                }
                else if (currentScene == "Dream_02_Mage_Lord" && objectThatWillBeReplaced.GetDatabaseKey().Contains("Phase2"))//
                {
                    specialAggroRange = 10f;
                }
                else if (currentScene == "Dream_02_Mage_Lord")//
                {
                    specialAggroRange = 10f;
                }
                else if (currentScene == "Ruins2_11_boss")//jar collector
                {
                    specialAggroRange = 10f;
                }
                else if (currentScene == "Deepnest_32")//nosk / mimic spider
                {
                    specialAggroRange = 6f;
                }
                else if (currentScene == "Fungus3_archive_02")//mega jellyfish
                {
                    specialAggroRange = 10f;
                }
                else if (currentScene == "Hive_05")//hive knight
                {
                    specialAggroRange = 10f;
                }
                else if (currentScene == "GG_Lurker")//
                {
                    specialAggroRange = 10f;
                }
                else if (currentScene == "Dream_Mighty_Zote" && objectThatWillBeReplaced.GetDatabaseKey().Contains("Prince"))//zote
                {
                    specialAggroRange = 20f;
                }
                else if (currentScene == "RestingGrounds_02_boss")//xero
                {
                    specialAggroRange = 10f;
                }
                else if (currentScene == "Fungus3_40_boss")//marmu
                {
                    specialAggroRange = 10f;
                }
                else if (currentScene == "Deepnest_40")//galien
                {
                    specialAggroRange = 10f;
                }
                else if (currentScene == "Cliffs_02_boss" || currentScene == "Cliffs_02")//gorb
                {
                    specialAggroRange = 10f;
                }
                else if (currentScene == "Fungus1_35")//no eyes
                {
                    specialAggroRange = 10f;
                }
                else if (currentScene == "Fungus2_32")//hu
                {
                    specialAggroRange = 10f;
                }
                else if (currentScene == "Deepnest_East_10")//markoth
                {
                    specialAggroRange = 10f;
                }
                ////////////////////////////////////////////////////////////////////////////////////////////////
            }
        }

        protected virtual void CheckHackyOnAggroFixes()
        {
            if (originialMetadata == null)
                return;

            if (doSpecialBossAggro)
            {
                if (MRenderer != null)
                    MRenderer.enabled = true;
                UnFreeze();
            }

            string currentScene = SpawnerExtensions.SceneName(gameObject);
            bool isBoss = SpawnerExtensions.IsBoss(originialMetadata.GetDatabaseKey());
            if (isBoss)
            {
                ////////////////////////////////////////////////////////////////////////////////////////////////
                if (currentScene == "Crossroads_09")//mawlek
                {
                    PlayMakerFSM.BroadcastEvent("START");
                }
                else
                if (currentScene == "Crossroads_04")//giant fly
                {
                    BattleManager.StateMachine.Value.FSM.SendEvent("START");
                }
                else if (currentScene == "Fungus1_29")//mega moss charger
                {
                    //NO FSM for the arena
                }
                else if (currentScene == "Ruins2_03_boss")//black knights
                {
                    BattleManager.AggroAllBossesNow = true;
                    if (this.Collider != null)
                        this.Collider.enabled = false;
                    EnemyHealthManager.IsInvincible = true;
                    hackyBlackKnightPreviousDamage = DamageDealt;
                    Freeze();
                    GameManager.instance.StartCoroutine(BlackKnightUnfreezeAfterTime(4f));
                    BattleManager.StateMachine.Value.FSM.SendEvent("BATTLE START");
                }
                else if (currentScene == "Waterways_12_boss")//fluke mother
                {
                    //NO FSM for the arena
                }
                else if (currentScene == "Fungus3_23_boss")//Mantis Traitor Lord
                {
                    PlayMakerFSM.BroadcastEvent("CLOTH ENTER");//just in case
                }
                else if (currentScene == "Fungus1_04_boss" || currentScene == "Fungus1_04")//hornet 1
                {
                    BattleStateMachine.CloseGates();
                }
                else if (currentScene == "Deepnest_East_Hornet_boss" || currentScene == "Deepnest_East_Hornet")//
                {
                    BattleStateMachine.CloseGates();
                    PlayMakerFSM.BroadcastEvent("START WINDY");
                }
                else if (currentScene == "Fungus1_20_v02")//giant buzzer (save zote)
                {
                    GameManager.instance.playerData.zoteRescuedBuzzer = true;
                }
                else if (currentScene == "Waterways_05" || currentScene == "Waterways_05_boss")//dung
                {
                    BattleManager.StateMachine.Value.FSM.SendEvent("ENTER");
                    BattleStateMachine.CloseGates();
                }
                else if (currentScene == "Dream_04_White_Defender")//
                {
                    BattleManager.StateMachine.Value.FSM.SendEvent("ENTER");
                }
                else if (currentScene == "Mines_18_boss" || currentScene == "Mines_18")//mega beam miner
                {
                    BattleStateMachine.CloseGates();
                }
                else if (currentScene == "Mines_32")//mega beam miner 2
                {
                    BattleStateMachine.CloseGates();
                }
                else if (currentScene == "Grimm_Main_Tent_boss" || currentScene == "Grimm_Main_Tent")//
                {
                    BattleStateMachine.CloseGates();
                }
                else if (currentScene == "Grimm_Nightmare")//
                {
                    BattleStateMachine.CloseGates();
                }
                else if (currentScene == "Abyss_19")//Infected Knight
                {
                    BattleStateMachine.CloseGates(true);
                    PlayMakerFSM.BroadcastEvent("START WINDY");
                }
                else if (currentScene == "Dream_03_Infected_Knight")//lost kin
                {
                    BattleStateMachine.CloseGates();
                }
                else if (currentScene == "Crossroads_10_boss" || currentScene == "Crossroads_10")//false knight
                {
                    BattleStateMachine.CloseGates(true);
                }
                else if (currentScene == "Dream_01_False_Knight")//dream fk
                {
                    BattleStateMachine.CloseGates();
                }
                else if ((currentScene == "Ruins1_24" || currentScene == "Ruins1_24_boss") && originialMetadata.GetDatabaseKey().Contains("Phase2"))//mage lord2
                {
                    PlayMakerFSM.BroadcastEvent("CANCEL SHAKE");
                }
                else if (currentScene == "Ruins1_24" || currentScene == "Ruins1_24_boss")//mage lord
                {
                    BattleStateMachine.CloseGates(true);
                    PlayMakerFSM.BroadcastEvent("CANCEL SHAKE");
                }
                else if (currentScene == "Dream_02_Mage_Lord" && originialMetadata.GetDatabaseKey().Contains("Phase2"))//
                {
                }
                else if (currentScene == "Dream_02_Mage_Lord")//
                {
                    BattleStateMachine.CloseGates(true);
                }
                else if (currentScene == "Ruins2_11_boss" || currentScene == "Ruins2_11")//jar collector
                {
                    BattleManager.StateMachine.Value.FSM.SendEvent("HIT");
                    BattleStateMachine.CloseGates(true);
                }
                else if (currentScene == "Deepnest_32")//nosk / mimic spider
                {
                    BattleStateMachine.CloseGates(true);
                }
                else if (currentScene == "Fungus3_archive_02" || currentScene == "Fungus3_archive_02_boss")//mega jellyfish
                {
                    BattleManager.StateMachine.Value.FSM.SendEvent("ENTER");
                    BattleStateMachine.CloseGates(true);
                }
                else if (currentScene == "Hive_05")//hive knight
                {
                    BattleManager.StateMachine.Value.FSM.SendEvent("WAKE");
                    BattleStateMachine.CloseGates(true);
                }
                else if (currentScene == "GG_Lurker")//
                {
                    BattleManager.StateMachine.Value.FSM.SendEvent("START");
                    BattleStateMachine.CloseGates(true);
                }
                else if (currentScene == "Dream_Mighty_Zote" && originialMetadata.GetDatabaseKey().Contains("Prince"))//zote
                {
                    BattleManager.StateMachine.Value.FSM.SendEvent("ENTER");
                    BattleManager.StateMachine.Value.FSM.SendEvent("ZOTE APPEAR");
                    BattleStateMachine.CloseGates(true);
                }
                else if (currentScene == "RestingGrounds_02_boss" || currentScene == "RestingGrounds_02")//xero
                {
                    BattleStateMachine.CloseGates(true);
                }
                else if (currentScene == "Fungus3_40" || currentScene == "Fungus3_40_boss")//marmu
                {
                    BattleStateMachine.CloseGates(true);
                }
                else if (currentScene == "Deepnest_40")//galien
                {
                    BattleStateMachine.CloseGates(true);
                }
                else if (currentScene == "Cliffs_02_boss" || currentScene == "Cliffs_02")//gorb
                {
                    BattleStateMachine.CloseGates(true);
                }
                else if (currentScene == "Fungus1_35")//no eyes
                {
                    BattleStateMachine.CloseGates(true);
                }
                else if (currentScene == "Fungus2_32")//hu
                {
                    BattleStateMachine.CloseGates(true);
                }
                else if (currentScene == "Deepnest_East_10")//markoth
                {
                    BattleStateMachine.CloseGates(true);
                }
                ////////////////////////////////////////////////////////////////////////////////////////////////
            }
        }

        protected virtual void CheckHackySceneFixesOnDeath()
        {
            if (originialMetadata == null)
                return;

            string currentScene = SpawnerExtensions.SceneName(gameObject);
            if (string.IsNullOrEmpty(currentScene))
                return;

            if (IsInColo())
                return;

            if (originialMetadata != null && originialMetadata.ObjectName.Contains("Jelly Egg Bomb"))
            {
                SpawnerExtensions.SpawnExplosionAt(pos2d);
            }

            if (awardGrimmFlameOnDeath)
            {
                var child = GameObject.FindGameObjectWithTag("Grimmchild");
                if (child != null)
                {
                    var sf = child.GetComponentInChildren<SpriteFlash>(true);
                    if (sf != null)
                    {
                        sf.FlashGrimmflame();
                        GameManager.instance.playerData.flamesCollected += 1;
                        GameManager.instance.AddToFlameList();
                        SpawnerExtensions.SpawnEntityAt("particle_flame_blast", child.transform.position, null, true, false);
                    }
                }
            }

            bool isBoss = SpawnerExtensions.IsBoss(originialMetadata.GetDatabaseKey());
            if (isBoss)
            {
                ApplyBossDeathFix(currentScene);
            }
        }

        protected virtual void ApplyBossDeathFix(string currentScene)
        {
            bool notifyBattleEnded = true;
            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (currentScene == "Crossroads_09")//mawlek
            {
                BattleManager.StateMachine.Value.FSM.SendEvent("BATTLE END");
                PlayMakerFSM.BroadcastEvent("CHARM DROP");
                GameManager.instance.playerData.mawlekDefeated = true;
            }
            else
            if (currentScene == "Crossroads_04")//giant fly
            {
                BattleManager.StateMachine.Value.FSM.SendEvent("END");
                GameManager.instance.playerData.giantFlyDefeated = true;
                GameManager.instance.BroadcastFSMEventAfterTime("END", 10f);
            }
            else if (currentScene == "Fungus1_29")//mega moss charger
            {
                //BattleManager.StateMachine.Value.FSM.SendEvent("END");
                GameManager.instance.playerData.megaMossChargerDefeated = true; //TODO: remove this from the corpse
            }
            else if (currentScene == "Ruins2_03_boss")//black knights
            {
                BattleManager.BlackKnightArenaKillCounter += 1;
                BattleManager.BlackKnightsActiveCounter -= 1;
                //var currentState = BattleManager.StateMachine.Value.FSM.ActiveStateName;
                BattleManager.StateMachine.Value.FSM.SendEvent("NEXT"); //TODO: check if it should skip the earlier states based on chandilier
                GameManager.instance.playerData.killsBlackKnight = BattleManager.BlackKnightArenaKillCounter;

                if (GameManager.instance.playerData.watcherChandelier && BattleManager.BlackKnightArenaKillCounter >= 3)
                {
                    BattleManager.StateMachine.Value.ForceBattleEnd();
                    BattleManager.BlackKnightArenaDoneEarly = true;
                    GameManager.instance.playerData.killsBlackKnight = 6;
                    GameManager.instance.StoryRecord_defeated("Watcher Knights");
                    BattleStateMachine.OpenGates(false);
                }
                else if (BattleManager.BlackKnightArenaKillCounter >= 6)
                {
                    BattleManager.StateMachine.Value.ForceBattleEnd();
                    BattleStateMachine.OpenGates(false);//just force open gates to be safe
                }
            }
            else if (currentScene == "Waterways_12_boss" || currentScene == "Waterways_12")//fluke mother
            {
                //BattleManager.StateMachine.Value.FSM.SendEvent("END");
                PlayMakerFSM.BroadcastEvent("CHARM DROP");
                GameManager.instance.playerData.flukeMotherDefeated = true; //TODO: take the setting of this out of the boss's corpse
            }
            else if (currentScene == "Fungus3_23_boss")//Mantis Traitor Lord
            {
                var currentState = BattleManager.StateMachine.Value.FSM.ActiveStateName;
                if (currentState != "Wave 3")
                {
                    BattleManager.StateMachine.Value.FSM.SetState("Wave 3");
                }

                BattleManager.StateMachine.Value.FSM.SendEvent("BOSS DEATH");
                GameManager.instance.playerData.killedTraitorLord = true;
                GameManager.instance.playerData.newDataTraitorLord = true;
                GameManager.instance.playerData.killsTraitorLord = 0;

                var cloth = GameObject.Find("Cloth Fighter");
                if (cloth != null)
                {
                    var clothControl = cloth.LocateMyFSM("Control");
                    clothControl.SetState("Land");
                }
                PlayMakerFSM.BroadcastEvent("CLOTH DESTROY");
                GameManager.instance.playerData.clothKilled = true;
                //{
                //    var currentState = BattleManager.StateMachine.Value.FSM.ActiveStateName;
                //    if (currentState == "Init")
                //        BattleManager.StateMachine.Value.FSM.SendEvent("HIT");
                //    else if (currentState == "Wave 1")
                //        BattleManager.StateMachine.Value.FSM.SendEvent("END");
                //    else if (currentState == "Wave 2")
                //        BattleManager.StateMachine.Value.FSM.SendEvent("END");
                //    else if (currentState == "Wave 3")
                //    {
                //        BattleManager.StateMachine.Value.FSM.SendEvent("BOSS DEATH");
                //        GameManager.instance.playerData.killedTraitorLord = true;
                //    }
                //}
            }
            else if (currentScene == "Fungus1_04_boss" || currentScene == "Fungus1_04")//hornet 1
            {
                PlayMakerFSM.BroadcastEvent("HORNET KILLED");

                BattleManager.SilenceMusic();

                {
                    var cc = GameObject.Find("Cloak Corpse");
                    if (cc != null)
                    {
                        var fsm = cc.LocateMyFSM("Control");
                        fsm.SendEvent("HORNET LEAVE");
                    }
                }

                {
                    var cc = GameObject.Find("Dreamer Scene 1");
                    if (cc != null)
                    {
                        var fsm = cc.LocateMyFSM("Control");
                        cc.GetComponent<Collider2D>().enabled = true;
                    }
                }

                GameManager.instance.StoryRecord_defeated("Hornet in Greenpath");
                GameManager.instance.playerData.hornet1Defeated = true;
                BattleStateMachine.OpenGates(false);
            }
            else if (currentScene == "Deepnest_East_Hornet_boss" || currentScene == "Deepnest_East_Hornet")//
            {
                GameManager.instance.playerData.hornetOutskirtsDefeated = true;
                BattleManager.StateMachine.Value.FSM.SendEvent("HORNET LEAVE");
                BattleStateMachine.OpenGates(false);//just force open gates, the fsm for this is a bit weird
            }
            else if (currentScene == "Fungus1_20_v02")//giant buzzer (save zote)
            {
                GameManager.instance.playerData.zoteDead = false;
                GameManager.instance.playerData.zoteRescuedBuzzer = true;
            }
            else if (currentScene == "Waterways_05_boss" || currentScene == "Waterways_05")//dung
            {
                BattleManager.StateMachine.Value.FSM.SendEvent("BATTLE END");
                PlayMakerFSM.BroadcastEvent("CHARM DROP");
                GameManager.instance.playerData.defeatedDungDefender = true;
                BattleStateMachine.OpenGates(false);//just in case, send this, the fsm for this is a bit weird
            }
            else if (currentScene == "Dream_04_White_Defender")//
            {
                GameManager.instance.playerData.whiteDefenderDefeated = true;
                GameManager.instance.playerData.whiteDefenderDefeats += 1;
                PlayMakerFSM.BroadcastEvent("FRIENDS OUT");
                SpawnerExtensions.DreamTransitionTo("Waterways_15", "door_dreamReturn");
                if (GameManager.instance.playerData.whiteDefenderDefeats >= 5)
                    GameManager.instance.playerData.dungDefenderAwoken = true;
            }
            else if (currentScene == "Mines_18_boss" || currentScene == "Mines_18")//mega beam miner
            {
                GameManager.instance.playerData.killedMegaBeamMiner = true;
                GameManager.instance.playerData.killsMegaBeamMiner = 1;
                BattleManager.StateMachine.Value.FSM.SendEvent("BATTLE END");
                BattleStateMachine.OpenGates(false);
                BattleStateMachine.UnlockCameras();
            }
            else if (currentScene == "Mines_32")//mega beam miner 2
            {
                PlayMakerFSM.BroadcastEvent("KILLED");
                BattleManager.StateMachine.Value.FSM.SetState("Wait");
                BattleStateMachine.OpenGates(false);
                BattleStateMachine.UnlockCameras();
                GameManager.instance.playerData.killsMegaBeamMiner = 0;
            }
            else if (currentScene == "Grimm_Main_Tent_boss" || currentScene == "Grimm_Main_Tent")//
            {
                PlayMakerFSM.BroadcastEvent("GRIMM DEFEATED");
                GameManager.instance.playerData.killedGrimm = true;
            }
            else if (currentScene == "Grimm_Nightmare")//
            {
                BattleManager.StateMachine.Value.FSM.SendEvent("GRIMM DEFEATED");
                GameManager.instance.playerData.defeatedNightmareGrimm = true;
                GameManager.instance.playerData.killedNightmareGrimm = true;
                GameManager.instance.playerData.newDataNightmareGrimm = true;
                GameManager.instance.playerData.troupeInTown = false;
                GameManager.instance.playerData.killsNightmareGrimm = 1;
                GameManager.instance.playerData.grimmChildLevel = 4;
            }
            else if (currentScene == "Abyss_19")//Infected Knight
            {
                BattleManager.StateMachine.Value.FSM.SendEvent("IK GATE OPEN");
                PlayMakerFSM.BroadcastEvent("IK GATE OPEN");
                GameManager.instance.playerData.killedInfectedKnight = true;
                BattleStateMachine.OpenGates(false);
            }
            else if (currentScene == "Dream_03_Infected_Knight")//lost kin
            {
                GameManager.instance.StoryRecord_defeated("Lost Kin");
                var battleScene = GameObject.FindGameObjectWithTag("Battle Scene");
                if (battleScene != null)
                {
                    var control = battleScene.LocateMyFSM("Battle Control");
                    if (control != null)
                        control.FsmVariables.GetFsmBool("Activated").Value = true;
                }

                GameManager.instance.playerData.infectedKnightDreamDefeated = true;
                var returnScene = GameManager.instance.playerData.dreamReturnScene;
                SpawnerExtensions.DreamTransitionTo(returnScene, "door_dreamReturn", 1f);
            }
            else if (currentScene == "Crossroads_10_boss" || currentScene == "Crossroads_10")//false knight
            {
                GameManager.instance.playerData.falseKnightDefeated = true;
                GameManager.instance.playerData.falseKnightFirstPlop = true;
                GameManager.instance.playerData.falseKnightWallBroken = true;
                GameManager.instance.playerData.killedFalseKnight = true;
                GameManager.instance.playerData.newDataFalseKnight = true;
                GameManager.instance.playerData.killsFalseKnight = 0;
                PlayMakerFSM.BroadcastEvent("FK DEATH");
                GameObject.Find("FK Floor").LocateMyFSM("Floor Control").SetState("Break");
                BattleManager.StateMachine.Value.FSM.SendEvent("END");
                BattleStateMachine.OpenGates(false);
            }
            else if (currentScene == "Dream_01_False_Knight")//dream fk
            {
                GameManager.instance.playerData.falseKnightDreamDefeated = true;
                BattleManager.StateMachine.Value.FSM.SendEvent("END");

                var returnScene = GameManager.instance.playerData.dreamReturnScene;
                SpawnerExtensions.DreamTransitionTo(returnScene, "door_dreamReturn", 1f);
            }
            else if ((currentScene == "Ruins1_24" || currentScene == "Ruins1_24_boss") && originialMetadata.GetDatabaseKey().Contains("Phase2"))//mage lord2
            {
                BattleStateMachine.OpenGates(false);
                GameManager.instance.playerData.mageLordDefeated = true;
                GameManager.instance.StoryRecord_defeated("Soul Master");
                PlayMakerFSM.BroadcastEvent("DISSIPATE");
                PlayMakerFSM.BroadcastEvent("QUAKE PICKUP START");
            }
            else if (currentScene == "Ruins1_24" || currentScene == "Ruins1_24_boss")//mage lord
            {
                PlayMakerFSM.BroadcastEvent("QUAKE FAKE APPEAR");
            }
            else if (currentScene == "Dream_02_Mage_Lord" && originialMetadata.GetDatabaseKey().Contains("Phase2"))//
            {
                GameManager.instance.StoryRecord_defeated("Soul Tyrant");
                PlayMakerFSM.BroadcastEvent("DISSIPATE");
                BattleStateMachine.OpenGates(false);
                GameManager.instance.playerData.mageLordDreamDefeated = true;

                var returnScene = GameManager.instance.playerData.dreamReturnScene;
                SpawnerExtensions.DreamTransitionTo(returnScene, "door_dreamReturn", 1f);
            }
            else if (currentScene == "Dream_02_Mage_Lord")//
            {
                PlayMakerFSM.BroadcastEvent("QUAKE FAKE APPEAR");
                PlayMakerFSM.BroadcastEvent("MAGE WINDOW BREAK");

                GameManager.instance.BroadcastFSMEventHeroHasMovedUnder("PHASE 2", 23f);

                PlayMakerFSM.BroadcastEvent("PHASE 2");
            }
            else if (currentScene == "Ruins2_11_boss" || currentScene == "Ruins2_11")//jar collector
            {
                BattleManager.StateMachine.Value.FSM.SendEvent("BATTLE END");
                GameManager.instance.playerData.killedJarCollector = true;
                BattleStateMachine.OpenGates(false);
            }
            else if (currentScene == "Deepnest_32")//nosk / mimic spider
            {
                BattleManager.StateMachine.Value.FSM.SendEvent("KILLED");
                GameManager.instance.playerData.killedMimicSpider = true;
                BattleStateMachine.OpenGates(false);
            }
            else if (currentScene == "Fungus3_archive_02" || currentScene == "Fungus3_archive_02_boss")//mega jellyfish
            {
                BattleManager.StateMachine.Value.FSM.SendEvent("BOSS DEATH");
                GameManager.instance.playerData.defeatedMegaJelly = true;
                BattleStateMachine.OpenGates(false);
            }
            else if (currentScene == "Hive_05")//hive knight
            {
                BattleManager.StateMachine.Value.FSM.SendEvent("BATTLE END");
                GameManager.instance.playerData.killedHiveKnight = true;
                BattleStateMachine.OpenGates(false);
            }
            else if (currentScene == "GG_Lurker")//
            {
                BattleManager.StateMachine.Value.FSM.SendEvent("SHINY PICKED UP");
                GameManager.instance.playerData.killedPaleLurker = true;
                BattleStateMachine.OpenGates(false);
            }
            else if (currentScene == "Dream_Mighty_Zote" && originialMetadata.GetDatabaseKey().Contains("Prince"))//zote
            {
                GameManager.instance.playerData.greyPrinceDefeated = true;

                var returnScene = GameManager.instance.playerData.dreamReturnScene;
                SpawnerExtensions.DreamTransitionTo(returnScene, "door_dreamReturn", 1f);
            }
            else if (currentScene == "RestingGrounds_02_boss" || currentScene == "RestingGrounds_02")//xero
            {
                GameManager.instance.playerData.xeroDefeated = 1;
                PlayMakerFSM.BroadcastEvent("GHOST DEAD");
                PlayMakerFSM.BroadcastEvent("GHOST DEFEAT");
                BattleStateMachine.OpenGates(false);
            }
            else if (currentScene == "Fungus3_40" || currentScene == "Fungus3_40_boss")//marmu
            {
                if (originialMetadata != null && originialMetadata.ObjectName.Contains("Marmu"))
                {
                    GameManager.instance.playerData.killedGhostMarmu = true;
                    PlayMakerFSM.BroadcastEvent("GHOST DEAD");
                    PlayMakerFSM.BroadcastEvent("GHOST DEFEAT");
                    BattleStateMachine.OpenGates(false);
                }
            }
            else if (currentScene == "Deepnest_40")//galien
            {
                GameManager.instance.playerData.galienDefeated = 1;
                PlayMakerFSM.BroadcastEvent("GHOST DEAD");
                PlayMakerFSM.BroadcastEvent("GHOST DEFEAT");
                BattleStateMachine.OpenGates(false);
            }
            else if (currentScene == "Cliffs_02_boss" || currentScene == "Cliffs_02")//gorb
            {
                GameManager.instance.playerData.aladarSlugDefeated = 1;
                PlayMakerFSM.BroadcastEvent("GHOST DEAD");
                PlayMakerFSM.BroadcastEvent("GHOST DEFEAT");
                BattleStateMachine.OpenGates(false);
            }
            else if (currentScene == "Fungus1_35")//no eyes
            {
                GameManager.instance.playerData.noEyesDefeated = 1;
                PlayMakerFSM.BroadcastEvent("GHOST DEAD");
                PlayMakerFSM.BroadcastEvent("GHOST DEFEAT");
                BattleStateMachine.OpenGates(false);
            }
            else if (currentScene == "Fungus2_32")//hu
            {
                GameManager.instance.playerData.elderHuDefeated = 1;
                PlayMakerFSM.BroadcastEvent("GHOST DEAD");
                PlayMakerFSM.BroadcastEvent("GHOST DEFEAT");
                BattleStateMachine.OpenGates(false);
            }
            else if (currentScene == "Deepnest_East_10")//markoth
            {
                GameManager.instance.playerData.markothDefeated = 1;
                PlayMakerFSM.BroadcastEvent("GHOST DEAD");
                PlayMakerFSM.BroadcastEvent("GHOST DEFEAT");
                PlayMakerFSM.BroadcastEvent("DREAM AREA DISABLE");
                BattleStateMachine.OpenGates(false);
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////

            if (notifyBattleEnded && BattleManager.StateMachine != null && BattleManager.StateMachine.Value != null)
                BattleManager.StateMachine.Value.ForceBattleEnd();
        }

        protected virtual void Freeze()
        {
            var pl = gameObject.GetOrAddComponent<PositionLocker>();
            pl.positionLock = transform.position;
        }

        protected virtual void UnFreeze()
        {
            var locker = gameObject.GetComponent<PositionLocker>();
            if (locker != null)
            {
                GameObject.Destroy(locker);
                PhysicsBody.velocity = Vector2.zero;//reset the velocity
            }
        }

        public int hackyBlackKnightPreviousDamage = -1;
        public int hackyBlackKnightIndex = -1;
        IEnumerator BlackKnightUnfreezeAfterTime(float time)
        {
            yield return new WaitUntil(() => BattleManager.BlackKnightsActiveCounter < 2);

            if (BattleManager.BlackKnightArenaDoneEarly
               || (BattleManager.BlackKnightArenaKillCounter == 2 && BattleManager.BlackKnightsActiveCounter >= 1))
            {
                SpawnerExtensions.DestroyObject(gameObject);
                yield break;
            }

            BattleManager.BlackKnightsActiveCounter += 1;

            var b1 = SpawnerExtensions.SpawnEntityAt("Bugs In 1", transform.position, null, true, false).GetComponent<ParticleSystem>();
            var b2 = SpawnerExtensions.SpawnEntityAt("Bugs In 2", transform.position, null, true, false).GetComponent<ParticleSystem>();
            var b3 = SpawnerExtensions.SpawnEntityAt("Entry Steam", transform.position, null, true, false).GetComponent<ParticleSystem>();

            b1.Emit(0);
            b2.Emit(0);
            b3.Emit(0);

            yield return new WaitForSeconds(time);

            b1.Stop();
            b2.Stop();
            b3.Stop();

            //make it flashy
            var get = SpawnerExtensions.SpawnEntityAt("Death Explode Boss", transform.position, null, true, false).GetComponent<ParticleSystem>();
            get.startColor = Colors.GetColor(13);
            UnFreeze();
            if (MRenderer != null)
                this.MRenderer.enabled = true;
            if (this.Collider != null)
                this.Collider.enabled = true;
            DamageDealt = hackyBlackKnightPreviousDamage;
            EnemyHealthManager.IsInvincible = false;
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