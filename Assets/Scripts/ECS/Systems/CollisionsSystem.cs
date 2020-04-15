using System.Collections.Generic;
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
        rigidbodies = World.Filter.With<Speed>().With<Collider>();
        colliders = World.Filter.With<Collider>();
    }

    public override void OnUpdate(float deltaTime)
    {
        RemoveCollisions();
        var collidersBag = colliders.Select<Collider>();
        foreach (var entity in rigidbodies)
        {
            ref var collider = ref entity.GetComponent<Collider>();
            for (var i = 0; i < colliders.Length; i++)
            {
                ref var currentCollider = ref collidersBag.GetComponent(i);
                if (entity.ID == colliders.GetEntity(i).ID) continue;
                var xPositionA = collider.xSize + collider.xOffset;
                var yPositionA = collider.ySize + collider.yOffset;
                var xPositionB = currentCollider.xSize + currentCollider.xOffset;
                var yPositionB = currentCollider.ySize + currentCollider.yOffset;
                var xDistance = Mathf.Abs(xPositionB - xPositionA);
                var yDistance = Mathf.Abs(yPositionB - yPositionA);
                var xMinDistance = (collider.xSize + currentCollider.xSize) / 2f;
                var yMinDistance = (collider.ySize + currentCollider.ySize) / 2f;
                var xOverlap = xMinDistance - xDistance;
                var yOverlap = yMinDistance - yDistance;
                if (!(xDistance <= xMinDistance) || !(yDistance <= yMinDistance)) continue;
                if (xOverlap > yOverlap)
                {
                    AddCollision(entity, new Collision.CollisionItem{collideWith = colliders.GetEntity(i),direction = yPositionA > yPositionB ? Collision.CollisionDirection.U:Collision.CollisionDirection.D});
                    AddCollision(colliders.GetEntity(i),new Collision.CollisionItem{collideWith = entity,direction = yPositionA > yPositionB ? Collision.CollisionDirection.D:Collision.CollisionDirection.U});
                }else if (yOverlap > xOverlap)
                {
                    AddCollision(entity,new Collision.CollisionItem{collideWith = colliders.GetEntity(i),direction = yPositionA > xPositionB?Collision.CollisionDirection.L:Collision.CollisionDirection.R});
                    AddCollision(colliders.GetEntity(i),new Collision.CollisionItem{collideWith = entity,direction = xPositionA>xPositionB?Collision.CollisionDirection.R:Collision.CollisionDirection.L});
                }
                else
                {
                    //AddCollision(entity, new Collision.CollisionItem{collideWith = colliders.GetEntity(i),direction = yPositionA > yPositionB ? Collision.CollisionDirection.U:Collision.CollisionDirection.D});
                    //AddCollision(colliders.GetEntity(i),new Collision.CollisionItem{collideWith = entity,direction = yPositionA > yPositionB ? Collision.CollisionDirection.D:Collision.CollisionDirection.U});
                    //AddCollision(entity,new Collision.CollisionItem{collideWith = colliders.GetEntity(i),direction = yPositionA > xPositionB?Collision.CollisionDirection.L:Collision.CollisionDirection.R});
                    //AddCollision(colliders.GetEntity(i),new Collision.CollisionItem{collideWith = entity,direction = xPositionA>xPositionB?Collision.CollisionDirection.R:Collision.CollisionDirection.L});
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