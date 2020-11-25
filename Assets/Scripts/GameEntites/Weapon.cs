using System.Collections.Generic;
using UnityEngine;

namespace rqgames.GameEntities
{
    public class Weapon : MonoBehaviour, IPooledGameEntities
    {
        public Stack<GameObject> Container { get; set; }

        public void OnDie()
        {
            Container.Push(this.gameObject);
        }

    }
}