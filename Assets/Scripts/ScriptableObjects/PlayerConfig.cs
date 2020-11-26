using UnityEngine;

namespace rqgames.gameconfig
{
    [CreateAssetMenu]
    public class PlayerConfig : ScriptableObject
    {
        public int LifeCount = 3;
        [Range(4, 8)]
        public float LateralSpeed = 6;
        public float InvulnerabilityTime = 2;

        [Range(0.1f, 0.3f)]
        public float MoveTime = 0.05f;

        [Range(0.12f, 0.4f)]
        public float AttackTime = 0.1f;

        [Range(3, 4f)]
        public float ProjectileSpeed = 3f;
    }
}