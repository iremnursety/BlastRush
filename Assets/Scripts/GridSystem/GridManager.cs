using System.Collections;
using System.Collections.Generic;
using AISystem;
using CanvasSystem;
using ScoreSystem;
using TimerSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GridSystem
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }

        [Range(2, 12)] public int rows; // Rows numbers for Grid System.
        [Range(2, 12)] public int columns; //Columns numbers for Grid System
        [Range(1, 6)] public int typeNumber; //Tile type number at the start of the game.
        public List<GridController> gridControllers = new List<GridController>();
        public List<AIController> aiControllers = new List<AIController>();
        public bool powerUpAvailable;

        public void GameStarted()
        {
            Debug.Log("Game Started");
            TimeManager.Instance.StartGame();
            
            foreach (var ai in aiControllers)
            {
                ai.StartAI();
            }
        }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            typeNumber = Random.Range(2, 6);
        }

        public void ResetGame()
        {
            typeNumber = Random.Range(2, 6);
            StartCoroutine(DestroyTiles());
        }

        private IEnumerator DestroyTiles()
        {
            TimeManager.Instance.resetGame = true;

            UIManager.Instance.ResetGame();
            ScoreManager.Instance.ResetScores();
            TimeManager.Instance.ResetGame();
        

            foreach (var ai in aiControllers)
            {
                ai.resetGame = true;
            }

            foreach (var gridController in gridControllers)
            {
                gridController.ResetGame();
                yield return null;
            }
        }

        public void TimeIsUp()
        {
            foreach (var gridCont in gridControllers)
            {
                gridCont.TimeIsUp();
            }
        }
    }
}