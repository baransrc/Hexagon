using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Pool : MonoBehaviour
{
    public static Pool SharedInstance = null;
    [SerializeField] private List<PoolItem> itemsToPool;
    [SerializeField] private Vector3 itemSpawnLocation = new Vector3 (0f,0f,0f);

    public Vector3 ItemSpawnLocation
    {
        get
        {
            return itemSpawnLocation;
        }
    }
    
    private Dictionary<PoolingId, List<GameObject>> _pool;
    
    private void Awake()
    {
        if (SharedInstance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        SharedInstance = this;
        
        InitializePool();
    }

    public void InitializePool()
    {
        if (itemsToPool == null)
            throw new System.NullReferenceException("Cannot call InitializePool(): _gameObjectsToPool is null.");

        if (itemsToPool.Count <= 0)
            throw new System.InvalidOperationException("Cannot call InitializePool(): _gameObjectsToPool is empty");

        _pool = new Dictionary<PoolingId, List<GameObject>>();
        
        foreach (var item in itemsToPool)
        {
            FillPoolWithObjects(item);
        }
    }

    private void FillPoolWithObjects(PoolItem item)
    {
        _pool.Add(item.PoolingId, new List<GameObject>());
        var list = _pool[item.PoolingId];

        for (var i = 0; i < item.PooledAmount; i++)
        {
            var newObject = Instantiate(item.GameObjectToPool, itemSpawnLocation, Quaternion.identity);
            newObject.SetActive(false);
            list.Add(newObject);
        }
    }

    private GameObject CreateObject(PoolItem item)
    {
        var list = _pool[item.PoolingId];
        var newObject = Instantiate(item.GameObjectToPool, itemSpawnLocation, Quaternion.identity);
        
        newObject.SetActive(false);
        list.Add(newObject);

        return newObject;
    }

    private PoolItem GetPoolItem(PoolingId id)
    {
        return itemsToPool[itemsToPool.FindIndex(x => id == x.PoolingId)];
    }

    public GameObject GetPooledObject(PoolingId poolingId)
    {
        var currentList = _pool[poolingId];
        
        foreach (var item in currentList)
        {
            if (!item.activeInHierarchy)
            {
                item.SetActive(true);
                return item;
            }
        }
        
        var gameObj = CreateObject(GetPoolItem(poolingId));
        gameObj.SetActive(true);

        return gameObj;
    }

    public List<GameObject> GetAllReferencesOf(PoolingId poolingId)
    {
        return _pool[poolingId];
    }
}