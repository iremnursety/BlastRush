using UnityEditor;
using UnityEngine;

namespace GridSystem
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] private GameObject tilePrefab;
        private static int Rows => GridManager.Instance.rows;
        private static int Columns => GridManager.Instance.columns;
        

        private void Start()
        {
            CreateGrid();
        }
        
        private void CreateGrid() //Creating Grid System at the start of the game.
        {
            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Columns; j++)
                {
                    var newTile = PrefabUtility.InstantiatePrefab(tilePrefab, gameObject.transform) as GameObject;
                    
                    var tilePos = new Vector2(i, j);
                    if (newTile == null)
                        continue;

                    newTile.transform.localPosition = tilePos;
                    var newTileCont = newTile.GetComponent<TileController>();
                    newTileCont.dimensionPos = new Vector2(i, j);
                    newTile.name = $"Tile[{i} / {j}]";
                    
                    GridManager.Instance.TileController[i, j] = newTileCont;
                    GridManager.Instance.Grid[i, j] = newTile;
                }
            }
        }
        
    }
}
