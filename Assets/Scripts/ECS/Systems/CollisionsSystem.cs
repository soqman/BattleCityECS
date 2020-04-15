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
                if (xDistance <= xMinDistance && yDistance <= yMinDistance)
                {
                    //AddCollisions(entity,colliders.GetEntity(i));
                }
            }
        }
    }

    private void AddCollisions(IEntity first, IEntity second, Collision.CollisionDirection directionFirst, Collision.CollisionDirection directionSecond )
    {
        if (first.Has<Collision>())
        {
            ref var alreadyCollision = ref first.GetComponent<Collision>();
            if (alreadyCollision.colliderWith.ID != second.ID)
            {
                ref var collision = ref first.AddComponent<Collision>();
                collision.colliderWith = second;
            }
        }
        else
        {
            ref var collision = ref first.AddComponent<Collision>();
            collision.colliderWith = second;
        }
        if (second.Has<Collision>())
        {
            /*ref var alreadyCollision = ref second.GetComponent<Collision>();
            if (alreadyCollision.colliderWith.ID != first.ID)
            {
                ref var collision = ref second.AddComponent<Collision>();
                collision.colliderWith = first;
            }*/
        }
        else
        {
            ref var collision = ref second.AddComponent<Collision>();
            collision.colliderWith = first;
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