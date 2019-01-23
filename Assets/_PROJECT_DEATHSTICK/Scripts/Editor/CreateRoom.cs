using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

public class CreateRoom : EditorWindow{

	//Could open a window and show presets for rooms :D

	private Preset _roomPreset;

	[MenuItem("Project DeathStick/Rooms/Create New Room")]
	public static void ShowWindow()
	{
		GetWindow<CreateRoom>("Create Room");
	}

	private void OnGUI()
	{
		_roomPreset = (Preset)EditorGUILayout.ObjectField("Room Preset : ", _roomPreset, typeof(Preset), false);

		if(GUILayout.Button("Create Room"))
		{
			//Creation des Objets
			GameObject room = new GameObject("NEW_DEFAULT_ROOM");
			GameObject lights = new GameObject("Lights");
			GameObject environment = new GameObject("Environment");
			GameObject collisions = new GameObject("Collisions");
			GameObject gameplay = new GameObject("Gameplay Items");

			//Ajout du room component
			Room roomComponent = room.AddComponent<Room>();

			//Application du preset
			if (_roomPreset != null)
			{
				_roomPreset.ApplyTo(roomComponent);
				room.name = roomComponent.RoomName;
			}

			//Parentage des objets
			lights.transform.parent = room.transform;
			environment.transform.parent = room.transform;
			collisions.transform.parent = room.transform;
			gameplay.transform.parent = room.transform;

		}
	}
	
}
