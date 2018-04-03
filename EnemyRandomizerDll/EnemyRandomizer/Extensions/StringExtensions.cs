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

        public static List<string> skipLoadingList = new List<string>()
        {
            "Hollow Shade",
            "Head",
            "Spawn Roller v2",
            "Hatcher Baby Spawner",
            "Hatcher NP",
            "Egg Sac",
            "Lil Jellyfish",
            "Gate Mantis",
            "Mantis Lord S2",
            "Mantis Lord",
            "Mantis Lord S1",
            "Dummy Mantis",
            "Real Bat",
            "Tinger",
            "Zote Balloon",
            "fluke_baby_01",
            "fluke_baby_02",
            "fluke_baby_03",
            "Hiveling Spawner",
            "Jellyfish Baby Inert",
            "Cap Hit",
            "Moss Knight C",
            "Moss Knight B",
            "Giant Buzzer",
            "Plant Turret Right",
            "Shell",
            "Buzzer R",
            "Spitter R",
            "Roller R",
            "Ruins Sentry Fat",
            "Great Shield Zombie bottom",
            "False Knight New",
            "Dream Mage Lord Phase2",
            "False Knight Dream",
            "Mage Lord Phase2",

            //TEMP: remove these enemies until they're fixed
            "Mage Lord Phase2",
            "Mega Moss Charger",
            "Dung Defender",
            "White Defender",
            "Mega Jellyfish",
            "NONE"
        };


        public static List<string> makeCopyOnLoadingList = new List<string>()
        {
            "Baby Centipede",
            "Shade Sibling",
            "Electric Mage",
            "Moss Charger"
        };

        
        public static bool IsCopyOnLoadingString( this string str )
        {
            if( string.IsNullOrEmpty( str ) )
                return false;

            string trimmedName = str.TrimGameObjectName();

            if( makeCopyOnLoadingList.Contains( trimmedName ) )
                return true;

            return false;
        }


        //TODO: move this list into a list
        public static bool IsSkipLoadingString( this string str )
        {
            if( string.IsNullOrEmpty( str ) )
                return true;

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

            if( str.Contains( "fluke_baby_03" ) )
                return true;
            if( str.Contains( "Flukeman Top" ) )
                return true;
            if( str.Contains( "Flukeman Bot" ) )
                return true;

            string trimmedName = str.TrimGameObjectName();

            if( skipLoadingList.Contains( trimmedName ) )
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

            //TODO: put a check in here to avoid skipping on just the city of tears scenes
            //if( str.Contains( "_Scenery" ) )
            //    return true;

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

            

            if( trimmedString != "Zombie Spider 1" && trimmedString != "Zombie Spider 2" && trimmedString != "Hornet Boss 1" && trimmedString != "Hornet Boss 2" )
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
