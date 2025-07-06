using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using static UnityEngine.GraphicsBuffer;

public class DirectiveMovement : Movement
{
    #region Methods
    public override async void MoveByDirection (Transform Movable, Vector3 Direction, Action OnMovementEnd = null)
    {
        if (isMoving)
            return;

        _cancellationTokenSource = new CancellationTokenSource();

        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            _currentDirection = Direction.normalized;
            Movable.position += Direction * _positionChangeModifier;

            await UniTask.Delay(delayTimeSpan: TimeSpan.FromSeconds(_movementRefreshTime),
                                cancellationToken: _cancellationTokenSource.Token);
        }

        if (OnMovementEnd != null)
            OnMovementEnd?.Invoke();

        StopMovement();
    }
    #endregion
}
