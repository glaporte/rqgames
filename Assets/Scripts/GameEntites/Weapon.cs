using System.Collections.Generic;
using UnityEngine;

namespace rqgames.GameEntities
{
    public class Weapon : MonoBehaviour, IPooledGameEntities
    {
        public const string WeaponTag = "Weapon";

        public Stack<GameObject> Container { get; set; }

        private void Start()
        {
        }

        public void OnDie()
        {
            gameObject.SetActive(false);
            Container.Push(this.gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == NPCs.NPC.NPCTag || other.tag == Playable.Player.PlayerTag)
            {
                OnDie();
            }
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