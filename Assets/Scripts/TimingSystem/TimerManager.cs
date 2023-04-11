using System.Collections;
using CanvasSystem;
using UnityEngine;


namespace TimingSystem
{
    public class TimerManager : MonoBehaviour
    {
        public static TimerManager Instance { get; private set; }

        public int gameTime;
        public int startingIn;

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
        }

        private IEnumerator StartOfGame()
        {
            CanvasManager.Instance.GameStartingVisible=true;
            CanvasManager.Instance.GameStartAnnouncement(startingIn);
            while (startingIn > 1)
            {
                yield return new WaitForSeconds(0.5f);
                startingIn -= 1;
                CanvasManager.Instance.GameStartAnnouncement(startingIn);
                yield return new WaitForSeconds(0.8f);
                
                if (startingIn <= 0)
                    break;
            }
            CanvasManager.Instance.BlockingPanelVisible=false;
            CanvasManager.Instance.GameStartingVisible=false;
            CanvasManager.Instance.ScoreVisible=true;
            CanvasManager.Instance.TimerVisible = true;
            
            StartCoroutine(DuringGame());
            yield return null;
        }

        private IEnumerator DuringGame()
        {
            CanvasManager.Instance.TimerText(gameTime);
            while (gameTime > 0)
            {
                yield return new WaitForSeconds(1.4f);
                gameTime -= 1;
                CanvasManager.Instance.TimerText(gameTime);

                if (gameTime <= 0)
                    break;
            }
            
            
        }
    }
}