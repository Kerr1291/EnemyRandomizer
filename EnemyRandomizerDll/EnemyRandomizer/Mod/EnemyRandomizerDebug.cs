using System.Collections;
using System.Collections.Generic;
using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;

using System.Linq;
using UniRx;
using System;
using System.Reflection;

namespace EnemyRandomizerMod
{
    public partial class EnemyRandomizer
    {
        //static SmartRoutine debugInputRoutine = new SmartRoutine();
        //static SmartRoutine noClipRoutine = new SmartRoutine();
        static string debugRecentHit = "";

        //static void SetNoClip(On.UIManager.orig_UIClosePauseMenu orig, UIManager self)
        //{
        //    orig.Invoke(self);
        //    bool noClip = EnemyRandomizer.GlobalSettings.RandomizeBosses;
        //    if (!noClip)
        //        noClipRoutine.Reset();
        //    else
        //        noClipRoutine = new SmartRoutine(DoNoClip());
        //}

        //static IEnumerator DoNoClip()
        //{
        //    Vector3 noclipPos = HeroController.instance.gameObject.transform.position;
        //    while (EnemyRandomizer.GlobalSettings.RandomizeBosses)
        //    {
        //        yield return null;

        //        if (HeroController.instance == null || HeroController.instance.gameObject == null || !HeroController.instance.gameObject.activeInHierarchy)
        //            continue;

        //        if (EnemyRandomizer.GlobalSettings.RandomizeBosses)
        //        {
        //            if (GameManager.instance.inputHandler.inputActions.left.IsPressed)
        //            {
        //                noclipPos = new Vector3(noclipPos.x - Time.deltaTime * 20f, noclipPos.y, noclipPos.z);
        //            }

        //            if (GameManager.instance.inputHandler.inputActions.right.IsPressed)
        //            {
        //                noclipPos = new Vector3(noclipPos.x + Time.deltaTime * 20f, noclipPos.y, noclipPos.z);
        //            }

        //            if (GameManager.instance.inputHandler.inputActions.up.IsPressed)
        //            {
        //                noclipPos = new Vector3(noclipPos.x, noclipPos.y + Time.deltaTime * 20f, noclipPos.z);
        //            }

        //            if (GameManager.instance.inputHandler.inputActions.down.IsPressed)
        //            {
        //                noclipPos = new Vector3(noclipPos.x, noclipPos.y - Time.deltaTime * 20f, noclipPos.z);
        //            }

        //            if (HeroController.instance.transitionState.ToString() == "WAITING_TO_TRANSITION")
        //            {
        //                HeroController.instance.gameObject.transform.position = noclipPos;
        //            }
        //            else
        //            {
        //                noclipPos = HeroController.instance.gameObject.transform.position;
        //            }
        //        }
        //    }
        //}

#if DEBUG
        //public static void SetDebugInput(bool enabled)
        //{
        //    if (enabled)
        //        debugInputRoutine = new SmartRoutine(DebugInput());
        //    else
        //        debugInputRoutine.Reset();
        //}

        static void DebugPrintObjectOnHit(Collider2D otherCollider, GameObject gameObject)
        {
            if (otherCollider.gameObject.name != debugRecentHit)
            {
                Dev.Log("Hero at " + HeroController.instance.transform.position + " HIT: " + otherCollider.gameObject.name + " at (" + otherCollider.gameObject.transform.position + ")");
                debugRecentHit = otherCollider.gameObject.name;
            }
        }

        static IEnumerator DebugInput()
        {
            for (; ; )
            {
                yield return new WaitForEndOfFrame();
                if (UnityEngine.Input.GetKeyDown(KeyCode.O))
                {
                    for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
                    {
                        Scene s = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                        bool status = s.IsValid();
                        if (status)
                        {
                            string outputPath = Application.dataPath + "/Managed/Mods/EnemyRandomizer/" + s.name;
                            Dev.Log("Dumping Loaded Scene to " + outputPath);
                            s.PrintHierarchy(outputPath, true);
                        }
                    }
                }
            }

            yield break;
        }

        //public static GameObject DebugSpawnEnemy(string enemyName, bool forcePlaceOnGround = false)
        //{
        //    try
        //    {
        //        var enemy = EnemyRandomizer.Instance.database.Spawn(enemyName);
        //        if (enemy != null)
        //        {
        //            var pos = HeroController.instance.transform.position + Vector3.right * 5f;
        //            //if(forcePlaceOnGround || data.randomizerObject.IsFlyer)
        //            //    pos = Mathnv.GetPointOn(pos, Vector2.down, 500f, EnemyRandomizer.IsSurfaceOrPlatform);                    
        //            //enemy.SetupRandomizerComponents(data.randomizerObject, null, null);
        //            enemy.transform.position = pos;
        //            enemy.SetActive(true);
        //            return enemy;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Dev.LogError("Error: " + e.Message);
        //    }

        //    return null;
        //}
#endif
    }
}
