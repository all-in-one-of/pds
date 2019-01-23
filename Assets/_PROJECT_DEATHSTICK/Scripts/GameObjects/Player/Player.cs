using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	/**********************/
	/* VISIBLE PROPERTIES */
	/**********************/

	//Basic Requirements
	[Header("Requirements")]
	[SerializeField]
	private GameObject _renderer;
	[SerializeField]
	private Animator _renderAnimator;

	//Configuration
	[Header("Configuration")]
	[SerializeField]
	private bool _usesJoypad = true;

	//Life properties
	[Header("Life properties")]
	[SerializeField]
	private int _maxLife = 5;
	[SerializeField]
	private int _life = 5;
	public int Life { get { return _life; } }

	//Movement properties
	[Header("Movement properties")]
	[SerializeField]
	private float _moveSpeed = 7.0f;

	[Header("Dodge properties")]
	[SerializeField]
	private float _dodgeSpeed = 15.0f;

	[Header("Attack properties")]
	[SerializeField]
	private int _damage = 1;

	[Header("Fire properties")]
	[SerializeField]
	private float _FireRate = 7.0f;

	//Shoot properties
	[Header("Fire properties")]
	[SerializeField]
	private GameObject _projectile;

	/**************************/
	/* NON VISIBLE PROPERTIES */
	/**************************/

	//Player Rotation
	private Vector2 _currentDirection;


	//Player States
	private bool _isMoving = true;
	private bool _isDodging = false;
	private bool _isAttacking = false;
	private bool _isFiring = false;

	private bool _lockMove = false;
	private bool _lockDodge = false;
	private bool _lockAttack = false;
	private bool _lockFire = false;

	//Player Components
	private Rigidbody rb;

	/*****************/
	/*    METHODS    */
	/*****************/

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	//Called once per frame
	private void Update () {

		SetDirectionVector();

		_renderer.transform.LookAt(_renderer.transform.position + new Vector3(_currentDirection.x, 0, _currentDirection.y));

		Move();

		if (_isDodging && !_lockDodge) StartCoroutine(Dodge());
		if (_isAttacking && !_lockAttack) StartCoroutine(Attack());
		if (_isFiring && !_lockFire) StartCoroutine(Fire());

	}

	private void SetDirectionVector()
	{
		Vector2 direction = new Vector2();

		Vector2 rightStick = new Vector2(T_Inputs.GetJoystickAxis(JOYSTICK.R, AXIS.X), T_Inputs.GetJoystickAxis(JOYSTICK.R, AXIS.Y));
		Vector2 leftStick = new Vector2(T_Inputs.GetJoystickAxis(JOYSTICK.L, AXIS.X), T_Inputs.GetJoystickAxis(JOYSTICK.L, AXIS.Y));

		if (rightStick.x != 0 || rightStick.y != 0)
		{
			direction = rightStick;
		}
		else if (leftStick.x != 0 || leftStick.y != 0)
		{
			direction = leftStick;
		}
		
		if(direction != Vector2.zero) _currentDirection = direction;
	}

	private void Move()
	{
		Vector3 newPos = transform.position;

		if (_usesJoypad)
		{
			float jlx = T_Inputs.GetJoystickAxis(JOYSTICK.L, AXIS.X);
			float jly = T_Inputs.GetJoystickAxis(JOYSTICK.L, AXIS.Y);

			float jrx = T_Inputs.GetJoystickAxis(JOYSTICK.R, AXIS.X);
			float jry = T_Inputs.GetJoystickAxis(JOYSTICK.R, AXIS.Y);

			Vector2 jl = new Vector2(jlx, jly);
			Vector2 jr = new Vector2(jrx, jry);
			Vector2 jrRight = new Vector2(jry, -jrx);


			if (jlx != 0 || jly != 0)
			{

				newPos += new Vector3(
					jl.x * _moveSpeed * Time.deltaTime,
					0,
					jl.y * _moveSpeed * Time.deltaTime
				);

				if (jrx == 0 && jry == 0) //pas d'input stick droit
				{
					_renderAnimator.SetFloat("Sideways", 0);
					_renderAnimator.SetFloat("Straight", Mathf.Sqrt((jlx * jlx) + (jly * jly)));
				}
				else //input stick droit
				{
					_renderAnimator.SetFloat("Sideways", Vector2.Dot(jrRight, jl));
					_renderAnimator.SetFloat("Straight", Vector2.Dot(jr, jl));
				}

				rb.MovePosition(newPos);
			}
			else
			{
				_renderAnimator.SetFloat("Sideways", 0);
				_renderAnimator.SetFloat("Straight", 0);
			}
		}
		else
		{
			if (Input.GetKey(KeyCode.Z)) newPos += new Vector3(0, 0, _moveSpeed * Time.deltaTime);
			if (Input.GetKey(KeyCode.S)) newPos += new Vector3(0, 0, -_moveSpeed * Time.deltaTime);
			if (Input.GetKey(KeyCode.D)) newPos += new Vector3(_moveSpeed * Time.deltaTime, 0, 0);
			if (Input.GetKey(KeyCode.Q)) newPos += new Vector3(-_moveSpeed * Time.deltaTime, 0, 0);

			rb.MovePosition(newPos);

		}

	}

	private IEnumerator Dodge()
	{
		_isDodging = false;
		yield return null;
	}

	private IEnumerator Attack()
	{
		_isAttacking = false;
		yield return null;
	}

	private IEnumerator Fire()
	{
		GameObject proj = Instantiate(_projectile, transform.position + new Vector3(_currentDirection.x, 0, _currentDirection.y), Quaternion.identity);
		proj.GetComponent<Projectile>().SetDirection(_currentDirection.normalized);

		_isFiring = false;
		yield return null;
	}

}
