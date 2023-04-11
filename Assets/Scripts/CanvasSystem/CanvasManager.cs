using TMPro;
using UnityEngine;

namespace CanvasSystem
{
    public class CanvasManager : MonoBehaviour
    {
        public static CanvasManager Instance { get; private set; }

        public TextMeshProUGUI gameStarting;
        public TextMeshProUGUI timerText;
        public GameObject blockingPanel;
        public TextMeshProUGUI scoreText;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            GameStartingVisible = false;
            TimerVisible = false;
            BlockingPanelVisible = true;
            ScoreVisible = false;
        }

        public void GameStartAnnouncement(int startingIn)
        {
            gameStarting.text = "Game Is Starting in " + startingIn + "..";
        }

        public void TimerText(int gameTime)
        {
            timerText.text = gameTime + " Sec" + " Left..";
        }

        public void ScoreText(int score)
        {
            scoreText.text = "Score: " + score;
        }

        public bool TimerVisible
        {
            //get => timerText;
            set => timerText.gameObject.SetActive(value);
        }

        public bool BlockingPanelVisible
        {
            //get => blockingPanel;
            set => blockingPanel.gameObject.SetActive(value);
        }

        public bool ScoreVisible
        {
            //get => scoreText;
            set => scoreText.gameObject.SetActive(value);
        }

        public bool GameStartingVisible
        {
            //get => gameStarting;
            set => gameStarting.gameObject.SetActive(value);
        }
    }
}