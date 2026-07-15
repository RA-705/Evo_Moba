using System.Collections.Generic;
using UnityEngine;

namespace Evo.Battle
{
    [System.Serializable]
    public class HeroStats
    {
        public int ID;
        public string Name;
        public float BaseHP;
        public float BaseMP;
        public float AttackDamage;
        public float AbilityPower;
        public float Armor;
        public float MagicResist;
        public float MoveSpeed;
        public float AttackRange;
        public float AttackSpeed;
        public float HpPerLevel;
        public float MpPerLevel;
        public float AttackPerLevel;
        public float ArmorPerLevel;

        public HeroStats(int id, string name, float hp, float mp, float ad, float ap, float armor, float mr,
            float speed, float range, float aspd, float hpLvl = 0, float mpLvl = 0, float adLvl = 0, float armorLvl = 0)
        {
            ID = id; Name = name; BaseHP = hp; BaseMP = mp;
            AttackDamage = ad; AbilityPower = ap; Armor = armor; MagicResist = mr;
            MoveSpeed = speed; AttackRange = range; AttackSpeed = aspd;
            HpPerLevel = hpLvl; MpPerLevel = mpLvl; AttackPerLevel = adLvl; ArmorPerLevel = armorLvl;
        }
    }

    public static class HeroStatsDatabase
    {
        private static Dictionary<int, HeroStats> _stats;
        private static bool _initialized;

        private static void Initialize()
        {
            if (_initialized) return;
            _stats = new Dictionary<int, HeroStats>();

            Register(new HeroStats(1, "Keth", 920, 380, 60, 0, 32, 28, 5.6f, 1.8f, 1.0f, 88, 28, 3.8f, 3.2f));
            Register(new HeroStats(4, "Mikal", 800, 450, 52, 0, 22, 30, 5.4f, 2.5f, 1.1f, 75, 32, 3.2f, 2.2f));
            Register(new HeroStats(8, "Tigria", 1150, 320, 55, 0, 42, 28, 4.8f, 1.5f, 0.75f, 98, 22, 3f, 3.8f));
            Register(new HeroStats(11, "Eida", 780, 520, 48, 0, 24, 35, 5.0f, 2.2f, 0.85f, 72, 38, 2.6f, 2f));
            Register(new HeroStats(17, "Reisa", 820, 500, 50, 0, 26, 34, 5.1f, 2.3f, 0.9f, 76, 36, 2.8f, 2.2f));
            Register(new HeroStats(24, "Kyouya", 860, 400, 62, 0, 28, 26, 5.8f, 1.6f, 1.05f, 82, 26, 4f, 2.8f));
            Register(new HeroStats(26, "Norah", 750, 550, 44, 0, 20, 36, 5.2f, 2.4f, 0.95f, 68, 40, 2.4f, 1.8f));
            Register(new HeroStats(27, "Salome", 800, 480, 50, 0, 25, 32, 5.3f, 2.2f, 0.9f, 74, 34, 2.8f, 2.2f));
            Register(new HeroStats(28, "Wulfric", 1050, 360, 58, 0, 38, 26, 4.7f, 1.8f, 0.8f, 92, 24, 3.5f, 3.5f));
            Register(new HeroStats(30, "Adele", 780, 530, 46, 0, 22, 36, 5.0f, 2.4f, 0.85f, 70, 38, 2.5f, 2f));
            Register(new HeroStats(36, "Lars", 1200, 300, 54, 0, 45, 24, 4.5f, 1.4f, 0.7f, 105, 18, 3.2f, 4.2f));

            _initialized = true;
        }

        private static void Register(HeroStats stats)
        {
            _stats[stats.ID] = stats;
        }

        public static HeroStats GetStats(int heroId)
        {
            Initialize();
            _stats.TryGetValue(heroId, out var stats);
            return stats;
        }

        public static int GetHeroCount()
        {
            Initialize();
            return _stats.Count;
        }
    }
}
