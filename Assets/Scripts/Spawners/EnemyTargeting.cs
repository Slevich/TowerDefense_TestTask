using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class EnemyTargeting : MonoBehaviour
{
    #region Fields
    private bool _shooterReady = false;
    private bool _isTargeting = false;
    private float _targetingUpdateTime = 0.05f;
    private EnemiesDetecting _detecting;
    private Shooter _shooter;
    private ObjectRotator _rotator;
    private GameObject _currentTarget;
    private CancellationTokenSource _cancellationTokenSource;
    #endregion

    #region Methods
    [Inject]
    public void Construct(EnemiesDetecting Detecting, Shooter Shooter, ObjectRotator Rotator)
    {
        _detecting = Detecting;
        _shooter = Shooter;
        _rotator = Rotator;
        _detecting.OnEnemyDetectionCallback += (target) => ManageTargeting(target);

        if (_rotator != null)
            _rotator.TargetRotationReached += (reached) => _shooterReady = reached;
    }

    private void ManageTargeting(GameObject CurrentTarget)
    {
        if(CurrentTarget != null)
        {
            if(CurrentTarget != _currentTarget)
            {
                StopTargeting();
                StartTargeting(CurrentTarget);
            }
        }
        else
        {
            StopTargeting();
        }

        _currentTarget = CurrentTarget;
    }

    private async void StartTargeting(GameObject Target)
    {
        if (_isTargeting)
            return;

        _isTargeting = true;
        _cancellationTokenSource = new CancellationTokenSource();

        while (_isTargeting)
        {
            if (_rotator != null)
            {
                _rotator.StartRotation(Target.transform);

                try
                {
                    await UniTask.WaitUntil(() => _shooterReady, PlayerLoopTiming.FixedUpdate, cancellationToken: _cancellationTokenSource.Token);
                }
                catch (OperationCanceledException exception)
                {
                    break;
                }
            }

            _shooter.CurrentTarget = Target;
            _shooter.StartSpawn();

            try
            {
                await UniTask.WaitUntil(() => !_shooterReady, PlayerLoopTiming.FixedUpdate, cancellationToken: _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException exception)
            {
                break;
            }
        }
    }

    private void StopTargeting()
    {
        if (!_isTargeting)
            return;

        if(_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            _cancellationTokenSource.Cancel();

        _isTargeting = false;
        _currentTarget = null;
        _shooterReady = false;

        if(_rotator != null)
            _rotator.StopRotation();

        _shooter.StopSpawn();
    }

    private void OnDisable() => StopTargeting();
    #endregion
}
