using System.Collections;
using CanvasSystem;
using TMPro;
using UnityEngine;

namespace ScoreSystem
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; set; }
        public int score;
        public TextMeshProUGUI scoreTextMp;
        private int _newScore;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            score = 0;
            CanvasManager.Instance.ScoreText(score);
        }

        private IEnumerator IncreaseScore()
        {
            while (score != _newScore)
            {
                score += 1;
                CanvasManager.Instance.ScoreText(score);

                if (score == _newScore)
                    break;

                yield return new WaitForSeconds(0.001f);
            }
            yield return null;
        }


        public void CalculateAddingScore(int matchedTileNum)
        {
            var tempScore = matchedTileNum * 10;
            _newScore = score + tempScore;
            StartCoroutine(IncreaseScore());
        }
    }
}