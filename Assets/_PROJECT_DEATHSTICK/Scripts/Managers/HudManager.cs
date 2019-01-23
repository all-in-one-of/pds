using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour {

	public Image roomSwitchImage;
	public Text roomSwitchText;
	
	private float timerB = 2f;

	private static string _currentRoomName = "";
	private static bool _phaseA = false;
	private static bool _phaseB = false;
	private static bool _phaseC = false;

	Color currentAlpha = new Color(1, 1, 1, 0f);

	private void Start()
	{
		currentAlpha = new Color(1, 1, 1, 0f);
		roomSwitchImage.color = currentAlpha;
		roomSwitchText.color = currentAlpha;
	}

	private void Update()
	{
		roomSwitchText.text = _currentRoomName;

		if (_phaseA)
		{
			if (currentAlpha.a < 1)
			{
				currentAlpha.a += Time.deltaTime / 2.0f;
				roomSwitchImage.color = currentAlpha;
				roomSwitchText.color = currentAlpha;
			}
			else
			{
				_phaseA = false;
				_phaseB = true;
			}
		}
		else if (_phaseB)
		{
			if ((timerB -= Time.deltaTime) <= 0)
			{
				timerB = 2f;
				_phaseB = false;
				_phaseC = true;
			}

		}
		else if (_phaseC)
		{
			if (currentAlpha.a > 0)
			{
				currentAlpha.a -= Time.deltaTime / 2.0f;
				roomSwitchImage.color = currentAlpha;
				roomSwitchText.color = currentAlpha;
			}
			else
			{
				_phaseC = false;
			}
		}
	}

	public static void OnRoomEnter(string roomName)
	{
		_currentRoomName = roomName;
		_phaseA = true;
	}

	private void SetRoomName(string roomName)
	{
		roomSwitchText.text = _currentRoomName;
	}

}
