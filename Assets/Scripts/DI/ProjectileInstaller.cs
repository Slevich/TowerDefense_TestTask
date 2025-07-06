using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ProjectileInstaller : MonoInstaller
{
    #region Fields
    [Header("Projectile movement container."), SerializeField] private MovementContainer _movement;
    [Header("Projectile damage container."), SerializeField] private DamageDealer _damage;
    [Header("Projectile damage area."), SerializeField] private CircleArea _circleArea;
    #endregion

    #region Methods
    public override void InstallBindings ()
    {
        this.Container
            .Bind<MovementContainer>()
            .FromInstance(this._movement)
            .AsSingle();

        this.Container
            .Bind<DamageDealer>()
            .FromInstance(this._damage)
            .AsSingle();

        this.Container
            .Bind<CircleArea>()
            .FromInstance(this._circleArea)
            .AsSingle();
    }
    #endregion
}
