using Photon.Pun;
using UnityEngine;

public class AreaObserver : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private AreaProvider areaProvider;
    [SerializeField] private TranslationProvider translationProvider;
    [SerializeField] private DamagedState networkState;
    [SerializeField] private float networkXPosition;
    [SerializeField] private float networkYPosition;
    [SerializeField] private string networkAreaType;
    private void UpdateData()
    {
        ref var area = ref areaProvider.GetData();
        area.State = networkState;
        ref var translation = ref translationProvider.GetData();
        translation.x = networkXPosition;
        translation.y = networkYPosition;
        areaProvider.Entity.AddComponent<AreaUpdateIndicator>();
        if (networkAreaType==null || networkAreaType.Length <= 0) return;
        if (networkAreaType != area.areaType)
        {
            area.areaType = networkAreaType;
            areaProvider.Entity.AddComponent<AreaInitIndicator>();
        }
        
    }
    
    private void GrabData()
    {
        networkState = areaProvider.GetData().State;
        networkXPosition = translationProvider.GetData().x;
        networkYPosition = translationProvider.GetData().y;
        networkAreaType = areaProvider.GetData().areaType;
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsReading)
        {
            networkState = (DamagedState)stream.ReceiveNext();
            networkXPosition = (float) stream.ReceiveNext();
            networkYPosition = (float) stream.ReceiveNext();
            networkAreaType = (string) stream.ReceiveNext();
            UpdateData();
        }
        else if(stream.IsWriting)
        {
            GrabData();
            stream.SendNext(networkState);
            stream.SendNext(networkXPosition);
            stream.SendNext(networkYPosition);
            stream.SendNext(networkAreaType);
        }
    }
}
