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
        protected List<PlayMakerFSM> FSMsUsingHiddenStates { get; set; }
        protected Dictionary<PlayMakerFSM, string> FSMsWithResetToStateOnHide { get; set; }

        public Vector2 pos2d => new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        public Vector2 pos2dWithOffset => new Vector2(gameObject.transform.position.x, gameObject.transform.position.y) + new Vector2(0, 1f);
        public Vector2 heroPos2d => new Vector2(HeroController.instance.transform.position.x, HeroController.instance.transform.position.y);
        public Vector2 heroPosWithOffset => heroPos2d + new Vector2(0, 1f);
        public float floorY => heroPosWithOffset.FireRayGlobal(Vector2.down, float.MaxValue).point.y;
        public float roofY => heroPosWithOffset.FireRayGlobal(Vector2.up, float.MaxValue).point.y;
        public float edgeL => heroPosWithOffset.FireRayGlobal(Vector2.left, float.MaxValue).point.x;
        public float edgeR => heroPosWithOffset.FireRayGlobal(Vector2.right, float.MaxValue).point.x;
        public float aggroRange => 50f;

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

            if(originialMetadata != null && originialMetadata.DatabaseName.Contains("Health"))
            {
                thisMetadata.EnemyHealthManager.OnDeath -= DoBlueHealHero;
                thisMetadata.EnemyHealthManager.OnDeath += DoBlueHealHero;
            }
        }

        protected virtual void OnEnable()
        {
            if (!thisMetadata.IsFlying)
            {
                if (thisMetadata.IsMobile)
                {
                    gameObject.StickToGround();
                }
            }
        }

        protected virtual void OnDisable()
        {
            disposables.Clear();
        }

        protected virtual void OnDestroy()
        {
            originialMetadata.Dispose();
            thisMetadata.Dispose();
            disposables.Dispose();
            if (hasCustomDreamnailReaction)
            {
                On.EnemyDreamnailReaction.SetConvoTitle -= EnemyDreamnailReaction_SetConvoTitle;
                On.Language.Language.Get_string_string -= Language_Get_string_string;
            }
            DieChildrenOnDeath();
        }

        protected virtual void Update()
        {
            UpdateFSMRefs();
            CheckFSMsUsingHiddenStates();
            UpdateAndTrackChildren();
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
            return previousHP;
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

        public void DoBlueHealHero()
        {
            StartCoroutine(this.BlueHealHero());
        }

        private IEnumerator BlueHealHero(float maxHealDistance = 40f)
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
    }
}
