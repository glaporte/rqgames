using UnityEngine;

namespace rqgames.gameconfig
{
    [CreateAssetMenu]
    public class GameConfig : ScriptableObject
    {
        [Range(0, 1f)]
        public float Npc1Ratio = 0.33f;
        [Range(0, 1f)]
        public float Npc2Ratio = 0.33f;
        [Range(0, 1f)]
        public float Npc3Ratio = 0.33f;

        [Range(4, 5)]
        public int NpcRows = 4;
        [Range(6, 8)]
        public int NpcCols = 6;

        private int? _countNpc;
        public int CountNpc => _countNpc ?? CountNpcCahe();

        [Range(1, 4)]
        public float SwapNPCTick = 5;

        public const int ExtraRows = 3;

        private int CountNpcCahe()
        {
            _countNpc = (ExtraRows + NpcRows) * NpcCols; // 3 extra rows
            return _countNpc.Value;
        }

        private int? _countNpc1;
        public int CountNpc1 => _countNpc1 ?? CountNpc1Cache();

        private int CountNpc1Cache()
        {
            _countNpc1 = (int)(CountNpc * Npc1Ratio) + 1;
            return _countNpc1.Value;
        }

        private int? _countNpc2;
        public int CountNpc2 => _countNpc2 ?? CountNpc2Cache();

        private int CountNpc2Cache()
        {
            _countNpc2 = (int)(CountNpc * Npc2Ratio) + 1;
            return _countNpc2.Value;
        }

        private int? _countNpc3;
        public int CountNpc3 => _countNpc3 ?? CountNpc3Cache();

        private int CountNpc3Cache()
        {
            _countNpc3 = (int)(CountNpc * Npc3Ratio) + 1;
            return _countNpc3.Value;
        }

        private int? _countMaxWeapon;
        public int CountMaxWeapon => _countMaxWeapon ?? CountMaxWeaponCache();

        private int CountMaxWeaponCache()
        {
            _countMaxWeapon = CountNpc * 2 + 30;
            return _countMaxWeapon.Value;
        }

    }
}