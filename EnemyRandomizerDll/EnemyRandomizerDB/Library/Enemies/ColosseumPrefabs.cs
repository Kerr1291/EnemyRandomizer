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

        public void Spawn(string thing = null, float delay = 0f)
        {
            owner.AddToQueue(this);
            spawnDelay = delay;
            StartCoroutine(DoSpawn(thing));
        }

        protected virtual IEnumerator DoSpawn(string thing)
        {
            if(spawnDelay > 0f)
            {
                yield return new WaitForSeconds(spawnDelay);
            }
            if (spawnAnimation != null)
            {
                spawnAnimation.gameObject.SetActive(true);
            }
            yield return new WaitForSeconds(1.25f);
            owner.PlayCageUp();
            SafeSpawn(thing);
            yield return new WaitForSeconds(1.0f);
            owner.PlayCageDown();
            yield return new WaitForSeconds(0.5f);
            if (spawnAnimation != null)
            {
                spawnAnimation.gameObject.SetActive(false);
            }
        }

        protected virtual bool SafeSpawn(string thing)
        {
            bool error = false;
            try
            {
                spawnedEnemy = owner.Spawn(transform.position, thing);
                var soc = spawnedEnemy.GetComponent<SpawnedObjectControl>();
                if (flingRandomly)
                {
                    if(soc != null)
                    {
                        float vel = owner.rng.Rand(20f, 60f);
                        var dir = spawnedEnemy.GetRandomDirectionFromSelf(true);
                        soc.PhysicsBody.velocity = dir * vel;
                    }
                }

                if(soc != null)
                {
                    if(gameObject.GetDatabaseKey() == "Ceiling Dropper")
                    {
                        var alertRange = gameObject.FindGameObjectInDirectChildren("Alert Range");
                        var aBox = alertRange.GetComponent<BoxCollider2D>();
                        aBox.size = new Vector2(aBox.size.x, 50f);
                    }
                }

                cageOpenEffect.gameObject.SetActive(true);
                owner.PlayCageOpen();
                if (!string.IsNullOrEmpty(playCustomEffectOnSpawn))
                    SpawnerExtensions.SpawnEntityAt(playCustomEffectOnSpawn, transform.position, true, false);
            }
            catch (Exception e) {
                error = true;
                Dev.LogError($"Caught exception while trying to spawn a thing from a cage \n{e.Message}\n{e.StackTrace}"); 
            }

            try
            {
                onSpawn?.Invoke(this, spawnedEnemy);
            }
            catch (Exception e)
            {
                Dev.LogError($"Caught exception while trying to invoke onSpawn callback \n{e.Message}\n{e.StackTrace}");
            }
            return !error;
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
        public float zoteMaxVel = 60f;
        public bool openCage = false;
        public ZoteMusicControl zmusic;

        public void PlayZoteTheme()
        {
            if (zmusic == null)
            {
                if(owner != null)
                    owner.CrowdLaugh();
                GameObject music = SpawnerExtensions.SpawnEntityAt("Zote Music", transform.position, true, false);
                zmusic = music.GetComponent<ZoteMusicControl>();

            }

            zmusic.PlayMusic();
            var normal = zmusic.audioSource.outputAudioMixerGroup.audioMixer.FindSnapshot("Normal");
            normal.TransitionTo(1f);

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
                    var normal = zmusic.audioSource.outputAudioMixerGroup.audioMixer.FindSnapshot("Normal");
                    normal.TransitionTo(1f);
                    yield return new WaitForSeconds(10f);
                }
            }
        }

        protected override IEnumerator DoSpawn(string thing)
        {
            yield return new WaitForSeconds(spawnDelay);

            yield return new WaitUntil(() => openCage);

            SafeSpawn(thing);
        }

        protected override bool SafeSpawn(string thing)
        {
            bool error = false;
            try
            {
                for (int i = 0; i < zotesToSpawn; ++i)
                {
                    spawnedEnemy = owner.Spawn(transform.position, thing);
                    var soc = spawnedEnemy.GetComponent<SpawnedObjectControl>();
                    if (flingRandomly)
                    {
                        if (soc != null)
                        {
                            float vel = owner.rng.Rand(zoteMinVel, zoteMaxVel);
                            var dir = spawnedEnemy.GetRandomDirectionFromSelf(true);
                            soc.PhysicsBody.gravityScale = owner.rng.Rand(0.3f, 1.7f);
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

                if (!string.IsNullOrEmpty(playCustomEffectOnSpawn))
                    SpawnerExtensions.SpawnEntityAt(playCustomEffectOnSpawn, transform.position, true, false);
            }
            catch (Exception e)
            {
                error = true;
                Dev.LogError($"Caught exception while trying to spawn a thing from a cage \n{e.Message}\n{e.StackTrace}");
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

        public void PlayMusic()
        {
            if(audioSource.isPlaying)
            {
                audioSource.volume = 1f;
            }
            else
            {
                audioSource.Play();
                audioSource.volume = 0.8f;
            }
        }

        protected virtual void OnDestory()
        {
            if (audioSource != null && audioSource.isPlaying)
                audioSource.Stop();
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
