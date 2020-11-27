using DUCK.FSM;
using UnityEngine;
using System.Collections.Generic;

namespace rqgames.GameEntities.Playable
{
    public static class FSMCommon
    {
        public const string IDLE_COMMAND = "idle";
        public const string ATTACK_COMMAND = "attack";
        public const string MOVE_COMMAND = "move";

        public enum State
        {
            Idle,
            Attack,
            Move,
        }

    }

    [RequireComponent(typeof(InputManagement.SwipeManager))]
    public class Player : MonoBehaviour
    {
        public const string PlayerTag = "Player";

        private rqgames.InputManagement.SwipeManager _input;
        private rqgames.Game.Game _game;
        private Rigidbody _body;




        protected FiniteStateMachine<FSMCommon.State> _fsm;
        private float _turretAngle;

        [SerializeField]
        private rqgames.gameconfig.PlayerConfig _config;
        private Quaternion _initRotation;

        private int _lateralSign = 1;

        private Dictionary<FSMCommon.State, float> PlayerStateEnterTime;

        private void Start()
        {
            _initRotation = this.transform.rotation;

            _input = GetComponent<rqgames.InputManagement.SwipeManager>();
            _input.OnSwipeLeft.AddListener(MoveLeft);
            _input.OnSwipeRight.AddListener(MoveRight);
            _input.OnStationaryPressDownUp.AddListener((Vector2 screenPos) => _fsm.IssueCommand(FSMCommon.ATTACK_COMMAND));

            _body = GetComponent<Rigidbody>();


            _fsm = FiniteStateMachine<FSMCommon.State>.FromEnum();


            
            AddDelayTransition(FSMCommon.State.Idle, FSMCommon.State.Attack, FSMCommon.ATTACK_COMMAND);
            AddDelayTransition(FSMCommon.State.Move, FSMCommon.State.Attack, FSMCommon.ATTACK_COMMAND);
            AddDelayTransition(FSMCommon.State.Attack, FSMCommon.State.Idle, FSMCommon.IDLE_COMMAND);
            AddDelayTransition(FSMCommon.State.Move, FSMCommon.State.Idle, FSMCommon.IDLE_COMMAND);
            AddDelayTransition(FSMCommon.State.Idle, FSMCommon.State.Move, FSMCommon.MOVE_COMMAND);
            AddDelayTransition(FSMCommon.State.Move, FSMCommon.State.Move, FSMCommon.MOVE_COMMAND);

            _fsm.OnEnter(FSMCommon.State.Attack, () => { Fire(); });
            _fsm.OnEnter(FSMCommon.State.Idle, () => { _body.velocity = Vector3.zero; });
            _fsm.OnEnter(FSMCommon.State.Move, () =>
            {
                _body.velocity = Vector3.right * _lateralSign * _config.LateralSpeed;
            });

            PlayerStateEnterTime = new Dictionary<FSMCommon.State, float>();
            PlayerStateEnterTime[FSMCommon.State.Attack] = 0;
            PlayerStateEnterTime[FSMCommon.State.Move] = 0;

            _fsm.OnChange((from, to) => { PlayerStateEnterTime[to] = Time.time; });

            _fsm.Begin(FSMCommon.State.Idle);
        }

        private void AddDelayTransition(FSMCommon.State from, FSMCommon.State to, string command, int duration = 20)
        {
            DelayTransition delayTransition = new DelayTransition(from, to, duration);
            _fsm.AddTransition(from, to, command);

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
            _fsm.IssueCommand(FSMCommon.MOVE_COMMAND);
        }

        private void MoveRight()
        {
            _lateralSign = 1;
            _fsm.IssueCommand(FSMCommon.MOVE_COMMAND);
        }

        private void IdleCheck()
        {
            bool check = false;

            if (_fsm.CurrentState == FSMCommon.State.Move && Time.time - PlayerStateEnterTime[FSMCommon.State.Move] > _config.MoveTime)
                check = true;
            if ((_fsm.CurrentState == FSMCommon.State.Attack && Time.time - PlayerStateEnterTime[FSMCommon.State.Attack] > _config.AttackTime))
                check = true;

            if (check)
            {
                _fsm.IssueCommand(FSMCommon.IDLE_COMMAND);
            }

        }

        private void Fire()
        {
            _body.velocity = Vector3.zero;
            float radians = _turretAngle * Mathf.Deg2Rad;
            Vector3 vel = new Vector3(-Mathf.Cos(radians), Mathf.Sin(radians), 0);
            vel = vel.normalized * _config.ProjectileSpeed;
            Vector3 pos = transform.position + transform.rotation * Vector3.back * transform.localScale.z;
            Init.PooledGameData.PopWeapon(pos,
                vel,
                Init.GlobalVariables.AllyLayer);
        }
    }
}