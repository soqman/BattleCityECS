using System.Collections.Generic;
using Morpeh;
using Photon.Pun;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(ViewUpdaterSystem))]
public sealed class ViewUpdaterSystem : UpdateSystem
{
    private Filter tankFilter;
    private Filter bulletFilter;
    private Filter areaFilter;
    private Filter areaInitFilter;
    [SerializeField] private List<AreaType> areaTypes;
    public override void OnAwake()
    {
        tankFilter=World.Filter.With<Translation>().With<Rotation>().With<TankView>();
        bulletFilter=World.Filter.With<Translation>().With<BulletView>().With<Rotation>();
        areaFilter = World.Filter.With<Translation>().With<AreaView>().With<Area>().With<AreaUpdateIndicator>();
        areaInitFilter = areaFilter.With<AreaInitIndicator>();
    }

    public override void OnUpdate(float deltaTime)
    {
        InitAreas();
        UpdateAreas();
        UpdateTanks();
        UpdateBullets();
    }

    private void UpdateTanks()
    {
        foreach (var entity in tankFilter)
        {
            ref var translation = ref entity.GetComponent<Translation>();
            ref var direction = ref entity.GetComponent<Rotation>();
            ref var tankView = ref entity.GetComponent<TankView>();
            tankView.Transform.position=new Vector3(translation.x,translation.y,0);
            switch (direction.direction)
            {
                case Direction.Up:
                    tankView.Transform.localRotation=Quaternion.Euler(0,0,0);
                    break;
                case Direction.Down:
                    tankView.Transform.localRotation=Quaternion.Euler(0,0,180);
                    break;
                case Direction.Left:
                    tankView.Transform.localRotation=Quaternion.Euler(0,0,90);
                    break;
                case Direction.Right:
                    tankView.Transform.localRotation=Quaternion.Euler(0,0,-90);
                    break;
            }
            if (PhotonNetwork.IsConnectedAndReady && !PhotonNetwork.IsMasterClient) continue;
            if(entity.Has<Engine>())tankView.NetworkAnimator.SetBool("on",true);
            else tankView.NetworkAnimator.SetBool("on",false);
        }
    }

    private void UpdateBullets()
    {
        if (PhotonNetwork.IsConnectedAndReady && !PhotonNetwork.IsMasterClient) return;
        foreach (var entity in bulletFilter)
        {
            var translation = entity.GetComponent<Translation>();
            var bulletView = entity.GetComponent<BulletView>();
            var direction = entity.GetComponent<Rotation>();
            bulletView.Transform.position=new Vector3(translation.x,translation.y,0);
            switch (direction.direction)
            {
                case Direction.Up:
                    bulletView.Transform.localRotation=Quaternion.Euler(0,0,0);
                    break;
                case Direction.Down:
                    bulletView.Transform.localRotation=Quaternion.Euler(0,0,180);
                    break;
                case Direction.Left:
                    bulletView.Transform.localRotation=Quaternion.Euler(0,0,90);
                    break;
                case Direction.Right:
                    bulletView.Transform.localRotation=Quaternion.Euler(0,0,-90);
                    break;
            }
        }
    }

    private void InitAreas()
    {
        foreach (var entity in areaInitFilter)
        {
            ref var areaView = ref entity.GetComponent<AreaView>();
            var area = entity.GetComponent<Area>();
            var areaType=areaTypes.Find(x => x.name == area.areaType);
            if (areaType != null)
            {
                areaView.wholeSprite = areaType.wholeSprite;
                areaView.leftSprite = areaType.leftSprite;
                areaView.rightSprite = areaType.rightSprite;
                areaView.downSprite = areaType.downSprite;
                areaView.upSprite = areaType.upSprite;
                areaView.leftUpSprite = areaType.leftUpSprite;
                areaView.rightUpSprite = areaType.rightUpSprite;
                areaView.leftDownSprite = areaType.leftDownSprite;
                areaView.rightDownSprite = areaType.rightDownSprite;
                areaView.destroyed = areaType.destroyed;
            }
            entity.RemoveComponent<AreaInitIndicator>();
        }
    }
    
    private void UpdateAreas()
    {
        foreach (var entity in areaFilter)
        {
            var translation = entity.GetComponent<Translation>();
            var areaView = entity.GetComponent<AreaView>();
            var area = entity.GetComponent<Area>();
            areaView.Transform.position=new Vector3(translation.x,translation.y,0);
            switch (area.State)
            {
                case DamagedState.Whole:
                    areaView.spriteRenderer.sprite = areaView.wholeSprite;
                    break;
                case DamagedState.Left:
                    areaView.spriteRenderer.sprite = areaView.leftSprite;
                    break;
                case DamagedState.Right:
                    areaView.spriteRenderer.sprite = areaView.rightSprite;
                    break;
                case DamagedState.Up:
                    areaView.spriteRenderer.sprite = areaView.upSprite;
                    break;
                case DamagedState.Down:
                    areaView.spriteRenderer.sprite = areaView.downSprite;
                    break;
                case DamagedState.LeftUp:
                    areaView.spriteRenderer.sprite = areaView.leftUpSprite;
                    break;
                case DamagedState.LeftDown:
                    areaView.spriteRenderer.sprite = areaView.leftDownSprite;
                    break;
                case DamagedState.RightUp:
                    areaView.spriteRenderer.sprite = areaView.rightUpSprite;
                    break;
                case DamagedState.RightDown:
                    areaView.spriteRenderer.sprite = areaView.rightDownSprite;
                    break;
                case DamagedState.Destroyed:
                    areaView.spriteRenderer.sprite = null;
                    break;
            }
            entity.RemoveComponent<AreaUpdateIndicator>();
        }
    }
}