using UnityEngine;

namespace testGame{
	public class Launcher : Photon.PunBehaviour {
		#region Public Variables

		/// <summary>
		/// The PUN loglevel
		/// </summary>
		public PhotonLogLevel Loglevel = PhotonLogLevel.Informational;

		/// <summary>
		/// The maximum number of players per room. When a room is full it can't be joined by new players, and so a new room will be created.
		/// </summary>
		[Tooltip("The maximum number of players per room. When a room is full it can't be joined by new players, and so a new room will be created.")]
		public byte MaxPlayersPerRoom = 4;

		/// <summary>
		/// The Ui Panel to let the user enter name, connect and play
		/// </summary>
		[Tooltip("The Ui Panel to let the user enter name, connect and play")]
		public GameObject controlPanel;

		/// <summary>
		/// The Ui Label to inform the user that the connection is in progress
		/// </summary>
		[Tooltip("The Ui Label to inform the user that the connection is in progress")]
		public GameObject progressLabel;

		#endregion


		#region Private Variables 

		/// <summary>
		/// This client's version number. Users are separated from each other by gameversion (which allows you to make breaking changes).
		/// </summary>
		string _gameVersion = "0";

		///<summary>
		/// Keep track of the current process. Since connection is asyncronous and is based on several callbacks from Photon,
		/// we need to keep track of this to properly adjust de behaviour when we receive a callback by Photon.
		/// Typically this is used for the OnConnectedToMaster() callback.
		/// </summary>
		bool isConnecting;

		#endregion


		#region MonoBehaviour Callbacks

		/// <summary>
		/// MonoBehaviour method called on GameObject by Unity during early initialization phase.
		/// </summary>
		void Awake(){

			// #Critical
			// we don't join the lobby. There is no need to join a lobby to get the list of rooms.
			PhotonNetwork.autoJoinLobby = false;

			// #Critical
			// this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
			PhotonNetwork.automaticallySyncScene = true;

			PhotonNetwork.logLevel = Loglevel;
		}

		void Start(){
			controlPanel.SetActive (true);
			progressLabel.SetActive (false);
		}

		#endregion


		#region Public Methods

		/// <summary>
		/// Start the connection process. 
		/// - If already connected, we attempt joining a random room
		/// - if not yet connected, Connect this application instance to Photon Cloud Network
		/// </summary>
		public void Connect(){
			isConnecting = true;

			controlPanel.SetActive (false);
			progressLabel.SetActive (true);

			// we check if we are connected or not, we join if we are , else we initiate the connection to the server.
			if (PhotonNetwork.connected) {
				// #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
				PhotonNetwork.JoinRandomRoom ();
			} else {
				// #Critical, we must first and foremost connect to Photon Online Server.
				PhotonNetwork.ConnectUsingSettings (_gameVersion);
			}
		}
		#endregion

		#region Photon.PunBehaivour Callbacks

		public override void OnConnectedToMaster(){
			Debug.Log ("Connected to Master");
			if (isConnecting) {
				PhotonNetwork.JoinRandomRoom ();
			}
		}

		public override void OnDisconnectedFromPhoton(){
			progressLabel.SetActive (false);
			controlPanel.SetActive (true);
		}

		public override void OnPhotonRandomJoinFailed (object[] codeAndMsg)
		{
			Debug.Log ("RandomJoinFailed");
			PhotonNetwork.CreateRoom (null, new RoomOptions (){ MaxPlayers = MaxPlayersPerRoom }, null);
		}
		public override void OnJoinedRoom ()
		{
			Debug.Log ("Joined room");

			if (PhotonNetwork.room.PlayerCount == 1) {
				PhotonNetwork.LoadLevel ("Room for 1");
			}
		}

		#endregion
	}
}
