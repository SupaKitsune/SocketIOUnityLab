using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayerData : MonoBehaviour {
	string dataFilePath = "/StreamingAssets/data.json";

	void Start () {
		GameData jsonObj = new GameData ();
		jsonObj.playerName = "Player";
		jsonObj.score = 2018;
		jsonObj.timePlayed = 512f;
		jsonObj.lastLogin = "March 1, 2018";

		string json = JsonUtility.ToJson (jsonObj);
		string path = Application.dataPath + dataFilePath;
		File.WriteAllText (path, json);

		Debug.Log (json);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
