# Контекст проекта: 2.5D Platformer

Канонический общий «мозг» для cloud Project-агентов и локальных чатов Cursor.  
Перед продолжением чужой работы — **прочитать**; после значимых решений или смены состояния — **обновить**.

---

## Цель

Unity-проект **2.5D Platformer** (репозиторий `Zenobiam/2.5D-Platformer`): платформер с перспективой ~2.5D, управление через New Input System, движение игрока на базе state machine + Rigidbody, рендер URP.  
Смысл этого файла — единый durable-контекст, чтобы cloud-агенты и локальные сессии продолжали работу друг друга без потери решений.

---

## Как пользоваться (cloud + local)

1. **Всегда начинать** с этого файла (`docs/project-context.md`), затем при необходимости читать исходники по путям ниже.
2. **Cloud Project agents** и **локальные workspace chats** обязаны читать и обновлять один и тот же файл в git — не дублировать правила в отдельных competing-системах.
3. **После сессии** кратко дописать:
   - что сделано / что осталось → **Текущее состояние**;
   - принятые решения → **Решения**;
   - что должен подхватить другой агент → **Handoff cloud↔local**.
4. Оригиналы правил (если появятся `.cursor/rules`, `AGENTS.md` и т.п.) **не удалять и не переписывать сюда целиком** — только краткое резюме + путь.
5. Project Agent Store (`notes.md`, `internal/`) — координация Project; **источник истины по коду и решениям** — этот файл в репозитории.

---

## Зафиксированные правила

На момент инвентаризации cloud-checkout **отдельных agent/project rule-файлов пользователя не найдено**.

| Что искали | Статус в checkout |
|---|---|
| `.cursor/**` (rules, skills) | отсутствует |
| `AGENTS.md` | отсутствует |
| `.cursorrules` | отсутствует |
| `*.mdc` | отсутствует |
| `CLAUDE.md` | отсутствует |
| `docs/**` (до этого файла) | отсутствовал |
| README | есть, почти пустой → [`README.md`](../README.md) |
| Rule/skill configs в репо | нет |

### Что есть в репо (не agent-rules, но полезный контекст)

| Путь | Кратко |
|---|---|
| [`README.md`](../README.md) | Заголовок `# 2.5D-Platformer`; указатель на этот файл |
| [`.gitignore`](../.gitignore) | Стандартный Unity gitignore (`Library/`, `Temp/`, `*.csproj`, `*.sln`, …) |
| [`ignore.conf`](../ignore.conf) | Plastic/Unity Version Control ignore (дублирует многое из gitignore) |
| [`.plastic/`](../.plastic/) | Следы Plastic SCM workspace (локальный VCS-контекст) |
| [`.vscode/settings.json`](../.vscode/settings.json) | excludes для Unity-артефактов; `dotnet.defaultSolution`: `2.5D Platformer.sln` |
| [`.vscode/extensions.json`](../.vscode/extensions.json) | рекомендации VS Code |
| [`.vscode/launch.json`](../.vscode/launch.json) | launch-конфиг |
| [`Packages/manifest.json`](../Packages/manifest.json) | URP 17, Input System, `com.unity.ide.cursor`, Rider/VS |
| [`ProjectSettings/ProjectVersion.txt`](../ProjectSettings/ProjectVersion.txt) | Unity **6000.0.46f1** |

При появлении новых правил: добавить строку в таблицу выше (резюме + путь), оригинал оставить на месте.

---

## Решения

Зафиксировано по коду и структуре репо (явных ADR/документов правил не было):

1. **Движок / пайплайн:** Unity 6 (`6000.0.46f1`), URP (`com.unity.render-pipelines.universal` 17.0.4).
2. **Ввод:** New Input System + asset [`Assets/PlayerControls.inputactions`](../Assets/PlayerControls.inputactions); обработчик [`PlayerInputController`](../Assets/Game/Scripts/Player/PlayerInputController.cs) (Move / Jump / Run / Crouch).
3. **Логика игрока:** иерархия состояний под [`Assets/Game/Scripts/State/`](../Assets/Game/Scripts/State/) (`PlayerStateController`, `PlayerStateFactory`, Idle/Walk/Run/Crouch/Jump/Air/Grounded/Death); старый [`PlayerMovement`](../Assets/Game/Scripts/Player/PlayerMovement.cs) по сути закомментирован / не активен.
4. **Физика:** 3D Rigidbody + BoxCollider; движение в плоскости через `Vector2` velocity; слой земли `Layers.Ground = 10` ([`Layers.cs`](../Assets/Game/Scripts/Enums/Layers.cs)).
5. **Камера:** [`CameraController`](../Assets/Game/Scripts/Camera/CameraController.cs) — есть TODO про follow / facing от `PlayerVisual`.
6. **Сцена:** основная сцена [`Assets/Game/Scenes/SampleScene.unity`](../Assets/Game/Scenes/SampleScene.unity).
7. **Общий контекст агентов:** канон — этот файл; не плодить параллельные rule-системы без нужды.

---

## Текущее состояние

_Плейсхолдер — обновлять в конце каждой значимой сессии._

- **Сделано:** базовый каркас движения / state machine игрока; Input Actions; URP settings; SampleScene.
- **В работе / неясно:** полнота префабов и моделей в git (см. handoff); активность `PlayerMovement` vs state machine; TODO камеры.
- **Следующий шаг:** _(заполнить агентом/человеком)_ 
- **Блокеры:** self-hosted worker `unity-pc` отмечен офлайн в Project notes — для Unity Editor / Play Mode нужна локальная машина или поднятый worker.

---

## Handoff cloud↔local

### Формат записи (копировать блок)

```text
### YYYY-MM-DD — кто (cloud|local) — кратко
Сделано:
Осталось:
Файлы:
Риски / не трогать:
```

### Активный handoff

```text
### 2026-09-16 — cloud — shared context bootstrap
Сделано:
- Инвентаризация agent-rules в cloud checkout: формальных правил нет.
- Создан канонический docs/project-context.md; указатель в README.md.
Осталось:
- Наполнять «Текущее состояние» и handoff по мере геймдев-работы.
- Сверить с self-hosted/local машиной наличие Library, префабов, моделей, UserSettings, .cursor rules, которых нет в этом checkout.
Файлы:
- docs/project-context.md
- README.md
Риски / не трогать:
- Не создавать вторую систему правил (.cursorrules / AGENTS.md с дублирующим текстом) без явной нужды — достаточно указателя сюда.
```

### Что может быть только на local / self-hosted (нет или неполно в cloud checkout)

- `Library/`, `Temp/`, `UserSettings/`, сгенерированные `*.csproj` / `*.sln` (в gitignore).
- Содержимое папок с одними `.meta` в git: `Assets/Game/Prefabs/`, `Assets/Game/Visuals/Models/`, `Assets/TestObjects/` — на машине с Unity могут лежать неотслеживаемые или ещё не закоммиченные ассеты.
- Локальные Cursor rules/skills вне репо (user/team rules в IDE).
- Plastic SCM / Unity Version Control workspace state (`.plastic/`, `ignore.conf`).
- Запущенный **self-hosted agent worker** (`unity-pc` в Project notes — офлайн, нужен `agent worker start`).
- Открытый Editor, Play Mode, сцены с локальными override в `UserSettings`.
