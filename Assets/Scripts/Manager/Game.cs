using System.Collections;
using System.Timers;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

public class Game : Singleton<Game>, IOnEventCallback
{
    private int readyPlayers;
    [SerializeField] private GameObject yellowUnit;
    [SerializeField] private GameObject greenUnit;
    [SerializeField] private UnityEvent startGameEvent;
    [SerializeField] private UnityEvent stopGameEvent;
    [SerializeField] private UnityEvent readyEvent;
    [SerializeField] private UnityEvent winYellowEvent;
    [SerializeField] private UnityEvent winGreenEvent;
    [SerializeField] private GameObject masterInstaller;
    [SerializeField] private float startGameTimer;
    private const byte readyToPlayNetworkEvent = 11;
    private const byte gameStartNetworkEvent = 12;
    private const byte gameStopNetworkEvent = 13;
    private const byte winYellowNetworkEvent = 14;
    private const byte winGreenNetworkEvent = 15;
    private readonly RaiseEventOptions raiseOptionsToMaster = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
    private readonly RaiseEventOptions raiseOptionsToClients = new RaiseEventOptions { Receivers = ReceiverGroup.Others};
    private readonly SendOptions sendOptions = new SendOptions { Reliability = true };
    private bool gameIsOver;
    public static bool inputActive;
  
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
            PhotonNetwork.RaiseEvent(readyToPlayNetworkEvent, null, raiseOptionsToMaster, sendOptions);
        }
    }

    public void BaseDestroyed(bool yellowWin)
    {
        if (gameIsOver) return;
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.RaiseEvent(yellowWin?winYellowNetworkEvent:winGreenNetworkEvent, null, raiseOptionsToClients, sendOptions);
        }
        if (yellowWin)
        {
            winYellowEvent.Invoke();
        }
        else
        {
            winGreenEvent.Invoke();
        }
        gameIsOver = true;
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
            PhotonNetwork.RaiseEvent(gameStartNetworkEvent, null, raiseOptionsToClients, sendOptions);
            readyPlayers = 0;
        }
        startGameEvent.Invoke();
        gameIsOver = false;
        StartCoroutine(StartTimer());
    }

    private IEnumerator StartTimer()
    {
        inputActive = false;
        yield return new WaitForSeconds(startGameTimer);
        inputActive = true;
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
        if(photonEvent.Code == readyToPlayNetworkEvent) ReadyToPlayRemote();
        if(photonEvent.Code == gameStartNetworkEvent) StartGame();
        if(photonEvent.Code == gameStopNetworkEvent) StopGame();
        if(photonEvent.Code == winYellowNetworkEvent) BaseDestroyed(true);
        if(photonEvent.Code == winGreenNetworkEvent) BaseDestroyed(false);
    }

    public void StopGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
            masterInstaller.SetActive(false);
            PhotonNetwork.RaiseEvent(gameStopNetworkEvent, null, raiseOptionsToClients, sendOptions);
        }
        PhotonNetwork.LeaveRoom();
        readyPlayers = 0;
        stopGameEvent.Invoke();
    }
}
