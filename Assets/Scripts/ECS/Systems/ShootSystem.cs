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
    private Filter filter;
    public GameObject prefab;
    
    public override void OnAwake() {
        filter = World.Filter.With<Translation>().With<Rotation>().With<Barrel>().With<Controller>();
    }

    public override void OnUpdate(float deltaTime) {
        foreach (var entity in filter)
        {
            ref var parentBarrel = ref entity.GetComponent<Barrel>();
            if(parentBarrel.lastShotRemain > parentBarrel.culldown)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    parentBarrel.lastShotRemain = 0;
                    var bullet = PhotonNetwork.Instantiate(prefab.name,Vector3.zero, Quaternion.identity);
                    ref var bulletTranslation = ref bullet.GetComponent<TranslationProvider>().GetData();
                    ref var bulletDirection = ref bullet.GetComponent<DirectionProvider>().GetData();
                    ref var bulletSpeed = ref bullet.GetComponent<SpeedProvider>().GetData();
                    ref var parentTranslation = ref entity.GetComponent<Translation>();
                    ref var parentDirection = ref entity.GetComponent<Rotation>();

                    bulletTranslation.x = parentTranslation.x;
                    bulletTranslation.y = parentTranslation.y;
                    bulletDirection.direction = parentDirection.direction;
                    bulletSpeed.value = parentBarrel.bulletSpeed;
                }
            }
            else
            {
                parentBarrel.lastShotRemain += deltaTime;
            } 
        }
    }
}