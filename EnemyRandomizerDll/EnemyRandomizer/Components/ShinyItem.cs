using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nv
{
    public class ShinyItem : GameStateMachine
    {
        public SpriteRenderer spriteRenderer;
        public Animator unityAnimator;
        
        public override bool Running {
            get {
                return gameObject.activeInHierarchy;
            }

            set {
                gameObject.SetActive( value );
            }
        }

        protected override void SetupRequiredReferences()
        {
            base.SetupRequiredReferences();
            spriteRenderer = GetComponent<SpriteRenderer>();
            unityAnimator = GetComponent<Animator>();
        }

        protected override IEnumerator Init()
        {
            yield return base.Init();


        }

        protected override IEnumerator ExtractReferencesFromExternalSources()
        {
            yield return base.ExtractReferencesFromExternalSources();
        }
    }
}