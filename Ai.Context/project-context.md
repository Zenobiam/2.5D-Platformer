# Контекст проекта: 2.5D Platformer

> Канон живёт в **`Ai.Context/`**. Индекс: [`README.md`](README.md). Прошлые решения: [`../Ai.Issues/`](../Ai.Issues/).

Канонический общий «мозг» для **cloud** Project-агентов и **локальных** чатов Cursor.  
Перед продолжением чужой работы — **прочитать**; после значимых решений или смены состояния — **обновить**.

Репозиторий: `Zenobiam/2.5D-Platformer` · локальный checkout: `D:\Unity\Projects\2.5D Platformer`

---

## 1. Цель проекта

Это не только «игра», а **учебный/эталонный пример + документация**, чтобы ускорять следующие Unity-проекты (в т.ч. с AI-агентами).

Следствия:

- код и структура **читаемые и повторяемые**, не только «чтобы заработало»;
- паттерны (bootstrap, state machine, services, factories, animator readers) — **образец для копирования**;
- при сомнении — ясность и совпадение с курсом/правилами стиля, а не уникальные трюки;
- документация и `.cursor/rules` должны совпадать с реальным кодом;
- решения агента должны переноситься в другой проект с минимальной адаптацией.

**Жанр:** 2.5D platformer (бок/слегка объёмный): движение по X, поворот модели, прыжки/земля, враги, save/load progress, сцены через game state machine.

Полный текст: [`.cursor/rules/project-goals.mdc`](../.cursor/rules/project-goals.mdc).

---

## 2. Как пользоваться (cloud ↔ local)

1. **Старт сессии:** прочитать этот файл → при необходимости исходники по путям ниже → при фичах из урока — CodeBase курса (см. §4).
2. **Обязательно для cloud и local:** один и тот же файл в git; не плодить параллельные «мозги» (второй `AGENTS.md` / `.cursorrules` с дублирующим текстом без нужды).
3. **После значимой сессии** кратко обновить:
   - §5 **Текущее состояние**;
   - §4 **Стек / решения** (если приняли новое);
   - §6 **Handoff cloud↔local** (что подхватить другому агенту).
4. **Оригиналы правил не удалять и не копировать сюда целиком** — только резюме + точный путь (§3).
5. Project Agent Store (`notes.md`, `internal/`) — координация Project; **источник истины по коду и решениям** — `Ai.Context/` (+ `Ai.Issues/` для прошлых фиксов) в репозитории.
6. **Новый Unity-репозиторий:** копировать шаблоны из [`Docs/AgentHandoff/HANDOFF.md`](../Docs/AgentHandoff/HANDOFF.md). **Этот репозиторий уже настроен** — правила стоят в `.cursor/rules`; handoff-kit не переустанавливать «с нуля».
7. Deliverables для пользователя — под `Ai.*` (см. [`preferences.md`](preferences.md)).

---

## 3. Зафиксированные правила

Сводка + пути. Полные тексты — только в оригиналах.

### В репозитории (проект)

| Путь | Резюме |
|---|---|
| [`.cursor/rules/project-goals.mdc`](../.cursor/rules/project-goals.mdc) | Эталон + доки для агентов; жанр 2.5D; ясность и переносимость паттернов |
| [`.cursor/rules/csharp-style.mdc`](../.cursor/rules/csharp-style.mdc) | Стиль под `Assets/Game/Scripts`: `_camelCase`, `Game.Scripts.*`, GetComponent/ServiceLocator в `Awake`, без SerializeField-everywhere; стек Bootstrapper → GameMain → GSM |
| [`.cursor/rules/course-reference.mdc`](../.cursor/rules/course-reference.mdc) | Сверяться с CodeBase курса k-syndicate; текущий блок Enemy; не «улучшать» без просьбы |
| [`.cursor/mcp.json`](../.cursor/mcp.json) | MCP `unity` → локальный `unity.exe mcp --project-path …` (пути машинно-зависимы) |
| [`Docs/AgentHandoff/HANDOFF.md`](../Docs/AgentHandoff/HANDOFF.md) | Kit для **нового** репо: что копировать, Unity CLI/pipeline, промпт; шаблоны в `Docs/AgentHandoff/rules-templates/` |
| [`README.md`](../README.md) | Точка входа → `Ai.Context/` |
| [`Docs/project-context.md`](../Docs/project-context.md) | Редирект на этот канон (старый путь) |

> На remote `main` правила могут ещё отсутствовать, пока `.cursor/` / `Docs/AgentHandoff/` не запушены с локальной машины. Канон целей/стиля всё равно зафиксирован здесь; при наличии файлов в checkout — читать оригиналы.

### Вне репозитория (машина пользователя)

| Путь | Резюме |
|---|---|
| `%USERPROFILE%\.cursor\rules\unity-csharp-principles.mdc` | Общий Unity C# для всех проектов; локальный стиль проекта побеждает |
| `%USERPROFILE%\.cursor\rules\explain-comparisons.mdc` | «Лучше/хуже» только с конкретной причиной |
| `%USERPROFILE%\.cursor\skills\**` | Глобальные Unity skills (cli, ui, urp, …) — не project-local |

### Чего нет (и не заводить без нужды)

`AGENTS.md`, `.cursorrules`, корневой `CLAUDE.md` — отсутствуют; достаточно этого файла + `.cursor/rules`.

---

## 4. Стек и решения

### Движок / пакеты

- **Unity 6** — локально `6000.0.60f1` (`ProjectSettings/ProjectVersion.txt`); cloud-scaffold раньше видел `6000.0.46f1`.
- **URP** `com.unity.render-pipelines.universal` 17.x.
- **Input System** `com.unity.inputsystem`; asset [`Assets/PlayerControls.inputactions`](../Assets/PlayerControls.inputactions).
- **AI Navigation** `com.unity.ai.navigation` (для Enemy/NavMesh по курсу).
- **Unity Pipeline / MCP:** пакет `com.unity.pipeline` + CLI `%LOCALAPPDATA%\Unity\bin\unity.exe`; конфиг [`.cursor/mcp.json`](../.cursor/mcp.json).

### Архитектура (как в коде и csharp-style)

```
GameBootstrapper → GameMain → GameStateMachine
  BootstrapState → LoadProgressState → LoadLevelState → GameLoopState
```

- **DI:** свой `ServiceLocator.Container` + `IService` / `RegisterSingle` / `Single` — без чужих DI-фреймворков.
- **Фабрики / ассеты:** `GameFactory` + `IAssetsProvider` + `AssetPath` (`Prefabs/Player/Player`, `Prefabs/Player/HUD`).
- **Прогресс:** `IPersistentProgressService` / `ISaveLoadService` / `ISaveProgress` + `SaveTrigger`.
- **Ввод:** `IInputService` / `InputService` / `InputReceiver` (не отдельный `PlayerInputController` как основной слой).
- **Игрок:** `PlayerMovementController` + `PlayerVisual` (движение/визуал); **не** иерархия `Assets/Game/Scripts/State/` — игровой SM живёт в `StateMachine/`.
- **Анимация / враги (блок Enemy):** `EnemyAnimator` + `IAnimationStateReader` + `AnimatorStateReporter` / `AnimatorState`.
- **Сцена:** `Assets/Game/Scenes/SampleScene.unity`; демо-геометрия — Tools → Build Demo Level (`Assets/Game/Editor/…`).
- **Namespaces:** `Game.Scripts.<Area>`; код в `Assets/Game/Scripts`.

### Референс курса

Корень:  
`G:\MediaGetDownloads\RealyGood\k-syndicate.school - Архитектура мобильных игр на UNITY для профессионалов (2021)\`

Актуальный блок **05 Enemy**:  
`...\05 Enemy\knowledge-is-power-master\src\KnowledgeIsPower\Assets\CodeBase`

Перед классом из урока — прочитать файл в CodeBase курса; адаптировать namespace под `Game.Scripts.*`; не улучшать без просьбы.

### Стиль (кратко)

- поля: `_camelCase`; хэши Animator: `private static readonly int XHash = …`;
- зависимости на том же объекте — `GetComponent` / `GetComponentInChildren` / ServiceLocator в `Awake`;
- SerializeField — когда реально вешают в Inspector;
- не рефакторить мимо задачи; не внедрять новый стек без запроса.

---

## 5. Текущее состояние / как продолжить

_Обновлять в конце значимых сессий._

### Сделано (локальный `develop` + WIP, на момент 2026-09-16)

- Каркас bootstrap / game state machine / ServiceLocator / factories / assets provider.
- Save/Load progress (`LoadProgressState`, persistent progress, save trigger).
- Игрок: движение + visual; Input System + `IInputService`.
- Блок Enemy: начат (`EnemyAnimator`, animation readers/reporter); ресурсы `Assets/Resources/Enemies/`.
- Demo level builder (Editor), URP, SampleScene.
- Локально: `.cursor/rules/*`, `Docs/AgentHandoff/*`, Unity MCP (`mcp.json` + `com.unity.pipeline`).

### В работе / дальше по курсу

1. Продолжить **Enemy** по CodeBase урока 05 (аггро, атаки, здоровье, спавн через factory — как в курсе).
2. Довести префабы/аниматор врага и связь с `EnemyAnimator`.
3. По необходимости: NavMesh, камера (`CameraController` TODO), полировка демо-уровня.
4. Запушить в git то, что ещё только локально (rules/handoff/WIP Enemy), когда пользователь готов.

### Не трогать без просьбы

- Логику save/load и существующий DI/ServiceLocator «ради красоты».
- Полную переписывание стиля public Inspector-полей / старых скриптов без namespace.
- Переустановку AgentHandoff-kit в **этом** репо (уже установлено).

### Ветки

| Ветка | Заметка |
|---|---|
| `main` | default на GitHub; базовый movement |
| `develop` | дальше save/load и текущая разработка (локально часто dirty) |
| PR #1 `cursor/shared-project-context-*` | этот shared-context документ |

---

## 6. Handoff cloud ↔ local

### Формат записи

```text
### YYYY-MM-DD — кто (cloud|local) — кратко
Сделано:
Осталось:
Файлы:
Риски / не трогать:
```

### Активный handoff

```text
### 2026-09-19 — worker — Ai.* docs structure
Сделано:
- Канон перенесён в Ai.Context/; Issues в Ai.Issues/; обзор в Ai.Architecture/.
- README + Docs/project-context.md указывают на Ai.Context.
- Прошлые задачи задокументированы как отдельные issue MD.
Осталось:
- Продолжать Enemy/Aggro по курсу 05 (параллельный агент — не конфликтовать).
- Закоммитить/запушить Ai.* когда координатор/пользователь готов.
Файлы:
- Ai.Context/**, Ai.Issues/**, Ai.Architecture/**
- README.md, Docs/project-context.md
Риски / не трогать:
- Не править Enemy/Aggro WIP чужого агента.
- Не плодить второй канон вне Ai.Context.

### 2026-09-16 — local (unity-pc) — канон project-context из локальных правил
Сделано:
- Заменён cloud-scaffold docs/project-context.md на канон с целями, правилами (пути), стеком из кода, протоколом handoff.
- Указатель в README сохранён/обновлён.
- Учтены локальные .cursor/rules, Docs/AgentHandoff, глобальные user rules, блок Enemy.
Осталось:
- Продолжать Enemy по курсу 05; синхронизировать незапушенные rules/WIP с remote по готовности.
Файлы:
- (исторически) docs/project-context.md → теперь Ai.Context/project-context.md
Риски / не трогать:
- Не дублировать полные тексты правил сюда; не переустанавливать AgentHandoff в этом репо.
```

### Только на local / self-hosted

- `Library/`, `Temp/`, `UserSettings/`, сгенерированные `*.csproj` / `*.sln`.
- Незакоммиченный WIP на `develop` (Enemy, Editor demo level, Resources, dirty scene/packages).
- Машинные пути в `.cursor/mcp.json` и курс на `G:\…`.
- Открытый Unity Editor / Play Mode / MCP `unity status → ready`.

---

## Указатель для новых репозиториев

Копирование шаблонов и первичный setup: **[`Docs/AgentHandoff/HANDOFF.md`](../Docs/AgentHandoff/HANDOFF.md)** — только для **нового** проекта. Здесь правила уже в `.cursor/rules`.
