using UnityEngine;
using System.Collections;

public class SimpleTower : MonoBehaviour 
{
    #region Properties
    [field: Header("Time in seconds between shots."), SerializeField] public float ShootInterval { get; set; } = 0.5f;
    [field: Header("Max distance to enemy."), SerializeField] public float ShootRange { get; set; } = 4f;
    [field: Header("Target transform of the movement."), SerializeField] public GameObject ProjectilePrefab { get; set; }
    #endregion

    #region Fields
    private float lastShotTime = -0.5f;
    #endregion

    #region Methods
    /*Убрать апдейт. Вынести из апдейта метод по детекту противника. Сделать отдельный объект - сферу детектинга, в которой настраивается радиус.
	* Вынести спавн снаряда в отдельный метод.
	*/
    void Update () {
		if (ProjectilePrefab == null)
			return;

		foreach (var monster in FindObjectsOfType<Monster>()) {
			if (Vector3.Distance (transform.position, monster.transform.position) > ShootRange)
				continue;

			if (lastShotTime + ShootInterval > Time.time)
				continue;

			// shot
			var projectile = Instantiate(ProjectilePrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity) as GameObject;
			var projectileBeh = projectile.GetComponent<GuidedProjectile> ();
			projectileBeh.Target = monster.gameObject;

			lastShotTime = Time.time;
		}
	}
    #endregion
}
