# Аудит раскладки кода (architecture review)

- **Status:** resolved (обзор, без рефакторинга)
- **Date:** 2026-09-19

## Problem

Нужна сверка структуры `Assets/Game/Scripts` с целями проекта и курсом — куда класть Enemy/Animation и что не ломать.

## Cause

Параллельные агенты и WIP могли разъехаться с каноном (лишние папки, «улучшения» DI, путаница StateMachine vs старый State/).

## What we did

- Прочитали дерево Scripts и правила `.cursor/rules`.
- Зафиксировали слои bootstrap → GSM → services/factories → Player/Enemy.
- Итог положили в `Ai.Architecture/code-folder-review.md` (этот issue — краткая запись).

## Files touched

- только документация: `Ai.Architecture/**`, этот файл  
- код не рефакторили

## How to verify

1. Открыть `Ai.Architecture/code-folder-review.md`.
2. Сверить с `Ai.Context/project-context.md` §4 и реальными папками.
