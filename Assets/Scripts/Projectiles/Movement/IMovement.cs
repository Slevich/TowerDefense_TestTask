using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovement
{
    Action OnMovementStopped { get; set; }

    void MoveToTarget (Transform Movable, Vector3 Target, Action OnMovementEnd = null);
    void MoveToTargetPorabolic (Transform Movable, Vector3 Target, float PorabolaHeight, Action OnMovementEnd = null);
    void MoveByDirection (Transform Movable, Vector3 Direction, Action OnMovementEnd = null);
    void MoveThroughPath (Transform Movable, Vector3[] Points);
    Vector3 CalculateFuturePosition (Transform Movable, float TimeInFuture, out float TimeToReach);
    float GetSpeedPerSecond ();
    Vector3 GetCurrentMovementDirection ();
    void StopMovement ();
}
