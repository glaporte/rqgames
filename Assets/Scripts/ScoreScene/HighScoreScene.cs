using System;
using rqgames.MainMenu;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace rqgames.Score
{
    public class HighScoreScene : MonoBehaviour
    {
        public const string SceneName = "HighScoreScene";

        [SerializeField]
        private Button _menu;
        [SerializeField]
        private Button _reset;

        [SerializeField]
        private GameObject _highScoreContainer;
        [SerializeField]
        private GameObject _highScoreItemPrefab;

        private void Start()
        {
            _menu.onClick.AddListener(() => { SceneManager.LoadScene(MainMenuScene.SceneName, LoadSceneMode.Single); });
            _reset.onClick.AddListener(() =>
            {
                for (int i = 0; i < Init.GlobalVariables.Scores.Scores.Length; i++)
                    Init.GlobalVariables.Scores.Scores[i] = null;

                Init.GlobalVariables.Scores.Last = null;
                PlayerPrefs.SetString("scores", "");
                PlayerPrefs.Save();
                for (int i = 0; i < _highScoreContainer.transform.childCount; i++)
                    Destroy(_highScoreContainer.transform.GetChild(i).gameObject);
                Display();
            });

            Display();
        }

        private void Display()
        {
            for (int i = 0; i < Init.GlobalVariables.Scores.Scores.Length; i++)
            {
                var curScore = Init.GlobalVariables.Scores.Scores[i];
                if (curScore != null && curScore.CurrentScore > 0)
                {
                    GameObject instance = Instantiate(_highScoreItemPrefab);
                    instance.transform.SetParent(_highScoreContainer.transform, false);

                    TimeSpan t = TimeSpan.FromSeconds(curScore.TimeElapsed);
                    string time = string.Format("{0}s", t.Seconds);
                    if (t.Minutes > 0)
                        time = string.Format("{0}m and " + time, t.Minutes);

                    instance.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"<size=25><B>{curScore.Rank}.</B></size>" +
                        $" <B>{time}</B> of survival with <B>{curScore.CurrentWave}</b>" +
                        $" waves and <B>{curScore.CurrentScore}</B> enemies killed.";
                }
            }
        }

        private void Update()
        {

        }
    }
}