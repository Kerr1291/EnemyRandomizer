using System.Collections;
using UnityEngine;

namespace EnemyRandomizerMod.Behaviours
{
    public class Flukemarm : EnemyBehaviour
    {
        private PlayMakerFSM _mother;
        
        private void Awake()
        {
            _mother = gameObject.LocateMyFSM("Fluke Mother");
        }

        private IEnumerator Start()
        {
            _mother.SetState("Init");

            //GameObject hatcherCage = Instantiate(CustomTrial.GameObjects["Hatcher Cage"], transform.position, Quaternion.identity);
            //hatcherCage.SetActive(true);
            //foreach (var collider in hatcherCage.GetComponents<BoxCollider2D>())
            //{
            //    Destroy(collider);
            //}

            yield return new WaitUntil(() => _mother.ActiveStateName == "Idle");

            _mother.SetState("Roar End");
        }
    }
}