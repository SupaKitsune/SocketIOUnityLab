using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class Network : MonoBehaviour {
	public static SocketIOComponent socket;
	public GameObject playerPrefab;

	Dictionary<string, GameObject> players;//new

	// Use this for initialization
	void Start () {
		socket = GetComponent<SocketIOComponent> ();
		socket.On ("open", OnConnected);
		socket.On ("spawn player", OnSpawned);
		socket.On ("disconnected", OnDisconnected);//new
		socket.On ("move", OnMove);//new
		players = new Dictionary<string, GameObject> ();//new
	}
	
	// Tells us we are connected
	void OnConnected (SocketIOEvent e) {
		Debug.Log ("We are Connected");
		socket.Emit ("playerhere");
	}

	void OnSpawned(SocketIOEvent e){
		Debug.Log ("Player " + e.data + " Spawned!");//new
		var player = Instantiate (playerPrefab);//new
		players.Add (e.data["id"].ToString(), player);//new
		Debug.Log ("count " + players.Count);//new
	}

	void OnDisconnected(SocketIOEvent e){//new
		string id = e.data["id"].ToString ();
		Debug.Log ("Player " + id + "disconnected.");

		Destroy (players [id]);
		players.Remove (id);
		Debug.Log ("count " + players.Count);
	}

	void OnMove(SocketIOEvent e){//new
		string id = e.data["id"].ToString();//.Replace("\"", "")
		//Debug.Log (id);
		GameObject player = players [id];

		var netMove = player.GetComponent<CharacterMovement> ();

		float x = GetFloatFromJson(e.data, "x");
		float y = GetFloatFromJson(e.data, "y");
		float h = GetFloatFromJson (e.data, "h");
		float v = GetFloatFromJson (e.data, "v");
		Vector3 objectPosition = new Vector3 (x, 0.0f, y);
		Vector3 objectMovement = new Vector3 (h, 0f, v);

		netMove.NetworkMovement (objectPosition, objectMovement);

		//player.transform.position = objectPosition;
		//Debug.Log("Networked player movement traffic: " + e.data);
	}

	float GetFloatFromJson(JSONObject data, string key){
		return float.Parse (data[key].ToString().Replace("\"", ""));
	}
}