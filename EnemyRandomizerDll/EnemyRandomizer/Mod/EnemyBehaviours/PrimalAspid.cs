using UnityEngine;

namespace EnemyRandomizerMod.Behaviours
{
    public class PrimalAspid : EnemyBehaviour
    {
        private PlayMakerFSM _fsm;
        
        private void Awake()
        {
            _fsm = GetComponent<PlayMakerFSM>();
        }

        private void Start()
        {
            _fsm.SetState("Init");
        }
    }
}