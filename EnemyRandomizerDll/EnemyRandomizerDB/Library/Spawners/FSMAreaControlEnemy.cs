using HutongGames.PlayMaker.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Satchel;
using Satchel.Futils;
using HutongGames.PlayMaker;

namespace EnemyRandomizerMod
{
    public abstract class FSMAreaControlEnemy : DefaultSpawnedEnemyControl
    {
        public PlayMakerFSM control;
        public abstract string FSMName { get; }
        public virtual string FSMHiddenStateName => "Hidden";

        public virtual string OnShowControlBroadcastEvent => string.Empty;

        protected Dictionary<string, Func<FSMAreaControlEnemy, float>> EMPTY_FLOAT_REFS = new Dictionary<string, Func<FSMAreaControlEnemy, float>>();
        public List<PlayMakerFSM> FSMsUsingHiddenStates { get; protected set; }
        public Dictionary<PlayMakerFSM,string> FSMsWithResetToStateOnHide { get; protected set; }

        public Range xR;
        public Range yR;

        public Rect bounds;

        protected virtual bool ControlCameraLocks { get => false; }

        protected virtual Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs
        {
            get => new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                { "Tele X Min", x => x.xR.Min},
                { "Tele X Max", x => x.xR.Max},
                { "Tele Y Max", x => x.yR.Max},
                { "Tele Y Min", x => x.yR.Min},
                { "Hero X", x => x.HeroX },
                { "Hero Y", x => x.HeroY },
                { "Left X", x => x.xR.Min},
                { "Right X", x => x.xR.Max},
                { "Top Y", x => x.yR.Max},
                { "Bot Y", x => x.yR.Min},
            };
        }

        protected FsmFloat heroX;
        protected FsmFloat heroY;

        public virtual float HeroX { get => HeroController.instance.transform.position.x; }
        public virtual float HeroY { get => HeroController.instance.transform.position.y; }

        public virtual float XMIN { get => xR.Min; }
        public virtual float XMAX { get => xR.Max; }
        public virtual float YMIN { get => yR.Min; }
        public virtual float YMAX { get => yR.Max; }

        public virtual float MidX { get => xR.Mid; }
        public virtual float MidY { get => xR.Mid; }

        protected virtual IEnumerable<CameraLockArea> cams { get; set; }

        protected virtual void BuildArena()
        {
            var hits = gameObject.GetNearestSurfaces(500f);

            //Dev.LogVarArray($"{FSMName} -- ArenaHits", hits.Select(x => $"DIR:{x.Key} POINT:{x.Value.point}").ToList());

            xR = new Range(hits[Vector2.left].point.x, hits[Vector2.right].point.x);
            yR = new Range(hits[Vector2.down].point.y, hits[Vector2.up].point.y);

            if (heroX == null)
            {
                heroX = control.FsmVariables.GetFsmFloat("Hero X");
            }

            if (heroY == null)
            {
                heroY = control.FsmVariables.GetFsmFloat("Hero Y");
            }

            bounds = new Rect(gameObject.transform.position.x, gameObject.transform.position.y, xR.Size, yR.Size);

            SetupCustomDebugArea();

            UpdateRefs(control, FloatRefs);
        }

        protected virtual void UpdateRefs(PlayMakerFSM fsm, Dictionary<string, Func<FSMAreaControlEnemy, float>> refs)
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

        protected virtual void UpdateHeroRefs()
        {
            if (control == null)
                return;

            if (heroX != null && FloatRefs.ContainsKey("Hero X"))
                heroX.Value = FloatRefs["Hero X"].Invoke(this);

            if (heroY != null && FloatRefs.ContainsKey("Hero Y"))
                heroY.Value = FloatRefs["Hero Y"].Invoke(this);
        }

        protected virtual void Hide()
        {
            if (control.enabled)
            {
                control.enabled = false;
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
                BuildArena();

                if(OnShowControlBroadcastEvent != null)
                {
                    control.Fsm.BroadcastEvent(OnShowControlBroadcastEvent);
                }
            }

            CheckFSMsUsingHiddenStates();
        }

        protected virtual void CheckFSMsUsingHiddenStates()
        {
            if (FSMsUsingHiddenStates == null)
                return;

            foreach(var fsm in FSMsUsingHiddenStates)
            {
                if (!fsm.enabled)
                    continue;

                if(fsm.ActiveStateName == FSMHiddenStateName)
                {
                    fsm.SendEvent("SHOW");
                }
            }
        }

        protected virtual void CheckFSMsWithResetOnHideStates()
        {
            if (FSMsWithResetToStateOnHide == null)
                return;

            foreach (var fsmStatePair in FSMsWithResetToStateOnHide)
            {
                if (!fsmStatePair.Key.enabled)
                    continue;

                fsmStatePair.Key.SetState(fsmStatePair.Value);
            }
        }

        protected virtual bool HeroInAggroRange()
        {
            var size = new Vector2(xR.Size, yR.Size);
            var center = new Vector2(transform.position.x, transform.position.y);
            var herop = new Vector2(HeroX, HeroY);
            var dist = herop - center;
            return (dist.sqrMagnitude < size.sqrMagnitude);
        }

        protected virtual void SetupCustomDebugArea()
        {
            var size = new Vector2(xR.Size, yR.Size);
            var center = new Vector2(transform.position.x, transform.position.y);
            var herop = new Vector2(HeroX, HeroY);
            var dist = herop - center;
            debugColliders.customLineCollections.Add(Color.yellow,
                DebugColliders.GetPointsFromCollider(Vector2.one,center,dist.magnitude).Select(x => new Vector3(x.x, x.y, debugColliders.zDepth)).ToList());
            debugColliders.customLineCollections.Add(Color.magenta, new List<Vector3>() { 
            herop, center, herop
            });
            debugColliders.customLineCollections.Add(Color.cyan, debugColliders.GetPointsFromCollider(bounds, false).Select(x => new Vector3(x.x, x.y, debugColliders.zDepth)).ToList());
        }

        protected virtual IEnumerator Start()
        {
            BuildArena();

            if(ControlCameraLocks)
                cams = GetCameraLocksFromScene();

            if (control == null)
                yield break;

            PreloadRefs();

            if (!HeroInAggroRange())
                Hide();

            yield return UpdateAggroRange();
        }

        protected virtual IEnumerator UpdateAggroRange()
        {
            for (; ; )
            {
                UpdateHeroRefs();

                UpdateCustomRefs();

                if (HeroInAggroRange())
                    Show();
                else
                    Hide();

                yield return new WaitForSeconds(1f);
            }
        }

        protected virtual void PreloadRefs()
        {

        }

        protected virtual void UpdateCustomRefs()
        {

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

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
            if(control == null)
            {
                control = gameObject.LocateMyFSM(FSMName);
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

                if(createNewPreTransitionEvent)
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
    }
}

//fsm.AddTransition(hiddenStateName, "SHOW", "Arrive");
//preHideState.ChangeTransition("FINISHED", hiddenStateName);



//protected virtual void InsertHiddenState(PlayMakerFSM fsm, 
//    string preHideStateName, string preHideTransitionEventName, 
//    string postHideStateName)
//{
//    if (FSMsUsingHiddenStates == null)
//        FSMsUsingHiddenStates = new List<PlayMakerFSM>();

//    try
//    {
//        //var preHideState = fsm.GetState("Music?");
//        var preHideState = fsm.GetState(preHideStateName);
//        var hidden = fsm.AddState(FSMHiddenStateName);

//        fsm.AddVariable<FsmBool>("IsAggro");
//        var isAggro = fsm.FsmVariables.GetFsmBool("IsAggro");

//        //change the teleport she does to keep her hidden until a player is nearby
//        //preHideState.ChangeTransition("FINISHED", hiddenStateName);
//        preHideState.ChangeTransition(preHideTransitionEventName, FSMHiddenStateName);
//        //fsm.AddTransition(hiddenStateName, "SHOW", "Arrive");
//        fsm.AddTransition(FSMHiddenStateName, "SHOW", postHideStateName);
//        isAggro.Value = false;
//        hidden.AddAction(new BoolTest() { boolVariable = isAggro, isTrue = new FsmEvent("SHOW"), everyFrame = true });

//        FSMsUsingHiddenStates.Add(fsm);
//    }
//    catch (Exception e) { Dev.Log($"Error in adding an aggro range state to FSM:{fsm.name} PRE STATE:{preHideStateName} EVENT:{preHideTransitionEventName} POST-STATE:{postHideStateName}"); }
//}