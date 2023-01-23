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

namespace EnemyRandomizerMod
{
    public interface ISceneDataProvider
    {
        List<(string SceneName, string GameObjectPath)> GetSceneDataList();
    }

    [XmlRoot("CustomSettings")]
    public class EnemyRandomizerCustomSettings
    {
    }

    [XmlRoot(ElementName = "enemyList")]
    public class GameObjectData : ISceneDataProvider
    {
        [XmlElement(ElementName = "enemy")]
        public List<EnemyData> enemyData;

        List<(string SceneName, string GameObjectPath)> ISceneDataProvider.GetSceneDataList()
        {
            Dev.Log("Count: " + enemyData.Count);
            return enemyData.Select(x => (SceneName: x.sceneName, GameObjectPath: x.gameObjectPath)).ToList();
        }
    }

    [XmlRoot(ElementName = "enemy")]
    public class EnemyData
    {
        [XmlElement(ElementName = "configurationTypeName", IsNullable = true)]
        public string configurationTypeName;

        //Core enemy data
        [XmlAttribute(AttributeName = "name")]
        public string name;

        [XmlElement(ElementName = "sceneName")]
        public string sceneName;

        [XmlElement(ElementName = "gameObjectPath")]
        public string gameObjectPath;

        [XmlElement(ElementName = "behaviourName", IsNullable = true)]
        public string behaviourName;

        //Randomizer specific metadata
        [XmlElement(ElementName = "typePool")]
        public string enemyType;

        [XmlElement(ElementName = "isLargeEnemy")]
        public bool isLargeEnemy;

        [XmlElement(ElementName = "origin", IsNullable = true)]
        public string areaOrigin;

        [XmlElement(ElementName = "isHard")]
        public bool isHard;

        [XmlElement(ElementName = "isBoss")]
        public bool isBoss;

        [XmlElement(ElementName = "isColosseum", IsNullable = false)]
        public bool isColosseum;

        [XmlElement(ElementName = "isGodmaster", IsNullable = false)]
        public bool isGodmaster;

        [XmlElement(ElementName = "isGhostWarrior", IsNullable = false)]
        public bool isGhostWarrior;

        [XmlElement(ElementName = "hasSpecialEnding", IsNullable = false)]
        public bool hasSpecialEnding;

        [XmlElement(ElementName = "excludeFromBattleArenas", IsNullable = false)]
        public bool excludeFromBattleArenas;

        [XmlElement(ElementName = "excludeFromColosseum", IsNullable = false)]
        public bool excludeFromColosseum;

        [XmlElement(ElementName = "excludeBeingReplacement", IsNullable = false)]
        public bool skipReplacement;

        [XmlElement(ElementName = "baseHP", IsNullable = false)]
        public int baseHP;

        [XmlElement(ElementName = "coloHP", IsNullable = false)]
        public int coloHP;

        [XmlElement(ElementName = "attunedHP", IsNullable = false)]
        public int attunedHP;

        [XmlElement(ElementName = "ascendedHP", IsNullable = false)]
        public int ascendedHP;

        [XmlElement(ElementName = "nail1HP", IsNullable = false)]
        public int nail1HP;

        [XmlElement(ElementName = "nail2HP", IsNullable = false)]
        public int nail2HP;

        [XmlElement(ElementName = "nail3HP", IsNullable = false)]
        public int nail3HP;

        [XmlElement(ElementName = "nail4HP", IsNullable = false)]
        public int nail4HP;

        [XmlElement(ElementName = "isCustom")]
        public bool isCustomEnemy;

        [XmlIgnore]
        public IRandomizerEnemy loadedEnemy;
    }


    [XmlRoot(ElementName = "exclusionList")]
    public class ExclusionData
    {
        [XmlElement(ElementName = "exclusion")]
        public List<ExclusionItem> enemyData;
    }

    [XmlRoot(ElementName = "exclusion")]
    public class ExclusionItem
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "room")]
        public int RoomID { get; set; }

        [XmlElement(ElementName = "roomName", IsNullable = true)]
        public string RoomName { get; set; }

        [XmlElement(ElementName = "enemy")]
        public string EnemyName { get; set; }

        [XmlElement(ElementName = "isUniqueEnemy")]
        public bool IsUniqueEnemy { get; set; }

        [XmlElement(ElementName = "doNotReplace")]
        public bool DoNotReplace { get; set; }

        [XmlElement(ElementName = "doNotUse")]
        public bool DoNotUse { get; set; }
    }



    [XmlRoot(ElementName = "randomizer")]
    public class ObjectData
    {
        [XmlArray(ElementName = "object")]
        public List<SceneObject> enemyData;
    }

    [XmlRoot(ElementName = "object")]
    public class SceneObject
    {
        [XmlAttribute(AttributeName = "name")]
        public string name;

        [XmlElement(ElementName = "isEffect")]
        public bool isEffect;

        [XmlElement(ElementName = "isOther")]
        public bool isOther;

        [XmlElement(ElementName = "isMisc")]
        public bool isMisc;
    }







    [XmlRoot("arenaList")]
    public class ArenaList
    {
        [XmlElement(ElementName = "arena")]
        public List<ArenaEntry> Arenas { get; set; }
    }

    [XmlRoot(ElementName = "cleanups")]
    public class Cleanups
    {
        [XmlElement(ElementName = "cleanup")]
        public List<string> Cleanup { get; set; }
    }

    [XmlRoot("arena")]
    public class ArenaEntry
    {
        [XmlElement("sceneName")]
        public string SceneName { get; set; }

        [XmlElement("battleSceneName")]
        public string BattleSceneName { get; set; }

        [XmlElement("minX")]
        public int MinX { get; set; }

        [XmlElement("maxX")]
        public int MaxX { get; set; }

        [XmlElement("minY")]
        public int MinY { get; set; }

        [XmlElement("maxY")]
        public int MaxY { get; set; }

        [XmlElement(ElementName = "totalEnemyCount", IsNullable = false)]
        public int TotalEnemyCount { get; set; }

        [XmlElement("waves")]
        public WaveList Waves { get; set; }

        [XmlElement(ElementName = "cleanups", IsNullable = true)]
        public Cleanups Cleanups { get; set; }

        [XmlIgnore]
        Rect? bounds;

        [XmlIgnore]
        public Rect Bounds
        {
            get
            {
                return bounds.HasValue ? bounds.Value : (bounds = new Rect(MinX, MinY, MaxX - MinX, MaxY - MinY)).Value;
            }
        }
    }

    [XmlRoot("waves")]
    public class WaveList
    {
        [XmlElement(ElementName = "wave")]
        public List<WaveEntry> Waves { get; set; }
    }

    [XmlRoot("wave")]
    public class WaveEntry
    {
        [XmlElement(ElementName = "waveName", IsNullable = true)]
        public string WaveName { get; set; }

        [XmlElement("waveNumber")]
        public int WaveNumber { get; set; }

        [XmlElement(ElementName = "enemies")]
        public Enemies Enemies { get; set; }
    }

    [XmlRoot(ElementName = "enemies")]
    public class Enemies
    {
        [XmlElement(ElementName = "enemy")]
        public List<string> Enemy { get; set; }
    }




    [XmlRoot("exclusionList")]
    public class ExclusionList
    {
        [XmlArray(ElementName = "exclusion")]
        public List<ExclusionEntry> exclusions { get; set; }
    }

    [XmlRoot("exclusion")]
    public class ExclusionEntry
    {
        [XmlElement("room")]
        public int Room { get; set; }

        [XmlElement("roomName")]
        public string RoomName { get; set; }

        [XmlElement("enemy")]
        public string Enemy { get; set; }

        [XmlElement("isUniqueEnemy")]
        public bool IsUniqueEnemy { get; set; }

        [XmlElement("doNotReplace")]
        public bool DoNotReplace { get; set; }

        [XmlElement("doNotUse")]
        public bool DoNotUse { get; set; }

    }
}
