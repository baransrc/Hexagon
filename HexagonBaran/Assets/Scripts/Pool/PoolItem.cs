using UnityEngine;

[System.Serializable]
public class PoolItem 
{
    [SerializeField] private int pooledAmount;
    [SerializeField] private GameObject gameObjectToPool;
    [SerializeField] private PoolingId poolingId;

    public int PooledAmount 
    { 
        get
        {
            return pooledAmount;
        }
        
        private set {}
    }

    public GameObject GameObjectToPool 
    { 
        get
        {
            return gameObjectToPool;
        }
        
        private set {}
    }

    public PoolingId PoolingId 
    { 
        get
        {
            return poolingId;
        }
        
        private set {}
    }
}