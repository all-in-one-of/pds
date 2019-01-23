using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
	
	/**********************/
	/* VISIBLE PROPERTIES */
	/**********************/

	[Header("Player Related")]
	[SerializeField]
	private Player _mainPlayer;

	[SerializeField]
	private float _playerRadius;

	[Space()]
	[SerializeField]
	private PlayerCamera _mainPlayerCamera;
	public PlayerCamera MainPlayerCamera { get { return _mainPlayerCamera; } }

	[Header("Room Related")]
	[SerializeField]
	private Room _startingRoom;

	/**************************/
	/* NON VISIBLE PROPERTIES */
	/**************************/
	
	public static List<Room> RoomList = new List<Room>();
	private Room _currentRoom;

	private Vector2 _playerRoomPos = new Vector2();

	private Vector2 _playerRoomSpawn = new Vector2();

	//Initialisation
	private void Awake()
	{
		if (_startingRoom == null)
		{
			Debug.LogError("MainManager Error : Add starting room to the Main Manager !", gameObject);
			return;
		}

		Room r = _startingRoom.GetComponent<Room>();
		if (r != null)
		{
			if (!r.HasSpawnPoint) Debug.LogError("MainManager Error : Main Room is not a starting room, please configure room prefab.", gameObject);
		}
		else Debug.LogError("MainManager Error : mainRoom is not a room. (Attach compenent Room)", gameObject);
		
	}

	//GameStart
	private void Start()
	{
		//Cleans all active rooms
		foreach (Room room in RoomList)
		{
			//room.gameObject.transform.position = new Vector3();
			room.gameObject.SetActive(false);
		}

		LoadRoom(_startingRoom);
		HudManager.OnRoomEnter(_startingRoom.RoomName);

		_mainPlayer.transform.position = new Vector3(_currentRoom.PlayerSpawn.x, _mainPlayer.transform.position.y, _currentRoom.PlayerSpawn.y);
		_mainPlayer.transform.position += _currentRoom.transform.position;
		_mainPlayerCamera.CameraTarget = _mainPlayer.transform.position;

		MainPlayerCamera.ResetCameraPos();
	}

	//Manager Loop
	private void Update()
	{
		Vector3 currentRoomPos = _currentRoom.transform.position;
		//Set player position in current room
		_playerRoomPos.Set(_mainPlayer.transform.position.x, _mainPlayer.transform.position.z);
		//Check if player is out of bounds and replace if necessary
		CheckMainPlayerBounds();
		//Check gameplay objects interaction (pickables, projectiles, etc...)
		CheckGameplayObjects();
		//Set Camera target relative to player pos
		_mainPlayerCamera.CameraTarget = GetCameraTarget(_mainPlayer.transform.position + _mainPlayerCamera.GetCameraTargetOffset());
		Debug.DrawLine(_mainPlayer.transform.position, _mainPlayerCamera.CameraTarget, Color.red);
	}
	
	private void CheckGameplayObjects()
	{
		//Check Pickables
		if (_currentRoom.Pickables.Count != 0)
		{
			Vector3 itemPosition;
			Vector2 itemRoomPos = new Vector2();

			foreach (Pickable item in _currentRoom.Pickables)
			{
				itemPosition = item.transform.position - _currentRoom.transform.position;
				itemRoomPos.Set(itemPosition.x, itemPosition.z);
				itemRoomPos += new Vector2(_currentRoom.transform.position.x, _currentRoom.transform.position.z);

				Debug.DrawLine(new Vector3(itemRoomPos.x,0,itemRoomPos.y), itemPosition + Vector3.up * 10);

				if (Vector2.Distance(itemRoomPos,_playerRoomPos) <= item.PickRadius)
				{
					item.OnPick();
				}
			}
		}

		//Checks for nearby doors in room
		if (_currentRoom.Doors.Count != 0)
		{
			foreach (RoomDoor door in _currentRoom.Doors)
			{
				if (!door.DoorController) continue;
				float dist = Vector3.Distance(door.doorFloorCenter, new Vector3(_playerRoomPos.x, 0 ,_playerRoomPos.y));
				if (dist <= door.DoorController.ActivationDistance) door.DoorController.OpenDoor();
			}
		}

		//Checks projectiles life span
		if (Projectile.Projectiles.Count != 0)
		{
			for (int i = Projectile.Projectiles.Count - 1; i >= 0; i--)
			{
				Projectile lProj = Projectile.Projectiles[i];
				if (lProj.LifeSpan > lProj.LifeTime)
				{ 
					Projectile.Projectiles.RemoveAt(i);
					Destroy(lProj.gameObject);
				}
			}
		}
	}

	//Keeps the main player within the room zone
	private void CheckMainPlayerBounds()
	{
		bool outOfBounds = false;
		ROOM_SIDE playerSide = ROOM_SIDE.UP;

		Vector2 currentRoomWorldPos = new Vector2(_currentRoom.transform.position.x, _currentRoom.transform.position.z);
		Vector2 currentBounds = _currentRoom.RoomBounds;
		Vector3 playerPosCorrection = _mainPlayer.transform.position;

		//Debug.Log("curr Room pos (minimals)= " + currentRoomWorldPos);
		//Debug.Log("curr Room Bounds (room size)= " + currentBounds);
		//Debug.Log("Room Bounds relative to pos (calculated maximal)= " + (currentBounds + currentRoomWorldPos));

		if (_playerRoomPos.x - _playerRadius < currentRoomWorldPos.x)
		{
			playerPosCorrection.x = currentRoomWorldPos.x + _playerRadius;
			outOfBounds = true;
			playerSide = ROOM_SIDE.LEFT;
		}
		else if (_playerRoomPos.x + _playerRadius > currentRoomWorldPos.x + currentBounds.x)
		{
			playerPosCorrection.x = currentRoomWorldPos.x + currentBounds.x - _playerRadius;
			outOfBounds = true;
			playerSide = ROOM_SIDE.RIGHT;
		}

		if (_playerRoomPos.y - _playerRadius < currentRoomWorldPos.y)
		{
			playerPosCorrection.z = currentRoomWorldPos.y + _playerRadius;
			outOfBounds = true;
			playerSide = ROOM_SIDE.DOWN;
		}
		else if (_playerRoomPos.y + _playerRadius > currentRoomWorldPos.y + currentBounds.y)
		{
			playerPosCorrection.z = currentRoomWorldPos.y + currentBounds.y - _playerRadius;
			outOfBounds = true;
			playerSide = ROOM_SIDE.UP;
		}

		if (outOfBounds)
		{
			Debug.Log("OOB " + playerSide);

			//Checks if player is in a door
			List<RoomDoor> potentialDoors = CheckPlayerDoorAlign(_currentRoom, _playerRoomPos);

			if (potentialDoors != null && potentialDoors.Count > 0)
			{
				RoomDoor passingDoor = null;
				foreach (RoomDoor door in potentialDoors)
				{
					if (door.Side == playerSide)
					{
						passingDoor = door;
						break;
					}
				}

				if (passingDoor != null && !passingDoor.IsLocked && passingDoor.RoomConnected != null)
					SwitchToRoom(passingDoor.RoomConnected, passingDoor.RoomConnected.Doors[(int)passingDoor.DoorIndex]);
				else
					_mainPlayer.transform.position = playerPosCorrection;
			}
			else _mainPlayer.transform.position = playerPosCorrection;
		}

	}

	//Returns all doors aligned with according target
	private List<RoomDoor> CheckPlayerDoorAlign(Room room, Vector2 playerPos)
	{
		if (room.Doors.Count == 0) return null;

		List<RoomDoor> doors = new List<RoomDoor>();
		foreach (RoomDoor door in room.Doors)
		{
			doors.Add(door);
		}

		if (doors.Count == 0) return doors;

		float doorMin;
		float doorMax;

		for (int i = doors.Count-1; i >= 0; i--)
		{
			doorMin = 0f;
			doorMax = 0f;
			switch (doors[i].Side)
			{
				case ROOM_SIDE.UP:
				case ROOM_SIDE.DOWN:

					doorMin = (room.transform.position.x + doors[i].WallPos) - (doors[i].DoorSize / 2);
					doorMax = (room.transform.position.x + doors[i].WallPos) + (doors[i].DoorSize / 2);

					if(!(playerPos.x - _playerRadius >= doorMin && playerPos.x + _playerRadius <= doorMax))
					{
						doors.Remove(doors[i]);
					}

					break;

				case ROOM_SIDE.LEFT:
				case ROOM_SIDE.RIGHT:

					doorMin = (room.transform.position.z + doors[i].WallPos) - (doors[i].DoorSize / 2);
					doorMax = (room.transform.position.z + doors[i].WallPos) + (doors[i].DoorSize / 2);

					if (!(playerPos.y - _playerRadius >= doorMin && playerPos.y + _playerRadius <= doorMax))
					{
						doors.Remove(doors[i]);
					}

					break;
			}
		}

		return doors;
	}

	//Returns the closest point to target within camera borders in current room
	private Vector3 GetCameraTarget(Vector3 target)
	{
		Vector3 l_target = target; // absolu

		Vector2 roomWorldPos = new Vector2(_currentRoom.transform.position.x, _currentRoom.transform.position.z); //absolu
		Vector2 currentBounds = _currentRoom.RoomBounds; //2 float pour x et y
		Vector2 cameraBoundsDist = _currentRoom.CameraBoundsDistance; //2 float pour x et y

		Vector2 targetRoomPos = new Vector2(l_target.x, l_target.z) + roomWorldPos; //relative

		//Si la camera bound est plus grande que la salle alors on restreint a la salle
		if (cameraBoundsDist.x > currentBounds.x / 2) cameraBoundsDist.x = currentBounds.x / 2;
		if (cameraBoundsDist.y > currentBounds.y / 2) cameraBoundsDist.y = currentBounds.y / 2;

		Rect CameraBounds = new Rect(
			roomWorldPos.x + cameraBoundsDist.x,
			roomWorldPos.y + cameraBoundsDist.y,
			currentBounds.x - cameraBoundsDist.x*2,
			currentBounds.y - cameraBoundsDist.y*2
		);

		Debug.DrawLine(l_target, l_target + new Vector3(0,10,0), Color.yellow);
		Debug.DrawLine(targetRoomPos, targetRoomPos + new Vector2(0,10), Color.yellow);

		if (l_target.x < CameraBounds.xMin) l_target.x = CameraBounds.xMin;
		if (l_target.z < CameraBounds.yMin) l_target.z = CameraBounds.yMin;
		if (l_target.x > CameraBounds.xMax) l_target.x = CameraBounds.xMax;
		if (l_target.z > CameraBounds.yMax) l_target.z = CameraBounds.yMax;

		return l_target;
	}
	
	//Loads the room in the scene. Doesn't replace the player. Great for initialisation.
	//To replace the player automatically use SwitchToRoom() instead;
	private void LoadRoom(Room room)
	{
		if (_currentRoom != null) _currentRoom.gameObject.SetActive(false);
		room.gameObject.SetActive(true);
		_currentRoom = room;
	}

	//Loads the room and replace the player at specified door spawn.
	private void SwitchToRoom(Room room, RoomDoor door)
	{
		LoadRoom(room);
		//reset position
		_mainPlayer.transform.position = new Vector3(door.RoomSpawn.x, _mainPlayer.transform.position.y, door.RoomSpawn.y);
		_mainPlayer.transform.position += _currentRoom.transform.position;
		//reset camera
		_mainPlayerCamera.CameraTarget = _mainPlayer.transform.position;
		_mainPlayerCamera.ResetCameraPos();
		//hud
		HudManager.OnRoomEnter(room.RoomName);
	}

	
}
