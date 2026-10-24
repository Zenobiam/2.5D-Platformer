# Agent playbook

Проектная часть регламента. **Общие правила** (порядок работы, Unity Editor и ассеты, сессии, временный код, git, отчёт) — `D:\Unity\Knowledge\process\agent-workflow.md`; здесь только то, что касается этого репозитория. При противоречии побеждает этот файл.

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
- **Чужой WIP Enemy/Aggro не трогать:** `Aggro.cs`, `Follow.cs`, `AgentMoveToPlayer.cs`, `AnimateAlongAgent.cs`, `RotateToHero.cs`, `TriggerObserver.cs`, `CheckAttackRange.cs`.
- **Ассеты:** `Assets/Resources/Yurowm/*` — только по прямой задаче. Повторяемые настройки — Editor-скриптами в `Assets/Game/Editor` (`Tools → Setup → …`).

## Проект: курс

- Эталон — курс k-syndicate. Код урока — `diff` папок курса N и N+1; соответствие имён и папок — [`../Ai.Architecture/course-map.md`](../Ai.Architecture/course-map.md); отступления — [`../Ai.Architecture/course-deviations.md`](../Ai.Architecture/course-deviations.md).
- Курс мобильный: экранная кнопка для каждого действия, без аллокаций в `Update`.
- Маркер временного кода по курсу: `// COURSE-TEMP: <что>, убрать в уроке <N>`. Известное исключение: `ActorUI.Start` помечен `// TEMP:` — хак курса; при ближайшей правке файла перевести в `COURSE-TEMP`.
- Документы после задачи — по таблице в [`README.md`](README.md) § «Что обновить».

## Ревью (проверяющая сессия)

Общие правила — `D:\Unity\Knowledge\process\self-review.md` и `review-insights.md`. Проект-специфично:

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
