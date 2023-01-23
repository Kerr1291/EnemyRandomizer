using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    public class TheHollowKnight : EnemyBehaviour
    {
        private PlayMakerFSM _control;
        private PlayMakerFSM _phaseCtrl;

        private void Awake()
        {
            _control = gameObject.LocateMyFSM("Control");
            _phaseCtrl = gameObject.LocateMyFSM("Phase Control");
        }

        private void Start()
        {
            _control.Fsm.GetFsmFloat("Left X").Value = bounds.xMin;
            _control.Fsm.GetFsmFloat("Right X").Value = bounds.xMax;
            _control.Fsm.GetFsmFloat("TeleRange Max").Value = bounds.xMax - 1;
            _control.Fsm.GetFsmFloat("TeleRange Min").Value = bounds.xMin + 4;
            
            _control.GetAction<FloatInRange>("Pos", 1).lowerValue.Value = bounds.center.x - 3;
            _control.GetAction<FloatInRange>("Pos", 1).upperValue.Value = bounds.center.x;
            _control.GetAction<FloatInRange>("Pos", 2).lowerValue.Value = bounds.center.x;
            _control.GetAction<FloatInRange>("Pos", 2).upperValue.Value = bounds.center.x + 3;
            _control.GetAction<SetPosition>("Pos").x.Value = bounds.center.x;
            _control.GetAction<FloatInRange>("Pos 2", 1).lowerValue.Value = bounds.center.x - 3;
            _control.GetAction<FloatInRange>("Pos 2", 1).upperValue.Value = bounds.center.x;
            _control.GetAction<FloatInRange>("Pos 2", 2).lowerValue.Value = bounds.center.x;
            _control.GetAction<FloatInRange>("Pos 2", 2).upperValue.Value = bounds.center.x + 3;
            _control.GetAction<SetPosition>("Pos 2").x.Value = bounds.center.x;
            _control.GetAction<SetPosition>("Shift L").x.Value = bounds.center.x - 3;
            _control.GetAction<SetPosition>("Shift L 2").x.Value = bounds.center.x - 3;
            _control.GetAction<SetPosition>("Shift R").x.Value = bounds.center.x + 3;
            _control.GetAction<SetPosition>("Shift R 2").x.Value = bounds.center.x + 3;
            _control.GetAction<FloatClamp>("TelePos Dstab").minValue.Value = bounds.xMin + 4;
            _control.GetAction<FloatClamp>("TelePos Dstab").maxValue.Value = bounds.xMax - 1;
            _control.GetAction<SetPosition>("TelePos Dstab").y.Value = bounds.yMin + 6;
            
            _control.GetState("Roar").RemoveAction<SendEventByName>();
            _control.GetState("Roar").RemoveAction<SetFsmGameObject>();
            _control.GetState("HK Decline Music").RemoveAction<ApplyMusicCue>();
            _control.GetState("Long Roar").RemoveAction<SendEventByName>();
            _control.GetState("Long Roar").RemoveAction<SetFsmGameObject>();
            _control.GetState("Long Roar End").RemoveAction<PlayerDataBoolTest>();

            _phaseCtrl.GetState("Set Phase 4").RemoveAction<PlayerDataBoolTest>();
            _phaseCtrl.GetState("HK DECLINE 2").RemoveAction<TransitionToAudioSnapshot>();
            _phaseCtrl.GetState("HK DECLINE 3").RemoveAction<TransitionToAudioSnapshot>();

            GameObject corpse = gameObject.transform.Find("Boss Corpse").gameObject;
            corpse.LocateMyFSM("Corpse").GetState("Burst").RemoveAction<SendEventByName>();
            corpse.LocateMyFSM("Corpse").GetState("Blow").InsertMethod(9, () =>
            {
                GameCameras.instance.StopCameraShake();
                Destroy(corpse);
                Destroy(gameObject);
            });

            _control.SetState("Init");
        }
    }
}