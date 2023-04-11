using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace GridSystem
{
    public class TileController : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private List<GameObject> typePrefabs = new List<GameObject>();
        public TileTypes tileType;
        public TileStates tileState;
        public Vector2 dimensionPos;
        public TileChildController childObj;
        public GridController gridCont;
        private bool _canBlast;
        private int TileTypeNumber => GridManager.Instance.typeNumber;

        private void Start()
        {
            FillTiles();
        }

        private void Update()
        {
            //_canBlast=GridManager.Instance.canBlast;
            _canBlast = gridCont.canBlast;
        }

        public void FllEmptyTile() //Added for reach from GridManager.
        {
            StartCoroutine(Fill());
        }
        private IEnumerator Fill() //For fill the empty tile.
        {
            yield return new WaitForSeconds(0.01f);
            FillTiles();
            //GridManager.Instance.CheckBlastedArea(Mathf.RoundToInt(dimensionPos.x),Mathf.RoundToInt(dimensionPos.y),this,Vector2.down);
            gridCont.CheckBlastedArea(Mathf.RoundToInt(dimensionPos.x), Mathf.RoundToInt(dimensionPos.y), this,
                Vector2.down);
        }
        private void FillTiles() //Filling the Tiles.
        {
            var randomTileType = Random.Range(0, TileTypeNumber);

            tileType = randomTileType switch
            {
                0 => TileTypes.Type1,
                1 => TileTypes.Type2,
                2 => TileTypes.Type3,
                3 => TileTypes.Type4,
                4 => TileTypes.Type5,
                5 => TileTypes.Type6,
                _ => TileTypes.Empty
            };

            //Instantiate the random TileType as Child object of TileController.
            var newTile =
                Instantiate(typePrefabs[randomTileType], gameObject.transform);
            if (newTile != null)
                childObj = newTile.GetComponent<TileChildController>();
            
            childObj.CreatedChild();
            childObj.FallingAnimation();
            //_activeCoroutine = null;
        }

        //Sending Tile Information to the GridManager with PointerClick for check Matching Tiles.
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_canBlast)
                return;
            //GridManager.Instance.CheckMatchingTiles(Mathf.RoundToInt(dimensionPos.x), Mathf.RoundToInt(dimensionPos.y));
            gridCont.CheckMatchingTiles(Mathf.RoundToInt(dimensionPos.x), Mathf.RoundToInt(dimensionPos.y));

        }
        
    }
}