using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefabToPool;
    public int poolSize = 10;
    private List<GameObject> _objectPool;


    private void Start()
    {
        _objectPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefabToPool);
            obj.SetActive(false);
            if (obj.TryGetComponent(out ReturnPool returnPool))
            {
                returnPool.objectPool = this;
            }

            _objectPool.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < _objectPool.Count; i++)
        {
            if (!_objectPool[i].activeInHierarchy)
            {
                return _objectPool[i];
            }
        }

        return _objectPool[0];
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
    }
}