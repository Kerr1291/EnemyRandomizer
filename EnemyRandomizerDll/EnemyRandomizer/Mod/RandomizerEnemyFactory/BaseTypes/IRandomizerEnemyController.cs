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

namespace EnemyRandomizerMod
{
    public interface IRandomizerEnemyController
    {
        /// <summary>
        /// Mostly for display or debug purposes, the real name of the object contained in Instance
        /// </summary>
        string InstanceDefinitionName { get; }
        /// <summary>
        /// A reference to the source data loaded to be loaded after LinkDataObjects has been called
        /// </summary>
        IRandomizerEnemy EnemyDefinition { get; }

        /// <summary>
        /// A reference to the object after Instantiate has been called
        /// </summary>
        GameObject Instance { get; }

        /// <summary>
        /// Is this enemy dead?
        /// </summary>
        bool IsDead { get; }

        /// <summary>
        /// Link the Data and Prefab objects to the types returned from the mod preloader
        /// </summary>
        void LinkDataObjects(GameObject newEnemy, IRandomizerEnemy enemyTypeData);

        /// <summary>
        /// Configure the Instance of this RandomizerEnemy
        /// </summary>
        void SetupInstance(GameObject enemyToReplace = null, EnemyData dataOfEnemyToReplace = null);

        /// <summary>
        /// Configure the parent of the new enemy
        /// </summary>
        void SetNewEnemyParent(Transform newParentObject = null);

        /// <summary>
        /// Configure the size of the new enemy
        /// </summary>
        void ScaleNewEnemy(GameObject enemyToReplace = null, EnemyData dataOfEnemyToReplace = null);

        /// <summary>
        /// Configure the orientation of the new enemy
        /// </summary>
        void RotateNewEnemy(GameObject enemyToReplace = null, EnemyData dataOfEnemyToReplace = null);

        /// <summary>
        /// Configure the position of the new enemy
        /// </summary>
        void PositionNewEnemy(GameObject enemyToReplace = null, EnemyData dataOfEnemyToReplace = null);

        /// <summary>
        /// Configure the geo of the new enemy
        /// </summary>
        void ModifyNewEnemyGeo(GameObject enemyToReplace = null, EnemyData matchidataOfEnemyToReplacengData = null);

        /// <summary>
        /// Do any remaining coniguration logic specific to this enemy type (add custom controller components etc)
        /// </summary>
        void FinalizeNewEnemy(GameObject enemyToReplace = null, EnemyData dataOfEnemyToReplace = null);

        /// <summary>
        /// Finally, name our new enemy
        /// </summary>
        void NameNewEnemy(GameObject enemyToReplace = null, EnemyData dataOfEnemyToReplace = null);

        ///// <summary>
        ///// Enable the Instance
        ///// </summary>
        //void Enable(GameObject enemyToReplace = null, EnemyData dataOfEnemyToReplace = null);
    }

    public static class IRandomizerEnemyControllerExtensions
    {
        public static HealthManager GetHealthManager(this IRandomizerEnemyController gameObject)
        {
            return gameObject.Instance.GetComponent<HealthManager>();
        }

        public static int? GetEnemyHP(this IRandomizerEnemyController gameObject)
        {
            HealthManager hm = gameObject.GetHealthManager();
            if (hm != null)
            {
                return hm.hp;
            }
            return null;
        }

        public static void SetEnemyHP(this IRandomizerEnemyController gameObject, int newHP)
        {
            HealthManager hm = gameObject.GetHealthManager();
            if (hm != null)
            {
                hm.hp = newHP;
                if (hm.hp <= 0)
                    hm.hp = 1;
            }
        }

        public static void ScaleEnemyHP(this IRandomizerEnemyController gameObject, float scale)
        {
            HealthManager hm = gameObject.GetHealthManager();
            if (hm != null)
            {
                hm.hp = UnityEngine.Mathf.FloorToInt(hm.hp * scale);
                if (hm.hp <= 0)
                    hm.hp = 1;
            }
        }

        public static GameObject GetBattleScene(this IRandomizerEnemyController enemyObject)
        {
            var hm = enemyObject.GetHealthManager();
            if (hm == null)
                return null;

            return hm.GetFieldValue<GameObject>("battleScene");
        }

        public static void SetBattleScene(this IRandomizerEnemyController enemyObject, GameObject newBattleScene)
        {
            var hm = enemyObject.GetHealthManager();
            if (hm != null)
                hm.SetBattleScene(newBattleScene);
        }

        public static (int smGeo, int mdGeo, int lgGeo) GetEnemyGeoRates(this IRandomizerEnemyController gameObject)
        {
            HealthManager hm = gameObject.GetHealthManager();
            if (hm != null)
                return (hm.GetGeoSmall(), hm.GetGeoMedium(), hm.GetGeoLarge());
            return (0, 0, 0);
        }

        public static void SetEnemyGeoRates(this IRandomizerEnemyController gameObject, int smallValue, int medValue, int largeValue)
        {
            gameObject.SetEnemyGeoRates((smallValue, medValue, largeValue));
        }

        public static void SetEnemyGeoRates(this IRandomizerEnemyController gameObject, (int smGeo, int mdGeo, int lgGeo) geoRates)
        {
            HealthManager hm = gameObject.GetHealthManager();
            if (hm != null)
            {
                hm.SetGeoSmall(geoRates.smGeo);
                hm.SetGeoMedium(geoRates.mdGeo);
                hm.SetGeoLarge(geoRates.lgGeo);
            }
        }

        public static void CopyEnemyGeoRates(this IRandomizerEnemyController gameObject, GameObject copyFrom, float scale = 1f)
        {
            HealthManager otherHM = copyFrom.GetComponent<HealthManager>();
            if (otherHM != null)
            {
                gameObject.SetEnemyGeoRates(
                    UnityEngine.Mathf.FloorToInt(otherHM.GetGeoSmall() * scale ),
                    UnityEngine.Mathf.FloorToInt(otherHM.GetGeoMedium()* scale ),
                    UnityEngine.Mathf.FloorToInt(otherHM.GetGeoLarge() * scale )
                    );
            }
        }

        public static void ScaleEnemyGeoRates(this IRandomizerEnemyController gameObject, float multiplier)
        {
            var result = gameObject.GetEnemyGeoRates();
            result = (UnityEngine.Mathf.FloorToInt(result.smGeo * multiplier),
                      UnityEngine.Mathf.FloorToInt(result.mdGeo * multiplier),
                      UnityEngine.Mathf.FloorToInt(result.lgGeo * multiplier));
            gameObject.SetEnemyGeoRates(result);
        }
    }
}
