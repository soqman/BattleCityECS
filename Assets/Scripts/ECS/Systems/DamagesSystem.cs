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
    private Filter unitsFilter;
    private Filter baseFilter;
    public override void OnAwake()
    {
        areasFilter = World.Filter.With<Collision>().With<Area>();
        unitsFilter = World.Filter.With<Collision>().With<Health>().With<Barrel>();
    }

    public override void OnUpdate(float deltaTime)
    {
        UpdateUnits();
        UpdateAreas();
    }

    private void UpdateUnits()
    {
        foreach (var entity in unitsFilter)
        {
            ref var collision = ref entity.GetComponent<Collision>();
            ref var health = ref entity.GetComponent<Health>();
            foreach (var collisionItem in collision.collisions)
            {
                if (collisionItem.collideWith.Has<Damage>())
                {
                    health.value -= collisionItem.collideWith.GetComponent<Damage>().value;
                }
            }
        }
    }

    private void UpdateAreas()
    {
        foreach (var entity in areasFilter)
        {
            ref var area = ref entity.GetComponent<Area>();
            ref var collision = ref entity.GetComponent<Collision>();
            
            foreach (var collisionItem in collision.collisions)
            {
                if (entity.Has<Health>())
                {
                    ref var health = ref entity.GetComponent<Health>();
                    health.value--;
                    area.State = AddDamageDependsOnHealth(health);
                }
                else area.State=AddDamage(area.State,collisionItem.direction);
                if (entity.Has<Collider>() && area.State == DamagedState.Destroyed)
                {
                    entity.RemoveComponent<Collider>();
                     entity.AddComponent<Destroyer>();
                }
                entity.AddComponent<AreaUpdateIndicator>();
            }
        }
    }

    private DamagedState AddDamageDependsOnHealth(Health health)
    {
        return health.value > 0 ? DamagedState.Whole : DamagedState.Destroyed;
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