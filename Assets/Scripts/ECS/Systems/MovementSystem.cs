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
        filterController = World.Filter.With<Translation>().With<Rotation>().With<Speed>().With<Controller>();
        filterBullets = World.Filter.With<Translation>().With<Rotation>().With<Speed>().Without<Controller>().Without<Collision>().With<BulletView>();
    }

    private void UnitsMove(float deltaTime)
    {
        foreach (var entity in filterController) {
            
            ref var translation = ref entity.GetComponent<Translation>();
            ref var direction = ref entity.GetComponent<Rotation>();
            ref var speed = ref entity.GetComponent<Speed>();
            
            if (Input.GetKey(KeyCode.UpArrow))
            {
                direction.direction = Direction.Up;
                translation.x = Closest(translation.x, 0.5f);
                if (entity.Has<Collision>())
                {
                    ref var collision = ref entity.GetComponent<Collision>();
                    if(HasCollisionInThisDirection(collision,Direction.Up))break;
                }
                translation.y += deltaTime * speed.value;
            }else if (Input.GetKey(KeyCode.DownArrow))
            {
                direction.direction = Direction.Down;
                translation.x = Closest(translation.x, 0.5f);
                if (entity.Has<Collision>())
                {
                    ref var collision = ref entity.GetComponent<Collision>();
                    if(HasCollisionInThisDirection(collision,Direction.Down))break;
                }
                translation.y -= deltaTime * speed.value;
            }else if (Input.GetKey(KeyCode.LeftArrow))
            {
                direction.direction = Direction.Left;
                translation.y = Closest(translation.y, 0.5f);
                if (entity.Has<Collision>())
                {
                    ref var collision = ref entity.GetComponent<Collision>();
                    if(HasCollisionInThisDirection(collision,Direction.Left))break;
                }
                translation.x -= deltaTime * speed.value;
            }else if (Input.GetKey(KeyCode.RightArrow))
            {
                direction.direction = Direction.Right;
                translation.y = Closest(translation.y, 0.5f);
                if (entity.Has<Collision>())
                {
                    ref var collision = ref entity.GetComponent<Collision>();
                    if(HasCollisionInThisDirection(collision,Direction.Right))break;
                }
                translation.x += deltaTime * speed.value;
            }
        }
    }

    private bool HasCollisionInThisDirection(Collision collision, Direction direction)
    {
        foreach (var item in collision.collisions)
        {
            if (item.collideWith.Has<Area>() && item.direction==direction)
            {
                return true;
            }
            if (item.collideWith.Has<Barrel>() && item.direction==direction)
            {
                return true;
            }
        }
        return false;
    }

    private void BulletsMove(float deltaTime)
    {
        foreach (var entity in filterBullets) {
            
            ref var translation = ref entity.GetComponent<Translation>();
            ref var rotation = ref entity.GetComponent<Rotation>();
            ref var speed = ref entity.GetComponent<Speed>();

            switch (rotation.direction)
            {
                case Direction.Up:
                    rotation.direction = Direction.Up;
                    translation.y -= -deltaTime * speed.value;
                    break;
                case Direction.Down:
                    rotation.direction = Direction.Down;
                    translation.y -= deltaTime * speed.value;
                    break;
                case Direction.Left:
                    rotation.direction = Direction.Left;
                    translation.x -= deltaTime * speed.value;
                    break;
                case Direction.Right:
                    rotation.direction = Direction.Right;
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