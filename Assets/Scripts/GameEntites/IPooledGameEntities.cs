using UnityEngine;
using System.Collections.Generic;


namespace rqgames.GameEntities
{
    public interface IPooledGameEntities
    {
        Stack<GameObject> DataContainer { get; set; }

        void OnDie();
    }
}