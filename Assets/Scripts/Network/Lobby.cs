using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;

public class Lobby : MonoBehaviourPunCallbacks
    {
        [SerializeField] private string version;
        [SerializeField] private GameObject masterUnit;
        [SerializeField] private GameObject clientUnit;
        [SerializeField] private GameObject installerMaster;
        [SerializeField] private GameObject lobbyMenu;
        [SerializeField] private UnityEvent connectedToMasterEvent;
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
            Debug.Log("CONNECTED TO ROOM");
            lobbyMenu.SetActive(false);
            if (PhotonNetwork.IsMasterClient)installerMaster.SetActive(true);
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
            PhotonNetwork.Instantiate( masterUnit.name, new Vector3(-1,-6,0), Quaternion.identity);
            PhotonNetwork.Instantiate( clientUnit.name, new Vector3(1,6,0),Quaternion.Euler(0,0,180));
        }

        public void JoinGame()
        {
            PhotonNetwork.JoinRoom("TEST-ROOM");
        }
        
        
    }
