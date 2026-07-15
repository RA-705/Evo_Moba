# Configuration Files

All game configuration is stored in JSON format for easy modification without code changes.

## Items (items.json)

Whoever edits this file can:
- Change item costs
- Modify stat bonuses
- Add new passive/active effects
- Create item recipes (buildsFrom/buildsInto)

## Abilities (abilities.json)

Add new abilities or modify existing ones:
- Change damage numbers
- Adjust cooldowns and mana costs
- Modify scaling (AD/AP ratios)
- Add CC effects (stun, slow, knockback)

## Heroes (heroes.json)

Define heroes with:
- Base stats
- Stat progression per level
- Assigned abilities (Q, W, E, R)

## How to Add New Content

### Adding a Hero
1. Add entry to heroes.json
2. Reference ability IDs from abilities.json
3. Client automatically loads from JSON

### Adding an Item
1. Add entry to items.json
2. Reference buildsFrom/buildsInto for recipes
3. Shop automatically shows new item

### Adding an Ability
1. Add entry to abilities.json
2. Reference in hero data
3. System automatically calculates damage scaling

## No Code Changes Needed!

Everything is data-driven. Designers can modify JSON files and see changes in-game immediately.
