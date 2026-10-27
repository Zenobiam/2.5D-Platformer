# Структура проекта: как сейчас и что поменять после курса

Статус: разбор и план; **полная перенастройка — после курса** (решение пользователя 2026-10-25). Сейчас только фиксируем проблемы и не двигаем ассеты, чтобы не ломать уроки. Сверено по файлам 2026-10-25 (ветка `feature/static-data`).

Общие правила — `<Knowledge>\conventions\unity-special-folders.md`, `project-structure.md`, `components-choice.md` (пишутся в Knowledge). Здесь — только применение к этому проекту.

## 1. Как сейчас

```
Assets/                      148 МБ всего почти целиком в Resources
  Game/            744 КБ    наш код и сцены
    Editor/                  Editor-скрипты (Tools → …), namespace Game.Editor
    Scenes/                  InitialScene, SampleScene; SampleScene/ — NavMesh-данные
    Scripts/                 Animation, BaseLogic, Camera, Constants, Core, Data, Enemy, Factories,
                             Infrastructure, Player, Providers, Services, StateMachine, StaticData, Triggers, UI
    Visuals/                 Materials/TestMaterial.mat, Models/ (пусто), UI/blank.png
  Resources/       148 МБ
    Effects/JMO Assets/      40 МБ  Cartoon FX Remaster (сторонний), с демо-сценой и asmdef
    Enemies/                 51 МБ  Enemy.prefab, Skeleton.prefab, EnemySpawner.prefab + Skeleton/ (FBX, анимации, контроллер)
    Environment/AssetHunts!/ 20 МБ  тайлы уровня (сторонний), с демо-сценой
    Prefabs/                 Player, HUD, GameBootstrapper, GameRunner, SaveTrigger, Curtain
    StaticData/              ассеты урока 09
    Yurowm/                  38 МБ  модель, анимации, контроллер игрока (сторонний), с демо-сценой
  Settings/                  URP-ассеты
  AgentScripts/              пусто (в .gitignore)
  TestObjects/               пусто
```

## 2. Проблемы

| # | Что | Почему плохо | Факт |
|---|---|---|---|
| 1 | **Сторонние пакеты целиком в `Resources`** (CFXR, AssetHunts, Yurowm, исходники Skeleton) | всё содержимое `Resources` попадает в билд, даже неиспользуемое: размер билда и индекс ассетов при старте растут. Код грузит по пути только `Prefabs/Player/Player` и `Prefabs/Player/HUD` (`AssetPath`), остальное доступно через ссылки в префабах и сценах — `Resources` для этого не нужен | 148 МБ в `Resources`; `Resources.Load` только в `AssetsProvider` |
| 2 | **Демо-сцены пакетов в `Resources`** | три лишние сцены и демо-скрипты в билде и в поиске | `CFXRF Demo.unity`, `Demo Scene 01.unity`, `Demo Cyberg Girl.unity` |
| 3 | **Своё и чужое вперемешку** | при обновлении пакета легко затереть свои файлы; непонятно, что наше | `Resources/Prefabs/Player/CFXR2 Blood Shape Splash.prefab` (эффект в папке игрока), `Resources/Enemies/Skeleton/FBX/CFXR4 Sword Hit PLAIN (Cross).prefab` (эффект в папке FBX), наши `Skeleton.controller`, `Damage.anim`, `Death.anim` рядом с исходником модели |
| 4 | **Мусор** | шум в Project, ложные находки при поиске | пустые `AgentScripts/`, `TestObjects/`, `Game/Visuals/Models/`; `NavMesh-DemoRoute.asset` — сцена ссылается только на `NavMesh-DemoRoute 1.asset`; `TestMaterial.mat` — ссылок в `Game/` и наших префабах не найдено |
| 5 | **Скрипты разложены смешанно** — по слою (`Core`, `Services`, `Factories`, `Providers`, `StateMachine`) и по фиче (`Player`, `Enemy`); спавнер в `BaseLogic/Enemy`, а враг в `Enemy/` | фичу «враг» приходится собирать из трёх папок | как в курсе (там тоже смешанно) — пока не трогаем |
| 6 | **Нет asmdef у своего кода** | всё в `Assembly-CSharp`: любая правка пересобирает всё; нельзя запретить рантайму ссылаться на Editor-код | у сторонних CFXR asmdef есть; для учебного проекта терпимо |

Что в порядке: сцены в `Game/Scenes`, NavMesh-данные рядом со сценой (так делает Unity), один `Game/Editor` с namespace `Game.Editor`, путь = namespace в `Game/Scripts`.

## 3. Целевая структура после курса (черновик)

```
Assets/
  Game/
    Scripts/      по фиче там, где фича самостоятельна (Player, Enemy, Spawners, UI), по слою — инфраструктура (Core, Services, StateMachine)
    Editor/
    Scenes/
    Prefabs/      свои префабы (Enemy + Variants, Player, UI)
    Data/         ScriptableObject-данные (StaticData)
    Art/          свои Animations, Controllers, Materials, Textures
  ThirdParty/     Yurowm, CartoonFX, AssetHunts, Skeleton — как пришли из Asset Store, без своих файлов внутри
  Resources/      только то, что грузится по пути (или ничего — после Addressables, урок 15)
  Settings/
```

| Вариант раскладки | Плюсы | Минусы |
|---|---|---|
| По типу (`Scripts/`, `Prefabs/`, `Materials/`) | привычно, как в курсе; легко найти «все префабы» | фича размазана по папкам; перенос фичи в другой проект — сбор по всему дереву |
| По фиче (`Enemy/{Scripts,Prefabs,Art}`) | фича в одном месте, легко переносить; asmdef на фичу | инфраструктура и общие ассеты всё равно нужны отдельно; дальше от курса |
| Смешанный (выше) | инфраструктура — по слою, игровые фичи — по фиче | нужно правило, что считать фичей |

## 4. Как переносить (когда дойдём)

1. Только в окне Project Unity, не в проводнике и не `git mv`: Unity сохраняет guid в `.meta`, ссылки в префабах и сценах не рвутся.
2. Один пакет или одна папка — один коммит; после каждого — открыть `SampleScene` и префабы, проверить Missing (Script/Prefab/Material) и Console.
3. Демо-сцены пакетов не удалять вслепую: сначала проверить, не тянет ли из них что-то наш уровень.
4. После переноса обновить `AssetPath`, `editor-setup.md`, `course-map.md`.

## 5. Можно раньше (по решению пользователя)

Удаление — только с подтверждения:

- пустые папки `Assets/TestObjects/`, `Assets/Game/Visuals/Models/` (`AgentScripts/` в `.gitignore`);
- `Assets/Game/Scenes/SampleScene/NavMesh-DemoRoute.asset` — на него не ссылается `SampleScene`;
- `Assets/Game/Visuals/Materials/TestMaterial.mat` — сначала проверить ссылки по всему проекту.

Всё остальное — после курса.

## 6. План переноса ассетов (готов, ждёт конца урока 10)

Решение пользователя 2026-10-26: переносить ассеты (без кода) **после закрытия урока 10**, агент напоминает при закрытии урока. Отвергнуто: «всё после курса» — переносить немного, а 194 МБ в `Resources` растут с каждым уроком; «сейчас» — посреди урока 10.

Строкой код грузит только `Prefabs/Player/Player`, `Prefabs/Player/HUD`, `Loot/Loot` (`AssetPath`) и `StaticData/Enemies` (`LoadAll`) — они остаются в `Resources`, runtime-код не меняется. Сверено 2026-10-26.

| Откуда (`Assets/…`) | Куда |
|---|---|
| `Resources/Yurowm/` | `ThirdParty/Yurowm/` |
| `Resources/Effects/JMO Assets/` | `ThirdParty/JMO Assets/` |
| `Resources/Environment/AssetHunts!/` | `ThirdParty/AssetHunts!/` |
| `Resources/Enemies/Skeleton/` (пак) | `ThirdParty/Skeleton/` |
| `Resources/Enemies/Golem/` (пак) | `ThirdParty/Golem/` |
| `Resources/Loot/Skull/` | `ThirdParty/Graveyard/` |
| наше внутри паков: `Skeleton/FBX/Skeleton.controller`, `Skeleton/Animations/Damage.anim`, `Death.anim`, `Golem/Golem.overrideController` | `Game/Prefabs/Enemies/Skeleton/`, `Game/Prefabs/Enemies/Golem/` |
| `Resources/Enemies/{Enemy,Skeleton,Golem,EnemySpawner}.prefab` | `Game/Prefabs/Enemies/` |
| `Resources/Loot/{PickupPopup.*,LootPickupFX,CFX3_MagicAura_D_Runic}` | `Game/Prefabs/Loot/` |
| `Resources/Prefabs/Player/CFXR2 Blood Shape Splash.prefab` | `Game/Prefabs/Player/` |
| `Resources/Prefabs/{Infrastructure,Triggers,UI}/` | `Game/Prefabs/{Infrastructure,Triggers,UI}/` |
| `Game/Visuals/` | `Game/Art/` |

Способ (рекомендация): Editor-скрипт `Tools → Setup → Restructure Assets` на `AssetDatabase.MoveAsset` (guid сохраняется, каждый шаг в лог, существующая цель — пропуск). Тем же изменением — пути-константы в Editor-скриптах (`EnemyVariantSetup`, `LootMaterialsSetup`, `PlayerAttackSetup`, `PlayerHudBuilder`, `SkeletonAnimationsSetup`, `BuildDemoLevel`, `DemoLevelBuilder`), `editor-setup.md`, `course-map.md`, `course-deviations.md`, путь `Resources/Yurowm` в `agent-playbook.md`. Перед переносом — закоммитить незакоммиченное; перенос — отдельным коммитом. Новые ассеты урока 10, появившиеся до переноса, — добавить в таблицу.
