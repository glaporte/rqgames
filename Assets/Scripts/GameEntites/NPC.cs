using System.Collections.Generic;
using DUCK.FSM;
using UnityEngine;
using UnityEngine.Events;

namespace rqgames.GameEntities.NPCs
{
    public class NPC : MonoBehaviour, IPooledGameEntities
    {
        public const string NPCTag = "NPC";

        [SerializeField]
        protected rqgames.gameconfig.NPCConfig _config;

        protected FiniteStateMachine<Playable.FSMCommon.State> _fsm;

        protected Quaternion InitialRotation { get; set; }
        protected Quaternion RotationOnLostIdle { get; set; }

        protected float _internalTimer = 0;
        protected float _rndMedium;

        protected float _weaponSpeed;
        protected float _countAttack;

        private float _timerAttack;

        public Stack<GameObject> DataContainer { get; set; }
        private rqgames.Game.Game _game;
        private Game.Game.Wave _gameContainer;
        private int _startY;

        private void Start()
        {
            _timerAttack = 0;
            int rndSign = UnityEngine.Random.Range(0, 100) % 2 == 0 ? 1 : -1;
            _rndMedium = UnityEngine.Random.Range(10f, 30f) * rndSign;
            InitialRotation = transform.rotation;
            _fsm = FiniteStateMachine<Playable.FSMCommon.State>.FromEnum();
            _fsm.AddTransition(Playable.FSMCommon.State.Idle, Playable.FSMCommon.State.Attack, Playable.FSMCommon.ATTACK_COMMAND);

            _fsm.AddTransition(Playable.FSMCommon.State.Attack, Playable.FSMCommon.State.Idle, Playable.FSMCommon.IDLE_COMMAND);
            _fsm.AddTransition(Playable.FSMCommon.State.Move, Playable.FSMCommon.State.Idle, Playable.FSMCommon.IDLE_COMMAND);

            _fsm.AddTransition(Playable.FSMCommon.State.Idle, Playable.FSMCommon.State.Move, Playable.FSMCommon.MOVE_COMMAND);
            _fsm.AddTransition(Playable.FSMCommon.State.Attack, Playable.FSMCommon.State.Move, Playable.FSMCommon.MOVE_COMMAND);

            _fsm.OnEnter(Playable.FSMCommon.State.Attack, () => { Invoke(nameof(BackToIdle), 0.3f); Fire(); });
            _fsm.OnExit(Playable.FSMCommon.State.Idle, () => { RotationOnLostIdle = transform.rotation; });
            _fsm.Begin(Playable.FSMCommon.State.Idle);

            InitNPC();
        }

        virtual protected void InitNPC() { }
        virtual protected void UpdateNPC() { }

        public void Reset(rqgames.Game.Game game, Game.Game.Wave row, int startY)
        {
            _internalTimer = 0;
            _game = game;
            _startY = startY;
            _gameContainer = row;
            _timerAttack = 0;

            if (_fsm != null)
                _fsm.IssueCommand(Playable.FSMCommon.IDLE_COMMAND);
        }

        private void Update()
        {
            UpdateNPC();
            _timerAttack += Time.deltaTime;
            if (_timerAttack > 1)
            {
                if (Random.Range(0, 1f) < _countAttack / _game.Difficulty)
                    _fsm.IssueCommand(Playable.FSMCommon.ATTACK_COMMAND);
                _timerAttack = 0;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == Init.GlobalVariables.AllyLayer && other.tag == Weapon.WeaponTag)
            {
                OnDie(true);
                Init.PooledGameData.Player.OnKillEnemy();
            }
        }

        virtual public void Rotate(float intensity, float oneWayIntensity)
        {
            transform.rotation = RotationOnLostIdle * Quaternion.Euler(0, 0, intensity * 30);
        }

        public void StartMove()
        {
            _fsm.IssueCommand(Playable.FSMCommon.MOVE_COMMAND);
        }

        public void EndMove()
        {
            _fsm.IssueCommand(Playable.FSMCommon.IDLE_COMMAND);
        }

        virtual protected void Fire()
        {
            Vector3 vel = Init.PooledGameData.Player.transform.position - transform.position;
            vel = vel.normalized * _weaponSpeed;
            Vector3 pos = transform.position + transform.rotation * Vector3.back * transform.localScale.z;
            Init.PooledGameData.PopWeapon(pos,
                vel,
                Init.GlobalVariables.EnemyLayer);
        }

        private void BackToIdle()
        {
            _fsm.IssueCommand(Playable.FSMCommon.IDLE_COMMAND);
        }

        public void OnDie(bool byAlly)
        {
            transform.rotation = InitialRotation;
            _game.NPCDie(this, _gameContainer);
            gameObject.SetActive(false);
            DataContainer.Push(this.gameObject);
        }
    }

}