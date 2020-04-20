using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(InputSystem))]
public sealed class InputSystem : UpdateSystem
{

    private Filter filter;
    public override void OnAwake()
    {
        filter = World.Filter.With<Controller>().With<InputController>();
    }

    public override void OnUpdate(float deltaTime) {
        foreach (var entity in filter)
        {
            ref var controller = ref entity.GetComponent<Controller>();
            if (Input.GetKey(KeyCode.W))
            {
                controller.up = true;
            }
            else
            {
                controller.up = false;
            }
            if (Input.GetKey(KeyCode.S))
            {
                controller.down = true;
            }
            else
            {
                controller.down = false;
            }
            if (Input.GetKey(KeyCode.A))
            {
                controller.left = true;
            }
            else
            {
                controller.left = false;
            }
            if (Input.GetKey(KeyCode.D))
            {
                controller.right = true;
            }
            else
            {
                controller.right = false;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                controller.fire = true;
            }
            else
            {
                controller.fire = false;
            }
        }
    }
}