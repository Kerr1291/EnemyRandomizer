using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using On.Language;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Satchel;
using Satchel.Futils;
using System.Collections.Generic;

namespace EnemyRandomizerMod
{
    public class DefaultSpawnedEnemyControl : MonoBehaviour
    {
        public ObjectMetadata thisMetadata; 
        public ObjectMetadata originialMetadata;
        public DebugColliders debugColliders;
        public EnemyDreamnailReaction edr;
        protected bool hasSeenPlayer;
        protected Action onHit;
        protected int hpFromLastUpdate = 0;

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
            thisMetadata = new ObjectMetadata();
            thisMetadata.Setup(gameObject, EnemyRandomizerDatabase.GetDatabase());
            originialMetadata = other;

            if (ControlCameraLocks)
                cams = GetCameraLocksFromScene();

            if (control == null)
                control = gameObject.LocateMyFSM(FSMName);

            SetDreamnailInfo();
            ConfigureRelativeToReplacement();

            if(thisMetadata != null && thisMetadata.EnemyHealthManager != null)
                hpFromLastUpdate = thisMetadata.EnemyHealthManager.hp;
#if DEBUG
            debugColliders = gameObject.AddComponent<DebugColliders>();
#endif
        }

        protected virtual void Update()
        {
            UpdatePreviousHPAndCheckForOnHit();
            UpdateFSMRefs();
            CheckFSMsUsingHiddenStates();
        }

        protected virtual void UpdatePreviousHPAndCheckForOnHit()
        {
            if (thisMetadata == null)
                return;

            if (thisMetadata.EnemyHealthManager == null)
                return;

            int newHp = thisMetadata.EnemyHealthManager.hp;
            if (hpFromLastUpdate > 0 && newHp < hpFromLastUpdate)
            {
                onHit?.Invoke();
                hpFromLastUpdate = newHp;
            }
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

            if (!control.enabled)
                return;

            if (FSMsUsingHiddenStates == null)
                return;

            if (FSMsUsingHiddenStates.Count <= 0)
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

        protected virtual void OnDestroy()
        {
            if (hasCustomDreamnailReaction)
            {
                On.EnemyDreamnailReaction.SetConvoTitle -= EnemyDreamnailReaction_SetConvoTitle;
                On.Language.Language.Get_string_string -= Language_Get_string_string;
            }
        }

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
            if (thisMetadata != null && originialMetadata != null)
            {
                if (thisMetadata.IsBoss && !originialMetadata.IsBoss)
                {
                    thisMetadata.EnemyHealthManager.hp = ScaleHPFromBossToNormal(thisMetadata.EnemyHealthManager.hp, originialMetadata.EnemyHealthManager.hp);
                }

                if (!thisMetadata.IsBoss && originialMetadata.IsBoss)
                {
                    thisMetadata.EnemyHealthManager.hp = ScaleHPFromBossToNormal(thisMetadata.EnemyHealthManager.hp, originialMetadata.EnemyHealthManager.hp);
                }
            }
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
            if(thisMetadata.GeoManager != null)
                this.thisMetadata.GeoManager.Value *= 5;
        }

        protected virtual void SetupBossAsNormalEnemy()
        {
            if (originialMetadata != null)
            {
                RNG rng = new RNG();
                rng.Reset();
                int extra = rng.Rand(0, 24);
                thisMetadata.GeoManager.Value = originialMetadata.GeoManager.Value + extra;
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
    }

    public class DefaultPrefabConfig : IPrefabConfig
    {
        public virtual void SetupPrefab(PrefabObject p)
        {
            p.prefabName = EnemyRandomizerDatabase.ToDatabaseKey(p.prefab.name);
        }
    }

    public class DefaultPrefabConfig<TControlComponent> : DefaultPrefabConfig
        where TControlComponent : DefaultSpawnedEnemyControl
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);
            p.prefab.AddComponent<TControlComponent>();
        }
    }

    public class DefaultSpawner : ISpawner
    {
        public virtual GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            GameObject gameObject = null;

            if(p.prefab == null)
            {
                Dev.LogError("Cannot Instantiate a null object!");
            }    

            try
            {
                gameObject = GameObject.Instantiate(p.prefab);
            }
            catch(Exception e)
            {
                Dev.LogError($"Error when trying to instantiate {p.prefab} from {p.prefabType} at {p.source.path} in {p.source.Name}");
            }

            if (gameObject == null)
                return null;

            if(source == null)
                gameObject.name = gameObject.name + "(" + Guid.NewGuid().ToString() + ")"; //name values in parenthesis will be trimmed out when converting to a database key'd name
            else
                gameObject.name = gameObject.name + $" ([{source.ObjectPosition.GetHashCode()}][{source.ScenePath.GetHashCode()}])"; //name values in parenthesis will be trimmed out when converting to a database key'd name
            return gameObject;
        }
    }

    public class DefaultSpawner<TControlComponent> : DefaultSpawner
        where TControlComponent : DefaultSpawnedEnemyControl
    {
        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            var newObject = base.Spawn(p, source);
            var control = newObject.GetOrAddComponent<TControlComponent>();
            control.Setup(source);
            return newObject;
        }
    }

    public class CorpseOrientationFixer : MonoBehaviour
    {
        public float corpseAngle;
        public float timeout = 5f;

        IEnumerator Start()
        {
            while (timeout > 0f)
            {
                var angles = transform.localEulerAngles;
                angles.z = corpseAngle;
                transform.localEulerAngles = angles;
                yield return null;
                timeout -= Time.deltaTime;
            }

            yield break;
        }
    }

    public class CorpseRemover : MonoBehaviour
    {
        public string replacementEffect = "Pt Feather Burst";
        protected virtual void OnEnable()
        {
            EnemyRandomizerDatabase.CustomSpawnWithLogic(gameObject.transform.position, "Pt Feather Burst", null, true);
            GameObject.Destroy(gameObject);
        }
    }
}
