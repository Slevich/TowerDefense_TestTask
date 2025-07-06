using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading;
using Zenject;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using System;

public class SpawnObjectOnMovingPath : MonoBehaviour, ISpawner
{
    #region Fields
    [Header("Time in seconds between spawns."), SerializeField, Range(0f, 60f)] private float _timeBetweenSpawns = 1f;
    private ObjectPool _pool;
    private bool _inProgress;
    private CancellationTokenSource _cancellationTokenSource;
    private LineRenderer _lineRenderer;
    private Vector3[] _positions = new Vector3[] { };
    #endregion

    #region Methods
    [Inject]
    public void Construct(ObjectPool Pool, LineRenderer Renderer)
    {
        _pool = Pool;
        _lineRenderer = Renderer;
    }

    public async void StartSpawn()
    {
        if (_pool == null)
            return;

        if(_inProgress)
            return;

        _inProgress = true;
        _cancellationTokenSource = new CancellationTokenSource();
        if (_positions.Length == 0)
        {
            int positionsCount = _lineRenderer.positionCount;
            List<Vector3> positions = new List<Vector3>();

            for (int i = 0; i < positionsCount; i++)
                positions.Add(_lineRenderer.GetPosition(i));

            _positions = positions.ToArray();
        }

        while (_inProgress)
        {
            GameObject poolObject = _pool.GetObjectFromPool(out PoolObjectInfo info);
            MovementContainer movementContainer = info.MovementContainer;
            Movement movement = movementContainer.Controller;
            movement.MoveThroughPath(poolObject.transform, _positions);

            try
            {
                await UniTask.Delay(delayTimeSpan: TimeSpan.FromSeconds(_timeBetweenSpawns), cancellationToken: _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException exception)
            {
                break;
            }
        }
    }

    public void StopSpawn()
    {
        if (!_inProgress)
            return;

        if(_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
        {
            _cancellationTokenSource.Cancel();
        }

        _inProgress = false;
    }

    private void OnEnable () => StartSpawn();

    private void OnDisable() => StopSpawn();
    #endregion
}