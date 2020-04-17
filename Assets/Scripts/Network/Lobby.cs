using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;

public class Lobby : MonoBehaviourPunCallbacks
    {
        [SerializeField] private string version;
        [SerializeField] private GameObject unit;
        [SerializeField] private GameObject installerECS;
        [SerializeField] private GameObject lobbyMenu;
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
            Debug.Log("CONNECTED TO PHOTON");
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("CONNECTED TO ROOM");
            lobbyMenu.SetActive(false);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            
        }

        public void CreateGame()
        {
            PhotonNetwork.NickName = "PLAYER"+Random.Range(0,100);
            PhotonNetwork.CreateRoom("TEST-ROOM", new RoomOptions(){MaxPlayers = 4}, TypedLobby.Default);
        }

        public override void OnCreatedRoom()
        {
            installerECS.SetActive(true);
            PhotonNetwork.Instantiate( unit.name, Vector3.zero, Quaternion.identity);
        }

        public void JoinGame()
        {
            PhotonNetwork.JoinRoom("TEST-ROOM");
        }
    }
