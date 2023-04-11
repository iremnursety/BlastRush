using System.Collections;
using System.Collections.Generic;
using ScoreSystem;
using TimingSystem;
using UnityEngine;

namespace GridSystem
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }

        [Range(2, 12)] public int rows; // Rows numbers for Grid System.
        [Range(2, 12)] public int columns; //Columns numbers for Grid System
        [Range(1, 6)] public int typeNumber; //Tile type number at the start of the game.
        public GameObject[,] Grid;
        public TileController[,] TileController;
        public List<TileController> matchingTiles = new List<TileController>();
        public bool canBlast;
        private Coroutine _activeCoroutine;
        

        public void GameStarted()
        {
            TimerManager.Instance.StartGame();
        }
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            TileController = new TileController[rows, columns];
            Grid = new GameObject[rows, columns];
        }

        private void Start()
        {
            canBlast = true;
        }

        private void Update()
        {
            canBlast = _activeCoroutine == null;
        }

        //TODO: Add Adjacent Tiles on a List as a group for check TileStates.
        public void CheckMatchingTiles(int x, int y)
        {
            //Resetting Matching List of Tiles.
            matchingTiles = new List<TileController>();

            var tileType = TileController[x, y].tileType;

            //Checking tiles for Adjacent on all directions.
            CheckAdjacentTiles(x, y, tileType, Vector2.up);
            CheckAdjacentTiles(x, y, tileType, Vector2.down);
            CheckAdjacentTiles(x, y, tileType, Vector2.left);
            CheckAdjacentTiles(x, y, tileType, Vector2.right);

            //If there are more than 1 matching tiles, add them to the matching list
            if (matchingTiles.Count < 1)
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
                tile.childObj.BlastAnimation();
            }
            yield return new WaitForSeconds(0.3f);
            foreach (var tile in matchingTiles)
            {
                tile.childObj.ActivateParticleSystem();
            }
            yield return new WaitForSeconds(0.3f);
            foreach (var tile in matchingTiles)
            {
                Destroy(tile.childObj.gameObject);
            }
            ScoreManager.Instance.CalculateAddingScore(matchingTiles.Count);
            _activeCoroutine = StartCoroutine(AfterBlast());
        }
        private IEnumerator AfterBlast() //For check all tiles for empty types.
        {
            foreach (var tile in TileController)
            {
                if (tile.childObj)
                    CheckBlastedArea(Mathf.RoundToInt(tile.dimensionPos.x), Mathf.RoundToInt(tile.dimensionPos.y), tile,
                        Vector2.down);
                yield return null;
            }
            foreach (var tile in TileController)
            {
                if (!tile.childObj && Mathf.RoundToInt(tile.dimensionPos.y) == columns - 1)
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
            if (blastedY < 0 || blastedY >= TileController.GetLength(1))
                return;

            var blastedTile = TileController[x, blastedY];

            //Checking the blasted tile If childCount more then 0 for return.

            if (blastedTile.transform.childCount > 0)
                return;

            /*Pre-check for tile y axis if its equals to the last axis of the board. If the tiles
             has no child then gonna activate TileController's FillEmptyTile function.*/

            if (y == columns - 1 && tile.transform.childCount == 0)
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
            if (y == columns - 1)
                CheckBlastedArea(x, blastedY, blastedTile, Vector2.down);
            else
            {
                CheckBlastedArea(x, y + 1, TileController[x, y + 1], Vector2.down);
                CheckBlastedArea(x, blastedY, blastedTile, Vector2.down);
            }
            
        }
        
    }
}