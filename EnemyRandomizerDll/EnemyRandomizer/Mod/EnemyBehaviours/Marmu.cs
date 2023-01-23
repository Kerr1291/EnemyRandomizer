using UnityEngine;

namespace EnemyRandomizerMod.Behaviours
{
    public class Marmu : EnemyBehaviour
    {
        private PlayMakerFSM _control;

        private void Awake()
        {
            _control = gameObject.LocateMyFSM("Control");
        }

        private void Start()
        {
            _control.Fsm.GetFsmFloat("Tele X Max").Value = bounds.xMax - 3;
            _control.Fsm.GetFsmFloat("Tele X Min").Value = bounds.xMin + 3;
            _control.Fsm.GetFsmFloat("Tele Y Max").Value = bounds.yMax - 3;
            _control.Fsm.GetFsmFloat("Tele Y Min").Value = bounds.yMin + 3;
        }
    }
}