using System;
using System.Collections.Generic;
using Evo.Core.ECS;
using Evo.MOBA.Combat;
using Evo.MOBA.Systems;

namespace Evo.MOBA.AI.MATE;

public enum ChatEvent : byte
{
    Kill,
    Death,
    Assist,
    DoubleKill,
    TripleKill,
    AllyDeath,
    EnemyLowHp,
    TeamfightStart,
    TowerDown,
    NexusDown,
    Farm,
    PushLane,
    Retreat,
    EnemyMissing,
    GameStart,
    GameEnd,
    SomeoneFeeding,
    NicePlay,
    BadPlay,
    Objective,
}

public struct BotChatComponent : IComponent
{
    public float NextChatTime;
    public float ChatCooldown;
    public int Personality;
    public string LastMessage;
}

public struct ChatMessage
{
    public int SenderId;
    public int SenderTeam;
    public string Text;
    public float Timestamp;
    public float Delay;
}

public struct ChatQueueComponent : IComponent
{
    public Queue<ChatMessage> PendingMessages;
}

public static class BotChatService
{
    private static readonly Random _rng = new();

    private static readonly Dictionary<ChatEvent, string[]> Messages = new()
    {
        [ChatEvent.Kill] = new[]
        {
            "jajaja re noob", "ez", "get rekt", "xd", "bye bye", "adios",
            "too easy", "manco", "jaja", "rekt", "gg", "nipotenete",
            "jajaja xq caminas solo", "te cogi", "jaja te agarre",
            "que pasooo", "te dormiste", "rapido y furioso", "na na na na",
        },
        [ChatEvent.Death] = new[]
        {
            "xq me focusearon", "solo uno por favor", "laggg", "bad luck",
            "xq hay 3 ahi", "me focusearon todo", "saben donde estoy o que",
            "xq me persiguen", "no era mi hora", "ya vuelvo", "buff please",
            "xq yo siempre", "re contra foco", "me dejaron solo",
        },
        [ChatEvent.Assist] = new[]
        {
            "nice", "bien ahi", "good", "wp", "gracias", "te ayude",
            "teamwork", "eso", "bueno",
        },
        [ChatEvent.DoubleKill] = new[]
        {
            "doble", "2 de uno", "jaja doble", "double",
            "dos por el precio de uno", "rekt x2",
        },
        [ChatEvent.TripleKill] = new[]
        {
            "TRIPLE", "3 de uno", "jajaja triple", "triple mat",
            "rekt x3", "me los como a todos",
        },
        [ChatEvent.AllyDeath] = new[]
        {
            "press f", "f", "rip", "murio", "que le paso", "noob",
            "feed", "int", "xq se murio", "mal ahi",
        },
        [ChatEvent.EnemyLowHp] = new[]
        {
            "low", "bajo", "hp bajo", "matalo", "uno mas", "low hp",
            "casi", "sale kill", "falta poco",
        },
        [ChatEvent.TeamfightStart] = new[]
        {
            "vamos", "group", "pelea", "teamfight", "5v5", "a pelear",
            "entren", "vamos todos", "no se rajen",
        },
        [ChatEvent.TowerDown] = new[]
        {
            "torre muerta", "tower down", "good", "nice push",
            "adentro", "siguiente", "push",
        },
        [ChatEvent.NexusDown] = new[]
        {
            "gg", "gg wp", "ganamos", "victoria", "se acabo",
            "buena partida", "ez",
        },
        [ChatEvent.Farm] = new[]
        {
            "farm", "oro", "gold", "creeps", "necesito items",
            "una waves mas", "cs",
        },
        [ChatEvent.PushLane] = new[]
        {
            "push", "lane", "push mid", "top", "bot", "mid",
            "push lane", "avanzar",
        },
        [ChatEvent.Retreat] = new[]
        {
            "back", "salgan", "retreat", "venganse", "no entren",
            "cuidado", "muchos ahi", "nos vamos",
        },
        [ChatEvent.EnemyMissing] = new[]
        {
            "mia", "missing", "no lo veo", "donde esta", "careful",
            "cuidado", "se fue", "no aparece",
        },
        [ChatEvent.GameStart] = new[]
        {
            "gl hf", "suerte", "a jugar", "vamos", "buena suerte",
            "hf", "good luck", "a ganar",
        },
        [ChatEvent.GameEnd] = new[]
        {
            "gg", "gg wp", "buena partida", "nos vemos",
            "hasta la proxima", "bye", "chau", "wp",
        },
        [ChatEvent.SomeoneFeeding] = new[]
        {
            "feed detected", "int detected", "report", "xq feedea",
            "deja de morir", "no te metas solo", "que haces",
        },
        [ChatEvent.NicePlay] = new[]
        {
            "nice", "bien", "wp", "good", "eso", "buen play",
            "juegazo", "crack", "te pasaste",
        },
        [ChatEvent.BadPlay] = new[]
        {
            "noob", "bad", "xq hiciste eso", "para que", "malo",
            "que fue eso", "xd", "jaja no",
        },
        [ChatEvent.Objective] = new[]
        {
            "objetivo", "tower", "torre", "nexus", "a la torre",
            "al nexus", "importante",
        },
    };

    public static string GetChatMessage(ChatEvent chatEvent, int personality)
    {
        if (!Messages.TryGetValue(chatEvent, out var options))
            return "";

        string msg = options[_rng.Next(options.Length)];

        if (personality % 3 == 0)
            msg = msg.ToUpper();
        else if (personality % 5 == 0)
            msg = msg + " xd";

        return msg;
    }

    public static ChatEvent DetectEvent(World world, EntityId botId, float deltaTime)
    {
        if (!world.TryGetComponent<HealthComponent>(botId, out var health))
            return 0;

        if (health.Current <= 0f)
            return ChatEvent.Death;

        if (world.TryGetComponent<AttackComponent>(botId, out var attack) &&
            attack.CurrentTargetId != Entity.Null.Id)
        {
            if (world.TryGetComponent<HealthComponent>(attack.CurrentTargetId, out var targetHp))
            {
                if (targetHp.Current > 0f && targetHp.Current / targetHp.Max < 0.2f)
                    return ChatEvent.EnemyLowHp;
            }
        }

        return 0;
    }
}
