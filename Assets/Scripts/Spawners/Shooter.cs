using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;
using static UnityEngine.GraphicsBuffer;

public class Shooter : MonoBehaviour, ISpawner 
{
    #region Fields
    [Header("Time in seconds between spawns."), SerializeField, Range(0f, 10f)] private float _timeBetweenSpawns = 1f;
    [Header("Is porabolic trajectory?"), SerializeField] private bool _isPorabolicTrajectory = false;
    [Header("Porabola height."), SerializeField, Range(0f, 100f)] private float _porabolaHeight = 1f;
    private static readonly float _predictionLength = 1f;
    private ObjectPool _pool;
    private EnemiesDetecting _detection;
    private CancellationTokenSource _cancellationTokenSource;
    private bool _inProgress = false;
    #endregion

    #region Properties
    public GameObject CurrentTarget { get; set; }
    #endregion

    #region Methods
    [Inject]
    public void Construct (ObjectPool Pool, EnemiesDetecting Detection)
    {
        _pool = Pool;
        _detection = Detection;
    }

    public async void StartSpawn ()
    {
        if (_pool == null)
            return;

        if (_inProgress)
            return;

        _cancellationTokenSource = new CancellationTokenSource();
        _inProgress = true;

        while (_inProgress)
        {
            SpawnSingleBullet(CurrentTarget);

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

    private void SpawnSingleBullet (GameObject target)
    {
        GameObject bullet = _pool.GetObjectFromPool(out PoolObjectInfo info);
        bullet.transform.position = transform.position;
        MovementContainer bulletMovementContainer = info.MovementContainer;

        if (bulletMovementContainer != null)
        {
            Movement bulletMovement = bulletMovementContainer.Controller;
            DamageDealer damageDealer = info.DamageDealer;
            MovementContainer targetMovementContainer = (MovementContainer)ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(target, typeof(MovementContainer));
            Vector3 bulletTarget = target.transform.position;

            if (targetMovementContainer != null)
            {
                //Vector3 predictedTarget = target.transform.position;
                //float distance = Vector3.Distance(bullet.transform.position, predictedTarget);
                //float projectileSpeed = bulletMovement.GetSpeedPerSecond(); // примерная скорость снаряда
                //float timeToReach = distance / projectileSpeed;
                //// Предсказываем положение цели через timeToReach секунд
                //Vector3 targetVelocity = targetMovementContainer.Controller.GetCurrentMovementDirection();
                //predictedTarget += targetVelocity * timeToReach;
                //bulletTarget = predictedTarget;
                Movement targetMovement = targetMovementContainer.Controller;
                float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
                float bulletSpeedPerSecond = bulletMovement.GetSpeedPerSecond();
                float bulletTimeToTarget = distanceToTarget / bulletSpeedPerSecond;

                float targetSpeedPerSecond = targetMovement.GetSpeedPerSecond();
                float speedDifferences = Mathf.Abs(targetSpeedPerSecond / bulletSpeedPerSecond);
                float predictionLength = _predictionLength * speedDifferences * distanceToTarget;

                bulletTarget = targetMovement.CalculateFuturePosition(target.transform, predictionLength, out float Time);

            }

            if (bulletMovement != null)
            {
                if(_isPorabolicTrajectory)
                {
                    bulletMovement.MoveToTargetPorabolic(bullet.transform, bulletTarget, _porabolaHeight, delegate { ExecuteDamageOnDealer(damageDealer, bulletTarget); });
                }
                else
                {
                    bulletMovement.MoveToTarget(bullet.transform, bulletTarget, delegate { ExecuteDamageOnDealer(damageDealer, bulletTarget); });
                }
            }
        }
    }

    private void ExecuteDamageOnDealer(DamageDealer dealer, Vector3 damagePosition)
    {
        if (dealer != null)
        {
            dealer.DamagePosition = damagePosition;
            dealer.ExecuteDamage();
        }
    }

    public void StopSpawn ()
    {
        if (!_inProgress)
            return;

        if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
        {
            _cancellationTokenSource.Cancel();
        }

        _inProgress = false;
        CurrentTarget = null;
    }

    private void OnDisable () => StopSpawn();
    #endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(Shooter))]
public class ShooterEditor : Editor
{
    private static readonly string _timeBetweenSpawnsPropertyName = "_timeBetweenSpawns";
    private static readonly string _isPorabolicPropertyName = "_isPorabolicTrajectory";
    private static readonly string _porabolaHeightPropertyName = "_porabolaHeight";

    private SerializedProperty _timeBetweenSpawnsProperty;
    private SerializedProperty _isPorabolicProperty;
    private SerializedProperty _porabolaHeightProperty;

    private void OnEnable()
    {
        _timeBetweenSpawnsProperty = serializedObject.FindProperty(_timeBetweenSpawnsPropertyName);
        _isPorabolicProperty = serializedObject.FindProperty(_isPorabolicPropertyName);
        _porabolaHeightProperty = serializedObject.FindProperty(_porabolaHeightPropertyName);
    }

    public override void OnInspectorGUI ()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_timeBetweenSpawnsProperty);

        EditorGUILayout.PropertyField(_isPorabolicProperty);
        bool isPorabolic = _isPorabolicProperty.boolValue;

        if (isPorabolic)
        {
            EditorGUILayout.PropertyField(_porabolaHeightProperty);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif