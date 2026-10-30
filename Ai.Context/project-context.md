# Контекст проекта: 2.5D Platformer

> Канон живёт в **`Ai.Context/`**. Индекс: [`README.md`](README.md). Прошлые решения: [`../Ai.Issues/`](../Ai.Issues/).

Канонический общий «мозг» для **cloud** Project-агентов и **локальных** чатов Cursor.  
Перед продолжением чужой работы — **прочитать**; после значимых решений или смены состояния — **обновить**.

Репозиторий: `Zenobiam/2.5D-Platformer` · локальный checkout: `D:\Unity\Projects\2.5D Platformer`

---

## 1. Цель проекта

Зачем проект, цели, не-цели и как выбирать при сомнении — [`../INTENT.md`](../INTENT.md) (источник истины). Кратко: учебный эталон по курсу k-syndicate + документация для переноса в следующие Unity-проекты; жанр 2.5D platformer.

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
6. **Новый Unity-репозиторий:** `<Knowledge>\starter-kit\README.md` (`new-project.ps1`). **Этот репозиторий уже настроен** — starter-kit сюда не запускать.
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
| [`Docs/AgentHandoff/HANDOFF.md`](../Docs/AgentHandoff/HANDOFF.md) | Устарело: новый репо настраивается через `<Knowledge>\starter-kit\` (`install-global.ps1`, `new-project.ps1`) |
| [`README.md`](../README.md) | Точка входа → `Ai.Context/` |
| [`Docs/project-context.md`](../Docs/project-context.md) | Редирект на этот канон (старый путь) |

> На remote `main` правила могут ещё отсутствовать, пока `.cursor/` / `Docs/AgentHandoff/` не запушены с локальной машины. Канон целей/стиля всё равно зафиксирован здесь; при наличии файлов в checkout — читать оригиналы.

### Вне репозитория (машина пользователя)

| Путь | Резюме |
|---|---|
| `%USERPROFILE%\.cursor\rules\unity-csharp-principles.mdc` | Общий Unity C# для всех проектов; локальный стиль проекта побеждает |
| `%USERPROFILE%\.cursor\rules\explain-comparisons.mdc` | «Лучше/хуже» только с конкретной причиной |
| `%USERPROFILE%\.cursor\skills\**` | Глобальные Unity skills (cli, ui, urp, …) — не project-local |

### Файлы входа агентов

Решение пользователя 2026-10-24 ([`Ai.Decisions/2026-10-24-agent-entry-files.md`](../Ai.Decisions/2026-10-24-agent-entry-files.md)): в корне есть [`AGENTS.md`](../AGENTS.md) (общее ядро, читает Cursor и другие агенты) и [`CLAUDE.md`](../CLAUDE.md) (импортирует `AGENTS.md`, `INTENT.md`, playbook для Claude Code). Канон по-прежнему здесь, в `Ai.Context/`; файлы входа — только порядок чтения и стоп-правила, без копий канона. Отвергнуто: «не заводить, вставлять стартовый промпт вручную» — промпт приходилось повторять каждую сессию. `.cursorrules` по-прежнему не заводим.

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
  BootstrapState → LoadProgressState → LoadSceneState → GameLoopState
```

- **DI:** свой `ServiceLocator.Container` + `IService` / `RegisterSingle` / `Single` — без чужих DI-фреймворков. Эволюция стека (не предлагать Zenject/VContainer до конца курса): [`preferences.md`](preferences.md) § Architecture evolution.
- **Фабрики / ассеты:** `GameFactory` + `IAssetsProvider` + `AssetPath` (`Prefabs/Player/Player`, `Prefabs/Player/HUD`).
- **Прогресс:** `IPersistentProgressService` / `ISaveLoadService` / `ISaveProgress` + `SaveTrigger`; `KillData.ClearedSpawners` — очищенные спавнеры (`EnemySpawner` + `UniqueId`, тег `EnemySpawner`).
- **Ввод:** `IInputService` / `InputService` / `InputReceiver` (не отдельный `PlayerInputController` как основной слой).
- **Игрок:** `PlayerMovementController` (движение), `PlayerFacing` (поворот модели), `PlayerAnimator` (Animator: `Speed`, `Damage`, `Death`, `Attack`, `IsAttacking`), `PlayerHealth` (HP, `IHealth`), `PlayerDeath` (смерть), `PlayerAttack` (кнопка атаки и событие `OnAttack`); **не** иерархия `Assets/Game/Scripts/State/` — игровой SM живёт в `StateMachine/`.
- **Бой:** `EnemyHealth` реализует `IHealth` (интерфейс в `Game.Scripts.BaseLogic.Interfaces`); `EnemyAnimator.PlayHit()` вызывает триггер `Damage` у Skeleton. Слой `Hittable` для целей удара игрока.
- **Анимация / враги (блок Enemy):** `EnemyAnimator` + `IAnimationStateReader` + `AnimatorStateReporter` / `AnimatorState`.
- **Сцена:** `Assets/Game/Scenes/SampleScene.unity`; демо-геометрия — `Tools → Build Demo Level Geometry` (`Assets/Game/Editor/BuildDemoLevel.cs`; **не запускать повторно** — удаляет `NavMeshSurface`, см. [`../Ai.Architecture/editor-setup.md`](../Ai.Architecture/editor-setup.md) § 5).
- **Namespaces:** `Game.Scripts.<Area>`; код в `Assets/Game/Scripts`.

### Референс курса

Корень:  
`G:\MediaGetDownloads\RealyGood\k-syndicate.school - Архитектура мобильных игр на UNITY для профессионалов (2021)\`

Пройдено: 05 Enemy, 06–07 Fighting (2026-10-24), 08 Enemy Spawner, 09 Static data + враг Golem (2026-10-25). Следующий урок — **10 Loot**: начало — `...\10 Loot\knowledge-is-power-master\…\CodeBase`, итог — `...\11 Spawner rework\knowledge-is-power-master\…\CodeBase` (есть и распакованная папка, и zip). Уроки делает пользователь сам, агент помогает по просьбе.

**Папка урока N — код на начало урока; итог урока N — в папке N+1** (в уроках 11–16 код лежит только в `.zip`).

Соответствие имён и папок курса нашим — [`../Ai.Architecture/course-map.md`](../Ai.Architecture/course-map.md). Перед классом из урока — прочитать файл в CodeBase курса; не улучшать без просьбы.

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

1. **Бой игрока** (см. `Ai.Tasks/2026-10-24-player-combat.md`, «Open (актуально)»): удар и смерть врага работают; осталось проверить экранную кнопку в Simulator и смерть игрока во время удара, настроить полоску HP Skeleton (`blank`), заменить заглушки анимаций Skeleton, закоммитить.
2. Продолжить **Enemy** по CodeBase урока 05–06 (аггро, атаки, здоровье, спавн через factory — как в курсе).
3. По необходимости: NavMesh, камера (`CameraController` TODO), полировка демо-уровня.
4. Запушить в git то, что ещё только локально, по задачам отдельными коммитами, когда пользователь готов.

**Курс мобильный.** Экранные кнопки и производительность (без аллокаций в кадре) — обязательны.

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

Здесь только последняя запись. Предыдущие — [`handoff-archive.md`](handoff-archive.md) (при новой записи старую переносить туда, наверх).

```text
### 2026-10-30 (вечер) — пользователь + координатор (Opus, local) — уроки 11–14 закрыты, следующий урок 15 Addressables
Где мы:
- Ветка `feature/rewarded-ads` (от `feature/ui` ← `feature/spawner-rework` ← `feature/static-data`), запушено, рабочая папка чистая (кроме локальных `Ai.Tasks`). Ветки уроков в `main` не слиты.
- Уроки 11 Spawner rework, 12 UI part 1, 13 UI part 2, 14 Rewarded ads — закрыты (Play Mode — пользователь). Подробности: `Ai.Architecture/course-map.md` (таблица уроков), `Ai.Issues/features/2026-10-2*`, `2026-10-30-*`.
- Как работали: код уроков 11–14 писал агент по просьбе пользователя; редактор — Editor-скриптами агента (`Tools → UI → Build Shop Window`, `Build Player HUD`), запускает пользователь, агент проверяет по логу и YAML.
Следующий шаг:
- Урок 15 Addressables: начало — zip `...\15 Addressables\knowledge-is-power-master.zip`, итог — zip `...\16 InApps\knowledge-is-power-master.zip` (распаковывать во временную папку агента, без тяжёлых ассетов). Порядок: ветка → бриф в `Ai.Tasks/` с разделом «Компоненты: что и зачем» → план → «да» пользователя.
- Урок 16 InApps: итога в архиве нет (папка `17 Build` — только видео) — переносить по видео; покупки на ПК — через магазин платформы, вариант выбрать (`Ai.Decisions/2026-10-30-pc-first.md` § «Монетизация на ПК»).
Решения, которые действуют:
- Пока основная платформа — ПК (`Ai.Decisions/2026-10-30-pc-first.md`, `INTENT.md`); реклама — заглушка `StubAdsService`.
- «Что и зачем по каждому компоненту» — `<Knowledge>\process\agent-workflow.md` § 2 п. 11; разобрали — сразу записали в файлы.
- Перенос ассетов из `Resources` — «позже» (2026-10-26), план — `project-structure-review.md` § 6.
Известные риски / хвосты:
- Первый `SpawnMarker` на сцене — пустой id (брал id из префаба); `BasicLevel` пока со старым id; пользователь: «забей». Исправление: выделить маркер, сохранить сцену, Collect, Clear Prefs.
- Окна: при открытом окне ввод игрока не блокируется; повторный Shop открывает второе окно (как в курсе).
- Мобилка: нет `OnScreenStick` в `HUD/Mobile/JoyStickArea` (не срочно при ПК-приоритете).
- Коллегам перед клоном — Git LFS. Старые сохранения после смены формата — `Tools → Clear Prefs`.
- Длинные сессии расходуют много лимита — новая сессия на каждый урок.
```

### Только на local / self-hosted

- `Library/`, `Temp/`, `UserSettings/`, сгенерированные `*.csproj` / `*.sln`.
- Незакоммиченный WIP на `develop` (Enemy, Editor demo level, Resources, dirty scene/packages).
- Машинные пути в `.cursor/mcp.json` и курс на `G:\…`.
- Открытый Unity Editor / Play Mode / MCP `unity status → ready`.

---

## Указатель для новых репозиториев

Первичный setup нового проекта: **`<Knowledge>\starter-kit\README.md`** — только для **нового** проекта. Здесь всё уже настроено.
