using System;
using UnityEngine;

namespace rqgames.Game
{
    public class UIGame : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TextMeshProUGUI _wave;
        [SerializeField]
        private TMPro.TextMeshProUGUI _score;
        [SerializeField]
        private TMPro.TextMeshProUGUI _life;
        [SerializeField]
        private TMPro.TextMeshProUGUI _time;
        private Game _game;

        private void Start()
        {
        }

        public void Init(Game g)
        {
            _game = g;
            rqgames.Init.PooledGameData.Player.CurrentScore.OnChange.AddListener(RefreshUI);
            RefreshUI();
            InvokeRepeating(nameof(RefreshTime), 0, 1);
        }

        private void RefreshTime()
        {
            TimeSpan t = TimeSpan.FromSeconds(Time.timeSinceLevelLoad);
            string time = string.Format("{0}s", t.Seconds);
            if (t.Minutes > 0)
                time = string.Format("{0}m and " + time, t.Minutes);

            _time.text = $"TIME <B>{time}</B>";
        }

        private void RefreshUI()
        {
            SetScore(rqgames.Init.PooledGameData.Player.CurrentScore.CurrentScore);
            SetWave(rqgames.Init.PooledGameData.Player.CurrentScore.CurrentWave);
            SetLife(rqgames.Init.PooledGameData.Player.CurrentScore.CurrentLife);
        }

        public void SetWave(int number)
        {
            _wave.text = $"WAVE <b>{number}</b>";
        }

        public void SetLife(int number)
        {
            _life.text = $"LIFE <b>{number}</b>";
        }

        public void SetScore(int number)
        {
            _score.text = $"SCORE <b>{number}</b>";
        }

        private void Update()
        {
        
        }
    }
}
