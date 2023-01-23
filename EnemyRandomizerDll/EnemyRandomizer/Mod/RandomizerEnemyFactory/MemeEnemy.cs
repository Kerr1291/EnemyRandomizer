using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using nv;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System;
//using EnemyRandomizerMod.Extensions;
using System.Collections;

namespace EnemyRandomizerMod
{
    public class MemeController : MonoBehaviour
    {
        public BoxCollider2D collider;
        public GameObject meme;

        RNG memerng;
        float memeCooldown = 10f;
        int memeLimit = 0;

        List<GameObject> memes = new List<GameObject>();

        private IEnumerator Start()
        {
            Dev.Where();
            while (collider == null && meme == null)
            {
                collider = GetComponent<BoxCollider2D>();
                meme = gameObject;
                yield return null;
            }

            memerng = new RNG();
            memerng.Reset();

            float timer = 0f;
            while (true)
            {
                yield return new WaitForEndOfFrame();
                timer += Time.deltaTime;
                if (timer > memeCooldown)
                {
                    timer = 0f;
                    memeCooldown = memerng.Rand(5f, 30f);
                }
                else
                {
                    continue;
                }

                for (int i = 0; i < memes.Count;)
                {
                    if (memes[i] == null)
                    {
                        memes.RemoveAt(i);
                        i = 0;
                        continue;
                    }

                    if ((memes[i].transform.position - meme.transform.position).magnitude > 20f)
                        memes[i].transform.position = meme.transform.position;

                    ++i;
                }

                //max hp is 2000, so 2000/400 = 5 memes at full hp
                memeLimit = (2000 - (meme.GetEnemyHP())) / 400;

                if (memes.Count >= memeLimit)
                {
                    continue;
                }

                var superSpitterData = EnemyRandomizer.Instance.EnemyDataMap["Super Spitter"];
                if (superSpitterData == null)
                    continue;

                Vector3 position = meme.transform.position;
                string originalName = superSpitterData.loadedEnemy.EnemyObject.name;

                //prevent the randomizer from shuffling our new guy
                superSpitterData.loadedEnemy.EnemyObject.name = EnemyRandomizer.ENEMY_RANDO_PREFIX + originalName;

                GameObject newEnemy = superSpitterData.loadedEnemy.Instantiate(superSpitterData, null, null);
                newEnemy.SafeSetActive(true);

                //fix the data...
                superSpitterData.loadedEnemy.EnemyObject.name = originalName;
                newEnemy.transform.position = position;

                memes.Add(newEnemy);
            }

            yield break;
        }

        void OnDestroy()
        {
            memes.ForEach(x => { if (x != null) GameObject.Destroy(x.gameObject); });
        }
    }

    public class MemeEnemy : DefaultEnemy
    {
        public override void Setup(EnemyData enemy, List<EnemyData> knownEnemyTypes, GameObject prefabObject)
        {
            Dev.Log("Creating meme");
            EnemyObject = prefabObject;
            try
            {
                var alt = prefabObject.LocateMyFSM("Spawn").Fsm.GetFsmGameObject("Enemy Type").Value;

                if (alt != null)
                {
                    EnemyObject = alt;
                }
            }
            catch(Exception)
            {
                Dev.LogError("Failed to load meme boss");
            }
        }

        public override void ModifyNewEnemyGeo(GameObject newEnemy, EnemyData sourceData, GameObject oldEnemy = null, EnemyData matchingData = null)
        {
            newEnemy.SetEnemyGeoRates(100, 75, 50);
        }

        public override void FinalizeNewEnemy(GameObject newEnemy, EnemyData sourceData, GameObject oldEnemy = null, EnemyData matchingData = null)
        {
            base.FinalizeNewEnemy(newEnemy, sourceData, oldEnemy, matchingData);

            newEnemy.transform.localScale = new Vector3(3.2f, 3.2f, 3.2f);
            newEnemy.SetEnemyHP(2000);

            HutongGames.PlayMaker.Actions.DistanceFly df = newEnemy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.DistanceFly>("Distance Fly", "spitter");
            df.distance.Value *= 1.5f;
            df.acceleration.Value *= 8f;
            df.speedMax.Value *= 200f;

            HutongGames.PlayMaker.Actions.FireAtTarget fa = newEnemy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.FireAtTarget>("Fire", "spitter");
            fa.speed.Value *= 2.24f;

            GameObject bullet = newEnemy.FindGameObjectInChildrenWithName("BulletSprite (1)");
            bullet.transform.localScale = Vector3.one * 3.2f;
        }

        public override GameObject Instantiate(EnemyData sourceData, GameObject enemyToReplace = null, EnemyData matchingData = null)
        {
            var newEnemy = base.Instantiate(sourceData, enemyToReplace, matchingData);

            newEnemy.AddComponent<MemeController>();

            return newEnemy;
        }
    }
}