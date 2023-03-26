using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using On.Language;

namespace EnemyRandomizerMod
{
    public class DefaultSpawnedEnemyControl : MonoBehaviour
    {
        public ObjectMetadata thisMetadata; 
        public ObjectMetadata originialMetadata;

        public EnemyDreamnailReaction edr;

        public bool hasCustomDreamnailReaction;
        public string customDreamnailKey;

        public virtual string customDreamnailSourceName { get => originialMetadata == null ? "meme" : originialMetadata.DatabaseName; }

        //TODO: put memes etc here
        public virtual string customDreamnailText { get => $"In another dream, I was a {customDreamnailSourceName}..."; }

        public virtual void Setup(ObjectMetadata other)
        {
            thisMetadata = new ObjectMetadata();
            thisMetadata.Setup(gameObject, EnemyRandomizerDatabase.GetDatabase());
            originialMetadata = other;

            SetDreamnailInfo();
            ConfigureRelativeToReplacement();
        }

        //protected virtual void OnDisable()
        //{
        //    if(thisMetadata.ObjectType == PrefabObject.PrefabType.Effect)
        //    {
        //        ObjectPool.Recycle(gameObject);
        //    }
        //    else if (thisMetadata.ObjectType == PrefabObject.PrefabType.Hazard)
        //    {
        //        ObjectPool.Recycle(gameObject);
        //    }
        //}

        protected virtual void OnDestroy()
        {
            if (hasCustomDreamnailReaction)
            {
                On.EnemyDreamnailReaction.SetConvoTitle -= EnemyDreamnailReaction_SetConvoTitle;
                On.Language.Language.Get_string_string -= Language_Get_string_string;
            }
        }

        protected virtual string Language_Get_string_string(On.Language.Language.orig_Get_string_string orig, string key, string sheetTitle)
        {
            if(key.Contains(customDreamnailKey))
            {
                return customDreamnailText;
            }
            else
            {
                return orig(key, sheetTitle);
            }
        }

        protected virtual void ConfigureRelativeToReplacement()
        {
            if (thisMetadata != null && thisMetadata.ObjectType != PrefabObject.PrefabType.Enemy)
                return;

            if (thisMetadata != null && originialMetadata != null)
            {
                if (thisMetadata.IsBoss && !originialMetadata.IsBoss)
                {
                    SetupBossAsNormalEnemy();
                }

                if (!thisMetadata.IsBoss && originialMetadata.IsBoss)
                {
                    SetupNormalEnemyAsBoss();
                }
            }

            ScaleHP();
        }

        protected virtual void ScaleHP()
        {
            if (thisMetadata != null && originialMetadata != null)
            {
                if (thisMetadata.IsBoss && !originialMetadata.IsBoss)
                {
                    //TODO: config to be not a boss
                    thisMetadata.EnemyHealthManager.hp = originialMetadata.EnemyHealthManager.hp;
                }

                if (!thisMetadata.IsBoss && originialMetadata.IsBoss)
                {
                    //???
                }
            }
        }

        protected virtual void SetDreamnailInfo()
        {
            hasCustomDreamnailReaction = GetComponent<EnemyDreamnailReaction>() != null;
            if (hasCustomDreamnailReaction)
            {
                edr = GetComponent<EnemyDreamnailReaction>();
                customDreamnailKey = Guid.NewGuid().ToString();

                On.Language.Language.Get_string_string -= Language_Get_string_string;
                On.Language.Language.Get_string_string += Language_Get_string_string;

                On.EnemyDreamnailReaction.SetConvoTitle -= EnemyDreamnailReaction_SetConvoTitle;
                On.EnemyDreamnailReaction.SetConvoTitle += EnemyDreamnailReaction_SetConvoTitle;

                SetDreamnailReactionToCustomText();
            }
        }

        private void EnemyDreamnailReaction_SetConvoTitle(On.EnemyDreamnailReaction.orig_SetConvoTitle orig, EnemyDreamnailReaction self, string title)
        {
            if (self != edr || edr == null)
            {
                orig(self, title);
            }
            else
            {
                orig(self, customDreamnailKey);
            }
        }

        protected virtual void SetDreamnailReactionToCustomText()
        {
            if (edr != null)
            {
                try
                {
                    edr.GetType().GetField("convoTitle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .SetValue(edr, customDreamnailKey);
                }
                catch(Exception e)
                {
                    Dev.LogError("Error settings custom dreamnail key for object "+thisMetadata.ScenePath);
                }
            }
        }

        protected virtual void SetupNormalEnemyAsBoss()
        {

        }

        protected virtual void SetupBossAsNormalEnemy()
        {

        }
    }

    public class DefaultPrefabConfig : IPrefabConfig
    {
        public virtual void SetupPrefab(PrefabObject p)
        {
            p.prefabName = EnemyRandomizerDatabase.ToDatabaseKey(p.prefab.name);
        }
    }

    public class DefaultPrefabConfig<TControlComponent> : DefaultPrefabConfig
        where TControlComponent : DefaultSpawnedEnemyControl
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);
            p.prefab.AddComponent<TControlComponent>();
        }
    }

    public class DefaultSpawner : ISpawner
    {
        public virtual GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            GameObject gameObject = null;

            if(p.prefab == null)
            {
                Dev.LogError("Cannot Instantiate a null object!");
            }    

            try
            {
                gameObject = GameObject.Instantiate(p.prefab);
            }
            catch(Exception e)
            {
                Dev.LogError($"Error when trying to instantiate {p.prefab} from {p.prefabType} at {p.source.path} in {p.source.Name}");
            }

            if (gameObject == null)
                return null;

            if(source == null)
                gameObject.name = gameObject.name + "(" + Guid.NewGuid().ToString() + ")"; //name values in parenthesis will be trimmed out when converting to a database key'd name
            else
                gameObject.name = gameObject.name + $" ([{source.ObjectPosition.GetHashCode()}][{source.ScenePath.GetHashCode()}])"; //name values in parenthesis will be trimmed out when converting to a database key'd name
            return gameObject;
        }
    }

    public class DefaultSpawner<TControlComponent> : DefaultSpawner
        where TControlComponent : DefaultSpawnedEnemyControl
    {
        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            var newObject = base.Spawn(p, source);
            var control = newObject.GetComponent<TControlComponent>();
            control.Setup(source);
            return newObject;
        }
    }

    public class CorpseOrientationFixer : MonoBehaviour
    {
        public float corpseAngle;
        public float timeout = 5f;

        IEnumerator Start()
        {
            while (timeout > 0f)
            {
                var angles = transform.localEulerAngles;
                angles.z = corpseAngle;
                transform.localEulerAngles = angles;
                yield return null;
                timeout -= Time.deltaTime;
            }

            yield break;
        }
    }


    public abstract class FSMAreaControlEnemy : DefaultSpawnedEnemyControl
    {
        public PlayMakerFSM control;
        public abstract string FSMName { get; }

        public Range xR;
        public Range yR;

        public Vector3 SpawnPoint;

        protected virtual bool ControlCameraLocks { get => false; }

        protected virtual Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs
        {
            get => new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                { "Tele X Min", x => x.xR.Min},
                { "Tele X Max", x => x.xR.Max},
                { "Tele Y Max", x => x.yR.Max},
                { "Tele Y Min", x => x.yR.Min},
                { "Hero X", x => x.HeroX },
                { "Hero Y", x => x.HeroY },
                { "Left X", x => x.xR.Min},
                { "Right X", x => x.xR.Max},
                { "Top Y", x => x.yR.Max},
                { "Bot Y", x => x.yR.Min},
            };
        }

        protected FsmFloat heroX;
        protected FsmFloat heroY;

        public virtual float HeroX { get => HeroController.instance.transform.position.x; }
        public virtual float HeroY { get => HeroController.instance.transform.position.y; }

        public virtual float XMIN { get => xR.Min; }
        public virtual float XMAX { get => xR.Max; }
        public virtual float YMIN { get => yR.Min; }
        public virtual float YMAX { get => yR.Max; }

        public virtual float MidX { get => xR.Mid; }
        
        protected virtual IEnumerable<CameraLockArea> cams { get; set; }

        protected virtual void BuildArena(Vector3 spawnPoint)
        {
            gameObject.transform.position = spawnPoint;
            var hits = gameObject.GetNearestSurfaces(500f);
            xR = new Range(hits[Vector2.left].point.x, hits[Vector2.right].point.x);
            yR = new Range(hits[Vector2.down].point.y, hits[Vector2.up].point.y);

            if (heroX == null)
            {
                heroX = control.FsmVariables.GetFsmFloat("Hero X");
            }

            if (heroY == null)
            {
                heroY = control.FsmVariables.GetFsmFloat("Hero Y");
            }

            UpdateRefs(control, FloatRefs);
        }

        protected virtual void UpdateRefs(PlayMakerFSM fsm, Dictionary<string, Func<FSMAreaControlEnemy, float>> refs)
        {
            if (fsm == null)
                return;

            foreach (var fref in refs)
            {
                var fvar = fsm.FsmVariables.GetFsmFloat(fref.Key);
                if (fvar != null)
                {
                    fvar.Value = fref.Value.Invoke(this);
                }
            }
        }

        protected virtual void UpdateHeroRefs()
        {
            if (control == null)
                return;

            if (heroX != null && FloatRefs.ContainsKey("Hero X"))
                heroX.Value = FloatRefs["Hero X"].Invoke(this);

            if (heroY != null && FloatRefs.ContainsKey("Hero Y"))
                heroY.Value = FloatRefs["Hero Y"].Invoke(this);
        }

        protected virtual void Hide()
        {
            if (!control.enabled)
                return;

            control.enabled = false;
        }

        protected virtual void Show()
        {
            if (control.enabled)
                return;

            control.enabled = true;
            if (ControlCameraLocks)
                UnlockCameras(cams);
            BuildArena(SpawnPoint);
        }

        protected virtual bool HeroInAggroRange()
        {
            var size = new Vector2(xR.Size, yR.Size);
            var center = new Vector2(xR.Mid, yR.Mid);
            var herop = new Vector2(HeroX, HeroY);
            var dist = herop - center;
            return (dist.sqrMagnitude < size.sqrMagnitude);
        }

        protected virtual IEnumerator Start()
        {
            BuildArena(SpawnPoint);

            if(ControlCameraLocks)
                cams = GetCameraLocksFromScene();

            if (control == null)
                yield break;

            PreloadRefs();

            Hide();

            for (; ; )
            {
                UpdateHeroRefs();

                UpdateCustomRefs();

                if(HeroInAggroRange())
                    Show();
                else
                    Hide();

                yield return new WaitForSeconds(1f);
            }
        }

        protected virtual void PreloadRefs()
        {

        }

        protected virtual void UpdateCustomRefs()
        {

        }

        protected virtual IEnumerable<CameraLockArea> GetCameraLocksFromScene()
        {
            return gameObject.GetComponentsFromScene<CameraLockArea>();
        }

        protected virtual void UnlockCameras(IEnumerable<CameraLockArea> cameraLocks)
        {
            if (!ControlCameraLocks)
                return;

            foreach (var c in cameraLocks)
            {
                c.gameObject.SetActive(false);
            }
        }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
            if(control == null)
            {
                control = gameObject.LocateMyFSM(FSMName);
            }
        }
    }
}
