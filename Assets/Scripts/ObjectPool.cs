using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ObjectPool : MonoBehaviour
{
    #region Fields
    [Header("Max number of objects in pool."), SerializeField, Range(0, 100)] private int _maxPoolSize = 100;
    [Header("Number of prepairing objects"), SerializeField, Range(0, 100)] private int _preparingAmount = 0;
    [Header("Pool object base prefab."), SerializeField] private GameObject _poolObjectPrefab;
    [Header("Event called on every new pool object created."), SerializeField] private UnityEvent OnNewObjectCreated;

    private Queue<GameObject> _currentPoolObjects = new Queue<GameObject>();
    private List<PoolObjectInfo> _poolObjectsInfos = new List<PoolObjectInfo>();
    private int _objectsCount = 0;
    #endregion

    #region Properties
    public List<GameObject> ActiveObjects { get; private set; } = new List<GameObject>();
    #endregion

    #region Methods
    private void OnValidate()
    {
        if(_preparingAmount < 0 )
            _preparingAmount = 0;
        else if(_preparingAmount > _maxPoolSize)
            _preparingAmount = _maxPoolSize;
    }

    private void Awake()
    {
        if (_preparingAmount > 0)
        {
            PrepareSomeObjects(_preparingAmount);
        }
    }

    private void PrepareSomeObjects(int Amount)
    {
        if(Amount < 0)
            Amount = 0;
        else if(Amount > _maxPoolSize)
            Amount = _maxPoolSize;

        for(int i = 1; i <= Amount; i++)
        {
            GameObject newPoolObject = SpawnNewObject();
            ReturnObjectToPool(newPoolObject);
        }
    }

    public GameObject GetObjectFromPool(out PoolObjectInfo Info)
    {
        GameObject poolObject = null;

        if(_currentPoolObjects.Count == 0)
        {
            poolObject = SpawnNewObject();
        }
        else
        {
            poolObject = _currentPoolObjects.Dequeue();

            IEnumerable<PoolObjectInfo> poolObjectMatchedInfo = _poolObjectsInfos.Where(objInfo => objInfo.ObjectReference == poolObject);
            PoolObjectInfo objectInfo = poolObjectMatchedInfo.FirstOrDefault();
            PoolObjectResetter resetter = objectInfo.Resetter;
            
            if (resetter != null)
                resetter.Reset();

            poolObject.transform.parent = null;
        }

        if(!poolObject.activeInHierarchy)
        {
            poolObject.SetActive(true);
        }

        ActiveObjects.Add(poolObject);

        IEnumerable<PoolObjectInfo> matchedInfo = _poolObjectsInfos.Where(objInfo => objInfo.ObjectReference == poolObject);
        Info = matchedInfo.First();

        return poolObject;
    }

    private GameObject SpawnNewObject()
    {
        GameObject poolObject = Instantiate(_poolObjectPrefab, null);
        PoolBelongingContainer returnToPool = poolObject.AddComponent<PoolBelongingContainer>();
        returnToPool.Pool = this;
        returnToPool.ReturnToPoolCallback = delegate { ReturnObjectToPool(poolObject); };
        returnToPool.FindReturnPoint();
        PoolObjectResetter resetter = poolObject.AddComponent<PoolObjectResetter>();
        poolObject.name += $"_{_objectsCount}";
        OnNewObjectCreated?.Invoke();
        PoolObjectInfo info = new PoolObjectInfo(poolObject);
        _poolObjectsInfos.Add(info);
        _objectsCount++;
        return poolObject;
    }

    public void ReturnObjectToPool(GameObject PoolObject)
    {
        if(_currentPoolObjects.Count == _maxPoolSize)
        {
            Debug.Log("Object pool is full!");
            return;
        }

        IEnumerable<PoolObjectInfo> matchedInfo = _poolObjectsInfos.Where(objInfo => objInfo.ObjectReference == PoolObject);
        PoolObjectInfo info = null;

        if(matchedInfo != null && matchedInfo.Count() > 0)
        {
            info = matchedInfo.First();
        }

        if (info == null)
            return;

        if (info.PoolBelonging == null)
            return;

        if(info.PoolBelonging.Pool != this)
            return;

        if(_currentPoolObjects.Contains(PoolObject))
            return;

        _currentPoolObjects.Enqueue(PoolObject);
        PoolObject.transform.parent = transform;
        PoolObject.transform.localPosition = Vector3.zero;

        if (PoolObject.activeInHierarchy)
            PoolObject.SetActive(false);

        ActiveObjects.Remove(PoolObject);
    }
    #endregion
}

public class PoolBelongingContainer : MonoBehaviour
{
    [field: Header("Object pool."), SerializeField] public ObjectPool Pool { get; set; } 

    public Action ReturnToPoolCallback { get; set; }

    public void FindReturnPoint()
    {
        Health health = (Health)ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(gameObject, typeof(Health));
        if (health != null)
        {
            health.OnDeathCallback += ReturnToPoolCallback;
        }

        MovementContainer movement = (MovementContainer)ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(gameObject, typeof(MovementContainer));
        if (movement != null)
        {
            movement.Controller.OnMovementStopped += ReturnToPoolCallback;
        }
    }
}

public class PoolObjectResetter : MonoBehaviour
{
    public void Reset()
    {
        Component[] resetComponents = ComponentsSearcher.GetComponentsOfTypeFromObjectAndAllChildren(gameObject, typeof(IReset));

        if(resetComponents != null && resetComponents.Length > 0)
        {
            foreach(Component component in resetComponents)
            {
                (component as IReset).Reset();
            }
        }
    }
}

public class PoolObjectInfo
{
    public GameObject ObjectReference { get; private set; }
    public Transform ObjectTransform { get; private set; }
    public Health Health { get; private set; }
    public MovementContainer MovementContainer { get; private set; }
    public DamageDealer DamageDealer { get; private set; }
    public PoolObjectResetter Resetter { get; private set; }
    public PoolBelongingContainer PoolBelonging { get; private set; }

    public PoolObjectInfo(GameObject Object)
    {
        ObjectReference = Object;
        ObjectTransform = Object.transform;
        Health = (Health)ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(Object, typeof(Health));
        MovementContainer = (MovementContainer)ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(Object, typeof(MovementContainer));
        DamageDealer = (DamageDealer)ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(Object, typeof(DamageDealer));
        Resetter = (PoolObjectResetter)ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(Object, typeof(PoolObjectResetter));
        PoolBelonging = (PoolBelongingContainer)ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(Object, typeof(PoolBelongingContainer));
    }
}