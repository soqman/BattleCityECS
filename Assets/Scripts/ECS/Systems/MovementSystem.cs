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
    [SerializeField] private GridInitializer gridInitializer;
    private GridSystem<Area> grid;
    
    public override void OnAwake() {
        filterController = World.Filter.With<Translation>().With<Direction>().With<Speed>().With<Controller>().With<Engine>().With<Size>();
        filterBullets = World.Filter.With<Translation>().With<Direction>().With<Speed>().Without<Controller>();
        grid = gridInitializer.Grid;
    }

    private bool IsAllowToMove(Translation translation, Size size, Direction direction)
    {
        var currentArea=new Area();
        var nextArea=new Area();
        switch (direction.lookAtDirection)
        {
            case LookAtDirection.Left:
                currentArea = grid.GetGridObject(new Vector3(translation.x + size.x / 2f-0.01f, translation.y-0.01f));
                nextArea = grid.GetGridObject(currentArea.x-1, currentArea.y);
                break;
            case LookAtDirection.Right:
                currentArea = grid.GetGridObject(new Vector3(translation.x - size.x / 2f-0.01f, translation.y-0.01f));
                nextArea = grid.GetGridObject(currentArea.x+1, currentArea.y);
                break;
            case LookAtDirection.Up:
                currentArea = grid.GetGridObject(new Vector3(translation.x-0.01f, translation.y-size.y/2f-0.01f));
                nextArea = grid.GetGridObject(currentArea.x, currentArea.y+1);
                break;
            case LookAtDirection.Down:
                currentArea = grid.GetGridObject(new Vector3(translation.x-0.01f ,translation.y+size.y/2f-0.01f));
                nextArea = grid.GetGridObject(currentArea.x, currentArea.y-1);
                break;
        }
        return nextArea.areaType.isWalkable;
    }



    public override void OnUpdate(float deltaTime) {
        foreach (var entity in filterController) {
            
            ref var translation = ref entity.GetComponent<Translation>();
            ref var direction = ref entity.GetComponent<Direction>();
            ref var speed = ref entity.GetComponent<Speed>();
            ref var engine = ref entity.GetComponent<Engine>();
            ref var size = ref entity.GetComponent<Size>();
            
            if (Input.GetKey(KeyCode.UpArrow))
            {
                direction.lookAtDirection = LookAtDirection.Up;
                if (IsAllowToMove(translation, size, direction))
                {
                    translation.y += deltaTime * speed.value;
                    translation.x = Closest(translation.x, size.x);
                }
                engine.isActive = true;

            }else if (Input.GetKey(KeyCode.DownArrow))
            {
                direction.lookAtDirection = LookAtDirection.Down;
                if (IsAllowToMove(translation, size, direction))
                {
                    translation.y -= deltaTime * speed.value;
                    translation.x = Closest(translation.x, size.x);
                }
                engine.isActive = true;
            }else if (Input.GetKey(KeyCode.LeftArrow))
            {
                direction.lookAtDirection = LookAtDirection.Left;
                if (IsAllowToMove(translation, size, direction))
                {
                    translation.x -= deltaTime * speed.value;
                    translation.y = Closest(translation.y, size.y);
                }
                engine.isActive = true;
            }else if (Input.GetKey(KeyCode.RightArrow))
            {
                direction.lookAtDirection = LookAtDirection.Right;
                if (IsAllowToMove(translation, size, direction))
                {
                    translation.x += deltaTime * speed.value;
                    translation.y = Closest(translation.y, size.y);
                }
                engine.isActive = true;
            }
            else
            {
                engine.isActive = false;
            }
        }
        foreach (var entity in filterBullets) {
            
            ref var translation = ref entity.GetComponent<Translation>();
            ref var direction = ref entity.GetComponent<Direction>();
            ref var speed = ref entity.GetComponent<Speed>();

            switch (direction.lookAtDirection)
            {
                case LookAtDirection.Up: 
                    translation.y -= -deltaTime * speed.value;
                    break;
                case LookAtDirection.Down: 
                    translation.y -= deltaTime * speed.value;
                    break;
                case LookAtDirection.Left: 
                    translation.x -= deltaTime * speed.value;
                    break;
                case LookAtDirection.Right: 
                    translation.x -= -deltaTime * speed.value;
                    break;
            }
        }
    }
    
    static float Closest(float src, float divider)
    {
        float mod = Math.Abs(src % divider);
        if (mod >= divider / 2)
        {
            Debug.Log("src:"+src+" mod:"+mod+" res+div:"+(src - mod+divider));
            return src - mod + divider;
        }

        Debug.Log("src:"+src+" mod:"+mod+" res:"+(src - mod));
        return src - mod;
    }
}