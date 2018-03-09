using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using nv;

namespace nv
{
    public static class ListExtensions
    {
        public static bool HasElementThatContains( this List<string> list, string str )
        {
            if( list == null || list.Count <= 0 )
                return false;

            foreach( string s in list )
            {
                if( s.Contains( str ) )
                    return true;
            }
            return false;
        }

        public static int IndexOfElementThatContains( this List<string> list, string str )
        {
            if( list == null || list.Count <= 0 )
                return -1;

            for( int i = 0; i < list.Count; ++i )
            {
                if( list[i].Contains( str ) )
                    return i;
            }
            return -1;
        }

        public static bool ContainsNameOfGameObject( this List<string> list, GameObject go )
        {
            if( list == null || list.Count <= 0 )
                return false;

            return list.Contains( go.name );
        }

        public static T GetRandomElementFromList<T>( List<T> list )
        {
            int index = GameRNG.Rand(0, list.Count);
            return list[ index ];
        }
    }
}
