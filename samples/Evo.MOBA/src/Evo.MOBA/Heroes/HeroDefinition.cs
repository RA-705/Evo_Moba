using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Evo.Core.ECS;
using Evo.MOBA.Abilities;
using Evo.MOBA.AI.MATE;
using Evo.MOBA.Buffs;
using Evo.MOBA.Combat;
using Evo.MOBA.Items;
using Evo.MOBA.Match;
using Evo.MOBA.Progression;
using Evo.MOBA.Systems;
using Evo.Shared.Math;

namespace Evo.MOBA.Heroes;

public class HeroDefinition
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string HeroType { get; set; } = ""; // Tank, Warrior, Assassin, Mage, Marksman, Support
    public string Role { get; set; } = ""; // Jungle, Top, Mid, Bot, Support
    
    // Base Stats
    public float Strength { get; set; }
    public float Intelligence { get; set; }
    public float Agility { get; set; }
    public float BaseHp { get; set; } = 150;
    public float BaseMp { get; set; } = 100;
    public float BaseRage { get; set; }
    public float BasePoint { get; set; }
    public float BaseDamage { get; set; } = 12;
    public float AbilityPower { get; set; }
    public float BaseArmor { get; set; } = 5;
    public float BaseMagicResist { get; set; } = 5;
    public float Critical { get; set; }
    public float MagicCritical { get; set; }
    public float HPRegen { get; set; }
    public float MPRegen { get; set; }
    public float AttackSpeed { get; set; }
    public float MoveSpeed { get; set; } = 5f;
    public float HitRate { get; set; }
    public float Dodge { get; set; }
    
    // Advanced Stats
    public float ArmorPenetration { get; set; }
    public float MagicResistIgnore { get; set; }
    public float LifeSteal { get; set; }
    public float MagicLifeSteal { get; set; }
    public float CDReduction { get; set; }
    public float ManaCostReduction { get; set; }
    public float CriticalFactor { get; set; }
    public float DamageReduction { get; set; }
    public float PhysicsImmunization { get; set; }
    public float MagicImmunization { get; set; }
    public float ReboundInjuryRatio { get; set; }
    
    // Growth Stats (per level)
    public float StrengthGrowth { get; set; }
    public float IntelligenceGrowth { get; set; }
    public float AgilityGrowth { get; set; }
    public float HPGrowth { get; set; }
    public float MPGrowth { get; set; }
    public float DamageGrowth { get; set; }
    public float ArmorGrowth { get; set; }
    public float MagicResistGrowth { get; set; }
    
    // Attack
    public float AttackRange { get; set; } = 2.5f;
    public float AttackCooldown { get; set; } = 1.2f;
    public int ClientAttackSpeed { get; set; }
    
    // Hero Ratings (1-10)
    public int SurvivalRating { get; set; }
    public int AttackRating { get; set; }
    public int SkillRating { get; set; }
    public int DifficultyRating { get; set; }
    
    // Abilities
    public int[] AbilityIds { get; set; } = Array.Empty<int>();
    public int[] PassiveAbilityIds { get; set; } = Array.Empty<int>();
    
    // Tags
    public string[] Tags { get; set; } = Array.Empty<string>();
    
    // Equipment Recommendations
    public int[] RecommendedGear { get; set; } = Array.Empty<int>();
    public int[] RecommendedGear2 { get; set; } = Array.Empty<int>();
    public int[] RecommendedGear3 { get; set; } = Array.Empty<int>();
    
    // Birth Abilities (starting skills)
    public int[] BirthAbilityIds { get; set; } = Array.Empty<int>();
}

public static class HeroRegistry
{
    public static readonly Dictionary<int, HeroDefinition> Database = new()
    {
        // === TANKS ===
        [1] = new HeroDefinition
        {
            Id = 1, Name = "Keth", HeroType = "Tank", Role = "Top",
            Strength = 28, Intelligence = 15, Agility = 12,
            BaseHp = 620, BaseMp = 280, BaseDamage = 54, AbilityPower = 0,
            BaseArmor = 28, BaseMagicResist = 32, HPRegen = 7.5f, MPRegen = 4.2f,
            AttackSpeed = 0.65f, MoveSpeed = 3.3f, AttackRange = 1.7f, AttackCooldown = 1.4f,
            HPGrowth = 95, MPGrowth = 35, DamageGrowth = 3.5f, ArmorGrowth = 4.2f, MagicResistGrowth = 2.0f,
            SurvivalRating = 9, AttackRating = 4, SkillRating = 5, DifficultyRating = 3,
            AbilityIds = new[] { 101, 102, 103, 104 },
            Tags = new[] { "tank", "melee", "top" },
        },
        [4] = new HeroDefinition
        {
            Id = 4, Name = "Mikal", HeroType = "Tank", Role = "Top",
            Strength = 30, Intelligence = 14, Agility = 10,
            BaseHp = 650, BaseMp = 260, BaseDamage = 52, AbilityPower = 0,
            BaseArmor = 30, BaseMagicResist = 30, HPRegen = 8.0f, MPRegen = 3.8f,
            AttackSpeed = 0.62f, MoveSpeed = 3.3f, AttackRange = 1.7f, AttackCooldown = 1.5f,
            HPGrowth = 100, MPGrowth = 30, DamageGrowth = 3.2f, ArmorGrowth = 4.5f, MagicResistGrowth = 2.2f,
            SurvivalRating = 9, AttackRating = 3, SkillRating = 6, DifficultyRating = 4,
            AbilityIds = new[] { 401, 402, 403, 404 },
            Tags = new[] { "tank", "melee", "top" },
        },
        [14] = new HeroDefinition
        {
            Id = 14, Name = "Memphis", HeroType = "Tank", Role = "Support",
            Strength = 26, Intelligence = 18, Agility = 11,
            BaseHp = 600, BaseMp = 300, BaseDamage = 48, AbilityPower = 0,
            BaseArmor = 26, BaseMagicResist = 34, HPRegen = 7.0f, MPRegen = 5.0f,
            AttackSpeed = 0.63f, MoveSpeed = 3.3f, AttackRange = 1.7f, AttackCooldown = 1.4f,
            HPGrowth = 90, MPGrowth = 40, DamageGrowth = 3.0f, ArmorGrowth = 3.8f, MagicResistGrowth = 2.5f,
            SurvivalRating = 8, AttackRating = 3, SkillRating = 7, DifficultyRating = 5,
            AbilityIds = new[] { 1401, 1402, 1403, 1404 },
            Tags = new[] { "tank", "melee", "support" },
        },
        [26] = new HeroDefinition
        {
            Id = 26, Name = "Norah", HeroType = "Tank", Role = "Top",
            Strength = 32, Intelligence = 12, Agility = 10,
            BaseHp = 680, BaseMp = 240, BaseDamage = 56, AbilityPower = 0,
            BaseArmor = 32, BaseMagicResist = 28, HPRegen = 8.5f, MPRegen = 3.5f,
            AttackSpeed = 0.60f, MoveSpeed = 3.2f, AttackRange = 1.7f, AttackCooldown = 1.6f,
            HPGrowth = 105, MPGrowth = 28, DamageGrowth = 3.8f, ArmorGrowth = 5.0f, MagicResistGrowth = 1.8f,
            SurvivalRating = 10, AttackRating = 4, SkillRating = 4, DifficultyRating = 2,
            AbilityIds = new[] { 2601, 2602, 2603, 2604 },
            Tags = new[] { "tank", "melee", "top" },
        },
        [36] = new HeroDefinition
        {
            Id = 36, Name = "Lars", HeroType = "Tank", Role = "Top",
            Strength = 29, Intelligence = 16, Agility = 11,
            BaseHp = 640, BaseMp = 290, BaseDamage = 50, AbilityPower = 0,
            BaseArmor = 29, BaseMagicResist = 31, HPRegen = 7.8f, MPRegen = 4.5f,
            AttackSpeed = 0.64f, MoveSpeed = 3.3f, AttackRange = 1.7f, AttackCooldown = 1.45f,
            HPGrowth = 98, MPGrowth = 36, DamageGrowth = 3.3f, ArmorGrowth = 4.3f, MagicResistGrowth = 2.1f,
            SurvivalRating = 9, AttackRating = 4, SkillRating = 6, DifficultyRating = 4,
            AbilityIds = new[] { 3601, 3602, 3603, 3604 },
            Tags = new[] { "tank", "melee", "top" },
        },
        [41] = new HeroDefinition
        {
            Id = 41, Name = "Mina", HeroType = "Tank", Role = "Support",
            Strength = 27, Intelligence = 19, Agility = 10,
            BaseHp = 610, BaseMp = 310, BaseDamage = 46, AbilityPower = 0,
            BaseArmor = 27, BaseMagicResist = 33, HPRegen = 7.2f, MPRegen = 5.2f,
            AttackSpeed = 0.62f, MoveSpeed = 3.3f, AttackRange = 1.7f, AttackCooldown = 1.5f,
            HPGrowth = 92, MPGrowth = 42, DamageGrowth = 2.8f, ArmorGrowth = 3.9f, MagicResistGrowth = 2.3f,
            SurvivalRating = 8, AttackRating = 3, SkillRating = 8, DifficultyRating = 6,
            AbilityIds = new[] { 4101, 4102, 4103, 4104 },
            Tags = new[] { "tank", "melee", "support" },
        },
        [42] = new HeroDefinition
        {
            Id = 42, Name = "Berenice", HeroType = "Tank", Role = "Top",
            Strength = 31, Intelligence = 15, Agility = 9,
            BaseHp = 660, BaseMp = 270, BaseDamage = 53, AbilityPower = 0,
            BaseArmor = 31, BaseMagicResist = 29, HPRegen = 8.2f, MPRegen = 4.0f,
            AttackSpeed = 0.61f, MoveSpeed = 3.2f, AttackRange = 1.7f, AttackCooldown = 1.55f,
            HPGrowth = 102, MPGrowth = 32, DamageGrowth = 3.4f, ArmorGrowth = 4.6f, MagicResistGrowth = 1.9f,
            SurvivalRating = 9, AttackRating = 4, SkillRating = 5, DifficultyRating = 3,
            AbilityIds = new[] { 4201, 4202, 4203, 4204 },
            Tags = new[] { "tank", "melee", "top" },
        },
        [44] = new HeroDefinition
        {
            Id = 44, Name = "Brando", HeroType = "Tank", Role = "Top",
            Strength = 33, Intelligence = 11, Agility = 10,
            BaseHp = 700, BaseMp = 230, BaseDamage = 58, AbilityPower = 0,
            BaseArmor = 33, BaseMagicResist = 27, HPRegen = 9.0f, MPRegen = 3.2f,
            AttackSpeed = 0.58f, MoveSpeed = 3.1f, AttackRange = 1.7f, AttackCooldown = 1.7f,
            HPGrowth = 110, MPGrowth = 25, DamageGrowth = 4.0f, ArmorGrowth = 5.2f, MagicResistGrowth = 1.6f,
            SurvivalRating = 10, AttackRating = 5, SkillRating = 3, DifficultyRating = 2,
            AbilityIds = new[] { 4401, 4402, 4403, 4404 },
            Tags = new[] { "tank", "melee", "top" },
        },

        // === WARRIORS ===
        [2] = new HeroDefinition
        {
            Id = 2, Name = "Cleo", HeroType = "Warrior", Role = "Top",
            Strength = 25, Intelligence = 14, Agility = 18,
            BaseHp = 550, BaseMp = 250, BaseDamage = 60, AbilityPower = 0,
            BaseArmor = 22, BaseMagicResist = 28, HPRegen = 6.5f, MPRegen = 4.0f,
            AttackSpeed = 0.70f, MoveSpeed = 3.4f, AttackRange = 1.7f, AttackCooldown = 1.2f,
            HPGrowth = 85, MPGrowth = 32, DamageGrowth = 4.2f, ArmorGrowth = 3.5f, MagicResistGrowth = 2.0f,
            SurvivalRating = 7, AttackRating = 7, SkillRating = 6, DifficultyRating = 5,
            AbilityIds = new[] { 201, 202, 203, 204 },
            Tags = new[] { "warrior", "melee", "top" },
        },
        [22] = new HeroDefinition
        {
            Id = 22, Name = "Orsour", HeroType = "Warrior", Role = "Top",
            Strength = 27, Intelligence = 12, Agility = 16,
            BaseHp = 580, BaseMp = 240, BaseDamage = 62, AbilityPower = 0,
            BaseArmor = 24, BaseMagicResist = 26, HPRegen = 7.0f, MPRegen = 3.8f,
            AttackSpeed = 0.68f, MoveSpeed = 3.4f, AttackRange = 1.7f, AttackCooldown = 1.25f,
            HPGrowth = 88, MPGrowth = 30, DamageGrowth = 4.5f, ArmorGrowth = 3.8f, MagicResistGrowth = 1.8f,
            SurvivalRating = 7, AttackRating = 8, SkillRating = 5, DifficultyRating = 4,
            AbilityIds = new[] { 2201, 2202, 2203, 2204 },
            Tags = new[] { "warrior", "melee", "top" },
        },
        [24] = new HeroDefinition
        {
            Id = 24, Name = "Kyouya", HeroType = "Warrior", Role = "Top",
            Strength = 26, Intelligence = 13, Agility = 19,
            BaseHp = 540, BaseMp = 245, BaseDamage = 64, AbilityPower = 0,
            BaseArmor = 21, BaseMagicResist = 27, HPRegen = 6.2f, MPRegen = 3.9f,
            AttackSpeed = 0.72f, MoveSpeed = 3.5f, AttackRange = 1.7f, AttackCooldown = 1.15f,
            HPGrowth = 82, MPGrowth = 31, DamageGrowth = 4.8f, ArmorGrowth = 3.2f, MagicResistGrowth = 2.1f,
            SurvivalRating = 6, AttackRating = 8, SkillRating = 7, DifficultyRating = 6,
            AbilityIds = new[] { 2401, 2402, 2403, 2404 },
            Tags = new[] { "warrior", "melee", "top" },
        },
        [28] = new HeroDefinition
        {
            Id = 28, Name = "Wulfric", HeroType = "Warrior", Role = "Top",
            Strength = 28, Intelligence = 11, Agility = 15,
            BaseHp = 590, BaseMp = 230, BaseDamage = 66, AbilityPower = 0,
            BaseArmor = 25, BaseMagicResist = 25, HPRegen = 7.2f, MPRegen = 3.5f,
            AttackSpeed = 0.67f, MoveSpeed = 3.3f, AttackRange = 1.7f, AttackCooldown = 1.3f,
            HPGrowth = 90, MPGrowth = 28, DamageGrowth = 5.0f, ArmorGrowth = 4.0f, MagicResistGrowth = 1.7f,
            SurvivalRating = 7, AttackRating = 9, SkillRating = 4, DifficultyRating = 3,
            AbilityIds = new[] { 2801, 2802, 2803, 2804 },
            Tags = new[] { "warrior", "melee", "top" },
        },
        [32] = new HeroDefinition
        {
            Id = 32, Name = "Xenos", HeroType = "Warrior", Role = "Top",
            Strength = 24, Intelligence = 15, Agility = 17,
            BaseHp = 530, BaseMp = 260, BaseDamage = 58, AbilityPower = 0,
            BaseArmor = 20, BaseMagicResist = 29, HPRegen = 6.0f, MPRegen = 4.2f,
            AttackSpeed = 0.71f, MoveSpeed = 3.4f, AttackRange = 1.7f, AttackCooldown = 1.18f,
            HPGrowth = 80, MPGrowth = 34, DamageGrowth = 4.0f, ArmorGrowth = 3.0f, MagicResistGrowth = 2.3f,
            SurvivalRating = 6, AttackRating = 7, SkillRating = 7, DifficultyRating = 6,
            AbilityIds = new[] { 3201, 3202, 3203, 3204 },
            Tags = new[] { "warrior", "melee", "top" },
        },
        [33] = new HeroDefinition
        {
            Id = 33, Name = "Garsea", HeroType = "Warrior", Role = "Jungle",
            Strength = 26, Intelligence = 13, Agility = 18,
            BaseHp = 560, BaseMp = 250, BaseDamage = 61, AbilityPower = 0,
            BaseArmor = 23, BaseMagicResist = 27, HPRegen = 6.8f, MPRegen = 4.0f,
            AttackSpeed = 0.69f, MoveSpeed = 3.4f, AttackRange = 1.7f, AttackCooldown = 1.22f,
            HPGrowth = 86, MPGrowth = 32, DamageGrowth = 4.3f, ArmorGrowth = 3.6f, MagicResistGrowth = 2.0f,
            SurvivalRating = 7, AttackRating = 7, SkillRating = 6, DifficultyRating = 5,
            AbilityIds = new[] { 3301, 3302, 3303, 3304 },
            Tags = new[] { "warrior", "melee", "jungle" },
        },
        [40] = new HeroDefinition
        {
            Id = 40, Name = "Sarn", HeroType = "Warrior", Role = "Top",
            Strength = 29, Intelligence = 10, Agility = 14,
            BaseHp = 600, BaseMp = 220, BaseDamage = 68, AbilityPower = 0,
            BaseArmor = 26, BaseMagicResist = 24, HPRegen = 7.5f, MPRegen = 3.2f,
            AttackSpeed = 0.66f, MoveSpeed = 3.3f, AttackRange = 1.7f, AttackCooldown = 1.35f,
            HPGrowth = 92, MPGrowth = 26, DamageGrowth = 5.2f, ArmorGrowth = 4.2f, MagicResistGrowth = 1.5f,
            SurvivalRating = 8, AttackRating = 9, SkillRating = 3, DifficultyRating = 2,
            AbilityIds = new[] { 4001, 4002, 4003, 4004 },
            Tags = new[] { "warrior", "melee", "top" },
        },

        // === ASSASSINS ===
        [12] = new HeroDefinition
        {
            Id = 12, Name = "Styx", HeroType = "Assassin", Role = "Jungle",
            Strength = 18, Intelligence = 14, Agility = 28,
            BaseHp = 450, BaseMp = 220, BaseDamage = 72, AbilityPower = 0,
            BaseArmor = 15, BaseMagicResist = 25, HPRegen = 5.0f, MPRegen = 4.5f,
            AttackSpeed = 0.80f, MoveSpeed = 3.6f, AttackRange = 1.7f, AttackCooldown = 1.0f,
            HPGrowth = 65, MPGrowth = 28, DamageGrowth = 5.5f, ArmorGrowth = 2.5f, MagicResistGrowth = 2.0f,
            SurvivalRating = 4, AttackRating = 9, SkillRating = 8, DifficultyRating = 7,
            AbilityIds = new[] { 1201, 1202, 1203, 1204 },
            Tags = new[] { "assassin", "melee", "jungle" },
        },
        [13] = new HeroDefinition
        {
            Id = 13, Name = "Clawdia", HeroType = "Assassin", Role = "Jungle",
            Strength = 17, Intelligence = 15, Agility = 29,
            BaseHp = 440, BaseMp = 230, BaseDamage = 70, AbilityPower = 0,
            BaseArmor = 14, BaseMagicResist = 26, HPRegen = 4.8f, MPRegen = 4.8f,
            AttackSpeed = 0.82f, MoveSpeed = 3.7f, AttackRange = 1.7f, AttackCooldown = 0.95f,
            HPGrowth = 62, MPGrowth = 30, DamageGrowth = 5.2f, ArmorGrowth = 2.2f, MagicResistGrowth = 2.2f,
            SurvivalRating = 4, AttackRating = 9, SkillRating = 8, DifficultyRating = 8,
            AbilityIds = new[] { 1301, 1302, 1303, 1304 },
            Tags = new[] { "assassin", "melee", "jungle" },
        },
        [16] = new HeroDefinition
        {
            Id = 16, Name = "Zoya", HeroType = "Assassin", Role = "Jungle",
            Strength = 16, Intelligence = 16, Agility = 30,
            BaseHp = 430, BaseMp = 240, BaseDamage = 68, AbilityPower = 10,
            BaseArmor = 13, BaseMagicResist = 27, HPRegen = 4.5f, MPRegen = 5.0f,
            AttackSpeed = 0.85f, MoveSpeed = 3.8f, AttackRange = 1.7f, AttackCooldown = 0.9f,
            HPGrowth = 58, MPGrowth = 32, DamageGrowth = 5.0f, ArmorGrowth = 2.0f, MagicResistGrowth = 2.3f,
            SurvivalRating = 3, AttackRating = 8, SkillRating = 9, DifficultyRating = 9,
            AbilityIds = new[] { 1601, 1602, 1603, 1604 },
            Tags = new[] { "assassin", "melee", "jungle" },
        },
        [19] = new HeroDefinition
        {
            Id = 19, Name = "Christine", HeroType = "Assassin", Role = "Jungle",
            Strength = 19, Intelligence = 13, Agility = 27,
            BaseHp = 460, BaseMp = 215, BaseDamage = 74, AbilityPower = 0,
            BaseArmor = 16, BaseMagicResist = 24, HPRegen = 5.2f, MPRegen = 4.2f,
            AttackSpeed = 0.78f, MoveSpeed = 3.6f, AttackRange = 1.7f, AttackCooldown = 1.05f,
            HPGrowth = 68, MPGrowth = 26, DamageGrowth = 5.8f, ArmorGrowth = 2.8f, MagicResistGrowth = 1.8f,
            SurvivalRating = 5, AttackRating = 9, SkillRating = 7, DifficultyRating = 6,
            AbilityIds = new[] { 1901, 1902, 1903, 1904 },
            Tags = new[] { "assassin", "melee", "jungle" },
        },
        [20] = new HeroDefinition
        {
            Id = 20, Name = "Raven", HeroType = "Assassin", Role = "Jungle",
            Strength = 15, Intelligence = 17, Agility = 31,
            BaseHp = 420, BaseMp = 250, BaseDamage = 66, AbilityPower = 15,
            BaseArmor = 12, BaseMagicResist = 28, HPRegen = 4.2f, MPRegen = 5.5f,
            AttackSpeed = 0.88f, MoveSpeed = 3.9f, AttackRange = 1.7f, AttackCooldown = 0.85f,
            HPGrowth = 55, MPGrowth = 35, DamageGrowth = 4.8f, ArmorGrowth = 1.8f, MagicResistGrowth = 2.5f,
            SurvivalRating = 3, AttackRating = 7, SkillRating = 10, DifficultyRating = 10,
            AbilityIds = new[] { 2001, 2002, 2003, 2004 },
            Tags = new[] { "assassin", "melee", "jungle" },
        },
        [29] = new HeroDefinition
        {
            Id = 29, Name = "Oboro", HeroType = "Assassin", Role = "Mid",
            Strength = 17, Intelligence = 18, Agility = 26,
            BaseHp = 445, BaseMp = 260, BaseDamage = 65, AbilityPower = 20,
            BaseArmor = 14, BaseMagicResist = 29, HPRegen = 4.6f, MPRegen = 5.2f,
            AttackSpeed = 0.83f, MoveSpeed = 3.7f, AttackRange = 1.7f, AttackCooldown = 0.92f,
            HPGrowth = 60, MPGrowth = 34, DamageGrowth = 4.9f, ArmorGrowth = 2.1f, MagicResistGrowth = 2.4f,
            SurvivalRating = 4, AttackRating = 8, SkillRating = 9, DifficultyRating = 8,
            AbilityIds = new[] { 2901, 2902, 2903, 2904 },
            Tags = new[] { "assassin", "melee", "mid" },
        },

        // === MAGES ===
        [5] = new HeroDefinition
        {
            Id = 5, Name = "Winona", HeroType = "Mage", Role = "Mid",
            Strength = 12, Intelligence = 30, Agility = 10,
            BaseHp = 420, BaseMp = 400, BaseDamage = 48, AbilityPower = 65,
            BaseArmor = 12, BaseMagicResist = 30, HPRegen = 4.0f, MPRegen = 7.0f,
            AttackSpeed = 0.60f, MoveSpeed = 3.3f, AttackRange = 5.5f, AttackCooldown = 1.2f,
            HPGrowth = 55, MPGrowth = 50, DamageGrowth = 2.5f, ArmorGrowth = 1.5f, MagicResistGrowth = 2.0f,
            SurvivalRating = 3, AttackRating = 8, SkillRating = 9, DifficultyRating = 7,
            AbilityIds = new[] { 501, 502, 503, 504 },
            Tags = new[] { "mage", "ranged", "mid" },
        },
        [7] = new HeroDefinition
        {
            Id = 7, Name = "Nivan", HeroType = "Mage", Role = "Mid",
            Strength = 11, Intelligence = 32, Agility = 9,
            BaseHp = 400, BaseMp = 420, BaseDamage = 45, AbilityPower = 70,
            BaseArmor = 10, BaseMagicResist = 32, HPRegen = 3.5f, MPRegen = 7.5f,
            AttackSpeed = 0.58f, MoveSpeed = 3.2f, AttackRange = 6.0f, AttackCooldown = 1.3f,
            HPGrowth = 50, MPGrowth = 55, DamageGrowth = 2.2f, ArmorGrowth = 1.2f, MagicResistGrowth = 2.2f,
            SurvivalRating = 2, AttackRating = 9, SkillRating = 10, DifficultyRating = 9,
            AbilityIds = new[] { 701, 702, 703, 704 },
            Tags = new[] { "mage", "ranged", "mid" },
        },
        [17] = new HeroDefinition
        {
            Id = 17, Name = "Reisa", HeroType = "Mage", Role = "Mid",
            Strength = 13, Intelligence = 28, Agility = 11,
            BaseHp = 440, BaseMp = 380, BaseDamage = 50, AbilityPower = 60,
            BaseArmor = 13, BaseMagicResist = 28, HPRegen = 4.5f, MPRegen = 6.5f,
            AttackSpeed = 0.62f, MoveSpeed = 3.4f, AttackRange = 5.0f, AttackCooldown = 1.15f,
            HPGrowth = 58, MPGrowth = 45, DamageGrowth = 2.8f, ArmorGrowth = 1.8f, MagicResistGrowth = 1.8f,
            SurvivalRating = 4, AttackRating = 7, SkillRating = 8, DifficultyRating = 6,
            AbilityIds = new[] { 1701, 1702, 1703, 1704 },
            Tags = new[] { "mage", "ranged", "mid" },
        },
        [18] = new HeroDefinition
        {
            Id = 18, Name = "Hagen", HeroType = "Mage", Role = "Mid",
            Strength = 14, Intelligence = 27, Agility = 12,
            BaseHp = 460, BaseMp = 370, BaseDamage = 52, AbilityPower = 58,
            BaseArmor = 14, BaseMagicResist = 27, HPRegen = 4.8f, MPRegen = 6.2f,
            AttackSpeed = 0.64f, MoveSpeed = 3.4f, AttackRange = 4.5f, AttackCooldown = 1.1f,
            HPGrowth = 62, MPGrowth = 42, DamageGrowth = 3.0f, ArmorGrowth = 2.0f, MagicResistGrowth = 1.6f,
            SurvivalRating = 5, AttackRating = 6, SkillRating = 8, DifficultyRating = 5,
            AbilityIds = new[] { 1801, 1802, 1803, 1804 },
            Tags = new[] { "mage", "ranged", "mid" },
        },
        [27] = new HeroDefinition
        {
            Id = 27, Name = "Salome", HeroType = "Mage", Role = "Mid",
            Strength = 10, Intelligence = 34, Agility = 8,
            BaseHp = 380, BaseMp = 440, BaseDamage = 42, AbilityPower = 75,
            BaseArmor = 8, BaseMagicResist = 34, HPRegen = 3.0f, MPRegen = 8.0f,
            AttackSpeed = 0.55f, MoveSpeed = 3.1f, AttackRange = 6.5f, AttackCooldown = 1.4f,
            HPGrowth = 45, MPGrowth = 58, DamageGrowth = 2.0f, ArmorGrowth = 1.0f, MagicResistGrowth = 2.5f,
            SurvivalRating = 2, AttackRating = 10, SkillRating = 10, DifficultyRating = 10,
            AbilityIds = new[] { 2701, 2702, 2703, 2704 },
            Tags = new[] { "mage", "ranged", "mid" },
        },
        [37] = new HeroDefinition
        {
            Id = 37, Name = "Naraku", HeroType = "Mage", Role = "Mid",
            Strength = 12, Intelligence = 29, Agility = 11,
            BaseHp = 430, BaseMp = 390, BaseDamage = 47, AbilityPower = 62,
            BaseArmor = 11, BaseMagicResist = 31, HPRegen = 4.2f, MPRegen = 6.8f,
            AttackSpeed = 0.61f, MoveSpeed = 3.3f, AttackRange = 5.5f, AttackCooldown = 1.18f,
            HPGrowth = 56, MPGrowth = 48, DamageGrowth = 2.6f, ArmorGrowth = 1.6f, MagicResistGrowth = 2.1f,
            SurvivalRating = 3, AttackRating = 8, SkillRating = 9, DifficultyRating = 8,
            AbilityIds = new[] { 3701, 3702, 3703, 3704 },
            Tags = new[] { "mage", "ranged", "mid" },
        },
        [38] = new HeroDefinition
        {
            Id = 38, Name = "Natasha", HeroType = "Mage", Role = "Mid",
            Strength = 11, Intelligence = 31, Agility = 10,
            BaseHp = 410, BaseMp = 410, BaseDamage = 46, AbilityPower = 68,
            BaseArmor = 9, BaseMagicResist = 33, HPRegen = 3.8f, MPRegen = 7.2f,
            AttackSpeed = 0.59f, MoveSpeed = 3.2f, AttackRange = 5.8f, AttackCooldown = 1.25f,
            HPGrowth = 52, MPGrowth = 52, DamageGrowth = 2.3f, ArmorGrowth = 1.3f, MagicResistGrowth = 2.3f,
            SurvivalRating = 2, AttackRating = 9, SkillRating = 9, DifficultyRating = 9,
            AbilityIds = new[] { 3801, 3802, 3803, 3804 },
            Tags = new[] { "mage", "ranged", "mid" },
        },

        // === MARKSMEN ===
        [9] = new HeroDefinition
        {
            Id = 9, Name = "Valena", HeroType = "Marksman", Role = "Bot",
            Strength = 10, Intelligence = 14, Agility = 28,
            BaseHp = 410, BaseMp = 280, BaseDamage = 62, AbilityPower = 0,
            BaseArmor = 10, BaseMagicResist = 25, HPRegen = 4.5f, MPRegen = 4.5f,
            AttackSpeed = 0.85f, MoveSpeed = 3.25f, AttackRange = 6.0f, AttackCooldown = 0.9f,
            HPGrowth = 52, MPGrowth = 35, DamageGrowth = 4.5f, ArmorGrowth = 1.5f, MagicResistGrowth = 1.5f,
            SurvivalRating = 3, AttackRating = 9, SkillRating = 6, DifficultyRating = 5,
            AbilityIds = new[] { 901, 902, 903, 904 },
            Tags = new[] { "marksman", "ranged", "bot" },
        },
        [11] = new HeroDefinition
        {
            Id = 11, Name = "Eida", HeroType = "Marksman", Role = "Bot",
            Strength = 9, Intelligence = 15, Agility = 30,
            BaseHp = 390, BaseMp = 290, BaseDamage = 65, AbilityPower = 0,
            BaseArmor = 8, BaseMagicResist = 26, HPRegen = 4.0f, MPRegen = 4.8f,
            AttackSpeed = 0.90f, MoveSpeed = 3.3f, AttackRange = 6.5f, AttackCooldown = 0.85f,
            HPGrowth = 48, MPGrowth = 38, DamageGrowth = 4.8f, ArmorGrowth = 1.2f, MagicResistGrowth = 1.6f,
            SurvivalRating = 2, AttackRating = 10, SkillRating = 7, DifficultyRating = 6,
            AbilityIds = new[] { 1101, 1102, 1103, 1104 },
            Tags = new[] { "marksman", "ranged", "bot" },
        },
        [8] = new HeroDefinition
        {
            Id = 8, Name = "Tigria", HeroType = "Marksman", Role = "Bot",
            Strength = 11, Intelligence = 13, Agility = 27,
            BaseHp = 430, BaseMp = 270, BaseDamage = 60, AbilityPower = 0,
            BaseArmor = 11, BaseMagicResist = 24, HPRegen = 4.8f, MPRegen = 4.2f,
            AttackSpeed = 0.82f, MoveSpeed = 3.2f, AttackRange = 5.5f, AttackCooldown = 0.95f,
            HPGrowth = 55, MPGrowth = 32, DamageGrowth = 4.2f, ArmorGrowth = 1.8f, MagicResistGrowth = 1.4f,
            SurvivalRating = 4, AttackRating = 8, SkillRating = 6, DifficultyRating = 4,
            AbilityIds = new[] { 801, 802, 803, 804 },
            Tags = new[] { "marksman", "ranged", "bot" },
        },
        [25] = new HeroDefinition
        {
            Id = 25, Name = "Scorpion", HeroType = "Marksman", Role = "Bot",
            Strength = 10, Intelligence = 16, Agility = 29,
            BaseHp = 400, BaseMp = 300, BaseDamage = 63, AbilityPower = 10,
            BaseArmor = 9, BaseMagicResist = 27, HPRegen = 4.2f, MPRegen = 5.0f,
            AttackSpeed = 0.88f, MoveSpeed = 3.35f, AttackRange = 6.2f, AttackCooldown = 0.88f,
            HPGrowth = 50, MPGrowth = 40, DamageGrowth = 4.6f, ArmorGrowth = 1.3f, MagicResistGrowth = 1.7f,
            SurvivalRating = 3, AttackRating = 9, SkillRating = 7, DifficultyRating = 6,
            AbilityIds = new[] { 2501, 2502, 2503, 2504 },
            Tags = new[] { "marksman", "ranged", "bot" },
        },
        [30] = new HeroDefinition
        {
            Id = 30, Name = "Adele", HeroType = "Marksman", Role = "Bot",
            Strength = 11, Intelligence = 12, Agility = 26,
            BaseHp = 440, BaseMp = 260, BaseDamage = 58, AbilityPower = 0,
            BaseArmor = 12, BaseMagicResist = 23, HPRegen = 5.0f, MPRegen = 4.0f,
            AttackSpeed = 0.80f, MoveSpeed = 3.2f, AttackRange = 5.0f, AttackCooldown = 1.0f,
            HPGrowth = 58, MPGrowth = 30, DamageGrowth = 4.0f, ArmorGrowth = 2.0f, MagicResistGrowth = 1.2f,
            SurvivalRating = 4, AttackRating = 8, SkillRating = 5, DifficultyRating = 3,
            AbilityIds = new[] { 3001, 3002, 3003, 3004 },
            Tags = new[] { "marksman", "ranged", "bot" },
        },
        [34] = new HeroDefinition
        {
            Id = 34, Name = "Austeja", HeroType = "Marksman", Role = "Bot",
            Strength = 10, Intelligence = 14, Agility = 28,
            BaseHp = 415, BaseMp = 285, BaseDamage = 61, AbilityPower = 5,
            BaseArmor = 10, BaseMagicResist = 25, HPRegen = 4.4f, MPRegen = 4.6f,
            AttackSpeed = 0.86f, MoveSpeed = 3.28f, AttackRange = 5.8f, AttackCooldown = 0.92f,
            HPGrowth = 53, MPGrowth = 36, DamageGrowth = 4.4f, ArmorGrowth = 1.4f, MagicResistGrowth = 1.5f,
            SurvivalRating = 3, AttackRating = 9, SkillRating = 7, DifficultyRating = 5,
            AbilityIds = new[] { 3401, 3402, 3403, 3404 },
            Tags = new[] { "marksman", "ranged", "bot" },
        },
        [39] = new HeroDefinition
        {
            Id = 39, Name = "Solo", HeroType = "Marksman", Role = "Bot",
            Strength = 12, Intelligence = 11, Agility = 25,
            BaseHp = 450, BaseMp = 250, BaseDamage = 56, AbilityPower = 0,
            BaseArmor = 13, BaseMagicResist = 22, HPRegen = 5.2f, MPRegen = 3.8f,
            AttackSpeed = 0.78f, MoveSpeed = 3.15f, AttackRange = 5.0f, AttackCooldown = 1.05f,
            HPGrowth = 60, MPGrowth = 28, DamageGrowth = 3.8f, ArmorGrowth = 2.2f, MagicResistGrowth = 1.0f,
            SurvivalRating = 5, AttackRating = 7, SkillRating = 5, DifficultyRating = 3,
            AbilityIds = new[] { 3901, 3902, 3903, 3904 },
            Tags = new[] { "marksman", "ranged", "bot" },
        },

        // === SUPPORTS ===
        [15] = new HeroDefinition
        {
            Id = 15, Name = "Loth", HeroType = "Support", Role = "Support",
            Strength = 18, Intelligence = 25, Agility = 12,
            BaseHp = 500, BaseMp = 350, BaseDamage = 42, AbilityPower = 40,
            BaseArmor = 18, BaseMagicResist = 32, HPRegen = 6.0f, MPRegen = 6.5f,
            AttackSpeed = 0.62f, MoveSpeed = 3.3f, AttackRange = 4.0f, AttackCooldown = 1.2f,
            HPGrowth = 70, MPGrowth = 45, DamageGrowth = 2.0f, ArmorGrowth = 2.5f, MagicResistGrowth = 2.5f,
            SurvivalRating = 6, AttackRating = 3, SkillRating = 9, DifficultyRating = 7,
            AbilityIds = new[] { 1501, 1502, 1503, 1504 },
            Tags = new[] { "support", "ranged", "support" },
        },
        [21] = new HeroDefinition
        {
            Id = 21, Name = "Judith", HeroType = "Support", Role = "Support",
            Strength = 16, Intelligence = 28, Agility = 10,
            BaseHp = 480, BaseMp = 380, BaseDamage = 38, AbilityPower = 45,
            BaseArmor = 16, BaseMagicResist = 34, HPRegen = 5.5f, MPRegen = 7.0f,
            AttackSpeed = 0.58f, MoveSpeed = 3.2f, AttackRange = 4.5f, AttackCooldown = 1.3f,
            HPGrowth = 65, MPGrowth = 50, DamageGrowth = 1.8f, ArmorGrowth = 2.0f, MagicResistGrowth = 2.8f,
            SurvivalRating = 5, AttackRating = 2, SkillRating = 10, DifficultyRating = 8,
            AbilityIds = new[] { 2101, 2102, 2103, 2104 },
            Tags = new[] { "support", "ranged", "support" },
        },
        [23] = new HeroDefinition
        {
            Id = 23, Name = "Idony", HeroType = "Support", Role = "Support",
            Strength = 15, Intelligence = 26, Agility = 13,
            BaseHp = 470, BaseMp = 360, BaseDamage = 40, AbilityPower = 42,
            BaseArmor = 15, BaseMagicResist = 33, HPRegen = 5.8f, MPRegen = 6.8f,
            AttackSpeed = 0.60f, MoveSpeed = 3.25f, AttackRange = 4.2f, AttackCooldown = 1.25f,
            HPGrowth = 68, MPGrowth = 48, DamageGrowth = 1.9f, ArmorGrowth = 2.2f, MagicResistGrowth = 2.6f,
            SurvivalRating = 5, AttackRating = 3, SkillRating = 9, DifficultyRating = 7,
            AbilityIds = new[] { 2301, 2302, 2303, 2304 },
            Tags = new[] { "support", "ranged", "support" },
        },
        [31] = new HeroDefinition
        {
            Id = 31, Name = "Joey", HeroType = "Support", Role = "Support",
            Strength = 20, Intelligence = 22, Agility = 14,
            BaseHp = 520, BaseMp = 340, BaseDamage = 44, AbilityPower = 35,
            BaseArmor = 20, BaseMagicResist = 30, HPRegen = 6.5f, MPRegen = 6.0f,
            AttackSpeed = 0.65f, MoveSpeed = 3.35f, AttackRange = 1.7f, AttackCooldown = 1.15f,
            HPGrowth = 75, MPGrowth = 42, DamageGrowth = 2.2f, ArmorGrowth = 3.0f, MagicResistGrowth = 2.2f,
            SurvivalRating = 7, AttackRating = 4, SkillRating = 8, DifficultyRating = 5,
            AbilityIds = new[] { 3101, 3102, 3103, 3104 },
            Tags = new[] { "support", "melee", "support" },
        },
        [35] = new HeroDefinition
        {
            Id = 35, Name = "Chloe", HeroType = "Support", Role = "Support",
            Strength = 14, Intelligence = 29, Agility = 11,
            BaseHp = 460, BaseMp = 390, BaseDamage = 36, AbilityPower = 48,
            BaseArmor = 14, BaseMagicResist = 35, HPRegen = 5.2f, MPRegen = 7.2f,
            AttackSpeed = 0.56f, MoveSpeed = 3.15f, AttackRange = 4.8f, AttackCooldown = 1.35f,
            HPGrowth = 62, MPGrowth = 52, DamageGrowth = 1.6f, ArmorGrowth = 1.8f, MagicResistGrowth = 3.0f,
            SurvivalRating = 4, AttackRating = 2, SkillRating = 10, DifficultyRating = 9,
            AbilityIds = new[] { 3501, 3502, 3503, 3504 },
            Tags = new[] { "support", "ranged", "support" },
        },

        // === UNIQUE/ALL ===
        [3] = new HeroDefinition
        {
            Id = 3, Name = "Brunhild", HeroType = "Warrior", Role = "Top",
            Strength = 26, Intelligence = 13, Agility = 17,
            BaseHp = 560, BaseMp = 245, BaseDamage = 63, AbilityPower = 0,
            BaseArmor = 23, BaseMagicResist = 27, HPRegen = 6.8f, MPRegen = 4.0f,
            AttackSpeed = 0.69f, MoveSpeed = 3.4f, AttackRange = 1.7f, AttackCooldown = 1.2f,
            HPGrowth = 87, MPGrowth = 31, DamageGrowth = 4.4f, ArmorGrowth = 3.7f, MagicResistGrowth = 1.9f,
            SurvivalRating = 7, AttackRating = 7, SkillRating = 6, DifficultyRating = 5,
            AbilityIds = new[] { 301, 302, 303, 304 },
            Tags = new[] { "warrior", "melee", "top" },
        },
        [6] = new HeroDefinition
        {
            Id = 6, Name = "Moros", HeroType = "Mage", Role = "Mid",
            Strength = 13, Intelligence = 29, Agility = 10,
            BaseHp = 430, BaseMp = 395, BaseDamage = 49, AbilityPower = 63,
            BaseArmor = 12, BaseMagicResist = 29, HPRegen = 4.2f, MPRegen = 6.8f,
            AttackSpeed = 0.61f, MoveSpeed = 3.3f, AttackRange = 5.2f, AttackCooldown = 1.18f,
            HPGrowth = 57, MPGrowth = 47, DamageGrowth = 2.7f, ArmorGrowth = 1.7f, MagicResistGrowth = 1.9f,
            SurvivalRating = 4, AttackRating = 7, SkillRating = 9, DifficultyRating = 7,
            AbilityIds = new[] { 601, 602, 603, 604 },
            Tags = new[] { "mage", "ranged", "mid" },
        },
        [10] = new HeroDefinition
        {
            Id = 10, Name = "Aramis", HeroType = "Marksman", Role = "Bot",
            Strength = 10, Intelligence = 13, Agility = 29,
            BaseHp = 405, BaseMp = 275, BaseDamage = 64, AbilityPower = 5,
            BaseArmor = 9, BaseMagicResist = 26, HPRegen = 4.3f, MPRegen = 4.6f,
            AttackSpeed = 0.87f, MoveSpeed = 3.3f, AttackRange = 6.3f, AttackCooldown = 0.87f,
            HPGrowth = 51, MPGrowth = 37, DamageGrowth = 4.7f, ArmorGrowth = 1.3f, MagicResistGrowth = 1.6f,
            SurvivalRating = 3, AttackRating = 9, SkillRating = 7, DifficultyRating = 6,
            AbilityIds = new[] { 1001, 1002, 1003, 1004 },
            Tags = new[] { "marksman", "ranged", "bot" },
        },
    };
}

public static class HeroFactory
{
    public static Entity CreateHero(World world, int heroId, int teamId, EvoVector3 position, MatchConfig config)
    {
        if (!HeroRegistry.Database.TryGetValue(heroId, out var hero))
            throw new ArgumentException($"Unknown hero id: {heroId}");

        var entity = world.Create();

        world.AddComponent(entity, new PositionComponent { Value = position });
        world.AddComponent(entity, new VelocityComponent { Value = EvoVector3.Zero });
        world.AddComponent(entity, new HealthComponent { Current = hero.BaseHp, Max = hero.BaseHp });
        world.AddComponent(entity, new ManaComponent { Current = hero.BaseMp, Max = hero.BaseMp });
        world.AddComponent(entity, new TeamComponent { TeamId = teamId });
        world.AddComponent(entity, new ArmorComponent { BaseArmor = hero.BaseArmor });
        world.AddComponent(entity, new MagicResistComponent { BaseResist = hero.BaseMagicResist });
        world.AddComponent(entity, new AttackComponent
        {
            Damage = hero.BaseDamage, Range = hero.AttackRange,
            CooldownTime = hero.AttackCooldown,
            TimeUntilNextAttack = 0f, CurrentTargetId = Entity.Null.Id,
        });
        world.AddComponent(entity, new MateBrainComponent
        {
            PerceptionRange = 18, IsAggressive = true, ReactionTime = 0.15f,
            PathRecalculationCooldown = 0.4f, RetreatHealthThreshold = 0.2f,
            ClumsinessFactor = 0.3f,
        });
        world.AddComponent(entity, new MOBAComponent());
        world.AddComponent(entity, new PathfollowComponent
        {
            Waypoints = new List<EvoVector3>(), CurrentWaypointIndex = 0,
            StoppingDistance = 0.3f,
        });
        world.AddComponent(entity, new InventoryComponent(500));
        world.AddComponent(entity, new CurrencyComponent
        {
            BattlePoints = config.StartGold,
            Diamonds = 0,
            Fragments = 0,
            Tickets = 0,
        });
        world.AddComponent(entity, new DailyDealsComponent());
        world.AddComponent(entity, new WeeklyDealsComponent());
        world.AddComponent(entity, new BattlePassComponent
        {
            CurrentLevel = 1,
            CurrentXp = 0,
            XpToNextLevel = 1000,
            IsPremium = false,
            ClaimedRewards = new bool[BattlePassService.MaxLevel],
            SeasonEnd = DateTime.UtcNow.AddDays(30),
        });
        world.AddComponent(entity, new AchievementProgressComponent
        {
            Unlocked = new bool[64],
        });
        world.AddComponent(entity, new DailyChallengesComponent
        {
            Challenges = DailyChallengeService.GenerateDailyChallenges(),
            LastReset = DateTime.UtcNow,
        });
        world.AddComponent(entity, new PlayerSkillTracker());
        world.AddComponent(entity, new AdaptiveDifficultyComponent());
        world.AddComponent(entity, new BotChatComponent
        {
            NextChatTime = 5f,
            ChatCooldown = 10f,
            Personality = heroId,
        });
        world.AddComponent(entity, new ChatQueueComponent
        {
            PendingMessages = new Queue<ChatMessage>(),
        });
        world.AddComponent(entity, new BotIntelligenceComponent
        {
            Personality = hero.HeroType switch
            {
                "Tank" => BotPersonalityType.Aggressive,
                "Mage" => BotPersonalityType.Strategic,
                "Assassin" => BotPersonalityType.Assassin,
                "Marksman" => BotPersonalityType.Strategic,
                "Support" => BotPersonalityType.Supportive,
                _ => BotPersonalityType.Aggressive,
            },
            CurrentEmotion = BotEmotion.Neutral,
        });
        world.AddComponent(entity, new PlayerPatternTracker
        {
            Patterns = new Dictionary<int, PlayerPattern>(),
        });
        world.AddComponent(entity, new MetaAdaptationComponent());
        world.AddComponent(entity, new TeamCoordinationComponent
        {
            AssignedRole = heroId,
            CoordinationCooldown = 5f,
        });
        world.AddComponent(entity, new LevelComponent
        {
            Level = 1, CurrentXp = 0, XpToNext = 100,
        });
        world.AddComponent(entity, new XpGrantComponent { Reward = 50 });
        world.AddComponent(entity, new BuffComponent());
        world.AddComponent(entity, new MatchStatsTracker());

        if (hero.AbilityIds.Length >= 1)
        {
            world.AddComponent(entity, new AbilitySlotComponent
            {
                Q = new AbilitySlot { DataId = hero.AbilityIds.Length > 0 ? hero.AbilityIds[0] : 0 },
                W = new AbilitySlot { DataId = hero.AbilityIds.Length > 1 ? hero.AbilityIds[1] : 0 },
                E = new AbilitySlot { DataId = hero.AbilityIds.Length > 2 ? hero.AbilityIds[2] : 0 },
                R = new AbilitySlot { DataId = hero.AbilityIds.Length > 3 ? hero.AbilityIds[3] : 0 },
            });
        }

        if (config.EnableFogOfWar)
            world.AddComponent(entity, new VisionComponent { SightRange = 15f, TeamId = teamId });

        if (config.EnableTeamPlanner)
            world.AddComponent(entity, new TeamGoalComponent { Goal = TeamGoalType.Farm });

        return entity;
    }
}
