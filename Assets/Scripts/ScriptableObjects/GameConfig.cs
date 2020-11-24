using UnityEngine;

namespace rqgames.gameconfig
{
    [CreateAssetMenu]
    public class GameConfig : ScriptableObject
    {
        public int LOAD_AND_MAX_NPC1 = 10;
        public int LOAD_AND_MAX_NPC2 = 10;
        public int LOAD_AND_MAX_NPC3 = 10;
        public int LOAD_AND_MAX_WEAPON = 50;
    }
}