using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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

        public static T GetRandomElementFromList<T>( this List<T> list )
        {
            int index = GameRNG.Rand(0, list.Count);
            return list[ index ];
        }

        public static T GetRandomElementFromList<T>( this List<T> list, RNG rng )
        {
            int index = rng.Rand(0, list.Count);
            return list[ index ];
        }

        public static GameObject CreateLineRenderer( this List<Vector2> points, Color c, float z = 0f, float width = .5f )
        {
            if( points == null || points.Count < 0 )
                return null;

            return CreateLineRenderer( points.Select( x => { return new Vector3( x.x, x.y, z ); } ).ToList(), c, width );
        }

        public static GameObject CreateLineRenderer( this List<Vector3> points, Color c, float width = .5f, Transform parent = null )
        {
            if( points == null || points.Count < 0 )
                return null;

            GameObject lineObj = new GameObject("LineRenderer created by ListExtensions.CreateLineRenderer");
            if( parent != null )
                lineObj.transform.SetParent( parent );

            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.SetVertexCount( points.Count );
            lr.SetPositions( points.ToArray() );
            lr.SetWidth( width, .001f );

            if( lr.GetComponent<Renderer>() )
                lr.GetComponent<Renderer>().material = new Material( Shader.Find( "Diffuse" ) );
            if( lr.GetComponent<Renderer>() )
                lr.GetComponent<Renderer>().material.color = c;

            lr.SetColors( c, c );

            return lineObj;
        }
    }
}
