using System.Collections;
using System.Linq;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    public class NightmareKingGrimm : EnemyBehaviour
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
            _control.SetState("Init");
            
            yield return new WaitWhile(() => _control.ActiveStateName != "Dormant");
            
            _constrainX.Fsm.GetFsmFloat("Edge L").Value = bounds.xMin;
            _constrainX.Fsm.GetFsmFloat("Edge R").Value = bounds.xMax;

            _constrainY.GetAction<FloatCompare>("Check").float2.Value = bounds.yMin;
            _constrainY.GetAction<SetFloatValue>("Constrain").floatValue.Value = bounds.yMin;

            _control.Fsm.GetFsmFloat("Min X").Value = bounds.xMin;
            _control.Fsm.GetFsmFloat("Mid Y").Value = bounds.center.y;
            _control.Fsm.GetFsmFloat("Max X").Value = bounds.xMax;
            _control.Fsm.GetFsmFloat("Ground Y").Value = bounds.yMin + 2;
            _control.GetAction<FloatCompare>("Balloon Check").float2 = bounds.yMin + 3;
            _control.GetAction<SetPosition>("Balloon Pos").x = bounds.center.x;

            _control.GetState("HUD Canvas OUT").RemoveAction<SendEventByName>();

            _control.GetState("Death Start").InsertMethod(0, () =>
            {
                Destroy(FindObjectsOfType<GameObject>().First(go => go.name.Contains("Grimm Spike Holder")));
            });

            _control.SendEvent("TELE OUT");
        }
    }
}