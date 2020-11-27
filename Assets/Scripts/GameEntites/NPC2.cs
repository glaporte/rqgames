using UnityEngine;

namespace rqgames.GameEntities.NPCs
{
    public class NPC2 : NPC
    {
        [SerializeField]
        private Transform _leftTentacle;
        [SerializeField]
        private Transform _rightTentacle;

        protected override void InitNPC()
        {
            _weaponSpeed = _config.NPC2WeaponSpeed;
            _countAttack = _config.NPC2AttackCount;
            base.InitNPC();
        }

        override protected void UpdateNPC()
        {
            if (_fsm.CurrentState == Playable.FSMCommon.State.Idle)
            {
                transform.rotation = InitialRotation *
                    Quaternion.Euler(Mathf.Cos(_internalTimer) * _rndMedium,
                    0,
                    Mathf.Cos(_internalTimer) * _rndMedium * 2);
                _internalTimer += Time.deltaTime;
            }
        }

        protected override void Fire()
        {
            Vector3 vel = transform.rotation * Vector3.back;
            vel = vel.normalized * _weaponSpeed;

            Vector3 right = _rightTentacle.transform.position;
            Init.PooledGameData.PopWeapon(right, vel, Init.GlobalVariables.EnemyLayer);

            Vector3 left = _leftTentacle.transform.position;
            Init.PooledGameData.PopWeapon(left, vel, Init.GlobalVariables.EnemyLayer);
        }
    }
}