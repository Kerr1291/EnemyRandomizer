using UnityEngine;

namespace EnemyRandomizerMod
{
    public class SpawnEffectOnDestroy : SpawnEffect
    {
        public override bool allowRandomizationOfSpawn => allowRandomization;

        public bool allowRandomization = false;
        public bool forceDestroyIfRendererBecomesDisabled = true;
        MeshRenderer mRenderer;
        bool wasEnabled = false;

        protected virtual void OnEnable()
        {
            mRenderer = GetComponent<MeshRenderer>();
        }

        protected override void OnDestroy()
        {
            if (isUnloading)
                return;

            Spawn(gameObject, transform.position); 
        }

        protected virtual void Update()
        {
            if (isUnloading)
                return;

            if (mRenderer == null)
                return;

            if(!wasEnabled)
                wasEnabled = mRenderer.enabled;

            if (wasEnabled && mRenderer.enabled == false)
            {
                GameObject.Destroy(gameObject);
            }
        }
    }
}
