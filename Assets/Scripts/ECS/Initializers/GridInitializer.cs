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
    private readonly Vector3 OFFSET = new Vector3(-7.5f,-7.5f,0);
    [SerializeField] private GameObject areaPrefab;
    [SerializeField] private GameObject greenBasePrefab;
    [SerializeField] private GameObject yellowBasePrefab;
    [SerializeField] private List<AreaType> areaTypes;
    [SerializeField] private AreaType empty;
    [SerializeField] private GameObject wallPrefab;
    private Vector2 greenBasePosition;
    private Vector2 yellowBasePosition;
    private Grid grid;

    public override void OnAwake()
    {
        GridInit();
        PlaceBases();
        PlaceWalls();
        PlaceAreas();
    }

    private void GridInit()
    {
        grid = new Grid(COLUMNS_COUNT,ROWS_COUNT, CELL_SIZE, OFFSET);
    }
    private void PlaceBases()
    {
        var yBase = PhotonNetwork.Instantiate(yellowBasePrefab.name,new Vector3(yellowBasePosition.x,yellowBasePosition.y), Quaternion.identity);
        var yellowEntity=yBase.GetComponent<AreaProvider>().Entity;
        var yellowTranslation = yBase.GetComponent<TranslationProvider>().GetData();
        yellowBasePosition=new Vector2(yellowTranslation.x,yellowTranslation.y);
        yellowEntity.AddComponent<AreaInitIndicator>();
        yellowEntity.AddComponent<AreaUpdateIndicator>();
        var gBase=PhotonNetwork.Instantiate(greenBasePrefab.name,new Vector3(greenBasePosition.x,greenBasePosition.y), Quaternion.identity);
        var greenEntity=gBase.GetComponent<AreaProvider>().Entity;
        var greenTranslation = gBase.GetComponent<TranslationProvider>().GetData();
        greenBasePosition=new Vector2(greenTranslation.x,greenTranslation.y);
        greenEntity.AddComponent<AreaInitIndicator>();
        greenEntity.AddComponent<AreaUpdateIndicator>();
    }

    private void PlaceWalls()
    {
        if (grid == null) return;
        for (var j = 0; j < ROWS_COUNT; j+=2)
        {
            for (var i = 0; i < COLUMNS_COUNT; i+=2)
            {
                if (i != 0 && i != COLUMNS_COUNT - 2 && j != 0 && j != ROWS_COUNT - 2) continue;
                var position = grid.GetWorldPosition(i, j);
                var areaGameObject=PhotonNetwork.Instantiate(wallPrefab.name,new Vector3(position.x,position.y,0), Quaternion.identity);
                var translationProvider =areaGameObject.GetComponent<TranslationProvider>();
                ref var translation =ref translationProvider.GetData();
                translation.x = position.x;
                translation.y = position.y;
                var entity = translationProvider.Entity;
                entity.AddComponent<AreaUpdateIndicator>();
                entity.AddComponent<AreaInitIndicator>();
            }
        }
    }

    private bool IsPlaceForBase(int x, int y)
    {
        if (grid == null) return false;
        var position = grid.GetWorldPosition(x, y);
        var res = position.x == greenBasePosition.x && position.y == greenBasePosition.y || position.x == yellowBasePosition.x && position.y == yellowBasePosition.y;
        var resRight = position.x == greenBasePosition.x+CELL_SIZE && position.y == greenBasePosition.y || position.x == yellowBasePosition.x+CELL_SIZE && position.y == yellowBasePosition.y;
        var resUp = position.x == greenBasePosition.x && position.y == greenBasePosition.y+CELL_SIZE || position.x == yellowBasePosition.x && position.y == yellowBasePosition.y+CELL_SIZE;
        var resRightUp = position.x == greenBasePosition.x+CELL_SIZE && position.y == greenBasePosition.y+CELL_SIZE || position.x == yellowBasePosition.x+CELL_SIZE && position.y == yellowBasePosition.y+CELL_SIZE;
        return res || resUp || resRight || resRightUp;
    }

    private void PlaceAreas()
    {
        if (grid == null) return;
        var areaTypesHolder = new AreaType[COLUMNS_COUNT,ROWS_COUNT];
        for (var j = 2; j < ROWS_COUNT-2; j++)
        {
            for (var i = 2; i < COLUMNS_COUNT-2; i++)
            {
                if(IsPlaceForBase(i,j))
                {
                    Debug.Log(i+" "+j);
                    continue;
                }
                var position = grid.GetWorldPosition(i, j);
                var areaGameObject=PhotonNetwork.Instantiate(areaPrefab.name,Vector3.zero, Quaternion.identity);
                var areaViewProvider = areaGameObject.GetComponent<AreaViewProvider>();
                var entity = areaViewProvider.Entity;
                ref var area = ref areaGameObject.GetComponent<AreaProvider>().GetData();
                ref var translation = ref areaGameObject.GetComponent<TranslationProvider>().GetData();
                AreaType areaType;
                if (i % 2 == 0 && j % 2 == 0)
                {
                    if (i / 2 % 2 != 0 || j / 2 % 2 != 0)
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