using System.Collections.Generic;
using UnityEngine;

using ModCommon;

namespace EnemyRandomizerMod
{
    public static class GameObjectExtensions
    { 
        public static bool IsRandomizerEnemy( this GameObject gameObject, List<string> enemyTypes )
        {
            if( gameObject == null )
                return false;

            if( !gameObject.IsGameEnemy() )
                return false;

            if( gameObject.name.Contains( "Corpse" ) )
                return false;

            if( gameObject.name.Contains( "Lil Jellyfish" ) )
                return false;

            //TEST: should randomize spawn rollers
            if( gameObject.name.Contains( "Spawn Roller v2" ) )
                return true;

            string enemyName = gameObject.name;
            string trimmedName = enemyName.TrimGameObjectName();

            bool isRandoEnemyType = enemyTypes.Contains( trimmedName );

            return isRandoEnemyType;
        }
    }
}
