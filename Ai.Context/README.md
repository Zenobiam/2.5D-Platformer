# Ai.Context — канон для агентов

**Читать в первую очередь** перед любой работой по этому репозиторию.

## Порядок чтения

Точка входа — [`../AGENTS.md`](../AGENTS.md) (Claude Code: [`../CLAUDE.md`](../CLAUDE.md) подгружает его сам). Дальше:

1. [`../INTENT.md`](../INTENT.md) — зачем проект, цели и не-цели  
2. Этот README  
3. [`agent-playbook.md`](agent-playbook.md) — **обязательно для workers**: роли, kickoff, порядок работы, временный код, коммит  
4. [`project-context.md`](project-context.md) — стек, пути к курсу, текущее состояние, активный handoff (прошлые — [`handoff-archive.md`](handoff-archive.md), читать только по нужде)  
5. [`preferences.md`](preferences.md) — куда класть deliverables, курс 2021 vs Unity 6  
6. Для задачи по уроку — [`../Ai.Architecture/course-map.md`](../Ai.Architecture/course-map.md) и [`../Ai.Architecture/course-deviations.md`](../Ai.Architecture/course-deviations.md)  
7. При необходимости — [`../Ai.Issues/`](../Ai.Issues/) (только записи по теме задачи)  
8. **Общая база (вне репо):** `D:\Unity\Knowledge\README.md` — правила Unity (компоненты, скрипты, DI, префабы, анимация, редакторные инструменты, SOLID, git), описания механик, правила ревью (`process/self-review.md`) и реестр классов дефектов (`process/review-insights.md`). Читать перед написанием кода и перед ревью.

Большие файлы (журналы, обзоры) читать по заголовкам, нужной секцией, а не целиком.

## Что обновить

Документ, который правка сделала неверным, обновляется **тем же изменением**, не «потом».

| Изменение | Обновить |
|---|---|
| перенесли класс из курса, переименовали или переместили файл | `Ai.Architecture/course-map.md` |
| отступили от кода курса (API Unity 6, мобилка) | `Ai.Architecture/course-deviations.md` |
| закончили урок или его часть | `course-map.md` § «Пройденные уроки», `project-context.md` § 5–6, `Ai.Tasks/<бриф>` § «Итог» |
| новый пункт `Tools → …`, сервис, состояние, путь в `AssetPath` | `project-context.md` § 4 |
| изменили механику (здоровье, бой, спавн) | `D:\Unity\Knowledge\mechanics/<механика>.md` |
| нашли класс дефекта или ложную тревогу | `D:\Unity\Knowledge\process\review-insights.md` |
| починили баг / сделали фичу | `Ai.Issues/bugs|features/YYYY-MM-DD-*.md` |
| решение пользователя (правило, процесс, архитектура) | `Ai.Decisions/YYYY-MM-DD-*.md` (с отвергнутой альтернативой) + сам файл правила |

## Папки `Ai.*` в корне репо

| Папка | Назначение |
|---|---|
| `Ai.Context/` | Долгоживущий канон (этот раздел) |
| `Ai.Issues/` | Issues в `bugs/` и `features/`: проблема → причина → что сделали |
| `Ai.Architecture/` | Обзоры структуры кода / сверка с курсом (`course-map.md`, `course-deviations.md`) |
| `Ai.Tasks/` | Брифы задач: Goal / Constraints / Paths / DoD / Don't-touch + Итог |
| `Ai.Decisions/` | Решения пользователя: что выбрали, почему, что отвергли |

Имена папок именно `Ai.<name>`, не `issues` / `context` / `docs` для агентского канона.

## Связанные пути

- Правила стиля/целей: [`.cursor/rules/`](../.cursor/rules/)  
- Handoff-kit для **нового** репо: [`Docs/AgentHandoff/HANDOFF.md`](../Docs/AgentHandoff/HANDOFF.md)  
- Старый указатель: [`Docs/project-context.md`](../Docs/project-context.md) → редирект сюда  

## Схема имён Issues

`Ai.Issues/bugs|features/YYYY-MM-DD-short-kebab.md` (дата сессии/фикса + короткий slug).
