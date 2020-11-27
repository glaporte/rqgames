using UnityEngine;
using System.Collections.Generic;
using rqgames.Init;
using System.Collections;

namespace rqgames.Game
{
    public class Game : MonoBehaviour
    {
        private float _frustumWidth;
        public float TopY { get; internal set; }

        private const int xOffset = 3;
        private const float npcMoveTime = 0.5f;
        private const float waitBeforeMoveNextNpc = 0.35f;

        private int RowsXCell = 0;
        private int RowsSideSign = 1;
        private bool _hasChangedSign = false;

        private List<List<GameObject>> NPCs;

        private void Start()
        {
            NPCs = new List<List<GameObject>>();
            float frustumHeight = 2.0f * 20 * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
            _frustumWidth = (int)(frustumHeight * Camera.main.aspect);

            TopY = frustumHeight / 2;

            int yOffset = 3;

            int startX = -(Init.GlobalVariables.GameConfig.NpcCols * xOffset) / 2;
            int startY = (int)(TopY - 2);
            int curX = startX;

            PooledGameData.Player.StartGame(this);

            for (int i = 0; i < Init.GlobalVariables.GameConfig.NpcRows; i++)
            {
                List<GameObject> row = new List<GameObject>();
                NPCs.Add(row);
                for (int j = 0; j < Init.GlobalVariables.GameConfig.NpcCols; j++)
                {
                    Stack<GameObject> container = GetContainer();
                    if (container == null)
                        break;
                    GameObject npc = container.Pop();
                    npc.SetActive(true);
                    npc.transform.position = new Vector3(curX, startY, 0);
                    curX += xOffset;
                    row.Add(npc);
                }
                curX = startX;
                startY -= yOffset;
            }

            Invoke("MoveRows", Init.GlobalVariables.GameConfig.SwapNPCTick);
        }

        private void MoveRows()
        {
            if (Mathf.Abs(RowsXCell) >= 1 && !_hasChangedSign)
            {
                RowsSideSign *= -1;
                _hasChangedSign = true;
            }
            else
            {
                _hasChangedSign = false;
            }

            RowsXCell += RowsSideSign;

            for (int i = 0; i < NPCs.Count; i++)
            {
                int sign = i % 2 == 0 ? RowsSideSign : -RowsSideSign;
                List<GameObject> curRow = new List<GameObject>(NPCs[i]);
                if (sign < 0)
                    curRow.Reverse();
                StartCoroutine(MoveRow(curRow, i, sign));
            }
            Invoke("MoveRows", Init.GlobalVariables.GameConfig.SwapNPCTick + MoveTimeAll);
        }

        public float MoveTimeAll => (GlobalVariables.GameConfig.NpcRows - 1) +
            (GlobalVariables.GameConfig.NpcCols - 1) * npcMoveTime * waitBeforeMoveNextNpc + npcMoveTime * 2;

        private IEnumerator MoveRow(List<GameObject> npcs, int listIndex, int sign)
        {
            yield return new WaitForSeconds(listIndex);

            for (int i = 0; i < npcs.Count; i++)
            {
                GameObject npc = npcs[i];
                StartCoroutine(MoveNpc(npc, npc.transform.position + Vector3.right * sign * xOffset, i));
            }
        }

        private IEnumerator MoveNpc(GameObject npc, Vector3 dstPos, int listIndex)
        {
            yield return new WaitForSeconds(npcMoveTime * listIndex * waitBeforeMoveNextNpc);

            Vector3 startPos = npc.transform.position;
            float sign = Mathf.Sign(dstPos.x - npc.transform.position.x);
            GameEntities.NPCs.NPC npcC = npc.GetComponent<GameEntities.NPCs.NPC>();
            npcC.StartMove();
            float curTime = 0;

            // Percent value
            float startMove = 0.5f;
            float endMove = 0.4f;
            float moveTime = 0;
            float moveDuration = (1f - startMove) * npcMoveTime + endMove * npcMoveTime;

            while (curTime < npcMoveTime)
            {
                curTime += Time.deltaTime;
                npcC.Rotate(curTime / npcMoveTime * sign);
                if (curTime > startMove * npcMoveTime)
                {
                    npc.transform.position = Vector3.Lerp(startPos, dstPos, moveTime / moveDuration);
                    moveTime += Time.deltaTime;
                }
                yield return null;
            }

            curTime = 0;
            while (curTime < npcMoveTime)
            {
                curTime += Time.deltaTime;
                npcC.Rotate((1f - (curTime / npcMoveTime)) * sign);
                if (curTime < endMove * npcMoveTime)
                {
                    npc.transform.position = Vector3.Lerp(startPos, dstPos, moveTime / moveDuration);
                    moveTime += Time.deltaTime;
                }
                yield return null;
            }
            Debug.Log(moveTime + " & " + moveDuration);
            npcC.EndMove();
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
    }
}