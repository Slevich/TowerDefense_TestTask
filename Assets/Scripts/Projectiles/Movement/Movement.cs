using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class Movement : IMovement
{
    #region Fields
    protected CancellationTokenSource _cancellationTokenSource;
    protected static readonly float _movementRefreshTime = 0.01f;
    protected float _positionChangeModifier = 1f;
    protected Vector3 _currentDirection = Vector3.zero;
    #endregion

    #region Properties
    public Action OnMovementStopped { get; set; }
    protected bool isMoving => _cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested;
    #endregion

    #region Methods
    public virtual void MoveToTarget (Transform Movable, Vector3 Target, Action OnMovementEnd = null) {}
    public virtual void MoveToTargetPorabolic (Transform Movable, Vector3 Target, float PorabolaHeight, Action OnMovementEnd = null) {}
    public virtual void MoveByDirection (Transform Movable, Vector3 Direction, Action OnMovementEnd = null) {}
    public virtual void MoveThroughPath (Transform Movable, Vector3[] Points) {}
    public virtual Vector3 GetCurrentMovementDirection () => _currentDirection;

    public virtual void StopMovement ()
    {
        if (!isMoving)
            return;

        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = null;
        _currentDirection = Vector3.zero;
        OnMovementStopped?.Invoke();
    }

    public virtual Vector3 CalculateFuturePosition (Transform Movable, float TimeInFuture, out float TimeToReach)
    {
        TimeToReach = 0f;
        return Movable.transform.position;
    }

    public virtual float GetSpeedPerSecond()
    {
        int timeStepsPerSecond = (int)Math.Round((1f / _movementRefreshTime));
        return _positionChangeModifier * timeStepsPerSecond;
    }

    public void SetPositionChangeModifier(float PositionChangeModifier) => _positionChangeModifier = PositionChangeModifier;
    #endregion
}
