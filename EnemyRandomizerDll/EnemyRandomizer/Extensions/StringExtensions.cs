using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ModCommon;

namespace EnemyRandomizerMod
{
    public static class StringExtensions
    {
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
            "Ruins Flying SentryB",
            "Ruins SentryB",
            "Ruins Sentry Fat B",
            "Shade Sibling(Clone)",
            "Shade Sibling(Clone)(Clone)",
            "Baby Centipede(Clone)",
            "Baby Centipede(Clone)(Clone)",
            "Zoteling",

            //TEMP: remove these enemies until they're fixed
            "Mage Lord Phase2",
            "Mega Moss Charger",
            "Dung Defender",
            "White Defender",
            "Mega Jellyfish",
            "Mega Zombie Beam Miner", //still broken
            "NONE"
        };


        public static List<string> makeCopyOnLoadingList = new List<string>()
        {
            "Baby Centipede",
            "Shade Sibling",
            "Electric Mage",
            "Hatcher Baby",
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

            if( str == "Giant Fly"  )
                return true;

            if( str.Contains( "Hornet Boss 1" ) )
                return true;

            return false;
        }
    }
}
