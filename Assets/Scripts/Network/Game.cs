using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

public class Game : MonoBehaviour, IOnEventCallback
{
    private int readyPlayers;
    [SerializeField] private GameObject yellowUnit;
    [SerializeField] private GameObject greenUnit;
    [SerializeField] private UnityEvent startGameEvent;
    [SerializeField] private UnityEvent readyEvent;
    [SerializeField] private GameObject masterInstaller;
    private const byte readyToPlayEvent = 1;
    private const byte gameStartEvent = 2;
    private readonly RaiseEventOptions raiseOptionsToMaster = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
    private readonly RaiseEventOptions raiseOptionsToClients = new RaiseEventOptions { Receivers = ReceiverGroup.Others};
    private readonly SendOptions sendOptions = new SendOptions { Reliability = true };
  
    private void ReadyToPlayRemote()
    {
        readyPlayers++;
        CheckPlayersCount();
    }

    public void Play()
    {
        readyEvent.Invoke();
        if (PhotonNetwork.IsMasterClient)
        {
            readyPlayers++;
            CheckPlayersCount();
        }
        else
        {
            PhotonNetwork.RaiseEvent(readyToPlayEvent, null, raiseOptionsToMaster, sendOptions);
        }
    }

    private void CheckPlayersCount()
    {
        if (PhotonNetwork.PlayerList.Length <= readyPlayers)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            masterInstaller.SetActive(true);
            PhotonNetwork.Instantiate(yellowUnit.name, Vector3.zero, Quaternion.identity);
            PhotonNetwork.Instantiate(greenUnit.name, Vector3.zero, Quaternion.identity);
            PhotonNetwork.RaiseEvent(gameStartEvent, null, raiseOptionsToClients, sendOptions);
        }
        startGameEvent.Invoke();
    }

    public void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    
    public void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == readyToPlayEvent) ReadyToPlayRemote();
        if(photonEvent.Code == gameStartEvent) StartGame();
    }
}
