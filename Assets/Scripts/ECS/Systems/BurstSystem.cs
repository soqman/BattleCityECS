using Morpeh;
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
    
    public override void OnAwake() {
        bulletsBurst=World.Filter.With<Collision>().With<BulletView>();
    }

    public override void OnUpdate(float deltaTime) {
        BurstUpdate();
    }

    private void BurstUpdate()
    {
        foreach (var entity in bulletsBurst)
        {
            var bulletView = entity.GetComponent<BulletView>();
            bulletView.Animator.SetTrigger("burst");
            Destroy(bulletView.GameObject,1f);
        }
    }
}