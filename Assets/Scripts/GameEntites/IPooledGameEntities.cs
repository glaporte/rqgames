using UnityEngine;
using System.Collections.Generic;


namespace rqgames.GameEntities
{
    public interface IPooledGameEntities
    {
        Stack<GameObject> Container { get; set; }

        void OnDie();
    }
}