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
    public class MemeController : DefaultEnemyController
    {
        public BoxCollider2D collider;

        RNG memerng;
        float memeCooldown = 10f;
        int memeLimit = 0;

        List<GameObject> memes = new List<GameObject>();

        private IEnumerator Start()
        {
            Dev.Where();
            while (collider == null && Instance == null)
            {
                collider = GetComponent<BoxCollider2D>();
                Instance = gameObject;
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

                    if ((memes[i].transform.position - Instance.transform.position).magnitude > 20f)
                        memes[i].transform.position = Instance.transform.position;

                    ++i;
                }

                //max hp is 2000, so 2000/400 = 5 memes at full hp
                memeLimit = (2000 - (this.GetEnemyHP().Value)) / 400;

                if (memes.Count >= memeLimit)
                {
                    continue;
                }

                var superSpitterData = EnemyRandomizer.Instance.EnemyDataMap["Super Spitter"];
                if (superSpitterData == null)
                    continue;

                Vector3 position = Instance.transform.position;

                GameObject newEnemy = superSpitterData.loadedEnemy.Instantiate();
                newEnemy.SetupRandomizerComponents(superSpitterData.loadedEnemy);
                newEnemy.SetActive(true);

                //fix the data...
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
        public override void SetupPrefab()
        {
            Dev.Where();
            Prefab.AddComponent<MemeController>();
            Prefab.transform.localScale = new Vector3(3.2f, 3.2f, 3.2f);
            Prefab.GetComponent<HealthManager>().hp = 2000;

            HutongGames.PlayMaker.Actions.DistanceFly df = Prefab.GetFSMActionOnState<HutongGames.PlayMaker.Actions.DistanceFly>("Distance Fly", "spitter");
            df.distance.Value *= 1.5f;
            df.acceleration.Value *= 8f;
            df.speedMax.Value *= 200f;

            HutongGames.PlayMaker.Actions.FireAtTarget fa = Prefab.GetFSMActionOnState<HutongGames.PlayMaker.Actions.FireAtTarget>("Fire", "spitter");
            fa.speed.Value *= 2.24f;

            GameObject bullet = Prefab.FindGameObjectInChildrenWithName("BulletSprite (1)");
            bullet.transform.localScale = Vector3.one * 3.2f;
        }
    }
}