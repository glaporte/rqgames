using System;
using UnityEngine;

namespace rqgames.Score
{
    public class ScoreScene : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TextMeshProUGUI _rank;

        [SerializeField]
        private TMPro.TextMeshProUGUI _score;

        

        private void Start()
        {
            GameEntities.Playable.PlayerScore score = rqgames.Init.PooledGameData.Player.CurrentScore;
            TimeSpan t = TimeSpan.FromSeconds(score.TimeElapsed);
            string time = string.Format("{0}s", t.Seconds);
            if (t.Minutes > 0)
                time = string.Format("{0}m and " + time, t.Minutes);

            _score.text = $"<B>{time}</B> of survival with <B>{score.CurrentWave}</b> waves and <B>{score.CurrentScore}</B> enemies killed.";

            string rank = "too bad";
            if (score.Rank > 0)
                rank = (score.Rank + 1).ToString();
            _rank.text = $"Your rank for this game is <B>{rank}</B>";

        }
    }
}
