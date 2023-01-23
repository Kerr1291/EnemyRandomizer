using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    public class HornetProtector : EnemyBehaviour
    {
        private PlayMakerFSM _control;

        private void Awake()
        {
            GetComponent<BoxCollider2D>().enabled = true;
            Destroy(gameObject.LocateMyFSM("Stun Control"));
            _control = gameObject.LocateMyFSM("Control");
        }

        private void Start()
        {
            _control.SetState("Pause");

            _control.GetState("Music").RemoveAction<TransitionToAudioSnapshot>();
            _control.GetState("Music").RemoveAction<ApplyMusicCue>();

            _control.Fsm.GetFsmFloat("Air Dash Height").Value = bounds.yMin + 4;
            _control.Fsm.GetFsmFloat("Floor Y").Value = bounds.yMin;
            _control.Fsm.GetFsmFloat("Left X").Value = bounds.xMin;
            _control.Fsm.GetFsmFloat("Min Dstab Height").Value = bounds.yMin + 6;
            _control.Fsm.GetFsmFloat("Right X").Value = bounds.xMax;
            _control.Fsm.GetFsmFloat("Roof Y").Value = bounds.yMax;
            _control.Fsm.GetFsmFloat("Sphere Y").Value = bounds.yMin + 6;
            _control.Fsm.GetFsmFloat("Throw X L").Value = bounds.xMin + 6.5f;
            _control.Fsm.GetFsmFloat("Throw X R").Value = bounds.xMax - 6.5f;
            _control.Fsm.GetFsmFloat("Wall X Left").Value = bounds.xMin - 1;
            _control.Fsm.GetFsmFloat("Wall X Right").Value = bounds.xMax + 1;

            _control.GetAction<BoolTestMulti>("Can Throw?", 4).boolVariables[0] = false;
            _control.GetAction<BoolTestMulti>("Can Throw?", 5).boolVariables[0] = false;

            var constrainPos = gameObject.GetComponent<ConstrainPosition>();
            constrainPos.constrainX = constrainPos.constrainY = false;
        }
    }
}