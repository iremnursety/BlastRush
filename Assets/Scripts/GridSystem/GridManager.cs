using TimerSystem;
using UnityEngine;

namespace GridSystem
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }

        [Range(2, 12)] public int rows; // Rows numbers for Grid System.
        [Range(2, 12)] public int columns; //Columns numbers for Grid System
        [Range(1, 6)] public int typeNumber; //Tile type number at the start of the game.
        
        public void GameStarted()
        {
            Debug.Log("Game Started");
            TimeManager.Instance.StartGame();
        }
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            typeNumber = Random.Range(2, 6);
        }
        
        
    }
}