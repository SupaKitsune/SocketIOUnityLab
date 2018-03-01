using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using SocketIO;

public class GameDataEditor : EditorWindow {
	string dataFilePath = "/StreamingAssets/data.json";
	public GameData editorData;
	private  GameObject server;
	private SocketIOComponent socket;

	[MenuItem("Window/Game Data Editor")]
	static void Init(){
		EditorWindow.GetWindow (typeof (GameDataEditor)).Show();
	}

	void OnGUI(){
		if(editorData != null){
			SerializedObject serializedObj = new SerializedObject (this);
			SerializedProperty serializedProperty = serializedObj.FindProperty ("editorData");
			EditorGUILayout.PropertyField (serializedProperty, true);
			serializedObj.ApplyModifiedProperties ();

			if(GUILayout.Button("Save Data")){
				SaveGameData ();
			}

			if (GUILayout.Button ("Send Data")) {
				SendGameData ();
			}
		}

		if(GUILayout.Button("Load Data")){
			LoadGameData ();
		}
	}

	void LoadGameData() {
		string filePath = Application.dataPath + dataFilePath;

		if (File.Exists (filePath)) {
			string gameData = File.ReadAllText (filePath);
			editorData = JsonUtility.FromJson<GameData> (gameData);
		} else {
			editorData = new GameData ();
		}
	}

	void SaveGameData() {
		string jsonObj = JsonUtility.ToJson (editorData);

		string filePath = Application.dataPath + dataFilePath;
		File.WriteAllText (filePath, jsonObj);
	}

	void SendGameData() {
		if(socket == null){
			server = GameObject.Find ("server");
			socket = server.GetComponent<SocketIOComponent> ();
		}

		string jsonObj = JsonUtility.ToJson (editorData);

		socket.Emit ("data pass", new JSONObject(jsonObj));
	}

//	void Start () {
//		
//	}
//
//	void Update () {
//		
//	}
}