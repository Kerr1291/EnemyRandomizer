using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;

using UniRx;

namespace EnemyRandomizerMod
{
    public class ManagedObject : MonoBehaviour
    {
        public ObjectMetadata myMetaData;
        public ObjectMetadata replacedMetaData;

        /// <summary>
        /// True if this has not been replaced
        /// </summary>
        public bool ThisIsSourceObject
        {
            get
            {
                return replacedMetaData == null ||
                    myMetaData == replacedMetaData ||
                    ((myMetaData != null && replacedMetaData != null) &&
                    myMetaData.Source == replacedMetaData.Source);
            }
        }

        public bool ThisHasBeenReplaced
        {
            get
            {
                return !ThisIsSourceObject;
            }
        }

        /// <summary>
        /// Pass "null" for the replacement if setting up a custom enemy
        /// </summary>
        public virtual void Setup(ObjectMetadata source, ObjectMetadata replacement)
        {
            myMetaData = source;
            replacedMetaData = replacement;
        }
    }

    public class BattleManagedObject : ManagedObject
    {
        public override void Setup(ObjectMetadata source, ObjectMetadata replacement)
        {
            base.Setup(source, replacement);

            RegisterWithBattleManager();
        }

        public virtual void RegisterWithBattleManager()
        {
            if (BattleManager.StateMachine.Value == null)
            {
                BattleManager.DoSceneCheck(gameObject);
            }

            if(BattleManager.StateMachine.Value == null)
            {
                StartCoroutine(FindStateMachine());
            }
            else
            {
                BattleManager.StateMachine.Value.RegisterEnemy(this);
            } 
        }

        IEnumerator FindStateMachine()
        {
            while(BattleManager.StateMachine.Value == null)
            {
                BattleManager.DoSceneCheck(gameObject);
                yield return new WaitForSeconds(1f);
            }

            BattleManager.StateMachine.Value.RegisterEnemy(this);
        }
    }
}
