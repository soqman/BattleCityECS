using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;

public class Lobby : MonoBehaviourPunCallbacks
    {
        [SerializeField] private string version;
        [SerializeField] private UnityEvent connectedToMasterEvent;
        [SerializeField] private UnityEvent disconnectedEvent;
        [SerializeField] private UnityEvent joinToRoomAsMasterEvent;
        [SerializeField] private UnityEvent joinToRoomAsClientEvent;
        [SerializeField] private UnityEvent readyToPlayEvent;
        [SerializeField] private UnityEvent notReadyToPlayEvent;
        private bool gameStarted;

        public bool GameStarted
        {
            get { return gameStarted; }
            set { gameStarted = value; }
        }

        private void Start()
        {
            ConnectToPhoton();
        }

        private void ConnectToPhoton()
        {
            if (PhotonNetwork.IsConnected) return;
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = version;
        }
        public override void OnConnectedToMaster()
        {
            connectedToMasterEvent.Invoke();
        }

        public override void OnJoinedRoom()
        {
              if (PhotonNetwork.IsMasterClient) joinToRoomAsMasterEvent.Invoke();
              else joinToRoomAsClientEvent.Invoke();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            CheckPlayerNums();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            CheckPlayerNums();
        }

        private void CheckPlayerNums()
        {
            if (!PhotonNetwork.IsMasterClient || gameStarted || !PhotonNetwork.IsConnectedAndReady) return;
            if (PhotonNetwork.InRoom && PhotonNetwork.PlayerList.Length >= 2)
            {
                readyToPlayEvent.Invoke();
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
            else
            {
                notReadyToPlayEvent.Invoke();
                PhotonNetwork.CurrentRoom.IsOpen = true;
            }
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (!gameStarted) return;
            if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
            gameStarted = false;
            connectedToMasterEvent.Invoke();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            disconnectedEvent.Invoke();
        }

        public void CreateGame(TMPro.TMP_InputField input)
        {
            if (input.text.Length == 0) return;
            PhotonNetwork.CreateRoom(input.text, new RoomOptions{MaxPlayers = 2, PlayerTtl=0}, TypedLobby.Default);
        }
        public void JoinGame(TMPro.TMP_InputField input)
        {
            if (input.text.Length == 0) return;
            PhotonNetwork.JoinRoom(input.text);
        }
    }
