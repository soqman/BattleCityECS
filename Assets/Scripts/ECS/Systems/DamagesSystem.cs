using System;
using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(DamagesSystem))]
public sealed class DamagesSystem : UpdateSystem
{
    private Filter areasFilter;
    public override void OnAwake()
    {
        areasFilter = World.Filter.With<Collision>().With<Area>();
    }

    public override void OnUpdate(float deltaTime) {
        foreach (var entity in areasFilter)
        {
            ref var area = ref entity.GetComponent<Area>();
            ref var collision = ref entity.GetComponent<Collision>();
            foreach (var collisionItem in collision.collisions)
            {
                area.State=AddDamage(area.State,collisionItem.direction);
                if (entity.Has<Collider>())
                {
                    UpdateCollider(area.State,ref entity.GetComponent<Collider>());
                    if (area.State == DamagedState.Destroyed) entity.RemoveComponent<Collider>();
                }
                entity.AddComponent<AreaUpdateIndicator>();
            }
        }
    }

    private void UpdateCollider(DamagedState state, ref Collider collider)
    {
        switch (state)
        {
            case DamagedState.Left:
                break;
            case DamagedState.Right:
                break;
            case DamagedState.Up:
                break;
            case DamagedState.Down:
                break;
            case DamagedState.LeftUp:
                break;
            case DamagedState.LeftDown:
                break;
            case DamagedState.RightUp:
                break;
            case DamagedState.RightDown:
                break;
        }
    }

    private DamagedState AddDamage(DamagedState damage, Direction direction)
    {
        switch (damage)
        {
            case DamagedState.Whole:
                switch (direction)
                {
                    case Direction.Left:
                        return DamagedState.Right;
                    case Direction.Right:
                        return DamagedState.Left;
                    case Direction.Up:
                        return DamagedState.Down;
                    case Direction.Down:
                        return DamagedState.Up;
                }
                break;
            case DamagedState.Left:
                switch (direction)
                {
                    case Direction.Left:
                    case Direction.Right:
                        return DamagedState.Destroyed;
                    case Direction.Up:
                        return DamagedState.LeftDown;
                    case Direction.Down:
                        return DamagedState.LeftUp;
                }
                break;
            case DamagedState.Right:
                switch (direction)
                {
                    case Direction.Left:
                    case Direction.Right:
                        return DamagedState.Destroyed;
                    case Direction.Up:
                        return DamagedState.RightDown;
                    case Direction.Down:
                        return DamagedState.RightUp;
                }
                break;
            case DamagedState.Up:
                switch (direction)
                {
                    case Direction.Left:
                        return DamagedState.RightUp;
                    case Direction.Right:
                        return DamagedState.LeftUp;
                    case Direction.Up:
                    case Direction.Down:
                        return DamagedState.Destroyed;
                }
                break;
            case DamagedState.Down:
                switch (direction)
                {
                    case Direction.Left:
                        return DamagedState.RightDown;
                    case Direction.Right:
                        return DamagedState.LeftDown;
                    case Direction.Up:
                    case Direction.Down:
                        return DamagedState.Destroyed;
                }
                break;
            case DamagedState.LeftUp:
            case DamagedState.LeftDown:
            case DamagedState.RightUp:
            case DamagedState.RightDown:
            case DamagedState.Destroyed:
                return DamagedState.Destroyed;
        }
        return DamagedState.Destroyed;
    }
}