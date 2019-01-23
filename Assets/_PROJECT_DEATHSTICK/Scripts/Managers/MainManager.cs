using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour {

	[Header("GameState variables")]
	[SerializeField]
	private GAME_STATE _startingState = GAME_STATE.GAME;
	private static GAME_STATE _gameState = GAME_STATE.GAME;
	public static GAME_STATE GameState { get { return _gameState; } }

	[Header("Main Game Infos")]
	[SerializeField]
	private float _pauseSpeed = 0.001f;
	
	[SerializeField]
	public static bool _debugRooms = true;

	// Use this for initialization
	private void Awake () {

		T_Inputs.Init();
		_gameState = _startingState;
		
	}

	private void PauseGame()
	{
		while(Time.timeScale > 0)
		{
			Time.timeScale -= _pauseSpeed;
			if (Time.timeScale > 0) Time.timeScale = 0;
		}
	}

}
