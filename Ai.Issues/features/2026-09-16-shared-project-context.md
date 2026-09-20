# Shared project context (канон для агентов)

- **Status:** resolved
- **Date:** 2026-09-16

## Problem

Cloud- и local-агенты не имели единого «мозга» в git: цели, стек, правила и handoff разъезжались между scaffold и локальными `.cursor/rules`.

## Cause

Первый cloud-scaffold дал черновик `docs/project-context.md`, не совпадающий с локальным каноном (цели, csharp-style, курс Enemy, реальные пути).

## What we did

- Заменили scaffold на канон: цели, таблица правил (пути), стек из кода, протокол handoff cloud↔local.
- Указатель в `README.md`.
- Позже (2026-09-19) канон перенесён в `Ai.Context/` — этот issue зафиксировал исходное решение.

## Files touched

- `Docs/project-context.md` (позже stub-редирект)
- `Ai.Context/project-context.md` (текущий канон)
- `README.md`
- ветка/PR `cursor/shared-project-context-*`

## How to verify

1. Открыть `Ai.Context/README.md` → `project-context.md`.
2. Сверить §3–§4 с `.cursor/rules` и `Assets/Game/Scripts`.
3. Старый `Docs/project-context.md` должен вести на `Ai.Context`.
