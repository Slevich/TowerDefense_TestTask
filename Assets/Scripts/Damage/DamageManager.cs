using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class DamageManager : MonoBehaviour
{
    #region Fields
    private Queue<DamageData> _damageDatas = new Queue<DamageData>();

    private bool _isUpdating = false;
    private static readonly float _damageUpdate = 0.25f;
    private static readonly int _datasAmountPerIteration = 20;
    private CancellationTokenSource _cancellationTokenSource;
    private static DamageManager _instance;
    #endregion

    #region Properties
    public static DamageManager Instance
    {
        get 
        {
            if(_instance == null)
                _instance = (DamageManager)FindObjectOfType(typeof(DamageManager));

            return _instance; 
        }
    }
    #endregion

    #region Methods
    public async void ExecuteDamagesData()
    {
        if (_isUpdating)
            return;

        _isUpdating = true;

        if(_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
            _cancellationTokenSource = new CancellationTokenSource();

        while(!_cancellationTokenSource.IsCancellationRequested)
        {
            for(int i = 0; i <= _datasAmountPerIteration && i < _damageDatas.Count && _damageDatas.Count > 0; i++)
            {
                DamageData data = _damageDatas.Dequeue();
                uint damageAmount = data.AmountOfDamage;
                Health damagedHealth = data.ReceiverHealth;
                damagedHealth.CauseDamage(damageAmount);
            }

            if(_damageDatas.Count == 0)
            {
                StopExecuting();
                break;
            }

            await UniTask.Delay(delayTimeSpan: TimeSpan.FromSeconds(_damageUpdate), cancellationToken: _cancellationTokenSource.Token);
        }

        _isUpdating = false;
    }

    private void StopExecuting()
    {
        if (_cancellationTokenSource == null && !_cancellationTokenSource.IsCancellationRequested)
        {
            _cancellationTokenSource.Cancel();
        }
    }

    public void AddData(DamageData Data)
    {
        _damageDatas.Enqueue(Data);

        if (_damageDatas.Count > 0 && _isUpdating == false)
            ExecuteDamagesData();
    }
    #endregion
}

public class DamageData
{
    #region Properties
    public Health ReceiverHealth { get; private set; }
    public uint AmountOfDamage { get; private set; } = 0;
    #endregion

    #region Constuctor
    public DamageData(Health DamagedHealth, uint DamageAmount)
    {
        ReceiverHealth = DamagedHealth;
        AmountOfDamage = DamageAmount;
    }
    #endregion
}