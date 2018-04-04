using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using nv;

namespace EnemyRandomizerMod
{
    //TODO: move into its own class...
    //parent this to the meme
    public class MemeController : MonoBehaviour
    {
        public BoxCollider2D collider;
        public GameObject meme;

        float memeCooldown = 10f;
        int memeLimit = 0;

        List<GameObject> memes = new List<GameObject>();

        private IEnumerator Start()
        {
            collider = GetComponent<BoxCollider2D>();
            meme = gameObject;

            while( collider == null && meme == null )
            {
                yield return null;
            }

            float timer = 0f;
            while( true )
            {
                yield return new WaitForEndOfFrame();
                timer += Time.deltaTime;
                if( timer > memeCooldown )
                {
                    timer = 0f;
                }
                else
                {
                    continue;
                }

                for( int i = 0; i < memes.Count; )
                {
                    if( memes[ i ] == null )
                    {
                        memes.RemoveAt( i );
                        i = 0;
                        continue;
                    }

                    if( ( memes[ i ].transform.position - meme.transform.position ).magnitude > 20f )
                        memes[ i ].transform.position = meme.transform.position;

                    ++i;
                }

                memeLimit = ( 2000 - ( meme.GetEnemyHP() ) ) / 400;

                if( memes.Count >= memeLimit )
                {
                    continue;
                }

                int replacementIndex = 0;
                GameObject enemyPrefab = EnemyRandomizerLogic.Instance.GetEnemyTypePrefab("Super Spitter", ref replacementIndex);
                if( enemyPrefab == null )
                    continue;

                //Vector3 position = new Vector3(226.4f, 10.0f, 0.0f);
                Vector3 position = meme.transform.position;// + (Vector3)GameRNG.Rand(Vector2.one * -5f, Vector2.one * 5f);
                GameObject newEnemy = EnemyRandomizerLogic.Instance.PlaceEnemy( enemyPrefab, enemyPrefab, replacementIndex, position );

                newEnemy.name = "Rando Sub Enemy: Super Spitter";

                memes.Add( newEnemy );
            }

            yield break;
        }

        private void OnTriggerEnter2D( Collider2D collision )
        {
        }

        public static GameObject CreateMemeEnemy()
        {
            GameObject memeEnemy = null;
            if( GameObject.Find( "Rando Custom: Super Spitter" ) != null )
                return memeEnemy;

            int replacementIndex = 0;

            GameObject enemyPrefab = EnemyRandomizerLogic.Instance.GetEnemyTypePrefab("Super Spitter", ref replacementIndex);
            if( enemyPrefab == null )
                return memeEnemy;

            Dev.Log( "Creating meme " );

            Vector3 position = new Vector3(226.4f, 10.0f, 0.0f);
            //Vector3 position = new Vector3(126.4f, 20.0f, 0.0f);
            GameObject newEnemy = EnemyRandomizerLogic.Instance.PlaceEnemy( enemyPrefab, enemyPrefab, replacementIndex, position );

            memeEnemy = newEnemy;

            newEnemy.name = "Rando Custom: Super Spitter";
            newEnemy.SetEnemyGeoRates( 100, 75, 50 );

            newEnemy.transform.localScale = new Vector3( 3.2f, 3.2f, 3.2f );
            newEnemy.SetEnemyHP( 2000 );

            newEnemy.AddComponent<MemeController>();

            //TODO: copy the roar FSM from another enemy?

            HutongGames.PlayMaker.Actions.DistanceFly df = newEnemy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.DistanceFly>("Distance Fly","spitter");
            df.distance.Value *= 1.5f;
            df.acceleration.Value *= 8f;
            df.speedMax.Value *= 200f;

            HutongGames.PlayMaker.Actions.FireAtTarget fa = newEnemy.GetFSMActionOnState<HutongGames.PlayMaker.Actions.FireAtTarget>("Fire","spitter");
            fa.speed.Value *= 2.24f;

            GameObject bullet = newEnemy.FindGameObjectInChildren( "BulletSprite (1)" );
            bullet.transform.localScale = Vector3.one * 3.2f;

            //DebugOnWake d = EnemyRandomizerLoader.Instance.AddDebugOnWake(newEnemy);
            //d.monitorFSMStates = false;

            //newEnemy.PrintSceneHierarchyTree( true );
            return memeEnemy;
        }
    }

}
