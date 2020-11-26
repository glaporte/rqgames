using UnityEngine;

namespace rqgames.gameconfig
{
    [CreateAssetMenu]
    public class PlayerConfig : ScriptableObject
    {
        public int LifeCount = 3;
        [Range(1, 5)]
        public float LateralSpeed = 2;
        public float InvulnerabilityTime = 2;
    }
}