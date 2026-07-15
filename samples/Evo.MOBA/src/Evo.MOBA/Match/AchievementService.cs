using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Evo.Core.ECS;
using Evo.MOBA.Items;

namespace Evo.MOBA.Match;

public enum AchievementRarity : byte
{
    Bronze,
    Silver,
    Gold,
    Platinum,
    Diamond,
}

public struct Achievement
{
    public int Id;
    public string Name;
    public string Description;
    public AchievementRarity Rarity;
    public CurrencyType RewardCurrency;
    public int RewardAmount;
    public Func<AchievementProgressComponent, bool> Condition;
}

public struct AchievementProgressComponent : IComponent
{
    public bool[] Unlocked;
    public int TotalKills;
    public int TotalDeaths;
    public int TotalAssists;
    public int TotalDamageDealt;
    public int TotalWins;
    public int TotalMatches;
    public int TotalCreepsKilled;
    public int TotalItemsBought;
    public int TotalAbilitiesUsed;
    public int MaxKillsInMatch;
    public int MaxDamageInMatch;
    public int WinStreak;
    public int MaxWinStreak;
}

public static class AchievementService
{
    public static readonly Achievement[] AllAchievements = new Achievement[]
    {
        new() { Id = 1, Name = "Primera Sangre", Description = "Consigue tu primer kill", Rarity = AchievementRarity.Bronze, RewardCurrency = CurrencyType.BattlePoints, RewardAmount = 100, Condition = s => s.TotalKills >= 1 },
        new() { Id = 2, Name = "Manco", Description = "Consigue 100 kills totales", Rarity = AchievementRarity.Silver, RewardCurrency = CurrencyType.BattlePoints, RewardAmount = 500, Condition = s => s.TotalKills >= 100 },
        new() { Id = 3, Name = "Asesino", Description = "Consigue 500 kills totales", Rarity = AchievementRarity.Gold, RewardCurrency = CurrencyType.Diamonds, RewardAmount = 100, Condition = s => s.TotalKills >= 500 },
        new() { Id = 4, Name = "Leyenda", Description = "Consigue 1000 kills totales", Rarity = AchievementRarity.Platinum, RewardCurrency = CurrencyType.Diamonds, RewardAmount = 300, Condition = s => s.TotalKills >= 1000 },

        new() { Id = 10, Name = "Veterano", Description = "Juega 10 partidas", Rarity = AchievementRarity.Bronze, RewardCurrency = CurrencyType.BattlePoints, RewardAmount = 200, Condition = s => s.TotalMatches >= 10 },
        new() { Id = 11, Name = "Guerrero", Description = "Juega 50 partidas", Rarity = AchievementRarity.Silver, RewardCurrency = CurrencyType.BattlePoints, RewardAmount = 800, Condition = s => s.TotalMatches >= 50 },
        new() { Id = 12, Name = "Maestro", Description = "Juega 200 partidas", Rarity = AchievementRarity.Gold, RewardCurrency = CurrencyType.Diamonds, RewardAmount = 200, Condition = s => s.TotalMatches >= 200 },

        new() { Id = 20, Name = "Victorioso", Description = "Gana tu primera partida", Rarity = AchievementRarity.Bronze, RewardCurrency = CurrencyType.BattlePoints, RewardAmount = 150, Condition = s => s.TotalWins >= 1 },
        new() { Id = 21, Name = "Invicto", Description = "Gana 10 partidas", Rarity = AchievementRarity.Silver, RewardCurrency = CurrencyType.BattlePoints, RewardAmount = 600, Condition = s => s.TotalWins >= 10 },
        new() { Id = 22, Name = "Imparable", Description = "Gana 50 partidas", Rarity = AchievementRarity.Gold, RewardCurrency = CurrencyType.Diamonds, RewardAmount = 150, Condition = s => s.TotalWins >= 50 },

        new() { Id = 30, Name = "Racha de Fuego", Description = "Consigue 5 victorias seguidas", Rarity = AchievementRarity.Silver, RewardCurrency = CurrencyType.Fragments, RewardAmount = 50, Condition = s => s.MaxWinStreak >= 5 },
        new() { Id = 31, Name = "Dios de la Guerra", Description = "Consigue 10 victorias seguidas", Rarity = AchievementRarity.Gold, RewardCurrency = CurrencyType.Diamonds, RewardAmount = 100, Condition = s => s.MaxWinStreak >= 10 },

        new() { Id = 40, Name = "Tirador", Description = "Consigue 15 kills en una partida", Rarity = AchievementRarity.Silver, RewardCurrency = CurrencyType.Fragments, RewardAmount = 30, Condition = s => s.MaxKillsInMatch >= 15 },
        new() { Id = 41, Name = "Destractor", Description = "Inflige 50000 daño total", Rarity = AchievementRarity.Gold, RewardCurrency = CurrencyType.Diamonds, RewardAmount = 200, Condition = s => s.TotalDamageDealt >= 50000 },

        new() { Id = 50, Name = "Granjero", Description = "Elimina 500 creeps", Rarity = AchievementRarity.Bronze, RewardCurrency = CurrencyType.BattlePoints, RewardAmount = 300, Condition = s => s.TotalCreepsKilled >= 500 },
        new() { Id = 51, Name = "Comprador", Description = "Compra 100 items", Rarity = AchievementRarity.Bronze, RewardCurrency = CurrencyType.Tickets, RewardAmount = 20, Condition = s => s.TotalItemsBought >= 100 },

        new() { Id = 60, Name = "Penta Kill", Description = "Consigue 5 kills en 10 segundos", Rarity = AchievementRarity.Platinum, RewardCurrency = CurrencyType.Diamonds, RewardAmount = 500, Condition = s => s.MaxKillsInMatch >= 5 },
    };

    public static void CheckAchievements(World world, EntityId playerId)
    {
        if (!world.TryGetComponent<AchievementProgressComponent>(playerId, out var progress))
            return;

        for (int i = 0; i < AllAchievements.Length; i++)
        {
            int idx = AllAchievements[i].Id - 1;
            if (idx < 0 || idx >= progress.Unlocked.Length) continue;
            if (progress.Unlocked[idx]) continue;

            if (AllAchievements[i].Condition(progress))
            {
                progress.Unlocked[idx] = true;
                GrantReward(world, playerId, AllAchievements[i]);
            }
        }

        world.SetComponent(playerId, progress);
    }

    private static void GrantReward(World world, EntityId playerId, Achievement achievement)
    {
        if (world.TryGetComponent<CurrencyComponent>(playerId, out var currency))
        {
            currency.Add(achievement.RewardCurrency, achievement.RewardAmount);
            world.SetComponent(playerId, currency);
        }
    }
}
