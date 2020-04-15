using System.Collections.Generic;
using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Initializers/" + nameof(GridInitializer))]
public sealed class GridInitializer : Initializer
{
    private const int COLUMNS_COUNT = 26;
    private const int ROWS_COUNT = 26;
    private const float CELL_SIZE = 0.5f;
    private readonly Vector3 OFFSET = new Vector3(-6.5f,-6.5f,0);
    [SerializeField] private GameObject areaPrefab;
    [SerializeField] private List<AreaType> areaTypes;
    [SerializeField] private AreaType empty;
    private Grid grid;

    public override void OnAwake()
    {
        grid = new Grid(COLUMNS_COUNT,ROWS_COUNT, CELL_SIZE, OFFSET);
        var areaTypesHolder = new AreaType[COLUMNS_COUNT,ROWS_COUNT];
        for (var j = 0; j < ROWS_COUNT; j++)
        {
            for (var i = 0; i < COLUMNS_COUNT; i++)
            {
                var entity = World.CreateEntity();
                ref var area = ref entity.AddComponent<Area>();
                AreaType areaType;
                if (i % 2 == 0 && j % 2 == 0)
                {
                    if (i / 2 % 2 == 0 || j / 2 % 2 == 0)
                    {
                        areaType = empty;
                    }
                    else
                    {
                        areaType = areaTypes[Random.Range(0,areaTypes.Count)];
                    }
                }
                else
                {
                    var x=i;
                    var y=j;
                    if (x % 2 != 0) x--;
                    if (y % 2 != 0) y--;
                    areaType = areaTypesHolder[x,y];
                }
                area.x = i;
                area.y = j;
                areaTypesHolder[i,j]=areaType;

                ref var translation = ref entity.AddComponent<Translation>();
                var position = grid.GetWorldPosition(i, j);
                translation.x = position.x;
                translation.y = position.y;

                if (areaType != empty)
                {
                    ref var collider = ref entity.AddComponent<Collider>();
                    collider.xSize = CELL_SIZE;
                    collider.ySize = CELL_SIZE;
                    collider.xOffset = CELL_SIZE / 2;
                    collider.yOffset = CELL_SIZE / 2;
                    collider.isBulletTransparent = areaType.isBulletTransparent;
                    collider.isWalkable = areaType.isWalkable;
                }

                entity.AddComponent<AreaUpdateIndicator>();
                
                var areaGameObject=Instantiate(areaPrefab);
                ref var areaView = ref entity.AddComponent<AreaView>();
                areaView.spriteRenderer = areaGameObject.GetComponentInChildren<SpriteRenderer>();
                areaView.Transform = areaGameObject.transform;
                areaView.wholeSprite = areaType.wholeSprite;
                areaView.leftSprite = areaType.leftSprite;
                areaView.rightSprite = areaType.rightSprite;
                areaView.downSprite = areaType.downSprite;
                areaView.upSprite = areaType.upSprite;
                areaView.leftUpSprite = areaType.leftUpSprite;
                areaView.rightUpSprite = areaType.rightUpSprite;
                areaView.leftDownSprite = areaType.leftDownSprite;
                areaView.rightDownSprite = areaType.rightDownSprite;
            }
        }
    }
    public override void Dispose() {
    }
}