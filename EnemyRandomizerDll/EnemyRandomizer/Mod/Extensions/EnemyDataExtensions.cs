using System.Collections;
using System.Collections.Generic;
using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using nv;
using System.Linq;
using UniRx;
using System;

namespace EnemyRandomizerMod
{
    public static class EnemyDataDefinitions
    {
        public static bool IsCrawlerEnemy(this EnemyData enemy)
        {
            return enemy.enemyType.Contains("CRAWLER");
        }

        public static bool IsGroundEnemy(this EnemyData enemy)
        {
            return enemy.enemyType.Contains("GROUND");
        }

        public static bool IsFlyingEnemy(this EnemyData enemy)
        {
            return enemy.enemyType.Contains("FLYER");
        }

        public static bool IsWallEnemy(this EnemyData enemy)
        {
            return enemy.enemyType.Contains("WALL");
        }

        //TODO: move this into the enemy type
        public static bool IsValidTypeReplacement(this EnemyData enemyA, EnemyData enemyB)
        {
            if (enemyA.isBoss && !enemyB.isBoss)
            {
                return false;
            }
            if (!enemyA.isBoss && enemyB.isBoss)
            {
                return false;
            }

            if (enemyA.isHard && !enemyB.isHard)
            {
                return false;
            }
            if (!enemyA.isHard && enemyB.isHard)
            {
                return false;
            }

            //TODO: change these to bools
            if (enemyA.enemyType.Contains("CRAWLER") && enemyB.enemyType.Contains("CRAWLER"))
            {
                return true;
            }
            if (enemyA.enemyType.Contains("GROUND") && enemyB.enemyType.Contains("GROUND"))
            {
                return true;
            }
            if (enemyA.enemyType.Contains("FLYER") && enemyB.enemyType.Contains("FLYER"))
            {
                return true;
            }
            if (enemyA.enemyType.Contains("WALL") && enemyB.enemyType.Contains("WALL"))
            {
                return true;
            }
            return false;
        }

        public static bool IsValidSizeReplacement(this EnemyData enemyA, EnemyData enemyB)
        {
            //var validTypes = enemyA.validSizeTypes.Split('+').ToList();
            //return (validTypes.Contains(enemyB.enemySize));
            return enemyA.isLargeEnemy == enemyB.isLargeEnemy;
        }
    }
}
