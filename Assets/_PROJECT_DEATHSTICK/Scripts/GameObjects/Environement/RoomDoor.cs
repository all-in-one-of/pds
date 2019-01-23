using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomDoor {

	//Visible values
	[Header("Door Parameters")]
	[SerializeField]
	private ROOM_SIDE _side = ROOM_SIDE.UP;
	public ROOM_SIDE Side { get { return _side; } }

	[SerializeField]
	private float _wallPos = 0f;
	public float WallPos { get { return _wallPos; } }

	[SerializeField]
	private float _doorSize = 5.0f;
	public float DoorSize { get { return _doorSize; } }

	[SerializeField]
	private Vector2 _roomSpawn = new Vector2();
	public Vector2 RoomSpawn { get { return _roomSpawn; } }

	[Header("Room Connections")]
	[SerializeField]
	private Room _roomConnected;
	public Room RoomConnected { get { return _roomConnected; } }

	[SerializeField]
	[Range(0,10)]
	private uint _doorIndex = 0;
	public uint DoorIndex { get { return _doorIndex; } }

	[Header("In Game Properties")]
	[SerializeField]
	private bool _isLocked = false;
	public bool IsLocked { get { return _isLocked; } }

	[SerializeField]
	private DoorController _doorController;
	public DoorController DoorController { get { return _doorController; } }

	//Hidden Values
	[HideInInspector]
	public Vector3 doorFloorCenter;
}
