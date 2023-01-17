using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;
using UnityEngine.Events;

namespace nv
{
    /// <summary>
    /// A collection with hooks for any action upon it.
    /// </summary>
    [Serializable]
    public class AnObservableCollection<T> : Collection<T>
    {
        public delegate void CollectionNotificationDelegate(AnObservableCollection<T> thisCollection);
        public delegate void ItemNotificationDelegate(int index, T item);
        public delegate void ItemsNotificationDelegate(IList<T> items);

        /// <summary>
        /// This callback may be used to globally register for the creation of any new collection
        /// </summary>
        public static CollectionNotificationDelegate OnNewCollectionCreated { get; set; }

        public virtual ItemNotificationDelegate OnSetItem { get; set; }

        public virtual ItemNotificationDelegate OnInsertItem { get; set; }
        public virtual ItemNotificationDelegate OnRemoveItem { get; set; }
        public virtual ItemsNotificationDelegate OnSetItems { get; set; }
        public virtual ItemsNotificationDelegate OnClearItems { get; set; }

        /// <summary>
        /// This callback may be used to register for any change that occurs
        /// </summary>
        public virtual Action OnChanged { get; set; }
        /// <summary>
        /// This callback may be used to register for any change that occurs
        /// </summary>
        public virtual CollectionNotificationDelegate OnChangedWithThis { get; set; }

        /// <summary>
        /// This callback may be used to register for any change except set (in otherwords, whenever the change would potentially effect the backing array's size)
        /// </summary>
        public virtual Action OnCountChanged { get; set; }
        /// <summary>
        /// This callback may be used to register for any change except set (in otherwords, whenever the change would potentially effect the backing array's size)
        /// </summary>
        public virtual CollectionNotificationDelegate OnCountChangedWithThis { get; set; }

        public static implicit operator List<T>(AnObservableCollection<T> items)
        {
            return items.Items.ToList();
        }

        public static implicit operator T[] (AnObservableCollection<T> items)
        {
            return items.Items.ToArray();
        }

        public AnObservableCollection()
            : base()
        {
            InvokeOnNewCollectionCreated();
        }

        public AnObservableCollection(IList<T> items)
            : base(items.ToList())
        {
            InvokeOnNewCollectionCreated();
        }

        public AnObservableCollection(ICollection<T> items)
            : base(items.ToList())
        {
            InvokeOnNewCollectionCreated();
        }

        public virtual T GetItem(int index)
        {
            return this[index];
        }

        public virtual void SetItemAt(int index, T item)
        {
            this[index] = item;
        }

        public virtual int GetCount()
        {
            return Count;
        }

        public virtual void SetItems(IEnumerable<T> items)
        {
            Items.Clear();
            if(Items is List<T>)
            {
                (Items as List<T>).AddRange(items);
            }
            else
            {
                foreach(var item in items)
                {
                    Items.Add(item);
                }
            }
            if(OnSetItems != null)
                OnSetItems.Invoke(Items);
            InvokeOnChanged();
            InvokeOnCountChanged();
        }

        public virtual void SetItems<U>(U items)
            where U : ICollection<T>
        {
            Items.Clear();
            if(Items is List<T>)
            {
                (Items as List<T>).AddRange(items);
            }
            else
            {
                foreach(var item in items)
                {
                    Items.Add(item);
                }
            }
            if(OnSetItems != null)
                OnSetItems.Invoke(Items);
            InvokeOnChanged();
            InvokeOnCountChanged();
        }

        protected override void ClearItems()
        {
            if(OnClearItems != null)
            {
                var itemsCopy = Items.ToList();
                base.ClearItems();
                OnClearItems.Invoke(itemsCopy);
            }
            else
            {
                base.ClearItems();
            }
            InvokeOnChanged();
            InvokeOnCountChanged();
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            if(OnInsertItem != null)
                OnInsertItem.Invoke(index, item);
            InvokeOnChanged();
            InvokeOnCountChanged();
        }

        protected override void SetItem(int index, T item)
        {
            base.SetItem(index, item);
            if(OnSetItem != null)
                OnSetItem.Invoke(index, item);
            InvokeOnChanged();
        }

        protected override void RemoveItem(int index)
        {
            T itemThatWasRemoved = Items[index];
            base.RemoveItem(index);
            if(OnRemoveItem != null)
                OnRemoveItem.Invoke(index, itemThatWasRemoved);
            InvokeOnChanged();
            InvokeOnCountChanged();
        }

        protected virtual void InvokeOnNewCollectionCreated()
        {
            if(OnNewCollectionCreated != null)
                OnNewCollectionCreated.Invoke(this);
        }

        protected virtual void InvokeOnCountChanged()
        {
            if(OnChanged != null)
                OnChanged.Invoke();
            if(OnChangedWithThis != null)
                OnChangedWithThis.Invoke(this);
        }

        protected virtual void InvokeOnChanged()
        {
            if(OnCountChanged != null)
                OnCountChanged.Invoke();
            if(OnCountChangedWithThis != null)
                OnCountChangedWithThis.Invoke(this);
        }
    }

}