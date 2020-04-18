using Morpeh;
using Photon.Pun;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(ShootSystem))]
public sealed class ShootSystem : UpdateSystem
{
    private Filter filterController;
    private Filter filterNetworkController;
    public GameObject yellowPrefab;
    public GameObject greenPrefab;
    
    public override void OnAwake()
    {
        var common = World.Filter.With<Translation>().With<Rotation>().With<Barrel>().With<TeamComponent>();
        filterController = common.With<Controller>();
        filterNetworkController=common.With<NetworkControllerComponent>();
    }

    public override void OnUpdate(float deltaTime) {
        foreach (var entity in filterController)
        {
            ref var parentBarrel = ref entity.GetComponent<Barrel>();
            if(parentBarrel.lastShotRemain > parentBarrel.culldown)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    parentBarrel.lastShotRemain = 0;
                    ref var parentTeam = ref entity.GetComponent<TeamComponent>();
                    ref var parentTranslation = ref entity.GetComponent<Translation>();
                    ref var parentDirection = ref entity.GetComponent<Rotation>();
                    
                    var position = new Vector3(parentTranslation.x,parentTranslation.y,0);
                    var bullet = PhotonNetwork.Instantiate(parentTeam.team == Team.Green ? greenPrefab.name : yellowPrefab.name, position, Quaternion.identity);
                    ref var bulletTranslation = ref bullet.GetComponent<TranslationProvider>().GetData();
                    ref var bulletDirection = ref bullet.GetComponent<DirectionProvider>().GetData();
                    ref var bulletSpeed = ref bullet.GetComponent<SpeedProvider>().GetData();
                    ref var bulletCollider = ref bullet.GetComponent<ColliderProvider>().GetData();
                    
                    bulletTranslation.x = parentTranslation.x;
                    bulletTranslation.y = parentTranslation.y;
                    bulletDirection.direction = parentDirection.direction;
                    bulletSpeed.value = parentBarrel.bulletSpeed;
                    bulletCollider.exceptionEntity = entity.ID;
                }
            }
            else
            {
                parentBarrel.lastShotRemain += deltaTime;
            } 
        }
        foreach (var entity in filterNetworkController)
        {
            ref var parentBarrel = ref entity.GetComponent<Barrel>();
            ref var networkController = ref entity.GetComponent<NetworkControllerComponent>();
            if(parentBarrel.lastShotRemain > parentBarrel.culldown)
            {
                if (networkController.fire)
                {
                    parentBarrel.lastShotRemain = 0;
                    ref var parentTeam = ref entity.GetComponent<TeamComponent>();
                    ref var parentTranslation = ref entity.GetComponent<Translation>();
                    ref var parentDirection = ref entity.GetComponent<Rotation>();
                    
                    var position = new Vector3(parentTranslation.x,parentTranslation.y,0);
                    var bullet = PhotonNetwork.Instantiate(parentTeam.team == Team.Green ? greenPrefab.name : yellowPrefab.name, position, Quaternion.identity);
                    ref var bulletTranslation = ref bullet.GetComponent<TranslationProvider>().GetData();
                    ref var bulletDirection = ref bullet.GetComponent<DirectionProvider>().GetData();
                    ref var bulletSpeed = ref bullet.GetComponent<SpeedProvider>().GetData();
                    ref var bulletCollider = ref bullet.GetComponent<ColliderProvider>().GetData();
                    
                    bulletTranslation.x = parentTranslation.x;
                    bulletTranslation.y = parentTranslation.y;
                    bulletDirection.direction = parentDirection.direction;
                    bulletSpeed.value = parentBarrel.bulletSpeed;
                    bulletCollider.exceptionEntity = entity.ID;
                }
            }
            else
            {
                parentBarrel.lastShotRemain += deltaTime;
            } 
        }
    }
}