using HutongGames.PlayMaker.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Satchel;
using Satchel.Futils;
using HutongGames.PlayMaker;

namespace EnemyRandomizerMod
{

    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BuzzerControl : DefaultSpawnedEnemyControl { }

    public class BuzzerSpawner : DefaultSpawner<BuzzerControl> { }

    public class BuzzerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MosquitoControl : DefaultSpawnedEnemyControl { }

    public class MosquitoSpawner : DefaultSpawner<MosquitoControl> { }

    public class MosquitoPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    ///// 
    public class BurstingBouncerControl : DefaultSpawnedEnemyControl { }

    public class BurstingBouncerSpawner : DefaultSpawner<BurstingBouncerControl>
    {
        public override bool spawnEffectOnCorpseRemoved => true;
        public override string spawnEffectOnCorpseRemovedEffectName => "Gas Explosion Recycle L";
    }

    public class BurstingBouncerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    ///// TODO: add explosion to corpse
    public class AngryBuzzerControl : DefaultSpawnedEnemyControl { }

    public class AngryBuzzerSpawner : DefaultSpawner<AngryBuzzerControl>
    {
        public override bool spawnEffectOnCorpseRemoved => true;
        public override string spawnEffectOnCorpseRemovedEffectName => "Gas Explosion Recycle L";
    }

    public class AngryBuzzerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MantisHeavyFlyerControl : DefaultSpawnedEnemyControl { }

    public class MantisHeavyFlyerSpawner : DefaultSpawner<MantisHeavyFlyerControl> { }

    public class MantisHeavyFlyerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FlyControl : DefaultSpawnedEnemyControl { }

    public class FlySpawner : DefaultSpawner<FlyControl> { }

    public class FlyPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BlobbleControl : DefaultSpawnedEnemyControl { }

    public class BlobbleSpawner : DefaultSpawner<BlobbleControl> { }

    public class BlobblePrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ShadeSiblingControl : DefaultSpawnedEnemyControl { }

    public class ShadeSiblingSpawner : DefaultSpawner<ShadeSiblingControl> { }

    public class ShadeSiblingPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    


    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FlukemanTopControl : DefaultSpawnedEnemyControl { }

    public class FlukemanTopSpawner : DefaultSpawner<FlukemanTopControl> { }

    public class FlukemanTopPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class SpitterControl : DefaultSpawnedEnemyControl
    {
    }

    public class SpitterSpawner : DefaultSpawner<SpitterControl> { }

    public class SpitterPrefabConfig : DefaultPrefabConfig { }


    public class SpitterRControl : DefaultSpawnedEnemyControl
    {
    }

    public class SpitterRSpawner : DefaultSpawner<SpitterRControl> { }

    public class SpitterRPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FungoonBabyControl : DefaultSpawnedEnemyControl
    {
    }

    public class FungoonBabySpawner : DefaultSpawner<FungoonBabyControl> { }

    public class FungoonBabyPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class AcidFlyerControl : DefaultSpawnedEnemyControl { }

    public class AcidFlyerSpawner : DefaultSpawner<AcidFlyerControl> { }

    public class AcidFlyerPrefabConfig : DefaultPrefabConfig
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);

            try
            {
                var db = EnemyRandomizerDatabase.GetDatabase();
                if (db != null && !db.otherNames.Contains(EnemyRandomizerDatabase.BlockHitEffectName))
                {
                    var tink = p.prefab.GetComponentInChildren<TinkEffect>(true);

                    var beClone = GameObject.Instantiate(tink.blockEffect);
                    beClone.SetActive(false);
                    GameObject.DontDestroyOnLoad(beClone);
                    PrefabObject p2 = new PrefabObject();
                    SceneObject sp2 = new SceneObject();
                    sp2.components = new List<string>();
                    sp2.Scene = p.source.Scene;
                    p2.prefabName = EnemyRandomizerDatabase.BlockHitEffectName;
                    beClone.name = p2.prefabName;
                    sp2.path = beClone.name;
                    p2.prefab = beClone;
                    p2.source = sp2;
                    sp2.LoadedObject = p2;
                    sp2.Scene.sceneObjects.Add(sp2);
                    db.otherPrefabs.Add(p2);
                    db.Others[p2.prefabName] = p2;
                    db.Objects[p2.prefabName] = p2;
                    sp2.Loaded = true;
                }
            }
            catch(Exception e)
            {
                Dev.LogError($"Error creating block hit effect:{e.Message} Stacktrace:{e.StackTrace}");
            }
        }
    }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MageBalloonControl : DefaultSpawnedEnemyControl { }

    public class MageBalloonSpawner : DefaultSpawner<MageBalloonControl> { }

    public class MageBalloonPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RuinsFlyingSentryControl : DefaultSpawnedEnemyControl { }

    public class RuinsFlyingSentrySpawner : DefaultSpawner<RuinsFlyingSentryControl> { }

    public class RuinsFlyingSentryPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RuinsFlyingSentryJavelinControl : DefaultSpawnedEnemyControl { }

    public class RuinsFlyingSentryJavelinSpawner : DefaultSpawner<RuinsFlyingSentryJavelinControl> { }

    public class RuinsFlyingSentryJavelinPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FatFlyControl : DefaultSpawnedEnemyControl { }

    public class FatFlySpawner : DefaultSpawner<FatFlyControl> { }

    public class FatFlyPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class LazyFlyerEnemyControl : DefaultSpawnedEnemyControl { }

    public class LazyFlyerEnemySpawner : DefaultSpawner<LazyFlyerEnemyControl> { }

    public class LazyFlyerEnemyPrefabConfig : DefaultPrefabConfig { }

    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class SuperSpitterControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "spitter";

        public AudioSource audio;

        public int chanceToSpawnSuperBossOutOf100 = 5; // -> ( 20 / 100 )
        bool isSuperBoss;

        public override string customDreamnailText => isSuperBoss ? "Destroy." : base.customDreamnailText;

        public override bool explodeOnDeath => isSuperBoss ? true : false;

        public override string spawnEntityOnDeath => isSuperBoss ? "wp_saw" : null;

        int currentStage = 0;

        int[] stageHP = new int[4];

        public float spawnCooldown = 10f;
        public float spawnTime = 0f;

        public void MakeSuperBoss()
        {
            if (isSuperBoss)
                return;

            //TODO: add roar and a few other moves

            isSuperBoss = true;
            Sprite.color = Color.red;
            gameObject.AddParticleEffect_TorchShadeEmissions();
            MaxHP = GetStartingMaxHP(gameObject);
            ConfigSuperbossFSM();
        }

        protected override void Update()
        {
            base.Update();

            if (isSuperBoss)
            {
                if (!Mathnv.FastApproximately(SizeScale, 2f, 0.01f))
                {
                    SizeScale = 2f;
                    gameObject.ScaleObject(2f);
                    gameObject.ScaleAudio(1f);
                }

                TryDodge();

                try
                {
                    if (CurrentHP < stageHP[currentStage])
                    {
                        currentStage++;
                        ChildController.maxChildren = currentStage;
                    }
                }
                catch (Exception) { }

                SpawnChild();
            }
        }

        protected virtual void SpawnChild()
        {
            try
            {
                spawnTime += Time.deltaTime;
                if (spawnTime >= spawnCooldown && !ChildController.AtMaxChildren)
                {
                    spawnTime = 0f;
                    var spawn = ChildController.SpawnCustomArenaEnemy(transform.position, "Super Spitter", null, null);
                    var soc = spawn.GetComponent<SuperSpitterControl>();
                    if (soc.isSuperBoss)
                    {
                        soc.isSuperBoss = false;
                        soc.gameObject.ScaleObject(1f);
                        soc.gameObject.ScaleAudio(1f);
                        soc.Sprite.color = Color.white;
                        soc.MaxHP = soc.gameObject.OriginalPrefabHP();
                    }
                }
            }
            catch (Exception) { }
        }

        protected virtual void ConfigureDistanceFly()
        {
            try
            {
                var distFly = control.GetState("Distance Fly");
                distFly.InsertCustomAction(() =>
                {
                    if (!isSuperBoss)
                        return;

                try
                {
                    var action = distFly.GetFirstActionOfType<DistanceFly>();
                    if (action != null)
                    {
                        if (HeroController.instance.cState != null && HeroController.instance.cState.superDashing)
                        {
                            action.distance = 10f;
                            action.speedMax = 100f;
                            action.acceleration = 20f;
                        }
                        else
                        {
                            action.distance = 10f;
                            action.speedMax = 9f;
                            action.acceleration = 0.5f;
                        }
                    }
                }
                catch (Exception e) { Dev.LogError("Error inside distance fly "+e.Message + " " + e.StackTrace); }
                }, 0);
            }
            catch (Exception) { Dev.LogError("Error config distance fly"); }
        }

        protected virtual void ConfigureShot(int actionIindex)
        {
            FsmState fire = control.GetState("Fire");

            {
                int fireAction = actionIindex;
                var originalFire = fire.GetAction(fireAction);
                fire.DisableAction(fireAction);
                fire.InsertCustomAction(() =>
                {
                    if (!isSuperBoss)
                    {
                        originalFire.Enabled = true;
                        return;
                    }
                    ShootSuperSpitterShot();

                }, fireAction);
            }
        }

        protected virtual void ConfigureFire()
        {
            try
            {
                ConfigureShot(11);
                ConfigureShot(8);
                ConfigureShot(1);
            }
            catch (Exception e) { Dev.Log("Error configuring fire " + e.StackTrace); }
        }

        protected virtual void ConfigSuperbossFSM()
        {
            if(isSuperBoss)
            {
                if (!Mathnv.FastApproximately(SizeScale, 2f, 0.01f))
                {
                    SizeScale = 2f;
                    gameObject.ScaleObject(2f);
                    gameObject.ScaleAudio(1f);
                }

                ConfigureDistanceFly();
                ConfigureFire();
            }
        }

        protected virtual void ShootSuperSpitterShot()
        {
            var spawnPos = gameObject.DirectionToPlayer() * 2f + pos2d;

            GameObject shot = null;

            if (HeroController.instance.cState.superDashing)
            {
                shot = SpawnerExtensions.SpawnEntityAt("Lil Jellyfish", spawnPos, null, true, false);
            }
            else
            {
                float yDist = Mathf.Abs(heroPos2d.y - pos2d.y);
                if (yDist > 10f)
                {
                    shot = SpawnerExtensions.SpawnEntityAt("Shot GladiatorSickle", spawnPos, null, true, false);
                }
                else if (gameObject.DistanceToPlayer() > 40f)
                {
                    shot = SpawnerExtensions.SpawnEntityAt("Lil Jellyfish", spawnPos, null, true, false);
                }
                else if (gameObject.DistanceToPlayer() > 12f)
                {
                    shot = SpawnerExtensions.SpawnEntityAt("Mage Orb", transform.position, null, true, false);
                }
                else
                {
                    shot = SpawnerExtensions.SpawnEntityAt("Spitter Shot R", transform.position, null, true, false);
                }
            }

            shot.ScaleObject(SizeScale);
            shot.ScaleAudio(1f);

            control.FsmVariables.GetFsmGameObject("Shot").Value = shot;
        }

        protected virtual void TryDodge()
        {
            if(HeroController.instance.cState.dashing)
            {
                var toPlayer = gameObject.DirectionToPlayer();
                var toPlayerDist = gameObject.DistanceToPlayer();
                if(toPlayerDist < 10f)
                {
                    var toBug = -toPlayer;

                    // Calculate the dot product between player's velocity and direction to the bug
                    float dotProduct = Vector2.Dot(HeroController.instance.current_velocity.normalized, toBug.normalized);

                    // Check if the dot product is greater than a threshold (e.g., 0.5) to determine if the player is moving towards the bug
                    if (dotProduct > 0.5f)
                    {
                        // Perform the dodge
                        PhysicsBody.velocity += Vector2.up * 20f;
                    }
                }
            }
        }

        public override void Setup(GameObject objectThatWillBeReplaced = null)
        {
            base.Setup(objectThatWillBeReplaced);
            isSuperBoss = SpawnerExtensions.RollProbability(out int _, chanceToSpawnSuperBossOutOf100, 100);

            audio = GetComponent<AudioSource>();
            if (isSuperBoss)
            {
                Sprite.color = Color.red;

                gameObject.AddParticleEffect_TorchShadeEmissions();


                ConfigSuperbossFSM();
            }
        }

        public override int GetStartingMaxHP(GameObject objectThatWillBeReplaced)
        {
            if(!isSuperBoss)
                return base.GetStartingMaxHP(objectThatWillBeReplaced);

            float min = 200f;
            float max = 1600f;
            float range = max - min;
            float progress = MetaDataTypes.ProgressionZoneScale[GameManager.instance.GetCurrentMapZone()];
            float t = progress / 50f;
            int superMax = Mathf.FloorToInt(min + (range * t));

            if (isSuperBoss)
            {
                stageHP[0] = superMax;
                stageHP[1] = superMax - superMax / 8;
                stageHP[2] = superMax / 2;
                stageHP[3] = superMax / 3;
            }

            return isSuperBoss ? superMax : base.GetStartingMaxHP(objectThatWillBeReplaced);
        }
    }

    public class SuperSpitterSpawner : DefaultSpawner<SuperSpitterControl> { }

    public class SuperSpitterPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class Colosseum_Armoured_MosquitoControl : DefaultSpawnedEnemyControl { }

    public class Colosseum_Armoured_MosquitoSpawner : DefaultSpawner<Colosseum_Armoured_MosquitoControl> { }

    public class Colosseum_Armoured_MosquitoPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FungusFlyerControl : DefaultSpawnedEnemyControl { }

    public class FungusFlyerSpawner : DefaultSpawner<FungusFlyerControl> { }

    public class FungusFlyerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class Colosseum_Flying_SentryControl : DefaultSpawnedEnemyControl { }

    public class Colosseum_Flying_SentrySpawner : DefaultSpawner<Colosseum_Flying_SentryControl> { }

    public class Colosseum_Flying_SentryPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////
    public class JellyfishBabyControl : DefaultSpawnedEnemyControl { }

    public class JellyfishBabySpawner : DefaultSpawner<JellyfishBabyControl> { }

    public class JellyfishBabyPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MossFlyerControl : DefaultSpawnedEnemyControl { }

    public class MossFlyerSpawner : DefaultSpawner<MossFlyerControl> { }

    public class MossFlyerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class CrystalFlyerControl : DefaultSpawnedEnemyControl { }

    public class CrystalFlyerSpawner : DefaultSpawner<CrystalFlyerControl> { }

    public class CrystalFlyerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////










    /////////////////////////////////////////////////////////////////////////////
    /////
    public class SpiderFlyerControl : DefaultSpawnedEnemyControl { }

    public class SpiderFlyerSpawner : DefaultSpawner<SpiderFlyerControl> { }

    public class SpiderFlyerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BlowFlyControl : DefaultSpawnedEnemyControl { }

    public class BlowFlySpawner : DefaultSpawner<BlowFlyControl> { }

    public class BlowFlyPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BeeHatchlingAmbientControl : DefaultSpawnedEnemyControl
    {
        public override void Setup(GameObject other)
        {
            base.Setup(other);

            Geo = 2;
        }
    }

    public class BeeHatchlingAmbientSpawner : DefaultSpawner<BeeHatchlingAmbientControl> { }

    public class BeeHatchlingAmbientPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ParasiteBalloonControl : DefaultSpawnedEnemyControl { }

    public class ParasiteBalloonSpawner : DefaultSpawner<ParasiteBalloonControl> { }

    public class ParasiteBalloonPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class InflaterControl : DefaultSpawnedEnemyControl { }

    public class InflaterSpawner : DefaultSpawner<InflaterControl> { }

    public class InflaterPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FlukeFlyControl : DefaultSpawnedEnemyControl { }

    public class FlukeFlySpawner : DefaultSpawner<FlukeFlyControl> { }

    public class FlukeFlyPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////








    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BeeStingerControl : DefaultSpawnedEnemyControl { }

    public class BeeStingerSpawner : DefaultSpawner<BeeStingerControl> { }

    public class BeeStingerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BigBeeControl : DefaultSpawnedEnemyControl { }

    public class BigBeeSpawner : DefaultSpawner<BigBeeControl> { }

    public class BigBeePrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////











    /////////////////////////////////////////////////////////////////////////////
    /////
    public class JellyfishControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Jellyfish";
        public int chanceToSpawnSuperJellyOutOf100 = 20; // -> ( 20 / 100 )
        public int chanceToSpawnWhiteJellyOutOf100 = 2; // -> ( 20 / 100 )

        public override string spawnEntityOnDeath => thingToSpawn;
        public string thingToSpawn = "Lil Jellyfish";

        Func<GameObject> spawnerFunc;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            bool isSuperJelly = SpawnerExtensions.RollProbability(out int _, chanceToSpawnSuperJellyOutOf100, 100);
            bool isWhiteJelly = SpawnerExtensions.RollProbability(out int _, chanceToSpawnWhiteJellyOutOf100, 100);

            if (isSuperJelly)
            {
                gameObject.AddParticleEffect_WhiteSoulEmissions(Colors.GetColor(33));//orange
                spawnerFunc = gameObject.GetRandomAttackSpawnerFunc();
                var detach = control.GetState("Detach");
                detach.AddCustomAction(() => {
                    SpawnerExtensions.StartTrailEffectSpawns(this, 4, 0.2f, spawnerFunc);
                });
            }

            if (isWhiteJelly)
            {
                thingToSpawn = "wp_saw";
                gameObject.AddParticleEffect_WhiteSoulEmissions();
            }
        }
    }

    public class JellyfishSpawner : DefaultSpawner<JellyfishControl>
    {
    }

    public class JellyfishPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class LilJellyfishControl : DefaultSpawnedEnemyControl
    {
        public override void Setup(GameObject objectThatWillBeReplaced = null)
        {
            base.Setup(objectThatWillBeReplaced);
        }

        //TODO: double check if the lil jellies that spawn from enemies have replacements or not and use that to determine if a freeze is necessary

        //protected virtual void OnEnable()
        //{
        //    Freeze();
        //    StartCoroutine(UnfreezeAfter(5f));
        //}

        //IEnumerator UnfreezeAfter(float time)
        //{
        //    yield return new WaitForSeconds(time);
        //    UnFreeze();
        //}


        //protected virtual void Freeze()
        //{
        //    var pl = gameObject.GetOrAddComponent<PositionLocker>();
        //    pl.positionLock = transform.position;
        //}

        //protected virtual void UnFreeze()
        //{
        //    var locker = gameObject.GetComponent<PositionLocker>();
        //    if (locker != null)
        //    {
        //        GameObject.Destroy(locker);
        //        PhysicsBody.velocity = Vector2.zero;//reset the velocity
        //    }
        //}
    }

    public class LilJellyfishSpawner : DefaultSpawner<LilJellyfishControl> { }

    public class LilJellyfishPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////   





    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////   





    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////   





    /////
    //////////////////////////////////////////////////////////////////////////////



}
