using UnityEngine;
using System.Collections.Generic;
using rqgames.Init;

namespace rqgames.Game
{
    public class Game : MonoBehaviour
    {
        private float _frustumWidth;
        private float _topY;


        private void Start()
        {
            float frustumHeight = 2.0f * 20 * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
            _frustumWidth = (int)(frustumHeight * Camera.main.aspect);

            _topY = frustumHeight / 2;

            int xOffset = 4;
            int yOffset = 3;

            int startX = -(Init.GlobalVariables.GameConfig.NpcCols * xOffset) / 2;
            int startY = (int)(_topY - 2);
            int curX = startX;

            for (int i = 0; i < Init.GlobalVariables.GameConfig.NpcRows; i++)
            {
                for (int j = 0; j < Init.GlobalVariables.GameConfig.NpcCols; j++)
                {
                    Stack<GameObject> container = GetContainer();
                    if (container == null)
                        break;
                    GameObject npc = container.Pop();
                    npc.SetActive(true);
                    npc.transform.position = new Vector3(curX, startY, 0);
                    curX += xOffset;
                }
                curX = startX;
                startY -= yOffset;
            }
        }

        private Stack<GameObject> GetContainer()
        {
            float ratio1 = PooledGameData.NPCs[0].Count > 0 ? Init.GlobalVariables.GameConfig.Npc1Ratio : 0;
            float ratio2 = PooledGameData.NPCs[1].Count > 0 ? Init.GlobalVariables.GameConfig.Npc2Ratio : 0;
            float ratio3 = PooledGameData.NPCs[2].Count > 0 ? Init.GlobalVariables.GameConfig.Npc3Ratio : 0;

            int index = Utils.CommonUtils.GetRandomWeightedIndex(ratio1, ratio2, ratio3);
            if (index == -1)
                return null;
            return PooledGameData.NPCs[index];
        }

        private void Update()
        {
        
        }
    }
}