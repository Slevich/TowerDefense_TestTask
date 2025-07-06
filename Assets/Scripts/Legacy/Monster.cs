using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour 
{
    #region Properties
    [field: Header("Target transform of the movement."), SerializeField] public GameObject MovementTarget { get; set; }
	[field: Header("Speed modifier of the movement."), SerializeField] public float Speed { get; set; } = 0.1f;
	[field: Header("Max amount of health."), SerializeField] public int MaxHealth { get; set; } = 30;
	[field: Header("Current amount of health."), SerializeField] public int CurrentHealth { get; set; } = 0;
    #endregion

    #region Fields
    private const float reachDistance = 0.3f;
    #endregion

    #region Methods
    void Start () {
		CurrentHealth = MaxHealth;
	}

	//Убрать апдейт! Вынести логику движения в отдельный метод. Или даже в отдельный класс.
	void Update () {
		if (MovementTarget == null)
			return;
		
		if (Vector3.Distance (transform.position, MovementTarget.transform.position) <= reachDistance) {
			Destroy (gameObject);
			return;
		}

		var translation = MovementTarget.transform.position - transform.position;
		if (translation.magnitude > Speed) {
			translation = translation.normalized * Speed;
		}
		transform.Translate (translation);
	}
    #endregion
}
