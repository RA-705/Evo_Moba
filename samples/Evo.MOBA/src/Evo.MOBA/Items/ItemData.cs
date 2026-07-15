using System.Collections.Generic;

namespace Evo.MOBA.Items;

public enum ItemCategory
{
    Consumable,
    PhysicalAttack,
    MagicAttack,
    Defense,
    Movement,
    Jungle,
    Support,
    Active,
}

public readonly struct ItemData
{
    public int Id { get; init; }
    public string Name { get; init; }
    public ItemCategory Category { get; init; }
    public int Tier { get; init; } // 1=Basic, 2=Intermediate, 3=Advanced
    public int Cost { get; init; }
    public int SellPrice { get; init; }
    
    // Base Stats
    public float BonusHp { get; init; }
    public float BonusMp { get; init; }
    public float BonusDamage { get; init; }
    public float BonusMagicPower { get; init; }
    public float BonusArmor { get; init; }
    public float BonusMagicResist { get; init; }
    public float BonusSpeed { get; init; }
    public float BonusAttackSpeed { get; init; }
    public float BonusCriticalChance { get; init; }
    public float BonusCriticalDamage { get; init; }
    public float BonusCooldownReduction { get; init; }
    public float BonusLifeSteal { get; init; }
    public float BonusSpellVamp { get; init; }
    public float BonusPhysicalPenetration { get; init; }
    public float BonusMagicPenetration { get; init; }
    public float BonusHPRegen { get; init; }
    public float BonusMPRegen { get; init; }
    public float BonusTenacity { get; init; }
    
    // Passive/Active
    public int ActiveAbilityId { get; init; }
    public string PassiveDescription { get; init; }
    public int[] Components { get; init; } // Build path
    
    // Tags
    public string[] Tags { get; init; }
}

public static class ItemRegistry
{
    public static readonly Dictionary<int, ItemData> Database = new()
    {
        // === PHYSICAL ATTACK - TIER 1 ===
        [1101] = new ItemData { Id = 1101, Name = "Iron Sword", Category = ItemCategory.PhysicalAttack, Tier = 1, Cost = 250, SellPrice = 150, BonusDamage = 8, Tags = new[] { "physical", "damage" } },
        [1102] = new ItemData { Id = 1102, Name = "Dagger", Category = ItemCategory.PhysicalAttack, Tier = 1, Cost = 280, SellPrice = 168, BonusAttackSpeed = 0.1f, Tags = new[] { "physical", "attackspeed" } },
        [1103] = new ItemData { Id = 1103, Name = "Boxing Gloves", Category = ItemCategory.PhysicalAttack, Tier = 1, Cost = 320, SellPrice = 192, BonusCriticalChance = 0.08f, Tags = new[] { "physical", "critical" } },
        [1104] = new ItemData { Id = 1104, Name = "Vampiric Sickle", Category = ItemCategory.PhysicalAttack, Tier = 1, Cost = 400, SellPrice = 240, BonusDamage = 4, BonusLifeSteal = 0.08f, Tags = new[] { "physical", "lifesteal" } },
        [1105] = new ItemData { Id = 1105, Name = "Thunder Blade", Category = ItemCategory.PhysicalAttack, Tier = 1, Cost = 450, SellPrice = 270, BonusDamage = 15, Tags = new[] { "physical", "damage" } },
        [1106] = new ItemData { Id = 1106, Name = "Charged Gauntlets", Category = ItemCategory.PhysicalAttack, Tier = 1, Cost = 600, SellPrice = 360, BonusCriticalChance = 0.15f, Tags = new[] { "physical", "critical" } },

        // === PHYSICAL ATTACK - TIER 2 ===
        [1201] = new ItemData { Id = 1201, Name = "Storm Greatsword", Category = ItemCategory.PhysicalAttack, Tier = 2, Cost = 900, SellPrice = 540, BonusDamage = 32, Components = new[] { 1101 }, Tags = new[] { "physical", "damage" } },
        [1202] = new ItemData { Id = 1202, Name = "Corona", Category = ItemCategory.PhysicalAttack, Tier = 2, Cost = 750, SellPrice = 450, BonusHp = 120, BonusDamage = 16, Components = new[] { 1101, 3101 }, PassiveDescription = "Unique passive", Tags = new[] { "physical", "hp", "damage" } },
        [1203] = new ItemData { Id = 1203, Name = "Berserker Blades", Category = ItemCategory.PhysicalAttack, Tier = 2, Cost = 850, SellPrice = 510, BonusAttackSpeed = 0.05f, BonusCriticalChance = 0.15f, BonusCriticalDamage = 0.1f, Components = new[] { 1102, 1103 }, Tags = new[] { "physical", "critical", "attackspeed" } },
        [1204] = new ItemData { Id = 1204, Name = "Meteor", Category = ItemCategory.PhysicalAttack, Tier = 2, Cost = 1100, SellPrice = 660, BonusDamage = 18, BonusCooldownReduction = 0.1f, Components = new[] { 1101, 1101 }, PassiveDescription = "Unique passive", Tags = new[] { "physical", "damage", "cdr" } },
        [1205] = new ItemData { Id = 1205, Name = "Rapid Fire Gun", Category = ItemCategory.PhysicalAttack, Tier = 2, Cost = 900, SellPrice = 540, BonusAttackSpeed = 0.25f, Components = new[] { 1102, 1102 }, PassiveDescription = "Unique passive", Tags = new[] { "physical", "attackspeed" } },
        [1206] = new ItemData { Id = 1206, Name = "Cloud Piercer", Category = ItemCategory.PhysicalAttack, Tier = 2, Cost = 1100, SellPrice = 660, BonusDamage = 16, BonusPhysicalPenetration = 0.1f, Components = new[] { 1105, 1102 }, PassiveDescription = "Unique passive", Tags = new[] { "physical", "penetration" } },

        // === PHYSICAL ATTACK - TIER 3 ===
        [1304] = new ItemData { Id = 1304, Name = "Star Breaker", Category = ItemCategory.PhysicalAttack, Tier = 3, Cost = 2100, SellPrice = 1260, BonusDamage = 32, BonusCooldownReduction = 0.1f, Components = new[] { 1204, 1101 }, PassiveDescription = "Unique passive", Tags = new[] { "physical", "damage", "cdr" } },
        [1305] = new ItemData { Id = 1305, Name = "Sanction Blade", Category = ItemCategory.PhysicalAttack, Tier = 3, Cost = 1800, SellPrice = 1080, BonusDamage = 40, BonusLifeSteal = 0.1f, Components = new[] { 1201, 1104 }, PassiveDescription = "Unique passive", Tags = new[] { "physical", "lifesteal" } },
        [1306] = new ItemData { Id = 1306, Name = "Doomsday", Category = ItemCategory.PhysicalAttack, Tier = 3, Cost = 2150, SellPrice = 1290, BonusDamage = 24, BonusAttackSpeed = 0.3f, BonusLifeSteal = 0.1f, Components = new[] { 1105, 1205, 1104 }, PassiveDescription = "Unique passive", Tags = new[] { "physical", "attackspeed", "lifesteal" } },
        [1307] = new ItemData { Id = 1307, Name = "Bloodblade", Category = ItemCategory.PhysicalAttack, Tier = 3, Cost = 1800, SellPrice = 1080, BonusDamage = 40, BonusLifeSteal = 0.25f, Components = new[] { 1104, 1201 }, Tags = new[] { "physical", "lifesteal" } },
        [1308] = new ItemData { Id = 1308, Name = "Infinity Edge", Category = ItemCategory.PhysicalAttack, Tier = 3, Cost = 2100, SellPrice = 1260, BonusDamage = 50, BonusCriticalChance = 0.25f, Components = new[] { 1201, 1101, 1106 }, PassiveDescription = "Unique passive", Tags = new[] { "physical", "critical" } },
        [1309] = new ItemData { Id = 1309, Name = "Might of the Master", Category = ItemCategory.PhysicalAttack, Tier = 3, Cost = 2100, SellPrice = 1260, BonusHp = 160, BonusMp = 400, BonusDamage = 25, BonusCooldownReduction = 0.2f, Components = new[] { 1105, 2203, 1106 }, PassiveDescription = "Unique passive", Tags = new[] { "physical", "hp", "cdr" } },
        [1310] = new ItemData { Id = 1310, Name = "Lightning Dagger", Category = ItemCategory.PhysicalAttack, Tier = 3, Cost = 2000, SellPrice = 1200, BonusCriticalChance = 0.08f, BonusCriticalDamage = 0.3f, Components = new[] { 1103, 1203 }, PassiveDescription = "Unique passive", Tags = new[] { "physical", "critical" } },
        [1312] = new ItemData { Id = 1312, Name = "Shadow Axe", Category = ItemCategory.PhysicalAttack, Tier = 3, Cost = 2200, SellPrice = 1320, BonusHp = 200, BonusDamage = 35, BonusCooldownReduction = 0.15f, Components = new[] { 1202, 1204 }, PassiveDescription = "Unique passive", Tags = new[] { "physical", "hp", "cdr" } },
        [1313] = new ItemData { Id = 1313, Name = "Army Breaker", Category = ItemCategory.PhysicalAttack, Tier = 3, Cost = 2900, SellPrice = 1740, BonusDamage = 70, Components = new[] { 1201, 1201 }, PassiveDescription = "Unique passive", Tags = new[] { "physical", "damage" } },
        [1315] = new ItemData { Id = 1315, Name = "Dawn Breaker", Category = ItemCategory.PhysicalAttack, Tier = 3, Cost = 3300, SellPrice = 1980, BonusDamage = 20, BonusAttackSpeed = 0.35f, BonusPhysicalPenetration = 0.15f, Components = new[] { 1206, 1205, 1106 }, PassiveDescription = "Unique passive", Tags = new[] { "physical", "penetration", "attackspeed" } },

        // === MAGIC ATTACK - TIER 1 ===
        [2101] = new ItemData { Id = 2101, Name = "Spell Tome", Category = ItemCategory.MagicAttack, Tier = 1, Cost = 280, SellPrice = 168, BonusMagicPower = 15, Tags = new[] { "magic", "damage" } },
        [2102] = new ItemData { Id = 2102, Name = "Blue Sapphire", Category = ItemCategory.MagicAttack, Tier = 1, Cost = 220, SellPrice = 132, BonusMp = 300, Tags = new[] { "magic", "mana" } },
        [2103] = new ItemData { Id = 2103, Name = "Alchemist Amulet", Category = ItemCategory.MagicAttack, Tier = 1, Cost = 120, SellPrice = 72, BonusMPRegen = 2, Tags = new[] { "magic", "mana" } },
        [2104] = new ItemData { Id = 2104, Name = "Sage Codex", Category = ItemCategory.MagicAttack, Tier = 1, Cost = 500, SellPrice = 300, BonusMagicPower = 10, BonusCooldownReduction = 0.08f, Tags = new[] { "magic", "cdr" } },
        [2105] = new ItemData { Id = 2105, Name = "Elemental Staff", Category = ItemCategory.MagicAttack, Tier = 1, Cost = 550, SellPrice = 330, BonusMagicPower = 30, Tags = new[] { "magic", "damage" } },

        // === MAGIC ATTACK - TIER 2 ===
        [2201] = new ItemData { Id = 2201, Name = "Big Rod", Category = ItemCategory.MagicAttack, Tier = 2, Cost = 800, SellPrice = 480, BonusMagicPower = 45, Components = new[] { 2101 }, Tags = new[] { "magic", "damage" } },
        [2202] = new ItemData { Id = 2202, Name = "Blood Grimoire", Category = ItemCategory.MagicAttack, Tier = 2, Cost = 1250, SellPrice = 750, BonusMagicPower = 30, BonusCooldownReduction = 0.1f, Components = new[] { 2101, 2104 }, PassiveDescription = "Unique passive", Tags = new[] { "magic", "cdr" } },
        [2203] = new ItemData { Id = 2203, Name = "Radiant Sword", Category = ItemCategory.MagicAttack, Tier = 2, Cost = 750, SellPrice = 450, BonusHp = 160, BonusMp = 400, Components = new[] { 3101, 2102 }, PassiveDescription = "Unique passive", Tags = new[] { "magic", "hp", "mana" } },
        [2204] = new ItemData { Id = 2204, Name = "Phantom Mask", Category = ItemCategory.MagicAttack, Tier = 2, Cost = 1000, SellPrice = 600, BonusHp = 120, BonusMagicPower = 28, Components = new[] { 2101, 3101 }, PassiveDescription = "Unique passive", Tags = new[] { "magic", "hp", "damage" } },
        [2205] = new ItemData { Id = 2205, Name = "Evolution Crystal", Category = ItemCategory.MagicAttack, Tier = 2, Cost = 750, SellPrice = 450, BonusHp = 160, BonusMp = 400, Components = new[] { 2102, 3101 }, PassiveDescription = "Unique passive", Tags = new[] { "magic", "hp", "mana" } },
        [2206] = new ItemData { Id = 2206, Name = "Broken Chalice", Category = ItemCategory.MagicAttack, Tier = 2, Cost = 900, SellPrice = 540, BonusMagicPower = 4, BonusMagicResist = 32, BonusCooldownReduction = 0.05f, Components = new[] { 2101, 2103 }, Tags = new[] { "magic", "resist", "cdr" } },

        // === MAGIC ATTACK - TIER 3 ===
        [2302] = new ItemData { Id = 2302, Name = "Nightmare Tooth", Category = ItemCategory.MagicAttack, Tier = 3, Cost = 2050, SellPrice = 1230, BonusMagicPower = 95, BonusAttackSpeed = 0.05f, Components = new[] { 2201, 2105 }, PassiveDescription = "Unique passive", Tags = new[] { "magic", "damage" } },
        [2303] = new ItemData { Id = 2303, Name = "Holy Grail", Category = ItemCategory.MagicAttack, Tier = 3, Cost = 1800, SellPrice = 1080, BonusMp = 500, BonusMagicPower = 72, BonusCooldownReduction = 0.15f, Components = new[] { 2104, 2206, 2101 }, PassiveDescription = "Unique passive", Tags = new[] { "magic", "cdr", "mana" } },
        [2304] = new ItemData { Id = 2304, Name = "Void Staff", Category = ItemCategory.MagicAttack, Tier = 3, Cost = 2100, SellPrice = 1260, BonusHp = 120, BonusMagicPower = 95, Components = new[] { 2204, 2105 }, PassiveDescription = "Unique passive", Tags = new[] { "magic", "damage" } },
        [2305] = new ItemData { Id = 2305, Name = "Scholar's Wrath", Category = ItemCategory.MagicAttack, Tier = 3, Cost = 2300, SellPrice = 1380, BonusMagicPower = 95, Components = new[] { 2201, 2101, 2101 }, PassiveDescription = "Unique passive", Tags = new[] { "magic", "damage" } },
        [2306] = new ItemData { Id = 2306, Name = "Echo Staff", Category = ItemCategory.MagicAttack, Tier = 3, Cost = 2100, SellPrice = 1260, BonusMagicPower = 95, BonusPhysicalPenetration = 0.07f, Components = new[] { 2201, 2101 }, PassiveDescription = "Unique passive", Tags = new[] { "magic", "penetration" } },
        [2307] = new ItemData { Id = 2307, Name = "Frost Staff", Category = ItemCategory.MagicAttack, Tier = 3, Cost = 2100, SellPrice = 1260, BonusHp = 420, BonusMagicPower = 60, Components = new[] { 2201, 3201 }, PassiveDescription = "Unique passive", Tags = new[] { "magic", "hp", "damage" } },
        [2308] = new ItemData { Id = 2308, Name = "Pain Mask", Category = ItemCategory.MagicAttack, Tier = 3, Cost = 2040, SellPrice = 1224, BonusHp = 200, BonusMagicPower = 55, BonusCooldownReduction = 0.05f, Components = new[] { 2204, 2101 }, PassiveDescription = "Unique passive", Tags = new[] { "magic", "hp", "cdr" } },
        [2309] = new ItemData { Id = 2309, Name = "Witch Staff", Category = ItemCategory.MagicAttack, Tier = 3, Cost = 2000, SellPrice = 1200, BonusHp = 160, BonusMp = 400, BonusMagicPower = 55, BonusAttackSpeed = 0.08f, Components = new[] { 2203, 2101 }, PassiveDescription = "Unique passive", Tags = new[] { "magic", "hp", "mana" } },
        [2311] = new ItemData { Id = 2311, Name = "Sage's Book", Category = ItemCategory.MagicAttack, Tier = 3, Cost = 2990, SellPrice = 1794, BonusMagicPower = 160, Components = new[] { 2201, 2201 }, PassiveDescription = "Unique passive", Tags = new[] { "magic", "damage" } },
        [2312] = new ItemData { Id = 2312, Name = "Radiant Moon", Category = ItemCategory.MagicAttack, Tier = 3, Cost = 2000, SellPrice = 1200, BonusMagicPower = 65, BonusCooldownReduction = 0.1f, Components = new[] { 2201, 2104 }, PassiveDescription = "Active: Become invulnerable", Tags = new[] { "magic", "cdr", "active" } },
        [2313] = new ItemData { Id = 2313, Name = "Devourer's Tome", Category = ItemCategory.MagicAttack, Tier = 3, Cost = 2100, SellPrice = 1260, BonusHp = 320, BonusMagicPower = 72, BonusCooldownReduction = 0.1f, Components = new[] { 2202, 2101, 3101 }, PassiveDescription = "Unique passive", Tags = new[] { "magic", "hp", "cdr" } },

        // === DEFENSE - TIER 1 ===
        [3101] = new ItemData { Id = 3101, Name = "Ruby", Category = ItemCategory.Defense, Tier = 1, Cost = 300, SellPrice = 180, BonusHp = 120, Tags = new[] { "defense", "hp" } },
        [3102] = new ItemData { Id = 3102, Name = "Cloth Armor", Category = ItemCategory.Defense, Tier = 1, Cost = 220, SellPrice = 132, BonusArmor = 15, Tags = new[] { "defense", "armor" } },
        [3103] = new ItemData { Id = 3103, Name = "Magic Cloak", Category = ItemCategory.Defense, Tier = 1, Cost = 220, SellPrice = 132, BonusMagicResist = 15, Tags = new[] { "defense", "mr" } },
        [3104] = new ItemData { Id = 3104, Name = "Rejuvenation Crystal", Category = ItemCategory.Defense, Tier = 1, Cost = 140, SellPrice = 84, BonusHPRegen = 2.4f, Tags = new[] { "defense", "regen" } },

        // === DEFENSE - TIER 2 ===
        [3201] = new ItemData { Id = 3201, Name = "Power Belt", Category = ItemCategory.Defense, Tier = 2, Cost = 900, SellPrice = 540, BonusHp = 400, Components = new[] { 3101, 3101 }, Tags = new[] { "defense", "hp" } },
        [3202] = new ItemData { Id = 3202, Name = "Molten Heart", Category = ItemCategory.Defense, Tier = 2, Cost = 900, SellPrice = 540, BonusHp = 280, Components = new[] { 3101 }, PassiveDescription = "Unique passive", Tags = new[] { "defense", "hp" } },
        [3203] = new ItemData { Id = 3203, Name = "Shadow Cloak", Category = ItemCategory.Defense, Tier = 2, Cost = 1050, SellPrice = 630, BonusHp = 280, BonusMPRegen = 4, BonusMagicResist = 20, Components = new[] { 3101, 3103 }, Tags = new[] { "defense", "hp", "mr" } },
        [3204] = new ItemData { Id = 3204, Name = "Snow Mountain Shield", Category = ItemCategory.Defense, Tier = 2, Cost = 900, SellPrice = 540, BonusMp = 400, BonusArmor = 18, BonusCooldownReduction = 0.1f, Components = new[] { 3102, 2102 }, Tags = new[] { "defense", "armor", "cdr" } },
        [3205] = new ItemData { Id = 3205, Name = "Guardian Armor", Category = ItemCategory.Defense, Tier = 2, Cost = 700, SellPrice = 420, BonusArmor = 35, Components = new[] { 3102, 3102 }, PassiveDescription = "Unique passive", Tags = new[] { "defense", "armor" } },

        // === DEFENSE - TIER 3 ===
        [3301] = new ItemData { Id = 3301, Name = "Thornmail", Category = ItemCategory.Defense, Tier = 3, Cost = 1850, SellPrice = 1110, BonusDamage = 16, BonusArmor = 70, Components = new[] { 1105, 3102, 3102 }, PassiveDescription = "Reflects damage", Tags = new[] { "defense", "armor", "damage" } },
        [3302] = new ItemData { Id = 3302, Name = "Blood Rage", Category = ItemCategory.Defense, Tier = 3, Cost = 2100, SellPrice = 1260, BonusHp = 400, BonusDamage = 8, Components = new[] { 3201, 1101 }, PassiveDescription = "Unique passive", Tags = new[] { "defense", "hp", "damage" } },
        [3303] = new ItemData { Id = 3303, Name = "Red Lotus Cape", Category = ItemCategory.Defense, Tier = 3, Cost = 1850, SellPrice = 1110, BonusHp = 400, BonusArmor = 40, Components = new[] { 3202, 3102 }, PassiveDescription = "Unique passive", Tags = new[] { "defense", "hp", "armor" } },
        [3304] = new ItemData { Id = 3304, Name = "Overlord's Platemail", Category = ItemCategory.Defense, Tier = 3, Cost = 2100, SellPrice = 1260, BonusHp = 600, BonusArmor = 40, Components = new[] { 3201, 3101, 3104 }, PassiveDescription = "Unique passive", Tags = new[] { "defense", "hp", "regen" } },
        [3305] = new ItemData { Id = 3305, Name = "Ominous Premonition", Category = ItemCategory.Defense, Tier = 3, Cost = 2200, SellPrice = 1320, BonusHp = 480, BonusArmor = 45, Components = new[] { 3201, 3205 }, PassiveDescription = "Unique passive", Tags = new[] { "defense", "hp", "armor" } },
        [3307] = new ItemData { Id = 3307, Name = "Witch's Cloak", Category = ItemCategory.Defense, Tier = 3, Cost = 2150, SellPrice = 1290, BonusHp = 400, BonusMagicResist = 60, Components = new[] { 3203, 3201 }, PassiveDescription = "Unique passive", Tags = new[] { "defense", "hp", "mr" } },
        [3308] = new ItemData { Id = 3308, Name = "Glacial Storm", Category = ItemCategory.Defense, Tier = 3, Cost = 2200, SellPrice = 1320, BonusMp = 500, BonusMagicResist = 60, BonusCooldownReduction = 0.2f, Components = new[] { 3204, 3205 }, PassiveDescription = "Unique passive", Tags = new[] { "defense", "mr", "cdr" } },

        // === MOVEMENT - TIER 1 ===
        [4101] = new ItemData { Id = 4101, Name = "Speed Boots", Category = ItemCategory.Movement, Tier = 1, Cost = 250, SellPrice = 150, BonusSpeed = 2f, PassiveDescription = "Movement speed increased", Tags = new[] { "movement" } },

        // === MOVEMENT - TIER 2 ===
        [4201] = new ItemData { Id = 4201, Name = "Ninja Tabi", Category = ItemCategory.Movement, Tier = 2, Cost = 720, SellPrice = 432, BonusSpeed = 2f, BonusArmor = 18, Components = new[] { 4101, 3102 }, PassiveDescription = "Reduces physical damage", Tags = new[] { "movement", "armor" } },
        [4202] = new ItemData { Id = 4202, Name = "Mercury's Treads", Category = ItemCategory.Movement, Tier = 2, Cost = 720, SellPrice = 432, BonusSpeed = 2f, BonusMagicResist = 18, Components = new[] { 4101, 3103 }, PassiveDescription = "Reduces crowd control", Tags = new[] { "movement", "mr" } },
        [4203] = new ItemData { Id = 4203, Name = "Boots of Lucidity", Category = ItemCategory.Movement, Tier = 2, Cost = 720, SellPrice = 432, BonusSpeed = 2f, BonusCooldownReduction = 0.15f, Components = new[] { 4101 }, PassiveDescription = "Reduces cooldowns", Tags = new[] { "movement", "cdr" } },
        [4204] = new ItemData { Id = 4204, Name = "Arcane Boots", Category = ItemCategory.Movement, Tier = 2, Cost = 720, SellPrice = 432, BonusSpeed = 2f, BonusMp = 500, Components = new[] { 4101, 2103 }, PassiveDescription = "Increases mana", Tags = new[] { "movement", "mana" } },
        [4205] = new ItemData { Id = 4205, Name = "Berserker's Greaves", Category = ItemCategory.Movement, Tier = 2, Cost = 720, SellPrice = 432, BonusSpeed = 2f, BonusAttackSpeed = 0.3f, Components = new[] { 4101, 1102 }, PassiveDescription = "Increases attack speed", Tags = new[] { "movement", "attackspeed" } },
        [4206] = new ItemData { Id = 4206, Name = "Boots of Swiftness", Category = ItemCategory.Movement, Tier = 2, Cost = 540, SellPrice = 324, BonusSpeed = 3f, Components = new[] { 4101 }, PassiveDescription = "Increases movement speed", Tags = new[] { "movement" } },

        // === JUNGLE - TIER 1 ===
        [5101] = new ItemData { Id = 5101, Name = "Hunter's Machete", Category = ItemCategory.Jungle, Tier = 1, Cost = 250, SellPrice = 150, PassiveDescription = "Increases damage to monsters", Tags = new[] { "jungle" } },

        // === JUNGLE - TIER 2 ===
        [5201] = new ItemData { Id = 5201, Name = "Ranger's Blade", Category = ItemCategory.Jungle, Tier = 2, Cost = 750, SellPrice = 450, Components = new[] { 5101 }, PassiveDescription = "Increases damage to monsters", Tags = new[] { "jungle" } },
        [5202] = new ItemData { Id = 5202, Name = "Tracker's Axe", Category = ItemCategory.Jungle, Tier = 2, Cost = 750, SellPrice = 450, Components = new[] { 5101 }, PassiveDescription = "Increases damage to monsters", Tags = new[] { "jungle" } },
        [5203] = new ItemData { Id = 5203, Name = "Assassin's Blade", Category = ItemCategory.Jungle, Tier = 2, Cost = 750, SellPrice = 450, Components = new[] { 5101 }, PassiveDescription = "Increases damage to monsters", Tags = new[] { "jungle" } },

        // === JUNGLE - TIER 3 ===
        [5301] = new ItemData { Id = 5301, Name = "Runeblade", Category = ItemCategory.Jungle, Tier = 3, Cost = 2150, SellPrice = 1290, BonusMp = 400, BonusMagicPower = 40, Components = new[] { 5201, 2201, 2102 }, PassiveDescription = "Increases damage to monsters", Tags = new[] { "jungle", "magic" } },
        [5302] = new ItemData { Id = 5302, Name = "Giant's Grip", Category = ItemCategory.Jungle, Tier = 3, Cost = 1800, SellPrice = 1080, BonusArmor = 20, BonusMagicResist = 20, Components = new[] { 5202, 3102, 3103 }, PassiveDescription = "Increases damage to monsters", Tags = new[] { "jungle", "defense" } },
        [5303] = new ItemData { Id = 5303, Name = "Greed's Fang", Category = ItemCategory.Jungle, Tier = 3, Cost = 2150, SellPrice = 1290, BonusDamage = 16, BonusAttackSpeed = 0.08f, BonusCriticalChance = 0.15f, Components = new[] { 5203, 1201, 1102 }, PassiveDescription = "Increases damage to monsters", Tags = new[] { "jungle", "physical" } },

        // === SUPPORT - TIER 1 ===
        [6102] = new ItemData { Id = 6102, Name = "Gem of Knowledge", Category = ItemCategory.Support, Tier = 1, Cost = 300, SellPrice = 180, BonusCooldownReduction = 0.05f, PassiveDescription = "Support gold generation", Tags = new[] { "support" } },

        // === SUPPORT - TIER 2 ===
        [6201] = new ItemData { Id = 6201, Name = "Phoenix Ring", Category = ItemCategory.Support, Tier = 2, Cost = 1050, SellPrice = 630, BonusHp = 200, BonusCooldownReduction = 0.05f, Components = new[] { 6102, 3101 }, PassiveDescription = "Support aura", Tags = new[] { "support", "hp" } },
        [6204] = new ItemData { Id = 6204, Name = "Inspiring Shield", Category = ItemCategory.Support, Tier = 2, Cost = 1200, SellPrice = 720, BonusHp = 200, BonusCooldownReduction = 0.05f, Components = new[] { 6102, 3101 }, PassiveDescription = "Support aura", Tags = new[] { "support", "hp" } },
        [6205] = new ItemData { Id = 6205, Name = "Wind Spirit", Category = ItemCategory.Support, Tier = 2, Cost = 1200, SellPrice = 720, BonusHp = 200, BonusCooldownReduction = 0.05f, Components = new[] { 6102, 3101 }, PassiveDescription = "Support active", Tags = new[] { "support", "hp", "active" } },

        // === SUPPORT - TIER 3 ===
        [6301] = new ItemData { Id = 6301, Name = "Shadow", Category = ItemCategory.Support, Tier = 3, Cost = 1900, SellPrice = 1140, BonusHp = 400, BonusCooldownReduction = 0.1f, Components = new[] { 6201, 3101 }, PassiveDescription = "Support aura", Tags = new[] { "support", "hp" } },
        [6304] = new ItemData { Id = 6304, Name = "Guardian's Glory", Category = ItemCategory.Support, Tier = 3, Cost = 2000, SellPrice = 1200, BonusHp = 400, BonusCooldownReduction = 0.1f, Components = new[] { 6204, 3101 }, PassiveDescription = "Support aura", Tags = new[] { "support", "hp" } },
        [6305] = new ItemData { Id = 6305, Name = "Wolf's Emblem", Category = ItemCategory.Support, Tier = 3, Cost = 2100, SellPrice = 1260, BonusHp = 400, BonusCooldownReduction = 0.1f, Components = new[] { 6205, 3101 }, PassiveDescription = "Support active", Tags = new[] { "support", "hp", "active" } },
    };
}
