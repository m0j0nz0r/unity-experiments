using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace testGame{
	public class GameManager : Photon.PunBehaviour {
		#region Public Properties

		static public GameManager Instance;

		#endregion

		#region Photon Messages

		///<summary>
		/// Called when the local player left the room. We need to load the launcher scene.
		/// </summary>
		public override void OnLeftRoom ()
		{
			SceneManager.LoadScene (0);
		}
		public override void OnPhotonPlayerConnected (PhotonPlayer newPlayer)
		{
			Debug.Log ("Player connected: " + newPlayer.NickName);

			if (PhotonNetwork.isMasterClient) {
				LoadArena ();
			}
		}

		public override void OnPhotonPlayerDisconnected (PhotonPlayer otherPlayer)
		{
			Debug.Log ("Player disconnected: " + otherPlayer.NickName);

			if (PhotonNetwork.isMasterClient) {
				LoadArena ();
			}
		}

		#endregion


		#region Public Methods

		public void LeaveRoom(){
			PhotonNetwork.LeaveRoom();
		}

		#endregion


		#region Private Methods

		void LoadArena(){
			if (!PhotonNetwork.isMasterClient) {
				Debug.Log ("Trying to load level, but we are not the master client");
			}
			Debug.Log ("Loading level...");
			PhotonNetwork.LoadLevel ("Room for 1");
		}

		#endregion

		void Start(){
			Instance = this;
		}

	}
}
