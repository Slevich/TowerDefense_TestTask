using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.WSA;

public class TargetedMovement : Movement
{
    #region Fields
    protected static readonly float _targetReachDistance = 0.1f;
    private static readonly float _gravity = -9.81f;
    #endregion

    #region Methods
    public override async void MoveToTarget (Transform Movable, Vector3 Target, Action OnMovementEnd = null)
    {
        if (isMoving)
            return;

        _cancellationTokenSource = new CancellationTokenSource();

        while (isMoving)
        {
            if (!Movable.gameObject.activeInHierarchy)
            {
                break;
            }

            Vector3 direction = (Target - Movable.transform.position).normalized;
            _currentDirection = direction;
            Movable.position += direction * _positionChangeModifier;

            if (Vector3.Distance(Movable.position, Target) <= _targetReachDistance)
            {
                break;
            }

            await UniTask.Delay(delayTimeSpan: TimeSpan.FromSeconds(_movementRefreshTime),
                                cancellationToken: _cancellationTokenSource.Token);
        }

        if (OnMovementEnd != null)
            OnMovementEnd?.Invoke();

        StopMovement();
        Movable.position = Target;
    }

    public override async void MoveToTargetPorabolic (Transform Movable, Vector3 Target, float PorabolaHeight, Action OnMovementEnd = null)
    {
        if (isMoving) 
            return;

        _cancellationTokenSource = new CancellationTokenSource();
        Vector3 startPosition = Movable.position;
        float totalDistance = Vector3.Distance(startPosition, Target);
        float duration = totalDistance / GetSpeedPerSecond();
        float elapsedTime = 0f;

        while (elapsedTime < duration && isMoving)
        {
            if (!Movable.gameObject.activeInHierarchy)
                break;

            elapsedTime += _movementRefreshTime;
            float clampedTime = Mathf.Clamp01(elapsedTime / duration);
            float height = CalculateParabolaHeight(clampedTime, PorabolaHeight);
            Vector3 horizontalPosition = Vector3.Lerp(startPosition, Target, clampedTime);
            Vector3 newPorabolicPosition = horizontalPosition + Vector3.up * height;
            _currentDirection = (newPorabolicPosition - Movable.position).normalized;
            Movable.position = newPorabolicPosition;

            if (Vector3.Distance(Movable.position, Target) <= _targetReachDistance)
            {
                break;
            }

            await UniTask.Delay(
                TimeSpan.FromSeconds(_movementRefreshTime),
                cancellationToken: _cancellationTokenSource.Token);
        }

        if (OnMovementEnd != null)
            OnMovementEnd?.Invoke();

        StopMovement();
        Movable.position = Target;
    }

    private float CalculateParabolaHeight (float t, float maxHeight)
    {
        return -4 * maxHeight * t * (t - 1);
    }
    #endregion
}
