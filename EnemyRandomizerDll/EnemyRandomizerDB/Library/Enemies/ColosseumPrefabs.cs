using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using EnemyRandomizerMod;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Collections;
using System;
using Satchel;
using Satchel.Futils;
using HutongGames.PlayMaker.Actions;

namespace EnemyRandomizerMod
{
    //this converts the colo cages into prefabs of the enemies that exist inside them
    public class ColosseumCageLargePrefabConfig : IPrefabConfig
    {
        public virtual void SetupPrefab(PrefabObject p)
        {
            if (p.source.Scene.name == "Room_Colosseum_Bronze" && p.source.path.Contains("(1)"))
            {
                try
                {
                    var prefab = p.prefab;
                    prefab.name = "Arena Cage Large";
                    string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);

                    Dev.Log("Loaded = " + p.prefab.name);

                    p.prefabName = prefab.name;
                    p.prefab = prefab;
                }
                catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
            }
            else
            {
                try
                {
                    var prefab = p.prefab.LocateMyFSM("Spawn").Fsm.GetFsmGameObject("Corpse to Instantiate").Value;

                    string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);

                    //get actual enemy prefab from the fsm
                    p.prefabName = keyName;
                    p.prefab = prefab;
                }
                catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
            }
        }

        public static string GetEnemyPrefabNameInsideCage(GameObject largeCage)
        {
            try
            {
                var prefab = largeCage.LocateMyFSM("Spawn").Fsm.GetFsmGameObject("Corpse to Instantiate").Value;

                string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);

                //get actual enemy prefab from the fsm
                return keyName;
            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
            return string.Empty;
        }
    }

    //this converts the colo cages into prefabs of the enemies that exist inside them
    public class ColosseumCageSmallPrefabConfig : IPrefabConfig
    {
        public virtual void SetupPrefab(PrefabObject p)
        {

            if (p.source.Scene.name == "Room_Colosseum_Bronze" && p.source.path.Contains("(1)"))
            {
                try
                {
                    var prefab = p.prefab;
                    prefab.name = "Arena Cage Small";
                    string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);

                    Dev.Log("Loaded = " + p.prefab.name);

                    p.prefabName = prefab.name;
                    p.prefab = prefab;
                }
                catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
            }
            else
            {
                GameObject prefab = null;

                try
                {
                    if (prefab == null)
                        prefab = p.prefab.LocateMyFSM("Spawn").Fsm.GetFsmGameObject("Enemy Type").Value;
                }
                catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }

                try
                {
                    if (prefab == null)
                        prefab = p.prefab.LocateMyFSM("Spawn").Fsm.GetFsmGameObject("z Corpse to Instantiate").Value;
                }
                catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }


                string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);

                //get actual enemy prefab from the fsm
                p.prefabName = keyName;
                p.prefab = prefab;
            }
        }

        public static string GetEnemyPrefabNameInsideCage(GameObject smallCage)
        {
            GameObject prefab = null;
            try
            {
                prefab = smallCage.LocateMyFSM("Spawn").Fsm.GetFsmGameObject("Enemy Type").Value;
            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }

            try
            {
                if (prefab == null)
                    prefab = smallCage.LocateMyFSM("Spawn").Fsm.GetFsmGameObject("z Corpse to Instantiate").Value;
            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }

            if (prefab != null)
            {
                string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);

                //get actual enemy prefab from the fsm
                return keyName;
            }
            return string.Empty;
        }
    }

    public class ColosseumCageZotePrefabConfig : IPrefabConfig
    {
        public virtual void SetupPrefab(PrefabObject p)
        {
            if (p.source.Scene.name == "Room_Colosseum_Bronze")
            {
                try
                {
                    var prefab = p.prefab;
                    prefab.name = "Arena Cage Zote";
                    string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);

                    Dev.Log("Loaded = " + p.prefab.name);

                    p.prefabName = prefab.name;
                    p.prefab = prefab;
                }
                catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
            }
            else
            {
            }
        }
    }


    public class MusicPrefabConfig : IPrefabConfig
    {
        public virtual void SetupPrefab(PrefabObject p)
        {
            if (p.source.Scene.name == "GG_Mighty_Zote")
            {
                try
                {
                    var prefab = p.prefab;
                    prefab.name = "Zote Music";
                    string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);

                    Dev.Log("Loaded = " + p.prefab.name);

                    p.prefabName = prefab.name;
                    p.prefab = prefab;
                }
                catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
            }
            else
            {
            }
        }
    }


    public class GrubMimicBottleControl : DefaultSpawnedEnemyControl
    {
        //TODO: 
    }

    public class GrubMimicBottleSpawner : DefaultSpawner<GrubMimicBottleControl> { }

    public class GrubMimicBottlePrefabConfig : DefaultPrefabConfig { }



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
                SpawnerExtensions.SpawnEntityAt(playCustomEffectOnSpawn, transform.position, true, false);
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
                    var aBox = alertRange.GetComponent<BoxCollider2D>();
                    aBox.size = new Vector2(aBox.size.x, 50f);
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











    public class ArenaCageZoteControl : ArenaCageControl
    {
        public PlayMakerFSM control;
        public int zotesToSpawn = 57;
        public float zoteMinVel = 20f;
        public float zoteMaxVel = 90f;
        public bool openCage = false;
        public ZoteMusicControl zmusic;

        public void PlayZoteTheme()
        {
            if (zmusic == null)
            {
                if(owner != null)
                    owner.CrowdLaugh();
                GameObject music = SpawnerExtensions.SpawnEntityAt("Zote Music", HeroController.instance.transform.position, true, false);
                zmusic = music.GetComponent<ZoteMusicControl>();

            }

            zmusic.PlayMusic();
            var normal = zmusic.audioSource.outputAudioMixerGroup.audioMixer.FindSnapshot("Action");
            normal.TransitionTo(2f);


            GameManager.instance.StartCoroutine(KeepPlaying());
        }

        public IEnumerator KeepPlaying()
        {
            bool isColoBronze = BattleManager.StateMachine.Value is ColoBronze;
            if (isColoBronze)
            {
                var bronze = BattleManager.StateMachine.Value as ColoBronze;

                while (!bronze.battleEnded)
                {
                    if(zmusic.coloMCaudioSource.clip != zmusic.audioSource.clip)
                    {
                        zmusic.coloMCaudioSource.Stop();
                        zmusic.coloMCaudioSource.clip = zmusic.audioSource.clip;
                        zmusic.coloMCaudioSource.Play();
                    }    

                    var normal = zmusic.audioSource.outputAudioMixerGroup.audioMixer.FindSnapshot("Action");
                    normal.TransitionTo(1f);
                    yield return new WaitForSeconds(10f);
                }
            }
        }

        protected override IEnumerator DoSpawn(string thing, string originalEnemy)
        {
            yield return new WaitForSeconds(spawnDelay);

            yield return new WaitUntil(() => openCage);

            if (spawnBatchSize > 1)
            {
                for (int i = 0; i < spawnBatchSize; ++i)
                    SafeSpawn(thing, originalEnemy);
            }
            else
            {
                SafeSpawn(thing, originalEnemy);
            }

            if (!string.IsNullOrEmpty(playCustomEffectOnSpawn))
                SpawnerExtensions.SpawnEntityAt(playCustomEffectOnSpawn, transform.position, true, false);
        }

        protected override bool SafeSpawn(string thing, string originalEnemy = null, bool preload = false)
        {
            bool error = false;
            try
            {
                if (!preload && !string.IsNullOrEmpty(thing) && preloads.ContainsKey(thing) && preloads[thing].Count > 0)
                {
                    spawnedEnemy = preloads[thing].Dequeue();
                    owner.AddExternalObjectToArena(spawnedEnemy);
                }
                else
                {
                    spawnedEnemy = owner.Spawn(transform.position, thing, originalEnemy, preload);
                }

                if (preload)
                {
                    if (!preloads.TryGetValue(thing, out var thingQueue))
                    {
                        thingQueue = new Queue<GameObject>();
                        preloads.Add(thing, thingQueue);
                    }

                    thingQueue.Enqueue(spawnedEnemy);
                }
                else
                {
                    if (!string.IsNullOrEmpty(thing) && preloads.ContainsKey(thing) && (preloads[thing] == null || preloads[thing].Count <= 0))
                        preloads.Remove(thing);

                    var soc = spawnedEnemy.GetComponent<SpawnedObjectControl>();
                    if (flingRandomly)
                    {
                        if (soc != null)
                        {
                            float vel = owner.rng.Rand(zoteMinVel, zoteMaxVel);
                            var dir = new Vector2(vel, 0f);
                            soc.PhysicsBody.velocity = dir * vel;
                        }
                    }

                    try
                    {
                        onSpawn?.Invoke(this, spawnedEnemy);
                    }
                    catch (Exception e)
                    {
                        Dev.LogError($"Caught exception while trying to invoke onSpawn callback \n{e.Message}\n{e.StackTrace}");
                    }
                }
            }
            catch (Exception e)
            {
                error = true;
                Dev.LogError($"Caught exception while trying to spawn a {thing} from a cage that used to hold {originalEnemy} \n{e.Message}\n{e.StackTrace}");
            }

            return !error;
        }
    }

    public class ArenaCageZoteSpawner : DefaultSpawner
    {
        public override GameObject Spawn(PrefabObject prefabToSpawn, GameObject objectToReplace)
        {
            var cage = base.Spawn(prefabToSpawn, null);
            var acage = cage.GetOrAddComponent<ArenaCageZoteControl>();
            acage.soc = cage.GetComponent<SpawnedObjectControl>();
            acage.spriteRenderer = null;
            acage.audioSource = null;

            acage.control = cage.LocateMyFSM("Control");

            var control = acage.control;
            var init = control.GetState("Init");
            init.DisableActions(1, 2);
            //init.AddCustomAction(() => {
            //    acage.owner.Bronze_ResetWallC();
            //});

            init.ChangeTransition("FINISHED", "Struts");

            var p3 = control.GetState("Pause 3");
            p3.GetAction<Wait>(0).time = 7f;
            p3.InsertCustomAction(() => { acage.PlayZoteTheme(); }, 0);

            var open = control.GetState("Open");
            open.DisableAction(3);
            open.InsertCustomAction(() => {
                acage.openCage = true;
                acage.owner.waitingOnZote = false;
            },5);

            return cage;
        }
    }

    public class ArenaCageZotePrefabConfig : DefaultPrefabConfig
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);
            p.prefab.AddComponent<ArenaCageSmallControl>();
        }
    }








    public class ZoteMusicControl : MonoBehaviour
    {
        public SpawnedObjectControl soc;
        public AudioSource audioSource;
        public AudioSource coloMCaudioSource;

        public void PlayMusic()
        {
            bool isColoBronze = BattleManager.StateMachine.Value is ColoBronze;
            if (isColoBronze)
            {
                var bronze = BattleManager.StateMachine.Value as ColoBronze;
                var mc = bronze.FSM.gameObject.FindGameObjectInDirectChildren("Music Control");
                coloMCaudioSource = mc.GetComponent<AudioSource>();
                coloMCaudioSource.Stop();
                coloMCaudioSource.clip = audioSource.clip;
            }

            if (coloMCaudioSource.isPlaying)
            {
                coloMCaudioSource.volume = 1f;
            }
            else
            {
                coloMCaudioSource.Play();
                coloMCaudioSource.volume = 0.8f;
            }
        }

        protected virtual void OnDestory()
        {
            if (coloMCaudioSource != null && coloMCaudioSource.isPlaying)
                coloMCaudioSource.Stop();
        }
    }

    public class ZoteMusicSpawner : DefaultSpawner
    {
        public override GameObject Spawn(PrefabObject prefabToSpawn, GameObject objectToReplace)
        {
            var cage = base.Spawn(prefabToSpawn, null);
            var acage = cage.GetOrAddComponent<ZoteMusicControl>();
            acage.soc = cage.GetComponent<SpawnedObjectControl>();
            acage.audioSource = cage.GetComponent<AudioSource>();

            return cage;
        }
    }

    public class ZoteMusicPrefabConfig : DefaultPrefabConfig
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);
            p.prefab.AddComponent<ArenaCageSmallControl>();
        }
    }
}
