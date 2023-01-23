using System.Collections;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    class Sibling : EnemyBehaviour
    {
        private PlayMakerFSM _control;

        private void Awake()
        {
            _control = gameObject.LocateMyFSM("Control");
        }

        private IEnumerator Start()
        {

            //_control.GetState("Destroy").InsertMethod(0, () => ColosseumManager.EnemyCount--);

            _control.SetState("Pause");

            yield return new WaitUntil(() => _control.ActiveStateName == "Idle");
            GetComponent<DamageHero>().damageDealt = 2;
            GetComponent<HealthManager>().hp = 20;
            transform.Find("Alert Range").gameObject.transform.localScale *= 2;
            _control.Fsm.GetFsmBool("Friendly").Value = false;
        }
    }
}
