using UnityEngine;

namespace rqgames.GameEntities.NPCs
{
    public class NPC1 : NPCs.NPC
    {
        private float _rnd1;
        private float _rnd2;
        protected override void InitNPC()
        {
            _rnd1 = UnityEngine.Random.Range(-15, 15);
            _rnd2 = UnityEngine.Random.Range(0.5f, 1f);
            base.InitNPC();
        }

        protected void Update()
        {
            if (_fsm.CurrentState == Playable.FSMCommon.State.Idle)
            {
                transform.rotation = InitialRotation *
                    (Quaternion.Euler(transform.rotation.x + Mathf.Cos(Time.time * _rnd2) * _rnd1,
                    transform.rotation.x + Mathf.Cos(Time.time * _rnd2) * _rnd1,
                    transform.rotation.eulerAngles.z + Time.deltaTime * _rnd1));
            }
        }

        override public void Rotate(float intensity)
        {
            if (intensity == 0)
                transform.rotation = InitialRotation;
            else
                transform.localRotation = InitialRotation * Quaternion.Euler(0, intensity * 30, 0);
        }
    }
}