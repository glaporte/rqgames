using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using rqgames.GameEntities;

namespace rqgames.Init
{
    public static class GlobalVariables
    {
        public static gameconfig.GameConfig GameConfig;
    }

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
        public static rqgames.GameEntities.Playable.Player Player;
        public static List<Stack<GameObject>> NPCs;
        public static Stack<GameObject> Weapons;
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
            GlobalVariables.GameConfig = _gameConfig;
        }

        private void Start()
        {
            PooledGameData.NPCs = new List<Stack<GameObject>>();
            PooledGameData.NPCs.Add(new Stack<GameObject>());
            PooledGameData.NPCs.Add(new Stack<GameObject>());
            PooledGameData.NPCs.Add(new Stack<GameObject>());

            PooledGameData.Weapons = new Stack<GameObject>();
            Addressables.LoadAssetsAsync<GameObject>(_assetLabelReference, LoadCallback).Completed += LoadingGameData_Completed;
        }

        private void LoadCallback(GameObject obj)
        {
            if (obj.tag == "Player")
                StartCoroutine(HandlePlayer(obj));
            else if (obj.GetComponent<rqgames.GameEntities.NPCs.NPC1>() != null)
            {
                LoadedGameData.NPC1 = obj;
                StartCoroutine(HandlePooled(obj, _gameConfig.CountNpc1, PooledGameData.NPCs[0]));
            }
            else if (obj.GetComponent<rqgames.GameEntities.NPCs.NPC2>() != null)
            {
                LoadedGameData.NPC2 = obj;
                StartCoroutine(HandlePooled(obj, _gameConfig.CountNpc2, PooledGameData.NPCs[1]));
            }
            else if (obj.GetComponent<rqgames.GameEntities.NPCs.NPC3>() != null)
            {
                LoadedGameData.NPC3 = obj;
                StartCoroutine(HandlePooled(obj, _gameConfig.CountNpc3, PooledGameData.NPCs[2]));
            }
            else if (obj.GetComponent<rqgames.GameEntities.Weapon>() != null)
            {
                LoadedGameData.Weapon = obj;
                StartCoroutine(HandlePooled(obj, _gameConfig.CountMaxWeapon, PooledGameData.Weapons));
            }
            else
                Debug.Log(obj.name + " not handled for loading.");
        }

        private IEnumerator HandlePlayer(GameObject pl)
        {
            if (LoadedGameData.Player != null)
                yield break;
            LoadedGameData.Player = pl;
            GameObject instance = Instantiate(pl);
            DontDestroyOnLoad(instance);

            PooledGameData.Player = instance.GetComponentInChildren<GameEntities.Playable.Player>();
            PooledGameData.Player.gameObject.SetActive(false);
            yield break;
        }

        private IEnumerator HandlePooled(GameObject npc, int max, Stack<GameObject> container)
        {
            for (int i = 0; i < max; i++)
            {
                GameObject go = Instantiate(npc);
                go.GetComponent<IPooledGameEntities>().Container = container;
                DontDestroyOnLoad(go);
                container.Push(go);
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