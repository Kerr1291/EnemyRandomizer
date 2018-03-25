using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nv
{
    public static class StringExtensions
    {
        public static List<int> AllIndexesOf(this string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        //TODO: move this list into a list
        public static bool IsSkipLoadingString( this string str )
        {
            if( string.IsNullOrEmpty( str ) )
                return false;

            if( str.Contains( "Corpse" ) )
                return true;

            if( str.Contains( "Rubble" ) )
                return true;

            if( str.Contains( "Chunk" ) )
                return true;

            if( str.Contains( "Rock" ) )
                return true;

            if( str.Contains( "Particle" ) )
                return true;

            if( str.Contains( "Prompt" ) )
                return true;

            if( str.Contains( "death" ) )
                return true;

            if( str.Contains( "Layer" ) )
                return true;

            if( str.Contains( "Terrain" ) )
                return true;

            if( str.Contains( "Rando" ) )
                return true;

            if( str.Contains( "Summon" ) )
                return true;

            return false;
        }

        public static bool IsSkipAlwaysString( this string str )
        {
            if( string.IsNullOrEmpty( str ) )
                return true;


            if( str.Contains( "Message" ) )
                return true;

            if( str.Contains( "Region" ) )
                return true;

            if( str.Contains( "BlurPlane" ) )
                return true;

            if( str.Contains( "Trigger" ) )
                return true;

            if( str.Contains( "_SceneManager" ) )
                return true;

            if( str.Contains( "GO UP" ) )
                return true;

            if( str.Contains( "Area Title Controller" ) )
                return true;

            if( str.Contains( "mask_container" ) )
                return true;

            if( str.Contains( "_Scenery" ) )
                return true;

            if( str.Contains( "Prompt" ) )
                return true;
            
            return false;
        }


        public static bool IsSkipRootString( this string str )
        {
            if( string.IsNullOrEmpty( str ) )
                return true;

            if( str.Contains( "Message" ) )
                return true;

            if( str.Contains( "Region" ) )
                return true;

            if( str.Contains( "BlurPlane" ) )
                return true;

            if( str.Contains( "Trigger" ) )
                return true;

            if( str.Contains( "_SceneManager" ) )
                return true;

            if( str.Contains( "GO UP" ) )
                return true;

            if( str.Contains( "Area Title Controller" ) )
                return true;

            if( str.Contains( "Rubble" ) )
                return true;

            if( str.Contains( "Tutorial" ) )
                return true;

            if( str.Contains( "mask_container" ) )
                return true;

            if( str.Contains( "tutorial_credits" ) )
                return true;

            if( str.Contains( "TileMap Render Data" ) )
                return true;

            if( str.Contains( "_Markers" ) )
                return true;

            if( str.Contains( "_Transition Gates" ) )
                return true;

            if( str.Contains( "_Scenery" ) )
                return true;

            if( str.Contains( "_Props" ) )
                return true;

            if( str.Contains( "Prompt" ) )
                return true;

            if( str.Contains( "Geo" ) )
                return true;

            if( str.Contains( "Cocoon" ) )
                return true;

            return false;
        }



        public static bool IsSkipRandomizingString( this string str )
        {
            if( string.IsNullOrEmpty( str ) )
                return true;

            if( str.Contains( "Corpse" ) )
                return true;

            if( str.Contains( "Rando" ) )
                return true;
            
            //don't randomize blockers - baulder
            if( str.Contains( "Blocker" ) )
                return true;

            //don't randomize shells - slightly different baulder
            if( str.Contains( "Shell" ) )
                return true; 

            if( str.Contains( "Pigeon" ) )
                return true;

            //don't randomize mender bug or else players will fall onto an enemy every time
            //they enter crossroads
            if( str.Contains( "Mender" ) )
                return true;

            if( str.Contains( "Giant Fly" ) )
                return true;

            return false;
        }

        public static string TrimGameObjectName( this string str )
        {
            if( string.IsNullOrEmpty( str ) )
                return string.Empty;

            string trimmedString = str;

            //trim off "(Clone)" from the word, if it's there
            int index = trimmedString.LastIndexOf("(Clone)");
            if( index > 0 )
                trimmedString = trimmedString.Substring( 0, index );

            index = trimmedString.LastIndexOf( " Fixed" );
            if( index > 0 )
                trimmedString = trimmedString.Substring( 0, index );

            int indexOfStartParethesis = trimmedString.IndexOf(" (");
            if( indexOfStartParethesis > 0 )
                trimmedString = trimmedString.Substring( 0, indexOfStartParethesis );


            ////trim off " (1)" from the word, if it's there
            //index = trimmedString.LastIndexOf( " (1)" );
            //if( index > 0 )
            //    trimmedString = trimmedString.Substring( 0, index );

            ////trim off " (2)" from the word, if it's there
            //index = trimmedString.LastIndexOf( " (2)" );
            //if( index > 0 )
            //    trimmedString = trimmedString.Substring( 0, index );

            ////trim off " (3)" from the word, if it's there
            //index = trimmedString.LastIndexOf( " (3)" );
            //if( index > 0 )
            //    trimmedString = trimmedString.Substring( 0, index );

            ////trim off " (4)" from the word, if it's there
            //index = trimmedString.LastIndexOf( " (4)" );
            //if( index > 0 )
            //    trimmedString = trimmedString.Substring( 0, index );

            ////trim off " (4)" from the word, if it's there
            //index = trimmedString.LastIndexOf( " (5)" );
            //if( index > 0 )
            //    trimmedString = trimmedString.Substring( 0, index );

            ////trim off " (4)" from the word, if it's there
            //index = trimmedString.LastIndexOf( " (6)" );
            //if( index > 0 )
            //    trimmedString = trimmedString.Substring( 0, index );

            ////trim off " (4)" from the word, if it's there
            //index = trimmedString.LastIndexOf( " (7)" );
            //if( index > 0 )
            //    trimmedString = trimmedString.Substring( 0, index );

            ////trim off " (4)" from the word, if it's there
            //index = trimmedString.LastIndexOf( " (8)" );
            //if( index > 0 )
            //    trimmedString = trimmedString.Substring( 0, index );

            ////trim off " (4)" from the word, if it's there
            //index = trimmedString.LastIndexOf( " (9)" );
            //if( index > 0 )
            //    trimmedString = trimmedString.Substring( 0, index );

            ////trim off " (4)" from the word, if it's there
            //index = trimmedString.LastIndexOf( " (10)" );
            //if( index > 0 )
            //    trimmedString = trimmedString.Substring( 0, index );

            ////trim off " (10)" from the word, if it's there
            //index = trimmedString.LastIndexOf( " (11)" );
            //if( index > 0 )
            //    trimmedString = trimmedString.Substring( 0, index );

            ////trim off " (10)" from the word, if it's there
            //index = trimmedString.LastIndexOf( " (12)" );
            //if( index > 0 )
            //    trimmedString = trimmedString.Substring( 0, index );

            //trim off " (10)" from the word, if it's there
            //index = trimmedString.LastIndexOf( " (13)" );
            //if( index > 0 )
            //    trimmedString = trimmedString.Substring( 0, index );

            if( trimmedString != "Zombie Spider 1" && trimmedString != "Zombie Spider 2" )
            {
                //trim off " 1" from the word, if it's there
                index = trimmedString.LastIndexOf( " 1" );
                if( index > 0 )
                    trimmedString = trimmedString.Substring( 0, index );

                index = trimmedString.LastIndexOf( " 2" );
                if( index > 0 )
                    trimmedString = trimmedString.Substring( 0, index );

                index = trimmedString.LastIndexOf( " 3" );
                if( index > 0 )
                    trimmedString = trimmedString.Substring( 0, index );

                index = trimmedString.LastIndexOf( " 4" );
                if( index > 0 )
                    trimmedString = trimmedString.Substring( 0, index );

                index = trimmedString.LastIndexOf( " 5" );
                if( index > 0 )
                    trimmedString = trimmedString.Substring( 0, index );

                index = trimmedString.LastIndexOf( " 6" );
                if( index > 0 )
                    trimmedString = trimmedString.Substring( 0, index );

                index = trimmedString.LastIndexOf( " 7" );
                if( index > 0 )
                    trimmedString = trimmedString.Substring( 0, index );

                index = trimmedString.LastIndexOf( " 8" );
                if( index > 0 )
                    trimmedString = trimmedString.Substring( 0, index );

                index = trimmedString.LastIndexOf( " 9" );
                if( index > 0 )
                    trimmedString = trimmedString.Substring( 0, index );
            }

            if( trimmedString.Contains( "Zombie Fungus" ) )
            {
                //trim off " B" from the word, if it's there
                index = trimmedString.LastIndexOf( " B" );
                if( index > 0 )
                    trimmedString = trimmedString.Substring( 0, index );
            }

            //trim off " New" from the word, if it's there
            if( trimmedString.Contains( "Electric Mage" ) )
            {
                index = trimmedString.LastIndexOf( " New" );
                if( index > 0 )
                    trimmedString = trimmedString.Substring( 0, index );
            }

            if( trimmedString.Contains( "Baby Centipede" ) )
            {
                index = trimmedString.LastIndexOf( " Summon" );
                if( index > 0 )
                    trimmedString = trimmedString.Substring( 0, index );
                index = trimmedString.LastIndexOf( " Summoner" );
                if( index > 0 )
                    trimmedString = trimmedString.Substring( 0, index );
                index = trimmedString.LastIndexOf( " Spawner" );
                if( index > 0 )
                    trimmedString = trimmedString.Substring( 0, index );
            }

            if( trimmedString.Contains( "Fluke Fly" ) )
            {
                index = trimmedString.LastIndexOf( " Summon" );
                if( index > 0 )
                    trimmedString = trimmedString.Substring( 0, index );
                index = trimmedString.LastIndexOf( " Summoner" );
                if( index > 0 )
                    trimmedString = trimmedString.Substring( 0, index );
                index = trimmedString.LastIndexOf( " Spawner" );
                if( index > 0 )
                    trimmedString = trimmedString.Substring( 0, index );
            }

            if( trimmedString.Contains( "Balloon" ) )
            {
                index = trimmedString.LastIndexOf( " Summon" );
                if( index > 0 )
                    trimmedString = trimmedString.Substring( 0, index );
                index = trimmedString.LastIndexOf( " Summoner" );
                if( index > 0 )
                    trimmedString = trimmedString.Substring( 0, index );
                index = trimmedString.LastIndexOf( " Spawner" );
                if( index > 0 )
                    trimmedString = trimmedString.Substring( 0, index );
            }

            return trimmedString;
        }
    }
}
