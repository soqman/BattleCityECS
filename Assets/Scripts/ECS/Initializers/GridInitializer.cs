using System.Collections.Generic;
using Morpeh;
using Photon.Pun;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Initializers/" + nameof(GridInitializer))]
public sealed class GridInitializer : Initializer
{
    private const int COLUMNS_COUNT = 30;
    private const int ROWS_COUNT = 30;
    private const float CELL_SIZE = 0.5f;
    private readonly Vector3 OFFSET = new Vector3(-7f,-7.5f,0);
    [SerializeField] private GameObject areaPrefab;
    [SerializeField] private GameObject greenBasePrefab;
    [SerializeField] private GameObject yellowBasePrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private List<AreaType> areaTypes;
    [SerializeField] private AreaType empty;
    [SerializeField] private AreaType wall;
    [SerializeField] private Vector2 greenBasePosition;
    [SerializeField] private Vector2 yellowBasePosition;
    private Grid grid;

    public override void OnAwake()
    {
        grid = new Grid(COLUMNS_COUNT,ROWS_COUNT, CELL_SIZE, OFFSET);
        var areaTypesHolder = new AreaType[COLUMNS_COUNT,ROWS_COUNT];
        for (var j = 0; j < ROWS_COUNT; j++)
        {
            for (var i = 0; i < COLUMNS_COUNT; i++)
            {
                var areaGameObject=PhotonNetwork.Instantiate(areaPrefab.name,Vector3.zero, Quaternion.identity);
                var areaViewProvider = areaGameObject.GetComponent<AreaViewProvider>();
                ref var areaView = ref areaViewProvider.GetData();
                var entity = areaViewProvider.Entity;
                ref var area = ref areaGameObject.GetComponent<AreaProvider>().GetData();
                ref var translation = ref areaGameObject.GetComponent<TranslationProvider>().GetData();
                AreaType areaType;
                if (i % 2 == 0 && j % 2 == 0)
                {
                    if (i == 0 || i == COLUMNS_COUNT - 2 || j == 0 || j == COLUMNS_COUNT - 2) areaType = wall;
                    else if (i / 2 % 2 != 0 || j / 2 % 2 != 0)
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
                    collider.mask = areaType.mask;
                    collider.layer = areaType.layer;
                    collider.exceptionEntity = -1;
                }

                area.areaType = areaType.name;
                entity.AddComponent<AreaInitIndicator>();
                entity.AddComponent<AreaUpdateIndicator>();
            }
        }
    }
    public override void Dispose() {
    }
}