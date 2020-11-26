using DUCK.FSM;
using UnityEngine;
using System.Collections.Generic;

namespace rqgames.GameEntities.Playable
{
    [RequireComponent(typeof(InputManagement.SwipeManager))]
    public class Player : MonoBehaviour
    {
        private rqgames.InputManagement.SwipeManager _input;
        private rqgames.Game.Game _game;
        private Rigidbody _body;

        private const string IDLE_COMMAND = "idle";
        private const string ATTACK_COMMAND = "attack";
        private const string MOVE_COMMAND = "move";

        public enum PlayerStates
        {
            Idle,
            Attack,
            Move,
        }


        protected FiniteStateMachine<PlayerStates> _fsm;
        private float _turretAngle;

        [SerializeField]
        private rqgames.gameconfig.PlayerConfig _config;
        private Quaternion _initRotation;

        private int _lateralSign = 1;

        private Dictionary<PlayerStates, float> PlayerStateEnterTime;

        private void Start()
        {
            _initRotation = this.transform.rotation;

            _input = GetComponent<rqgames.InputManagement.SwipeManager>();
            _input.OnSwipeLeft.AddListener(MoveLeft);
            _input.OnSwipeRight.AddListener(MoveRight);
            _input.OnSwipeRelease.AddListener((Vector2 screenPos) => _fsm.IssueCommand(ATTACK_COMMAND));

            _body = GetComponent<Rigidbody>();



            _fsm = FiniteStateMachine<PlayerStates>.FromEnum();


            _fsm.AddTransition(PlayerStates.Idle, PlayerStates.Attack, ATTACK_COMMAND);
            _fsm.AddTransition(PlayerStates.Move, PlayerStates.Attack, ATTACK_COMMAND);
            _fsm.AddTransition(PlayerStates.Attack, PlayerStates.Idle, IDLE_COMMAND);
            _fsm.AddTransition(PlayerStates.Move, PlayerStates.Idle, IDLE_COMMAND);
            _fsm.AddTransition(PlayerStates.Idle, PlayerStates.Move, MOVE_COMMAND);
            _fsm.AddTransition(PlayerStates.Move, PlayerStates.Move, MOVE_COMMAND);

            _fsm.OnEnter(PlayerStates.Attack, () => { Fire(); });
            _fsm.OnEnter(PlayerStates.Idle, () => { _body.velocity = Vector3.zero; });
            _fsm.OnEnter(PlayerStates.Move, () =>
            {
                _body.velocity = Vector3.right * _lateralSign;
            });

            PlayerStateEnterTime = new Dictionary<PlayerStates, float>();
            PlayerStateEnterTime[PlayerStates.Attack] = 0;
            PlayerStateEnterTime[PlayerStates.Move] = 0;

            _fsm.OnChange((from, to) => { PlayerStateEnterTime[to] = Time.time; });

            _fsm.Begin(PlayerStates.Idle);
        }

        public void StartGame(rqgames.Game.Game game)
        {
            _game = game;
            gameObject.SetActive(true);
            transform.position = new Vector3(0, -_game.TopY + 4, 0);
        }


        private void Update()
        {
            TurretFollowMouseDirection();
            IdleCheck();
        }

        private void TurretFollowMouseDirection()
        {
            Vector3 turretPos = Camera.main.WorldToScreenPoint(this.transform.position);
            _turretAngle = Vector2.Angle(Vector2.left, Input.mousePosition - turretPos);
            _turretAngle = Mathf.Clamp(_turretAngle, 0, 180);
            transform.rotation = _initRotation * Quaternion.Euler(0, _turretAngle - 90, 0);
        }

        private void MoveLeft()
        {
            _lateralSign = -1;
            _fsm.IssueCommand(MOVE_COMMAND);
        }

        private void MoveRight()
        {
            _lateralSign = 1;
            _fsm.IssueCommand(MOVE_COMMAND);
        }

        private void IdleCheck()
        {
            bool check = false;

            if (_fsm.CurrentState == PlayerStates.Move && Time.time - PlayerStateEnterTime[PlayerStates.Move] > _config.MoveTime)
                check = true;
            if ((_fsm.CurrentState == PlayerStates.Attack && Time.time - PlayerStateEnterTime[PlayerStates.Attack] > _config.AttackTime))
                check = true;

            if (check)
            {
                _fsm.IssueCommand(IDLE_COMMAND);
            }

        }

        private void Fire()
        {
            float radians = _turretAngle * Mathf.Deg2Rad;
            Vector3 vel = new Vector3(-Mathf.Cos(radians), Mathf.Sin(radians), 0);
            vel = vel.normalized * _config.ProjectileSpeed;
            Init.PooledGameData.PopWeapon(transform.position,
                vel,
                Init.GlobalVariables.AllyLayer);
        }
    }
}