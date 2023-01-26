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
    public interface IRandomizerEnemy
    {
        /// <summary>
        /// A reference to the source data loaded from xml after LinkDataObjects has been called
        /// </summary>
        EnemyData Data { get; }

        /// <summary>
        /// A reference to the prefab after LinkDataObjects has been called
        /// </summary>
        GameObject Prefab { get; }

        /// <summary>
        /// Is this a boss type?
        /// </summary>
        bool IsBoss { get; }

        /// <summary>
        /// Is this a flyer type?
        /// </summary>
        bool IsFlyer { get; }

        /// <summary>
        /// Get the difficulty score for this enemy
        /// </summary>
        int Difficulty { get; }

        /// <summary>
        /// Link the Data and Prefab objects to the types returned from the mod preloader
        /// </summary>
        void LinkDataObjects(EnemyData enemyTypeData, List<EnemyData> knownEnemyTypes, GameObject prefabObject);

        /// <summary>
        /// Logic to run on the source prefab of the enemy after its given during initialization
        /// </summary>
        void SetupPrefab();

        /// <summary>
        /// A hook to place code for debugging the prefab
        /// </summary>
        void DebugPrefab();

        /// <summary>
        /// Is the given enemy a size that can replace this one?
        /// </summary>
        bool IsValidDifficultyReplacement(EnemyData dataOfEnemyToReplace);

        /// <summary>
        /// Is the given enemy a type that can replace this one?
        /// </summary>
        bool IsValidTypeReplacement(EnemyData dataOfEnemyToReplace);

        /// <summary>
        /// Create an instance using the prefab object 
        /// </summary>
        GameObject Instantiate();

        /// <summary>
        /// Figure out how hard this enemy is
        /// </summary>
        int CalculateDifficulty();
    }

    //public static class IRandomizerEnemyExtensions
    //{
    //    //change crawler/wall to be IS STATIONARY bool
    //    public static bool IsCrawlerEnemy(this IRandomizerEnemy enemy)
    //    {
    //        return enemy.Data.enemyType.Contains("CRAWLER");
    //    }

    //    public static bool IsWallEnemy(this IRandomizerEnemy enemy)
    //    {
    //        return enemy.Data.enemyType.Contains("WALL");
    //    }

    //    //wrap into an IS GROUND bool
    //    public static bool IsGroundEnemy(this IRandomizerEnemy enemy)
    //    {
    //        return enemy.Data.enemyType.Contains("GROUND");
    //    }

    //    public static bool IsFlyingEnemy(this IRandomizerEnemy enemy)
    //    {
    //        return enemy.Data.enemyType.Contains("FLYER");
    //    }
    //}
    
}
