using System.Collections.Generic;
using UnityEngine;

namespace rqgames.GameEntities
{
    public class Weapon : MonoBehaviour, IPooledGameEntities
    {
        public const string WeaponTag = "Weapon";

        public Stack<GameObject> DataContainer { get; set; }

        public void OnDie(bool byAlly)
        {
            CancelInvoke();
            gameObject.SetActive(false);
            rqgames.Init.PooledGameData.UsedWeapons.Remove(this.gameObject);
            DataContainer.Push(this.gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == NPCs.NPC.NPCTag || other.tag == Playable.Player.PlayerTag)
            {
                OnDie(false);
            }
        }

        private void Unused()
        {
            OnDie(false);
        }

        public void Proc(Vector3 position, Vector3 velocity, int layer)
        {
            gameObject.layer = layer;
            position.z = 0;
            gameObject.transform.position = position;
            gameObject.GetComponent<Rigidbody>().velocity = velocity;
            Vector3 inverseVel = velocity;
            inverseVel *= -1;
            transform.rotation = Quaternion.LookRotation(inverseVel);
            
            Invoke(nameof(Unused), 20);
        }
    }
}