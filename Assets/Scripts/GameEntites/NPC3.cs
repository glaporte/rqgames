using UnityEngine;

namespace rqgames.GameEntities.NPCs
{
    public class NPC3 : NPC
    {
        private readonly Vector2 MinMaxZAngle = new Vector2(10, 30);

        protected override void InitNPC()
        {
            base.InitNPC();
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
                    0,
                    UnityEngine.Random.Range(MinMaxZAngle.x, MinMaxZAngle.y));
        }

        protected void Update()
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

        override public void Rotate(float intensity)
        {
            transform.localRotation = InitialRotation * Quaternion.Euler(0, 0, -intensity * 30);
        }
    }
}