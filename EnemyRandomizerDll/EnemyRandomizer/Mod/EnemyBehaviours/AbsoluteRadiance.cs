using System.Collections;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    class AbsoluteRadiance : EnemyBehaviour
    {
        private PlayMakerFSM _commands;
        private PlayMakerFSM _control;

        private void Awake()
        {
            _commands = gameObject.LocateMyFSM("Attack Commands");
            _control = gameObject.LocateMyFSM("Control");
        }

        private IEnumerator Start()
        {
            _commands.Fsm.GetFsmFloat("Orb Max X").Value = bounds.xMax - 1;
            _commands.Fsm.GetFsmFloat("Orb Max Y").Value = bounds.yMax - 1;
            _commands.Fsm.GetFsmFloat("Orb Min X").Value = bounds.xMin + 1;
            _commands.Fsm.GetFsmFloat("Orb Min Y").Value = bounds.yMin + 3;

            GameObject comb = _commands.GetAction<SpawnObjectFromGlobalPool>("Comb Top").gameObject.Value;
            comb.transform.position = new Vector3(bounds.center.x, bounds.center.y, 0.006f);

            PlayMakerFSM combControl = comb.LocateMyFSM("Control");
            combControl.GetAction<SetPosition>("TL").x = bounds.xMin;
            combControl.GetAction<SetPosition>("TR").x = bounds.xMax;
            combControl.GetAction<RandomFloat>("Top").min = bounds.center.x - 1;
            combControl.GetAction<RandomFloat>("Top").max = bounds.center.x + 1;
            combControl.GetAction<SetPosition>("Top").y = bounds.yMax;
            combControl.GetAction<SetPosition>("L").x = bounds.xMin;
            combControl.GetAction<SetPosition>("L").y = bounds.center.y;
            combControl.GetAction<SetPosition>("R").x = bounds.xMax;
            combControl.GetAction<SetPosition>("R").y = bounds.center.y;
            
            _commands.GetAction<SpawnObjectFromGlobalPool>("Comb Top").gameObject = comb;
            _commands.GetAction<SpawnObjectFromGlobalPool>("Comb L").gameObject = comb;
            _commands.GetAction<SpawnObjectFromGlobalPool>("Comb R").gameObject = comb;

            _control.Fsm.GetFsmFloat("A1 X Max").Value = bounds.xMax - 2;
            _control.Fsm.GetFsmFloat("A1 X Min").Value = bounds.xMin + 2;

            _control.GetAction<RandomFloat>("Set Dest", 4).min = transform.position.y - 1;
            _control.GetAction<RandomFloat>("Set Dest", 4).max = transform.position.y + 1;
            _control.GetAction<RandomFloat>("Set Dest 2", 4).min = transform.position.y - 1;
            _control.GetAction<RandomFloat>("Set Dest 2", 4).max = transform.position.y + 1;
            _control.GetAction<SetFsmVector3>("First Tele").setValue = transform.position;
            _control.GetAction<SetFsmVector3>("Rage1 Tele").setValue = transform.position;
            _control.GetState("Climb Plats").InsertMethod(0, () =>
            {
                Destroy(gameObject);
            });

            _commands.SetState("Init");
            _control.SetState("Init");

            yield return new WaitUntil(() => _control.ActiveStateName == "Intro End");

            _control.SetState("Arena 1 Idle");
        }
    }
}
