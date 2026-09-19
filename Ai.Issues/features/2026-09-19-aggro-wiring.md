# Aggro: проводка как в курсе

- **Status:** open / in progress
- **Date:** 2026-09-19

## Problem

Нужно подключить aggro врага как в CodeBase урока 05: зона-триггер включает/выключает follow к игроку.

## Cause

Компоненты есть или появляются (`Aggro`, `TriggerObserver`, `AgentMoveToPlayer`), но параллельный агент («Wire Aggro like course») ещё ведёт работу — нельзя конфликтовать по Enemy/Aggro.

## What we did

- Каркас по курсу: `TriggerObserver` → события Enter/Exit; `Aggro` включает/выключает `Follow` (`AgentMoveToPlayer` или `RotateToHero`).
- Поле Aggro — базовый `Follow`; cooldown с фиксом re-enter → см. [`../bugs/2026-09-19-aggro-cooldown.md`](../bugs/2026-09-19-aggro-cooldown.md).
- **Осталось:** проводка на префабе/сцене (Collider isTrigger + ссылки в Inspector), если ещё не сделана.

## Files touched

- `Assets/Game/Scripts/Enemy/Aggro.cs`
- `Assets/Game/Scripts/Enemy/Follow.cs`
- `Assets/Game/Scripts/Enemy/RotateToHero.cs`
- `Assets/Game/Scripts/Enemy/TriggerObserver.cs`
- `Assets/Game/Scripts/Enemy/AgentMoveToPlayer.cs`
- префаб врага (Collider isTrigger + ссылки в Inspector)

## How to verify

1. Игрок входит в aggro-зону → follow enabled, агент идёт к игроку.
2. Выход из зоны → follow disabled, движение останавливается.
3. Сверка с курсом: `...\05 Enemy\...\CodeBase` (`Aggro`, `TriggerObserver`).
