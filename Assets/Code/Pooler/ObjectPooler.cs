using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public enum ObjectType
    {
        Projectile,
        Cannon,
        Tower,
        Shield
    }

    [Serializable] public struct ObjectInfo
    {
        public ObjectType Type;
        public GameObject gameObject;
        public int StartAmount;
    }

    [SerializeField] private List<ObjectInfo> _poolObjectsInfo;

    public static ObjectPooler Instance;

    private Dictionary<ObjectType, Pool> _pools;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        InitPools();
    }
    private void InitPools()
    {
        _pools = new Dictionary<ObjectType, Pool>();

        var emptyGO = new GameObject();

        foreach (var obj in _poolObjectsInfo)
        {
            var container = Instantiate(emptyGO, transform, false);
            var type = obj.Type;
            container.name = obj.Type.ToString();
            _pools[type] = new Pool(container.transform);

            for (int i = 0; i < obj.StartAmount; i++)
            {
                var GO = InstantiateObject(type, container.transform);
                _pools[type].Objects.Enqueue(GO);
            }
        }

        Destroy(emptyGO);
    }
    private GameObject InstantiateObject(ObjectType type, Transform parent)
    {
        var data = _poolObjectsInfo.Find(x => x.Type == type);
        GameObject obj = Instantiate(data.gameObject, parent);
        obj.gameObject.SetActive(false);
        return obj;
    }
    public GameObject GetObject(ObjectType type, Vector3 position,Quaternion rotation)
    {
        var obj = _pools[type].Objects.Count > 0 ?
            _pools[type].Objects.Dequeue() : InstantiateObject(type, _pools[type].Container);
        obj.gameObject.SetActive(true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        return obj;
    }
    public void DestroyObject(GameObject obj)
    {
        var objType = obj.GetComponent<IPooledObject>().Type;
        _pools[objType].Objects.Enqueue(obj);
        obj.gameObject.SetActive(false);
    }
}
