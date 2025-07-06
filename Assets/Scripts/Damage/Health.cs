using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IReset
{
    #region Fields
    [Header("Max amount of health."), SerializeField, Range(1, 1000)] private uint _maxHealth = 100;
    [Header("Current amount of health."), SerializeField, Range(1, 1000)] private uint _currentHealth = 100;
    [Header("Event called when health changed."), SerializeField] private UnityEvent OnHealthChanged;
    #endregion

    #region Properties
    public Action OnDeathCallback { get; set; }
    public bool IsDead => _currentHealth == 0;
    #endregion

    #region Methods
    private void OnValidate()
    {
       if(_currentHealth > _maxHealth)
            _currentHealth = _maxHealth;
    }

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void CauseDamage(uint Damage)
    {
        if (IsDead)
            return;

        uint clampedDamage = (uint)Mathf.Clamp(Damage, 0, _currentHealth);
        uint leftHealth = _currentHealth - clampedDamage;
        _currentHealth = leftHealth;
        OnHealthChanged?.Invoke();

        if(leftHealth == 0)
        {
            OnDeathCallback?.Invoke();
            return;
        }
    }

    public void Reset() => _currentHealth = _maxHealth;
    #endregion
}
