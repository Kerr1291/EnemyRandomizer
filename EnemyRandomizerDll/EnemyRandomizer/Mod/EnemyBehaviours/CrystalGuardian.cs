using System.Collections;
using UnityEngine;

namespace EnemyRandomizerMod.Behaviours
{
    public class CrystalGuardian : EnemyBehaviour
    {
        private PlayMakerFSM _miner;

        private void Awake()
        {
            _miner = gameObject.LocateMyFSM("Beam Miner");
        }

        private IEnumerator Start()
        {
            _miner.SetState("Battle Init");

            _miner.Fsm.GetFsmFloat("Jump Max X").Value = bounds.xMin;
            _miner.Fsm.GetFsmFloat("Jump Min X").Value = bounds.xMax;
            
            yield return null;
        }
    }
}