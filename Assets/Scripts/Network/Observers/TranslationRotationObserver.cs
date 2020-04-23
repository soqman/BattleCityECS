using System;
using Photon.Pun;
using UnityEngine;

public class TranslationRotationObserver : MonoBehaviourPun, IPunObservable, IPunInstantiateMagicCallback 
{
    [SerializeField] private TranslationProvider translationProvider;
    [SerializeField] private RotationProvider rotationProvider;
    [SerializeField] private SpeedProvider speedProvider;
    private float networkXPosition;
    private float networkYPosition;
    private Direction networkDirection;
    private float networkSpeed;
    private bool firstDataReceived;

    private void FixedUpdate()
    {
        if (photonView.IsMine || !firstDataReceived) return;
        switch (networkDirection)
        {
            case Direction.Left:
                networkXPosition -= networkSpeed * Time.fixedDeltaTime;
                break;
            case Direction.Right:
                networkXPosition += networkSpeed * Time.fixedDeltaTime;
                break;
            case Direction.Up:
                networkYPosition += networkSpeed * Time.fixedDeltaTime;
                break;
            case Direction.Down:
                networkYPosition -= networkSpeed * Time.fixedDeltaTime;
                break;
        }
        SetTranslation();
    }
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

    private void GetSpeed()
    {
        ref var speed = ref speedProvider.GetData();
        networkSpeed = speed.currentSpeed;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsReading)
        {
            networkXPosition = (float) stream.ReceiveNext();
            networkYPosition = (float) stream.ReceiveNext();
            networkDirection = (Direction) stream.ReceiveNext();
            networkSpeed = (float) stream.ReceiveNext();
            var lag = Mathf.Abs((float) (PhotonNetwork.Time - info.timestamp));
            switch (networkDirection)
            {
                case Direction.Left:
                    networkXPosition -= networkSpeed * lag;
                    break;
                case Direction.Right:
                    networkXPosition += networkSpeed * lag;
                    break;
                case Direction.Up:
                    networkYPosition += networkSpeed * lag;
                    break;
                case Direction.Down:
                    networkYPosition -= networkSpeed * lag;
                    break;
            }
            firstDataReceived = true;
            SetTranslation();
            SetRotation();
        }
        else if (stream.IsWriting)
        {
            GetTranslation();
            GetRotation();
            GetSpeed();
            stream.SendNext(networkXPosition);
            stream.SendNext(networkYPosition);
            stream.SendNext(networkDirection);
            stream.SendNext(networkSpeed);
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (info.photonView.InstantiationData == null) return;
        var position = (Vector2)info.photonView.InstantiationData[0];
        networkXPosition = position.x;
        networkYPosition = position.y;
        SetTranslation();
    }
}
