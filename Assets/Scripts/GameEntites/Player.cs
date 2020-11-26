using UnityEngine;

namespace rqgames.GameEntities
{
    [RequireComponent(typeof(InputManagement.SwipeManager))]
    public class Player : MonoBehaviour
    {
        private rqgames.InputManagement.SwipeManager _input;
        private rqgames.Game.Game _game;

        [SerializeField]
        private Transform _target;

        private void Start()
        {
            _input = GetComponent<rqgames.InputManagement.SwipeManager>();
            _input.OnSwipeLeft.AddListener(MoveLeft);
            _input.OnSwipeRight.AddListener(MoveRight);
        }

        private void MoveLeft()
        {
            Debug.Log("moveleft");
        }

        private void MoveRight()
        {
            Debug.Log("moveright");
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