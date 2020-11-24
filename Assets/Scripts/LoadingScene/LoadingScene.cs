using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace rqgames.Loading
{
    public static class LoadedGameData
    {
        public static GameObject Player;
        public static GameObject NPC1;
        public static GameObject NPC2;
        public static GameObject NPC3;
        public static GameObject Weapon;
    }

    public static class PooledGameData
    {
        public static GameObject Player;
        public static List<GameObject> NPC1s;
        public static List<GameObject> NPC2s;
        public static List<GameObject> NPC3s;
        public static List<GameObject> Weapons;
    }

    public class LoadingScene : MonoBehaviour
    {
        [SerializeField]
        private AssetLabelReference _assetLabelReference;

        [SerializeField]
        private gameconfig.GameConfig _gameConfig;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            PooledGameData.NPC1s = new List<GameObject>();
            PooledGameData.NPC2s = new List<GameObject>();
            PooledGameData.NPC3s = new List<GameObject>();
            PooledGameData.Weapons = new List<GameObject>();
            Addressables.LoadAssetsAsync<GameObject>(_assetLabelReference, LoadCallback).Completed += LoadingGameData_Completed;
        }

        private void LoadCallback(GameObject obj)
        {
            if (obj.GetComponent<rqgames.GameEntities.Player>() != null)
                StartCoroutine(HandlePlayer(obj));
            else if (obj.GetComponent<rqgames.GameEntities.NPC1>() != null)
            {
                LoadedGameData.NPC1 = obj;
                StartCoroutine(HandleNPCs(obj, _gameConfig.LOAD_AND_MAX_NPC1, PooledGameData.NPC1s));
            }
            else if (obj.GetComponent<rqgames.GameEntities.NPC2>() != null)
            {
                LoadedGameData.NPC2 = obj;
                StartCoroutine(HandleNPCs(obj, _gameConfig.LOAD_AND_MAX_NPC2, PooledGameData.NPC2s));
            }
            else if (obj.GetComponent<rqgames.GameEntities.NPC3>() != null)
            {
                LoadedGameData.NPC3 = obj;
                StartCoroutine(HandleNPCs(obj, _gameConfig.LOAD_AND_MAX_NPC3, PooledGameData.NPC3s));
            }
            else if (obj.GetComponent<rqgames.GameEntities.Weapon>() != null)
            {
                LoadedGameData.Weapon = obj;
                StartCoroutine(HandleNPCs(obj, _gameConfig.LOAD_AND_MAX_WEAPON, PooledGameData.Weapons));
            }
            else
                Debug.Log(obj.name + " not handled for loading.");
        }

        private IEnumerator HandlePlayer(GameObject pl)
        {
            if (LoadedGameData.Player != null)
                yield break;
            LoadedGameData.Player = pl;
            PooledGameData.Player = Instantiate(pl);
            DontDestroyOnLoad(PooledGameData.Player);
            PooledGameData.Player.SetActive(false);
            yield break;
        }

        private IEnumerator HandleNPCs(GameObject npc, int max, List<GameObject> container)
        {
            for (int i = 0; i < max; i++)
            {
                GameObject go = Instantiate(npc);
                DontDestroyOnLoad(go);
                container.Add(go);
                go.SetActive(false);
                yield return null;
            }
        }
  

        private void LoadingGameData_Completed(AsyncOperationHandle<IList<GameObject>> obj)
        {
            SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
        }
    }
}