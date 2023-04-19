using System.Collections;
using CanvasSystem;
using GridSystem;
using TimerSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AISystem
{
    public class AIController : MonoBehaviour
    {
        private TileController[,] _tileController;
        private int _selectedList;
        public GridController gridCont;
        public int thinkingTime;
        private bool _timeUp;
        
        private void Start()
        {
            StartCoroutine(AITurn());
        }

        private void Update()
        {
            _timeUp = TimeManager.Instance.timeUp;
        }

        private IEnumerator AITurn()//AI Random selection for tiles.
        {
            Debug.Log("Started Coroutine");
            _timeUp = TimeManager.Instance.timeUp;
            while (!_timeUp)
            {
                if (_timeUp)
                    break;
                if (UIManager.Instance.BlockingPanelVisible)
                {
                    Debug.Log("Blocking Test");
                    yield return new WaitForSeconds(thinkingTime);
                    continue;
                }
                if (!gridCont.canBlast)
                {
                    yield return new WaitForSeconds(1f);
                    continue;
                }
                if (_timeUp)
                    break;
                _tileController = gridCont.TileController;

                var randomX = Random.Range(0,_tileController.GetLength(0));
                var randomY = Random.Range(0,_tileController.GetLength(1));
                
                gridCont.CheckMatchingTiles(randomX,randomY);
                yield return new WaitForSeconds(1f);
                Debug.Log("Test");
            }
           
        }
    }
    
}
