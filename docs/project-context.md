# Контекст проекта: 2.5D Platformer

Канонический общий «мозг» для cloud Project-агентов и локальных чатов Cursor.  
Перед продолжением чужой работы — **прочитать**; после значимых решений или смены состояния — **обновить**.

---

## 1. Цель

Проект — **учебный / эталонный пример** 2.5D platformer + документация для агентов: ясность и переносимость паттернов важнее «улучшений ради улучшений».

Жанр и механики (ориентир): движение по X, поворот, прыжки, враги, save/load, state machine.  
Репозиторий: `Zenobiam/2.5D-Platformer`. Локальный checkout (unity-pc): `D:\Unity\Projects\2.5D Platformer`.

Источник формулировки цели: [`.cursor/rules/project-goals.mdc`](../.cursor/rules/project-goals.mdc) (локальный канон; на `origin/main` пока может отсутствовать — см. §3).

---

## 2. Cloud ↔ local: как пользоваться

1. **Всегда начинать** с этого файла (`docs/project-context.md`), затем читать исходники и правила по путям ниже.
2. **Cloud-агенты** и **локальные чаты** обязаны читать и обновлять **один и тот же** файл в git — не плодить параллельные «мозги».
3. **Handoff:** после сессии кратко дописать блок в §6 (формат ниже), чтобы следующий агент продолжил без потери контекста.
4. Оригиналы правил (`.cursor/rules/*.mdc`, global user rules, `Docs/AgentHandoff/`) **не удалять и не копировать сюда целиком** — только резюме + точный путь.
5. Project Agent Store (`notes.md`, `internal/`) — координация Project; **источник истины по коду и решениям для агентов** — этот файл в репозитории.

Формат handoff-записи:

```text
### YYYY-MM-DD — кто (cloud|local) — кратко
Сделано:
Осталось:
Файлы:
Риски / не трогать:
```

---

## 3. Правила (локальный канон) — резюме и пути

Инвентаризация: `internal/local-rules-delta.md` (unity-pc, 2026-09-16). На cloud/`origin/main` этих файлов **ещё нет в git** (локально untracked), но они — действующий канон на машине разработчика.

### Project rules (`alwaysApply: true`)

| Путь | Смысл |
|---|---|
| `.cursor/rules/project-goals.mdc` | Учебный/эталонный 2.5D + доки для агентов; ясность и переносимость паттернов |
| `.cursor/rules/csharp-style.mdc` | Стиль под `Assets/Game/Scripts`: `_camelCase`, namespaces `Game.Scripts.*`, GetComponent/ServiceLocator в Awake; стек Bootstrapper→GameMain→GSM, ServiceLocator, GameFactory, IAssetsProvider |
| `.cursor/rules/course-reference.mdc` | Сверяться с CodeBase курса k-syndicate; сейчас блок Enemy; **не «улучшать» без просьбы** |

### Machine-global (вне репо; действуют на всех проектах машины)

| Путь | Смысл |
|---|---|
| `%USERPROFILE%\.cursor\rules\unity-csharp-principles.mdc` | Общий Unity C#: `_camelCase`, GetComponent в Awake, без лишнего SerializeField; **локальный стиль проекта побеждает** |
| `%USERPROFILE%\.cursor\rules\explain-comparisons.mdc` | «Лучше/хуже» только с конкретной причиной |

### Handoff kit (перенос в **новый** репозиторий)

| Путь | Смысл |
|---|---|
| `Docs/AgentHandoff/HANDOFF.md` | Как переносить эталон: что глобально, что копировать, Unity CLI+pipeline, путь курса, промпт для нового чата |
| `Docs/AgentHandoff/rules-templates/*.mdc` | Байт-копии трёх project rules выше |

**Важно:** kit — для **нового** репо. В **этом** проекте правила уже на месте → при работе здесь **ничего делать не надо** (не переустанавливать kit, не дублировать правила).

### MCP (machine-local)

| Путь | Смысл |
|---|---|
| `.cursor/mcp.json` | MCP `unity` → `%LOCALAPPDATA%\Unity\bin\unity.exe mcp --project-path d:\Unity\Projects\2.5D Platformer` |

### Не канон стиля игры

- `Packages/com.unity.pipeline/CLAUDE.md` и skill пакета — **вендор** UPM `com.unity.pipeline`, не игровой `_camelCase`-стиль.

Отсутствуют (и не нужны дубликаты): `AGENTS.md`, `.cursorrules`, корневой `CLAUDE.md`.

---

## 4. Стек и соглашения

### Архитектура (из csharp-style + код на `develop`)

Цепочка старта:

`GameBootstrapper` → `GameMain` → `GameStateMachine` (GSM)

- **ServiceLocator** (`Game.Scripts.Infrastructure`) — регистрация/резолв сервисов (`RegisterSingle` / `Single`).
- **GameFactory** + **IAssetsProvider** — создание игровых объектов / загрузка ассетов.
- Состояния GSM (на `develop`): `BootstrapState` → `LoadProgressState` / `LoadLevelState` → `GameLoopState`.
- Namespaces: `Game.Scripts.*`.
- Поля: **`_camelCase`**.
- GetComponent / резолв из ServiceLocator — в **Awake**; не размазывать `[SerializeField]` everywhere.

Ключевые пути (ветка `develop`, богаче `main`):

- `Assets/Game/Scripts/Core/` — `GameBootstrapper`, `GameMain`, `GameRunner`
- `Assets/Game/Scripts/StateMachine/` — GSM, scene loader, curtain, states
- `Assets/Game/Scripts/Infrastructure/` — `ServiceLocator`, `IService`
- `Assets/Game/Scripts/Factories/` — `GameFactory`
- `Assets/Game/Scripts/Providers/` — `AssetsProvider` / `IAssetsProvider`
- `Assets/Game/Scripts/Services/` — Input, PersistentProgress, SaveLoad

На `main` / раннем scaffold PR ещё виден старый player state machine под `Assets/Game/Scripts/State/` — при расхождении **ориентир: `develop` + csharp-style**.

### Движок / пакеты

| Что | Значение |
|---|---|
| Unity | **6000.0.46f1** (`ProjectSettings/ProjectVersion.txt`) |
| Render | URP 17 (`com.unity.render-pipelines.universal`) |
| Input | New Input System |
| IDE | Cursor / Rider / VS (`com.unity.ide.cursor` и др. в `Packages/manifest.json`) |

### Референс курса

Сверяться с CodeBase курса на машине разработчика:

`G:\MediaGetDownloads\RealyGood\...` (полный путь — в `course-reference.mdc` / `HANDOFF.md`).

Сейчас фокус курса: блок **Enemy**. Не рефакторить «лучше курса» без явной просьбы.

### Unity MCP

Конфиг: `.cursor/mcp.json` (см. §3). Пути machine-specific — на cloud без unity-pc MCP недоступен.

---

## 5. Текущее состояние и как продолжать

### Сделано

- Базовый каркас инфраструктуры на `develop`: Bootstrapper → GameMain → GSM, ServiceLocator, factories, save/load, input services.
- Shared context: этот файл + указатель в [`README.md`](../README.md).
- Локальный канон правил на unity-pc (инвентарь в Agent Store: `internal/local-rules-delta.md`).

### В работе / расхождения

- **Правила и HANDOFF** есть локально (untracked), **не на `origin/main`** — их ещё нужно закоммитить отдельной работой, если решим хранить в git.
- Cloud checkout / `main` беднее `develop` по скриптам инфраструктуры.
- Локально на `develop` также dirty геймплей (EnemyAnimator, DemoLevel, SampleScene, Packages/`com.unity.pipeline` и т.д.) — **не agent-rules**; не смешивать с этим PR.
- Prefabs/models могут быть неполны в git (есть `.meta` без контента).

### Как продолжать (следующему агенту)

1. Прочитать **этот файл** целиком.
2. Для стиля/архитектуры — `.cursor/rules/*.mdc` на local (или резюме §3–§4); сверять с курсом по Enemy.
3. Код смотреть с **`develop`**, не только `main`.
4. Не переустанавливать AgentHandoff kit в этом репо.
5. После работы — обновить §5 и добавить handoff в §6; закоммитить этот файл.

### Блокеры

- Self-hosted worker **unity-pc** может быть офлайн → для Editor/Play Mode / MCP нужен `agent worker start` на машине.
- Полный путь курса и содержимое `.mdc` — только на local, пока не в git.

---

## 6. Handoff cloud↔local

### Активный handoff

```text
### 2026-09-16 — cloud — local-canon в shared context (после обрыва self-hosted)
Сделано:
- Заменён старый scaffold docs/project-context.md на канон из local-rules-delta:
  цель (учебный/эталонный 2.5D), cloud↔local протокол, пути правил,
  стек Bootstrapper→GameMain→GSM, текущее состояние, handoff.
- README pointer сохранён.
Осталось:
- (опционально) закоммитить .cursor/rules + Docs/AgentHandoff с unity-pc в git.
- Продолжать геймплей (Enemy и т.д.) по course-reference на develop.
Файлы:
- docs/project-context.md
- README.md
Риски / не трогать:
- Не дублировать правила в AGENTS.md / .cursorrules.
- Не «улучшать» код курса без просьбы.
- Не тащить в этот PR локальный dirty геймплей / com.unity.pipeline.
```

### Предыдущий

```text
### 2026-09-16 — cloud — shared context bootstrap
Сделано: инвентаризация cloud checkout; создан scaffold docs/project-context.md + README.
Заметка: формальных rules в cloud не было — позже заменено local-каноном (запись выше).
```
