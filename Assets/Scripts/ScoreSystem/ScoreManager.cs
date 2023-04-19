using System.Collections;
using CanvasSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScoreSystem
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }
        [FormerlySerializedAs("score")] public int scorePlayer;
        private int _newScore;
        public int scoreAI;
        private int _newScoreAI;
        

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            scorePlayer = 0;
            scoreAI = 0;
          
            UIManager.Instance.PlayerScoreText = scorePlayer;
            UIManager.Instance.AIScoreText = scoreAI;
        }

        private IEnumerator IncreaseScorePlayer()
        {
            while (scorePlayer != _newScore)
            {
                scorePlayer += 1;
                UIManager.Instance.PlayerScoreText=scorePlayer;

                if (scorePlayer == _newScore)
                    break;

                yield return new WaitForSeconds(0.001f);
            }
            yield return null;
        }

        private IEnumerator IncreaseScoreAI()
        {
            while (scoreAI != _newScoreAI)
            {
                scoreAI += 1;
                UIManager.Instance.AIScoreText=scoreAI;

                if (scoreAI == _newScoreAI)
                    break;

                yield return new WaitForSeconds(0.001f);
            }
            yield return null;
        }

        public void CalculateAddingPlayerScore(int matchedTileNum)
        {
            var tempScore = matchedTileNum * 10;
            _newScore = scorePlayer + tempScore;
            StartCoroutine(IncreaseScorePlayer());
        }
        public void CalculateAddAIScore(int matchedTileNum)
        {
            var tempScore = matchedTileNum * 10;
            _newScoreAI = scoreAI + tempScore;
            StartCoroutine(IncreaseScoreAI());
        }

        public void WinnerIs()
        {
            StartCoroutine(CheckWinner());
        }
        private IEnumerator CheckWinner()
        {
            yield return new WaitForSeconds(1f);
            UIManager.Instance.WinnerVisible = true;
            UIManager.Instance.WinnerText = scorePlayer > scoreAI ? "Player" : "Enemy";
        }
    }
}