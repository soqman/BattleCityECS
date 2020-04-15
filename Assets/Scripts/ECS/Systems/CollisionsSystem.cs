﻿using System.Collections.Generic;
using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(CollisionsSystem))]
public sealed class CollisionsSystem : UpdateSystem
{
    private Filter rigidbodies;
    private Filter colliders;
    
    public override void OnAwake()
    {
        rigidbodies = World.Filter.With<Speed>().With<Collider>().With<Translation>();
        colliders = World.Filter.With<Collider>().With<Translation>();
    }

    public override void OnUpdate(float deltaTime)
    {
        RemoveCollisions();
        var collidersBag = colliders.Select<Collider>();
        var translationsBag = colliders.Select<Translation>();
        foreach (var entity in rigidbodies)
        {
            ref var collider = ref entity.GetComponent<Collider>();
            ref var translation = ref entity.GetComponent<Translation>();
            for (var i = 0; i < colliders.Length; i++)
            {
                ref var currentCollider = ref collidersBag.GetComponent(i);

                if (entity.ID == colliders.GetEntity(i).ID) continue;
                var xPositionA = collider.xOffset+translation.x;
                var yPositionA = collider.yOffset+translation.y;
                var xPositionB = currentCollider.xOffset+translationsBag.GetComponent(i).x;
                var yPositionB = currentCollider.yOffset+translationsBag.GetComponent(i).y;
                var xDistance = Mathf.Abs(xPositionB - xPositionA);
                var yDistance = Mathf.Abs(yPositionB - yPositionA);
                var xMinDistance = (collider.xSize + currentCollider.xSize) / 2f;
                var yMinDistance = (collider.ySize + currentCollider.ySize) / 2f;
                var xOverlap = xMinDistance - xDistance;
                var yOverlap = yMinDistance - yDistance;
                
                if (!(xDistance < xMinDistance) || !(yDistance < yMinDistance)) continue;
                if (xOverlap > yOverlap)
                {
                    if (collider.mask == (collider.mask | (1 << currentCollider.layer)))
                    AddCollision(entity, new Collision.CollisionItem{collideWith = colliders.GetEntity(i),direction = yPositionA > yPositionB ? Direction.Down:Direction.Up});
                    if (currentCollider.mask == (currentCollider.mask | (1 << collider.layer)))
                    AddCollision(colliders.GetEntity(i),new Collision.CollisionItem{collideWith = entity,direction = yPositionA > yPositionB ? Direction.Up:Direction.Down});
                }else if (yOverlap > xOverlap)
                {
                    if (collider.mask == (collider.mask | (1 << currentCollider.layer)))
                    AddCollision(entity,new Collision.CollisionItem{collideWith = colliders.GetEntity(i),direction = xPositionA > xPositionB?Direction.Left:Direction.Right});
                    if (currentCollider.mask == (currentCollider.mask | (1 << collider.layer)))
                    AddCollision(colliders.GetEntity(i),new Collision.CollisionItem{collideWith = entity,direction = xPositionA>xPositionB?Direction.Right:Direction.Left});
                }
            }
        }
    }

    private void AddCollision(IEntity collisionTarget,Collision.CollisionItem collisionItem )
    {
        if (collisionTarget.Has<Collision>())
        {
            ref var alreadyCollision = ref collisionTarget.GetComponent<Collision>();
            alreadyCollision.collisions.Add(collisionItem);
        }
        else
        {
            ref var collision = ref collisionTarget.AddComponent<Collision>();
            collision.collisions = new List<Collision.CollisionItem>{collisionItem};
        }
    }

    private void RemoveCollisions()
    {
        foreach (var entity in colliders)
        {
            if (entity.Has<Collision>()) entity.RemoveComponent<Collision>();
        }
    }
}