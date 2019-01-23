using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Room))]
public class RoomEditor : Editor
{
	Room room;

	private void OnEnable()
	{
		room = (Room)target;
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.HelpBox("Rooms must be Prefabs that will be instanciated at position 0,0,0", MessageType.Info);

		//Editeur classique du monobehaviour
		base.OnInspectorGUI();
	}
}

public class Room : MonoBehaviour
{
	[Header("Editor Values")]
	[SerializeField]
	private bool _showDebug = true;
	[SerializeField]
	private bool _showRoomName = true;
	[SerializeField]
	private bool _showCameraDebug = true;
	[SerializeField]
	private bool _showDoorSpawns = true;
	[SerializeField]
	private Color _roomDebugColor = Color.white;
	[SerializeField]
	private Color _cameraDebugColor = Color.red;
	[SerializeField]
	private Color _doorSpawnDebugColor = Color.yellow;
	[SerializeField]
	private float _editorDoorHeight = 2.0f;
	
	private const float ARROW_ROOMS_DIV = 2.0f;
	private const float ARROW_ROOMS_RATIO = 5.0f;

	[Header("Starting Room ?")]

	[SerializeField]
	private bool _hasSpawnPoint = false;
	public bool HasSpawnPoint { get { return _hasSpawnPoint; } }

	[SerializeField]
	private Vector2 _playerSpawn = new Vector2();
	public Vector2 PlayerSpawn { get { return _playerSpawn; } }

	[Header("Room Description")]

	[SerializeField]
	private string _roomName = "Room";
	public string RoomName { get { return _roomName; } }

	[Space()]
	[SerializeField]
	private Vector2 _roomBounds = new Vector2();
	public Vector2 RoomBounds { get { return _roomBounds; } }

	[SerializeField]
	private Vector2 _cameraBoundsDistance;
	public Vector2 CameraBoundsDistance { get { return _cameraBoundsDistance; } }

	[Header("Doors Descriptions")]
	[SerializeField]
	private List<RoomDoor> _doors;
	public List<RoomDoor> Doors { get { return _doors; } }

	[Header("Gameplay Objects")]
	[SerializeField]
	private List<Pickable> _pickables;
	public List<Pickable> Pickables { get { return _pickables; } }

	public void Awake()
	{
		AddToLevelManager();
	}

	private void AddToLevelManager()
	{
		LevelManager.RoomList.Add(this);
	}

	//EDITOR PART
	//DO NOT TOUCH UNLESS YOU KNOW WHAT YOU'RE DOING

	#region EDITOR
	private void OnDrawGizmos()
	{
		if (!MainManager._debugRooms) return;

		Gizmos.color = _roomDebugColor;

		GUIStyle handlesStyle = new GUIStyle();
		handlesStyle.fontStyle = FontStyle.Bold;
		handlesStyle.alignment = TextAnchor.MiddleCenter;
		handlesStyle.normal.textColor = _roomDebugColor;

		if (_showDebug)
		{
			Vector3 roomPos = transform.position;

			Vector3[] corners = {
				new Vector3(roomPos.x, roomPos.y, roomPos.z),
				new Vector3(roomPos.x, roomPos.y, roomPos.z + RoomBounds.y),
				new Vector3(roomPos.x + RoomBounds.x, roomPos.y, roomPos.z + RoomBounds.y),
				new Vector3(roomPos.x + RoomBounds.x, roomPos.y, roomPos.z)
			};
			
			Gizmos.DrawLine(corners[0], corners[1]);
			Gizmos.DrawLine(corners[1], corners[2]);
			Gizmos.DrawLine(corners[2], corners[3]);
			Gizmos.DrawLine(corners[3], corners[0]);

			if (_showRoomName)
			{
				Vector3 roomNamePos = new Vector3(RoomBounds.x / 2, 0, RoomBounds.y / 2);
				Handles.Label(roomNamePos + transform.position, gameObject.name, handlesStyle);
			}

			if (_showCameraDebug)
			{
				Vector3[] cameraPoints = CalcCameraPoints();
				if (cameraPoints != null || cameraPoints.Length != 0)
				{
					Gizmos.color = _cameraDebugColor;
					Gizmos.DrawLine(cameraPoints[0], cameraPoints[1]);
					Gizmos.DrawLine(cameraPoints[1], cameraPoints[2]);
					Gizmos.DrawLine(cameraPoints[2], cameraPoints[3]);
					Gizmos.DrawLine(cameraPoints[3], cameraPoints[0]);
					Gizmos.color = _roomDebugColor;
				}
			}

			//Drawing doors
			if (Doors != null && Doors.Count != 0)
			{
				foreach (RoomDoor door in Doors)
				{
					if (door == null) continue;

					Vector3[] doorPoints = CalcDoorPoints(door);

					if (doorPoints != null || doorPoints.Length != 0)
					{
						door.doorFloorCenter = (doorPoints[0] + doorPoints[3]) / 2;

						//Drawing Room Links
						if (door.RoomConnected != null)
						{
							if(door.RoomConnected.Doors[(int)door.DoorIndex] != null)
							{
								Vector3 otherRoomDoorCenter = door.RoomConnected.Doors[(int)door.DoorIndex].doorFloorCenter;
								Vector3 doorConnectionArrowRatio = (door.doorFloorCenter - otherRoomDoorCenter) / ARROW_ROOMS_RATIO;
								Vector3 arrowPostiveCross = Vector3.Cross(doorConnectionArrowRatio, Vector3.up).normalized / ARROW_ROOMS_DIV;

								//Draws Arrow to connected room door
								Gizmos.DrawLine(door.doorFloorCenter, otherRoomDoorCenter);
								Gizmos.DrawLine(otherRoomDoorCenter, otherRoomDoorCenter + (doorConnectionArrowRatio + arrowPostiveCross));
								Gizmos.DrawLine(otherRoomDoorCenter, otherRoomDoorCenter + (doorConnectionArrowRatio - arrowPostiveCross));
								Gizmos.DrawLine(otherRoomDoorCenter + (doorConnectionArrowRatio + arrowPostiveCross), otherRoomDoorCenter + (doorConnectionArrowRatio - arrowPostiveCross));
							}
						}

						//door is red if nor connected
						Gizmos.color = door.RoomConnected != null ? _roomDebugColor : Color.red;

						Gizmos.DrawLine(doorPoints[0], doorPoints[1]);
						Gizmos.DrawLine(doorPoints[1], doorPoints[2]);
						Gizmos.DrawLine(doorPoints[2], doorPoints[3]);

						Handles.color = _roomDebugColor;
						Handles.Label(-Vector3.up*_editorDoorHeight/3 + (doorPoints[1] + doorPoints[2]) / 2, _doors.IndexOf(door).ToString(), handlesStyle);
						
						//Drawing spawn
						if (_showDoorSpawns)
						{
							Gizmos.color = _doorSpawnDebugColor;
							Vector3 spawnPos = transform.position + new Vector3(door.RoomSpawn.x, 0, door.RoomSpawn.y);
							Gizmos.DrawSphere(spawnPos, 0.5f);

							Gizmos.DrawLine(door.doorFloorCenter, spawnPos);
							Gizmos.color = _roomDebugColor;
						}

					}
				}
			}

			if (_hasSpawnPoint)
			{
				Gizmos.DrawSphere(transform.position + new Vector3(_playerSpawn.x, roomPos.y, _playerSpawn.y), .5f);
			}
		}
	}

	private Vector3[] CalcCameraPoints()
	{
		Vector3[] points = { new Vector3(), new Vector3(), new Vector3(), new Vector3() };

		if (RoomBounds.x / 2 > _cameraBoundsDistance.x)
		{
			points[0].x = transform.position.x + _cameraBoundsDistance.x;
			points[1].x = transform.position.x + _cameraBoundsDistance.x;
			points[2].x = transform.position.x + RoomBounds.x - _cameraBoundsDistance.x;
			points[3].x = transform.position.x + RoomBounds.x - _cameraBoundsDistance.x;
		}
		else
		{
			for (int i = 0; i < 4; i++)
			{
				points[i].x = transform.position.x + RoomBounds.x / 2;
			}
		}

		if (RoomBounds.y / 2 > _cameraBoundsDistance.y)
		{
			points[0].z = transform.position.z + _cameraBoundsDistance.y;
			points[3].z = transform.position.z + _cameraBoundsDistance.y;
			points[1].z = transform.position.z + RoomBounds.y - _cameraBoundsDistance.y;
			points[2].z = transform.position.z + RoomBounds.y - _cameraBoundsDistance.y;
		}
		else
		{
			for (int i = 0; i < 4; i++)
			{
				points[i].z = transform.position.x + RoomBounds.y / 2;
			}
		}

		for (int i = 0; i < 4; i++)
		{
			points[i].y = transform.position.y;
		}

		return points;
	}

	private Vector3[] CalcDoorPoints(RoomDoor d)
	{
		Vector3[] points = { new Vector3(), new Vector3(), new Vector3(), new Vector3() };

		Vector3 roomPos = transform.position;

		switch (d.Side)
		{
			case ROOM_SIDE.UP:
				//Low points
				points[0] = new Vector3(roomPos.x + d.WallPos - d.DoorSize / 2, transform.position.y, roomPos.z + RoomBounds.y);
				points[3] = new Vector3(roomPos.x + d.WallPos + d.DoorSize / 2, transform.position.y, roomPos.z + RoomBounds.y);
				break;
			case ROOM_SIDE.DOWN:
				//Low points
				points[0] = new Vector3(roomPos.x + d.WallPos - d.DoorSize / 2, transform.position.y, roomPos.z);
				points[3] = new Vector3(roomPos.x + d.WallPos + d.DoorSize / 2, transform.position.y, roomPos.z);
				break;
			case ROOM_SIDE.LEFT:
				//Low points
				points[0] = new Vector3(roomPos.x, transform.position.y, roomPos.z + d.WallPos - d.DoorSize / 2);
				points[3] = new Vector3(roomPos.x, transform.position.y, roomPos.z + d.WallPos + d.DoorSize / 2);
				break;
			case ROOM_SIDE.RIGHT:
				//Low points
				points[0] = new Vector3(roomPos.x + RoomBounds.x, transform.position.y, roomPos.z + d.WallPos - d.DoorSize / 2);
				points[3] = new Vector3(roomPos.x + RoomBounds.x, transform.position.y, roomPos.z + d.WallPos + d.DoorSize / 2);
				break;
			default:
				break;
		}

		//High Points
		points[1] = points[0] + Vector3.up * _editorDoorHeight;
		points[2] = points[3] + Vector3.up * _editorDoorHeight;
		return points;
	}
	#endregion
	
	//!EDITOR PART
}
