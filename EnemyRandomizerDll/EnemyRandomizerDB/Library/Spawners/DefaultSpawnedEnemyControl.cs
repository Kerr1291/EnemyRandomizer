using UnityEngine;
using System;
using System.Linq;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Satchel;
using Satchel.Futils;
using System.Collections.Generic;
using UniRx;
using System.Collections;

namespace EnemyRandomizerMod
{
    public class DefaultSpawnedEnemyControl : MonoBehaviour, IExtraDamageable, IHitResponder
    {
        public ObjectMetadata thisMetadata;
        public ObjectMetadata originialMetadata;
        public DebugColliders debugColliders;
        public EnemyDreamnailReaction edr;
        protected bool hasSeenPlayer;
        public virtual int maxBabies => 5;
        public virtual bool takesSpecialCharmDamage => this.thisMetadata.IsTinker ? true : false;
        public virtual bool takesSpecialSpellDamage => this.thisMetadata.IsTinker ? true : false;

        //if true, will set the max babies to 5
        public virtual bool dieChildrenOnDeath => false;
        public virtual bool useCustomPositonOnShow => false;
        public virtual bool showWhenHeroIsInAggroRange => false;

        protected bool didShowWhenHeroWasInAggroRange = false;

        public virtual bool explodeOnDeath => false;
        public virtual string spawnEntityOnDeath => null;
        public virtual bool doBlueHealHeroOnDeath => false;
        public virtual bool didOriginalDoBlueHealHeroOnDeath => originialMetadata != null && originialMetadata.DatabaseName.Contains("Health");

        public List<GameObject> children = new List<GameObject>();

        protected CompositeDisposable disposables = new CompositeDisposable();

        public virtual PlayMakerFSM control { get; protected set; }

        /// <summary>
        /// override to autmatically set the control fsm used by this enemy in setup
        /// </summary>
        public virtual string FSMName => null; 

        protected Dictionary<string, Func<DefaultSpawnedEnemyControl, float>> CustomFloatRefs = new Dictionary<string, Func<DefaultSpawnedEnemyControl, float>>();
        protected virtual Dictionary<string, Func<DefaultSpawnedEnemyControl, float>> FloatRefs => CustomFloatRefs;

        protected virtual string FSMHiddenStateName => "Hidden";
        protected virtual List<PlayMakerFSM> FSMsUsingHiddenStates { get; set; }
        protected virtual Dictionary<PlayMakerFSM, string> FSMsWithResetToStateOnHide { get; set; }

        public virtual Vector2 pos2d => new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        public virtual Vector2 pos2dWithOffset => new Vector2(gameObject.transform.position.x, gameObject.transform.position.y) + new Vector2(0, 1f);
        public virtual Vector2 heroPos2d => new Vector2(HeroController.instance.transform.position.x, HeroController.instance.transform.position.y);
        public virtual Vector2 heroPosWithOffset => heroPos2d + new Vector2(0, 1f);
        public virtual float floorY => heroPosWithOffset.FireRayGlobal(Vector2.down, float.MaxValue).point.y;
        public virtual float roofY => heroPosWithOffset.FireRayGlobal(Vector2.up, float.MaxValue).point.y;
        public virtual float edgeL => heroPosWithOffset.FireRayGlobal(Vector2.left, float.MaxValue).point.x;
        public virtual float edgeR => heroPosWithOffset.FireRayGlobal(Vector2.right, float.MaxValue).point.x;
        public virtual float aggroRange => 40f;

        public bool hasCustomDreamnailReaction;
        public string customDreamnailKey;

        public virtual string customDreamnailSourceName { get => originialMetadata == null ? "meme" : originialMetadata.DatabaseName; }

        //TODO: put memes etc here
        public virtual string customDreamnailText { get => $"In another dream, I was a {customDreamnailSourceName}..."; }

        /// <summary>
        /// Override to enable this enemy to disable camera locks
        /// </summary>
        protected virtual bool ControlCameraLocks { get => false; }

        protected virtual IEnumerable<CameraLockArea> cams { get; set; }

        public virtual void Setup(ObjectMetadata other)
        {
            disposables.Clear();
            thisMetadata = new ObjectMetadata();
            thisMetadata.Setup(gameObject, EnemyRandomizerDatabase.GetDatabase());
            originialMetadata = other;

            if (ControlCameraLocks)
                cams = GetCameraLocksFromScene();

            if (control == null)
                control = gameObject.LocateMyFSM(FSMName);

            SetDreamnailInfo();
            ConfigureRelativeToReplacement();

#if DEBUG
            debugColliders = gameObject.AddComponent<DebugColliders>(); 
#endif
            ImportItemFromReplacement();
        }

        protected virtual void ImportItemFromReplacement()
        {
            if(originialMetadata != null && originialMetadata != thisMetadata && originialMetadata.Source != thisMetadata.Source && originialMetadata.HasAvailableItem)
            {
                thisMetadata.ImportItem(originialMetadata);
            }
        }

        protected virtual void SpawnAndFlingItem()
        {
            if(thisMetadata.HasAvailableItem)
            {
                FlingUtils.SelfConfig fling = new FlingUtils.SelfConfig()
                {
                    Object = thisMetadata.AvailableItem.Spawn(transform.position),
                    SpeedMin = 5f,
                    SpeedMax = 10f,
                    AngleMin = 0f,
                    AngleMax = 180f
                };
                FlingUtils.FlingObject(fling, null, Vector3.zero);
            }
        }

        protected virtual void OnEnable()
        {
            if (useCustomPositonOnShow)
                SetCustomPositionOnShow();
            else
                SetDefaultPosition();

            if (showWhenHeroIsInAggroRange)
            {
                if (!HeroInAggroRange())
                    Hide();
            }
        }

        protected virtual void OnDisable()
        {
            disposables.Clear();
        }

        protected virtual void OnDestroy()
        {
            if (hasCustomDreamnailReaction)
            {
                On.EnemyDreamnailReaction.SetConvoTitle -= EnemyDreamnailReaction_SetConvoTitle;
                On.Language.Language.Get_string_string -= Language_Get_string_string;
            }

            if(dieChildrenOnDeath)
                DieChildrenOnDeath();

            if(explodeOnDeath)
                ExplodeOnDeath();

            if(!string.IsNullOrEmpty(spawnEntityOnDeath))
                SpawnEntityOnDeath();

            if (doBlueHealHeroOnDeath || didOriginalDoBlueHealHeroOnDeath)
                DoBlueHealHero();

            if (thisMetadata.HasAvailableItem)
                SpawnAndFlingItem();

            //TODO: see if the default works, first
            //ForceUpdateJournal();

            if(originialMetadata != null)
                originialMetadata.Dispose();

            if(originialMetadata != thisMetadata)
                thisMetadata.Dispose();

            disposables.Dispose();
        }

        protected virtual void ForceUpdateJournal()
        {
            var pdName = thisMetadata.PlayerDataName;
            RecordCustomJournalOnDeath(pdName);
        }

        protected virtual void RecordCustomJournalOnDeath(string pdName)
        {
            PlayerData playerData = GameManager.instance.playerData;
            string text = "killed" + pdName;
            string text2 = "kills" + pdName;
            string text3 = "newData" + pdName;
            bool flag = false;
            if (!playerData.GetBool(text))
            {
                flag = true;
                playerData.SetBool(text, true);
                playerData.SetBool(text3, true);
            }
            bool flag2 = false;
            int num = playerData.GetInt(text2);
            if (num > 0)
            {
                num--;
                playerData.SetInt(text2, num);
                if (num <= 0)
                {
                    flag2 = true;
                }
            }
            if (playerData.hasJournal)
            {
                bool flag3 = false;
                if (flag2)
                {
                    flag3 = true;
                    playerData.journalEntriesCompleted++;
                }
                else if (flag)
                {
                    flag3 = true;
                    playerData.journalNotesCompleted++;
                }
                if (flag3)
                {
                    //in lieu of the proper journal unlock effect, just do something
                    EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "dream_particle_03", null, true);
                }
            }
        }

        protected virtual void ExplodeOnDeath()
        {
            if (!explodeOnDeath)
                return;

            EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Gas Explosion Recycle L", null, true);
        }

        protected virtual void SpawnExplosionAt(Vector3 pos)
        {
            EnemyRandomizerDatabase.CustomSpawnWithLogic(pos, "Gas Explosion Recycle M", null, true);
        }

        protected virtual void SpawnEntityOnDeath()
        {
            if (string.IsNullOrEmpty(spawnEntityOnDeath))
                return;

            EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, spawnEntityOnDeath, null, true);
        }

        protected virtual bool RollProbability(out int result, int needValueOrLess = 5, int maxPossibleValue = 20)
        {
            RNG rng = new RNG();
            rng.Reset();
            result = rng.Rand(0, maxPossibleValue);
            return result < needValueOrLess;
        }

        protected virtual void SetGeoRandomBetween(int minGeo, int maxGeo)
        {
            RNG rng = new RNG();
            rng.Reset();
            thisMetadata.Geo = rng.Rand(minGeo, maxGeo);
        }

        protected virtual void Update()
        {
            if(showWhenHeroIsInAggroRange)
            {
                if(!didShowWhenHeroWasInAggroRange && HeroInAggroRange())
                {
                    didShowWhenHeroWasInAggroRange = true;
                    Show();
                }
            }


            UpdateFSMRefs();
            CheckFSMsUsingHiddenStates();
            UpdateAndTrackChildren();
        }

        protected IEnumerator DistanceFlyChase(GameObject self, GameObject target, float distance, float acceleration, float speedMax, float? followHeightOffset = null)
        {
            var rb2d = self.GetComponent<Rigidbody2D>();
            if ( rb2d == null)
            {
                yield break;
            }
            var distanceAway = Mathf.Sqrt(Mathf.Pow( self.transform.position.x -  target.transform.position.x, 2f) + Mathf.Pow( self.transform.position.y -  target.transform.position.y, 2f));
            Vector2 velocity =  rb2d.velocity;
            if ( distanceAway >  distance )
            {
                if ( self.transform.position.x <  target.transform.position.x)
                {
                    velocity.x +=  acceleration ;
                }
                else
                {
                    velocity.x -=  acceleration ;
                }
                if (followHeightOffset == null)
                {
                    if ( self.transform.position.y <  target.transform.position.y)
                    {
                        velocity.y +=  acceleration ;
                    }
                    else
                    {
                        velocity.y -=  acceleration ;
                    }
                }
            }
            else
            {
                if ( self.transform.position.x <  target.transform.position.x)
                {
                    velocity.x -=  acceleration ;
                }
                else
                {
                    velocity.x +=  acceleration ;
                }
                if (followHeightOffset == null)
                {
                    if ( self.transform.position.y <  target.transform.position.y)
                    {
                        velocity.y -=  acceleration ;
                    }
                    else
                    {
                        velocity.y +=  acceleration ;
                    }
                }
            }
            if (followHeightOffset != null)
            {
                if ( self.transform.position.y <  target.transform.position.y + followHeightOffset.Value)
                {
                    velocity.y +=  acceleration ;
                }
                if ( self.transform.position.y >  target.transform.position.y + followHeightOffset.Value)
                {
                    velocity.y -=  acceleration ;
                }
            }
            if (velocity.x >  speedMax )
            {
                velocity.x =  speedMax ;
            }
            if (velocity.x < - speedMax )
            {
                velocity.x = - speedMax ;
            }
            if (velocity.y >  speedMax )
            {
                velocity.y =  speedMax ;
            }
            if (velocity.y < - speedMax )
            {
                velocity.y = - speedMax ;
            }
             rb2d.velocity = velocity;
        }

        protected virtual Func<GameObject> GetRandomAttackSpawnerFunc()
        {
            RNG rng = new RNG();
            rng.Reset();

            List<Func<GameObject>> trailEffects = new List<Func<GameObject>>()
                {
                    //WARNING: using "CustomSpawnWithLogic" will override any replacement randomization modules
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Mega Jelly Zap", null, false),
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Falling Barrel", null, false),
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Electro Zap", null, false),
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Shot PickAxe", null, false),
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Dung Ball Small", null, false),
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Paint Shot P Down", null, false),
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Paint Shot B_fix", null, false),
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Paint Shot R", null, false),
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Gas Explosion Recycle L", null, false),
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Lil Jellyfish", null, false),
                };

            var selection = trailEffects.GetRandomElementFromList(rng);
            return selection;
        }

        protected virtual void UpdateRefs(PlayMakerFSM fsm, Dictionary<string, Func<DefaultSpawnedEnemyControl, float>> refs)
        {
            if (fsm == null)
                return;

            foreach (var fref in refs)
            {
                var fvar = fsm.FsmVariables.GetFsmFloat(fref.Key);
                if (fvar != null)
                {
                    fvar.Value = fref.Value.Invoke(this);
                }
            }
        }

        protected virtual void UpdateFSMRefs()
        {
            if (control != null && control.enabled && FloatRefs != null && FloatRefs.Count > 0)
                UpdateRefs(control, FloatRefs);
        }

        protected virtual void StopPhysicsBody()
        {
            var body = thisMetadata.PhysicsBody;
            if (body != null)
            {
                body.velocity = Vector2.zero;
                body.angularVelocity = 0f;
            }
        }

        protected virtual bool CanEnemySeePlayer()
        {
            HeroController instance = HeroController.instance;
            if (instance == null)
            {
                return false;
            }
            Vector2 vector = base.transform.position;
            Vector2 vector2 = instance.transform.position;
            Vector2 vector3 = vector2 - vector;
            if (Physics2D.Raycast(vector, vector3.normalized, vector3.magnitude, 256))
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        protected virtual Vector2 DirectionToPlayer()
        {
            Vector2 vector = pos2d;
            Vector2 vector2 = heroPos2d;
            return (vector2 - vector).normalized;
        }

        protected virtual float DistanceToPlayer()
        {
            Vector2 vector = pos2d;
            Vector2 vector2 = heroPos2d;
            Vector2 vector3 = vector2 - vector;
            return vector3.magnitude;
        }

        protected virtual float DistanceFromPointToPlayer(Vector2 point)
        {
            Vector2 vector = point;
            Vector2 vector2 = heroPos2d;
            Vector2 vector3 = vector2 - vector;
            return vector3.magnitude;
        }

        protected virtual bool IsPointNearPlayer(Vector2 point, float near = 1f)
        {
            Vector2 vector = point;
            Vector2 vector2 = heroPos2d;
            Vector2 vector3 = vector2 - vector;
            return vector3.magnitude < near;
        }

        protected virtual bool IsEnemyNearPlayer(float dist)
        {
            return DistanceToPlayer() <= dist;
        }

        protected virtual bool HeroInAggroRange()
        {
            if(hasSeenPlayer)
            {
                return IsEnemyNearPlayer(aggroRange);
            }
            else
            {
                if(CanEnemySeePlayer())
                {
                    hasSeenPlayer = true;
                    return true;
                }

                return false;
            }
        }

        protected virtual void Hide()
        {
            if (control.enabled)
            {
                control.enabled = false;
                StopPhysicsBody();
            }

            CheckFSMsWithResetOnHideStates();
        }

        protected virtual void Show()
        {
            if (!control.enabled)
            {
                control.enabled = true;
                if (ControlCameraLocks)
                    UnlockCameras(cams);
            }

            if (useCustomPositonOnShow)
                SetCustomPositionOnShow();
        }

        protected virtual void CheckFSMsUsingHiddenStates()
        {
            if (control == null)
                return;

            if (FSMsUsingHiddenStates == null || FSMsUsingHiddenStates.Count <= 0)
                return;

            List<PlayMakerFSM> activatedFSMs = new List<PlayMakerFSM>();

            foreach (var fsm in FSMsUsingHiddenStates)
            {
                if (!fsm.enabled && control.enabled)
                    fsm.enabled = true;

                if (fsm.ActiveStateName == FSMHiddenStateName)
                {
                    fsm.SendEvent("SHOW");

                    activatedFSMs.Add(fsm);
                }
            }

            activatedFSMs.ForEach(x => FSMsUsingHiddenStates.Remove(x));
        }

        protected virtual void CheckFSMsWithResetOnHideStates()
        {
            if (FSMsWithResetToStateOnHide == null)
                return;

            if (FSMsUsingHiddenStates.Count <= 0)
                return;

            foreach (var fsmStatePair in FSMsWithResetToStateOnHide)
            {
                if (!fsmStatePair.Key.enabled)
                    continue;

                fsmStatePair.Key.SetState(fsmStatePair.Value);
            }
        }

        //protected virtual void OnDisable()
        //{
        //    if(thisMetadata.ObjectType == PrefabObject.PrefabType.Effect)
        //    {
        //        ObjectPool.Recycle(gameObject);
        //    }
        //    else if (thisMetadata.ObjectType == PrefabObject.PrefabType.Hazard)
        //    {
        //        ObjectPool.Recycle(gameObject);
        //    }
        //}


        protected virtual string Language_Get_string_string(On.Language.Language.orig_Get_string_string orig, string key, string sheetTitle)
        {
            if(key.Contains(customDreamnailKey))
            {
                return customDreamnailText;
            }
            else
            {
                return orig(key, sheetTitle);
            }
        }

        protected virtual GameObject AddParticleEffect_TorchFire(int fireSize = 5, Transform customParent = null)
        {
            GameObject effect = EnemyRandomizerDatabase.GetDatabase().Spawn("Fire Particles", null);
            if(customParent == null)
                effect.transform.parent = transform;
            else
                effect.transform.parent = customParent;

            effect.transform.localPosition = Vector3.zero;
            var pe = effect.GetComponent<ParticleSystem>();
            pe.startSize = fireSize;
            pe.simulationSpace = ParticleSystemSimulationSpace.World;
            effect.SafeSetActive(true);
            return effect;
        }

        protected virtual GameObject AddParticleEffect_TorchShadeEmissions()
        {
            GameObject effect = EnemyRandomizerDatabase.GetDatabase().Spawn("Particle System B", null);
            effect.transform.parent = transform;
            effect.transform.localPosition = Vector3.zero;
            effect.SafeSetActive(true);
            return effect;
        }

        protected virtual ParticleSystem AddParticleEffect_WhiteSoulEmissions()
        {
            var glow = EnemyRandomizerDatabase.GetDatabase().Spawn("Summon", null);
            var ge = glow.GetComponent<ParticleSystem>();
            glow.transform.parent = transform;
            glow.transform.localPosition = Vector3.zero;
            ge.simulationSpace = ParticleSystemSimulationSpace.World;
            ge.startSize = 3;
            glow.SetActive(true);
            return ge;
        }

        protected virtual void StartTrailEffectSpawns(int count, float spawnRate, string entityToSpawn)
        {
            StartCoroutine(TrailEffectSpawns(count,spawnRate,entityToSpawn));
        }

        protected virtual IEnumerator TrailEffectSpawns(int count, float spawnRate, string entityToSpawn)
        {
            for (int i = 0; i < count; ++i)
            {
                var result = EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, entityToSpawn, null, false);
                result.transform.position = transform.position;
                result.SetActive(true);
                yield return new WaitForSeconds(spawnRate);
            }
        }

        protected virtual void StartTrailEffectSpawns(int count, float spawnRate, Func<GameObject> spawner)
        {
            StartCoroutine(TrailEffectSpawns(count, spawnRate, spawner));
        }

        protected virtual IEnumerator TrailEffectSpawns(int count, float spawnRate, Func<GameObject> spawner)
        {
            for (int i = 0; i < count; ++i)
            {
                var result = spawner.Invoke();
                result.transform.position = transform.position;
                result.SetActive(true);
                yield return new WaitForSeconds(spawnRate);
            }
        }


        protected virtual void ConfigureRelativeToReplacement()
        {
            if (thisMetadata != null && thisMetadata.ObjectType != PrefabObject.PrefabType.Enemy)
                return;

            if (thisMetadata != null && originialMetadata != null)
            {
                if (thisMetadata.IsBoss && !originialMetadata.IsBoss)
                {
                    SetupBossAsNormalEnemy();
                }

                if (!thisMetadata.IsBoss && originialMetadata.IsBoss)
                {
                    SetupNormalEnemyAsBoss();
                }
            }

            ScaleHP();
        }

        protected virtual void ScaleHP()
        {
            if (thisMetadata != null && originialMetadata != null && thisMetadata.EnemyHealthManager != null && originialMetadata.EnemyHealthManager != null)
            {
                if (thisMetadata.IsBoss && !originialMetadata.IsBoss)
                {
                    thisMetadata.EnemyHealthManager.hp = ScaleHPFromBossToNormal(thisMetadata.EnemyHealthManager.hp, originialMetadata.EnemyHealthManager.hp);
                }

                else if (!thisMetadata.IsBoss && originialMetadata.IsBoss)
                {
                    thisMetadata.EnemyHealthManager.hp = ScaleHPFromBossToNormal(thisMetadata.EnemyHealthManager.hp, originialMetadata.EnemyHealthManager.hp);
                }

                else if (thisMetadata.IsBoss && originialMetadata.IsBoss)
                {
                    thisMetadata.EnemyHealthManager.hp = ScaleHPFromNormalToNormal(thisMetadata.EnemyHealthManager.hp, originialMetadata.EnemyHealthManager.hp);
                }

                else if (!thisMetadata.IsBoss && !originialMetadata.IsBoss)
                {
                    thisMetadata.EnemyHealthManager.hp = ScaleHPFromBossToBoss(thisMetadata.EnemyHealthManager.hp, originialMetadata.EnemyHealthManager.hp);
                }
            }
        }

        protected virtual int ScaleHPFromNormalToNormal(int defaultHP, int previousHP)
        {
            if (defaultHP > previousHP * 2)
            {
                return previousHP * 2;
            }
            else if (defaultHP > previousHP)
            {
                return defaultHP;
            }

            return previousHP;
        }

        protected virtual int ScaleHPFromBossToBoss(int defaultHP, int previousHP)
        {
            if (defaultHP > previousHP * 2)
            {
                return previousHP * 2;
            }
            else if (defaultHP > previousHP)
            {
                return defaultHP;
            }

            return previousHP;
        }

        protected virtual int ScaleHPFromBossToNormal(int defaultHP, int previousHP)
        {
            if (previousHP * 2 < defaultHP)
                return Mathf.FloorToInt(previousHP * 2f);
            else if (previousHP < defaultHP)
                return previousHP;
            else
                return defaultHP;
        }

        protected virtual void SetCustomPositionOnShow()
        {
            gameObject.StickToGround();
        }

        protected virtual void SetDefaultPosition()
        {
            if (!thisMetadata.IsFlying)
            {
                if (thisMetadata.IsMobile)
                {
                    gameObject.StickToGround();
                }
            }
            else
            {
                if (originialMetadata != null && originialMetadata.IsInGroundEnemy)
                    transform.position = transform.position.ToVec2() + GetUpFromSelfAngle(false) * 2f;
            }
        }

        protected virtual void AddTimeoutAction(FsmState state, string eventName, float timeout)
        {
            state.AddCustomAction(() => { StartTimeoutState(state.Name, eventName, timeout); });
        }

        protected virtual void DisableKillFreeze()
        {
            var deathEffects = gameObject.GetComponentInChildren<EnemyDeathEffectsUninfected>(true);
            if (deathEffects != null)
            {
                deathEffects.doKillFreeze = false;
            }
        }

        protected virtual void StartTimeoutState(string currentState, string endEvent, float timeout)
        {
            StartCoroutine(TimeoutState(currentState, endEvent, timeout));
        }

        protected virtual IEnumerator TimeoutState(string currentState, string endEvent, float timeout)
        {
            while (control.ActiveStateName == currentState)
            {
                timeout -= Time.deltaTime;

                if (timeout <= 0f)
                {
                    control.SendEvent(endEvent);
                    break;
                }
                yield return new WaitForEndOfFrame();
            }

            yield break;
        }

        protected virtual IEnumerator TimeoutState(PlayMakerFSM fsm, string currentState, string endEvent, float timeout)
        {
            while (fsm.ActiveStateName == currentState)
            {
                timeout -= Time.deltaTime;

                if (timeout <= 0f)
                {
                    fsm.SendEvent(endEvent);
                    break;
                }
                yield return new WaitForEndOfFrame();
            }

            yield break;
        }

        protected virtual int ScaleHPFromNormalToBoss(int defaultHP, int previousHP)
        {
            return previousHP;
        }

        protected virtual void SetDreamnailInfo()
        {
            hasCustomDreamnailReaction = GetComponent<EnemyDreamnailReaction>() != null;
            if (hasCustomDreamnailReaction)
            {
                edr = GetComponent<EnemyDreamnailReaction>();
                customDreamnailKey = Guid.NewGuid().ToString();

                On.Language.Language.Get_string_string -= Language_Get_string_string;
                On.Language.Language.Get_string_string += Language_Get_string_string;

                On.EnemyDreamnailReaction.SetConvoTitle -= EnemyDreamnailReaction_SetConvoTitle;
                On.EnemyDreamnailReaction.SetConvoTitle += EnemyDreamnailReaction_SetConvoTitle;

                SetDreamnailReactionToCustomText();
            }
        }

        private void EnemyDreamnailReaction_SetConvoTitle(On.EnemyDreamnailReaction.orig_SetConvoTitle orig, EnemyDreamnailReaction self, string title)
        {
            if (self != edr || edr == null)
            {
                orig(self, title);
            }
            else
            {
                orig(self, customDreamnailKey);
            }
        }

        protected virtual void SetDreamnailReactionToCustomText()
        {
            if (edr != null)
            {
                try
                {
                    edr.GetType().GetField("convoTitle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .SetValue(edr, customDreamnailKey);
                }
                catch(Exception e)
                {
                    Dev.LogError("Error settings custom dreamnail key for object "+thisMetadata.ScenePath);
                }
            }
        }

        protected virtual void SetupNormalEnemyAsBoss()
        {
            this.thisMetadata.Geo *= 5;
        }

        protected virtual void SetupBossAsNormalEnemy()
        {
            if (originialMetadata != null)
            {
                RNG rng = new RNG();
                rng.Reset();
                int extra = rng.Rand(0, 24);
                thisMetadata.Geo = originialMetadata.Geo + extra;
            }
        }

        protected virtual void DisableSendEvents(PlayMakerFSM fsm, params (string StateName, int ActionIndex)[] stateActions)
        {
            foreach (var sa in stateActions)
            {
                fsm.GetState(sa.StateName).GetAction<SendEventByName>(sa.ActionIndex).sendEvent = string.Empty;
            }
        }

        protected virtual void ChangeRandomIntRange(PlayMakerFSM fsm, string stateName, int min, int max)
        {
            fsm.GetState(stateName).GetFirstActionOfType<RandomInt>().min.Value = min;
            fsm.GetState(stateName).GetFirstActionOfType<RandomInt>().max.Value = max;
        }

        protected virtual void SetAudioOneShotVolume(PlayMakerFSM fsm, string stateName, float vol = 0f)
        {
            fsm.GetState(stateName).GetFirstActionOfType<AudioPlayerOneShotSingle>().volume = vol;
        }

        /// <summary>
        /// WARNING: will remove ALL previous actions on the state
        /// </summary>
        protected virtual void OverrideState(PlayMakerFSM fsm, string stateName, Action stateAction)
        {
            var overrideState = fsm.GetState(stateName);
            overrideState.Actions = new FsmStateAction[] {
                new CustomFsmAction(stateAction)
            };
        }

        protected virtual Vector2 GetUpFromSelfAngle( bool isFlipped )
        {
            Vector2 up = Vector2.zero;

            float angle = transform.localEulerAngles.z % 360f;
            if (!isFlipped)
            {
                angle = (angle + 180f) % 360f;
            }

            if (angle < 5f && angle < 355f)
            {
                up = Vector2.up;
            }
            else if (angle > 85f && angle < 95f)
            {
                up = Vector2.left;
            }
            else if (angle > 175f && angle < 185f)
            {
                up = Vector2.down;
            }
            else if (angle > 265f || angle < 275f)
            {
                up = Vector2.right;
            }

            return up;
        }

        protected virtual IEnumerable<CameraLockArea> GetCameraLocksFromScene()
        {
            return gameObject.GetComponentsFromScene<CameraLockArea>();
        }

        protected virtual void UnlockCameras(IEnumerable<CameraLockArea> cameraLocks)
        {
            if (!ControlCameraLocks)
                return;

            foreach (var c in cameraLocks)
            {
                c.gameObject.SetActive(false);
            }
        }

        protected virtual void InsertHiddenState(PlayMakerFSM fsm,
            string preHideStateName, string preHideTransitionEventName,
            string postHideStateName, bool createNewPreTransitionEvent = false)
        {
            if (FSMsUsingHiddenStates == null)
                FSMsUsingHiddenStates = new List<PlayMakerFSM>();

            try
            {
                //var preHideState = fsm.GetState("Music?");
                var preHideState = fsm.GetState(preHideStateName);
                var hidden = fsm.AddState(FSMHiddenStateName);

                fsm.AddVariable<FsmBool>("IsAggro");
                var isAggro = fsm.FsmVariables.GetFsmBool("IsAggro");
                isAggro.Value = false;

                if (createNewPreTransitionEvent)
                {
                    preHideState.RemoveTransition(preHideTransitionEventName);
                    preHideState.AddTransition("FINISHED", FSMHiddenStateName);

                    var waitAction = preHideState.Actions.OfType<Wait>().LastOrDefault();
                    if (waitAction == null)
                        preHideState.AddAction(new Wait() { finishEvent = new FsmEvent("FINISHED") });
                    else
                        waitAction.finishEvent = new FsmEvent("FINISHED");
                }
                else
                {
                    //change the state to stall the FSM until a player is nearby
                    preHideState.ChangeTransition(preHideTransitionEventName, FSMHiddenStateName);
                }

                fsm.AddTransition(FSMHiddenStateName, "SHOW", postHideStateName);
                hidden.AddAction(new BoolTest() { boolVariable = isAggro, isTrue = new FsmEvent("SHOW"), everyFrame = true });

                FSMsUsingHiddenStates.Add(fsm);
            }
            catch (Exception e) { Dev.Log($"Error in adding an aggro range state to FSM:{fsm.name} PRE STATE:{preHideStateName} EVENT:{preHideTransitionEventName} POST-STATE:{postHideStateName}"); }
        }

        protected virtual void AddResetToStateOnHide(PlayMakerFSM fsm, string resetToState)
        {
            if (FSMsWithResetToStateOnHide == null)
                FSMsWithResetToStateOnHide = new Dictionary<PlayMakerFSM, string>();

            FSMsWithResetToStateOnHide.Add(fsm, resetToState);
        }

        protected virtual void DisableActions(FsmState state, params int[] indices)
        {
            foreach(int i in indices)
            {
                state.DisableAction(i);
            }
        }


        public virtual Vector3 GetRandomDirectionFromSelf(float mindDist = 2f)
        {
            int tries = 10;
            int i = 0;

            Vector2 bestInvalidTry = UnityEngine.Random.insideUnitCircle;
            Vector2 movementDir = bestInvalidTry;

            for (i = 0; i < tries; ++i)
            {
                movementDir = UnityEngine.Random.insideUnitCircle;
                var spawnRay = SpawnerExtensions.GetRayOn(heroPosWithOffset, movementDir, mindDist);
                float sdist = spawnRay.distance;


                if (spawnRay.distance >= 2f)
                {
                    bestInvalidTry = movementDir;
                }

                if (spawnRay.distance < mindDist)
                    continue;

                if (spawnRay.distance - 1f < mindDist)
                    continue;

                break;
            }

            if (i == tries)
                movementDir = bestInvalidTry;

            return movementDir;
        }

        public virtual Vector3 GetRandomPositionInLOSofSelf(float minTeleportDistance, float maxTeleportDistance, float bufferDistanceFromWall = 0f, float minDistanceFromPlayer = 0f)
        {
            RNG rng = new RNG();
            rng.Reset();

            int tries = 10;
            int i = 0;

            Vector2 bestInvalidTry = pos2dWithOffset;
            Vector2 telePoint = pos2dWithOffset;

            for (i = 0; i < tries; ++i)
            {
                float teleDist = rng.Rand(minTeleportDistance, maxTeleportDistance);
                var randomDir = UnityEngine.Random.insideUnitCircle;
                var spawnRay = SpawnerExtensions.GetRayOn(pos2dWithOffset, randomDir, teleDist);
                float sdist = spawnRay.distance;


                if (spawnRay.distance > 2f)
                {
                    bestInvalidTry = spawnRay.point;
                }

                if (IsPointNearPlayer(spawnRay.point, minDistanceFromPlayer))
                    continue;

                if (spawnRay.distance - bufferDistanceFromWall < minTeleportDistance)
                    continue;

                if (spawnRay.distance < teleDist)
                {
                    telePoint = spawnRay.point - spawnRay.normal * bufferDistanceFromWall;
                }
                else
                {
                    telePoint = spawnRay.point;
                }
                break;
            }

            if (i == tries)
                telePoint = bestInvalidTry;

            return telePoint;
        }

        public static Vector2 BounceReflect(Vector2 velocity, Vector2 normal)
        {
            // Calculate the dot product of the velocity and the normal
            float dotProduct = Vector2.Dot(velocity, normal);

            // Calculate the reflection vector
            Vector2 reflection = velocity - 2f * dotProduct * normal;

            // Return the reflected vector
            return reflection;
        }

        public virtual void DoBlueHealHero()
        {
            StartCoroutine(this.BlueHealHero());
        }

        protected virtual IEnumerator BlueHealHero(float maxHealDistance = 40f)
        {
            var flash = HeroController.instance.GetComponent<SpriteFlash>();
            
            if( flash != null)
                flash.flashHealBlue();

            GameManager.UnloadLevel doHeal = null;
            doHeal = delegate ()
            {
                EventRegister.SendEvent("ADD BLUE HEALTH");
                GameManager.instance.UnloadingLevel -= doHeal;
                doHeal = null;
            };
            GameManager.instance.UnloadingLevel += doHeal;
            if (HeroController.instance && Vector2.Distance(base.transform.position, HeroController.instance.transform.position) > maxHealDistance)
            {
                //too far to heal
                yield break;
            }

            yield return new WaitForSeconds(1.2f);
            if (doHeal != null)
            {
                doHeal();
            }
            yield break;
        }

        public virtual Vector3 GetRandomPositionInLOSofPlayer(float minTeleportDistance, float maxTeleportDistance, float bufferDistanceFromWall = 0f, float minDistanceFromPlayer = 0f)
        {
            RNG rng = new RNG();
            rng.Reset();

            int tries = 10;
            int i = 0;

            Vector2 bestInvalidTry = heroPosWithOffset;
            Vector2 telePoint = heroPosWithOffset;

            for (i = 0; i < tries; ++i)
            {
                float teleDist = rng.Rand(minTeleportDistance, maxTeleportDistance);
                var randomDir = UnityEngine.Random.insideUnitCircle;
                var spawnRay = SpawnerExtensions.GetRayOn(heroPosWithOffset, randomDir, teleDist);
                float sdist = spawnRay.distance;


                if (spawnRay.distance > 2f)
                {
                    bestInvalidTry = spawnRay.point;
                }

                if (spawnRay.distance < minDistanceFromPlayer)
                    continue;

                if (spawnRay.distance - bufferDistanceFromWall < minDistanceFromPlayer)
                    continue;                    

                if (spawnRay.distance < teleDist)
                {
                    telePoint = spawnRay.point - spawnRay.normal * bufferDistanceFromWall;
                }
                else
                {
                    telePoint = spawnRay.point;
                }
                break;
            }

            if (i == tries)
                telePoint = bestInvalidTry;

            return telePoint;
        }

        protected virtual void ActivateAndTrackSpawnedObject(GameObject objectThatWillBeReplaced)
        {
            var handle = EnemyRandomizerDatabase.OnObjectReplaced.AsObservable().Subscribe(x =>
            {
                if (children.Contains(x.oldObject.Source))
                    children.Remove(x.oldObject.Source);
                if (!children.Contains(x.newObject.Source))
                    children.Add(x.newObject.Source);
            });

            var child = objectThatWillBeReplaced;
            children.Add(child);
            child.SafeSetActive(true);

            handle.Dispose();
        }


        protected virtual void SpawnAndTrackChild(string objectName, Vector3 spawnPoint)
        {
            var handle = EnemyRandomizerDatabase.OnObjectReplaced.AsObservable().Subscribe(x =>
            {
                if (children.Contains(x.oldObject.Source))
                    children.Remove(x.oldObject.Source);
                if (!children.Contains(x.newObject.Source))
                    children.Add(x.newObject.Source);
            });

            var child = thisMetadata.DB.Spawn(objectName, null);// "Parasite Balloon", null);
            child.transform.position = spawnPoint;
            children.Add(child);
            child.SafeSetActive(true);

            handle.Dispose();
        }

        protected virtual void SetXScaleSign(bool makeNegative)
        {
            float scale = transform.localScale.x;
            if (scale > 0 && makeNegative)
                transform.localScale = new Vector3(-scale, transform.localScale.y, transform.localScale.z);
            else if (scale < 0 && !makeNegative)
                transform.localScale = new Vector3(-scale, transform.localScale.y, transform.localScale.z);
        }

        protected virtual void DieChildrenOnDeath()
        {
            if (dieChildrenOnDeath)
            {
                children.ForEach(x =>
                {
                    if (x == null)
                        return;

                    var hm = x.GetComponent<HealthManager>();
                    if (hm != null)
                    {
                        hm.Die(null, AttackTypes.Generic, true);
                    }
                });
            }
        }

        protected virtual void UpdateAndTrackChildren()
        {
            if (children == null)
                return;

            for (int i = 0; i < children.Count;)
            {
                if (i >= children.Count)
                    break;

                if (children[i] == null)
                {
                    children.RemoveAt(i);
                    continue;
                }
                else
                {
                    var hm = children[i].GetComponent<HealthManager>();
                    if (hm.hp <= 0 || hm.isDead)
                    {
                        children.RemoveAt(i);
                        continue;
                    }
                }

                ++i;
            }
        }

        protected virtual void RemoveFSM(string name)
        {
            var fsm = gameObject.LocateMyFSM(name);
            if (fsm != null)
                Destroy(fsm);
        }

        void IExtraDamageable.RecieveExtraDamage(ExtraDamageTypes extraDamageType)
        {
            if(takesSpecialCharmDamage && this.thisMetadata.EnemyHealthManager != null)
            {
                int dmgAmount = ExtraDamageable.GetDamageOfType(extraDamageType);
                thisMetadata.EnemyHealthManager.ApplyExtraDamage(dmgAmount);
            }
        }

        void IHitResponder.Hit(HitInstance damageInstance)
        {
            if (takesSpecialSpellDamage && this.thisMetadata.EnemyHealthManager != null && damageInstance.AttackType == AttackTypes.Spell)
            {
                int dmgAmount = damageInstance.DamageDealt;
                thisMetadata.EnemyHealthManager.ApplyExtraDamage(dmgAmount);
            }
        }

        public static bool CreateNewDatabasePrefabObject(string databaseID, GameObject source, SceneData sourceScene, PrefabObject.PrefabType prefabType = PrefabObject.PrefabType.Other)
        {
            bool result = false;
            try
            {
                var db = EnemyRandomizerDatabase.GetDatabase();
                if (db != null && !db.otherNames.Contains(databaseID))
                {
                    var beClone = GameObject.Instantiate(source);
                    beClone.SetActive(false);
                    GameObject.DontDestroyOnLoad(beClone);
                    PrefabObject p2 = new PrefabObject();
                    SceneObject sp2 = new SceneObject();
                    sp2.components = new List<string>();
                    sp2.Scene = sourceScene;
                    p2.prefabName = databaseID;
                    p2.prefabType = prefabType;
                    beClone.name = p2.prefabName;
                    sp2.path = beClone.name;
                    p2.prefab = beClone;
                    p2.source = sp2;
                    sp2.LoadedObject = p2;
                    sp2.Scene.sceneObjects.Add(sp2);
                    db.otherPrefabs.Add(p2);
                    db.Others[p2.prefabName] = p2;
                    db.Objects[p2.prefabName] = p2;
                    sp2.Loaded = true;
                    result = true;
                }
            }
            catch(Exception e)
            {
                Dev.Log($"Error creating new database prefab object with ID {databaseID} from source {source}. ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
            }

            return result;
        }
    }
}
