using System.Collections;
using UnityEngine;

namespace EnemyRandomizerMod.Behaviours
{
    public class VoltTwister : EnemyBehaviour
    {
        private PlayMakerFSM _mage;

        private void Awake()
        {
            _mage = gameObject.LocateMyFSM("Electric Mage");
        }

        private IEnumerator Start()
        {
            _mage.SetState("Init");

            yield return null;
            
            GetComponent<BoxCollider2D>().enabled = true;
            GetComponent<MeshRenderer>().enabled = true;
            _mage.SetState("Idle");
        }
        
        private void Update()
        {
            Modding.Logger.Log("[Eletric Mage] " + _mage.ActiveStateName);
        }
    }
}