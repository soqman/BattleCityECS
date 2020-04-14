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
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private List<AreaType> areaTypes;
    [SerializeField] private AreaType empty;
    private Area[,] areas;
    public Grid Grid { get; private set; }

    public override void OnAwake()
    {
        Grid = new Grid(COLUMNS_COUNT,ROWS_COUNT, CELL_SIZE, OFFSET);
        areas=new Area[COLUMNS_COUNT,ROWS_COUNT];
        for (var j = 0; j < ROWS_COUNT; j++)
        {
            for (var i = 0; i < COLUMNS_COUNT; i++)
            {
                var entity = World.CreateEntity();
                ref var area = ref entity.AddComponent<Area>();
                if (i % 2 == 0 && j % 2 == 0)
                {
                    if (i / 2 % 2 == 0 || j / 2 % 2 == 0)
                    {
                        area.areaType = empty;
                    }
                    else
                    {
                        area.areaType = areaTypes[Random.Range(0, areaTypes.Count)];
                    }
                }
                else
                {
                    var x=i;
                    var y=j;
                    if (x % 2 != 0) x--;
                    if (y % 2 != 0) y--;
                    area.areaType = areas[x,y].areaType;
                }
                area.x = i;
                area.y = j;
                areas[i,j]=area;

                ref var translation = ref entity.AddComponent<Translation>();
                var position = Grid.GetWorldPosition(i, j);
                translation.x = position.x;
                translation.y = position.y;

                entity.AddComponent<AreaUpdateIndicator>();
                
                var areaGameObject=Instantiate(cellPrefab);
                ref var areaView = ref entity.AddComponent<AreaView>();
                areaView.spriteRenderer = areaGameObject.GetComponentInChildren<SpriteRenderer>();
                areaView.Transform = areaGameObject.transform;
            }
        }
    }

    public void MarkForUpdate(int x, int y)
    {
    }

    public bool GetArea(int x, int y, ref Area area)
    {
        if (x < 0 || x >= COLUMNS_COUNT || y < 0 || y >= ROWS_COUNT) return false;
        area = areas[x, y];
        return true;
    }

    public override void Dispose() {
    }
}