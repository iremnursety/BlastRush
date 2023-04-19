using System.Collections;
using CanvasSystem;
using ScoreSystem;
using UnityEngine;

namespace TimerSystem
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }

        public float gameTime;
        public float startingIn;

        public bool timeUp;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public void StartGame()
        {
            StartCoroutine(StartOfGame());
            timeUp = false;
        }


        private IEnumerator StartOfGame() //Pre Countdown.
        {
            UIManager.Instance.GameStartingVisible = true;
            UIManager.Instance.StartAnnouncement = Mathf.RoundToInt(startingIn);
            while (startingIn > 1)
            {
                if (startingIn == 0)
                    break;
                
                startingIn-=1*Time.deltaTime/2;
                UIManager.Instance.StartAnnouncement = Mathf.RoundToInt(startingIn);
             
            }
            UIManager.Instance.BlockingPanelVisible = false;
            UIManager.Instance.GameStartingVisible = false;

            UIManager.Instance.ScorePlayerVisible = true;
            UIManager.Instance.ScoreAIVisible = true;
            UIManager.Instance.GameTimeVisible = true;

            StartCoroutine(GameCountdown());
            yield return null;
        }

        private IEnumerator GameCountdown() //Main Game Countdown.
        {
            UIManager.Instance.GameTimeText = Mathf.RoundToInt(gameTime);
            
            while (gameTime > 0)
            {
                if (gameTime == 0)
                    break;
                
                gameTime-=1*Time.deltaTime/2;
                UIManager.Instance.GameTimeText = Mathf.RoundToInt(gameTime);
                
                yield return null;
            }
            UIManager.Instance.BlockingPanelVisible = false;
            timeUp = true;
            ScoreManager.Instance.WinnerIs();
        }
    }
}