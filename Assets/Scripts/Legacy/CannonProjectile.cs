using UnityEngine;
using System.Collections;

public class CannonProjectile : MonoBehaviour 
{
    #region Properties
    [field: Header("Projectile's movement speed modifier."), SerializeField] public float Speed { get; set; } = 0.2f;
	//Урон вынести в отдельный класс.
	[field: Header("Amount of projectile damage."), SerializeField] public int Damage { get; set; } = 10;
    #endregion

    #region Methods
	//Движение переделать с указанием цели движения, вынести в отдельный асинхронный метод, который запускается в OnEnable и выключается в OnDisable.
    private void Update () {
		var translation = transform.forward * Speed;
		transform.Translate (translation);
	}

	private void OnEnable()
	{
		//При включении - логика движения, если еще не у цели.
	}

	private void OnDisable()
	{
		//При выключении - остановка движения, если до этого еще не достиг цели.
	}

	//Вынести логику коллайдинга в отдельный класс. Нанесение урона делегировать DamageManager.
	void OnTriggerEnter(Collider other) {
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
