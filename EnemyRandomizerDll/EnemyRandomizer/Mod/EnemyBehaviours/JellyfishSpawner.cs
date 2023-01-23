using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    public class JellyfishSpawner : EnemyBehaviour
    {
        private PlayMakerFSM _spawn;

        private void Awake()
        {
            _spawn = gameObject.LocateMyFSM("Spawn");
        }

        private void Start()
        {
            _spawn.GetAction<RandomFloat>("Spawn", 0).min.Value = bounds.xMin + 2;
            _spawn.GetAction<RandomFloat>("Spawn", 0).max.Value = bounds.xMax - 2;
            _spawn.GetAction<SetPosition>("Spawn", 2).y.Value = bounds.yMin - 5;
            
            _spawn.GetAction<RandomFloat>("Spawn", 3).min.Value = bounds.xMin + 2;
            _spawn.GetAction<RandomFloat>("Spawn", 3).max.Value = bounds.xMax - 2;
            _spawn.GetAction<SetPosition>("Spawn", 5).y.Value = bounds.yMin - 8;
            
            _spawn.GetAction<RandomFloat>("Spawn", 6).min.Value = bounds.xMin + 2;
            _spawn.GetAction<RandomFloat>("Spawn", 6).max.Value = bounds.xMax - 2;
            _spawn.GetAction<SetPosition>("Spawn", 8).y.Value = bounds.yMin - 11;
            
            _spawn.GetAction<RandomFloat>("Spawn", 9).min.Value = bounds.xMin + 2;
            _spawn.GetAction<RandomFloat>("Spawn", 9).max.Value = bounds.xMax - 2;
            _spawn.GetAction<SetPosition>("Spawn", 11).y.Value = bounds.yMin - 14;

            _spawn.SetState("Init");
        }
    }
}