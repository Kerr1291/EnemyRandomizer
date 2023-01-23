using System.Collections;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    public class MossKnight : EnemyBehaviour
    {
        private PlayMakerFSM _control;

        private void Awake()
        {
            _control = gameObject.LocateMyFSM("Moss Knight Control");
        }

        private IEnumerator Start()
        {
            _control.Fsm.GetFsmBool("Dormant").Value = false;
            _control.GetAction<BoolTest>("Initialise", 17).isTrue = null;
            
            _control.SetState("Pause Frame");

            yield return new WaitUntil(() => _control.ActiveStateName == "Detect");

            GetComponent<BoxCollider2D>().enabled = true;
            GetComponent<NonBouncer>().SetActive(false);
            GetComponent<HealthManager>().SetPreventInvincibleEffect(false);
            GetComponent<DamageHero>().damageDealt = 1;
            _control.SetState("Shield Start");
        }
    }
}