using UnityEngine;

namespace rqgames.GameEntities.NPCs
{
    public class NPC1 : NPCs.NPC
    {
        protected override void InitNPC()
        {
            _weaponSpeed = _config.NPC1WeaponSpeed;
            _countAttack = _config.NPC1AttackCount;
            base.InitNPC();
        }

        override protected void UpdateNPC()
        {
            if (_fsm.CurrentState == Playable.FSMCommon.State.Idle)
            {
                transform.rotation = InitialRotation *
                    Quaternion.Euler(Mathf.Cos(_internalTimer) * _rndMedium,
                    Mathf.Cos(_internalTimer) * _rndMedium,
                    Mathf.Cos(_internalTimer) * _rndMedium);
                _internalTimer += Time.deltaTime;
            }
        }

        override public void Rotate(float intensity)
        {
            transform.localRotation = RotationOnLostIdle * Quaternion.Euler(0, intensity * 30, 0);
        }
    }
}