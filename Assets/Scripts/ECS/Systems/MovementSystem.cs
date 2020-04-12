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

    /*private bool IsAllowToMove(Translation translation, Size size, Direction direction)
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
    }*/
    
    private bool IsAllowToMove(Translation translation, Size size, Direction direction)
    {
        var firstArea=new Area();
        var secondArea=new Area();
        switch (direction.lookAtDirection)
        {
            case LookAtDirection.Left:
                grid.GetPareOfGridObjectVertical(new Vector3(translation.x - size.x / 2f, translation.y),out firstArea,out secondArea);
                break;
            case LookAtDirection.Right:
                grid.GetPareOfGridObjectVertical(new Vector3(translation.x + size.x / 2f, translation.y),out firstArea,out secondArea);
                break;
            case LookAtDirection.Up:
                grid.GetPareOfGridObjectHorizontal(new Vector3(translation.x, translation.y+size.y/2f),out firstArea,out secondArea);
                break;
            case LookAtDirection.Down:
                grid.GetPareOfGridObjectHorizontal(new Vector3(translation.x ,translation.y-size.y/2f),out firstArea,out secondArea);
                break;
        }
        if (firstArea.areaType == null || secondArea.areaType == null) return false;
        return firstArea.areaType.isWalkable && secondArea.areaType.isWalkable;
    }

    private bool CheckObstacle(Translation translation,Direction direction)
    {
        var firstArea=new Area();
        var secondArea=new Area();
        switch (direction.lookAtDirection)
        {
            case LookAtDirection.Left:
            case LookAtDirection.Right:
                grid.GetPareOfGridObjectVertical(new Vector3(translation.x, translation.y),out firstArea,out secondArea);
                break;
            case LookAtDirection.Up:
            case LookAtDirection.Down:
                grid.GetPareOfGridObjectHorizontal(new Vector3(translation.x ,translation.y),out firstArea,out secondArea);
                break;
        }
        if (firstArea.areaType == null || secondArea.areaType==null) return true;
        return !firstArea.areaType.isBulletTransparent || !secondArea.areaType.isBulletTransparent;;
    }

    private void UnitsMove(float deltaTime)
    {
        foreach (var entity in filterController) {
            
            ref var translation = ref entity.GetComponent<Translation>();
            ref var direction = ref entity.GetComponent<Direction>();
            ref var speed = ref entity.GetComponent<Speed>();
            ref var engine = ref entity.GetComponent<Engine>();
            ref var size = ref entity.GetComponent<Size>();
            
            if (Input.GetKey(KeyCode.UpArrow))
            {
                direction.lookAtDirection = LookAtDirection.Up;
                translation.x = Closest(translation.x, size.x/2);
                if (IsAllowToMove(translation, size, direction))
                {
                    translation.y += deltaTime * speed.value;
                }
                engine.isActive = true;

            }else if (Input.GetKey(KeyCode.DownArrow))
            {
                direction.lookAtDirection = LookAtDirection.Down;
                translation.x = Closest(translation.x, size.x/2);
                if (IsAllowToMove(translation, size, direction))
                {
                    translation.y -= deltaTime * speed.value;
                }
                engine.isActive = true;
            }else if (Input.GetKey(KeyCode.LeftArrow))
            {
                direction.lookAtDirection = LookAtDirection.Left;
                translation.y = Closest(translation.y, size.y/2);
                if (IsAllowToMove(translation, size, direction))
                {
                    translation.x -= deltaTime * speed.value;
                }
                engine.isActive = true;
            }else if (Input.GetKey(KeyCode.RightArrow))
            {
                direction.lookAtDirection = LookAtDirection.Right;
                translation.y = Closest(translation.y, size.y/2);
                if (IsAllowToMove(translation, size, direction))
                {
                    translation.x += deltaTime * speed.value;
                }
                engine.isActive = true;
            }
            else
            {
                engine.isActive = false;
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

            if (CheckObstacle(translation,direction))
            {
                entity.RemoveComponent<Direction>();
                entity.RemoveComponent<Speed>();
                entity.AddComponent<Burst>();
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
        float mod = src % divider;
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