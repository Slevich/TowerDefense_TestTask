using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TowerInstaller : MonoInstaller
{
    #region Fields
    [Header("Enemies detecting area."), SerializeField] private CircleArea _area;
    [Header("Bullets object pool."), SerializeField] private ObjectPool _bulletsPool;
    [Header("Enemies targeting."), SerializeField] private EnemyTargeting _targeting;
    [Header("Enemies detecting."), SerializeField] private EnemiesDetecting _enemiesDetecting;
    [Header("Shooter."), SerializeField] private Shooter _shooter;
    [Header("Rotation toward enemy."), SerializeField] private ObjectRotator _rotator;
    #endregion

    #region Methods
    public override void InstallBindings ()
    {
        this.Container
            .Bind<CircleArea>()
            .FromInstance(this._area)
            .AsSingle();

        this.Container 
            .Bind<ObjectPool>()
            .FromInstance(this._bulletsPool)
            .AsSingle();

        this.Container
            .Bind<EnemiesDetecting>()
            .FromInstance(this._enemiesDetecting)
            .AsSingle();

        this.Container
            .Bind<Shooter>()
            .FromInstance(this._shooter)
            .AsSingle();

        this.Container
            .Bind<EnemyTargeting>()
            .FromInstance(this._targeting)
            .AsSingle();

        this.Container
            .Bind<ObjectRotator>()
            .FromInstance(this._rotator)
            .AsSingle();
    }
    #endregion
}
