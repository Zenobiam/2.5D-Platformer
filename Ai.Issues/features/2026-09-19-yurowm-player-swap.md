# Смена модели игрока на Yurowm

- **Status:** resolved
- **Date:** 2026-09-19

## Problem

Нужно заменить визуал игрока на Yurowm FreeHand (аватар + аниматор), сохранив наш `PlayerMovementController` / CC / Input.

## Cause

Штатный placeholder не совпадал с желаемым ассетом; нельзя было тащить чужой `PlayerController`/`Actions` как основной слой ввода.

## What we did

- Модель/контроллер в `Assets/Resources/Yurowm/`.
- Визуал через `PlayerVisual` (Speed 0/1 под FreeHand blend).
- Smoke-проверки: `AgentScripts/SmokeYurowmPlayer.cs`, `SmokeYurowmMove.cs` (Editor eval).
- Чужие Yurowm PC/Actions на префабе — детект в smoke, не как основной input.

## Files touched

- `Assets/Resources/Yurowm/**`
- `Assets/Resources/Prefabs/Player/Player.prefab`
- `Assets/Game/Scripts/Player/PlayerVisual.cs`
- `AgentScripts/SmokeYurowm*.cs`

## How to verify

1. Play Mode: на Player нет magenta materials, есть Animator с контроллером Yurowm.
2. Движение вправо/влево меняет Speed и клип Idle/Run.
3. CharacterController двигает root; модель — child с поворотом.
