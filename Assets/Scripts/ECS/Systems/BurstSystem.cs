﻿using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using UnityEngine.UI;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(BurstSystem))]
public sealed class BurstSystem : UpdateSystem
{
    private Filter bulletsBurst;
    private Filter tankBurst;
    
    public override void OnAwake() {
        bulletsBurst=World.Filter.With<Collision>().With<BulletView>();
        tankBurst = World.Filter.With<Health>().With<TankView>().Without<TankResetIndicator>();
    }

    public override void OnUpdate(float deltaTime) {
        TanksUpdate();
        BulletsUpdate();
    }

    private void TanksUpdate()
    {
        foreach (var entity in tankBurst)
        {
            ref var tankView = ref entity.GetComponent<TankView>();
            ref var health = ref entity.GetComponent<Health>();
            if (health.value == 0)
            {
                tankView.Animator.SetTrigger("burst");
                if(entity.Has<Collider>())entity.RemoveComponent<Collider>();
                entity.AddComponent<TankResetIndicator>();
            }
        }
    }

    private void BulletsUpdate()
    {
        foreach (var entity in bulletsBurst)
        {
            ref var bulletView = ref entity.GetComponent<BulletView>();
            if (entity.Has<Collider>()) entity.RemoveComponent<Collider>();
            bulletView.Animator.SetTrigger("burst");
            Destroy(bulletView.GameObject,1f);
        }
    }
}