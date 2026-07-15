using System;
using System.Collections.Generic;
using Evo.Core.ECS;
using Evo.MOBA.Items;

namespace Evo.MOBA.Match;

public enum ChallengeType : byte
{
    WinMatches,
    GetKills,
    DealDamage,
    PlayHeroes,
    BuyItems,
    UseAbilities,
    GetAssists,
    FarmCreeps,
}

public struct DailyChallenge
{
    public int Id;
    public ChallengeType Type;
    public int TargetAmount;
    public int CurrentAmount;
    public CurrencyType RewardCurrency;
    public int RewardAmount;
    public string Description;
    public bool Completed;
    public bool Claimed;
}

public struct DailyChallengesComponent : IComponent
{
    public DailyChallenge[] Challenges;
    public DateTime LastReset;
}

public static class DailyChallengeService
{
    private static readonly Random _rng = new();

    private static readonly (ChallengeType type, int minTarget, int maxTarget, CurrencyType currency, int minReward, int maxReward)[] Templates =
    {
        (ChallengeType.WinMatches, 1, 3, CurrencyType.BattlePoints, 200, 500),
        (ChallengeType.GetKills, 5, 15, CurrencyType.BattlePoints, 150, 400),
        (ChallengeType.DealDamage, 5000, 20000, CurrencyType.Fragments, 10, 30),
        (ChallengeType.PlayHeroes, 2, 5, CurrencyType.BattlePoints, 300, 600),
        (ChallengeType.BuyItems, 3, 8, CurrencyType.Tickets, 5, 15),
        (ChallengeType.UseAbilities, 20, 50, CurrencyType.BattlePoints, 100, 300),
        (ChallengeType.GetAssists, 5, 15, CurrencyType.Fragments, 15, 40),
        (ChallengeType.FarmCreeps, 20, 50, CurrencyType.BattlePoints, 200, 500),
    };

    public static DailyChallenge[] GenerateDailyChallenges()
    {
        var challenges = new DailyChallenge[4];
        var usedTypes = new HashSet<ChallengeType>();

        for (int i = 0; i < 4; i++)
        {
            ChallengeType type;
            do
            {
                type = Templates[_rng.Next(Templates.Length)].type;
            } while (usedTypes.Contains(type));
            usedTypes.Add(type);

            var template = Templates[Array.FindIndex(Templates, t => t.type == type)];

            challenges[i] = new DailyChallenge
            {
                Id = i + 1,
                Type = type,
                TargetAmount = _rng.Next(template.minTarget, template.maxTarget + 1),
                CurrentAmount = 0,
                RewardCurrency = template.currency,
                RewardAmount = _rng.Next(template.minReward, template.maxReward + 1),
                Description = GetDescription(type),
                Completed = false,
                Claimed = false,
            };
        }

        return challenges;
    }

    public static void TrackEvent(World world, EntityId playerId, ChallengeType type, int amount)
    {
        if (!world.TryGetComponent<DailyChallengesComponent>(playerId, out var challenges))
            return;

        for (int i = 0; i < challenges.Challenges.Length; i++)
        {
            if (challenges.Challenges[i].Type == type && !challenges.Challenges[i].Completed)
            {
                challenges.Challenges[i].CurrentAmount += amount;
                if (challenges.Challenges[i].CurrentAmount >= challenges.Challenges[i].TargetAmount)
                {
                    challenges.Challenges[i].Completed = true;
                }
            }
        }

        world.SetComponent(playerId, challenges);
    }

    public static bool ClaimChallenge(World world, EntityId playerId, int challengeId)
    {
        if (!world.TryGetComponent<DailyChallengesComponent>(playerId, out var challenges))
            return false;

        for (int i = 0; i < challenges.Challenges.Length; i++)
        {
            if (challenges.Challenges[i].Id == challengeId &&
                challenges.Challenges[i].Completed &&
                !challenges.Challenges[i].Claimed)
            {
                challenges.Challenges[i].Claimed = true;
                world.SetComponent(playerId, challenges);

                if (world.TryGetComponent<CurrencyComponent>(playerId, out var currency))
                {
                    currency.Add(challenges.Challenges[i].RewardCurrency, challenges.Challenges[i].RewardAmount);
                    world.SetComponent(playerId, currency);
                }

                return true;
            }
        }

        return false;
    }

    private static string GetDescription(ChallengeType type) => type switch
    {
        ChallengeType.WinMatches => "Gana partidas",
        ChallengeType.GetKills => "Consigue kills",
        ChallengeType.DealDamage => "Inflige daño total",
        ChallengeType.PlayHeroes => "Juega con diferentes héroes",
        ChallengeType.BuyItems => "Compra items en la tienda",
        ChallengeType.UseAbilities => "Usa habilidades",
        ChallengeType.GetAssists => "Consigue asistencias",
        ChallengeType.FarmCreeps => "Elimina creeps",
        _ => "Completá desafíos",
    };
}
