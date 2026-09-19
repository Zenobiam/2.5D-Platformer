# Yurowm: направление взгляда (2.5D)

- **Status:** resolved
- **Date:** 2026-09-19

## Problem

Модель Yurowm смотрела «не туда» при движении влево/вправо (камера с −Z, движение по X).

## Cause

Для 2.5D facing — yaw ±90° вокруг Y (вдоль ±X), а не ±Z. Плюс нужен `defaultFacingRight` и пустой `OnAnimatorMove`, чтобы root motion клипов не крутил CC.

## What we did

- В `PlayerVisual`: `_targetRotation` 90 / −90 по знаку `moveDirection.x`.
- `defaultFacingRight` + старт в `Start`.
- `OnAnimatorMove` пустой.

## Files touched

- `Assets/Game/Scripts/Player/PlayerVisual.cs`
- ссылки `modelTransform` / `defaultFacingRight` на префабе

## How to verify

1. Play Mode: бег вправо — модель roughly Y=90; влево Y=−90 (или эквивалент после lerp).
2. Smoke `DriveRight` / `TickAnim` → `facingY` соответствует направлению.
3. Root не уезжает от root motion.
