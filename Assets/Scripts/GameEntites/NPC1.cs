﻿using UnityEngine;

namespace rqgames.GameEntities
{
    public class NPC1 : NPC
    {
        override protected void InitNPC()
        {
            transform.rotation = Quaternion.Euler(Random.Range(5, 15), 0, 0);
        }

        protected void Update()
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
                0,
                transform.rotation.eulerAngles.z + Time.deltaTime * 10);
        }
    }
}