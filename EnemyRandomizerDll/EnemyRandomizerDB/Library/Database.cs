using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System;

namespace EnemyRandomizerMod
{
    public class PrefabObject
    { 
        public enum PrefabType
        { 
            None, Enemy, Hazard, Effect, Other
        }

        [XmlIgnore]
        public string prefabName; 

        [XmlIgnore]
        public GameObject prefab;

        [XmlIgnore]
        public SceneObject source;

        [XmlIgnore]
        public PrefabType prefabType;
    }

    [XmlRoot]
    public partial class EnemyRandomizerDatabase
    {
        [XmlAttribute(AttributeName = "version")]
        public string version; 

        [XmlArray]
        public List<SceneData> scenes;

        [XmlArray]
        public List<string> enemyNames;
        [XmlArray]
        public List<string> hazardNames;
        [XmlArray]
        public List<string> effectNames;

        [XmlArray]
        public List<SceneData> badSceneData;

        [XmlIgnore]
        public List<PrefabObject> enemyPrefabs = new List<PrefabObject>();
        [XmlIgnore]
        public List<PrefabObject> hazardPrefabs = new List<PrefabObject>();
        [XmlIgnore]
        public List<PrefabObject> effectPrefabs = new List<PrefabObject>();


        [XmlIgnore]
        public Dictionary<string, SceneData> Scenes = new Dictionary<string, SceneData>();

        [XmlIgnore]
        public Dictionary<string, PrefabObject> Enemies = new Dictionary<string, PrefabObject>();
        [XmlIgnore]
        public Dictionary<string, PrefabObject> Hazards = new Dictionary<string, PrefabObject>();
        [XmlIgnore]
        public Dictionary<string, PrefabObject> Effects = new Dictionary<string, PrefabObject>();

        [XmlIgnore]
        public Dictionary<string, PrefabObject> Objects = new Dictionary<string, PrefabObject>();
    }

    [XmlRoot]
    public class SceneData
    {
        [XmlAttribute(AttributeName = "name")]
        public string name;

        [XmlArray]
        public List<SceneObject> sceneObjects;
    }

    [XmlRoot]
    public class SceneObject
    {
        [XmlAttribute(AttributeName = "path")]
        public string path;

        [XmlElement(IsNullable = true)]
        public string customTypeName;

        [XmlArray]
        public List<string> components;

        [XmlIgnore]
        public string Name
        {
            get
            {
                return path.Split('/').Last();
            }
        }

        [XmlIgnore]
        public SceneData Scene
        {
            get; set;
        }

        //true if the scene object has been loaded successfully
        [XmlIgnore]
        public bool Loaded { get; set; }

        [XmlIgnore]
        public PrefabObject LoadedObject { get; set; }
    }
}
