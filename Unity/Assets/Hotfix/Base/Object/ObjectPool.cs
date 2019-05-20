
using System;
using System.Collections.Generic;

namespace ETHotfix
{
    public interface IPoolAllocatedObject<T> where T : IPoolAllocatedObject<T>
    {
        void InitPool(ObjectPool<T> pool);
        T Downcast();
    }
    public class ObjectPool<T> where T : IPoolAllocatedObject<T>
    {
        public void Init(int initPoolSize, Func<T> factory)
        {
            for (int i = 0; i < initPoolSize; ++i)
            {
                T t = factory();
                t.InitPool(this);
                m_UnusedObjects.Enqueue(t);
            }
        }
        public T Alloc(Func<T> factory)
        {
            if (m_UnusedObjects.Count > 0)
                return m_UnusedObjects.Dequeue();
            else
            {
                T t = factory();
                if (null != t)
                {
                    t.InitPool(this);
                }
                return t;
            }
        }
        public void Recycle(IPoolAllocatedObject<T> t)
        {
            if (null != t)
            {
                m_UnusedObjects.Enqueue(t.Downcast());
            }
        }
        public void Clear(Action<T> destroyFunc = null)
        {
            if (destroyFunc != null)
            {
                foreach (var item in m_UnusedObjects)
                {
                    destroyFunc(item);
                }
            }
            m_UnusedObjects.Clear();
        }
        public int Count
        {
            get
            {
                return m_UnusedObjects.Count;
            }
        }

        private Queue<T> m_UnusedObjects = new Queue<T>();
    }
}
