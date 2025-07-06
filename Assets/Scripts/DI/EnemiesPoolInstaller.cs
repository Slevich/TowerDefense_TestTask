using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemiesPoolInstaller : MonoInstaller
{
    #region Fields
    [Header("Enemies line renderer path."), SerializeField] private LineRenderer _lineRenderer;
    [Header("Enemies pool."), SerializeField] private ObjectPool _pool;
    #endregion

    #region Methods
    public override void InstallBindings ()
    {
        this.Container
            .Bind<LineRenderer>()
            .FromInstance(this._lineRenderer)
            .AsSingle();

        this.Container
            .Bind<ObjectPool>()
            .FromInstance(this._pool)
            .AsSingle();
    }

    //public void Rebind()
    //{
    //    this.Container
    //        .Rebind<LineRenderer>()
    //        .FromInstance(this._lineRenderer)
    //        .AsSingle();

    //    this.Container
    //        .Rebind<ObjectPool>()
    //        .FromInstance(this._pool)
    //        .AsSingle();
    //}
    #endregion
}
