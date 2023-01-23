using System.Collections;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    public class GreatNailsageSly : EnemyBehaviour
    {
        private PlayMakerFSM _control;

        private void Awake()
        {
            _control = gameObject.LocateMyFSM("Control");
        }

        private IEnumerator Start()
        {
            _control.SetState("Init");

            _control.Fsm.GetFsmBool("Final Rage").Value = true;
            
            _control.GetAction<FloatCompare>("Cyc Down").float2.Value = bounds.yMin + 4;
            _control.GetAction<FloatOperator>("Cyc Jump Launch").float1.Value = bounds.center.x;
            _control.GetAction<SetFloatValue>("Jump To L", 0).floatValue.Value = bounds.xMax - 8;
            _control.GetAction<SetFloatValue>("Jump To L", 1).floatValue.Value = bounds.xMin;
            _control.GetAction<SetFloatValue>("Jump To R", 0).floatValue.Value = bounds.xMin + 8;
            _control.GetAction<SetFloatValue>("Jump To R", 1).floatValue.Value = bounds.xMax;
            
            _control.GetState("Bow").InsertMethod(0, () => Destroy(gameObject, 3));
            _control.GetState("Stun Wait").AddMethod(() => _control.SendEvent("READY"));
            _control.GetState("Grabbing").AddMethod(() => _control.SendEvent("GRABBED"));
            
            _control.GetState("Cyclone Start").RemoveAction<SetPolygonCollider>();
            _control.GetState("Cyclone Start").RemoveAction(8);
            _control.GetState("Cyclone End").RemoveAction<SetPolygonCollider>();
            _control.GetState("Stun Reset").RemoveAction<SetPolygonCollider>();
            _control.GetState("Death Reset").RemoveAction<SetPolygonCollider>();
            
            yield return new WaitWhile(() => _control.ActiveStateName != "Docile");

            GameObject spinTink = new GameObject("Spin Tink");
            var collider = spinTink.AddComponent<CircleCollider2D>();
            spinTink.AddComponent<DamageHero>();
            collider.isTrigger = true;
            collider.radius = 3;
            spinTink.transform.SetParent(gameObject.transform, false);
            _control.Fsm.GetFsmGameObject("Spin Tink").Value = spinTink;

            _control.SetState("Battle Start");
        }

        private void Update()
        {
            Modding.Logger.Log("[Sly] " + _control.ActiveStateName);
        }
    }
}