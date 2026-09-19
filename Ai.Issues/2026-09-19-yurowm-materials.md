# Yurowm: magenta / битые материалы

- **Status:** resolved
- **Date:** 2026-09-19

## Problem

После подключения Yurowm модель могла отображаться magenta (Error shader) или с неверными materials в URP.

## Cause

Импорт FBX/материалы под Built-in или сломанные ссылки на shader; URP требует корректных Lit/простых материалов.

## What we did

- Проверка renderers/sharedMaterials в smoke (`magenta` если shader name содержит `Error`).
- Починка/переназначение материалов под URP в проекте.
- Отчёт агента «Fix Yurowm materials report».

## Files touched

- `Assets/Resources/Yurowm/**` (materials/FBX import)
- префаб Player
- `AgentScripts/SmokeYurowmPlayer.cs` (детект magenta)

## How to verify

1. Game/Scene view: Yurowm не розовый.
2. Smoke `Inspect()` → `magenta: false`.
3. Materials без Error-shader.
