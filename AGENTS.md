# AGENTS.md — вход для AI-агентов

Unity 6 проект 2.5D Platformer — учебный эталон по курсу k-syndicate. Здесь только порядок чтения и стоп-правила. Канон — в `Ai.Context/`, при расхождении побеждает он.

**`<Knowledge>`** — общая база правил вне репо (`https://github.com/Zenobiam/unity-knowledge`). Путь у каждого свой: он задан в `~/.claude/CLAUDE.md` (строка `@…/process/agent-workflow.md`), у автора — `D:\Unity\Knowledge`. Нет базы — настройка по `<Knowledge>\starter-kit\README.md`.

## Перед работой прочитать

1. [`INTENT.md`](INTENT.md) — зачем проект, цели, не-цели, как выбирать при сомнении.
2. [`Ai.Context/README.md`](Ai.Context/README.md) — порядок чтения канона и таблица «что обновить».
3. `<Knowledge>\process\agent-workflow.md` (вне репо) — общий регламент: порядок работы, Unity Editor, сессии, временный код, git, отчёт. Затем [`Ai.Context/agent-playbook.md`](Ai.Context/agent-playbook.md) — проектная часть (worker, чужой WIP, курс); она побеждает при противоречии.
4. [`Ai.Context/project-context.md`](Ai.Context/project-context.md) §§ 4–6 — стек, пути к курсу, текущее состояние, активный handoff.
5. Бриф задачи в [`Ai.Tasks/`](Ai.Tasks/), если есть. Для урока — [`Ai.Architecture/course-map.md`](Ai.Architecture/course-map.md) и [`course-deviations.md`](Ai.Architecture/course-deviations.md).
6. `<Knowledge>\README.md` (вне репо) — перед кодом и ревью: `conventions/`, нужный файл `mechanics/`, `process/review-insights.md`.

## Первая фраза ответа

Текущий шаг и что нужно от пользователя.

## Стоп-правила

- **План до кода.** Бриф и план → показать → ждать явного «да». После «да» — без вопросов «продолжить?» до точки, где нужен пользователь.
- **Курс:** архитектура из курса, API Unity 6; каждое отступление — строкой в `Ai.Architecture/course-deviations.md`. Интерфейсы, DI-контейнеры, паттерны и рефакторинги сверх курса — не делать; предлагать можно.
- **Чужой WIP Enemy/Aggro не трогать:** `Aggro`, `Follow`, `AgentMoveToPlayer`, `AnimateAlongAgent`, `RotateToHero`, `TriggerObserver`, `CheckAttackRange` — только по согласованию.
- **YAML `.prefab`, `.unity`, `.controller`, `ProjectSettings/*.asset` руками не править:** Editor-скрипт (`Tools → …`) или точные шаги пользователю в Inspector.
- **Мобилка:** экранная кнопка для каждого действия, без аллокаций в `Update`.
- **Unity Editor:** Refresh, пункты меню, пакеты — только с разрешения пользователя; Play Mode запускает пользователь. Unity CLI — через свою связанную сессию-worker (только чтение), создавать её — с подтверждения пользователя; чужой worker не использовать.
- **Git:** не коммитить и не пушить без явной просьбы; файлы поимённо. Удалять файлы — только с подтверждения.
- **Проверка:** «собралось» — только по выводу консоли (`0 error CS`).
- **Отчёт** после каждой задачи — по формату `agent-workflow.md` § 7, Observations/Risks/Proposals ≤ 5 пунктов.
