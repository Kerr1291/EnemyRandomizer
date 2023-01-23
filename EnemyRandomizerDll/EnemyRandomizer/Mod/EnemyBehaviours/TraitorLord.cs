using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    public class TraitorLord : EnemyBehaviour
    {
        private const float GroundY = 6.0f;

        private PlayMakerFSM _mantis;
        
        private void Awake()
        {
            _mantis = gameObject.LocateMyFSM("Mantis");
        }

        private IEnumerator Start()
        {
            _mantis.SetState("Init");
            
            yield return new WaitUntil(() => _mantis.ActiveStateName == "Emerge Dust");

            GetComponent<HealthManager>().hasSpecialDeath = false;
            
            gameObject.transform.Find("Emerge Dust").GetComponent<ParticleSystem>().Stop();
            GetComponent<MeshRenderer>().enabled = true;
            GameCameras.instance.cameraShakeFSM.Fsm.GetFsmBool("RumblingMed").Value = false;
            
            _mantis.GetAction<FloatCompare>("DSlash").float2 = GroundY + 2f;
            _mantis.GetAction<SetPosition>("Land").y = GroundY + 2f;
            _mantis.SetState("Idle");
        }
    }
}