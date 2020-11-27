using UnityEngine;
using System.Collections.Generic;
using rqgames.Init;
using System.Collections;

namespace rqgames.Game
{
    public class Game : MonoBehaviour
    {
        public class MovementData
        {
            public int CountPassXMove = 0;
            public bool MoveY;

            public const int xOffsetNPC = 3;
            public const int yOffsetNPC = 3;

            public const float npcMoveTime = 0.3f;
            public const float waitBeforeMoveNextNpc = 0.35f; // percentage

            public const float MoveXRowWait = 0.3f;
        }

        public class Wave
        {
            public List<GameObject> npcs = new List<GameObject>();
            public int Depth;
            public int Killed = 0;
            public int XCell = 0;
            public int RowsSideSign = 1;
            public bool HasChangedSign = false;
        }

        public const string TeleportTag = "Teleport";

        [SerializeField]
        private UIGame _ui;
        [SerializeField]
        private Transform _teleportLeft;
        [SerializeField]
        private Transform _teleportRight;

        private MovementData _moveData;


        private List<Wave> NPCs;
        public float TopY { get; internal set; }

        private void Start()
        {
            NPCs = new List<Wave>();
            _moveData = new MovementData();
            float frustumHeight = 2.0f * 20 * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
            TopY = frustumHeight / 2;
            //float _frustumWidth = (int)(frustumHeight * Camera.main.aspect);


            InitGrid();
            PooledGameData.Player.StartGame(this);
            _ui.Init(this);
        }

        private int StartX => -(Init.GlobalVariables.GameConfig.NpcCols * MovementData.xOffsetNPC) / 2;
        private int StartY => (int)(TopY - 2);

        private void InitGrid()
        {
            int startY = StartY;

            for (int i = 0; i < Init.GlobalVariables.GameConfig.NpcRows; i++)
            {
                Wave row = new Wave();
                row.Depth = i;
                row.RowsSideSign = i % 2 == 1 ? -1 : 1;
                NPCs.Add(row);
                CreateRow(startY, row);
                startY -= MovementData.yOffsetNPC;
            }

            InvokeRepeating(nameof(MoveRows), Init.GlobalVariables.GameConfig.SwapNPCTick,
                Init.GlobalVariables.GameConfig.SwapNPCTick + GlobalMoveXTime);
        }

        private void CreateRow(int startY, Wave row)
        {
            int curX = StartX;

            for (int j = 0; j < Init.GlobalVariables.GameConfig.NpcCols; j++)
            {
                Stack<GameObject> container = GetNPCContainer();
                if (container == null)
                    return;
                GameObject npc = container.Pop();
                npc.GetComponent<GameEntities.NPCs.NPC>().Reset(this, row, startY);
                npc.SetActive(true);
                npc.transform.position = new Vector3(curX, startY, 0);
                curX += MovementData.xOffsetNPC;
                row.npcs.Add(npc);
            }
        }

        public void NPCDie(GameEntities.NPCs.NPC npc, Wave wave)
        {
            wave.npcs.Remove(npc.gameObject);
            if (wave.npcs.Count == 0)
            {
                if (wave.Killed == GlobalVariables.GameConfig.NpcCols)
                    PooledGameData.Player.CurrentScore.CurrentWave++;
                NPCs.Remove(wave);
                if (wave.Depth <= GlobalVariables.GameConfig.NpcRows)
                {
                    StartCoroutine(MoveForwardCondition(wave.Depth));
                }
            }
        }

        private IEnumerator MoveForwardCondition(int depth)
        {
            while (_moveData.MoveY)
                yield return null;
            MoveForward(depth);
        }

        public float GlobalMoveYTime(int depth) => depth * MovementData.npcMoveTime + MovementData.npcMoveTime * 2;

        public float GlobalMoveXTime => (GlobalVariables.GameConfig.NpcRows) * MovementData.MoveXRowWait +
            (GlobalVariables.GameConfig.NpcCols - 1) * MovementData.npcMoveTime * MovementData.waitBeforeMoveNextNpc + MovementData.npcMoveTime * 2;

        private void MoveForward(int startDepth)
        {
            _moveData.MoveY = true;
            for (int i = NPCs.Count - 1; i >= 0; i--)
            {
                if (NPCs[i].Depth >= startDepth)
                    continue;
                NPCs[i].Depth++;
                if (NPCs[i].Depth > GlobalVariables.GameConfig.NpcRows)
                {
                    int count = NPCs[i].npcs.Count;
                    for (int j = 0; j < count; j++)
                    {
                        GameObject go = NPCs[i].npcs[0]; // OnDie will remove it from container
                        go.GetComponent<rqgames.GameEntities.NPCs.NPC>().OnDie(false);
                    }
                    continue;
                }

                List<GameObject> curRow = NPCs[i].npcs;
                StartCoroutine(MoveRow(curRow, NPCs.Count - i - 1, 0, 1));
            }
            Invoke(nameof(CreateNewRow), GlobalMoveYTime(startDepth));
            Invoke(nameof(ResetMoveY), GlobalMoveYTime(startDepth));
        }
        

        private void CreateNewRow()
        {
            Wave row = new Wave();
            row.Depth = 0;
            if (NPCs.Count > 0)
                row.RowsSideSign = -NPCs[0].RowsSideSign;
            NPCs.Insert(0, row);
            CreateRow(StartY, row);
        }

        private void MoveRows()
        {
            if (_moveData.CountPassXMove > 0 && _moveData.CountPassXMove % 4 == 0)
            {
                _moveData.CountPassXMove++;
                StartCoroutine(MoveForwardCondition(GlobalVariables.GameConfig.NpcRows));
                return;
            }
            _moveData.CountPassXMove++;

            for (int i = 0; i < NPCs.Count; i++)
            {
                if (Mathf.Abs(NPCs[i].XCell) >= 1 && !NPCs[i].HasChangedSign)
                {
                    NPCs[i].RowsSideSign *= -1;
                    NPCs[i].HasChangedSign = true;
                }
                else
                    NPCs[i].HasChangedSign = false;
                int sign = NPCs[i].RowsSideSign;
                NPCs[i].XCell += NPCs[i].RowsSideSign;

                List<GameObject> curRow = new List<GameObject>(NPCs[i].npcs);
                if (sign < 0)
                    curRow.Reverse();
                StartCoroutine(MoveRow(curRow, i, sign));
            }
        }

        private void ResetMoveY()
        {
            _moveData.MoveY = false;
        }

        private IEnumerator MoveRow(List<GameObject> npcs, int listIndex, int sign, int ySign = 0)
        {
            if (ySign != 0)
                yield return new WaitForSeconds(listIndex * MovementData.npcMoveTime);
            else
                yield return new WaitForSeconds(listIndex * MovementData.MoveXRowWait);

            for (int i = 0; i < npcs.Count; i++)
            {
                GameObject npc = npcs[i];
                StartCoroutine(MoveNpc(npc, npc.transform.position
                    + Vector3.right * sign * MovementData.xOffsetNPC
                    + Vector3.down * ySign * MovementData.yOffsetNPC
                    , i, new Vector2(sign, ySign)));
            }
        }

        private IEnumerator MoveNpc(GameObject npc, Vector3 dstPos, int listIndex, Vector2 signs)
        {
            if (signs.x != 0)
                yield return new WaitForSeconds(MovementData.npcMoveTime * listIndex * MovementData.waitBeforeMoveNextNpc);

            Vector3 startPos = npc.transform.position;
            float sign = signs.x;
            GameEntities.NPCs.NPC npcC = npc.GetComponent<GameEntities.NPCs.NPC>();
            npcC.StartMove();
            float curTime = 0;

            // Percent value
            float startMove = 0.5f; // percent value
            float endMove = 0.4f; // percent value
            float moveTime = 0;
            float moveDuration = (1f - startMove) * MovementData.npcMoveTime + endMove * MovementData.npcMoveTime;

            while (curTime < MovementData.npcMoveTime)
            {
                curTime += Time.deltaTime;
                if (sign != 0)
                    npcC.Rotate(curTime / MovementData.npcMoveTime * sign);
                if (curTime >= startMove * MovementData.npcMoveTime)
                {
                    moveTime += Time.deltaTime;
                    npc.transform.position = Vector3.Lerp(startPos, ModifyPosAccordingSign(npc, dstPos, signs), moveTime / moveDuration);
                }
                yield return null;
            }
            moveTime += Time.deltaTime;
            curTime = 0;
            while (curTime < MovementData.npcMoveTime)
            {
                curTime += Time.deltaTime;
                if (sign != 0)
                    npcC.Rotate((1f - (curTime / MovementData.npcMoveTime)) * sign);
                if (curTime <= endMove * MovementData.npcMoveTime)
                {
                    moveTime += Time.deltaTime;
                    npc.transform.position = Vector3.Lerp(startPos, ModifyPosAccordingSign(npc, dstPos, signs), moveTime / moveDuration);
                }
                yield return null;
            }
            npc.transform.position = dstPos;

            npcC.EndMove();
        }

        private Vector3 ModifyPosAccordingSign(GameObject npc, Vector3 dstPos ,Vector2 signs)
        {
            if (signs.x == 0)
                dstPos.x = npc.transform.position.x;
            if (signs.y == 0)
                dstPos.y = npc.transform.position.y;
            return dstPos;
        }

        private Stack<GameObject> GetNPCContainer()
        {
            float ratio1 = PooledGameData.NPCs[0].Count > 0 ? Init.GlobalVariables.GameConfig.Npc1Ratio : 0;
            float ratio2 = PooledGameData.NPCs[1].Count > 0 ? Init.GlobalVariables.GameConfig.Npc2Ratio : 0;
            float ratio3 = PooledGameData.NPCs[2].Count > 0 ? Init.GlobalVariables.GameConfig.Npc3Ratio : 0;

            int index = Utils.CommonUtils.GetRandomWeightedIndex(ratio1, ratio2, ratio3);
            if (index == -1)
                return null;
            return PooledGameData.NPCs[index];
        }

        public void TakeTeleport(GameObject src, GameObject teleport)
        {
            src.transform.position = teleport.transform == _teleportLeft
                ? _teleportRight.transform.GetChild(0).position :_teleportLeft.transform.GetChild(0).position;
        }
    }
}