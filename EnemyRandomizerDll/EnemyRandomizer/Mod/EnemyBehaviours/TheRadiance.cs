using System.Collections;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    public class TheRadiance : EnemyBehaviour
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
            _commands.Fsm.GetFsmFloat("Orb Max X").Value = bounds.xMax - 3;
            _commands.Fsm.GetFsmFloat("Orb Max Y").Value = bounds.yMax - 3;
            _commands.Fsm.GetFsmFloat("Orb Min X").Value = bounds.xMin + 3;
            _commands.Fsm.GetFsmFloat("Orb Min Y").Value = bounds.yMin + 3;
            
            _control.Fsm.GetFsmFloat("A1 X Max").Value = bounds.xMax;
            _control.Fsm.GetFsmFloat("A1 X Min").Value = bounds.xMin;

            _control.GetAction<RandomFloat>("Set Dest", 4).min.Value = bounds.yMin + 5;
            _control.GetAction<RandomFloat>("Set Dest", 4).max.Value = bounds.yMin + 5.2f;

            _control.GetAction<SetFsmVector3>("First Tele").setValue.Value = new Vector3(bounds.center.x, bounds.yMin + 5, 0.006f);
            
            yield return null;
        }
    }
}