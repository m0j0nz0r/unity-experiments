using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : Photon.MonoBehaviour {
	int state;
	public int maxPlayers;
	public GameObject[] playerSpawns;

	int labelX = 10;
	int labelY = 10;
	int buttonX = 10;
	int buttonY = 50;
	void Start () {
		state = 0;
	}
	
	void Update () {
		
	}
	void OnGUI(){
		switch (state) {
		case 0: //start
			GUI.Label (new Rect (labelX, labelY, 200, 30), "Start");
			if (GUI.Button (new Rect (buttonX, buttonY, 200, 30), "Connect")) {
				PhotonNetwork.ConnectUsingSettings ("V0");
				state = 4;
			}
			break;
		case 1: //Connected to Server
			GUI.Label (new Rect (labelX, labelY, 200, 30), "Connected to Server");
			if (GUI.Button (new Rect (buttonX, buttonY, 200, 30), "Join Lobby")) {
				PhotonNetwork.JoinLobby ();
				state = 5;
			}
			break;
		case 2: //Connected to Lobby
			GUI.Label (new Rect (labelX, labelY, 200, 30), "Connected to Lobby");
			if (GUI.Button (new Rect (buttonX, buttonY, 200, 30), "Join Room")) {
				PhotonNetwork.JoinRandomRoom();
				state = 4;
			}
			break;
		case 3: //Connected to Room
			GUI.Label (new Rect (labelX, labelY, 200, 30), "Now Playing");
			break;
		case 4: //Break
			break;
		case 5: //Matchmaking
			GUI.Label(new Rect(10, Screen.height - 50, 200, 30), "Players: " + PhotonNetwork.playerList.Length.ToString() + "/" + maxPlayers.ToString());
			if (PhotonNetwork.inRoom == true && PhotonNetwork.playerList.Length == maxPlayers){
				GetComponent<PhotonView> ().RPC ("StartGame", PhotonTargets.All);
			}
			break;
		}
	}
	void OnConnectedToPhoton(){
		state = 1;
	}
	void OnJoinedLobby(){
		state = 2;
	}
	void OnPhotonRandomJoinFailed(){
		PhotonNetwork.CreateRoom (null);
	}

	[PunRPC]
	void StartGame(){
		state = 3;
		GameObject spawn = playerSpawns [0];
		GameObject hero = (GameObject)PhotonNetwork.Instantiate ("PlayerTest", spawn.transform.position, spawn.transform.rotation, 0);

		Camera.main.GetComponent<SmoothFollow> ().target = hero.transform;
	}
}
