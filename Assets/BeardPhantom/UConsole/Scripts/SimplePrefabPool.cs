using System.Collections.Generic;
using UnityEngine;

namespace BeardPhantom.UConsole
{
    public class SimplePrefabPool<T> where T : Component
    {
        public readonly List<T> Alive = new List<T>();

        private readonly GameObject _prefab;

        private readonly Queue<T> _pool = new Queue<T>();

        public SimplePrefabPool(T prefab)
        {
            _prefab = prefab.gameObject;
        }

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

        public void Allocate(int count, Transform parent)
        {
            for (var i = 0; i < count; i++)
            {
                var instance = CreateNew(parent);
                Alive.Add(instance);
                Return(instance);
            }
        }

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

        private T CreateNew(Transform parent)
        {
            return Object.Instantiate(
                _prefab,
                Vector3.zero,
                Quaternion.identity,
                parent).GetComponent<T>();
        }

        public void ReturnAll()
        {
            for (var i = Alive.Count - 1; i >= 0; i--)
            {
                Return(Alive[i]);
            }
        }

        public void Return(T instance)
        {
            Alive.Remove(instance);
            instance.gameObject.SetActive(false);
            _pool.Enqueue(instance);
        }
    }
}
