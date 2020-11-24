using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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

    public class LoadingScene : MonoBehaviour
    {
        [SerializeField]
        private AssetLabelReference _assetLabelReference;

        private void Start()
        {
            Addressables.LoadAssetsAsync<GameObject>(_assetLabelReference, LoadCallback).Completed += LoadingGameData_Completed;
        }

        private void LoadCallback(GameObject obj)
        {
            if (obj.GetComponent<rqgames.GameEntities.Player>() != null)
                LoadedGameData.Player = obj;
            else if (obj.GetComponent<rqgames.GameEntities.NPC1>() != null)
                LoadedGameData.NPC1 = obj;
            else if (obj.GetComponent<rqgames.GameEntities.NPC2>() != null)
                LoadedGameData.NPC2 = obj;
            else if (obj.GetComponent<rqgames.GameEntities.NPC3>() != null)
                LoadedGameData.NPC3 = obj;
            else if (obj.GetComponent<rqgames.GameEntities.Weapon>() != null)
                LoadedGameData.Weapon = obj;
            else
                Debug.Log(obj.name + " not handled for loading.");
        }

        private void LoadingGameData_Completed(AsyncOperationHandle<IList<GameObject>> obj)
        {

        }
    }
}