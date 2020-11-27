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

        private Game _game;

        private void Start()
        {
        }

        public void Init(Game g)
        {
            _game = g;
            rqgames.Init.PooledGameData.Player.CurrentScore.OnChange.AddListener(RefreshUI);
            RefreshUI();
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
