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
    private GridSystem<Area> grid;
    private const int COLUMNS_COUNT = 13;
    private const int ROWS_COUNT = 13;
    private const float CELL_SIZE = 1f;
    private readonly Vector3 OFFSET = new Vector3(-6.5f,-6.5f,0);
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private List<AreaType> areaTypes;
    [SerializeField] private AreaType empty;

    public GridSystem<Area> Grid
    {
        get { return grid; }
    }


    public override void OnAwake()
    {
        grid = new GridSystem<Area>(COLUMNS_COUNT,ROWS_COUNT, CELL_SIZE, OFFSET);
        for (var j = 0; j < ROWS_COUNT; j++)
        {
            for (var i = 0; i < COLUMNS_COUNT; i++)
            {
                var cell=Instantiate(cellPrefab);
                
                var sizeProvider=cell.AddComponent<SizeProvider>();
                ref var size = ref sizeProvider.GetData();
                size.x = CELL_SIZE;
                size.y = CELL_SIZE;
                
                var translationProvider = cell.AddComponent<TranslationProvider>();
                ref var translation = ref translationProvider.GetData();
                var position = grid.GetWorldPosition(i, j);
                translation.x = position.x;
                translation.y = position.y;
                
                var areaProvider = cell.AddComponent<AreaProvider>();
                ref var area = ref areaProvider.GetData();
                if (i % 2 == 0 || j%2==0) area.areaType = empty;
                else area.areaType = areaTypes[Random.Range(0, areaTypes.Count)];
                area.x = i;
                area.y = j;
                grid.SetGridObject(i,j,area);
            }
        }
    }

    public override void Dispose() {
    }
}