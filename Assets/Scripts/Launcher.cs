using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    #region Private Serializable Fields
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.")]
    [SerializeField]
    private byte maxPlayersPerRoom = 4;
    #endregion

    #region Public Fields
    [Tooltip("The UI Panel to let the user enter name, connect and play.")]
    [SerializeField]
    private GameObject controlPanel;
    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    [SerializeField]
    private GameObject progressLabel;
    #endregion

    #region Private Fields
    // This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
    string gameVersion = "1";
    /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
    /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
    /// Typically this is used for the OnConnectedToMaster() callback.
    bool isConnecting; 
    #endregion

    #region MonoBehaviour CallBacks
    // MonoBehaviour method called on GameObject by Unity during early initialization phase.
    private void Awake() {
        // Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all 
        // clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    // MonoBehaviour method called on GameObject by Unity during initialization phase.
    void Start()
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }
    #endregion

    #region Public Methods
    // Start connection process.
    //  - If already connected, attempt to join a random room.
    //  - If not yet connected, Connect this application instance to the Photon Cloud Network
    public void Connect(){
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);

        // we check if we are connected or not, we join if we are,
        // else we initiate the connection to the server.
        if(PhotonNetwork.IsConnected){
            // Critical we need at this point to attempt joining random room.
            // If it fails, we'll get notified in OnJoinRandomFailed(),
            // Then we will create a new room and join it.
            if (isConnecting)
            {
                PhotonNetwork.JoinRandomRoom();
                isConnecting = false;
            }
            PhotonNetwork.JoinRandomRoom();
        }else{
            // Critical, we must first and foremost connect to Photon Online Server.
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            // Set the gameVersion.
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random available, so we create one.\ncalling: PhotonNetwork.CreateRoom");
        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions{MaxPlayers = maxPlayersPerRoom});

    }

    public override void OnJoinedRoom()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("We load the 'Room for 1'");

            // #Critical
            // Load the Room Level
            PhotonNetwork.LoadLevel("Room for 1");
        }
    }
    #endregion

    #region MonoBehaviourPunCallbacks Callbacks
    public override void OnConnectedToMaster()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was callled by PUN");
        // #Critical: The first we try to do is to join a potential existing room. If there is, good, else we'll be called back with OnJoinedRandomFailed()
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);
        isConnecting = false;
    }
    #endregion
}
