using System.Collections;
using System.Linq;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    public class TroupeMasterGrimm : EnemyBehaviour
    {
        private PlayMakerFSM _constrainX;
        private PlayMakerFSM _constrainY;
        private PlayMakerFSM _control;

        private void Awake()
        {
            _constrainX = gameObject.LocateMyFSM("constrain_x");
            _constrainY = gameObject.LocateMyFSM("Constrain Y");
            _control = gameObject.LocateMyFSM("Control");
        }

        private IEnumerator Start()
        {
            _constrainX.Fsm.GetFsmFloat("Edge L").Value = bounds.xMin + 1;
            _constrainX.Fsm.GetFsmFloat("Edge R").Value = bounds.xMax - 1;

            _constrainY.GetAction<FloatCompare>("Check").float2.Value = bounds.yMin;
            _constrainY.GetAction<SetFloatValue>("Constrain").floatValue.Value = bounds.yMin;

            _control.Fsm.GetFsmFloat("AD Max X").Value = bounds.xMax - 1;
            _control.Fsm.GetFsmFloat("AD Min X").Value = bounds.xMin + 5;
            _control.Fsm.GetFsmFloat("Ground Y").Value = bounds.yMin + 2;
            _control.Fsm.GetFsmFloat("Max X").Value = bounds.xMax;
            _control.Fsm.GetFsmFloat("Min X").Value = bounds.xMin;

            _control.GetAction<SetPosition>("Balloon Pos").x = bounds.center.x;
            _control.GetAction<SetPosition>("Balloon Pos").y = 14.0f;

            _control.GetState("Death Start").InsertMethod(0, () =>
            {
                Destroy(FindObjectsOfType<GameObject>().First(go => go.name.Contains("Grimm Spike Holder")));
            });

            _control.GetState("Bow").RemoveAction<ApplyMusicCue>();
            _control.GetState("Bow").RemoveAction<TransitionToAudioSnapshot>();

            _control.SetState("Init");

            yield return new WaitUntil(() => _control.ActiveStateName == "Bow");

            _control.SetState("Tele Out");
        }
    }
}