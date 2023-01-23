using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    public class Xero : EnemyBehaviour
    {
        private PlayMakerFSM _movement;
        private PlayMakerFSM _yLimit;

        private void Awake()
        {
            _movement = gameObject.LocateMyFSM("Movement");
            _yLimit = gameObject.LocateMyFSM("Y Limit");
        }

        private void Start()
        {
            for (int i = 1; i <= 4; i++)
            {
                gameObject.transform.Find("Sword " + i).gameObject.LocateMyFSM("xero_nail").SetState("Init");
            }
            
            _movement.Fsm.GetFsmVector3("P1").Value = RandomVector3();
            _movement.Fsm.GetFsmVector3("P2").Value = RandomVector3();
            _movement.Fsm.GetFsmVector3("P3").Value = RandomVector3();
            _movement.Fsm.GetFsmVector3("P4").Value = RandomVector3();
            _movement.Fsm.GetFsmVector3("P5").Value = RandomVector3();
            _movement.Fsm.GetFsmVector3("P6").Value = RandomVector3();
            _movement.Fsm.GetFsmVector3("P7").Value = RandomVector3();

            _yLimit.GetAction<FloatClamp>("Limit").maxValue = bounds.center.y;
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