using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    #region Fields
    [Header("Damage manager."), SerializeField] private DamageManager _damageManager;
    [Header("Enemies obkect pool."), SerializeField] private ObjectPool _enemiesPool;
    #endregion

    #region Methods
    public override void InstallBindings ()
    {
        this.Container
            .Bind<DamageManager>()
            .FromInstance(this._damageManager)
            .AsSingle();

        this.Container
            .Bind<ObjectPool>()
            .FromInstance (this._enemiesPool)
            .AsSingle();
    }
    #endregion
}
