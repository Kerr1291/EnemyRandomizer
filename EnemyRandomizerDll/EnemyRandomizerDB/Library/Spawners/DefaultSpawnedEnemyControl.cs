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
                else if(objectThatWillBeReplaced.name.Contains("fluke_baby"))
                {
                    MaxHP = 1;
                    gameObject.ScaleObject(0.25f);
                    gameObject.ScaleAudio(0.25f);
                    bool canDamageHero = SpawnerExtensions.RollProbability(out _, 10, 100);
                    if(!canDamageHero)
                    {
                        var damageHero = gameObject.GetComponent<DamageHero>();
                        if(damageHero != null)
                        {
                            damageHero.damageDealt = 0;
                        }
                    }
                }
                else if(objectThatWillBeReplaced.IsBoss())
                {
                    CheckHackyBossSetup();
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

        public virtual int GetStartingMaxHP(string prefabName)
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
                if(doSpecialBossAggro)
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

        protected virtual void Update()
        {
            if (!loaded)
                return;

            if (gameObject.ObjectType() != PrefabObject.PrefabType.Enemy)
                return;

            CheckHackySceneFixesOnUpdate();

            if (counter > 0)
            {
                counter--;
                if (counter <= 0 && lastSetHP > 0)
                {
                    MaxHP = lastSetHP;

                    var orig = ObjectMetadata.GetOriginal(gameObject);

                    if(specialAggroRange != null || (orig != null && orig.ObjectName.Contains("False Knight")))
                    {
                        Dev.Log("FALSE KNIGHT REPLACEMENT FOUND");
                        var poob = gameObject.GetComponent<PreventOutOfBounds>();
                        string currentScene = SpawnerExtensions.SceneName(gameObject);
                        if (currentScene == "Crossroads_10_boss" || currentScene == "Crossroads_10")//false knight
                        {

                            Dev.Log("FORCING FALSE KNGIHT REPLACEMENT INTO ARENA");
                            UnFreeze();
                            transform.position = new Vector3(29f, 38f);
                            if (poob != null)
                                poob.ForcePosition(new Vector3(29f, 38f));
                        }
                    }
                }
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
                bool didAggro = false;
                string currentScene = SpawnerExtensions.SceneName(gameObject);
                if (currentScene == "Crossroads_09")//mawlek
                {
                    specialAggroRange = 1f;
                }
                else if (HeroInAggroRange() || BattleManager.AggroAllBossesNow)
                {
                    didAggro = true;
                }


                if(didAggro)
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

        float hackyClimberFixResetTimer = 0f;
        float hackyClimberFixTimer = 0f;
        float previousClimberY = 0f;
        bool hackyClimberFixOnce = false;
        protected virtual void CheckHackySceneFixesOnUpdate()
        {
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
                        Dev.Log("FIXING CLIMBER");
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
                        Dev.Log("FIXING CLIMBER");
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




            if (doSpecialBossAggro && ( currentScene == "Mines_18_boss" || currentScene == "Mines_18" || currentScene == "Mines_32"))
            {
                if(BattleManager.StateMachine.Value.battleStarted && !BattleManager.StateMachine.Value.battleEnded)
                {
                    hackyLaserCDTimer += Time.deltaTime;
                    if(hackyLaserCDTimer > 6f)
                    {
                        PlayMakerFSM.BroadcastEvent("LASER SHOOT");
                        hackyLaserCDTimer = SpawnerExtensions.GetRandomValueBetween(-2, 2);
                    }
                }
            }

            if (needUnstuck)
            {
                if(poob != null)
                {
                    poob.ForcePosition(fixedPos);
                    transform.position = fixedPos;
                }
            }
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

                if(gameObject.IsClimbing())
                {
                    if(!isRoofFlat)
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
                if (MRenderer != null)
                    MRenderer.enabled = false;

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
        }

        protected virtual void CheckHackyBossSetup()
        {
            string currentScene = SpawnerExtensions.SceneName(gameObject);
            bool isBoss = SpawnerExtensions.IsBoss(originialMetadata.GetDatabaseKey());
            if (isBoss)
            {
                doSpecialBossAggro = true;

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
                else if ((currentScene == "Ruins1_24" || currentScene == "Ruins1_24_boss") && originialMetadata.GetDatabaseKey().Contains("Phase2"))//mage lord2
                {
                    specialAggroRange = 10f;
                }
                else if (currentScene == "Ruins1_24" || currentScene == "Ruins1_24_boss")//mage lord
                {
                    specialAggroRange = 10f;
                }
                else if (currentScene == "Dream_02_Mage_Lord" && originialMetadata.GetDatabaseKey().Contains("Phase2"))//
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
                else if (currentScene == "Dream_Mighty_Zote" && originialMetadata.GetDatabaseKey().Contains("Prince"))//zote
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
                    BattleManager.StateMachine.Value.FSM.SendEvent("START");
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
                }
                else if (currentScene == "Ruins1_24" || currentScene == "Ruins1_24_boss")//mage lord
                {
                    BattleStateMachine.CloseGates(true);
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
                BattleManager.StateMachine.Value.FSM.SendEvent("KILLED");
                BattleStateMachine.OpenGates(false);
                BattleStateMachine.UnlockCameras();
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

                GameManager.instance.BroadcastFSMEventAfterTime("PHASE 2", 5f);

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
                BattleManager.StateMachine.Value.FSM.SendEvent("GHOST DEAD");
                PlayMakerFSM.BroadcastEvent("GHOST DEFEAT");
            }
            else if (currentScene == "Fungus3_40" || currentScene == "Fungus3_40_boss")//marmu
            {
                if (originialMetadata != null && originialMetadata.ObjectName.Contains("Marmu"))
                {
                    GameManager.instance.playerData.killedGhostMarmu = true;
                    BattleManager.StateMachine.Value.FSM.SendEvent("GHOST DEAD");
                    PlayMakerFSM.BroadcastEvent("GHOST DEFEAT");
                }
            }
            else if (currentScene == "Deepnest_40")//galien
            {
                GameManager.instance.playerData.galienDefeated = 1;
                BattleManager.StateMachine.Value.FSM.SendEvent("GHOST DEAD");
                PlayMakerFSM.BroadcastEvent("GHOST DEFEAT");
            }
            else if (currentScene == "Cliffs_02_boss" || currentScene == "Cliffs_02")//gorb
            {
                GameManager.instance.playerData.aladarSlugDefeated = 1;
                BattleManager.StateMachine.Value.FSM.SendEvent("GHOST DEAD");
                PlayMakerFSM.BroadcastEvent("GHOST DEFEAT");
            }
            else if (currentScene == "Fungus1_35")//no eyes
            {
                GameManager.instance.playerData.noEyesDefeated = 1;
                BattleManager.StateMachine.Value.FSM.SendEvent("GHOST DEAD");
                PlayMakerFSM.BroadcastEvent("GHOST DEFEAT");
            }
            else if (currentScene == "Fungus2_32")//hu
            {
                GameManager.instance.playerData.elderHuDefeated = 1;
                BattleManager.StateMachine.Value.FSM.SendEvent("GHOST DEAD");
                PlayMakerFSM.BroadcastEvent("GHOST DEFEAT");
            }
            else if (currentScene == "Deepnest_East_10")//markoth
            {
                GameManager.instance.playerData.markothDefeated = 1;
                BattleManager.StateMachine.Value.FSM.SendEvent("GHOST DEAD");
                PlayMakerFSM.BroadcastEvent("GHOST DEFEAT");
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////
            if (notifyBattleEnded)
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