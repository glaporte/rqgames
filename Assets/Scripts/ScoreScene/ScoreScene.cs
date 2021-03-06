﻿using System;
using rqgames.MainMenu;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace rqgames.Score
{
    public class ScoreScene : MonoBehaviour
    {
        public const string SceneName = "ScoreScene";

        [SerializeField]
        private TMPro.TextMeshProUGUI _rank;

        [SerializeField]
        private TMPro.TextMeshProUGUI _score;

        [SerializeField]
        private Button _playAgain;
        [SerializeField]
        private Button _menu;

        private void Start()
        {
            _playAgain.onClick.AddListener(() => { SceneManager.LoadScene(Game.Game.SceneName, LoadSceneMode.Single); });
            _menu.onClick.AddListener(() => { SceneManager.LoadScene(MainMenuScene.SceneName, LoadSceneMode.Single); });

            GameEntities.Playable.PlayerScore score = rqgames.Init.PooledGameData.Player.CurrentScore;
            TimeSpan t = TimeSpan.FromSeconds(score.TimeElapsed);
            string time = string.Format("{0}s", t.Seconds);
            if (t.Minutes > 0)
                time = string.Format("{0}m and " + time, t.Minutes);

            _score.text = $"<B>{time}</B> of survival with <B>{score.CurrentWave}</b> waves and <B>{score.CurrentScore}</B> enemies killed.";

            string rank = "too bad to be displayed.";
            if (score.Rank > 0)
                rank = score.Rank.ToString();
            _rank.text = $"Your rank for this game is <B>{rank}</B>";
        }
    }
}
