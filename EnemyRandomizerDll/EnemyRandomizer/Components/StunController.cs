using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nv
{
    public class StunController : MonoBehaviour
    {
        public HealthManager healthManager;
        public Action onStun;

        public bool isSuspended = false;

        public int hitsToStun = 10;
        public int maxStuns = 5;
        public int stuns = 0;

        //temporary patch to not having an 'onhit' hook, ignore any damage under this amount
        public int ignoreDamageDeltaUnder = 5;

        int hitsTaken = 0;
        int previousHP = 0;

        void OnHit( int damageTaken )
        {
            hitsTaken++;
        }

        void OnEnable()
        {
            healthManager = GetComponent<HealthManager>();

            hitsTaken = 0;
            stuns = 0;
            previousHP = healthManager.hp;

            StartCoroutine( MainAILoop() );
        }

        public void SetSuspend( bool suspended )
        {
            isSuspended = suspended;
        }

        //TODO: add a hook for when a health manager takes a hit, for now just poll the difference in hp
        //the side effect of this is that things that damage enemies but aren't player hits may cause stuns depending on the value of ignoreDamageDeltaUnder
        private void Update()
        {
            if( healthManager.hp != previousHP && healthManager.hp < previousHP )
            {
                int damage = previousHP - healthManager.hp;
                if( damage >= ignoreDamageDeltaUnder )
                    OnHit( damage );
            }

            previousHP = healthManager.hp;
        }

        IEnumerator MainAILoop()
        {
            Dev.Where();

            for(; ; )
            {
                yield return new WaitForEndOfFrame();

                if( stuns >= maxStuns )
                    break;

                if( isSuspended )
                    continue;

                if( hitsTaken < hitsToStun )
                    continue;

                yield return Stun();
            }

            this.enabled = false;
        }

        IEnumerator Stun()
        {
            Dev.Where();

            stuns++;
            hitsTaken = 0;
            onStun?.Invoke();

            yield break;
        }
    }
}
