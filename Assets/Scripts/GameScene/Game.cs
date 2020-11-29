using UnityEngine;
using System.Collections.Generic;
using rqgames.Init;
using System.Collections;

namespace rqgames.Game
{
    public class Game : MonoBehaviour
    {
        public const string SceneName = "GameScene";

        public class MovementData
        {
            public int CountPassXMove = 0;

            public const int xOffsetNPC = 3;
            public const int yOffsetNPC = 3;

            public const float npcMoveTime = 0.3f;
            public const float waitBeforeMoveNextNpc = 0.35f; // percentage

            public const float MoveXRowWait = 0.3f;
        }

        public class Wave
        {
            private Game _game;
            public Wave(Game game)
            {
                _game = game;
            }
            public List<GameObject> npcs = new List<GameObject>();
            public int Depth;
            public int TargetDepth;

            public int XCell = 0;
            public int RowsSideSign = 1;
            public bool HasChangedSign = false;

            private float _moveTime;

            public void Update(float dt)
            {
                if (Depth != TargetDepth)
                {
                    _moveTime += dt;
                    SetNPCsY(_moveTime / MovementData.npcMoveTime);
                    if (_moveTime >= MovementData.npcMoveTime)
                    {
                        SetNPCsY(1);
                        _moveTime = 0;
                        Depth++;
                        CheckOut();
                    }
                }
            }

            private void SetNPCsY(float percent)
            {
                for (int i = 0; i < npcs.Count; i++)
                {
                    Vector3 targetPos = npcs[i].transform.position;
                    targetPos.y = Mathf.Lerp(targetPos.y, _game.StartY - (MovementData.yOffsetNPC) * (Depth + 1), percent);
                    npcs[i].transform.position = targetPos;
                }
            }

            private void CheckOut()
            {
                if (Depth > GlobalVariables.GameConfig.NpcRows)
                {
                    int count = npcs.Count;
                    for (int j = 0; j < count; j++)
                    {
                        GameObject go = npcs[0]; // OnDie will remove it from container
                        go.GetComponent<rqgames.GameEntities.NPCs.NPC>().OnDie(false);
                    }
                }
            }
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

        private int StartX => -(Init.GlobalVariables.GameConfig.NpcCols * MovementData.xOffsetNPC) / 2;
        private int StartY => (int)(TopY - 2);

        public float TopY { get; internal set; }
        public float GlobalMoveXTime => (GlobalVariables.GameConfig.NpcRows) * MovementData.MoveXRowWait +
            (GlobalVariables.GameConfig.NpcCols - 1) * MovementData.npcMoveTime * MovementData.waitBeforeMoveNextNpc + MovementData.npcMoveTime * 2;

        public int Difficulty { get; internal set; } = 100; // if _countAttack is max (10), each second attack has _countAttack / _difficulty chance to proc.
        public int FancyDifficulty { get; internal set; } = 1;
        public const int MaxDifficultyLevel = 10;
        public float TimerDifficulty { get; internal set; }

        private void Start()
        {
            NPCs = new List<Wave>();
            _moveData = new MovementData();
            float frustumHeight = 2.0f * 20 * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
            TopY = frustumHeight / 2;

            InitGrid();
            PooledGameData.Player.StartGame(this);
            _ui.Init(this);
        }

        private void InitGrid()
        {
            int startY = StartY;

            for (int i = 0; i < Init.GlobalVariables.GameConfig.NpcRows; i++)
            {
                Wave row = new Wave(this);
                row.RowsSideSign = i % 2 == 1 ? -1 : 1;
                row.Depth = i; row.TargetDepth = i;
                NPCs.Add(row);
                CreateRow(startY, row);
                startY -= MovementData.yOffsetNPC;
            }

            InvokeRepeating(nameof(MovementRowsX), Init.GlobalVariables.GameConfig.SwapNPCTick,
                GlobalVariables.GameConfig.SwapNPCTick + GlobalMoveXTime);
            InvokeRepeating(nameof(CheckAddDepth), 0, MovementData.npcMoveTime * 1.5f);
        }

        private void CheckAddDepth()
        {
            if (NPCs[0].Depth > 0)
                AppendWave();
            if (NPCs.Count < Init.GlobalVariables.GameConfig.NpcRows)
                NPCs.ForEach(w => { w.TargetDepth++; });
        }

        private void CreateRow(int startY, Wave row)
        {
            int curX = StartX;

            for (int j = 0; j < GlobalVariables.GameConfig.NpcCols; j++)
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

        private void AppendWave()
        {
            PooledGameData.Player.CurrentScore.CurrentWave++;
            Wave row = new Wave(this);
            if (NPCs.Count > 0)
                row.RowsSideSign = -NPCs[0].RowsSideSign;
            NPCs.Insert(0, row);
            CreateRow(StartY, row);
        }

        public void NPCDie(GameEntities.NPCs.NPC npc, Wave wave)
        {
            wave.npcs.Remove(npc.gameObject);
            if (wave.npcs.Count == 0)
                NPCs.Remove(wave);
        }

        private void Update()
        {
            for (int i = 0; i < NPCs.Count; i++)
                NPCs[i].Update(Time.deltaTime);

            if (Difficulty > 10)
            {
                TimerDifficulty += Time.deltaTime;
                if (TimerDifficulty > GlobalVariables.GameConfig.ChangeDifficultyTime)
                {
                    TimerDifficulty = 0;
                    FancyDifficulty++;
                    if (Difficulty > 20)
                        Difficulty -= 20;
                    else
                        Difficulty -= 2;
                    _ui.RefreshDifficulty();
                }
            }

        }

        private void MovementRowsX()
        {
            if (_moveData.CountPassXMove > 0 && _moveData.CountPassXMove % 4 == 0)
            {
                _moveData.CountPassXMove++;
                NPCs.ForEach(w => w.TargetDepth++);
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


                StartCoroutine(MovementRowX(NPCs[i], i, sign));
            }
        }

        private IEnumerator MovementRowX(Wave wave, int listIndex, int sign)
        {
            yield return new WaitForSeconds(listIndex * MovementData.MoveXRowWait);
            List<GameObject> curRow = new List<GameObject>(wave.npcs);
            if (sign < 0)
                curRow.Reverse();
            for (int i = 0; i < curRow.Count; i++)
            {
                GameObject npc = curRow[i];
                StartCoroutine(MovementNpcX(wave, npc, npc.transform.position
                    + Vector3.right * sign * MovementData.xOffsetNPC
                    , i, sign));
            }
        }

        private IEnumerator MovementNpcX(Wave wave, GameObject npc, Vector3 dstPos, int listIndex, float sign)
        {
            yield return new WaitForSeconds(MovementData.npcMoveTime * listIndex * MovementData.waitBeforeMoveNextNpc);

            Vector3 startPos = npc.transform.position;
            GameEntities.NPCs.NPC npcC = npc.GetComponent<GameEntities.NPCs.NPC>();
            npcC.StartMove();

            float startMove = 0.5f; // percent value
            float endMove = 0.4f; // percent value
            float moveDuration = (1f - startMove) * MovementData.npcMoveTime + endMove * MovementData.npcMoveTime;

            yield return StartCoroutine(OnWayMovementNpcX(npc, startPos, dstPos, sign, startMove, 1, 0, moveDuration, false));
            yield return StartCoroutine(OnWayMovementNpcX(npc, startPos, dstPos, sign, -1, endMove,
                (1f - startMove) * MovementData.npcMoveTime, moveDuration, true));


            npcC.EndMove();
        }

        private IEnumerator OnWayMovementNpcX(GameObject npc,
            Vector3 startPos, Vector3 dstPos,
            float sign, float startMove,
            float endMove, float moveTime,
            float moveDuration, bool oppositeProgress)
        {
            float curTime = 0;
            GameEntities.NPCs.NPC npcC = npc.GetComponent<GameEntities.NPCs.NPC>();
            
            while (curTime < MovementData.npcMoveTime)
            {
                curTime += Time.deltaTime;
                float oneWayIntensity = curTime / (MovementData.npcMoveTime * 2);
                float signedIntensity = curTime / MovementData.npcMoveTime;
                if (oppositeProgress)
                    signedIntensity = 1f - signedIntensity;
                if (oppositeProgress)
                    oneWayIntensity += 0.5f;
                npcC.Rotate(signedIntensity * sign, oneWayIntensity);
                if (curTime >= startMove * MovementData.npcMoveTime && curTime <= endMove * MovementData.npcMoveTime)
                {
                    moveTime += Time.deltaTime;
                    Vector3 tgtPos = dstPos;
                    tgtPos.y = npc.transform.position.y;
                    tgtPos.x = Mathf.Lerp(startPos.x, dstPos.x, moveTime / moveDuration);
                    npc.transform.position = tgtPos;
                }
                yield return new WaitForEndOfFrame();
            }
            Vector3 tgtPos2 = dstPos;
            tgtPos2.y = npc.transform.position.y;
            npc.transform.position = tgtPos2;
        }

        public void TakeTeleport(GameObject src, GameObject teleport)
        {
            src.transform.position = teleport.transform == _teleportLeft
                ? _teleportRight.transform.GetChild(0).position :_teleportLeft.transform.GetChild(0).position;
        }

        public void Finish()
        {
            CancelInvoke();
            int waveCount = NPCs.Count;
            for (int i = 0; i < waveCount; i++)
            {
                Wave w = NPCs[0];
                int count = w.npcs.Count;
                for (int j = 0; j < count; j++)
                {
                    GameObject go = w.npcs[0]; // OnDie will remove it from container
                    go.GetComponent<rqgames.GameEntities.NPCs.NPC>().OnDie(false);
                }
            }
            PooledGameData.Finish();
        }
    }
}