using UnityEngine;

namespace rqgames.GameEntities.NPCs
{
    public class NPC3 : NPC
    {
        private readonly Vector2 MinMaxZAngle = new Vector2(10, 30);
        private int _sign = 1;
        protected override void InitNPC()
        {
            base.InitNPC();
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
                    0,
                    UnityEngine.Random.Range(MinMaxZAngle.x, MinMaxZAngle.y));
        }

        protected void Update()
        {
            /*transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
                0,
                (transform.rotation.eulerAngles.z + Time.deltaTime * 10 * _sign));
            if (transform.rotation.eulerAngles.z > MinMaxZAngle.y && _sign > 0)
                _sign *= -1;
            else if (transform.rotation.eulerAngles.z < MinMaxZAngle.x && _sign < 0)
                _sign *= -1;
            */
        }


        override public void Rotate(float intensity)
        {
            if (intensity == 0)
                transform.rotation = InitialRotation;
            else
                transform.localRotation = InitialRotation * Quaternion.Euler(0, 0, -intensity * 30);
        }
    }
}