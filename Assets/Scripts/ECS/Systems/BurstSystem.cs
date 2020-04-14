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
        bulletsBurst=World.Filter.With<Burst>();
    }

    public override void OnUpdate(float deltaTime) {
        foreach (var entity in bulletsBurst)
        {
            var burst = entity.GetComponent<Burst>();
            if (burst.isActive)
            {
                burst.Animator.SetTrigger("burst");
                Destroy(burst.GameObject,1f);
            }
        }
    }
}