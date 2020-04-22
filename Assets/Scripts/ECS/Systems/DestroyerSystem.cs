using Morpeh;
using Photon.Pun;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(DestroyerSystem))]
public sealed class DestroyerSystem : UpdateSystem
{
    private Filter destroyerFilterBullets;
    private Filter destroyerFilterAreas;
    
    public override void OnAwake()
    {
        destroyerFilterBullets = World.Filter.With<Destroyer>().With<BulletView>();
        destroyerFilterAreas = World.Filter.With<Destroyer>().With<AreaView>();
    }

    public override void OnUpdate(float deltaTime) {
        foreach (var entity in destroyerFilterBullets)
        {
            ref var destroyer = ref entity.GetComponent<Destroyer>();
            ref var bulletView = ref entity.GetComponent<BulletView>();
            destroyer.timer -= deltaTime;
            if (destroyer.timer <= 0)
            {
                PhotonNetwork.Destroy(bulletView.GameObject);
            }
        }
        foreach (var entity in destroyerFilterAreas)
        {
            ref var destroyer = ref entity.GetComponent<Destroyer>();
            ref var areaView = ref entity.GetComponent<AreaView>();
            destroyer.timer -= deltaTime;
            if (destroyer.timer <= 0)
            {
                entity.RemoveComponent<Destroyer>();
                PhotonNetwork.Destroy(areaView.Transform.gameObject);
            }
        }
    }
}