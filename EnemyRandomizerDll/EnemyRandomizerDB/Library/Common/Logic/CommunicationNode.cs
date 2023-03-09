#define UNITY_5_3_OR_NEWER 
#define UNITY_5_6_OR_NEWER
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

#if UNITY_5_3_OR_NEWER
using DebugLogger = EnemyRandomizerMod.Dev;
#else
using DebugLogger = System.Console;
#endif

namespace EnemyRandomizerMod
{
    /// <summary>
    /// Apply this attribute to a method in a class that will be used by a CommunicationNode to cause that method to become a handler on the networks the node is on.
    /// The attribute may take an optional list of tags that may be used to further qualify the invoke behavior of the handlers.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CommunicationCallback : Attribute
    {
        /// Used to add additional requirements to handlers via the CommunicationCallback attribute.
        public Tags tags;

        /// <summary>
        /// Denote this method as a handler for a CommunicationNode.
        /// </summary>
        /// <param name="matching">The how the tags specified should be matched in a call to publish.</param>
        /// <param name="tags">Strings that will be required by publish to invoke this callback.</param>
        public CommunicationCallback(Tags.Matching matching = Tags.Matching.Any, params string[] tags)
        {
            this.tags.matching = matching;
            this.tags = tags;
        }
    }

    /// <summary>
    /// This class is used to send and receive data in publisher/subscriber manner. 
    /// </summary>
    /// <remarks>
    /// The object that is passed into EnableNode() has its methods checked with reflection for any that are decorated with the [CommunicationCallback] attribute.
    /// Those methods are subscribed as handlers for any Publish() calls. 
    /// Calls to publish are resolved by matching data types. ie. A call to Publish("Hello") will invoke any method with the following signature: [CommunicationCallback] void ExampleHandler(string data) {}
    /// </remarks>
    [Serializable]
    public class CommunicationNode
    {
#if UNITY_5_6_OR_NEWER
        protected static bool queuePublishes = true;

        protected class QueuedPublish
        {
            public QueuedPublish(object publishedData, object publisher, Tags tags, List<string> publishToNetworks, string sourceScene)
            {
                this.publishedData = publishedData;
                this.publisher = publisher;
                this.tags = tags;
                this.publishToNetworks = publishToNetworks;
                wasGlobal = publisher == null;
                this.sourceScene = sourceScene;
            }

            public object publishedData;
            public object publisher;
            public Tags tags;
            public List<string> publishToNetworks;
            public bool wasGlobal;
            public string sourceScene;
        }

        protected static Queue<QueuedPublish> onSceneLoadedQueue = new Queue<QueuedPublish>();
        protected static List<string> loadedScenes = new List<string>();

        protected static void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            loadedScenes.Add(scene.name);
            queuePublishes = false;

            var queuedPublishes = onSceneLoadedQueue.ToList();
            foreach(QueuedPublish p in queuedPublishes)
            {
                if(!p.wasGlobal && p.publisher == null)
                {
                    //the object that queued this publish was destroyed (likely in scene unload); don't invoke this publish action
                    continue;
                }
                if(!loadedScenes.Contains(p.sourceScene))
                {
                    //the object was queued from a scene that was unloaded
                    continue;
                }
                publish(p.publishedData, p.publisher, p.tags, p.publishToNetworks);
            }
            onSceneLoadedQueue.Clear();
            CleanNetwork();
        }

        protected static void OnSceneUnloaded(UnityEngine.SceneManagement.Scene current)
        {
            loadedScenes.Remove(current.name);
            //queue events until the active scene has finished changing
            if(current == UnityEngine.SceneManagement.SceneManager.GetActiveScene())
                queuePublishes = true;
            onSceneLoadedQueue.Clear();
            CleanNetwork();
        }
#endif
        /// <summary>
        /// Optional list of networks to join when this node is enabled.
        /// </summary>
        public List<string> networksToJoinOnEnable = new List<string>();

        /// <summary>
        /// Enable this to allow an object to publish events to itself.
        /// </summary>
        public bool allowPublishToSelf = false;

        /// <summary>
        /// Enable this to get debug output when published data is not handled.
        /// </summary>
        public static bool debugNotifyUnhandledMessages = true;

        /// <summary>
        /// If debugNotifyUnhandledMessages is true this will be used to check if publish is invoked by any handler.
        /// </summary>
        protected static bool debugPublishWasHandled = false;

        /// <summary>
        /// Enable this to get debug output when nodes are enabled, disabled, publish is called, or handlers are invoked. (Yes it will be very verbose)
        /// </summary>
        public static bool debugPrintAllActivity = false;

        /// <summary>
        /// All node networks. The networks are just looked up by mapping the name to the root node. The nodes are assembled as intrusive linked lists.
        /// </summary>
        protected static Dictionary<string, CommunicationNode> root;

        /// <summary>
        /// The next links of this node on each network it is part of.
        /// </summary>
        protected Dictionary<string, CommunicationNode> next;

        /// <summary>
        /// The previous links of this node on each network it is part of.
        /// </summary>
        protected Dictionary<string, CommunicationNode> prev;

        /// <summary>
        /// The actions taken when Publish() is called. Use the property to add/remove actions.
        /// </summary>
        protected static Action<object, object, Tags, List<string>> publishAction;
        
        /// <summary>
        /// The actions that happen when Publish() is called.
        /// </summary>
        public static event Action<object, object, Tags, List<string>> PublishAction
        {
            add
            {
                lock (publishAction)
                {
                    publishAction += value;
                }
            }
            remove
            {
                lock (publishAction)
                {
                    publishAction -= value;
                }
            }
        }

        /// <summary>
        /// The object bound to this node that receives any incomming published data.
        /// </summary>
        public virtual object NodeOwner { get; private set; }

        /// <summary>
        /// The names of all networks.
        /// </summary>
        // Dev Note: This networks is lowercase because Networks is already used in this class to denote the local networks a node has.
        public static List<string> networks
        {
            get
            {
                return root.Keys.ToList();
            }
        }

        /// <summary>
        /// The networks this node is in.
        /// </summary>
        public virtual List<string> Networks
        {
            get
            {
                return next.Keys.ToList();
            }
        }

        public override string ToString()
        {
            return GetObjectTypeName(NodeOwner) + "::" + GetObjectName(NodeOwner) + "@" + string.Join(", ", Networks.ToArray());
        }

        protected readonly Dictionary<Type,Dictionary<string, MethodInfo>> enabledHandlers = new Dictionary<Type, Dictionary<string, MethodInfo>>();

        /// <summary>
        /// Cached reflection helper used to help get methods on an object.
        /// </summary>
        protected const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        /// <summary>
        /// Invokes any matching handlers contained in nodes on the given networks. By default (no parameters) this will publish to every node on all networks.
        /// </summary>
        /// <param name="dataToPublish">The data that will be passed to the matching handlers.</param>
        /// <param name="publisher">The object that is publishing data onto the network.</param>
        /// <param name="tags">Tags that may be used to invoke specific handlers.</param>
        /// <param name="publishToNetworks">Zero or more optional strings, arrays of string, or lists of strings specifying the networks to publish to. If empty, will publish to all networks.</param>
        // Dev Note: This publish is lowercase because Publish is already used in this class in a way that using Publish for this method would cause compiler ambiguity.
        public static void publish(object dataToPublish, object publisher, Tags tags, params object[] publishToNetworks)
        {
            List<string> totalNetworks = new List<string>();

            foreach(var n in publishToNetworks)
            {
                if(n == null)
                    continue;

                if(n as string[] != null)
                {
                    totalNetworks.AddRange(n as string[]);
                }
                else if(n as List<string> != null)
                {
                    totalNetworks.AddRange(n as List<string>);
                }
                else if(n as string != null)
                {
                    totalNetworks.Add(n as string);
                }
                else
                {
#if UNITY_5_3_OR_NEWER
                    DebugLogger.LogError("Non-string parameter type or container found in network params. All network params must be string, arrays of string, or lists of string.");
#else
                    DebugLogger.WriteLine("Non-string parameter type or container found in network params. All network params must be string, arrays of string, or lists of string.");
#endif
                }
            }

            try
            {
#if UNITY_5_6_OR_NEWER
                if(queuePublishes)
                {
                    string publisherScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                    if(publisher is UnityEngine.Component)
                        publisherScene = (publisher as UnityEngine.Component).gameObject.scene.name;
                    if(publisher is UnityEngine.GameObject)
                        publisherScene = (publisher as UnityEngine.GameObject).scene.name;
                    onSceneLoadedQueue.Enqueue(new QueuedPublish(dataToPublish, publisher, tags, totalNetworks, publisherScene));
                }
                else
#endif
                {
                    publishAction.Invoke(dataToPublish, publisher, tags, totalNetworks);
                }
            }
            catch(Exception e)
            {
                string publisherName = GetObjectName(publisher);
#if UNITY_5_3_OR_NEWER
                DebugLogger.LogError("Unhandled Exception caught when " + (queuePublishes ? "queuing" : "publishing") + " data of type " + GetObjectTypeName(dataToPublish) + " sent by " + publisherName + ": " + e.Message);
                DebugLogger.LogError(e.StackTrace);
#else
                DebugLogger.WriteLine( "Unhandled Exception caught when " + "publishing" +" data of type " + dataToPublish.GetType().Name + " sent by " + publisherName + ": " + e.Message + " at " + e.StackTrace );
#endif
            }
        }

        /// <summary>
        /// Invokes any matching handlers contained in nodes on the given networks. By default (no parameters) this will publish to every node on all networks.
        /// </summary>
        /// <param name="dataToPublish">The data that will be passed to the matching handlers.</param>
        /// <param name="publisher">The object that is publishing data onto the network.</param>
        /// <param name="publishToNetworks">Zero or more optional strings, arrays of string, or lists of strings specifying the networks to publish to. If empty, will publish to all networks.</param>
        // Dev Note: This publish is lowercase because Publish is already used in this class in a way that using Publish for this method would cause compiler ambiguity.
        public static void publish(object dataToPublish, object publisher, params object[] publishToNetworks)
        {
            publish(dataToPublish, publisher, default(Tags), publishToNetworks);
        }

        /// <summary>
        /// Invokes any matching handlers contained in nodes on the given networks. By default (no parameters) this will publish to every node on all networks.
        /// </summary>
        /// <param name="dataToPublish">The data that will be passed to the matching handlers.</param>
        /// <param name="publisher">The object that is publishing data onto the network.</param>
        /// <param name="tags">Tags that may be used to invoke specific handlers.</param>
        /// <param name="publishToNetworks">Zero or more optional strings, arrays of string, or lists of strings specifying the networks to publish to. If empty, will publish to all networks.</param>
        public virtual void Publish(object dataToPublish, Tags tags, params object[] publishToNetworks)
        {
            publish(dataToPublish, NodeOwner, tags, publishToNetworks);
        }

        /// <summary>
        /// Invokes any matching handlers contained in nodes on the given networks. By default (no parameters) this will publish to every node on all networks.
        /// </summary>
        /// <param name="dataToPublish">The data that will be passed to the matching handlers.</param>
        /// <param name="publisher">The object that is publishing data onto the network.</param>
        /// <param name="publishToNetworks">Zero or more optional strings, arrays of string, or lists of strings specifying the networks to publish to. If empty, will publish to all networks.</param>
        public virtual void Publish(object dataToPublish, params object[] publishToNetworks)
        {
            publish(dataToPublish, NodeOwner, default(Tags), publishToNetworks);
        }

        /// <summary>
        /// Enables this node on the network. 
        /// Any methods in the nodeOwner object that have the [CommunicationCallback] attribute will be registered as handlers on the network.
        /// Typically this should be put in Start/Awake/OnEnable or some other similar method.
        /// </summary>
        /// <param name="nodeOwner">An object with zero or more methods decorated with the [CommunicationCallback] attribute.</param>
        /// <param name="networksToJoin">(Optional) Zero or more strings, arrays of string, or lists of strings specifying the networks to publish to. If empty, will publish to all networks.</param>
        public virtual void EnableNode( object nodeOwner, params object[] networksToJoin )
        {
            //error: nodes themselves cannot be owners
            if(nodeOwner.GetType() == this.GetType())
            {
                DisableNode();
                throw new InvalidOperationException("A CommunicationNode may not be the owner of itself. A CommunicationNode needs a reference to a class with zero or more methods that have the [CommunicationCallback] attribute.");
            }

            //There must be an owner, so treat this as a removal.
            if( nodeOwner == null )
            {
                DisableNode();
                if(debugPrintAllActivity)
                    DebugLogger.LogError("Cannot add a node with a null node owner");
                return;
            }

            this.NodeOwner = nodeOwner;

            List<string> totalNetworks = new List<string>();

            //Attempt to cast the params to strings, arrays of strings, or lists of strings and concat them into totalNetworks
            foreach(var n in networksToJoin)
            {
                if(n == null)
                    continue;

                if(n as string[] != null)
                {
                    totalNetworks.AddRange(n as string[]);
                }
                else if(n as List<string> != null)
                {
                    totalNetworks.AddRange(n as List<string>);
                }
                else if(n as string != null)
                {
                    totalNetworks.Add(n as string);
                }
                else
                {
#if UNITY_5_3_OR_NEWER
                    DebugLogger.LogError("Non-string parameter type or container found in networksToJoin params. All network params must be string, arrays of string, or lists of string.");
#else
                    DebugLogger.Write("Non-string parameter type or container found in networksToJoin params. All network params must be string, arrays of string, or lists of string.");
#endif
                }
            }

            totalNetworks.AddRange(networksToJoinOnEnable);

            //By default, use the type name to add the object to its own network
            if(totalNetworks.Count <= 0)
            {
                if(debugPrintAllActivity)
                {
                    DebugLogger.Log("Since no networks were specified, adding this node to a network with the object's type name " + GetObjectTypeName(nodeOwner));
                }
                totalNetworks.Add(GetObjectTypeName(nodeOwner));
            }

            if(debugPrintAllActivity)
            {
                string addingToNetworks = "";
                networksToJoin.Select(x => { addingToNetworks += x + ", "; return x; });
                addingToNetworks = addingToNetworks.TrimEnd(',', ' ');
                DebugLogger.Log("Adding ".Colorize(UnityEngine.Color.green) + GetObjectTypeName(nodeOwner).Colorize(UnityEngine.Color.green) + "::" + GetObjectName(nodeOwner).Colorize(UnityEngine.Color.yellow) + " these networks: " + addingToNetworks);
            }
            CommunicationNode.AddNode(this, totalNetworks);
            RefreshCallbackBindings();
        }

        /// <summary>
        /// Removes the node from all networks. For a unity object this should typically go in OnDisable or OnDestroy, depending on the desired behavior.
        /// </summary>
        public virtual void DisableNode()
        {
            CommunicationNode.RemoveNode( this );
            NodeOwner = null;
        }

        /// <summary>
        /// Setup the default publish actions.
        /// </summary>
        static CommunicationNode()
        {
            root = new Dictionary<string, CommunicationNode>();

            if(publishAction == null)
                publishAction = EmptyPublish;

            PublishAction -= DefaultPublish;
            PublishAction += DefaultPublish;

#if UNITY_5_6_OR_NEWER
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;

            UnityEngine.SceneManagement.SceneManager.sceneUnloaded -= OnSceneUnloaded;
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded += OnSceneUnloaded;
#endif
        }

        /// <summary>
        /// Prevent publish from ever being null, as without this several nullrefs may happen.
        /// </summary>
        static void EmptyPublish(object publishedData, object publisher, Tags tags, List<string> publishToNetworks = null){}

        /// <summary>
        /// The standard way data is passed through the networks; Iterate over the intrusive link-list and invoke all matching handlers.
        /// </summary>
        protected static void DefaultPublish(object publishedData, object publisher, Tags tags, List<string> publishToNetworks = null)
        {
            string sentNetworks = null;
            string notFoundNetworks = null;

            debugPublishWasHandled = false;

            //optimzation to make the default publish as efficient as possible
            if(publishToNetworks == null || publishToNetworks.Count <= 0)
            {
                foreach(var pair in root)
                {
                    if(debugNotifyUnhandledMessages)
                    {
                        if(sentNetworks == null)
                            sentNetworks = string.Empty;

                        sentNetworks += pair.Key + ", ";
                    }
                    PublishToNetwork(publishedData, publisher, tags, pair.Key);
                }
            }
            else
            {
                foreach(string n in publishToNetworks)
                {
                    if(root.ContainsKey(n))
                    {
                        if(debugNotifyUnhandledMessages)
                        {
                            if(sentNetworks == null)
                                sentNetworks = string.Empty;

                            sentNetworks += n + ", ";
                        }
                        PublishToNetwork(publishedData, publisher, tags, n);
                    }
                    else
                    {
                        if(debugNotifyUnhandledMessages)
                        {
                            if(notFoundNetworks == null)
                                notFoundNetworks = string.Empty;

                            notFoundNetworks += n + ", ";
                        }
                    }
                }
            }


            if(!debugPublishWasHandled && debugNotifyUnhandledMessages)
            {
                if(sentNetworks == null)
                    sentNetworks = string.Empty;
                sentNetworks = sentNetworks.TrimEnd(',', ' ');

                if(!string.IsNullOrEmpty(notFoundNetworks))
                {
                    notFoundNetworks = notFoundNetworks.TrimEnd(',', ' ');
                    notFoundNetworks = "These networks were not found: " + notFoundNetworks;
                }

                DebugLogger.Log("Publish: " + GetObjectTypeName(publishedData) + " sent by " + GetObjectTypeName(publisher) + "::" + GetObjectName(publisher)
                    + " was not handled by any handlers in these networks: " + sentNetworks + "; " + (notFoundNetworks == null ? string.Empty : notFoundNetworks));
            }
        }

        public static void CleanNetwork()
        {
            List<CommunicationNode> orphanList = null;
            
            //remove orphans
            foreach(var network in root)
            {
                CommunicationNode current = network.Value;

                //iterate over all nodes on the network
                do
                {
                    //keep track of orphaned nodes (unity objects that have been deleted will register as null)
                    if(current.NodeOwner == null)
                    {
                        if(orphanList == null)
                            orphanList = new List<CommunicationNode>();                        

                        orphanList.Add(current);
                    }
                    
                    current = current.next[network.Key];

                } while(current != root[network.Key]);
            }

            if(orphanList != null)
            {
                //remove/clean up orphaned nodes
                for(int i = 0; i < orphanList.Count; ++i)
                {
                    if(debugPrintAllActivity)
                    {
                        DebugLogger.Log("Cleaning orphaned node " + orphanList[i]);
                    }

                    RemoveNode(orphanList[i]);
                }
            }
        }

        /// <summary>
        /// Invokes all matching methods contained in nodes on the given network.
        /// </summary>
        /// <param name="publishedData">The data passed to the methods to be invoked.</param>
        /// <param name="publisher">Typically, the object that is invoking publish.</param>
        /// <param name="network">The network to publish to.</param>
        protected static void PublishToNetwork(object publishedData, object publisher, Tags tags, string network)
        {
            CommunicationNode current = null;

            if(!root.TryGetValue(network, out current))
                return;

            List<CommunicationNode> orphanList = null;

            //iterate over all nodes on the network
            do
            {
                //keep track of orphaned nodes (unity objects that have been deleted will register as null)
                if(current.NodeOwner == null)
                {
                    if(orphanList == null)
                        orphanList = new List<CommunicationNode>();

                    orphanList.Add(current);
                    continue;
                }

                //prevent sources from publishing to themselves
                if(current.allowPublishToSelf || current.NodeOwner != publisher)
                {
                    if(debugPrintAllActivity)
                    {
                        if(current.HasMatchingCallback(publishedData, publisher, tags))
                            DebugLogger.Log("Publish is Invoking handler for : " + GetObjectTypeName(publishedData).Colorize(UnityEngine.Color.green) + " on " + GetObjectName(current.NodeOwner).Colorize(UnityEngine.Color.yellow) + " sent by " + GetObjectName(publisher).Colorize(UnityEngine.Color.yellow));
                    }
                    bool result = current.InvokeMatchingCallback(publishedData, publisher, tags);
                    if(result)
                    {
                        debugPublishWasHandled = true;
                    }
                }

                current = current.next[network];

            } while(current != root[network]);

            if(orphanList != null)
            {
                //remove/clean up orphaned nodes
                for(int i = 0; i < orphanList.Count; ++i)
                {
                    RemoveNode(orphanList[i]);
                }
            }
        }

        /// <summary>
        /// Init the previous and next members.
        /// </summary>
        public CommunicationNode()
        {
            next = new Dictionary<string, CommunicationNode>();
            prev = new Dictionary<string, CommunicationNode>();
        }

        /// <summary>
        /// Does this node have any non-null connections on any networks?
        /// </summary>
        protected static bool NodeHasAnyConnections(CommunicationNode node)
        {
            foreach(var n in node.next)
            {
                if(n.Value == null || n.Value.next != null)
                    return true;
            }

            foreach(var p in node.prev)
            {
                if(p.Value == null || p.Value.prev != null)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Create any networks that don't already exist
        /// </summary>
        protected static void CreateNetworks(CommunicationNode node, List<string> networkCollection)
        {
            foreach(string n in networkCollection)
            {
                if(root.ContainsKey(n))
                {
                    if(root[n] != null)
                        continue;

                    root[n] = node;
                }
                else
                {
                    root.Add(n, node);
                }

                root[n].next[n] = root[n];
                root[n].prev[n] = root[n];
            }
        }

        /// <summary>
        /// Add the given node to all networks. 
        /// This method assumes that the networks exist and that the node is not already part of these networks.
        /// </summary>
        protected static void AddNodeToNetworks(CommunicationNode node, List<string> networkCollection)
        {
            foreach(string n in networkCollection)
            {
                CommunicationNode prev = root[n];
                CommunicationNode next = root[n].next[n];

                node.next[n] = next;
                node.prev[n] = prev;

                next.prev[n] = node;
                prev.next[n] = node;
            }
        }

        /// <summary>
        /// Internal method that adds a node to the given networks in an intrusive linked-list style.
        /// </summary>
        protected static void AddNode( CommunicationNode node, List<string> networks )
        {
            if(debugPrintAllActivity)
            {
                string addingToNetworks = "";
                networks.Select(x => { addingToNetworks += x + ", "; return x; });
                addingToNetworks = addingToNetworks.TrimEnd(',',' ');
                DebugLogger.Log("Adding node with owner "+ GetObjectTypeName(node.NodeOwner).Colorize(UnityEngine.Color.green)+"::"+GetObjectName(node.NodeOwner).Colorize( UnityEngine.Color.yellow ) + " to network(s): "+addingToNetworks);
            }

            //if the node has non-null connections, clear them by removing it before we process the insertion
            if(NodeHasAnyConnections(node))
                RemoveNode( node );

            CreateNetworks(node, networks);

            //add new nodes to the root
            AddNodeToNetworks(node, networks);
        }

        /// <summary>
        /// Internal method that removes a node from the network in an intrusive linked-list style.
        /// </summary>
        protected static void RemoveNode( CommunicationNode node )
        {
            if(node.next == null)
                return;

            if(debugPrintAllActivity)
            {
                string removingNetworks = "";
                if(node != null && node.Networks != null )
                    node.Networks.ToList().Select(x => { removingNetworks += x + ", "; return x; });
                removingNetworks = removingNetworks.TrimEnd(',', ' ');
#if UNITY_EDITOR
                if(!UnityEngine.Application.isPlaying)
#endif
                    DebugLogger.Log("Removing node with owner " + GetObjectTypeName(node.NodeOwner).Colorize( UnityEngine.Color.green ) + "::" + GetObjectName(node.NodeOwner).Colorize( UnityEngine.Color.yellow ) + " from network(s): " + removingNetworks);
            }

            foreach(string n in networks)
            {
                if(!node.next.ContainsKey(n))
                    continue;

                if(node.next[n] != null)
                    node.next[n].prev[n] = node.prev[n];
                if(node.prev[n] != null)
                    node.prev[n].next[n] = node.next[n];

                node.next[n] = null;
                node.prev[n] = null;

                //remove the network if it exists and it's empty
                if(root != null && root.ContainsKey(n) && root[n].next[n] == null && root[n].prev[n] == null)
                    root.Remove(n);
            }
        }

        protected virtual bool HasMatchingCallback( object publishedData, object publisher, Tags tags )
        {
            bool result = false;

            if( NodeOwner == null )
                return result;

            Type handlerKey = publishedData.GetType();
            Dictionary<string, MethodInfo> matchingHandlers;

            //Does a method handling this data type exist?
            if( !enabledHandlers.TryGetValue( handlerKey, out matchingHandlers ) )
                return result;

            MethodInfo method = null;

            //were any tags specified on publish?
            if( tags.Count > 0 )
            {
                //if any, check each tag and take the first matching key
                if( tags.matching == Tags.Matching.Any )
                {
                    foreach( var t in tags.tags )
                    {
                        if( matchingHandlers.ContainsKey( t ) )
                        {
                            method = matchingHandlers[ t ];
                            break;
                        }
                    }
                }
                //if all, try to match all tags
                else if( tags.matching == Tags.Matching.All )
                {
                    if( !matchingHandlers.TryGetValue( tags.ToString(), out method ) )
                        return result;
                }
            }
            else
            {
                //none specified? see if there's a default
                if( !matchingHandlers.TryGetValue( string.Empty, out method ) )
                    return result;
            }

            result = ( method != null );
            return result;            
        }

        /// <summary>
        /// Invoke the callback that matches the data type of the data passed to the publish method.
        /// </summary>
        /// <returns>Returns true if this object contained a handler that was invoked.</returns>
        protected virtual bool InvokeMatchingCallback( object publishedData, object publisher, Tags tags )
        {
            bool result = false;

            if( NodeOwner == null )
                return result;

            Type handlerKey = publishedData.GetType();
            Dictionary<string, MethodInfo> matchingHandlers;

            //Does a method handling this data type exist?
            if(!enabledHandlers.TryGetValue( handlerKey, out matchingHandlers))
                return result;

            MethodInfo method = null;

            //were any tags specified on publish?
            if(tags.Count > 0)
            {
                //if any, check each tag and take the first matching key
                if(tags.matching == Tags.Matching.Any)
                {
                    foreach(var t in tags.tags)
                    {
                        if(matchingHandlers.ContainsKey(t))
                        {
                            method = matchingHandlers[t];
                            break;
                        }
                    }
                }
                //if all, try to match all tags
                else if(tags.matching == Tags.Matching.All)
                {
                    if(!matchingHandlers.TryGetValue(tags.ToString(), out method))
                        return result;
                }
            }
            else
            {
                //none specified? see if there's a default
                if(!matchingHandlers.TryGetValue(string.Empty, out method))
                    return result;
            }

            if(method == null)
                return result;

            if(NodeOwner == null)
                return result;
            
            try
            {
                //also send out the publisher
                if(method.GetParameters().Length == 2)
                {
                    Type publisherType = (publisher == null ? null : publisher.GetType());
                    Type publisherParameterType = method.GetParameters()[1].ParameterType;

                    //only warn if the reciever method is expecting a specific type for the publisher and it doesn't match
                    if(publisherParameterType != typeof(object) && publisherParameterType != publisherType)
                    {
                        string publisherTypeName = GetObjectTypeName(publisher);
                        DebugLogger.Log(method.Name + " parameter 2 has type " + publisherTypeName + " which does not match the method's type " + publisherParameterType.Name);
                    }

                    method.Invoke(NodeOwner, new object[] { publishedData, publisher });
                }
                else
                {
                    //don't send the publisher
                    method.Invoke(NodeOwner, new object[] { publishedData });
                }
                result = true;
            }
            catch(Exception e)
            {
                string monoName = ") ";
                if(NodeOwner as UnityEngine.MonoBehaviour != null)
                    monoName = ", " + (NodeOwner as UnityEngine.MonoBehaviour).gameObject.name + ") ";
                else if(NodeOwner as UnityEngine.Component != null)
                    monoName = ", " + (NodeOwner as UnityEngine.Component).gameObject.name + ") ";
                else if(NodeOwner as UnityEngine.Object != null)
                    monoName = ", " + (NodeOwner as UnityEngine.Object).name + ") ";

                if(tags.Count > 0)
                {
                    monoName = monoName.TrimEnd(')',' ');
                    monoName += " with tags " + tags.ToString() + ") ";
                }

                string publisherName = GetObjectName(publisher);

                string errorString = "Unhandled exception caught when invoking matching callback from publisher "+ publisherName + " in handler " + method.Name + " on object (" + NodeOwner.GetType().Name + monoName + ": " + e.Message + " -> " + e.InnerException.Message + " at " + e.InnerException.StackTrace;
#if UNITY_5_3_OR_NEWER
                DebugLogger.LogError(errorString);
#else
                        DebugLogger.WriteLine(errorString);
#endif
            }
            return result;
        }
        
        /// <summary>
        /// Searches the NodeOwner object for any methods that have the attribute [CommunicationCallback] and have a function signature that matches the requirements.
        /// </summary>
        protected virtual void RefreshCallbackBindings()
        {
            if( debugPrintAllActivity )
            {
                if( enabledHandlers.Count > 0 )
                    DebugLogger.Log( "On " + GetObjectTypeName( NodeOwner ) + "::" + GetObjectName( NodeOwner ) + " Clearing all handlers");
            }
            enabledHandlers.Clear();
            
            // Iterate over all methods on the owner object.
            var methodInfos = NodeOwner.GetType().GetMethods(bindingFlags);
            foreach( MethodInfo mi in methodInfos )
            {
                // the method has a [CommunicationCallback] attribute
                bool isHandler = mi.GetCustomAttributes( true ).OfType<CommunicationCallback>().Any();
                if(!isHandler)
                    continue;

                ParameterInfo[] parameters = mi.GetParameters();
                
                //handlers must take 1 or 2 parameters
                if( parameters.Length > 0 || parameters.Length < 3)
                {
                    Type handlerKey = parameters[0].ParameterType;
                    CommunicationCallback attribute = mi.GetCustomAttributes(true).OfType<CommunicationCallback>().FirstOrDefault();

                    if( !enabledHandlers.ContainsKey( handlerKey ) )
                    {
                        enabledHandlers.Add( handlerKey, new Dictionary<string, MethodInfo>() );
                    }

                    string[] tags = attribute.tags;
                    if(tags.Length > 0 && attribute.tags.matching == Tags.Matching.Any)
                    {
                        foreach(string s in tags)
                        {
                            enabledHandlers[handlerKey].Add(s, mi);
                            
                            if( debugPrintAllActivity )
                            {
                                DebugLogger.Log( "On "+ GetObjectTypeName(NodeOwner)+"::"+GetObjectName(NodeOwner)+" Adding handler for type "+ handlerKey.Name.Colorize(UnityEngine.Color.green) + " with tags " + attribute.tags );
                            }
                        }
                    }
                    else if(tags.Length <= 0 || attribute.tags.matching == Tags.Matching.All)
                    {
                        enabledHandlers[handlerKey].Add(string.Empty, mi);

                        if( debugPrintAllActivity )
                        {
                            DebugLogger.Log( "On " + GetObjectTypeName( NodeOwner ) + "::" + GetObjectName( NodeOwner ) + " Adding handler for type " + handlerKey.Name.Colorize( UnityEngine.Color.green ) );
                        }
                    }
                    else
                    {
#if UNITY_5_3_OR_NEWER
                        DebugLogger.LogError("Unknown tag configuration "+ attribute.tags.matching + ". Handler "+ mi.Name + " not added to subscribed callbacks.");
#else
                        DebugLogger.WriteLine("Unknown tag configuration "+ attribute.tags.matching + ". Handler "+ mi.Name + " not added to subscribed callbacks.");
#endif
                    }
                }
                else
                {
#if UNITY_5_3_OR_NEWER
                    DebugLogger.LogError(mi.Name+" - Handler callbacks must take 1 or 2 parameters. The first parameter is the data type to be handled, the 2nd is optional and will be the object invoking the handler.");
#else
                    DebugLogger.WriteLine(methodInfo.Name+" - Handler callbacks must take 1 or 2 parameters. The first parameter is the data type to be handled, the 2nd is optional and will be the object invoking the handler.");
#endif
                }
            }
        }

        /// <summary>
        /// Debug helper: Get the unity name or type name of the object.
        /// </summary>
        /// <returns>The unity name of the object or the type name if the type is not a unity type.</returns>
        protected static string GetObjectName(object thing)
        {
            if(thing == null)
                return "null";

            string name = GetUnityObjectName(thing);
            if(string.IsNullOrEmpty(name))
                name = thing.GetType().Name;
            return name;
        }

        protected static string GetObjectTypeName( object thing )
        {
            if( thing == null )
                return "null";            
            return thing.GetType().Name;
        }

        /// <summary>
        /// Debug helper: Get the unity name of the object.
        /// </summary>
        /// <returns>The unity name of the object.</returns>
        protected static string GetUnityObjectName(object thing)
        {
            string name = "";
#if UNITY_5_3_OR_NEWER
            if(thing as UnityEngine.MonoBehaviour != null)
                name = (thing as UnityEngine.MonoBehaviour).gameObject.name;
            else if(thing as UnityEngine.Component != null)
                name = (thing as UnityEngine.Component).gameObject.name;
            else if(thing as UnityEngine.Object != null)
                name = (thing as UnityEngine.Object).name;
#endif
            return name;
        }
    }
}
