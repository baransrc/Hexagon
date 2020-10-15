using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolItem 
{
    [SerializeField] private int _pooledAmount;
    [SerializeField] private GameObject _gameObjectToPool;
    [SerializeField] private PoolingId _poolingId;

    public int PooledAmount 
    { 
        get
        {
            return _pooledAmount;
        }
        
        private set {}
    }

    public GameObject GameObjectToPool 
    { 
        get
        {
            return _gameObjectToPool;
        }
        
        private set {}
    }

    public PoolingId PoolingId 
    { 
        get
        {
            return _poolingId;
        }
        
        private set {}
    }
}