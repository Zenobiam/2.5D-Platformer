# Ошибки консоли Unity (компиляция / Pipeline)

- **Status:** resolved
- **Date:** 2026-09-19

## Problem

В Console / `Editor.log` сыпались ошибки компиляции и Package Manager (`ENOTFOUND` / proxy), из‑за чего нельзя было нормально работать с Editor и Pipeline.

## Cause

1. Кривой системный HTTP-прокси VPN (`http://http//127.0.0.1:10809`) ломал `packages.unity.com`.
2. После локальной установки `com.unity.pipeline` не хватало транзитивных пакетов (например Mono.Cecil) → CS0246 в codegen.
3. Отдельно — ошибки игрового кода (namespace EnemyAnimator и т.п., см. соседние issues).

## What we did

- Диагностика через `Editor.log` и `unity status` / Pipeline.
- Рекомендация: VPN в режиме **TUN**, не Proxy; Retry Package Manager.
- Pipeline при необходимости как `file:com.unity.pipeline` в `Packages/`.
- Игровые CS-ошибки закрыты отдельными фиксами (EnemyAnimator и др.).

## Files touched

- `Packages/manifest.json`, `Packages/com.unity.pipeline/` (локально)
- `.cursor/mcp.json`
- связанные скрипты под консольные CS-ошибки

## How to verify

1. Unity открыт на проекте, Console без CS-ошибок по `Assets/Game`.
2. `unity status` → ready (Pipeline reachable), если нужен CLI/MCP.
3. Package Manager не ругается на `com.unity.pipeline`.
