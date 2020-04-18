using Photon.Pun;
using UnityEngine;

public class NetworkController : MonoBehaviourPun{
    
    [SerializeField] private NetworkControllerProvider networkControllerProvider;
    [PunRPC]
    public void NetworkInput(bool up, bool down, bool left, bool right, bool fire)
    {
        ref var networkController=ref networkControllerProvider.GetData();
        networkController.up = up;
        networkController.down = down;
        networkController.left = left;
        networkController.right = right;
        networkController.fire = fire;
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient) return;
        if (Input.GetKeyUp(KeyCode.UpArrow) || 
            Input.GetKeyDown(KeyCode.UpArrow) || 
            Input.GetKeyUp(KeyCode.DownArrow)||
            Input.GetKeyDown(KeyCode.DownArrow)||
            Input.GetKeyUp(KeyCode.LeftArrow)||
            Input.GetKeyDown(KeyCode.LeftArrow)||
            Input.GetKeyUp(KeyCode.RightArrow)||
            Input.GetKeyDown(KeyCode.RightArrow)||
            Input.GetKeyUp(KeyCode.Space)||
            Input.GetKeyDown(KeyCode.Space))
        {
            photonView.RPC("NetworkInput",RpcTarget.MasterClient,Input.GetKey(KeyCode.UpArrow),Input.GetKey(KeyCode.DownArrow),Input.GetKey(KeyCode.LeftArrow),Input.GetKey(KeyCode.RightArrow),Input.GetKey(KeyCode.Space));
        }
    }
}
