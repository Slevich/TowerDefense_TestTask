using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MovementContainer : MonoBehaviour
{
    #region Fields
    [Header("Speed modifier of the movement."), SerializeField, Range(0f, 100f)]
    private float _speed = 1f;
    [Header("Link to movement realization script (IMovement)."), SerializeField]
    private MonoScript _movementRealization;
    #endregion

    #region Properties
    public Movement Controller { get; private set; }
    public float Speed => _speed;
    #endregion

    #region Methods
    private void OnValidate()
    {
        if(_movementRealization != null && !_movementRealization.GetClass().IsSubclassOf(typeof(Movement)))
        {
            Debug.Log("Wrong Movement realization!");
            _movementRealization = null;
        }
    }

    private void Awake()
    {
        if(_movementRealization != null && Controller == null)
        {
            Type controllerType = _movementRealization.GetClass();
            Controller = Activator.CreateInstance(controllerType) as Movement;
            Controller.SetPositionChangeModifier(_speed * Time.fixedDeltaTime);
        }
    }
    #endregion
}
