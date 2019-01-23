 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public static List<Projectile> Projectiles = new List<Projectile>();

	private Vector2 _direction = Vector2.zero;

	private float _lifeSpan = 0f;
	public float LifeSpan { get { return _lifeSpan; } }

	[SerializeField]
	private float _speed = 10f;
	[SerializeField]
	private float _radius = 0.1f;
	[SerializeField]
	[Range(0.01f, 10f)]
	private float _lifeTime = 1f;
	public float LifeTime { get { return _lifeTime; } }


	private void Start()
	{
		Projectiles.Add(this);
	}

	// Update is called once per frame
	void Update()
	{
		_lifeSpan += Time.deltaTime;
		transform.position += new Vector3(_direction.normalized.x, 0, _direction.normalized.y) * _speed * Time.deltaTime;
	}

	public void SetDirection(Vector2 dir)
	{
		_direction = dir;
		transform.Rotate(Vector3.up, -Mathf.Atan2(dir.y,dir.x)*180/Mathf.PI);
	}
}
