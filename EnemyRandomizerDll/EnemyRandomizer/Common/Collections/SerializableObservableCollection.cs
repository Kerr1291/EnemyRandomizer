using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace nv
{
    /// <summary>
    /// A collection with hooks for any action upon it.
    /// </summary>
    [Serializable]
    public abstract class SerializableObservableCollection<T, 
        TCollectionNotificationEvent,
        TItemNotificationEvent,
        TItemsNotificationEvent> : AnObservableCollection<T>, ISerializationCallbackReceiver
        where TCollectionNotificationEvent : UnityEvent<AnObservableCollection<T>>, new()
        where TItemNotificationEvent : UnityEvent<int, T>, new()
        where TItemsNotificationEvent : UnityEvent<IList<T>>, new() 
    {
        public SerializableObservableCollection()
            : base()
        {
            InvokeOnNewCollectionCreated();
        }

        public SerializableObservableCollection(IList<T> items)
            : base(items.ToList())
        {
            InvokeOnNewCollectionCreated();
        }

        public SerializableObservableCollection(ICollection<T> items)
            : base(items.ToList())
        {
            InvokeOnNewCollectionCreated();
        }

        public static implicit operator List<T>(SerializableObservableCollection<T, TCollectionNotificationEvent, TItemNotificationEvent, TItemsNotificationEvent> items)
        {
            return items.Items.ToList();
        }

        public static implicit operator T[] (SerializableObservableCollection<T, TCollectionNotificationEvent, TItemNotificationEvent, TItemsNotificationEvent> items)
        {
            return items.Items.ToArray();
        }

        public virtual void ForEach(Action<T> action)
        {
            if(Items is List<T>)
            {
                (Items as List<T>).ForEach(action);
            }
            else
            {
                Items.ToList().ForEach(action);
            }
        }

        protected override void ClearItems()
        {
            var itemsCopy = Items.ToList();
            base.ClearItems();
            if(!isSerializing)
                onClearItemsEvent.Invoke(itemsCopy);
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            if(!isSerializing)
                onInsertItemEvent.Invoke(index, item);
        }

        protected override void InvokeOnChanged()
        {
            base.InvokeOnChanged();
            if(!isSerializing)
            {
                onChangedEvent.Invoke();
                onChangedWithThisEvent.Invoke(this);
            }
        }

        protected override void InvokeOnCountChanged()
        {
            base.InvokeOnCountChanged();
            if(!isSerializing)
            {
                onCountChangedEvent.Invoke();
                onCountChangedWithThisEvent.Invoke(this);
            }
        }

        protected override void RemoveItem(int index)
        {
            T itemThatWasRemoved = Items[index];
            base.RemoveItem(index);
            if(!isSerializing)
                onRemoveItemEvent.Invoke(index, itemThatWasRemoved);
        }

        protected override void SetItem(int index, T item)
        {
            base.SetItem(index, item);
            if(!isSerializing)
                onSetItemEvent.Invoke(index, item);
        }

        public override void SetItems(IEnumerable<T> items)
        {
            base.SetItems(items);
            if(!isSerializing)
                onSetItemsEvent.Invoke(Items);
        }

        public override void SetItems<U>(U items)
        {
            base.SetItems(items);
            if(!isSerializing)
                onSetItemsEvent.Invoke(Items);
        }

        public TItemNotificationEvent onSetItemEvent;
        public TItemNotificationEvent onInsertItemEvent;
        public TItemNotificationEvent onRemoveItemEvent;
        public TItemsNotificationEvent onSetItemsEvent;
        public TItemsNotificationEvent onClearItemsEvent;
        public UnityEvent onChangedEvent;
        public TCollectionNotificationEvent onChangedWithThisEvent;
        public UnityEvent onCountChangedEvent;
        public TCollectionNotificationEvent onCountChangedWithThisEvent;
        
        [SerializeField]
        protected T[] itemsEditorBackingField;

        bool isSerializing;

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if(Items != null)
                itemsEditorBackingField = Items.ToArray();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if(itemsEditorBackingField != null && itemsEditorBackingField.Length > 0)
            {
                //can't invoke unity events during serialize (and you wouldn't want to)
                isSerializing = true;
                SetItems(itemsEditorBackingField);
                itemsEditorBackingField = null;
                isSerializing = false;
            }
        }
    }

    //[Serializable]
    //public abstract class FastUnityEventBase
    //{
    //    [SerializeField]
    //    protected string editorEventName;

    //    protected bool IsSerializing()

    //    protected abstract UnityEventBase CreateEvent();
        
    //    protected abstract void UpdateEvent();

    //    protected abstract void UpdateAction();
    //}

    //[Serializable]
    //public class FastUnityEvent<TDelegate, TUnityEvent> : FastUnityEventBase
    //    where TUnityEvent : UnityEventBase, new()
    //    where TDelegate : class
    //{
    //    public FastUnityEvent()
    //    {
    //        editorEventName = typeof(TUnityEvent).GetType().Name;
    //        if(!typeof(Delegate).IsAssignableFrom(typeof(TDelegate)))
    //            throw new InvalidCastException(typeof(TDelegate).Name + " <- type is not a Delegate! The first type parameter of a FastUnityEvent<> must be a Delegate!");
    //    }

    //    protected override UnityEventBase CreateEvent()
    //    {
    //        return new TUnityEvent();
    //    }

    //    protected override void UpdateEvent()
    //    {
    //        if(Event == null)
    //            Event = CreateEvent() as TUnityEvent;

    //        if(action != null)
    //        {
    //            Delegate[] invocationList = (Delegate[])typeof(TDelegate).GetType().GetMethod("GetInvocationList").Invoke(action, null);
    //            foreach(var d in invocationList)
    //            {
    //                Event.AddPersistentListener(d.Target as UnityEngine.Object, d.Method);
    //            }
    //        }
    //    }

    //    protected override void UpdateAction()
    //    {
    //        action = Event.ToDelegate<TDelegate>();
    //    }

    //    [SerializeField]
    //    protected TUnityEvent Event;

    //    protected TDelegate action;
    //    public virtual TDelegate Action
    //    {
    //        get
    //        {
    //            if(IsSerializing || !Application.isPlaying)
    //            {
    //                return OnSetItemEditor.ToDelegate<ItemNotificationDelegate>();
    //            }
    //            else
    //            {
    //                return onSetItem ?? (onSetItem = (OnSetItemEditor != null ? OnSetItemEditor.ToDelegate<ItemNotificationDelegate>() : onSetItem));
    //            }
    //        }
    //        set
    //        {
    //            action = value;
    //            if(!Application.isPlaying)
    //                UpdateEvent();
    //        }
    //    }
    //}
}



/*
 * 

        protected abstract UnityEvent<int, T> CreateItemNotificationEvent();

        protected abstract UnityEvent<int, T> OnSetItemEditor { get; set; }
        public ItemNotificationDelegate onSetItem;
        public override ItemNotificationDelegate OnSetItem
        {
            get
            {
                if(IsSerializing || !Application.isPlaying)
                {
                    return OnSetItemEditor.ToDelegate<ItemNotificationDelegate>();
                }
                else
                {
                    return onSetItem ?? (onSetItem = (OnSetItemEditor != null ? OnSetItemEditor.ToDelegate<ItemNotificationDelegate>() : onSetItem));
                }
            }
            set
            {
                if(IsSerializing || !Application.isPlaying)
                {
                    if(OnSetItemEditor == null)
                        OnSetItemEditor = CreateItemNotificationEvent();

                    if(value != null)
                    {
                        foreach(var d in value.GetInvocationList())
                        {
                            OnSetItemEditor.AddPersistentListener(d.Target as UnityEngine.Object, d.Method);
                        }
                    }
                }
                else
                {
                    onSetItem = value;
                    OnSetItemEditor = null;
                }
            }
        }

    */