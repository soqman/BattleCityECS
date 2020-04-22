using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(RespawnerSystem))]
public sealed class RespawnerSystem : UpdateSystem
{
    [SerializeField] private float invincibleTime;
    private Filter tanks;
    
    
    public override void OnAwake()
    {
        tanks = World.Filter.With<Respawn>().With<TankResetIndicator>().With<Translation>().With<Rotation>().With<Health>().With<TankView>().With<Collider>();
    }

    public override void OnUpdate(float deltaTime)
    {
        RespawnTanks(deltaTime);
    }

    private void RespawnTanks(float deltaTime)
    {
        foreach (var entity in tanks)
        {
            ref var respawn = ref entity.GetComponent<Respawn>();
            respawn.currentTime += deltaTime;
            if (respawn.currentTime >= respawn.delay)
            {
                Respawn(entity,ref respawn);
            }
        }
    }

    private void Respawn(IEntity entity, ref Respawn respawn)
    {
        ref var translation = ref entity.GetComponent<Translation>();
        ref var rotation = ref entity.GetComponent<Rotation>();
        ref var health = ref entity.GetComponent<Health>();
        ref var tankView = ref entity.GetComponent<TankView>();
        ref var collider = ref entity.GetComponent<Collider>();
        collider.isActive = true;
        respawn.currentTime = 0;
        translation.x = respawn.x;
        translation.y = respawn.y;
        rotation.direction = respawn.direction;
        health.value = health.max;
        tankView.NetworkAnimator.SetTrigger("reset");
        entity.RemoveComponent<TankResetIndicator>();
        entity.AddComponent<Controller>();
        ref var invincible = ref entity.AddComponent<Invincible>();
        invincible.maxTimer = invincibleTime;
    }
}