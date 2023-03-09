using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;

namespace EnemyRandomizerMod
{
    /// <summary>
    /// An indexable queue data type with wrapping-offset index access. (all index values are valid, even negative values).
    /// </summary>
    [Serializable]
    public class CircularBuffer<T>
    {
        public virtual T this[int index]
        {
            get
            {
                if(index < 0)
                {
                    int resultIndex = (StartIndex + Items.Length) + index;
                    return Items[resultIndex.Modulus(Items.Length)];
                }
                else
                {
                    return Items[(StartIndex + index).Modulus(Items.Length)];
                }
            }
        }

        public virtual T this[int index, bool reverse]
        {
            get
            {
                if(reverse)
                {
                    int resultIndex = (StartIndex + Items.Length - 1) - index;
                    return Items[resultIndex.Modulus(Items.Length)];
                }
                else
                {
                    return Items[(StartIndex + index).Modulus(Items.Length)];
                }
            }
        }

        public IEnumerable<T> Enumerator
        {
            get
            {
                for(int i = 0; i < Count; ++i)
                    yield return this[i];
            }
        }

        public IEnumerable<T> ReverseEnumerator
        {
            get
            {
                for(int i = 0; i < Count; ++i)
                    yield return this[-i];
            }
        }

        //TODO: update the buffer if MaxSize becomes smaller than count
        [SerializeField]
        protected int maxSize;
        public virtual int MaxSize
        {
            get
            {
                return maxSize;
            }
            set
            {
                maxSize = value;
            }
        }

        [SerializeField, HideInInspector]
        protected int count;
        public virtual int Count
        {
            get
            {
                return count;
            }
        }

        [SerializeField,HideInInspector]
        protected int startIndex;
        protected virtual int StartIndex
        {
            get
            {
                return startIndex;
            }
        }

        [SerializeField]
        protected T[] items;
        public virtual T[] Items
        {
            get
            {
                return items;
            }
        }

        public CircularBuffer(int initialBufferSize = 10, int maxSize = int.MaxValue)
        {
            Setup(initialBufferSize, maxSize);
        }

        public CircularBuffer(CircularBuffer<T> other)
        {
            items = new T[other.Items.Length];
            other.Items.CopyTo(Items, 0);
            MaxSize = other.MaxSize;
            count = other.Count;
            startIndex = other.StartIndex;
        }

        public virtual void Setup(int initialBufferSize = 10, int maxSize = int.MaxValue)
        {
            if(maxSize < initialBufferSize)
                throw new System.ArgumentException("maxSize cannot be less than initialBufferSize.");
            if(maxSize == 0)
                throw new System.ArgumentException("maxSize cannot be zero.");
            items = new T[initialBufferSize];
            startIndex = 0;
            count = 0;
            MaxSize = maxSize;
        }

        public virtual void Enqueue(T t)
        {
            if(Count == Items.Length)
            {
                if(Count < MaxSize)
                {
                    //increase the size of the cicularBuffer, and copy everything
                    T[] bigger = new T[System.Math.Min(Items.Length * 2, MaxSize)];
                    for(int i = 0; i < Count; i++)
                    {
                        bigger[i] = Items[(StartIndex + i) % Count];
                    }
                    startIndex = 0;
                    items = bigger;
                }
                else
                {
                    Dequeue();
                }
            }

            Items[(StartIndex + Count) % Items.Length] = t;
            ++count;
        }

        public virtual T Dequeue()
        {
            var result = Items[StartIndex];
            startIndex = (StartIndex + 1) % Items.Length;
            --count;
            return result;
        }

        public virtual List<T> Clear()
        {
            List<T> itemsRemoved = new List<T>();
            while(Count > 0)
                itemsRemoved.Add(Dequeue());
            return itemsRemoved;
        }

        ///// <summary>
        ///// Mathematical modulus, different from the % operation that returns the remainder.
        ///// This performs a "wrap around" of the given value assuming the range [0, mod)
        ///// Define mod 0 to return the value unmodified
        ///// </summary>
        //protected virtual int Modulus(int value, int mod)
        //{
        //    if(value > 0)
        //        return (value % mod);
        //    else if(value < 0)
        //        return (value % mod + mod) % mod;
        //    else//value == 0
        //        return value;
        //}
    }
}