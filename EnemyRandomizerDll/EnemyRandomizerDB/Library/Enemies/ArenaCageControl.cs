using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

namespace EnemyRandomizerMod
{
    public class ArenaCageControl : MonoBehaviour
    {
        public CustomArena owner;
        public SpawnedObjectControl soc;
        public SpriteRenderer spriteRenderer;
        public AudioSource audioSource;

        public Animator spawnAnimation;
        public GameObject cageOpenEffect;
        public string playCustomEffectOnSpawn = null;

        public GameObject spawnedEnemy;
        public Action<ArenaCageControl, GameObject> onSpawn;
        public float spawnDelay = 0f;
        public bool flingRandomly = false;
        public Dictionary<string, Queue<GameObject>> preloads = new Dictionary<string, Queue<GameObject>>();
        public int spawnBatchSize = 1;

        public virtual void Preload(string thing = null, int count = 1)
        {
            StartCoroutine(DoPreload(thing, count));
        }

        public virtual IEnumerator DoPreload(string thing, int count, int batchsize = 5)
        {
            for (int i = 0; i < count; i += batchsize)
            {
                for (int j = 0; j < batchsize; ++j)
                {
                    if ((i + j) >= count)
                        break;

                    Dev.Log((i + j) + " / " + (count-1) + " preloading " + thing);
                    SafeSpawn(thing, null, true);
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        public void SpawnBatch(string thing = null, string originalEnemy = null, float delay = 0f, int batchSize = 1)
        {
            spawnBatchSize = batchSize;
            Spawn(thing, originalEnemy, delay);
        }

        public void Spawn(string thing = null, string originalEnemy = null, float delay = 0f)
        {
            owner.AddToQueue(this);
            spawnDelay = delay;
            StartCoroutine(DoSpawn(thing, originalEnemy));
        }

        protected virtual IEnumerator DoSpawn(string thing, string originalEnemy)
        {
            if(spawnDelay > 0f)
            {
                yield return new WaitForSeconds(spawnDelay);
            }
            if (spawnAnimation != null)
            {
                spawnAnimation.gameObject.SetActive(true);
            }
            owner.PlayCageUp();
            yield return new WaitForSeconds(1.25f);
            if (spawnBatchSize > 1)
            {
                for(int i = 0; i < spawnBatchSize; ++i)
                    SafeSpawn(thing, originalEnemy);
            }
            else
            {
                SafeSpawn(thing, originalEnemy);
            }
            yield return new WaitForSeconds(1.0f);
            //cage goes back down
            yield return new WaitForSeconds(0.5f);
            if (spawnAnimation != null)
            {
                spawnAnimation.gameObject.SetActive(false);
            }
        }

        protected virtual GameObject GetEnemyForSafeSpawn(string thing, string originalEnemy = null, bool preload = false)
        {
            if (!preload && !string.IsNullOrEmpty(thing) && preloads.ContainsKey(thing) && preloads[thing].Count > 0)
            {
                spawnedEnemy = preloads[thing].Dequeue();
                owner.AddExternalObjectToArena(spawnedEnemy);
            }
            else
            {
                spawnedEnemy = owner.Spawn(transform.position, thing, originalEnemy, preload);
                spawnedEnemy.SafeSetActive(true);
            }
            return spawnedEnemy;
        }

        protected virtual bool SafeSpawn(string thing, string originalEnemy = null, bool preload = false)
        {
            bool error = false;
            try
            {
                spawnedEnemy = GetEnemyForSafeSpawn(thing, originalEnemy, preload);

                if (preload && !string.IsNullOrEmpty(thing))
                {
                    PreloadThing(thing);
                }
                else
                {
                    CheckRemoveFromPreloads(thing);
                    SpawnThing();
                    PlayCageOpenEffects();
                }
            }
            catch (Exception e) {
                error = true;
                Dev.LogError($"Caught exception while trying to spawn a {thing} from a cage that used to hold {originalEnemy} \n{e.Message}\n{e.StackTrace}"); 
            }

            if (!preload)
            {
                try
                {
                    onSpawn?.Invoke(this, spawnedEnemy);
                }
                catch (Exception e)
                {
                    Dev.LogError($"Caught exception while trying to invoke onSpawn callback \n{e.Message}\n{e.StackTrace}");
                }
            }
            return !error;
        }

        private void CheckRemoveFromPreloads(string thing)
        {
            if (!string.IsNullOrEmpty(thing) && preloads.ContainsKey(thing) && preloads[thing].Count <= 0)
                preloads.Remove(thing);
        }

        private void PlayCageOpenEffects()
        {
            cageOpenEffect.gameObject.SetActive(true);
            owner.PlayCageOpen();
            if (!string.IsNullOrEmpty(playCustomEffectOnSpawn))
                SpawnerExtensions.SpawnEntityAt(playCustomEffectOnSpawn, transform.position, null, true, false);
        }

        private void SpawnThing()
        {            
            if (flingRandomly)
            {
                FlingLastSpawnedThing();
            }

            FixLastSpawnedThing();
        }

        private void FixLastSpawnedThing()
        {
            if (spawnedEnemy == null)
                return;

            var soc = spawnedEnemy.GetComponent<SpawnedObjectControl>();
            if (soc != null)
            {
                if (soc.gameObject.GetDatabaseKey() == "Ceiling Dropper")
                {
                    var alertRange = gameObject.FindGameObjectInDirectChildren("Alert Range");
                    if (alertRange != null)
                    {
                        var aBox = alertRange.GetComponent<BoxCollider2D>();
                        if (aBox != null)
                        {
                            aBox.size = new Vector2(aBox.size.x, 50f);
                        }
                    }
                }
                if (soc.gameObject.GetDatabaseKey() == "Fluke Mother")
                {
                    soc.gameObject.ScaleObject(2f);
                    soc.gameObject.ScaleAudio(1.5f);
                    soc.SizeScale = 2f;
                }
            }
        }

        private void FlingLastSpawnedThing()
        {
            var soc = spawnedEnemy.GetComponent<SpawnedObjectControl>();
            if (soc != null)
            {
                float vel = owner.rng.Rand(45f, 120f);
                var dir = spawnedEnemy.GetRandomDirectionFromSelf(true);
                soc.PhysicsBody.velocity = dir * vel;
            }
        }

        private void PreloadThing(string thing)
        {
            if (!preloads.TryGetValue(thing, out var thingQueue))
            {
                thingQueue = new Queue<GameObject>();
                preloads.Add(thing, thingQueue);
            }
            Dev.Log("Preload queued");
            thingQueue.Enqueue(spawnedEnemy);
        }
    }



    public class ArenaCageSmallControl : ArenaCageControl
    {

    }

    public class ArenaCageSmallSpawner : DefaultSpawner
    {
        public override GameObject Spawn(PrefabObject prefabToSpawn, GameObject objectToReplace)
        {
            var cage = base.Spawn(prefabToSpawn, null);
            var acage = cage.GetOrAddComponent<ArenaCageSmallControl>();
            acage.soc = cage.GetComponent<SpawnedObjectControl>();
            acage.spriteRenderer = cage.GetComponent<SpriteRenderer>();
            acage.audioSource = cage.GetComponent<AudioSource>();

            acage.spriteRenderer.enabled = false;
            acage.spawnAnimation = cage.FindGameObjectInDirectChildren("Anim").GetComponent<Animator>();
            acage.cageOpenEffect = cage.FindGameObjectInDirectChildren("Strike Nail");

            return cage;
        }
    }

    public class ArenaCageSmallPrefabConfig : DefaultPrefabConfig
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);
            p.prefab.AddComponent<ArenaCageSmallControl>();
        }
    }

    public class ArenaCageLargeControl : ArenaCageControl
    {

    }

    public class ArenaCageLargeSpawner : DefaultSpawner
    {
        public override GameObject Spawn(PrefabObject prefabToSpawn, GameObject objectToReplace)
        {
            var cage = base.Spawn(prefabToSpawn, null);
            var acage = cage.GetOrAddComponent<ArenaCageLargeControl>();
            SetupComponents(acage);
            SetupInitialState(acage);

            return cage;
        }

        protected virtual void SetupComponents(ArenaCageControl acage)
        {
            acage.soc = acage.GetComponent<SpawnedObjectControl>();
            acage.spriteRenderer = acage.GetComponent<SpriteRenderer>();
            acage.audioSource = acage.GetComponent<AudioSource>();
        }

        protected virtual void SetupInitialState(ArenaCageControl acage)
        {
            Hide(acage);
            GetAnim(acage);
            GetCageOpenEffect(acage);
        }

        protected virtual void Hide(ArenaCageControl acage)
        {
            acage.spriteRenderer.enabled = false;
        }

        protected virtual void GetAnim(ArenaCageControl acage)
        {
            acage.spawnAnimation = acage.gameObject.FindGameObjectInDirectChildren("Anim").GetComponent<Animator>();
        }

        protected virtual void GetCageOpenEffect(ArenaCageControl acage)
        {
            acage.cageOpenEffect = acage.gameObject.FindGameObjectInDirectChildren("Strike Nail");
        }
    }

    public class ArenaCageLargePrefabConfig : DefaultPrefabConfig
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);
            p.prefab.AddComponent<ArenaCageLargeControl>();

            //var fsm = p.prefab.LocateMyFSM("Spawn");
            //fsm.enabled = false;
        }
    }

}
