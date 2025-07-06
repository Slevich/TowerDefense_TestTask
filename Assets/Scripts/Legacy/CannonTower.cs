using UnityEngine;
using System.Collections;

public class CannonTower : MonoBehaviour 
{
    #region Properties
    [field: Header("Time in seconds between shots."), SerializeField] public float ShootInterval { get; set; } = 0.5f;
	[field: Header("Radius of the enemy detection."), SerializeField] public float Range { get; set; } = 4f;
	[field: Header("Prefab of the projectile."), SerializeField] public GameObject ProjectilePrefab { get; set; }
	[field: Header("Origin of the projectile movement while shot."), SerializeField] public Transform ShootPoint { get; set; }
    #endregion

    #region Fields
    private float _lastShotTime = -0.5f;
    #endregion

    #region Methods
    /*Убрать апдейт. Вынести из апдейта метод по детекту противника. Сделать отдельный объект - сферу детектинга, в которой настраивается радиус.
	 * Вынести спавн снаряда в отдельный метод.
	 */
    void Update () {
		if (ProjectilePrefab == null || ShootPoint == null)
			return;

		foreach (var monster in FindObjectsOfType<Monster>()) {
			if (Vector3.Distance (transform.position, monster.transform.position) > Range)
				continue;

			if (_lastShotTime + ShootInterval > Time.time)
				continue;

			// shot
			Instantiate(ProjectilePrefab, ShootPoint.position, ShootPoint.rotation);

			_lastShotTime = Time.time;
		}
	}
    #endregion
}
