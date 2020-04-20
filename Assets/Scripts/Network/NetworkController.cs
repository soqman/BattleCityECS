using Photon.Pun;
using UnityEngine;

public class NetworkController : MonoBehaviourPun{
    
    [SerializeField] private ControllerProvider controllerProvider;
    [PunRPC]
    public void NetworkInput(bool up, bool down, bool left, bool right, bool fire)
    {
        ref var controller=ref controllerProvider.GetData();
        controller.up = up;
        controller.down = down;
        controller.left = left;
        controller.right = right;
        controller.fire = fire;
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient) return;
        if (Input.GetKeyUp(KeyCode.W) || 
            Input.GetKeyDown(KeyCode.W) || 
            Input.GetKeyUp(KeyCode.S)||
            Input.GetKeyDown(KeyCode.S)||
            Input.GetKeyUp(KeyCode.A)||
            Input.GetKeyDown(KeyCode.A)||
            Input.GetKeyUp(KeyCode.D)||
            Input.GetKeyDown(KeyCode.D)||
            Input.GetKeyUp(KeyCode.Space)||
            Input.GetKeyDown(KeyCode.Space))
        {
            photonView.RPC("NetworkInput",RpcTarget.MasterClient,Input.GetKey(KeyCode.W),Input.GetKey(KeyCode.S),Input.GetKey(KeyCode.A),Input.GetKey(KeyCode.D),Input.GetKey(KeyCode.Space));
        }
    }
}
