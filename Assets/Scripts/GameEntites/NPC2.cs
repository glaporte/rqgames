using UnityEngine;

namespace rqgames.GameEntities.NPCs
{
    public class NPC2 : NPC
    {
        protected override void InitNPC()
        {
            base.InitNPC();
        }

        protected void Update()
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
    }
}