# Agent playbook

Проектная часть регламента. **Общие правила** (порядок работы, Unity Editor и ассеты, сессии, временный код, git, отчёт) — `<Knowledge>\process\agent-workflow.md`; здесь только то, что касается этого репозитория. При противоречии побеждает этот файл.

## Роли

| Роль | Задача |
|---|---|
| **Human** | Цель, ограничения, DoD; один таск → один агент |
| **Coordinator** | Разбивка, маршрутизация, сверка статуса; не трогает Editor без нужды |
| **unity-pc worker** | Локальный Editor/терминал/файлы на Windows-ПК |
| **Ai.Context** | Долгоживущий канон (этот раздел, `project-context`, preferences) |
| **Ai.Issues** | Архив: `bugs/` и `features/` — проблема → причина → что сделали |

## Советы человеку

- Одна задача — один агент
- Геймплей и доки — отдельными коммитами, когда удобно
- Worker `unity-pc` держать онлайн, если нужен Editor

## Проект: сессии и Unity-worker

- **Свой worker:** сейчас `unity-reader`, `local_d6a7af1a-2068-4e0f-8011-33108e0fc865`, Haiku, только чтение. Чужой worker (сессии «Main old») не использовать. Refresh/перекомпиляцию worker без явного разрешения не делает — проще попросить пользователя переключиться в окно Unity.
- **Чужой WIP Enemy/Aggro не трогать:** `Aggro.cs`, `Follow.cs`, `AgentMoveToPlayer.cs`, `AnimateAlongAgent.cs`, `RotateToPlayer.cs`, `TriggerObserver.cs`, `CheckAttackRange.cs`.
- **Ассеты:** `Assets/Resources/Yurowm/*` — только по прямой задаче. Повторяемые настройки — Editor-скриптами в `Assets/Game/Editor` (`Tools → Setup → …`).

## Проект: скиллы плагинов Unity и unity-perf

Подключены пользователем 2026-10-26. Общее (установка, ограничения) — `<Knowledge>\process\agent-tooling.md` § 2. Скилл — справочник, эталон — курс: совет, расходящийся с кодом курса, — только по решению пользователя и строкой в `course-deviations.md`.

| Когда | Скилл |
|---|---|
| Розовые материалы из паков курса (лут, новые враги) | `unity:migrate-birp-to-urp` (у нас для лута — `Tools → Setup → Loot Materials to URP`) |
| TMP: `PickupPopup`, счётчик лута (урок 10), UI уроков 12–13 | `unity:optimize-text-mesh-pro`, `unity:ui-ugui` |
| NavMesh, агенты врагов | `unity:initialize-ai-navigation` |
| Триггеры: подбор лута, зоны `Aggro`/`AttackZone`, проход сквозь врагов | `unity:physics-3d-collision` |
| Найти ассеты и объекты сцены (вместо полного скана `Assets/`) | `unity:generate-editor-search-query` |
| Ревью, именование | `unity-perf:code-standards` (наши правила — `<Knowledge>\conventions\` — важнее) |
| «Без аллокаций в `Update`», профилирование под мобилку | `unity-perf:csharp-zero-gc`, `unity-perf:unity-performance` |
| Урок 14 Rewarded ads, урок 16 InApps | `unity:levelplay-unity-integration`, `unity:implement-in-app-purchases` — только справочно: курс делает рекламу и покупки по-своему |

Не использовать без явной просьбы пользователя: `unity:unity-cli` на изменения (Editor — через свой worker `unity-reader`, только чтение), `unity:unity-package-management` (пакеты — с разрешения), `unity:new-unity-project`.

## Проект: курс

- Эталон — курс k-syndicate. Код урока — `diff` папок курса N и N+1; соответствие имён и папок — [`../Ai.Architecture/course-map.md`](../Ai.Architecture/course-map.md); отступления — [`../Ai.Architecture/course-deviations.md`](../Ai.Architecture/course-deviations.md).
- Курс мобильный: экранная кнопка для каждого действия, без аллокаций в `Update`.
- **«Что и зачем» по каждому компоненту** — общее правило `<Knowledge>\process\agent-workflow.md` § 2 п. 11 (решение пользователя 2026-10-30). Для курса: мало собрать «как в уроке» — в брифе урока раздел «Компоненты: что и зачем», включая UI и настройки импорта.
- **Новый враг** — сначала `<Knowledge>\mechanics\new-enemy.md`: компоненты из `Enemy.prefab`, что настраивает фабрика, дочерние зоны, правила дистанций и событий атаки; сборка — `Tools → Setup → Enemy Variant: …`.
- Маркер временного кода по курсу: `// COURSE-TEMP: <что>, убрать в уроке <N>`. `ActorUI.Start` — `// COURSE-TEMP` (переведён 2026-10-30): хак курса, курс оставляет его и после урока 13.
- Документы после задачи — по таблице в [`README.md`](README.md) § «Что обновить».

## Ревью (проверяющая сессия)

Общие правила — `<Knowledge>\process\self-review.md` и `review-insights.md`. Проект-специфично:

- Namespace и путь файла совпадают (`Game.Scripts.*`).
- `Ai.Tasks`-бриф (если есть) совпадает с diff.
- Не ревьюить WIP Enemy/Aggro чужого агента без просьбы координатора.

## После успеха

Добавить или обновить запись в `Ai.Issues/bugs/` или `Ai.Issues/features/` (`YYYY-MM-DD-short-kebab.md`).

## Отчёт: на что смотреть в Observations

Формат отчёта — `agent-workflow.md` § 7. Для этого проекта отмечать:

- path ≠ namespace
- misuse папок `Interfaces/`
- drift от CodeBase курса
- устаревший `Ai.Context`
- лишние full-repo scans / ненужный Play Mode
