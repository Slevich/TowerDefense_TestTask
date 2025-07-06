using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour 
{
    #region Properties
    [field: Header("Time in seconds between shots."), SerializeField] public float m_interval = 3;
    [field: Header("Time in seconds between shots."), SerializeField] public GameObject m_moveTarget;
    #endregion

    #region Fields
    private float lastSpawn = -1;
	#endregion

	#region Methods
	//Убрать апдейт. Сделать асинхронный метод. Сделать, чтобы заспауненный объекты добавлялись в пул.
	void Update ()
	{
		if (Time.time > lastSpawn + m_interval)
		{
			var newMonster = GameObject.CreatePrimitive(PrimitiveType.Capsule);
			var r = newMonster.AddComponent<Rigidbody>();
			r.useGravity = false;
			newMonster.transform.position = transform.position;
			var monsterBeh = newMonster.AddComponent<Monster>();
			monsterBeh.MovementTarget = m_moveTarget;

			lastSpawn = Time.time;
		}
	}
    #endregion
}
