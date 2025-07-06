using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(CircleArea))]
public class DamageDealer : MonoBehaviour, IReset
{
    #region Fields
    [field: Header("Amount of damage."), SerializeField, Range(1, 1000)] public uint DamageAmount = 10;
    private CircleArea _damageArea = null;
    private DamageManager _manager = null; 
    private bool _damageExecuted = false;
    #endregion

    #region Properties
    public Vector3 DamagePosition { get; set; } = Vector3.zero;
    #endregion

    #region Methods
    private void GetDamageManager() => _manager = DamageManager.Instance;

    public void ExecuteDamage()
    {
        Debug.Log("Damage executed? " + _damageExecuted);

        if (_damageExecuted)
            return;

        if (_manager == null)
        {
            GetDamageManager();
        }

        if (_damageArea == null)
        {
            _damageArea = (CircleArea)gameObject.GetComponent(typeof(CircleArea));
        }

        Collider[] colliders = Physics.OverlapSphere(DamagePosition, _damageArea.Radius);

        if(colliders != null && colliders.Length > 0)
        {
            IEnumerable<GameObject> enemies = colliders.Where(collider => collider.tag == "Enemy").Select(collider => collider.gameObject);

            if(enemies != null && enemies.Count() > 0)
            {
                IEnumerable<GameObject> enemiesSortedByDistance = enemies.OrderBy(enemy => Vector3.Distance(DamagePosition, enemy.transform.position));
                GameObject closestEnemy = enemiesSortedByDistance.First();

                if (closestEnemy != null && closestEnemy.activeInHierarchy)
                {
                    Debug.Log("Нанесён урон врагу");
                    SendDamageData(closestEnemy);
                }
            }
        }
        
        _damageExecuted = true;
    }

    public void SendDamageData(GameObject ReceiverObject)
    {
        Health receiverHealth = (Health)ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(ReceiverObject, typeof(Health));
        SendData(receiverHealth);
    }

    public void SendDamageData (Health ReceiverHealth)
    {
        SendData(ReceiverHealth);
    }

    private void SendData(Health Receiver)
    {
        if (Receiver == null)
            return;

        DamageData data = new DamageData(Receiver, DamageAmount);
        _manager.AddData(data);
    }

    public void Reset ()
    {
        _damageExecuted = false;
        DamagePosition = Vector3.zero;
    }
    #endregion
}
