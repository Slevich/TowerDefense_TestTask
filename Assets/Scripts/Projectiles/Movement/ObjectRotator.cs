using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.TextCore;

public class ObjectRotator : MonoBehaviour
{
    #region Fields
    [Header("Speed modifier of the rotation."), SerializeField, Range(0f, 100f)] private float _rotationSpeed = 5f;
    private bool _isRotating = false;
    private static readonly float _rotationUpdateTime = 0.01f;
    private CancellationTokenSource _cancellationTokenSource;
    private float _rotationDifference = 3f;
    #endregion

    #region Properties
    public Action<bool> TargetRotationReached { get; set; }
    #endregion

    #region Methods
    public async void StartRotation(Transform Target)
    {
        if(Target == null)
            return;

        if (_isRotating)
            return;

        _isRotating = true;
        _cancellationTokenSource = new CancellationTokenSource();

        while (_isRotating)
        {
            Vector3 target = new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z);
            Vector3 targetDirection = target - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(targetDirection);
            Quaternion currentRotation = transform.rotation;

            if (Quaternion.Angle(currentRotation, lookRotation) > _rotationDifference)
            {
                transform.rotation = Quaternion.Lerp(currentRotation, lookRotation, Time.fixedDeltaTime * _rotationSpeed);
                TargetRotationReached?.Invoke(false);
            }
            else
            {
                TargetRotationReached?.Invoke(true);
            }

            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_rotationUpdateTime), cancellationToken: _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException exception)
            {
                break;
            }
        }
    }

    public void StopRotation()
    {
        if (!_isRotating)
            return;

        if(_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            _cancellationTokenSource.Cancel();

        TargetRotationReached?.Invoke(false);
        _isRotating = false;
    }

    private void OnDisable() => StopRotation();
    #endregion
}
