using System;
using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(ViewUpdaterSystem))]
public sealed class ViewUpdaterSystem : UpdateSystem
{
    private Filter tankFilter;
    private Filter bulletFilter;
    private Filter areaFilter;
    public override void OnAwake()
    {
        tankFilter=World.Filter.With<Translation>().With<Direction>().With<Engine>().With<TankView>();
        bulletFilter=World.Filter.With<Translation>().With<BulletView>().With<Direction>();
        areaFilter = World.Filter.With<Translation>().With<AreaView>().With<Area>().With<AreaUpdateIndicator>();
    }

    public override void OnUpdate(float deltaTime) {
        UpdateTanks();
        UpdateBullets();
        UpdateAreas();
    }

    private void UpdateTanks()
    {
        foreach (var entity in tankFilter)
        {
            var translation = entity.GetComponent<Translation>();
            var direction = entity.GetComponent<Direction>();
            var engine = entity.GetComponent<Engine>();
            var tankView = entity.GetComponent<TankView>();
            tankView.Transform.position=new Vector3(translation.x,translation.y,0);
            switch (direction.lookAtDirection)
            {
                case LookAtDirection.Up:
                    tankView.Transform.localRotation=Quaternion.Euler(0,0,0);
                    break;
                case LookAtDirection.Down:
                    tankView.Transform.localRotation=Quaternion.Euler(0,0,180);
                    break;
                case LookAtDirection.Left:
                    tankView.Transform.localRotation=Quaternion.Euler(0,0,90);
                    break;
                case LookAtDirection.Right:
                    tankView.Transform.localRotation=Quaternion.Euler(0,0,-90);
                    break;
            }
            tankView.Animator.SetBool("on",engine.isActive);
        }
    }

    private void UpdateBullets()
    {
        foreach (var entity in bulletFilter)
        {
            var translation = entity.GetComponent<Translation>();
            var bulletView = entity.GetComponent<BulletView>();
            var direction = entity.GetComponent<Direction>();
            bulletView.Transform.position=new Vector3(translation.x,translation.y,0);
            switch (direction.lookAtDirection)
            {
                case LookAtDirection.Up:
                    bulletView.Transform.localRotation=Quaternion.Euler(0,0,0);
                    break;
                case LookAtDirection.Down:
                    bulletView.Transform.localRotation=Quaternion.Euler(0,0,180);
                    break;
                case LookAtDirection.Left:
                    bulletView.Transform.localRotation=Quaternion.Euler(0,0,90);
                    break;
                case LookAtDirection.Right:
                    bulletView.Transform.localRotation=Quaternion.Euler(0,0,-90);
                    break;
            }
        }
    }
    
    private void UpdateAreas()
    {
        foreach (var entity in areaFilter)
        {
            var translation = entity.GetComponent<Translation>();
            var areaView = entity.GetComponent<AreaView>();
            var area = entity.GetComponent<Area>();
            areaView.Transform.position=new Vector3(translation.x,translation.y,0);
            areaView.spriteRenderer.sprite = areaView.wholeSprite;
            switch (area.State)
            {
                case DamageType.Whole:
                    break;
                case DamageType.Left:
                    areaView.spriteRenderer.sprite = areaView.leftSprite;
                    break;
                case DamageType.Right:
                    areaView.spriteRenderer.sprite = areaView.rightSprite;
                    break;
                case DamageType.Up:
                    areaView.spriteRenderer.sprite = areaView.upSprite;
                    break;
                case DamageType.Down:
                    areaView.spriteRenderer.sprite = areaView.downSprite;
                    break;
                case DamageType.LeftUp:
                    areaView.spriteRenderer.sprite = areaView.leftUpSprite;
                    break;
                case DamageType.LeftDown:
                    areaView.spriteRenderer.sprite = areaView.leftDownSprite;
                    break;
                case DamageType.RightUp:
                    areaView.spriteRenderer.sprite = areaView.rightUpSprite;
                    break;
                case DamageType.RightDown:
                    areaView.spriteRenderer.sprite = areaView.rightDownSprite;
                    break;
                case DamageType.Destroyed:
                    areaView.spriteRenderer.sprite = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            entity.RemoveComponent<AreaUpdateIndicator>();
        }
    }
}