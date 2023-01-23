using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    public class ElderHu : EnemyBehaviour
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

            _movement.GetAction<FloatCompare>("Choose L").float2 = bounds.center.x - 5;
            _movement.GetAction<FloatCompare>("Choose R").float2 = bounds.center.x + 5;
            _movement.GetAction<FloatCompare>("Set Warp").float2 = bounds.center.x;
            _movement.GetAction<SetVector3XYZ>("Choose L").x = bounds.xMin + 2;
            _movement.GetAction<SetVector3XYZ>("Choose L").y = transform.position.y;
            _movement.GetAction<SetVector3XYZ>("Choose R").x = bounds.xMax - 2;
            _movement.GetAction<SetVector3XYZ>("Choose R").y = transform.position.y;
            _movement.GetAction<SetPosition>("Return").x = bounds.center.x;
            _movement.GetAction<SetPosition>("Return").y = bounds.center.y;
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