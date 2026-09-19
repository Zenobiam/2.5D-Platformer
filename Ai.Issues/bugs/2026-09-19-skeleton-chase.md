# Skeleton не преследует игрока

- **Status:** resolved
- **Date:** 2026-09-19

## Problem

Скелет на сцене не шёл за игроком (стоит / нет path / destination не обновляется).

## Cause

Типичный набор из курса + демо-уровня:

- нет ссылки на игрока до `PlayerCreated` (factory);
- агент не на NavMesh;
- follow-компонент выключен или не получил `Agent`;
- root motion аниматора конкурировал с NavMesh (лечится пустым `OnAnimatorMove`).

## What we did

- `AgentMoveToPlayer`: ждёт `IGameFactory.PlayerCreated`, пишет `Agent.destination`, `EnsureOnNavMesh`.
- `AnimateAlongAgent` синхронизирует Speed аниматора с velocity агента.
- Проверки Play Mode через eval (`VerifyChase`, `DiagnoseSkeleton` в `.tmp-course-enemy`).

## Files touched

- `Assets/Game/Scripts/Enemy/AgentMoveToPlayer.cs`
- `Assets/Game/Scripts/Enemy/AnimateAlongAgent.cs`
- `Assets/Game/Scripts/Enemy/Follow.cs`
- префаб/сцена Skeleton (WIP, может быть dirty)

## How to verify

1. Play Mode, игрок на NavMesh.
2. У Skeleton follow включён: `hasPath`, `velocity` ненулевые, позиция сближается с игроком.
3. Аниматор Speed реагирует на движение агента.
