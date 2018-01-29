using System.Collections.Generic;
using UnityEngine;

namespace BeardPhantom.UConsole
{
    /// <summary>
    /// A simple prefab pool
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimplePrefabPool<T> where T : Component
    {
        /// <summary>
        /// All live objects
        /// </summary>
        public readonly List<T> Alive = new List<T>();

        /// <summary>
        /// Prefab to instantiate
        /// </summary>
        private readonly GameObject _prefab;

        /// <summary>
        /// Inactive pooled objects
        /// </summary>
        private readonly Queue<T> _pool = new Queue<T>();

        public SimplePrefabPool(T prefab)
        {
            _prefab = prefab.gameObject;
        }

        /// <summary>
        /// Destroys all live and pooled objects
        /// </summary>
        public void Clear()
        {
            foreach (var obj in _pool)
            {
                Object.Destroy(obj.gameObject);
            }
            _pool.Clear();
            foreach (var obj in Alive)
            {
                Object.Destroy(obj.gameObject);
            }
            Alive.Clear();
        }

        /// <summary>
        /// Preallocates objects in the pool
        /// </summary>
        /// <param name="count"></param>
        /// <param name="parent"></param>
        public void Allocate(int count, Transform parent)
        {
            for (var i = 0; i < count; i++)
            {
                var instance = CreateNew(parent);
                Alive.Add(instance);
                Return(instance);
            }
        }

        /// <summary>
        /// Retrieves an instance and parents it to a transform
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public T Retrieve(Transform parent)
        {
            T instance;
            if (_pool.Count > 0)
            {
                instance = _pool.Dequeue();
                instance.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                if (instance.transform.parent != parent)
                {
                    instance.transform.SetParent(parent);
                }
            }
            else
            {
                instance = CreateNew(parent);
            }
            instance.gameObject.SetActive(true);
            Alive.Add(instance);
            return instance;
        }

        /// <summary>
        /// Instantiates a new instance of the prefab
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private T CreateNew(Transform parent)
        {
            return Object.Instantiate(
                _prefab,
                Vector3.zero,
                Quaternion.identity,
                parent).GetComponent<T>();
        }

        /// <summary>
        /// Returns all live objects
        /// </summary>
        public void ReturnAll()
        {
            for (var i = Alive.Count - 1; i >= 0; i--)
            {
                Return(Alive[i]);
            }
        }

        /// <summary>
        /// Returns a single live object
        /// </summary>
        /// <param name="instance"></param>
        public void Return(T instance)
        {
            Alive.Remove(instance);
            instance.gameObject.SetActive(false);
            _pool.Enqueue(instance);
        }
    }
}
