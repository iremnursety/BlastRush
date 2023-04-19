using TMPro;
using UnityEngine;

namespace CanvasSystem
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        public TextMeshProUGUI gameStarting;
        public TextMeshProUGUI timerText;
        public GameObject blockingPanel;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI scoreAIText;
        public TextMeshProUGUI winner;
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            GameStartingVisible = false;
            GameTimeVisible = false;
            BlockingPanelVisible = true;
            ScorePlayerVisible = false;
            ScoreAIVisible = false;
            WinnerVisible = false;
        }

      
        public int StartAnnouncement
        {
            set=> gameStarting.text = "Game Is Starting in " + value + "..";
        }

        public int GameTimeText
        {
            set => timerText.text = value + " Sec Left..";
        }

        public int PlayerScoreText
        {
            set => scoreText.text = "Your Score: " + value;
        }
        public int AIScoreText
        {
            set => scoreAIText.text = "Enemy Score: " + value;
        }
        public string WinnerText
        {
            set => winner.text = "Winner is " + value;
        }
        public bool WinnerVisible
        {
            set => winner.gameObject.SetActive(value);
        }
        public bool GameStartingVisible
        {
            set => gameStarting.gameObject.SetActive(value);
        }
        public bool GameTimeVisible
        {
            set => timerText.gameObject.SetActive(value);
        }

        public bool BlockingPanelVisible
        {
            get => blockingPanel.activeSelf;
            set => blockingPanel.gameObject.SetActive(value);
        }

        public bool ScorePlayerVisible
        {
            set => scoreText.gameObject.SetActive(value);
        }
        public bool ScoreAIVisible
        {
            set => scoreAIText.gameObject.SetActive(value);
        }
        
    }
}