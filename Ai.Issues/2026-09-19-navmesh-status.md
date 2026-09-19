# NavMesh: статус поверхности и агентов

- **Status:** resolved (инспекция + рабочий bake)
- **Date:** 2026-09-19

## Problem

Нужно было понять, есть ли в SampleScene валидный NavMesh и могут ли враги (`NavMeshAgent`) стоять на нём — без этого chase/aggro бессмысленны.

## Cause

Демо-геометрия / `NavMeshSurface` могли быть не собраны; агент вне меша (`isOnNavMesh == false`) игнорирует destination.

## What we did

- Инспекция через Unity CLI/eval: `NavMeshSurface.BuildNavMesh()`, `SamplePosition`, triangulation bounds.
- В `AgentMoveToPlayer` — `EnsureOnNavMesh()` + `Agent.Warp` к ближайшей точке.
- Диагностические скрипты в `.tmp-course-enemy/` (`ProbeNav`, и т.д.) — временные, не часть продакшн-кода.

## Files touched

- сцена / surface bake (SampleScene / demo level)
- `Assets/Game/Scripts/Enemy/AgentMoveToPlayer.cs` (`EnsureOnNavMesh`)
- `.tmp-course-enemy/ProbeNav.cs` (диагностика)

## How to verify

1. Window → AI → Navigation: видна поверхность на демо-маршруте.
2. В Play Mode у Skeleton: `agent.isOnNavMesh == true`.
3. `NavMesh.SamplePosition` около спавна игрока/врага успешен.
