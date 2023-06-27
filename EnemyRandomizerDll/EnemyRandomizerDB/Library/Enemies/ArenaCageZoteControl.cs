using UnityEngine;
using UnityEngine.SceneManagement;
using Satchel;
using Satchel.Futils;
using HutongGames.PlayMaker.Actions;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace EnemyRandomizerMod
{
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
                GameObject music = SpawnerExtensions.SpawnEntityAt("Zote Music", HeroController.instance.transform.position, null, true, false);
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
                    if (!EnemyRandomizerDatabase.GetGlobalSettings().allowEnemyRandoExtras)
                    {
                        if(zmusic != null)
                        {
                            zmusic.StopMusic();
                            yield break;
                        }
                    }


                    if (zmusic.coloMCaudioSource.clip != zmusic.audioSource.clip)
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
                SpawnerExtensions.SpawnEntityAt(playCustomEffectOnSpawn, transform.position, null, true, false);
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

            if (EnemyRandomizerDatabase.GetGlobalSettings().allowEnemyRandoExtras)
            {
                p3.GetAction<Wait>(0).time = 7f;
                p3.InsertCustomAction(() => { acage.PlayZoteTheme(); }, 0);
            }
            else
            {
                p3.GetAction<Wait>(0).time = 2f;
            }

            var open = control.GetState("Open");
            open.DisableAction(3);
            open.InsertCustomAction(() => {
                acage.openCage = true;
                acage.owner.waitingOnZote = false;
            }, 5);

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
}
