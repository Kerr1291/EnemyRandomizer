using System.Collections;
using UnityEngine;

namespace EnemyRandomizerMod.Behaviours
{
    public class PaintmasterSheo : EnemyBehaviour
    {
        private PlayMakerFSM _sheo;

        private void Awake()
        {
            _sheo = gameObject.LocateMyFSM("nailmaster_sheo");

            GetComponent<HealthManager>().OnDeath += OnDeath;
        }

        private IEnumerator Start()
        {
            _sheo.SetState("Init");

            yield return new WaitWhile(() => _sheo.ActiveStateName != "Painting");

            _sheo.SetState("Battle Start");
        }

        private void OnDeath()
        {
            Destroy(gameObject, 8);
        }
    }
}