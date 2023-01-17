using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using nv;

namespace EnemyRandomizerMod
{
    public static class Sandbox
    {
        public static void EnterSandbox()
        {
            GameManager.instance.StartCoroutine(DoEnterSandbox());
        }

        public static IEnumerator DoEnterSandbox()
        {
            //find a source transition
            string currentSceneTransition = GameObject.FindObjectOfType<TransitionPoint>().gameObject.name;
            string currentScene = GameManager.instance.sceneName;

            //update the last entered
            TransitionPoint.lastEntered = currentSceneTransition;

            //place us in sly's storeroom
            GameManager.instance.BeginSceneTransition(new GameManager.SceneLoadInfo
            {
                SceneName = "Room_Sly_Storeroom",
                EntryGateName = "top1",
                HeroLeaveDirection = new GlobalEnums.GatePosition?(GlobalEnums.GatePosition.door),
                EntryDelay = 1f,
                WaitForSceneTransitionCameraFade = true,
                Visualization = GameManager.SceneLoadVisualizations.Default,
                AlwaysUnloadUnusedAssets = false
            });

            while (GameObject.Find("Sly Basement NPC") == null)
                yield return new WaitForEndOfFrame();

            foreach (var roof in GameObject.FindObjectsOfType<Roof>())
            {
                GameObject.Destroy(roof);
            }

            //remove the roofs
            GameObject.Destroy(GameObject.Find("Chunk 0 0").GetComponents<EdgeCollider2D>()[1]);
            GameObject.Destroy(GameObject.Find("Chunk 0 1").GetComponents<EdgeCollider2D>()[1]);


            GameObject.Destroy(GameObject.Find("wall collider"));


            GameObject.Destroy(GameObject.Find("Walk Area"));
            GameObject.Destroy(GameObject.Find("Shop Menu"));
            GameObject.Destroy(GameObject.Find("Sly Basement NPC"));
            GameObject.Destroy(GameObject.Find("Roof Collider (2)"));
            GameObject.Destroy(GameObject.Find("Roof Collider (1)"));
            GameObject.Destroy(GameObject.Find("Sly_Storeroom_0008_18"));
            GameObject.Destroy(GameObject.Find("Sly_Storeroom_0004_21"));
            GameObject.Destroy(GameObject.Find("Sly_Storeroom_0003_22"));
            GameObject.Destroy(GameObject.Find("Sly_Storeroom_0027_1 (3)"));
            GameObject.Destroy(GameObject.Find("Sly_Storeroom_0009_17 (3)"));
            GameObject.Destroy(GameObject.Find("Sly_Storeroom_0027_1 (2)"));


            Scene s = UnityEngine.SceneManagement.SceneManager.GetSceneByName("Room_Sly_Storeroom");

            GameObject.Destroy(s.FindGameObject("Shop Item ShellFrag Sly1(Clone)"));
            GameObject.Destroy(s.FindGameObject("Shop Item VesselFrag Sly1"));
            GameObject.Destroy(s.FindGameObject("Shop Item Ch GeoGatherer(Clone)"));
            GameObject.Destroy(s.FindGameObject("Shop Item Ch Wayward Compass(Clone)"));
            GameObject.Destroy(s.FindGameObject("Shop Item Lantern(Clone)"));
            GameObject.Destroy(s.FindGameObject("Shop Item White Key(Clone)"));
            GameObject.Destroy(s.FindGameObject("Shop Item VesselFrag Sly1"));
            GameObject.Destroy(s.FindGameObject("Shop Item VesselFrag Sly1(Clone)"));

            GameObject.Destroy(s.FindGameObject("Dream Dialogue"));

            foreach (var roof in GameObject.FindObjectsOfType<SpriteRenderer>())
            {
                if (roof.transform.position.x < 80f && roof.transform.position.x > -1f)
                {
                    if (roof.transform.position.y > 5f)
                        GameObject.Destroy(roof.gameObject);

                    else if (roof.transform.position.z < -2f)
                        GameObject.Destroy(roof.gameObject);
                }
            }

            foreach (var roof in GameObject.FindObjectsOfType<MeshRenderer>())
            {
                GameObject.Destroy(roof);
            }

            //Good ground spawn: 64, 6, 0

            //TODO: Make the exit back to the previous scene work
            TransitionPoint exit = GameObject.Find("door1").GetComponent<TransitionPoint>();
            exit.targetScene = currentScene;
            exit.entryPoint = currentSceneTransition;
        }

        //copied and modified from "TransitionPoint.cs"
        public static GlobalEnums.GatePosition GetGatePosition(string name)
        {
            if (name.Contains("top"))
            {
                return GlobalEnums.GatePosition.top;
            }
            if (name.Contains("right"))
            {
                return GlobalEnums.GatePosition.right;
            }
            if (name.Contains("left"))
            {
                return GlobalEnums.GatePosition.left;
            }
            if (name.Contains("bot"))
            {
                return GlobalEnums.GatePosition.bottom;
            }
            if (name.Contains("door"))
            {
                return GlobalEnums.GatePosition.door;
            }
            Dev.LogError("Gate name " + name + "does not conform to a valid gate position type. Make sure gate name has the form 'left1'");
            return GlobalEnums.GatePosition.unknown;
        }

        //from will be top1,left1,right1,door1,etc...
        public static IEnumerator EnterZone(string name, string enterFrom, string exitTransition, string waitUntilGameObjectIsLoaded = "", List<string> removeList = null)
        {
            Dev.Where();
            //find a source transition
            string currentSceneTransition = GameObject.FindObjectOfType<TransitionPoint>().gameObject.name;
            string currentScene = GameManager.instance.sceneName;

            //update the last entered
            TransitionPoint.lastEntered = currentSceneTransition;

            Dev.Log("Creating transition");
            GameManager.instance.BeginSceneTransition(new GameManager.SceneLoadInfo
            {
                SceneName = name,
                EntryGateName = enterFrom,
                HeroLeaveDirection = new GlobalEnums.GatePosition?(GetGatePosition(exitTransition)),
                EntryDelay = 0.1f,
                WaitForSceneTransitionCameraFade = true,
                Visualization = GameManager.SceneLoadVisualizations.Default,
                AlwaysUnloadUnusedAssets = false
            });

            Dev.Log("waitUntilGameObjectIsLoaded??");
            if (!string.IsNullOrEmpty(waitUntilGameObjectIsLoaded))
            {
                while (GameObject.Find(waitUntilGameObjectIsLoaded) == null)
                    yield return new WaitForEndOfFrame();
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
            Dev.Log("Done!");

            if (removeList != null && removeList.Count > 0)
            {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(name);
                foreach (string s in removeList)
                {
                    GameObject.Destroy(scene.FindGameObject(s));
                }
            }
        }

        public static void SetNoclip(bool state)
        {
            noclip = state;

            if (noclip)
            {
                noclipPos = HeroController.instance.gameObject.transform.position;
                noClipRunner.UpdateBehavior = DoNoclip;
                noClipRunner.Start();
            }
            else
            {
                noClipRunner.Reset();
            }
        }

        public static bool NoClipState
        {
            get
            {
                return noclip;
            }
        }

        static SmartRoutine noClipRunner = new SmartRoutine();
        static Vector3 noclipPos;
        static bool noclip = false;
        public static IEnumerator DoNoclip(params object[] args)
        {
            while (HeroController.instance == null || HeroController.instance.gameObject == null || !HeroController.instance.gameObject.activeInHierarchy)
                yield return null;

            for (; ; )
            {
                if (noclip)
                {
                    if (GameManager.instance.inputHandler.inputActions.left.IsPressed)
                    {
                        noclipPos = new Vector3(noclipPos.x - Time.deltaTime * 20f, noclipPos.y, noclipPos.z);
                    }

                    if (GameManager.instance.inputHandler.inputActions.right.IsPressed)
                    {
                        noclipPos = new Vector3(noclipPos.x + Time.deltaTime * 20f, noclipPos.y, noclipPos.z);
                    }

                    if (GameManager.instance.inputHandler.inputActions.up.IsPressed)
                    {
                        noclipPos = new Vector3(noclipPos.x, noclipPos.y + Time.deltaTime * 20f, noclipPos.z);
                    }

                    if (GameManager.instance.inputHandler.inputActions.down.IsPressed)
                    {
                        noclipPos = new Vector3(noclipPos.x, noclipPos.y - Time.deltaTime * 20f, noclipPos.z);
                    }

                    if (HeroController.instance.transitionState.ToString() == "WAITING_TO_TRANSITION")
                    {
                        HeroController.instance.gameObject.transform.position = noclipPos;
                    }
                    else
                    {
                        noclipPos = HeroController.instance.gameObject.transform.position;
                    }
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
