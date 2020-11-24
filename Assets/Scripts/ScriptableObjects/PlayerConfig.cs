﻿using UnityEngine;

namespace rqgames.MainMenuScene
{
    [CreateAssetMenu]
    public class PlayerConfig : ScriptableObject
    {
        public int LifeCount = 3;
        public int LateralSpeed = 2;
        public float InvulnerabilityTime = 2;
    }
}