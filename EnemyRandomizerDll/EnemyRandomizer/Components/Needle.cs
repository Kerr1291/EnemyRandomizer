using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nv
{
    public class Needle : MonoBehaviour
    {
        public tk2dSpriteAnimator tk2dAnimator;
        public PolygonCollider2D bodyCollider;
        public Rigidbody2D body;
        public MeshRenderer meshRenderer;

        public GameObject owner;
        public GameObject thread;
        IEnumerator currentState = null;

        public bool isAnimating = false;

        float startDelay;
        float throwMaxTravelTime;
        Ray throwRay;
        float throwDistance;
        float needleYOffset = -.35f;
        Vector3 startPos;

        void Awake()
        {
            bodyCollider = gameObject.GetComponent<PolygonCollider2D>();
            bodyCollider.offset = new Vector2( 0f, -.3f );
        }

        public void Play( GameObject owner, float startDelay, float throwMaxTravelTime, Ray throwRay, float throwDistance )
        {
            this.owner = owner;
            tk2dAnimator = gameObject.GetComponent<tk2dSpriteAnimator>();
            bodyCollider = gameObject.GetComponent<PolygonCollider2D>();
            body = gameObject.GetComponent<Rigidbody2D>();
            meshRenderer = gameObject.GetComponent<MeshRenderer>();

            meshRenderer.enabled = false;
            startPos = throwRay.origin + new Vector3( 0f, needleYOffset, 0f );
            transform.position = startPos;
            gameObject.SetActive( true );

            thread = gameObject.FindGameObjectInChildren( "Thread" );

            isAnimating = true;

            this.startDelay = startDelay;
            this.throwMaxTravelTime = throwMaxTravelTime;
            this.throwRay = throwRay;
            this.throwDistance = throwDistance;

            StartCoroutine( MainAILoop() );
        }

        public void Stop()
        {
            if( meshRenderer != null )
                meshRenderer.enabled = false;
            isAnimating = false;
            gameObject.SetActive( false );
        }

        IEnumerator MainAILoop()
        {
            Dev.Where();
            currentState = Out();
            //StartCoroutine( Debug() );

            for(; ; )
            {
                if( owner == null )
                    yield break;

                yield return currentState;
            }
        }

        IEnumerator Out()
        {
            Dev.Where();

            yield return new WaitForSeconds( startDelay );

            meshRenderer.enabled = true;

            transform.localRotation = Quaternion.identity;

            Vector2 throwTarget = throwRay.direction * throwDistance;

            Vector3 throwDirection = ((Vector3)throwTarget + throwRay.origin) - transform.position;
            if( throwDirection != Vector3.zero )
            {
                float angle = Mathf.Atan2(throwDirection.y, throwDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis( angle + 180f, Vector3.forward );
            }

            AnimationCurve throwCurve = new AnimationCurve();
            throwCurve.AddKey( 0f, 0f );
            throwCurve.AddKey( .1f, .2f );
            throwCurve.AddKey( .2f, .4f );
            throwCurve.AddKey( .3f, .6f );
            throwCurve.AddKey( .4f, .75f );
            throwCurve.AddKey( .5f, .85f );
            throwCurve.AddKey( .6f, .92f );
            throwCurve.AddKey( .7f, .95f );
            throwCurve.AddKey( .8f, .97f );
            throwCurve.AddKey( .9f, .98f );
            throwCurve.AddKey( 1f, 1f );

            float throwTime = throwMaxTravelTime;
            float time = 0f;

            while( time < throwTime )
            {
                float t = time/throwTime;

                transform.position = throwCurve.Evaluate( t ) * (Vector3)throwTarget + startPos;

                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            currentState = Return();

            yield break;
        }

        IEnumerator Return()
        {
            Dev.Where();

            thread.SetActive( true );

            Vector2 returnTarget = startPos;

            float time = 0f;

            float returnTimeRatio = .6f;

            AnimationCurve returnCurve = new AnimationCurve();
            returnCurve.AddKey( 0f, 0f );
            returnCurve.AddKey( .2f, .1f );
            returnCurve.AddKey( .4f, .2f );
            returnCurve.AddKey( .6f, .4f );
            returnCurve.AddKey( .8f, .6f );
            returnCurve.AddKey( 1f, 1f );

            float returnTime = throwMaxTravelTime * returnTimeRatio;
            Vector3 returnStartPos = transform.position;
            Vector3 returnVector = (Vector3)returnTarget - transform.position;

            while( time < returnTime )
            {
                float t = time/returnTime;

                transform.position = returnCurve.Evaluate( t ) * returnVector + returnStartPos;

                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            currentState = Complete();

            yield break;
        }

        IEnumerator Complete()
        {
            Dev.Where();

            meshRenderer.enabled = false;
            isAnimating = false;
            gameObject.SetActive( false );

            yield break;
        }
    }
}
