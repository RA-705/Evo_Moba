# Evo MOBA - Unity Client

## Architecture

Unity is **presentation only**. All game logic lives in **Evo_Core** (.NET server).

### Structure

```
Assets/
├── Scripts/
│   ├── Network/      - UDP client, snapshot deserialization
│   ├── View/         - Rendering entities from snapshots
│   ├── Input/        - Capture and send player commands
│   ├── UI/           - HUD, shop, abilities
│   ├── Systems/      - Camera, etc
│   └── Data/         - ScriptableObjects for Heroes, Items, Abilities
├── Prefabs/          - All game object templates
├── Resources/        - Configuration data (Heroes, Items, Abilities)
└── Scenes/           - Main gameplay scene
```

## Key Components

### GameClient (Network/GameClient.cs)
- Connects to server via UDP
- Receives WorldSnapshots
- Sends player inputs
- Thread-safe snapshot queue

### WorldSnapshot (Network/WorldSnapshot.cs)
- Serialized state from server
- Contains all entities, game state
- Deserialized each frame

### WorldRenderer (View/WorldRenderer.cs)
- Updates entity views from snapshots
- Creates/destroys entities as needed
- Handles all rendering

### ShopUI (UI/ShopUI.cs)
- Displays available items
- Handles item purchases
- Updates item descriptions dynamically

### ScriptableObjects
- **HeroData**: Stats, abilities, progression
- **AbilityData**: Damage, scaling, cooldowns, effects
- **ItemData**: Stats, costs, recipes, active/passive effects
- **TowerData**: Health, damage, behavior

## Configuration

All game data is **configurable** without code changes:

1. Create Hero ScriptableObjects in `Resources/Heroes/`
2. Create Ability ScriptableObjects in `Resources/Abilities/`
3. Create Item ScriptableObjects in `Resources/Items/`
4. Assign them in prefabs or directly in game

## Connection

```csharp
GameClient.Instance.ConnectToServer("127.0.0.1", 27015);
```

## Important

- **Unity knows NOTHING about Evo_Core internals**
- **Evo_Core knows NOTHING about Unity**
- Communication: WorldSnapshot (server → client), Input (client → server)
- This is a **pure network-based architecture**
