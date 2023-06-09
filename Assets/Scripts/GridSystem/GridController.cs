using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ScoreSystem;
using UnityEngine;

namespace GridSystem
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] private GameObject tilePrefab;
        private static int Rows => GridManager.Instance.rows;
        private static int Columns => GridManager.Instance.columns;
        private static bool PowerUpsAvailable => GridManager.Instance.powerUpAvailable;
        private bool _powerUpBlasted;
        private GameObject[,] _grid;
        public TileController[,] TileController;
        public List<TileController> matchingTiles = new List<TileController>();
        
        private Coroutine _activeCoroutine;
        public bool canBlast;
        public bool forAI;
       


        public void Start()
        {
            TileController = new TileController[Rows, Columns];
            _grid = new GameObject[Rows, Columns];
            _powerUpBlasted = false;
            canBlast = false;
            
            _activeCoroutine = StartCoroutine(CreateGrid());
            RegisterToManager();
        }

        private void RegisterToManager()
        {
            if(!GridManager.Instance.gridControllers.Contains(this))
                GridManager.Instance.gridControllers.Add(this);
        }

        public void ResetGame()
        {
            StartCoroutine(DestroyAll());
        }

        public void TimeIsUp()
        {
            foreach (var tile in TileController)
            {
                if(tile.childObj)
                    tile.childObj.StopAnimation();
            }
        }
        private IEnumerator DestroyAll()
        {
            foreach (var tile in TileController)
            {
                tile.childObj.BlastTileChild();
                yield return null;
            }
            yield return new WaitForSeconds(0.6f);
            foreach (var tile in TileController)
            {
                Destroy(tile);
            }
            foreach (var grid in _grid )
            {
                Destroy(grid);
            }
            
            TileController = new TileController[Rows, Columns];
            _grid = new GameObject[Rows, Columns];
            
            _powerUpBlasted = false;
            canBlast = false;
            
            _activeCoroutine = StartCoroutine(CreateGrid());
            
        }

        private void Update()
        {
            canBlast = _activeCoroutine == null;
        }

        private IEnumerator CreateGrid()//Creating Grid System at the start of the game.
        {
            yield return new WaitForSeconds(0.1f);
            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Columns; j++)
                {
                    var newTile = Instantiate(tilePrefab, gameObject.transform);

                    var tilePos = new Vector2(i, j);
                    if (newTile == null)
                        continue;

                    newTile.transform.localPosition = tilePos;
                    var newTileCont = newTile.GetComponent<TileController>();
                    newTileCont.gridCont = this;
                    newTileCont.dimensionPos = new Vector2Int(i, j);
                    newTile.name = $"Tile[{i} / {j}]";

                    TileController[i, j] = newTileCont;
                    _grid[i, j] = newTile;
                    yield return new WaitForSeconds(0.07f);
                }
            }
            
            GridManager.Instance.GameStarted();
            _activeCoroutine = null;
          
        }

        public void CheckMatchingTiles(int x, int y)
        {
            canBlast = false;
            //Resetting Matching List of Tiles.
            matchingTiles = new List<TileController>();
            var tileType = TileController[x, y].tileType;

            if (tileType == TileTypes.PowerUp1)
            {
                canBlast = false;
                PowerUpBlasting(TileController[x, y]);
                return;
            }
            //Checking tiles for Adjacent on all directions.
            CheckAdjacentTiles(x, y, tileType, Vector2.up);
            CheckAdjacentTiles(x, y, tileType, Vector2.down);
            CheckAdjacentTiles(x, y, tileType, Vector2.left);
            CheckAdjacentTiles(x, y, tileType, Vector2.right);

            //If there are more than 1 matching tiles, add them to the matching list
            if (matchingTiles.Count < 1)
            {
                _activeCoroutine = null;
                return;
            }

            //Blasting Matching Tiles.
            _activeCoroutine = StartCoroutine(BlastMatchingTiles());
        }

        private void CheckAdjacentTiles(int x, int y, TileTypes tileType, Vector2 direction)
        {
            //Getting Adjacent Tiles coordinate.
            var adjacentX = Mathf.RoundToInt(x + direction.x);
            var adjacentY = Mathf.RoundToInt(y + direction.y);

            //Checking the board limits.
            if (adjacentX < 0 || adjacentX >= TileController.GetLength(0) || adjacentY < 0 ||
                adjacentY >= TileController.GetLength(1))
                return;

            var adjacentTileType = TileController[adjacentX, adjacentY].tileType;

            //Checking the Adjacent tile if match with original tile type.
            if (adjacentTileType != tileType || matchingTiles.Contains(TileController[adjacentX, adjacentY]))
                return;

            matchingTiles.Add(TileController[adjacentX, adjacentY]);

            //Checking Adjacent tiles neighbors for more adjacent.
            CheckAdjacentTiles(adjacentX, adjacentY, tileType, Vector2.up);
            CheckAdjacentTiles(adjacentX, adjacentY, tileType, Vector2.down);
            CheckAdjacentTiles(adjacentX, adjacentY, tileType, Vector2.right);
            CheckAdjacentTiles(adjacentX, adjacentY, tileType, Vector2.left);
        }

        private IEnumerator BlastMatchingTiles()
        {
            foreach (var tile in matchingTiles)
            {
                tile.childObj.BlastTileChild();
            }
            yield return new WaitForSeconds(0.6f);
           
            foreach (var tile in matchingTiles.Where(tile => tile.childObj))
            {
                Destroy(tile.childObj.gameObject);
            }

            if (forAI)
                ScoreManager.Instance.CalculateAddAIScore(matchingTiles.Count);
            else
                ScoreManager.Instance.CalculateAddingPlayerScore(matchingTiles.Count);


            _activeCoroutine = StartCoroutine(AfterBlast());
        }

        private void CreatePowerUp()
        {
            if (matchingTiles.Count >= 5)
            {
                matchingTiles[0].PowerUpChanges(6);
            }
        }
        public bool PowerUpBlasted
        {
            set => _powerUpBlasted = value;
        }

        public void PowerUpBlasting(TileController tileCont)
        {
            _activeCoroutine=StartCoroutine(PowerUpBlast(tileCont));
        }

        private IEnumerator PowerUpBlast(TileController tileCont)
        {
            var tileType = tileCont.tileType;
            matchingTiles = new List<TileController>();
            
            if (tileType == TileTypes.PowerUp1)
            {
                var tileCoordinate = tileCont.dimensionPos;
                for (var x = tileCoordinate.x-1; x < tileCoordinate.x + 2; x++)
                {
                    for (var y = tileCoordinate.y-1; y < tileCoordinate.y + 2; y++)
                    {
                        if (y < 0 || y >= TileController.GetLength(1))
                            continue;
                        if (x < 0 || x >= TileController.GetLength(0))
                            continue;
                        
                        matchingTiles.Add(TileController[x,y]);
                        yield return null;
                    }
                }
                _activeCoroutine=StartCoroutine(BlastMatchingTiles());
            }
                
        }
        private IEnumerator AfterBlast() //For check all tiles for empty types.
        {
            if(PowerUpsAvailable&&!_powerUpBlasted)
                CreatePowerUp();
            
            foreach (var tile in TileController)
            {
                if (tile.childObj)
                    CheckBlastedArea(tile.dimensionPos.x, tile.dimensionPos.y, tile,
                        Vector2.down);
                yield return null;
            }
            foreach (var tile in TileController)
            {
                if (tile.childObj || tile.dimensionPos.y != Columns - 1)
                    continue;
                tile.FllEmptyTile();
                CheckBlastedArea(tile.dimensionPos.x, tile.dimensionPos.y, tile,
                    Vector2.down);
                yield return null;
            }
            _activeCoroutine = null;
            canBlast = true;
            yield return null;
        }

        public void CheckBlastedArea(int x, int y, TileController tile, Vector2 direction)
        {
            //Getting Blasted Tile coordinate.
            var blastedY = Mathf.RoundToInt(y + direction.y);

            //Checking the board limits.
            if (blastedY < 0 || blastedY >= TileController.GetLength(1))
                return;

            var blastedTile = TileController[x, blastedY];

            //Checking the blasted tile If childCount more then 0 for return.

            if (blastedTile.transform.childCount > 0)
                return;

            /*Pre-check for tile y axis if its equals to the last axis of the board. If the tiles
             has no child then gonna activate TileController's FillEmptyTile function.*/

            if (y == Columns - 1 && tile.transform.childCount == 0)
            {
                TileController[x, y].FllEmptyTile();
            }

            //Checking If tile not have childObject for return.
            if (!tile.childObj)
                return;

            //Change Tile Type's parent to the empty Tile type.
            tile.childObj.transform.SetParent(blastedTile.transform, true);

            blastedTile.childObj = tile.childObj;
            blastedTile.tileType = tile.tileType;
            blastedTile.childObj.FallingAnimation(); //FallingAnimation from TileChildController.

            tile.childObj = null;
            tile.tileType = TileTypes.Empty;

            //Checking if tile y axis is equals to the top board tile.
            if (y == Columns - 1)
                CheckBlastedArea(x, blastedY, blastedTile, Vector2.down);
            else
            {
                CheckBlastedArea(x, y + 1, TileController[x, y + 1], Vector2.down);
                CheckBlastedArea(x, blastedY, blastedTile, Vector2.down);
            }
        }
    }
}