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
        protected float _rndSmall;
        protected float _rndMedium;

        protected float _weaponSpeed;
        protected float _countAttack;

        private float _timerAttack;

        private int _difficulty = 150; // if _countAttack is max (10), each second attack has _countAttack / _difficulty chance to proc.

        public Stack<GameObject> DataContainer { get; set; }
        private rqgames.Game.Game _game;
        private Game.Game.Wave _gameContainer;
        private int _startY;

        private void Start()
        {
            _timerAttack = 0;
            _rndMedium = UnityEngine.Random.Range(-15, 15);
            _rndSmall = UnityEngine.Random.Range(-1f, 1f);
            InitialRotation = transform.rotation;
            _fsm = FiniteStateMachine<Playable.FSMCommon.State>.FromEnum();
            _fsm.AddTransition(Playable.FSMCommon.State.Idle, Playable.FSMCommon.State.Attack, Playable.FSMCommon.ATTACK_COMMAND);

            _fsm.AddTransition(Playable.FSMCommon.State.Attack, Playable.FSMCommon.State.Idle, Playable.FSMCommon.IDLE_COMMAND);
            _fsm.AddTransition(Playable.FSMCommon.State.Move, Playable.FSMCommon.State.Idle, Playable.FSMCommon.IDLE_COMMAND);

            _fsm.AddTransition(Playable.FSMCommon.State.Idle, Playable.FSMCommon.State.Move, Playable.FSMCommon.MOVE_COMMAND);
            _fsm.AddTransition(Playable.FSMCommon.State.Attack, Playable.FSMCommon.State.Move, Playable.FSMCommon.MOVE_COMMAND);

            _fsm.OnEnter(Playable.FSMCommon.State.Attack, Fire);
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
                if (UnityEngine.Random.Range(0, 1f) < _countAttack / _difficulty)
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

        virtual public void Rotate(float intensity)
        {
            transform.localRotation = RotationOnLostIdle * Quaternion.Euler(0, 0, intensity * 30);
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

            _fsm.IssueCommand(Playable.FSMCommon.IDLE_COMMAND);
        }

        public void OnDie(bool byAlly)
        {
            transform.rotation = InitialRotation;
            if (byAlly)
                _gameContainer.Killed++;
            _game.NPCDie(this, _gameContainer);
            gameObject.SetActive(false);
            DataContainer.Push(this.gameObject);
        }
    }

}