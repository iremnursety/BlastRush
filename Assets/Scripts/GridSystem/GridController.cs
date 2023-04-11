using System.Collections;
using System.Collections.Generic;
using ScoreSystem;
using UnityEngine;

namespace GridSystem
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] private GameObject tilePrefab;
        private static int Rows => GridManager.Instance.rows;
        private static int Columns => GridManager.Instance.columns;
        
        private GameObject[,] _grid;
        private TileController[,] _tileController;
        private List<TileController> _matchingTiles=new List<TileController>();
        private Coroutine _activeCoroutine;
        public bool canBlast;
        
        public void Start()
        {
            _tileController = new TileController[Rows, Columns];
            _grid = new GameObject[Rows, Columns];
            canBlast = true;
            StartCoroutine(CreateGrid());
        }

        public void Update()
        {
            canBlast = _activeCoroutine == null;
            //GridManager.Instance.canBlast = true;
        }

        private IEnumerator CreateGrid() //Creating Grid System at the start of the game.
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
                    newTileCont.dimensionPos = new Vector2(i, j);
                    newTile.name = $"Tile[{i} / {j}]";
                    
                    _tileController[i, j] = newTileCont;
                    _grid[i, j] = newTile;
                    yield return new WaitForSeconds(0.07f);
                }
            }
            GridManager.Instance.GameStarted();
        }
        public void CheckMatchingTiles(int x, int y)
        {
            //Resetting Matching List of Tiles.
            _matchingTiles=new List<TileController>();
            var tileType = _tileController[x, y].tileType;

            //Checking tiles for Adjacent on all directions.
            CheckAdjacentTiles(x, y, tileType, Vector2.up);
            CheckAdjacentTiles(x, y, tileType, Vector2.down);
            CheckAdjacentTiles(x, y, tileType, Vector2.left);
            CheckAdjacentTiles(x, y, tileType, Vector2.right);

            //If there are more than 1 matching tiles, add them to the matching list
            if (_matchingTiles.Count < 1)
                return;

            //Blasting Matching Tiles.
            _activeCoroutine = StartCoroutine(BlastMatchingTiles());
        }

        private void CheckAdjacentTiles(int x, int y, TileTypes tileType, Vector2 direction)
        {
            //Getting Adjacent Tiles coordinate.
            var adjacentX = Mathf.RoundToInt(x + direction.x);
            var adjacentY = Mathf.RoundToInt(y + direction.y);

            //Checking the board limits.
            if (adjacentX < 0 || adjacentX >= _tileController.GetLength(0) || adjacentY < 0 ||
                adjacentY >= _tileController.GetLength(1))
                return;

            var adjacentTileType = _tileController[adjacentX, adjacentY].tileType;

            //Checking the Adjacent tile if match with original tile type.
            if (adjacentTileType != tileType || _matchingTiles.Contains(_tileController[adjacentX, adjacentY]))
                return;

            _matchingTiles.Add(_tileController[adjacentX, adjacentY]);

            //Checking Adjacent tiles neighbors for more adjacent.
            CheckAdjacentTiles(adjacentX, adjacentY, tileType, Vector2.up);
            CheckAdjacentTiles(adjacentX, adjacentY, tileType, Vector2.down);
            CheckAdjacentTiles(adjacentX, adjacentY, tileType, Vector2.right);
            CheckAdjacentTiles(adjacentX, adjacentY, tileType, Vector2.left);
        }
        private IEnumerator BlastMatchingTiles()
        {

            foreach (var tile in _matchingTiles)
            {
                tile.childObj.BlastAnimation();
            }
            yield return new WaitForSeconds(0.3f);
            foreach (var tile in _matchingTiles)
            {
                tile.childObj.ActivateParticleSystem();
            }
            yield return new WaitForSeconds(0.3f);
            foreach (var tile in _matchingTiles)
            {
                Destroy(tile.childObj.gameObject);
            }
            ScoreManager.Instance.CalculateAddingScore(_matchingTiles.Count);
            _activeCoroutine = StartCoroutine(AfterBlast());
        }
        private IEnumerator AfterBlast() //For check all tiles for empty types.
        {
            foreach (var tile in _tileController)
            {
                if (tile.childObj)
                    CheckBlastedArea(Mathf.RoundToInt(tile.dimensionPos.x), Mathf.RoundToInt(tile.dimensionPos.y), tile,
                        Vector2.down);
                yield return null;
            }
            foreach (var tile in _tileController)
            {
                if (!tile.childObj && Mathf.RoundToInt(tile.dimensionPos.y) == Columns - 1)
                {
                    tile.FllEmptyTile();
                    CheckBlastedArea(Mathf.RoundToInt(tile.dimensionPos.x), Mathf.RoundToInt(tile.dimensionPos.y), tile,
                        Vector2.down);
                }
                yield return null;
            }
            _activeCoroutine = null;
        }

        public void CheckBlastedArea(int x, int y, TileController tile, Vector2 direction)
        {
            //Getting Blasted Tile coordinate.
            var blastedY = Mathf.RoundToInt(y + direction.y);

            //Checking the board limits.
            if (blastedY < 0 || blastedY >= _tileController.GetLength(1))
                return;

            var blastedTile = _tileController[x, blastedY];

            //Checking the blasted tile If childCount more then 0 for return.

            if (blastedTile.transform.childCount > 0)
                return;

            /*Pre-check for tile y axis if its equals to the last axis of the board. If the tiles
             has no child then gonna activate TileController's FillEmptyTile function.*/

            if (y == Columns - 1 && tile.transform.childCount == 0)
            {
                _tileController[x, y].FllEmptyTile();
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
                CheckBlastedArea(x, y + 1, _tileController[x, y + 1], Vector2.down);
                CheckBlastedArea(x, blastedY, blastedTile, Vector2.down);
            }
            
        }
        
    }
}
