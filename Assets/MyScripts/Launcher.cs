#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

namespace com.Ale.Chess
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields

        /// <summary>
        /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
        /// </summary>
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;

        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        [SerializeField]
        private GameObject controlPanel;
        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        [SerializeField]
        private Text progressLabel;
        [SerializeField]
        private Text playerStatus;
        [SerializeField]
        private GameObject buttonLoadArena;
        [SerializeField]
        private GameObject buttonJoinRoom;

        [SerializeField]
        private InputField playerNameField;
        [SerializeField]
        private InputField roomNameField;

        #endregion


        #region Private Fields


        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        /// </summary>
        string gameVersion = "1";


        #endregion


        #region MonoBehaviour CallBacks


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Connecting to Photon Network");
            controlPanel.SetActive(false);
            buttonLoadArena.SetActive(false);
            playerStatus.text = "Player Status";
            Connect();
        }


        #endregion


        #region Public Methods


        /// <summary>
        /// Start the connection process.
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        public void Connect()
        {
            progressLabel.text = "Connecting...";
            PhotonNetwork.GameVersion = gameVersion; //1
            PhotonNetwork.ConnectUsingSettings(); //2
        }

        public void LoadArena()
        {
            // 5
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                PhotonNetwork.LoadLevel("ARScene");
            }
            else
            {
                playerStatus.text = "2 Players required to Load Arena!";
            }
        }

        public void JoinRoom()
        {
            string roomName = roomNameField.text;
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("PhotonNetwork.IsConnected! | Trying to Create/Join Room " + roomName);
                RoomOptions roomOptions = new RoomOptions(); //2
                TypedLobby typedLobby = new TypedLobby(roomName, LobbyType.Default); //3
                PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions {MaxPlayers = maxPlayersPerRoom} , typedLobby); //4
            }
        }

        #endregion

        #region MonoBehaviourPunCallbacks Callbacks

        //shows the connection to photon network
        public override void OnConnected()
        {
            // 1
            base.OnConnected();
            // 2
            progressLabel.text = "Connected to Photon!";
            progressLabel.color = Color.green;
            controlPanel.SetActive(true);
            buttonLoadArena.SetActive(false);
        }


        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
            if (PhotonNetwork.IsMasterClient)
            {
                buttonLoadArena.SetActive(true);
                buttonJoinRoom.SetActive(false);
                playerStatus.text = playerNameField.text + " ,you are the Lobby Leader";
            }
            else
            {
                playerStatus.text = playerNameField.text + " ,you are connected to Lobby";
            }
        }

        #endregion
    }
}

