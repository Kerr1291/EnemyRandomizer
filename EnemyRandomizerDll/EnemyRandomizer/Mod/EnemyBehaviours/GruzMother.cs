using System.Collections;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    public class GruzMother : EnemyBehaviour
    {
        private PlayMakerFSM _control;
        
        private void Awake()
        {
            _control = gameObject.LocateMyFSM("Big Fly Control");
            
        }

        public override void AdjustPosition()
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 6, transform.position.z);
        }

        private IEnumerator Start()
        {
            _control.GetState("Fly").RemoveAction<ApplyMusicCue>();
            
            _control.SetState("Init");

            GetComponent<HealthManager>().IsInvincible = false;

            yield return new WaitWhile(() => _control.ActiveStateName != "Invincible" && _control.ActiveStateName != "Wake");

            _control.SetState("Fly");
        }
    }
}