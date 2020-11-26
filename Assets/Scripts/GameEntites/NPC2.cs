using UnityEngine;

namespace rqgames.GameEntities.NPCs
{
    public class NPC2 : NPC
    {
        private int _sign = 1;
        protected override void InitNPC()
        {
            base.InitNPC();

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
                UnityEngine.Random.Range(0, 360)
                , 0);
            _sign = Random.Range(0, 100) % 2 == 0 ? 1 : -1;
        }

        protected void Update()
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
                transform.rotation.eulerAngles.y + Time.deltaTime * 10 * _sign,
                0);
        }
    }
}