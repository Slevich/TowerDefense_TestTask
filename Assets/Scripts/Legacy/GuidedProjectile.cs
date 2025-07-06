using UnityEngine;
using System.Collections;

public class GuidedProjectile : MonoBehaviour 
{
    #region Properties
    [field: Header("Target of the projectile movement."), SerializeField] public GameObject Target { get; set; }
	[field: Header("Speed modifier of the projectile's movement."), SerializeField] public float Speed { get; set; } = 0.2f;
	[field: Header("Amount of projectile damage."), SerializeField] public int Damage { get; set; } = 10;
    #endregion

    #region Methods
    //Движение переделать с указанием цели движения, вынести в отдельный асинхронный метод, который запускается в OnEnable и выключается в OnDisable.
    void Update () {
		if (Target == null) {
			Destroy (gameObject);
			return;
		}

		var translation = Target.transform.position - transform.position;
		if (translation.magnitude > Speed) {
			translation = translation.normalized * Speed;
		}
		transform.Translate (translation);
	}

    //Вынести логику коллайдинга в отдельный класс. Нанесение урона делегировать DamageManager.
    void OnTriggerEnter (Collider other) {
		var monster = other.gameObject.GetComponent<Monster> ();
		if (monster == null)
			return;

		monster.CurrentHealth -= Damage;
		if (monster.CurrentHealth <= 0) {
			Destroy (monster.gameObject);
		}
		Destroy (gameObject);
	}
    #endregion
}
