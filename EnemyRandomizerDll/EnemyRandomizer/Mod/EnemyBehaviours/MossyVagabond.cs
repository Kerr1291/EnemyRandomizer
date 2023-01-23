using System.Collections;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    public class MossyVagabond : EnemyBehaviour
    {
        private PlayMakerFSM _control;

        private void Awake()
        {
            Destroy(gameObject.LocateMyFSM("FSM"));
        }
    }
}