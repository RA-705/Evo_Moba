using System;
using System.Collections.Generic;
using Evo.Core.ECS;
using Evo.MOBA.Combat;
using Evo.MOBA.Heroes;
using Evo.MOBA.Systems;

namespace Evo.MOBA.Match;

public struct SynergyBonus
{
    public float DamageBonus;
    public float ArmorBonus;
    public float MagicResistBonus;
    public float MoveSpeedBonus;
    public float HpRegenBonus;
    public string Description;
}

public static class HeroSynergyService
{
    private static readonly Dictionary<string, SynergyBonus> Synergies = new()
    {
        ["tank_melee"] = new()
        {
            DamageBonus = 0.05f,
            ArmorBonus = 10f,
            Description = "Bastión: +5% daño, +10 armadura",
        },
        ["mage_ranged"] = new()
        {
            DamageBonus = 0.08f,
            MagicResistBonus = 8f,
            Description = "Arcana: +8% daño mágico, +8 resistencia mágica",
        },
        ["assassin_melee"] = new()
        {
            DamageBonus = 0.10f,
            MoveSpeedBonus = 0.5f,
            Description = "Filorga: +10% daño, +0.5 velocidad",
        },
        ["all_same_team"] = new()
        {
            DamageBonus = 0.03f,
            ArmorBonus = 5f,
            MagicResistBonus = 5f,
            HpRegenBonus = 2f,
            Description = "Unidad: +3% daño, +5 armadura, +5 MR, +2 regen",
        },
        ["tank_mage"] = new()
        {
            ArmorBonus = 8f,
            MagicResistBonus = 8f,
            Description = "Equilibrio: +8 armadura, +8 resistencia mágica",
        },
        ["tank_assassin"] = new()
        {
            DamageBonus = 0.05f,
            ArmorBonus = 12f,
            Description = "Vanguardia: +5% daño, +12 armadura",
        },
        ["mage_assassin"] = new()
        {
            DamageBonus = 0.07f,
            MoveSpeedBonus = 0.3f,
            Description = "Omnis: +7% daño, +0.3 velocidad",
        },
    };

    public static SynergyBonus CalculateTeamSynergy(World world, int teamId)
    {
        var heroTags = new List<string>();

        foreach (var id in world.GetEntityIds<HealthComponent>())
        {
            if (!world.TryGetComponent<TeamComponent>(id, out var team) || team.TeamId != teamId)
                continue;

            if (id.Value >= 1000) continue;

            if (HeroRegistry.Database.TryGetValue(id.Value, out var hero))
            {
                foreach (var tag in hero.Tags)
                    heroTags.Add(tag);
            }
        }

        var total = new SynergyBonus();
        bool allSame = heroTags.Count > 1 && heroTags.TrueForAll(t => t == heroTags[0]);

        if (allSame)
            ApplyBonus(ref total, Synergies["all_same_team"]);

        for (int i = 0; i < heroTags.Count; i++)
        {
            for (int j = i + 1; j < heroTags.Count; j++)
            {
                string key = string.Compare(heroTags[i], heroTags[j]) < 0
                    ? $"{heroTags[i]}_{heroTags[j]}"
                    : $"{heroTags[j]}_{heroTags[i]}";

                if (Synergies.TryGetValue(key, out var bonus))
                    ApplyBonus(ref total, bonus);
            }
        }

        return total;
    }

    private static void ApplyBonus(ref SynergyBonus total, SynergyBonus bonus)
    {
        total.DamageBonus += bonus.DamageBonus;
        total.ArmorBonus += bonus.ArmorBonus;
        total.MagicResistBonus += bonus.MagicResistBonus;
        total.MoveSpeedBonus += bonus.MoveSpeedBonus;
        total.HpRegenBonus += bonus.HpRegenBonus;
    }
}

public sealed class SynergyApplySystem : ISystem
{
    public void OnTick(World world, float deltaTime)
    {
        foreach (var id in world.GetEntityIds<HealthComponent>())
        {
            if (!world.TryGetComponent<TeamComponent>(id, out var team))
                continue;

            if (id.Value >= 1000) continue;

            var synergy = HeroSynergyService.CalculateTeamSynergy(world, team.TeamId);

            if (world.TryGetComponent<ArmorComponent>(id, out var armor))
            {
                armor.BonusArmor = synergy.ArmorBonus;
                world.SetComponent(id, armor);
            }

            if (world.TryGetComponent<MagicResistComponent>(id, out var mr))
            {
                mr.BonusResist = synergy.MagicResistBonus;
                world.SetComponent(id, mr);
            }

            if (world.TryGetComponent<AttackComponent>(id, out var attack))
            {
                if (!HeroRegistry.Database.TryGetValue(id.Value, out var hero)) continue;
                attack.Damage = hero.BaseDamage * (1f + synergy.DamageBonus);
                world.SetComponent(id, attack);
            }
        }
    }
}
