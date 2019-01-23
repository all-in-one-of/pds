using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class T_Inputs
{
	//Pad String Configuration
	private const string PAD_PREFIX = "Pad_";
	private const string LEFT_JOYSTICK_PREFIX = "LeftJoy_";
	private const string RIGHT_JOYSTICK_PREFIX = "RightJoy_";
	private const string TRIGGERS_PREFIX = "Trigger_";
	private const string BUMPERS_PREFIX = "Bump_";
	private const string BUTTONS_PREFIX = "Btn_";

	private const string A = "A";
	private const string B = "B";
	private const string X = "X";
	private const string Y = "Y";
	private const string L = "L";
	private const string R = "R";
	private const string SELECT = "Select";
	private const string START = "Start";

	private static Dictionary<string, bool> _axisValues;

	/// <summary>
	/// 
	/// </summary>
	public static void Init()
	{
		_axisValues = new Dictionary<string, bool>();
	}

	/// <summary>
	/// Returns true while the button is held down
	/// </summary>
	/// <param name="btn"></param>
	/// <returns></returns>
	public static bool GetButton(BUTTON btn)
	{
		string input = PAD_PREFIX + BUTTONS_PREFIX;
		switch (btn)
		{
			case BUTTON.A:
				input += A;
				break;
			case BUTTON.B:
				input += B;
				break;
			case BUTTON.X:
				input += X;
				break;
			case BUTTON.Y:
				input += Y;
				break;
			case BUTTON.SELECT:
				input += SELECT;
				break;
			case BUTTON.START:
				input += START;
				break;
		}

		return Input.GetButton(input);
	}

	/// <summary>
	/// Returns true on the frame the button is pressed
	/// </summary>
	/// <param name="btn"></param>
	/// <returns></returns>
	public static bool GetButtonDown(BUTTON btn)
	{
		string input = PAD_PREFIX + BUTTONS_PREFIX;
		switch (btn)
		{
			case BUTTON.A:
				input += A;
				break;
			case BUTTON.B:
				input += B;
				break;
			case BUTTON.X:
				input += X;
				break;
			case BUTTON.Y:
				input += Y;
				break;
			case BUTTON.SELECT:
				input += SELECT;
				break;
			case BUTTON.START:
				input += START;
				break;
		}

		return Input.GetButtonDown(input);
	}

	/// <summary>
	/// Returns true on the frame the button is released
	/// </summary>
	/// <param name="btn"></param>
	/// <returns></returns>
	public static bool GetButtonUp(BUTTON btn)
	{
		string input = PAD_PREFIX + BUTTONS_PREFIX;
		switch (btn)
		{
			case BUTTON.A:
				input += A;
				break;
			case BUTTON.B:
				input += B;
				break;
			case BUTTON.X:
				input += X;
				break;
			case BUTTON.Y:
				input += Y;
				break;
			case BUTTON.SELECT:
				input += SELECT;
				break;
			case BUTTON.START:
				input += START;
				break;
		}

		return Input.GetButtonUp(input);
	}


	/// <summary>
	/// Returns the value of the specified joystick axis
	/// </summary>
	/// <param name="btn"></param>
	/// <returns></returns>
	public static float GetJoystickAxis(JOYSTICK joy, AXIS axis)
	{
		string input = PAD_PREFIX;

		switch (joy)
		{
			case JOYSTICK.L:
				input += LEFT_JOYSTICK_PREFIX;
				break;
			case JOYSTICK.R:
				input += RIGHT_JOYSTICK_PREFIX;
				break;
		}

		switch (axis)
		{
			case AXIS.X:
				input += X;
				break;
			case AXIS.Y:
				input += Y;
				break;
		}

		return Input.GetAxisRaw(input);
	}

	/// <summary>
	/// Returns true on the frame the axis passes the trigger value
	/// Use negative values for negative axis
	/// Ex : left means the trigger value is -0.5f on X Axis
	/// </summary>
	/// <param name="btn"></param>
	/// <returns></returns>
	public static bool GetJoystickAxisDown(JOYSTICK joy, AXIS axis, float triggerValue)
	{
		bool value = false;

		string input = PAD_PREFIX;

		switch (joy)
		{
			case JOYSTICK.L:
				input += LEFT_JOYSTICK_PREFIX;
				break;
			case JOYSTICK.R:
				input += RIGHT_JOYSTICK_PREFIX;
				break;
		}

		switch (axis)
		{
			case AXIS.X:
				input += X;
				break;
			case AXIS.Y:
				input += Y;
				break;
		}

		bool test = false;
		if (triggerValue < 0)
		{
			test = Input.GetAxis(input) <= triggerValue;
			input += "_Neg";
		}
		else
		{
			test = Input.GetAxis(input) >= triggerValue;
			input += "_Pos";
		}

		if (test)
		{
			if (!_axisValues.ContainsKey(input))
			{
				_axisValues.Add(input, true);
				value = true;
			}
			else
			{
				value = !_axisValues[input];
				_axisValues[input] = true;
			}
		}
		else
		{
			if (!_axisValues.ContainsKey(input))
			{
				_axisValues.Add(input, false);
			}
			else
			{
				_axisValues[input] = false;
			}

			value = false;

		}

		return value;
	}

	/// <summary>
	/// Returns true on the frame the trigger passes the trigger value
	/// </summary>
	/// <param name="btn"></param>
	/// <returns></returns>
	public static bool GetTriggerAxisDown(TRIGGER trig, float triggerValue)
	{
		bool value = false;

		string input = PAD_PREFIX;

		switch (trig)
		{
			case TRIGGER.L:
				input += TRIGGERS_PREFIX + L;
				break;
			case TRIGGER.R:
				input += TRIGGERS_PREFIX + R;
				break;
		}

		bool test = Input.GetAxis(input) >= triggerValue;

		if (test)
		{
			if (!_axisValues.ContainsKey(input))
			{
				_axisValues.Add(input, true);
				value = true;
			}
			else
			{
				value = !_axisValues[input];
				_axisValues[input] = true;
			}
		}
		else
		{
			if (!_axisValues.ContainsKey(input))
			{
				_axisValues.Add(input, false);
			}
			else
			{
				_axisValues[input] = false;
			}

			value = false;

		}

		return value;
	}

	/// <summary>
	/// Returns true on the frame the trigger passes the trigger value
	/// </summary>
	/// <param name="btn"></param>
	/// <returns></returns>
	public static bool GetTriggerAxis(TRIGGER trig, float triggerValue)
	{
		string input = PAD_PREFIX;

		switch (trig)
		{
			case TRIGGER.L:
				input += TRIGGERS_PREFIX + L;
				break;
			case TRIGGER.R:
				input += TRIGGERS_PREFIX + R;
				break;
		}

		bool test = Input.GetAxis(input) >= triggerValue;
		
		return test;
	}
}