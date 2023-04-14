using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using On.Language;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Satchel;
using Satchel.Futils;

namespace EnemyRandomizerMod
{
    public class DefaultSpawnedEnemyControl : MonoBehaviour
    {
        public ObjectMetadata thisMetadata; 
        public ObjectMetadata originialMetadata;
        public DebugColliders debugColliders;

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
#if DEBUG
            debugColliders = gameObject.AddComponent<DebugColliders>();
#endif
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
                    thisMetadata.EnemyHealthManager.hp = ScaleHPFromBossToNormal(thisMetadata.EnemyHealthManager.hp, originialMetadata.EnemyHealthManager.hp);
                }

                if (!thisMetadata.IsBoss && originialMetadata.IsBoss)
                {
                    thisMetadata.EnemyHealthManager.hp = ScaleHPFromBossToNormal(thisMetadata.EnemyHealthManager.hp, originialMetadata.EnemyHealthManager.hp);
                }
            }
        }

        protected virtual int ScaleHPFromBossToNormal(int defaultHP, int previousHP)
        {
            return previousHP;
        }

        protected virtual int ScaleHPFromNormalToBoss(int defaultHP, int previousHP)
        {
            return previousHP;
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


        protected virtual void DisableSendEvents(PlayMakerFSM fsm, params (string StateName, int ActionIndex)[] stateActions)
        {
            foreach (var sa in stateActions)
            {
                fsm.GetState(sa.StateName).GetAction<SendEventByName>(sa.ActionIndex).sendEvent = string.Empty;
            }
        }

        protected virtual void ChangeRandomIntRange(PlayMakerFSM fsm, string stateName, int min, int max)
        {
            fsm.GetState(stateName).GetFirstActionOfType<RandomInt>().min.Value = min;
            fsm.GetState(stateName).GetFirstActionOfType<RandomInt>().max.Value = max;
        }

        protected virtual void SetAudioOneShotVolume(PlayMakerFSM fsm, string stateName, float vol = 0f)
        {
            fsm.GetState(stateName).GetFirstActionOfType<AudioPlayerOneShotSingle>().volume = vol;
        }

        /// <summary>
        /// WARNING: will remove ALL previous actions on the state
        /// </summary>
        protected virtual void OverrideState(PlayMakerFSM fsm, string stateName, Action stateAction)
        {
            var overrideState = fsm.GetState(stateName);
            overrideState.Actions = new FsmStateAction[] {
                new CustomFsmAction(stateAction)
            };
        }

        protected virtual Vector2 GetUpFromSelfAngle( bool isFlipped )
        {
            Vector2 up = Vector2.zero;

            float angle = transform.localEulerAngles.z % 360f;

            if (angle < 5f && angle < 355f)
            {
                up = Vector2.up;
            }
            else if (angle > 85f && angle < 95f)
            {
                up = Vector2.left;
            }
            else if (angle > 175f && angle < 185f)
            {
                up = Vector2.down;
            }
            else if (angle > 265f || angle < 275f)
            {
                up = Vector2.right;
            }

            return up;
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
            var control = newObject.GetOrAddComponent<TControlComponent>();
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
}
