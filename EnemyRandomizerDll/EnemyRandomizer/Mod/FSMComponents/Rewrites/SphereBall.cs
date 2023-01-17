using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using nv;

namespace EnemyRandomizerMod
{
    public class SphereBall : MonoBehaviour
    {
        public tk2dSpriteAnimator tk2dAnimator;

        public GameObject parent;
        IEnumerator currentState = null;

        public bool isAnimating = false;

        float sphereTime;
        float sphereStartSize;
        float sphereEndSize;

        public void Play( GameObject parent, float sphereTime, float sphereStartSize, float sphereEndSize )
        {
            this.parent = parent;
            tk2dAnimator = GetComponent<tk2dSpriteAnimator>();

            gameObject.SetActive( true );

            isAnimating = true;

            this.sphereTime = sphereTime;
            this.sphereStartSize = sphereStartSize;
            this.sphereEndSize = sphereEndSize;

            transform.localScale = new Vector3( sphereStartSize, sphereStartSize, 0f );

            StartCoroutine( MainAILoop() );
        }

        public void Stop()
        {
            isAnimating = false;
            currentState = null;
            gameObject.SetActive( false );
        }

        IEnumerator MainAILoop()
        {
            Dev.Where();
            currentState = Grow();

            for(; ; )
            {
                if( parent == null )
                    yield break;

                yield return currentState;
            }
        }

        IEnumerator Grow()
        {
            Dev.Where();

            Vector3 targetScale = new Vector3(sphereEndSize,sphereEndSize,1f);
            Vector3 velocity = Vector3.zero;

            float time = 0f;
            while( time < sphereTime )
            {
                Vector3 scale = transform.localScale;
                scale = Vector3.SmoothDamp( scale, targetScale, ref velocity, sphereTime );
                transform.localScale = scale;
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            currentState = Complete();

            yield break;
        }

        IEnumerator Complete()
        {
            Dev.Where();

            Stop();

            yield break;
        }
    }
}
