using UnityEngine;

namespace rqgames.gameconfig
{
    [CreateAssetMenu]
    public class NPCConfig : ScriptableObject
    {
        [Range(1, 3f)]
        public float NPC1WeaponSpeed = 1;
        [Range(1, 3f)]
        public float NPC2WeaponSpeed = 2;
        [Range(1, 3f)]
        public float NPC3WeaponSpeed = 3;

        [Range(1, 10)]
        public int NPC1AttackCount = 4;
        [Range(1, 10)]
        public int NPC2AttackCount = 2;
        [Range(1, 10)]
        public int NPC3AttackCount = 8;
    }
}
