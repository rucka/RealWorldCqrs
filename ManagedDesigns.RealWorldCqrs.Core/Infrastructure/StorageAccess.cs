using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ManagedDesigns.RealWorldCqrs.Core.Infrastructure
{
    public interface IUpdateStorage : IQueryStorage, IDisposable
    {
        void Add<TItem>(TItem item) where TItem : class;
        void Remove<TItem>(TItem item) where TItem : class;
        void Update<TItem>(TItem item) where TItem : class;
    }

    public interface IQueryStorage
    {
        IEnumerable<TItem> Items<TItem>() where TItem : class;
        IEnumerable<TItem> Items<TItem>(Func<TItem, bool> where) where TItem : class;
    }

    public class InMemoryDbUpdateStorage : IUpdateStorage
    {
        private readonly IDictionary<Type, ArrayList> Data = new Dictionary<Type, ArrayList>();

        #region IUpdateStorage Members

        public IEnumerable<TItem> Items<TItem>() where TItem : class
        {
            return Items<TItem>((i) => true);
        }

        public IEnumerable<TItem> Items<TItem>(Func<TItem, bool> where) where TItem : class
        {
            lock(this)
            {
                Type type = typeof (TItem);
                if (!Data.ContainsKey(type))
                {
                    return new List<TItem>();
                }

                return Data[type].OfType<TItem>().ToArray();
            }
        }


        public void Add<TItem>(TItem item) where TItem : class
        {
            lock (this)
            {
                Type type = typeof (TItem);
                if (!Data.ContainsKey(type))
                {
                    Data.Add(type, new ArrayList());
                }
                Data[type].Add(item);
            }
        }

        public void Remove<TItem>(TItem item) where TItem : class
        {
            lock (this)
            {
                Type type = typeof (TItem);
                if (!Data.ContainsKey(type))
                {
                    return;
                }
                if (!Data[type].Contains(item))
                {
                    return;
                }
                Data[type].Remove(item);
            }
        }

        public void Update<TItem>(TItem item) where TItem : class
        {
        }

        public void Dispose()
        {
        }

        #endregion
    }
}