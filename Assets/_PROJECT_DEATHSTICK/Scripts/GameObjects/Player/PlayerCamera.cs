using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

	[SerializeField]
	private float _distance = 10.0f;

	[SerializeField]
	private float _damper = 0.01f;

	[SerializeField]
	private float _leftStickRatio = 1f;

	[SerializeField]
	private float _rightStickRatio = 1f;

	[SerializeField]
	[Range(0,90)]
	private float _angle = 45.0f;
	private float _angleRad;

	private Vector3 _lastCameraTarget;

	[HideInInspector]
	public Vector3 CameraTarget;
	
	// Update is called once per frame
	private void LateUpdate ()
	{
		//Angle application
		_angleRad = _angle * Mathf.PI / 180;

		//Camera position on the arc above the player + distance
		transform.position = Vector3.Lerp(transform.position, CameraTarget + new Vector3(0, Mathf.Sin(_angleRad), -Mathf.Cos(_angleRad)) * _distance, _damper);

		//Camera rotation based on the angle chosen
		transform.rotation = Quaternion.Euler(_angle, 0, 0);

		//Camera Target assignation
		_lastCameraTarget = CameraTarget;
		
	}

	public void ResetCameraPos()
	{
		_angleRad = _angle * Mathf.PI / 180;
		transform.position = CameraTarget + new Vector3(0, Mathf.Sin(_angleRad), -Mathf.Cos(_angleRad)) * _distance;
	}

	/// <summary>
	/// Returns a value between -stickRatio and +stickRatio in both x and y axis
	/// </summary>
	/// <returns></returns>
	public Vector3 GetCameraTargetOffset()
	{
		float limit = .2f;

		Vector3 _camoffset = new Vector3();

		Vector2 rightStick = new Vector2(T_Inputs.GetJoystickAxis(JOYSTICK.R, AXIS.X), T_Inputs.GetJoystickAxis(JOYSTICK.R, AXIS.Y));
		Vector2 leftStick = new Vector2(T_Inputs.GetJoystickAxis(JOYSTICK.L, AXIS.X), T_Inputs.GetJoystickAxis(JOYSTICK.L, AXIS.Y));

		if (rightStick.x > limit || rightStick.x < -limit || rightStick.y > limit || rightStick.y < -limit)
		{
			_camoffset.Set(rightStick.x, 0, rightStick.y);
			_camoffset *= _rightStickRatio;
		}
		else if (leftStick.x > limit || leftStick.x < -limit || leftStick.y > limit || leftStick.y < -limit)
		{
			_camoffset.Set(leftStick.x, 0, leftStick.y);
			_camoffset *= _leftStickRatio;
		}

		return _camoffset;
	}

}
