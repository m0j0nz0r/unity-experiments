using UnityEngine;
using UnityEngine.UI;
namespace testGame{
	/// <summary>
	/// Player name input field. Let the user input his name, will appear above the player in the game.
	/// </summary>
	[RequireComponent(typeof(InputField))]
	public class PlayerNameInputField : MonoBehaviour {
	
		#region Private variables

		//Store the playerPref Key to avoid typos
		static string playerNamePrefKey = "PlayerName";

		#endregion


		#region Monobehaviour CallBacks

		void Start(){

			string defaultName = "";

			InputField _inputField = this.GetComponent<InputField> ();

			if (_inputField != null) {
				if (PlayerPrefs.HasKey (playerNamePrefKey)) {
					defaultName = PlayerPrefs.GetString (playerNamePrefKey);
				}
			}

			_inputField.text = defaultName;

			PhotonNetwork.playerName = defaultName;
		}

		#endregion


		#region Public Methods

		///<summary>
		/// Sets the name of the player, and saves it in the PlayerPrefs for future sessions.
		/// </summary>
		/// <param name="value">The name of the Player </param>
		public void SetPlayerName(string value){
			PhotonNetwork.playerName = value + " ";

			PlayerPrefs.SetString (playerNamePrefKey, value);
		}

		#endregion
	}
}
