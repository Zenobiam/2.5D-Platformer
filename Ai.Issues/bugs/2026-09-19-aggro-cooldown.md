# Aggro cooldown — re-enter во время кулдауна

- **Status:** resolved
- **Date:** 2026-09-19
- **Type:** bug

## Problem

После кулдауна враг **не возобновляет** преследование (`AgentMoveToPlayer` / Follow), даже если игрок снова в зоне aggro (или остался в ней после краткого Exit→Enter во время кулдауна).

## Cause

В `Aggro.cs` флаг `_hasAggroTarget`:

1. Ставился в `true` только в `TriggerEnter`.
2. В варианте «как в курсе» early-return на Enter при уже `true` **не стопал** корутину кулдауна.
3. Типичный сценарий: Exit → кулдаун → быстрый Enter (no-op) → `SwitchFollowOff` по таймеру → игрок уже в зоне, повторный Enter не приходит → Follow навсегда off.

## What we did

Файл: `Assets/Game/Scripts/Enemy/Aggro.cs`

- `_hasAggroTarget` = **игрок сейчас в aggro-зоне** (`true` на Enter, `false` на Exit).
- Enter: всегда `StopAggroCoroutine` + `SwitchFollowOn`.
- Exit: guard + сброс флага + старт кулдауна.
- После кулдауна: если `_hasAggroTarget` снова `true` → On, иначе → Off.
- Поле Follow — базовый тип `Follow` (можно chase или `RotateToHero`).

## How to verify

1. Войти в зону → Follow on.
2. Выйти, дождаться Cooldown → Follow off; снова войти → Follow on.
3. Выйти и сразу вернуться до конца Cooldown → Follow не гаснет.
