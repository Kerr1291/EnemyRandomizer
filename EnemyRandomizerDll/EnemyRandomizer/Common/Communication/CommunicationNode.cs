using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace nv
{
    public class CommunicationCallback : Attribute
    {
    }
    
    [System.Serializable]
    public class CommunicationNode
    {
        protected static CommunicationNode root;
        protected CommunicationNode next;
        protected CommunicationNode prev;

        protected static Action<object,object> publishAction;

        protected readonly Dictionary<Type, MethodInfo> enabledCallbacks = new Dictionary<Type, MethodInfo>();
        protected List<SerializableMethodInfo> enabledMethodInfos = new List<SerializableMethodInfo>();

        public virtual object NodeOwner { get; private set; }

        public static void Publish( object data, object publisher )
        {
            publishAction.Invoke( data, publisher );
        }

        public virtual void Publish( object data )
        {
            Publish( data, NodeOwner );
        }

        public virtual void EnableNode( object nodeOwner )
        {
            if(nodeOwner.GetType() == this.GetType())
            {
                //error: nodes themselves cannot be owners
                //TODO: throw assert
                DisableNode();
                return;
            }

            if( nodeOwner == null )
            {
                DisableNode();
                return;
            }

            this.NodeOwner = nodeOwner;

            CommunicationNode.AddNode( this );
            RefreshCallbackBindings();
        }

        public virtual void DisableNode()
        {
            CommunicationNode.RemoveNode( this );
            NodeOwner = null;
        }

        static CommunicationNode()
        {
            //prevent publishAction from ever being null 
            publishAction -= EmptyAction;
            publishAction += EmptyAction;

            PublishAction -= DefaultPublish;
            PublishAction += DefaultPublish;
        }

        static void EmptyAction(object data, object publisher) { }
        
        protected static void AddNode( CommunicationNode node )
        {
            //if the node has non-null connections, clear them by removing it before we process the insertion
            if( node.next != null || node.prev != null )
            {
                RemoveNode( node );
            }

            if( root == null )
            {
                root = node;
                root.next = root;
                root.prev = root;
            }

            //add new nodes to the root
            CommunicationNode prev = root;
            CommunicationNode next = root.next;

            node.next = next;
            node.prev = prev;

            next.prev = node;
            prev.next = node;
        }

        protected static void RemoveNode( CommunicationNode node )
        {
            if( node.next != null )
                node.next.prev = node.prev;
            if( node.prev != null )
                node.prev.next = node.next;

            node.next = null;
            node.prev = null;

            if( root != null && root.next == null && root.prev == null )
                root = null;
        }

        protected virtual void InvokeMatchingCallback( object data, object publisher )
        {
            MethodInfo method;
            if( enabledCallbacks.TryGetValue( data.GetType(), out method ) )
            {
                if(NodeOwner != null)
                {
                    //also broadcast the publisher
                    if(method.GetParameters().Length == 2)
                    {
                        method.Invoke(NodeOwner, new object[] { data, publisher });
                    }
                    else
                    {
                        //standard way, don't send the owner
                        method.Invoke(NodeOwner, new object[] { data });
                    }
                }
            }
        }
        
        protected virtual void RefreshCallbackBindings()
        {
            enabledMethodInfos.Clear();
            enabledCallbacks.Clear();

            // get ALL public, protected, private, and internal methods defined on the node
            var methodInfos = NodeOwner.GetType().GetMethods( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );
            foreach( var methodInfo in methodInfos )
            {
                bool isReceiverMethod = methodInfo.GetCustomAttributes( true ).OfType<CommunicationCallback>().Any();
                ParameterInfo[] parameters = methodInfo.GetParameters();

                // the method has a [CommunicationCallback] attribute
                if( isReceiverMethod )
                {
                    if( parameters.Length == 1 )
                    {
                        Dev.Log("adding single parameter callback");
                        // the method has a single parameter, the callback binder doesn't expect 
                        enabledMethodInfos.Add( new SerializableMethodInfo( methodInfo ) );
                        enabledCallbacks.Add( parameters[ 0 ].ParameterType, methodInfo );
                    }
                    //this method takes two parameters, the 2nd of which is the sending object
                    else if(parameters.Length == 2)
                    {
                        Dev.Log("adding double parameter callback");
                        // the method has a two parameters, the 2nd is the sending object
                        enabledMethodInfos.Add(new SerializableMethodInfo(methodInfo));
                        enabledCallbacks.Add(parameters[0].ParameterType, methodInfo);
                    }
                    else
                    {
                        Dev.Log("method did not match any callback configuration");
                        //TODO: change to using standard exceptions so we don't have a dependency on debug logging
                        //Debug.LogErrorFormat("{0} is an invalid receiver method!  It must have exactly 1 parameter!", methodInfo.Name);
                        //Dev.Log( methodInfo.Name + "is an invalid receiver method!  It must have exactly 1 parameter!" );
                    }
                }
            }
        }

        protected static void DefaultPublish( object data, object publisher )
        {
            CommunicationNode current = root;

            List<CommunicationNode> orphanList = null;

            if( current != null )
            {
                do
                {
                    //keep track of orphaned nodes
                    if(current.NodeOwner == null)
                    {
                        if(orphanList == null)
                            orphanList = new List<CommunicationNode>();

                        orphanList.Add(current);
                        continue;
                    }

                    //prevent sources from publishing to themselves
                    if( current.NodeOwner != publisher )
                    {
                        current.InvokeMatchingCallback( data, publisher );
                    }

                    current = current.next;

                } while( current != root );
            }

            if(orphanList != null)
            {
                //remove/clean up orphaned nodes
                for(int i = 0; i < orphanList.Count; ++i)
                {
                    RemoveNode(orphanList[i]);
                }
            }
        }

        public static event Action<object, object> PublishAction
        {
            add
            {
                lock( publishAction )
                {
                    publishAction += value;
                }
            }
            remove
            {
                lock( publishAction )
                {
                    publishAction -= value;
                }
            }
        }
    }
}
