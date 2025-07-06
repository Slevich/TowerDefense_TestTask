using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileContainer : MonoBehaviour
{
    #region Fields
    [field: Header("Projectile movement container."), SerializeField] public MovementContainer Movement { get; set; }
    [field: Header("Projectile damage container."), SerializeField] public DamageDealer Damage { get; set; }
    [field: Header("Projectile parent transform"), SerializeField] public Transform Parent { get; set; }
    #endregion
}
