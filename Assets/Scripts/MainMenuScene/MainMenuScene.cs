using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace rqgames.MainMenu
{ 
    public class MainMenuScene : MonoBehaviour
    {
        public const string SceneName = "MainMenuScene";

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
            SceneManager.LoadScene(Game.Game.SceneName, LoadSceneMode.Single);
        }

        private void HighScore()
        {

        }
    }
}
