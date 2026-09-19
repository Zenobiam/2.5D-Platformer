# EnemyAnimator: namespace и reader

- **Status:** resolved
- **Date:** 2026-09-19

## Problem

`EnemyAnimator` не компилировался / не стыковался с animation-readers курса (неверный namespace, нет `IAnimationStateReader`, путаница с root motion).

## Cause

Черновик врага не следовал CodeBase урока 05: нужен `Game.Scripts.Enemy`, хэши параметров, `GetComponent<Animator>` в `Awake`, пустой `OnAnimatorMove`, события Entered/Exited через `AnimatorStateReporter`.

## What we did

- Привели `EnemyAnimator` к стилю курса + project csharp-style.
- Подключили `IAnimationStateReader` / `AnimatorState` из `Game.Scripts.Animation`.
- Пустой `OnAnimatorMove` — движение даёт NavMeshAgent, не клипы.

## Files touched

- `Assets/Game/Scripts/Enemy/EnemyAnimator.cs`
- `Assets/Game/Scripts/Animation/**` (`AnimatorStateReporter`, interfaces)

## How to verify

1. Console без ошибок по `EnemyAnimator`.
2. На префабе врага: Animator + `EnemyAnimator` + StateReporter на слоях/стейтах.
3. `Move`/`StopMoving` меняют float `Speed`; атаки — triggers.
