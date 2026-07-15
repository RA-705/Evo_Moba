using System;
using System.Collections.Generic;
using Evo.Core.ECS;
using Evo.MOBA.Combat;

namespace Evo.MOBA.AI.MATE;

public sealed class BotChatSystem : ISystem
{
    private readonly Random _rng = new();
    private float _gameTime;

    private const float ChatCooldownMin = 3f;
    private const float ChatCooldownMax = 15f;
    private const float ArtificialDelayMin = 1f;
    private const float ArtificialDelayMax = 3f;

    public void OnTick(World world, float deltaTime)
    {
        _gameTime += deltaTime;

        foreach (var id in world.GetEntityIds<BotChatComponent>())
        {
            if (!world.TryGetComponent<BotChatComponent>(id, out var chat))
                continue;

            if (_gameTime < chat.NextChatTime)
                continue;

            var detectedEvent = BotChatService.DetectEvent(world, id, deltaTime);

            if (detectedEvent == 0)
                continue;

            string message = BotChatService.GetChatMessage(detectedEvent, chat.Personality);

            if (string.IsNullOrEmpty(message))
                continue;

            var chatMsg = new ChatMessage
            {
                SenderId = id.Value,
                SenderTeam = world.TryGetComponent<TeamComponent>(id, out var team) ? team.TeamId : 0,
                Text = message,
                Timestamp = _gameTime,
                Delay = ArtificialDelayMin + (float)_rng.NextDouble() * (ArtificialDelayMax - ArtificialDelayMin),
            };

            EnqueueToAll(world, chatMsg);

            chat.NextChatTime = _gameTime + ChatCooldownMin + (float)_rng.NextDouble() * (ChatCooldownMax - ChatCooldownMin);
            chat.LastMessage = message;
            world.SetComponent(id, chat);
        }
    }

    private void EnqueueToAll(World world, ChatMessage msg)
    {
        foreach (var id in world.GetEntityIds<ChatQueueComponent>())
        {
            if (!world.TryGetComponent<ChatQueueComponent>(id, out var queue))
                continue;

            if (queue.PendingMessages == null)
                queue.PendingMessages = new Queue<ChatMessage>();

            queue.PendingMessages.Enqueue(msg);
            world.SetComponent(id, queue);
        }
    }
}
