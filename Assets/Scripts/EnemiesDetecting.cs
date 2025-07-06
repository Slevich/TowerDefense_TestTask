using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Zenject;
using Cysharp.Threading;
using System.Linq;
using Cysharp.Threading.Tasks;
using System;

[RequireComponent(typeof(CircleArea))]
public class EnemiesDetecting : MonoBehaviour
{
    #region Fields
    [SerializeField] private ObjectPool _enemiesPool;
    private GameObject _lastDetectedEnemy;
    private CircleArea _detectingArea;
    private CancellationTokenSource _cancellationTokenSource;
    private bool _detecting = false;
    private static readonly float _detectingUpdateTime = 0.1f;
    #endregion

    #region Properties
    public Action<GameObject> OnEnemyDetectionCallback { get; set; } = delegate { };
    public ObjectPool EnemiesPool => _enemiesPool;
    #endregion

    #region Methods
    [Inject]
    public void Construct(CircleArea DetectingArea)
    {
        _detectingArea = DetectingArea;
    }

    public async void StartDetecting()
    {
        if(_enemiesPool == null)
            return;

        if (_detectingArea == null)
            return;

        if (_detecting) 
            return;

        _cancellationTokenSource = new CancellationTokenSource();
        _detecting = true;

        while (_detecting || !_cancellationTokenSource.IsCancellationRequested)
        {
            List<GameObject> enemies = _enemiesPool.ActiveObjects;
            IEnumerable<GameObject> enemiesInRange = enemies.Where(enemy => _detectingArea.PointIsInArea(enemy.transform.position, out float centerAppcroaching));
            GameObject nearestEnemy = null;

            if (enemiesInRange.Count() > 0)
            {
                IEnumerable<GameObject> orderedEnemies = enemiesInRange.OrderBy(enemy => Vector3.Distance(enemy.transform.position, transform.position));
                nearestEnemy = orderedEnemies.FirstOrDefault();
            }

            if(_lastDetectedEnemy != nearestEnemy)
            {
                OnEnemyDetectionCallback?.Invoke(nearestEnemy);
            }

            _lastDetectedEnemy = nearestEnemy;

            try
            {
                await UniTask.Delay(delayTimeSpan: TimeSpan.FromSeconds(_detectingUpdateTime), cancellationToken: _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException exception) 
            {
                break;
            }
        }
    }

    public void StopDetecting()
    {
        if (!_detecting) 
            return;

        _detecting = false;
        _lastDetectedEnemy = null;
        OnEnemyDetectionCallback?.Invoke(_lastDetectedEnemy);

        if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            _cancellationTokenSource.Cancel();
    }

    private void OnEnable() => StartDetecting();
    private void OnDisable() => StopDetecting();
    #endregion
}
