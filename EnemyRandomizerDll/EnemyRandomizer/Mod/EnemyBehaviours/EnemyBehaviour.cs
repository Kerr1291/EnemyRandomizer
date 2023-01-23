using HutongGames.PlayMaker;
using UnityEngine;

namespace EnemyRandomizerMod.Behaviours
{
    public class EnemyBehaviour : MonoBehaviour
    {
        public Rect bounds;
        public HealthManager _hm;
        public PlayMakerFSM _fsm;

        private void Awake()
        {
            _hm = GetComponent<HealthManager>();
            _fsm = GetComponent<PlayMakerFSM>();
        }

        public virtual void AdjustPosition()
        {

        }
    }
}
