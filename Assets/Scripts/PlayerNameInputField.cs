using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

/// Player name input field. Let the user input his name, will appear above the player in the game.
[RequireComponent(typeof(InputField))]
public class PlayerNameInputField : MonoBehaviour
{
    #region Private Constants
    const string playerNamePrefKey = "PlayerName";
    #endregion

    #region MonoBehaviour CallBacks
    // Start is called before the first frame update
    void Start()
    {
        string defaultName = string.Empty;
        InputField _inputField = this.GetComponent<InputField>();
        if (_inputField != null)
        {
            defaultName = PlayerPrefs.GetString(playerNamePrefKey);
            _inputField.text = defaultName;
        }

        PhotonNetwork.NickName = defaultName;
    }
    #endregion

    #region Public Methods
        /// Sets the name of the player, and save it in the PlayerPrefs for future sessions.
        public void SetPlayerName(string value){
            // #Important
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }
            PhotonNetwork.NickName = value;
            PlayerPrefs.SetString(playerNamePrefKey, value);
        }
    #endregion
}