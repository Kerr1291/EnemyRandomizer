using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using EnemyRandomizerMod;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Collections;
using System;
using HutongGames.PlayMaker;
using Modding;
using System.Linq;
using HutongGames.PlayMaker.Actions;
using Satchel;
using Satchel.Futils;

namespace EnemyRandomizerMod
{
    public class GhostWarriorMarmuControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Control";
    }

    public class GhostWarriorMarmuSpawner : DefaultSpawner<GhostWarriorMarmuControl>
    {
    }

    public class GhostWarriorMarmuPrefabConfig : DefaultPrefabConfig<GhostWarriorMarmuControl>
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);

            {
                var fsm = p.prefab.LocateMyFSM("Set Ghost PD Int");
                GameObject.Destroy(fsm);
            }

            {
                var fsm = p.prefab.LocateMyFSM("FSM");
                GameObject.Destroy(fsm);
            }
        }
    }
}
