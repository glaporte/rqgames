using UnityEngine;

namespace rqgames.GameEntities.NPCs
{
    public class NPC3 : NPC
    {
        [SerializeField]
        private Transform _target;
        private readonly Vector2 MinMaxZAngle = new Vector2(10, 30);

        protected override void InitNPC()
        {
            _weaponSpeed = _config.NPC3WeaponSpeed;
            _countAttack = _config.NPC3AttackCount;

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
                    0,
                    Random.Range(MinMaxZAngle.x, MinMaxZAngle.y));
        }

        override protected void UpdateNPC()
        {
            if (_fsm.CurrentState == Playable.FSMCommon.State.Idle)
            {
                transform.rotation = InitialRotation * Quaternion.Euler(
                    0,
                    0,
                    Mathf.Cos(_internalTimer) * _rndMedium);
                _internalTimer += Time.deltaTime;
            }
        }

        override protected void Fire()
        {
            Vector3 vel = transform.rotation * Vector3.forward;
            vel = vel.normalized * _weaponSpeed;
            Init.PooledGameData.PopWeapon(_target.position, vel, Init.GlobalVariables.EnemyLayer);
        }

        override public void Rotate(float intensity)
        {
            transform.localRotation = InitialRotation * Quaternion.Euler(0, 0, -intensity * 30);
        }
    }
}