using System.Collections.Generic;
using UnityEngine;

namespace rqgames.GameEntities
{
    public class Weapon : MonoBehaviour, IPooledGameEntities
    {
        public Stack<GameObject> Container { get; set; }

        private void Start()
        {
        }

        public void OnDie()
        {
            Init.PooledGameData.ReleaseWeapon(this.gameObject);
        }

        public void Proc(Vector3 position, Vector3 velocity, int layer)
        {
            gameObject.layer = layer;
            gameObject.transform.position = position;
            gameObject.GetComponent<Rigidbody>().velocity = velocity;
            Vector3 inverseVel = velocity;
            inverseVel *= -1;
            transform.rotation = Quaternion.LookRotation(inverseVel);
        }
    }
}