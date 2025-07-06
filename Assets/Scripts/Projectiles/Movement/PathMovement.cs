using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System.Linq;
using System.Reflection;

public class PathMovement : Movement
{
    #region Fields
    protected static readonly float _targetReachDistance = 0.05f;
    private Vector3[] _currentPathPoints = new Vector3[] { };
    private int _targetIndex = -1;
    private float _currentSpeed = 0f;
    #endregion

    #region Methods
    public override async void MoveThroughPath (Transform Movable, Vector3[] Points)
    {
        if (isMoving)
            return;

        _cancellationTokenSource = new CancellationTokenSource();
        Movable.transform.position = Points.FirstOrDefault();
        _currentPathPoints = Points;
        bool canceled = false;

        for (int i = 1; i < Points.Length; i++)
        {
            _targetIndex = i;
            Vector3 target = Points[_targetIndex];

            while (Vector3.Distance(Movable.position, target) >= _targetReachDistance && !canceled)
            {
                if (!Movable.gameObject.activeInHierarchy)
                {
                    canceled = true;
                    //Debug.Log("Объект неактивен!");
                    break;
                }

                Vector3 direction = (target - Movable.transform.position).normalized;
                _currentDirection = direction;
                _currentSpeed = _positionChangeModifier;
                Movable.position += direction * _positionChangeModifier;

                try
                {
                    await UniTask.Delay(delayTimeSpan: TimeSpan.FromSeconds(_movementRefreshTime),
                                        cancellationToken: _cancellationTokenSource.Token);
                }
                catch (OperationCanceledException exception)
                {
                    canceled = true;
                }
            }

            if (canceled)
                break;
        }

        StopMovement();
        _currentPathPoints = new Vector3[] { };
        _targetIndex = -1;
    }

    public override Vector3 CalculateFuturePosition (Transform Movable, float PredictionLength, out float TimeToReach)
    {
        if(!isMoving || _targetIndex < 0)
        {
            TimeToReach = 0;
            return Movable.transform.position;
        }

        //Debug.Log("Расчет!");
        float leftLength = PredictionLength;
        float totalLength = 0;
        Vector3 anchor = Movable.transform.position;
        Vector3 currentTarget = _currentPathPoints[_targetIndex];
        Vector3 predictedPosition = Movable.transform.position;
        Vector3 currentDirection = Vector3.zero;
        float lengthToCurrentPoint = 1f;

        for (int i = _targetIndex; i < _currentPathPoints.Length; i++)
        {
            currentTarget = _currentPathPoints[i];
            currentDirection = currentTarget - anchor;
            lengthToCurrentPoint = currentDirection.magnitude;
            
            if(lengthToCurrentPoint >= leftLength)
            {
                totalLength += leftLength;
                break; 
            }
            else
            {
                leftLength -= lengthToCurrentPoint;
                totalLength += lengthToCurrentPoint;
                anchor = currentTarget;
            }
        }

        float speed = GetSpeedPerSecond();
        TimeToReach = totalLength / speed;

        return anchor += (currentDirection * (leftLength / lengthToCurrentPoint));
    }
    #endregion
}
