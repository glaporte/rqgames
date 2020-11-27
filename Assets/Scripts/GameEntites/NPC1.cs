using UnityEngine;

namespace rqgames.GameEntities.NPCs
{
    public class NPC1 : NPCs.NPC
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
                    Mathf.Cos(_internalTimer) * _rndMedium,
                    Mathf.Cos(_internalTimer) * _rndMedium);
                _internalTimer += Time.deltaTime;
            }
        }

        override public void Rotate(float intensity)
        {
            transform.localRotation = RotationOnStartMove * Quaternion.Euler(0, intensity * 30, 0);
        }
    }
}