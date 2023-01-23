using System.Collections;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    public class Grimmkin : EnemyBehaviour
    {
        public int grimmchildLevel;

        private Vector3 _offset;

        private PlayMakerFSM _control;

        public void Awake()
        {
            _control = gameObject.LocateMyFSM("Control");
        }

        private IEnumerator Start()
        {
            _control.Fsm.GetFsmInt("Grimmchild Level").Value = grimmchildLevel;

            _control.GetState("Follow").InsertMethod(8, () => _offset = HeroController.instance.transform.position - transform.position);
            //_control.GetState("Death Start").InsertMethod(0, () => ColosseumManager.EnemyCount--);
            
            _control.GetState("Explode").RemoveAction<ApplyMusicCue>();
            _control.GetState("Music").RemoveAction<ApplyMusicCue>();
            _control.GetState("Follow").RemoveAction<DistanceFlySmooth>();
            _control.GetState("Init").RemoveAction<GetPlayerDataInt>();

            _control.SetState("Init");

            yield return new WaitWhile(() => _control.ActiveStateName != "Init");

            _control.SendEvent("START");
            
        }

        private void FixedUpdate()
        {
            if (_control.ActiveStateName == "Follow")
            {
                transform.SetPosition2D(HeroController.instance.transform.position - _offset);
            }
        }

        private void Log(object message) => Modding.Logger.Log("[Grimmkin] " + message);
    }
}