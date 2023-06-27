using UnityEngine;

namespace EnemyRandomizerMod
{
    public class FSMWaker : MonoBehaviour
    {
        public PlayMakerFSM fsm;
        public string wakeState = "Sleep";
        public string wakeString = "WAKE";

        protected virtual void OnEnable()
        {
            fsm = gameObject.LocateMyFSM("Control");//default
        }

        protected virtual void Update()
        {
            if(fsm != null)
            {
                if(fsm.ActiveStateName == wakeState)
                {
                    fsm.SendEvent(wakeString);
                }
            }
        }
    }
}
