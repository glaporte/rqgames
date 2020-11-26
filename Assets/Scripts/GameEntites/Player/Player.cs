using DUCK.FSM;
using UnityEngine;

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


        [SerializeField]
        private rqgames.gameconfig.PlayerConfig _config;

        private void Start()
        {
            _input = GetComponent<rqgames.InputManagement.SwipeManager>();
            _input.OnSwipeLeft.AddListener(MoveLeft);
            _input.OnSwipeRight.AddListener(MoveRight);
            _input.OnSwipeRelease.AddListener(Fire);

            _body = GetComponent<Rigidbody>();


            _fsm = FiniteStateMachine<PlayerStates>.FromEnum();

            AttackTransition _atk = new AttackTransition(PlayerStates.Idle, PlayerStates.Attack);


            _fsm.AddTransition(PlayerStates.Idle, PlayerStates.Attack, ATTACK_COMMAND, _atk);
            _fsm.AddTransition(PlayerStates.Attack, PlayerStates.Idle, IDLE_COMMAND);
            _fsm.AddTransition(PlayerStates.Idle, PlayerStates.Move, MOVE_COMMAND);

            _fsm.OnEnter(PlayerStates.Attack, () => { });

            _fsm.Begin(PlayerStates.Idle);
        }

        private void Fire(Vector2 screenPos)
        {
            if (screenPos.y > 50)
            {
                _fsm.IssueCommand(ATTACK_COMMAND);
            }
        }

        private void MoveLeft()
        {
            Debug.Log("moveleft");
            _body.velocity = Vector3.left * _config.LateralSpeed;
        }

        private void MoveRight()
        {
            Debug.Log("moveright");
            _body.velocity = Vector3.right * _config.LateralSpeed;
        }

        public void StartGame(rqgames.Game.Game game)
        {
            _game = game;
            gameObject.SetActive(true);
            transform.position = new Vector3(0, - _game.TopY + 4, 0);
        }

        private void Update()
        {
            TurretFollowMouseDirection();
        }

        public void Shoot()
        {

        }

        private void TurretFollowMouseDirection()
        {
            Vector3 turretPos = Camera.main.WorldToScreenPoint(this.transform.position);
            float angle = Vector2.Angle(Vector2.left, Input.mousePosition - turretPos);
            angle = Mathf.Clamp(angle, 0, 180);

            angle -= 90; // initialy look at forward
            transform.localRotation = Quaternion.Euler(0, angle, 0);
        }
    }
}