using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(RespawnerSystem))]
public sealed class RespawnerSystem : UpdateSystem
{
    private Filter tanks;
    
    
    public override void OnAwake()
    {
        tanks = World.Filter.With<Respawn>().With<TankResetIndicator>().With<Translation>().With<Rotation>().With<Health>().With<TankView>();
    }

    public override void OnUpdate(float deltaTime)
    {
        RespawnTanks(deltaTime);
    }

    private void RespawnTanks(float deltaTime)
    {
        foreach (var entity in tanks)
        {
            /*ref var respawn = ref entity.GetComponent<Respawn>();
            if (respawn.currentTime == respawn.invisibleTime)
            {
                ref var translation = ref entity.GetComponent<Translation>();
                ref var rotation = ref entity.GetComponent<Rotation>();
                ref var health = ref entity.GetComponent<Health>();
                ref var tankView = ref entity.GetComponent<TankView>();
                translation.x = respawn.x;
                translation.y = respawn.y;
                rotation.direction = respawn.direction;
                health.value = health.max;
                //tankView.NetworkAnimator.SetTrigger("reset");
            }
            respawn.currentTime -= deltaTime;
            if (respawn.currentTime <= 0)
            {
                ref var collider = ref entity.GetComponent<Collider>();
                collider.isActive = true;
                respawn.currentTime = respawn.invisibleTime;
                entity.RemoveComponent<TankResetIndicator>();
            }*/
        }
    }
}