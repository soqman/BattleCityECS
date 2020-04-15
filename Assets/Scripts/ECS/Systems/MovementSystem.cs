using System;
using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(MovementSystem))]
public sealed class MovementSystem : UpdateSystem
{
    private Filter filterController;
    private Filter filterBullets;
    
    public override void OnAwake() {
        filterController = World.Filter.With<Translation>().With<Direction>().With<Speed>().With<Controller>().With<Engine>().Without<Collision>();
        filterBullets = World.Filter.With<Translation>().With<Direction>().With<Speed>().Without<Controller>().Without<Collision>();
    }

    private void UnitsMove(float deltaTime)
    {
        foreach (var entity in filterController) {
            
            ref var translation = ref entity.GetComponent<Translation>();
            ref var direction = ref entity.GetComponent<Direction>();
            ref var speed = ref entity.GetComponent<Speed>();
            
            if (Input.GetKey(KeyCode.UpArrow))
            {
                direction.lookAtDirection = LookAtDirection.Up;
                translation.x = Closest(translation.x, 0.5f);
                translation.y += deltaTime * speed.value;
            }else if (Input.GetKey(KeyCode.DownArrow))
            {
                direction.lookAtDirection = LookAtDirection.Down;
                translation.x = Closest(translation.x, 0.5f);
                translation.y -= deltaTime * speed.value;
            }else if (Input.GetKey(KeyCode.LeftArrow))
            {
                direction.lookAtDirection = LookAtDirection.Left;
                translation.y = Closest(translation.y, 0.5f);
                translation.x -= deltaTime * speed.value;
            }else if (Input.GetKey(KeyCode.RightArrow))
            {
                direction.lookAtDirection = LookAtDirection.Right;
                translation.y = Closest(translation.y, 0.5f);
                translation.x += deltaTime * speed.value;
            }
        }
    }

    private void BulletsMove(float deltaTime)
    {
        foreach (var entity in filterBullets) {
            
            ref var translation = ref entity.GetComponent<Translation>();
            ref var direction = ref entity.GetComponent<Direction>();
            ref var speed = ref entity.GetComponent<Speed>();

            switch (direction.lookAtDirection)
            {
                case LookAtDirection.Up:
                    direction.lookAtDirection = LookAtDirection.Up;
                    translation.y -= -deltaTime * speed.value;
                    break;
                case LookAtDirection.Down:
                    direction.lookAtDirection = LookAtDirection.Down;
                    translation.y -= deltaTime * speed.value;
                    break;
                case LookAtDirection.Left:
                    direction.lookAtDirection = LookAtDirection.Left;
                    translation.x -= deltaTime * speed.value;
                    break;
                case LookAtDirection.Right:
                    direction.lookAtDirection = LookAtDirection.Right;
                    translation.x -= -deltaTime * speed.value;
                    break;
            }
        }
    }
    
    public override void OnUpdate(float deltaTime)
    {
        UnitsMove(deltaTime);
        BulletsMove(deltaTime);
    }
    
    private float Closest(float src, float divider)
    {
        var mod = src % divider;
        if (mod <= -divider / 2)
        {
            return src - mod - divider;
        }
        if (mod >= divider / 2)
        {
            return src - mod + divider;
        }
        return src - mod;
    }
}