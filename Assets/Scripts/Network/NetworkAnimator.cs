using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public class NetworkAnimator : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private Animator animator;
    [ShowInInspector] private bool networkOn;
    [SerializeField] private string onKey = "on";
    [SerializeField] private string resetKey = "reset";
    [SerializeField] private string burstKey = "burst";

    [PunRPC]
    private void SetBurstTriggerRPC()
    {
        animator.SetTrigger(burstKey);
    }
    
    [PunRPC]
    private void SetResetTriggerRPC()
    {
        animator.SetTrigger(resetKey);
    }

    public void SetTrigger(string trigger)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if(trigger==burstKey)photonView.RPC("SetBurstTriggerRPC", RpcTarget.All);
        if(trigger==resetKey)photonView.RPC("SetResetTriggerRPC", RpcTarget.All);
    }

    private void UpdateBoolValues()
    {
        animator.SetBool(onKey,networkOn);
    }

    public void SetBool(string key, bool value)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (key == onKey)
        {
            animator.SetBool(onKey,value);
            networkOn = value;
        }
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsReading)
        {
            networkOn = (bool) stream.ReceiveNext();
            UpdateBoolValues();
        }
        else if (stream.IsWriting)
        {
            stream.SendNext(networkOn);
        }
    }
}
