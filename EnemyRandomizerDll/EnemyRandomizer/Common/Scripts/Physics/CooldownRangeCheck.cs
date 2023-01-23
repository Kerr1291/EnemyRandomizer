using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nv
{
    public class CooldownRangeCheck : RangeCheck
    {
        public bool IsOnCooldown { get; private set; }

        public float onTimeMin = 1f;
        public float onTimeMax = 2f;

        public float offTimeMin = 2f;
        public float offTimeMax = 3f;

        BoxCollider2D bodyCollider;

        IEnumerator currentState = null;

        public void DisableForTime(float disableTime)
        {
            if(IsOnCooldown)
                return;

            StartCoroutine(EnableAfterTime(disableTime));
        }

        IEnumerator EnableAfterTime(float time)
        {
            IsOnCooldown = true;
            if(bodyCollider == null)
                bodyCollider = GetComponent<BoxCollider2D>();
            bodyCollider.enabled = false;
            yield return new WaitForSeconds(time);
            bodyCollider.enabled = true;
            IsOnCooldown = false;
        }

        IEnumerator Start()
        {
            bodyCollider = GetComponent<BoxCollider2D>();
            currentState = On();

            while(IsOnCooldown)
            {
                yield return new WaitForEndOfFrame();
            }

            yield return currentState;
        }

        IEnumerator On()
        {
            float time = RNG.pRNG.Rand(onTimeMin, onTimeMax);
            yield return new WaitForSeconds(time);
            bodyCollider.enabled = true;
            currentState = Off();
            yield break;
        }

        IEnumerator Off()
        {
            float time = RNG.pRNG.Rand(offTimeMin, offTimeMax);
            yield return new WaitForSeconds(time);
            ObjectIsInRange = false;
            bodyCollider.enabled = false;
            currentState = On();
            yield break;
        }
    }
}