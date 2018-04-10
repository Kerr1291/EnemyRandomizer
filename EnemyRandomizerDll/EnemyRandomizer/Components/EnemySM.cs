using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using nv.Tests;
#endif

namespace nv
{
    public class EnemySM : Physics2DSM
    {
        public MeshRenderer meshRenderer;
        public tk2dSpriteAnimator tk2dAnimator;
        public HealthManager healthManager;


        public bool wasHitRecently;
        public int ignoreDamageDeltaUnder = 5;
        public Action<int> OnHit { get; set; }

        public int maxHP = 225;

        protected int previousHP = 0;

        public override bool Running {
            get {
                return healthManager.hp > 0 && !healthManager.isDead;
            }

            set {
                gameObject.SetActive( value );
            }
        }

        protected override void SetupRequiredReferences()
        {
            base.SetupRequiredReferences();
            meshRenderer = GetComponent<MeshRenderer>();
            tk2dAnimator = GetComponent<tk2dSpriteAnimator>();
            healthManager = GetComponent<HealthManager>();

            healthManager.hp = maxHP;
            previousHP = maxHP;

            OnHit -= DefaultOnHit;
            OnHit += DefaultOnHit;
        }

        protected virtual void OnCollisionStay2D( Collision2D collision )
        {
            if( collision.gameObject.layer == collisionLayer )
            {
                CheckTouching( collisionLayer );
            }
        }

        protected override void Update()
        {
            base.Update();
            if( healthManager.hp != previousHP && healthManager.hp < previousHP )
            {
                int damage = previousHP - healthManager.hp;
                if( damage >= ignoreDamageDeltaUnder )
                {
                    if(OnHit != null)
                        OnHit.Invoke(damage);
                }
            }

            previousHP = healthManager.hp;
        }

        protected virtual void DefaultOnHit( int damage )
        {
            wasHitRecently = true;
        }
    }
}