﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace rqgames.MainMenuScene
{ 
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private Button _startButton;
        [SerializeField]
        private Button _highScoreButton;

        void Start()
        {
            _startButton.onClick.AddListener(() => StartGame());
            _highScoreButton.onClick.AddListener(() => HighScore());
        }

        private void StartGame()
        {
            SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        }

        private void HighScore()
        {

        }
    }
}
