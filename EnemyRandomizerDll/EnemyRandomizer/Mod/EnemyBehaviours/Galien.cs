using UnityEngine;

namespace EnemyRandomizerMod.Behaviours
{
    public class Galien : EnemyBehaviour
    {
        private PlayMakerFSM _movement;

        private void Awake()
        {
            _movement = gameObject.LocateMyFSM("Movement");
        }

        private void Start()
        {
            _movement.Fsm.GetFsmVector3("P1").Value = RandomVector3();
            _movement.Fsm.GetFsmVector3("P2").Value = RandomVector3();
            _movement.Fsm.GetFsmVector3("P3").Value = RandomVector3();
            _movement.Fsm.GetFsmVector3("P4").Value = RandomVector3();
            _movement.Fsm.GetFsmVector3("P5").Value = RandomVector3();
            _movement.Fsm.GetFsmVector3("P6").Value = RandomVector3();
            _movement.Fsm.GetFsmVector3("P7").Value = RandomVector3();
        }

        private Vector3 RandomVector3()
        {
            float x = Random.Range(bounds.xMin, bounds.xMax);
            float y = Random.Range(bounds.yMin, bounds.yMax);
            float z = 0.006f;

            return new Vector3(x, y, z);
        }
    }
}