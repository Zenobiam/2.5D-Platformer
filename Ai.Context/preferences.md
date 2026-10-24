# Preferences (агенты)

## Deliverables для пользователя

Пользовательские и агентские deliverables по проекту класть **в репозиторий** под префиксом `Ai.*`:

- канон / handoff → `Ai.Context/`
- разобранные баги → `Ai.Issues/bugs/`; фичи/wiring/обзоры → `Ai.Issues/features/`
- архитектурные обзоры → `Ai.Architecture/`

Не плодить параллельные «мозги» в корне (`AGENTS.md`, дубли `docs/` с тем же текстом).  
Project Agent Store (`notes.md`, `internal/`) — координация; **источник истины по решениям** — `Ai.Context` + `Ai.Issues` в git.

## Не коммитить без нужды

WIP Enemy/Aggro и чужой незавершённый код — не трогать и не форсировать merge, если параллельный агент уже работает над этим.

## Architecture evolution

Идём по обучающим урокам курса — учебный/эталонный стек (в т.ч. самописный DI/ServiceLocator).  
Не предлагать и не внедрять ранний переход на Zenject/VContainer и т.п.  
В конце цикла курса — отдельная ревизия архитектуры и выбор готовых инструментов.  
В Observations/Proposals можно упоминать future tooling; реализовывать только по явной просьбе.

## Курс 2021 (Unity 2019.3) vs наш Unity 6

**Архитектура — из курса, API Unity — современные.**

- Из курса без изменений: bootstrap, game state machine, сервисы, фабрики, провайдер ассетов, ServiceLocator/DI, прогресс, `IHealth`, порядок уроков.
- Отступать от кода курса можно, только если его API в Unity 6 устарел, удалён или плохо работает (мобилка, Input System). Пример: `UnityEngine.Input` + SimpleInput → Input System.
- Каждое отступление — одна строка «курс → у нас → почему» в [`../Ai.Architecture/course-deviations.md`](../Ai.Architecture/course-deviations.md).
- Новые паттерны и фреймворки сверх курса (Zenject и т. п.) — по-прежнему только по явной просьбе (см. Architecture evolution).
