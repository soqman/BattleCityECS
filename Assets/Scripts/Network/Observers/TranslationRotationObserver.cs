using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TranslationRotationObserver : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private TranslationProvider translationProvider;
    [SerializeField] private RotationProvider rotationProvider;
    private float networkXPosition;
    private float networkYPosition;
    private Direction networkDirection;
    private void GetTranslation()
    {
        ref var translation = ref translationProvider.GetData();
        networkXPosition = translation.x;
        networkYPosition = translation.y;
    }

    private void SetTranslation()
    {
        ref var translation = ref translationProvider.GetData();
        translation.x = networkXPosition;
        translation.y = networkYPosition;
    }

    private void GetRotation()
    {
        ref var rotation = ref rotationProvider.GetData();
        networkDirection = rotation.direction;
    }

    private void SetRotation()
    {
        ref var rotation = ref rotationProvider.GetData();
        rotation.direction = networkDirection;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsReading)
        {
            networkXPosition = (float) stream.ReceiveNext();
            networkYPosition = (float) stream.ReceiveNext();
            networkDirection = (Direction) stream.ReceiveNext();
            SetTranslation();
            SetRotation();
        }
        else if (stream.IsWriting)
        {
            GetTranslation();
            GetRotation();
            stream.SendNext(networkXPosition);
            stream.SendNext(networkYPosition);
            stream.SendNext(networkDirection);
        }
    }
}
