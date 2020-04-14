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
    private Grid grid;
    
    public override void OnAwake() {
        filterController = World.Filter.With<Translation>().With<Direction>().With<Speed>().With<Controller>().With<Engine>().With<Size>();
        filterBullets = World.Filter.With<Translation>().With<Direction>().With<Speed>().Without<Controller>();
        grid = gridInitializer.Grid;
    }

    private void Check()
    {
        for (int i = 0; i < 26; i++)
        {
            for (int j = 0; j < 26; j++)
            {
                var area=new Area();
                gridInitializer.GetArea(i, j, ref area);
                area.Damage = DamageType.RightUp;
                //gridInitializer.MarkForUpdate(i,j);
            }
            
        }

        Debug.Log("CHECK");
    }
    
    private bool IsAllowToMove(Translation translation, Size size, Direction direction)
    {
        var firstAreaX=0;
        var firstAreaY=0;
        var secondAreaX=0;
        var secondAreaY=0;
        switch (direction.lookAtDirection)
        {
            case LookAtDirection.Left:
                
                grid.GetXYPareByVertical(new Vector3(translation.x - size.x / 2f, translation.y),out firstAreaX,out firstAreaY,out secondAreaX,out secondAreaY);
                break;
            case LookAtDirection.Right:
                grid.GetXYPareByVertical(new Vector3(translation.x + size.x / 2f, translation.y),out firstAreaX,out firstAreaY,out secondAreaX,out secondAreaY);
                break;
            case LookAtDirection.Up:
                grid.GetXYPareByHorizontal(new Vector3(translation.x, translation.y+size.y/2f),out firstAreaX,out firstAreaY,out secondAreaX,out secondAreaY);
                break;
            case LookAtDirection.Down:
                grid.GetXYPareByHorizontal(new Vector3(translation.x ,translation.y-size.y/2f),out firstAreaX,out firstAreaY,out secondAreaX,out secondAreaY);
                break;
        }
        var firstArea=new Area();
        var secondArea = new Area();
        if (!gridInitializer.GetArea(firstAreaX, firstAreaY, ref firstArea)) return false;
        if (!gridInitializer.GetArea(secondAreaX, secondAreaY, ref secondArea)) return false;
        return firstArea.areaType.isWalkable && secondArea.areaType.isWalkable;
    }

    private bool CheckObstacle(Translation translation,Direction direction)
    {
        var res = false;
        var firstAreaX=0;
        var firstAreaY=0;
        var secondAreaX=0;
        var secondAreaY=0;
        switch (direction.lookAtDirection)
        {
            case LookAtDirection.Left:
            case LookAtDirection.Right:
                grid.GetXYPareByVertical(new Vector3(translation.x, translation.y),out firstAreaX,out firstAreaY,out secondAreaX,out secondAreaY);
                break;
            case LookAtDirection.Up:
            case LookAtDirection.Down:
                grid.GetXYPareByHorizontal(new Vector3(translation.x ,translation.y),out firstAreaX,out firstAreaY,out secondAreaX,out secondAreaY);
                break;
        }
        var firstArea=new Area();
        var secondArea = new Area();
        if (gridInitializer.GetArea(firstAreaX, firstAreaY, ref firstArea))
        {
            if (!firstArea.areaType.isBulletTransparent)
            {
                if (firstArea.areaType.health > 0)
                {
                    SetDamage(ref firstArea,direction);
                    gridInitializer.MarkForUpdate(firstAreaX,firstAreaY);
                }
                res = true;
            }
        }
        else
        {
            res = true;
        }

        if (gridInitializer.GetArea(secondAreaX, secondAreaY, ref secondArea))
        {
            if (!secondArea.areaType.isBulletTransparent)
            {
                if (secondArea.areaType.health > 0)
                {
                    SetDamage(ref secondArea,direction);
                    gridInitializer.MarkForUpdate(secondAreaX,secondAreaY);
                }
                res = true;
            }
        }
        else
        {
            res = true;
        }
        return res;
    }

    private void SetDamage(ref Area area, Direction direction)
    {
        switch (area.Damage)
        {
            case DamageType.Whole:
            {
                switch (direction.lookAtDirection)
                {
                    case LookAtDirection.Left:
                        area.Damage = DamageType.Left;
                        break;
                    case LookAtDirection.Right:
                        area.Damage = DamageType.Right;
                        break;
                    case LookAtDirection.Up:
                        area.Damage = DamageType.Up;
                        break;
                    case LookAtDirection.Down:
                        area.Damage = DamageType.Down;
                        break;
                }
                break;
            }
            case DamageType.Left:
                switch (direction.lookAtDirection)
                {
                    case LookAtDirection.Left:
                    case LookAtDirection.Right:
                        area.Damage = DamageType.Destroyed;
                        break;
                    case LookAtDirection.Up:
                        area.Damage = DamageType.LeftUp;
                        break;
                    case LookAtDirection.Down:
                        area.Damage = DamageType.LeftDown;
                        break;
                }
                break;
            case DamageType.Right:
                switch (direction.lookAtDirection)
                {
                    case LookAtDirection.Left:
                    case LookAtDirection.Right:
                        area.Damage = DamageType.Destroyed;
                        break;
                    case LookAtDirection.Up:
                        area.Damage = DamageType.RightUp;
                        break;
                    case LookAtDirection.Down:
                        area.Damage = DamageType.RightDown;
                        break;
                }
                break;
            case DamageType.Up:
                switch (direction.lookAtDirection)
                {
                    case LookAtDirection.Left:
                        area.Damage = DamageType.LeftUp;
                        break;
                    case LookAtDirection.Right:
                        area.Damage = DamageType.RightUp;
                        break;
                    case LookAtDirection.Up:
                    case LookAtDirection.Down:
                        area.Damage = DamageType.Destroyed;
                        break;
                }
                break;
            case DamageType.Down:
                switch (direction.lookAtDirection)
                {
                    case LookAtDirection.Left:
                        area.Damage = DamageType.LeftDown;
                        break;
                    case LookAtDirection.Right:
                        area.Damage = DamageType.RightDown;
                        break;
                    case LookAtDirection.Up:
                    case LookAtDirection.Down:
                        area.Damage = DamageType.Destroyed;
                        break;
                }
                break;
            case DamageType.LeftUp:
            case DamageType.LeftDown:
            case DamageType.RightUp:
            case DamageType.RightDown:
                area.Damage = DamageType.Destroyed;
                break;
            case DamageType.Destroyed:
                break;
        }
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
                if (entity.Has<Burst>()) entity.GetComponent<Burst>().isActive = true;
            }
        }
    }
    
    public override void OnUpdate(float deltaTime)
    {
        UnitsMove(deltaTime);
        BulletsMove(deltaTime);
        if (Input.GetKeyDown(KeyCode.A))
        {
            Check();
        }
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