using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using UniRx;
using EnemyRandomizerMod;
using System;
using System.Collections;
using Satchel;
using Satchel.Futils;

#if DEBUG
public class edc : GameSingleton<edc>
{
    public DebugColliders debugColliders;
    public bool loaded = false;
    public bool flipRay = false;
    private IEnumerator<string> debugTestEnemyIter;
    float debugRayDistance = 50f;

    public static Vector2 heropos => HeroController.instance.transform.position;
    public static Vector2 mousepos => GetInGameWorldPositionFromMouse();
    public static float heroposXoffset = 5f;
    public static bool keybindsEnabled = false;
    private static bool periodKeyPressed = false;
    private static float periodKeyPressTime = 0f;

    private const float doubleKeyPressDelay = 0.5f;
    static IEnumerator debugInput = null;
    static bool forceSingleEntry = false;

    public static string lastSpawnedName;
    public static GameObject lastSpawned;
    public static SpawnedObjectControl soc => lastSpawned == null ? null : lastSpawned.GetComponent<SpawnedObjectControl>();
    public static PlayMakerFSM controlFSM => soc == null ? null : soc.control;
    public static float customSpawnOffset = 0f;

    public static void spawn(string enemyName, string replacement = null)
    {
        Dev.Log($"Spawning a {enemyName} to the right of the hero");
        lastSpawned = EnemyRandomizer.CustomSpawn(heropos + Vector2.right * heroposXoffset, enemyName, replacement, true);
        customSpawnOffset = soc.spawnPositionOffset;
        lastSpawnedName = enemyName;
        Dev.Log("spawned with offset " + soc.spawnPositionOffset);
        Dev.Log("--");
    }

    public static void spawn_withoff(string enemyName, float offset)
    {
        customSpawnOffset = offset;
        Dev.Log($"Spawning a {enemyName} to the right of the hero");
        lastSpawned = EnemyRandomizer.CustomSpawn(heropos + Vector2.right * heroposXoffset, enemyName, null, true);
        soc.spawnPositionOffsetOverride = offset;
        soc.SetPositionOnSpawn(null);
        lastSpawnedName = enemyName;
        Dev.Log("spawned with custom offset " + soc.spawnPositionOffsetOverride);
        Dev.Log("--");
    }

    public static void repos()
    {
        if (lastSpawned == null)
            return;

        lastSpawned.transform.position = heropos + Vector2.right * heroposXoffset;

        var poob = lastSpawned.GetComponent<PreventOutOfBounds>();
        if (poob != null)
            poob.ForcePosition(heropos + Vector2.right * heroposXoffset);

        if (soc != null)
            soc.SetPositionOnSpawn(null);
        var offset = soc.spawnPositionOffsetOverride == null ? soc.spawnPositionOffset : soc.spawnPositionOffsetOverride.Value;
        Dev.Log("positioned with offset " + offset);
    }

    public static void repos(float offset)
    {
        if (lastSpawned == null)
            return;

        lastSpawned.transform.position = heropos + Vector2.right * heroposXoffset;

        var poob = lastSpawned.GetComponent<PreventOutOfBounds>();
        if (poob != null)
            poob.ForcePosition(heropos + Vector2.right * heroposXoffset);

        if (soc != null)
        {
            customSpawnOffset = offset;
            soc.spawnPositionOffsetOverride = offset;
            soc.SetPositionOnSpawn(null);
        }

        Dev.Log("positioned with offset " + soc.spawnPositionOffsetOverride);
    }

    public static void repos_up()
    {
        if(soc != null)
        {
            customSpawnOffset += 0.1f;
            repos(customSpawnOffset);
        }
    }

    public static void repos_down()
    {
        if (soc != null)
        {
            customSpawnOffset -= 0.1f;
            repos(customSpawnOffset);
        }
    }

    public static void respawn_up()
    {
        customSpawnOffset += 0.1f;
        respawn(customSpawnOffset);
    }

    public static void respawn_down()
    {
        customSpawnOffset -= 0.1f;
        respawn(customSpawnOffset);
    }

    public static void _delete()
    {
        if (lastSpawned != null)
        {
            lastSpawned.SetActive(false);
            GameObject.Destroy(lastSpawned);
            lastSpawned = null;
        }
    }

    public static void respawn()
    {
        if(lastSpawned != null)
        {
            lastSpawned.SetActive(false);
            GameObject.Destroy(lastSpawned);
        }
        spawn(lastSpawnedName);
    }

    public static void respawn_withoff()
    {
        if (lastSpawned != null)
        {
            lastSpawned.SetActive(false);
            GameObject.Destroy(lastSpawned);
        }
        spawn(lastSpawnedName);
    }

    public static void respawn(float offset)
    {
        if (lastSpawned != null)
        {
            lastSpawned.SetActive(false);
            GameObject.Destroy(lastSpawned);
        }
        customSpawnOffset = offset;
        spawn_withoff(lastSpawnedName, offset);
    }

    public static void printstate()
    {
        if (controlFSM == null)
            return;

        Dev.Log($"{lastSpawned}: active state = {controlFSM.ActiveStateName}");
        Dev.Log("--");
    }

    public static void printtransitions()
    {
        if (controlFSM == null)
            return;

        var data = controlFSM.GetState(controlFSM.ActiveStateName).Transitions.Select(x => $"\n {x.EventName} -> {x.ToState}").ToList();
        var fdata = string.Join(" ", data);
        Dev.Log($"{lastSpawned}: {controlFSM.ActiveStateName} Has Transitions: {fdata}");
        Dev.Log("--");
    }

    public static void printactions()
    {
        if (controlFSM == null)
            return;

        var data = controlFSM.GetState(controlFSM.ActiveStateName).Actions.Select(x => $"\n [{x.Name}, {x.GetType().Name}]").ToList();
        var fdata = string.Join(" ", data);
        Dev.Log($"{lastSpawned}: {controlFSM.ActiveStateName} Has Actions: {fdata}");
        Dev.Log("--");
    }

    public static void sendevent(string eventName)
    {
        if (controlFSM == null)
            return;
        controlFSM.SendEvent(eventName);
        Dev.Log($"{lastSpawned}: {controlFSM.ActiveStateName} Send Event: {eventName}");
        Dev.Log("--");
    }

    public static void setstate(string stateName)
    {
        if (controlFSM == null)
            return;
        controlFSM.SetState(stateName);
        Dev.Log($"{lastSpawned}: {controlFSM.ActiveStateName} Set State {stateName}");
        Dev.Log("--");
    }

    public static void nexttest()
    {
        if (lastSpawned != null)
        {
            lastSpawned.SetActive(false);
            GameObject.Destroy(lastSpawned);
        }
        Dev.Log("Spawning a debug test enemy to the right of the hero");
        Instance.HandleDebugSpawnEnemy();
        Dev.Log("--");
    }

    public static void lockpos()
    {
        if(lastSpawned != null)
        {
            var pl = lastSpawned.GetOrAddComponent<PositionLocker>();
        }
    }

    public static void lockpos(float x, float y)
    {
        if (lastSpawned != null)
        {
            var pl = lastSpawned.GetOrAddComponent<PositionLocker>();
            pl.positionLock = new Vector2(x, y);

            var poob = lastSpawned.GetComponent<PreventOutOfBounds>();
            if (poob != null)
                poob.ForcePosition(pl.positionLock.Value);
        }
    }

    public static void setposoff(float x, float y)
    {
        if (lastSpawned != null)
        {
            var pl = lastSpawned.GetOrAddComponent<PositionLocker>();
            pl.positionLock = heropos + new Vector2(x, y);

            var poob = lastSpawned.GetComponent<PreventOutOfBounds>();
            if (poob != null)
                poob.ForcePosition(pl.positionLock.Value);
        }
    }

    public static void unlockpos()
    {
        if (lastSpawned != null)
        {
            var pl = lastSpawned.GetComponent<PositionLocker>();
            if (pl != null)
                GameObject.Destroy(pl);
        }
    }

    public static void addfilter(string filter)
    {
        DevLogger.Instance.IgnoreFilters.Add(filter);
    }

    public static void removefilter(string filter)
    {
        DevLogger.Instance.IgnoreFilters.Remove(filter);
    }

    public static void showlog()
    {
        Dev.Logger.LoggingEnabled = true;
        Dev.Logger.GuiLoggingEnabled = true;
        DevLogger.Instance.ShowSlider();
        DevLogger.Instance.Show(true);
    }

    public static void hidelog()
    {
        Dev.Logger.GuiLoggingEnabled = false;
        DevLogger.Instance.HideSlider();
        DevLogger.Instance.Show(false);
        Dev.Logger.LoggingEnabled = true;
    }

    public static void disablelog()
    {
        Dev.Logger.GuiLoggingEnabled = false;
        DevLogger.Instance.HideSlider();
        DevLogger.Instance.Show(false);
        Dev.Logger.LoggingEnabled = false;
    }

    public static void logv(bool enabled)
    {
        EnemyReplacer.VERBOSE_LOGGING = enabled;
        SpawnedObjectControl.VERBOSE_DEBUG = enabled;
    }

    public static void logvv(bool enabled)
    {
        EnemyReplacer.VVERBOSE_LOGGING = enabled;
    }

    public IEnumerator DebugInput()
    {
        bool suspended = false;
        for (; ; )
        {
            if (suspended)
                Time.timeScale = 0f;

            if (!CheckKeybindsEnabled())
            {
                if (Input.GetKeyDown(KeyCode.I))
                {
                    Dev.Log("Press period twice to enable/disable debug keys, then use this key again for info");
                    Dev.Log("Press period twice to enable/disable debug keys, then use this key again for info");
                }
                yield return null;
                continue;
            }


            if (Input.GetKeyDown(KeyCode.I))
            {
                Dev.Log("Keys:");
                Dev.Log("I: this info");
                Dev.Log("period twice: enable/disable these debug keys");
                Dev.Log("Delete: Delete recently spawned enemy");
                Dev.Log("Backspace: Delete ALL enemies");
                Dev.Log("[ and ]: reposition enemy down/up");
                Dev.Log("1 and 2: respawn and reposition enemy down/up");
                Dev.Log("3: print current fsm state");
                Dev.Log("4: toggle debug collider rendering");
                Dev.Log("5: toggle rendering disabled colliders");
                Dev.Log("6: restart debug test iterator");
                Dev.Log("7: respawn test");
                Dev.Log("8: spawn next test");
                Dev.Log("9: clear debug rays");
                Dev.Log("0: flip debug ray generation");
                Dev.Log("U/J/H/K: spawn debug rays out of hero (or two hero if flipped)");
                Dev.Log("Q/W/E/R: timescale control, freeze/1-frame/many-frames/unfreeze");
                Dev.Log("--");
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Dev.Log("Deleting enemy");
                _delete();
                Dev.Log("--");
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                Dev.Log("Deleting all enemies");
                foreach(var soc in SpawnedObjectControl.GetAll)
                {
                    if(soc.gameObject.ObjectType() == PrefabObject.PrefabType.Enemy)
                        GameObject.Destroy(soc.gameObject);
                }
                Dev.Log("--");
            }

            if (Input.GetKeyDown(KeyCode.RightBracket))
            {
                Dev.Log("Repositioning the enemy up a bit");
                repos_up();
                Dev.Log("--");
            }

            if (Input.GetKeyDown(KeyCode.LeftBracket))
            {
                Dev.Log("Repositioning the enemy down a bit");
                repos_down();
                Dev.Log("--");
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Dev.Log("Respawn the enemy up a bit");
                respawn_up();
                Dev.Log("--");
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Dev.Log("Printing current fsm state of last enemy");
                printstate();
                Dev.Log("--");
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Dev.Log("Respawn the enemy down a bit");
                respawn_down();
                Dev.Log("--");
            }

            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                Dev.Log("respawn test");
                respawn();
                Dev.Log("--");
            }

            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                Dev.Log("Spawning a debug test enemy to the right of the hero");
                HandleDebugSpawnEnemy();
                Dev.Log("--");
            }

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                Dev.Log("flipping ray generation direction");
                HandleFlipRay();
                Dev.Log("--");
            }

            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                Dev.Log("Clearing spawned debug rays");
                HandleClearRays();
                Dev.Log("--");
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                Dev.Log("Restarting the debug spawn enemy test iter");
                HandleResetIter();
                Dev.Log("--");
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                Dev.Log("Spawning a ray between the hero and something UP");
                if (Input.GetKey(KeyCode.LeftShift))
                    MakeHeroRaysUp(debugRayDistance, flipRay);
                else
                    MakeHeroRayUp(debugRayDistance, flipRay);
                Dev.Log("--");
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                Dev.Log("Spawning a ray between the hero and something DOWN");
                if (Input.GetKey(KeyCode.LeftShift))
                    MakeHeroRaysDown(debugRayDistance, flipRay);
                else
                    MakeHeroRayDown(debugRayDistance, flipRay);
                Dev.Log("--");
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                Dev.Log("Spawning a ray between the hero and something LEFT");
                if (Input.GetKey(KeyCode.LeftShift))
                    MakeHeroRaysLeft(debugRayDistance, flipRay);
                else
                    MakeHeroRayLeft(debugRayDistance, flipRay);
                Dev.Log("--");
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                Dev.Log("Spawning a ray between the hero and something RIGHT");
                if (Input.GetKey(KeyCode.LeftShift))
                    MakeHeroRaysRight(debugRayDistance, flipRay);
                else
                    MakeHeroRayRight(debugRayDistance, flipRay);
                Dev.Log("--");
            }

            //toggle renderers on/off
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha4))
            {
                Dev.Log("Renderer state toggled   ");
                //toggle renderers on/off
                if (UnityEngine.Input.GetKeyDown(KeyCode.Slash))
                {
                    var dc = GameObjectExtensions.FindObjectsOfType<DebugColliders>(true, false);
                    dc.ForEach(x => x.ToggleRenderers());
                }
                Dev.Log("--");
            }

            //disable rendering
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha5))
            {
                Dev.Log(@"Toggling allow rendering disabled colliders  ");
                DebugColliders.renderDisabledColliders = !DebugColliders.renderDisabledColliders;
                Dev.Log("--");
            }

            //suspend
            if (!forceSingleEntry && UnityEngine.Input.GetKeyDown(KeyCode.Q))
            {
                Dev.Log("Timescale suspended with key : Q  ");
                forceSingleEntry = true;
                Time.timeScale = 0f;
                suspended = true;
                Dev.Log("--");
            }
            //advance by about one frame
            if (!forceSingleEntry && UnityEngine.Input.GetKeyDown(KeyCode.W))
            {
                Dev.Log("Timescale moved forward by 1 frame with key : W  ");
                forceSingleEntry = true;
                Time.timeScale = 1f;
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                Time.timeScale = 0f;
                Dev.Log("--");
            }
            //advance by many frames (hold R)
            if (!forceSingleEntry && UnityEngine.Input.GetKey(KeyCode.R))
            {
                Dev.Log("Timescale moving with key : R  ");
                forceSingleEntry = true;
                Time.timeScale = 1f;
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                Time.timeScale = 0f;
                Dev.Log("--");
            }
            //resume from suspend
            if (!forceSingleEntry && UnityEngine.Input.GetKeyDown(KeyCode.E))
            {
                Dev.Log("Timescale resumed with key : E  ");
                forceSingleEntry = true;
                Time.timeScale = 1f;
                suspended = false;
                Dev.Log("--");
            }

            yield return new WaitForEndOfFrame();
            forceSingleEntry = false;
        }
        yield break;
    }

    public static Vector2 GetInGameWorldPositionFromMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.transform.position.z;

        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    public static GameObject DebugSpawnEnemy(string enemyName, string replacement = null, bool useMouse = true)
    {
        if (!useMouse)
        {
            spawn(enemyName, replacement);
            return lastSpawned;
        }
        else
        {
            lastSpawned = EnemyRandomizer.CustomSpawn(useMouse ? mousepos : heropos, enemyName, replacement, true);
            customSpawnOffset = soc.spawnPositionOffset;
            lastSpawnedName = enemyName;
            return lastSpawned;
        }
    }

    public static void MakeRays(Vector2 from, Vector2 to)
    {
        List<RaycastHit2D> hits = SpawnerExtensions.GetRaysOn(from, (to - from).normalized, (to - from).magnitude);
        edc.Instance.debugColliders.CreateRayFromRaycastHits(edc.Instance.gameObject, from, to, hits);
    }

    public static void MakeRay(Vector2 from, Vector2 to)
    {
        List<RaycastHit2D> hits = new List<RaycastHit2D>() { SpawnerExtensions.GetRayOn(from, (to - from).normalized, (to - from).magnitude) };
        edc.Instance.debugColliders.CreateRayFromRaycastHits(edc.Instance.gameObject, from, to, hits);
    }

    private void HandleDebugSpawnEnemy()
    {
        _delete();
        if (debugTestEnemyIter.MoveNext())
        {
            var spawnName = debugTestEnemyIter.Current;
            lastSpawned = DebugSpawnEnemy(spawnName, null);
            lastSpawnedName = spawnName;
            if(lastSpawned.GetDatabaseKey() != spawnName)
            {
                Dev.LogWarning("Test spawned a different object than requested");
                Dev.LogWarning($"requested {spawnName}");
                Dev.LogWarning($"got {lastSpawned.GetDatabaseKey()}");
                Dev.Log("--");
            }
        }
        else
        {
            ResetIter();
        }
    }

    private void HandleFlipRay()
    {
        flipRay = !flipRay;
    }

    private void HandleClearRays()
    {
        ClearRays();
    }

    private void HandleResetIter()
    {
        ResetIter();
    }


    public static void MakeRay(float fx, float fy, float tx, float ty)
    {
        edc.MakeRay(new Vector2(fx, fy), new Vector2(tx, ty));
    }

    public static void MakeRays(float fx, float fy, float tx, float ty)
    {
        edc.MakeRays(new Vector2(fx, fy), new Vector2(tx, ty));
    }

    public static void MakeHeroRay(float x, float y, bool toHero = false)
    {
        var h = HeroController.instance.transform.position;
        if (toHero)
        {
            edc.MakeRay(new Vector2(x, y), new Vector2(h.x, h.y));
        }
        else
        {
            edc.MakeRay(new Vector2(h.x, h.y), new Vector2(x, y));
        }
    }

    public static void MakeHeroRays(float x, float y, bool toHero = false)
    {
        var h = HeroController.instance.transform.position;
        if (toHero)
        {
            edc.MakeRays(new Vector2(x, y), new Vector2(h.x, h.y));
        }
        else
        {
            edc.MakeRays(new Vector2(h.x, h.y), new Vector2(x, y));
        }
    }

    public static void MakeHeroRayLeft(float dist, bool toHero = false)
    {
        var h = HeroController.instance.transform.position;
        if (toHero)
        {
            edc.MakeRay(new Vector2(h.x - dist, h.y), new Vector2(h.x, h.y));
        }
        else
        {
            edc.MakeRay(new Vector2(h.x, h.y), new Vector2(h.x - dist, h.y));
        }
    }

    public static void MakeHeroRayRight(float dist, bool toHero = false)
    {
        var h = HeroController.instance.transform.position;
        if (toHero)
        {
            edc.MakeRay(new Vector2(h.x + dist, h.y), new Vector2(h.x, h.y));
        }
        else
        {
            edc.MakeRay(new Vector2(h.x, h.y), new Vector2(h.x + dist, h.y));
        }
    }


    public static void MakeHeroRayUp(float dist, bool toHero = false)
    {
        var h = HeroController.instance.transform.position;
        if (toHero)
        {
            edc.MakeRay(new Vector2(h.x, h.y + dist), new Vector2(h.x, h.y));
        }
        else
        {
            edc.MakeRay(new Vector2(h.x, h.y), new Vector2(h.x, h.y + dist));
        }
    }

    public static void MakeHeroRayDown(float dist, bool toHero = false)
    {
        var h = HeroController.instance.transform.position;
        if (toHero)
        {
            edc.MakeRay(new Vector2(h.x, h.y - dist), new Vector2(h.x, h.y));
        }
        else
        {
            edc.MakeRay(new Vector2(h.x, h.y), new Vector2(h.x, h.y - dist));
        }
    }








    public static void MakeHeroRaysLeft(float dist, bool toHero = false)
    {
        var h = HeroController.instance.transform.position;
        if (toHero)
        {
            edc.MakeRays(new Vector2(h.x - dist, h.y), new Vector2(h.x, h.y));
        }
        else
        {
            edc.MakeRays(new Vector2(h.x, h.y), new Vector2(h.x - dist, h.y));
        }
    }

    public static void MakeHeroRaysRight(float dist, bool toHero = false)
    {
        var h = HeroController.instance.transform.position;
        if (toHero)
        {
            edc.MakeRays(new Vector2(h.x + dist, h.y), new Vector2(h.x, h.y));
        }
        else
        {
            edc.MakeRays(new Vector2(h.x, h.y), new Vector2(h.x + dist, h.y));
        }
    }


    public static void MakeHeroRaysUp(float dist, bool toHero = false)
    {
        var h = HeroController.instance.transform.position;
        if (toHero)
        {
            edc.MakeRays(new Vector2(h.x, h.y + dist), new Vector2(h.x, h.y));
        }
        else
        {
            edc.MakeRays(new Vector2(h.x, h.y), new Vector2(h.x, h.y + dist));
        }
    }

    public static void MakeHeroRaysDown(float dist, bool toHero = false)
    {
        var h = HeroController.instance.transform.position;
        if (toHero)
        {
            edc.MakeRays(new Vector2(h.x, h.y - dist), new Vector2(h.x, h.y));
        }
        else
        {
            edc.MakeRays(new Vector2(h.x, h.y), new Vector2(h.x, h.y - dist));
        }
    }


    private void Awake()
    {
        debugColliders = gameObject.GetOrAddComponent<DebugColliders>();
        ResetIter();
    }

    private void ResetIter()
    {
        debugTestEnemyIter = MetaDataTypes.DebugTestEnemies.Keys.ToList().GetEnumerator();
    }



    public static void ClearRays()
    {
        edc.Instance.debugColliders.ClearRays();
    }


    private void OnEnable()
    {
        if (debugInput == null)
        {
            debugInput = DebugInput();
        }

        GameManager.instance.StartCoroutine(debugInput);
    }

    private bool CheckKeybindsEnabled()
    {
        if (Input.GetKeyDown(KeyCode.Period))
        {
            if (periodKeyPressed && (Time.unscaledTime - periodKeyPressTime) < doubleKeyPressDelay)
            {
                keybindsEnabled = !keybindsEnabled;

                if (keybindsEnabled)
                {
                    Dev.Log("Debug keys enabled");
                }
                else
                {
                    Dev.Log("Debug keys disabled");
                }
                Dev.Log("--");

                periodKeyPressed = false;
                periodKeyPressTime = 0f;
            }
            else
            {
                periodKeyPressed = true;
                periodKeyPressTime = Time.unscaledTime;
            }
        }
        else
        {
            if (periodKeyPressed && (Time.unscaledTime - periodKeyPressTime) >= doubleKeyPressDelay)
            {
                periodKeyPressed = false;
                periodKeyPressTime = 0f;
            }
        }

        return keybindsEnabled;
    }
}
#endif





//TODO: will need to do in a different place since other mods could load their modules later on
//Dev.Log("checking missing");
//List<string> missingLogics = EnemyRandomizer.instance.enemyReplacer.loadedLogics.Select(x => x.Name).Where(x => !GlobalSettings.loadedLogics.Contains(x)).ToList();

//missingLogics.ForEach(x =>
//{
//    Dev.LogWarning($"Last time EnemyRandomizer had loaded the logic module {x}, which no longer exists!");
//    //TODO: remove them? for now just post a warning
//});
//}