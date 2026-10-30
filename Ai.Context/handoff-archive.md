# Архив handoff

Предыдущие записи из `project-context.md` § 6, новые сверху. Исторические записи не переписываются: имена и пути в них — на момент записи.

```text
### 2026-10-30 — пользователь + координатор (Opus, local) — урок 11 Spawner rework, старт урока 12 UI
Сделано:
- Урок 11 (код — агент по просьбе, редактор — пользователь), ветка `feature/spawner-rework` (`e066983`, `5591207`, запушено): `LevelStaticData` + Collect, `SpawnMarker`/`SpawnPoint`, `CreateSpawner`, `Register` private, `SerializableDictionary`, `UniqueId` → `BaseLogic/`; старый `EnemySpawner` удалён. **В Play Mode не проверен.**
- Урок 12: ветка `feature/ui`; паки UI в `Assets/ThirdParty/` (`Simple Fantasy GUI` совпадает с курсом по guid); пакет `com.unity.2d.sprite` (Sprite Editor); бриф `Ai.Tasks/2026-10-30-ui-shop-layout.md` (иерархия, якоря, спрайты, «Компоненты: что и зачем»), макет показан; сборка UI — Editor-скриптом агента (ждёт «да»).
- Правило «что и зачем по каждому компоненту» — в `<Knowledge>\process\agent-workflow.md` § 2 п. 11. Knowledge: выводы урока 11 (prototyping, DI Construct/ServiceLocator, данные уровня), `mechanics/loot.md`, `conventions/unity-input.md`, частицы/рендер, AssetRipper и инструменты декомпиляции.
Осталось:
- Урок 11: проверен пользователем в Play Mode (2026-10-30, со слов); `GolemData` `MinLoot/MaxLoot` 10/20, `Id` в `SpawnMarker.prefab` очищен (не закоммичено). **Известный риск (пользователь: «забей», 2026-10-30):** первый `SpawnMarker` на сцене брал id из префаба — теперь пустой; игра пока берёт старый id из `BasicLevel`, но следующий Collect запишет пустой id. Исправление: выделить маркер (id сгенерируется), сохранить сцену, Collect, Clear Prefs.
- Уроки 12–14 **закрыты** (2026-10-30): окно магазина, окна в игре, реклама за вознаграждение на заглушке `StubAdsService` (ветка `feature/rewarded-ads`). Дальше — урок 15 Addressables: начало — zip `15 Addressables`, итог — zip `16 InApps`. Урок 16 (покупки): на ПК возможны через магазин платформы — выбрать вариант по `Ai.Decisions/2026-10-30-pc-first.md` § «Монетизация на ПК».
- **Мобилка:** в `HUD/Mobile/JoyStickArea` нет `OnScreenStick` — на телефоне нечем ходить; нет кнопок прыжка/бега/приседа. Решение пользователя.
- Перенос ассетов из `Resources` — «позже» (решение 2026-10-26), `project-structure-review.md` § 6.
Риски / не трогать:
- Коллегам перед клоном — Git LFS. Старые сохранения — `Tools → Clear Prefs`.

### 2026-10-27 — пользователь + координатор (Opus, local) — урок 10 Loot
Сделано:
- Урок 10 (код пользователя + агент по просьбам): `EnemyLootSpawner` и `LootPiece` в `BaseLogic/LootLogic/` (папка не `Loot`: namespace перекрывал класс `Data.Loot`), `Loot`/`LootData`, `IRandomService`/`RandomService` (в видео не пишется), `GameFactory.CreateLoot` и параметры `IRandomService`, `IPersistentProgressService` (регистрация в `BootstrapState` до фабрики), `EnemyDeath.Died` (было `DeathAction`).
- Счётчик душ на HUD: `UI/LootCounter` (+ отписка в `OnDestroy`), `HUD/LootCounter` через `Tools → UI → Build Player HUD`; `WorldData` создаёт `LootData` (был NRE на новом прогрессе).
- Сохранение лута на сцене по курсу (`LootPiecesOnScene`, `DictionaryData_Base`, `UniqueId.GenerateId`, `InitLootPieces`); души сохраняются сразу при подборе — отступление, решение пользователя. Ассеты лута из курса, `Tools → Setup → Loot Materials to URP`. Всё проверено пользователем в Play Mode.
- Документы: `course-map.md`, `course-deviations.md`, `editor-setup.md`, `Ai.Issues/features/2026-10-27-loot-*`; Knowledge: `/// <summary>` для описаний, скиллы плагинов Unity/unity-perf.
Осталось:
- **НАПОМНИТЬ (урок 10 закрыт):** перенос ассетов из `Resources` в `Game/Prefabs` и `ThirdParty/` — `Ai.Architecture/project-structure-review.md` § 6. Напомнено 2026-10-27, пользователь: «позже» — снова не напоминать без повода; предложить при следующей задаче со структурой или ассетами.
- Урок 11 Spawner rework: начало — `...\11 Spawner rework\knowledge-is-power-master\…\CodeBase`, итог — `...\12 UI part 1\…` (zip). Курс переносит спавнер в `Logic/EnemySpawners/`.
- `GolemData`: нет `MinLoot`/`MaxLoot` (0/0 → лут со значением 0); спавнер в `SampleScene` переключён на Skeleton.
- `EnemySpawner`/`UniqueId` в `BaseLogic/EnemySpawner/` при namespace `….BaseLogic.Enemy`; `UniqueIdEditor` — `FindObjectsOfType` (`CS0618`); `PlayerHudBuilder` — `?? AddComponent` в старом коде; Rigidbody на `Player` — оставлен (решение 2026-10-26).
Файлы: `Ai.Tasks/2026-10-27-loot-hud.md`, `2026-10-27-loot-save.md` (локально).
Риски / не трогать:
- Коллегам перед клоном — установить Git LFS. Старые сохранения без `LootPiecesOnScene` — `Tools → Clear Prefs`. Уроки делает пользователь, агент помогает по просьбе.

### 2026-10-25 (вечер) — пользователь + координатор (Opus, local) — урок 09 Static data + враг Golem
Сделано:
- Урок 09 (код пользователя, рефакторинг компонентов врага — агент): `EnemyStaticData` (ScriptableObject), `EnemyTypeId` (`Skeleton = 0`, `Golem = 10`), `IStaticDataService`/`StaticDataService` (`LoadAll` из `Resources/StaticData/Enemies`, регистрация в `BootstrapState` **до** `GameFactory`), `GameFactory.CreateEnemy`; `AgentMoveToPlayer`, `EnemyAttack`, `RotateToPlayer` — `Construct` вместо `PlayerCreated`; `PlayerGameObject`/`PlayerCreated` убраны из фабрики.
- Враг Golem: пак из курса в `Resources/Enemies/Golem` (PNG пересжаты без потерь 97 → 21 МБ), Git LFS для бинарников, `Tools → Setup → Enemy Variant: Golem` (override на базе `Skeleton.controller`, Variant от `Enemy`, Exit Time атак = 1). Урон и остановка проверены пользователем в Play Mode.
- Фикс направления игрока (`PlayerFacing.Direction`), решения: `Ai.Decisions/2026-10-25-*`; документы: правило «Редактор — часть задачи», `editor-setup.md`, `project-structure-review.md`, Knowledge `mechanics/new-enemy.md`.
Осталось:
- Урок 10 Loot: начало — `...\10 Loot\knowledge-is-power-master\…\CodeBase`, итог — `...\11 Spawner rework\knowledge-is-power-master\…\CodeBase` (есть и zip). Ассеты лута из курса уже перенесены (2026-10-25): `Resources/Loot` (префабы с Missing Script до кода урока), `Resources/Effects/JMO Assets/Cartoon FX` (часть legacy), `Tools → Setup → Loot Materials to URP` — запущен, материалы на URP.
- **НАПОМНИТЬ пользователю после закрытия урока 10:** перенос ассетов из `Resources` в `Game/Prefabs` и `ThirdParty/` — карта и способ в `Ai.Architecture/project-structure-review.md` § 6 (решение пользователя 2026-10-26: «перенос позже, напомни когда завершим урок»).
- `EnemySpawner`/`UniqueId` в папке `BaseLogic/EnemySpawner/`, а namespace `Game.Scripts.BaseLogic.Enemy` — путь ≠ namespace (решение пользователя).
- `UniqueIdEditor`: устаревший `FindObjectsOfType` (`CS0618`). Игрок может зайти в любого врага (нет коллайдера тела).
- Хвосты боя: кнопка атаки в Simulator, смерть игрока во время удара; `Tools → Build Demo Level Geometry` не запускать.
Файлы: `Ai.Tasks/2026-10-25-static-data.md` (локально), `Ai.Architecture/course-map.md`, `editor-setup.md`, `course-deviations.md`.
Не закоммичено на конец сессии: исправления ссылки на `new-enemy.md` (`agent-playbook.md`, `editor-setup.md`) и `RotateToHero` → `RotateToPlayer` в списке WIP (`AGENTS.md`, `agent-playbook.md`), эта строка; `ProjectSettings/TagManager.asset` — новый слой `Pickup` (индекс 16, как в курсе) (пользователь, урок 10). Брифы `Ai.Tasks/*` — локально, по решению пользователя.
Риски / не трогать:
- Коллегам перед клоном — установить Git LFS. Уроки делает пользователь, агент помогает по просьбе.

### 2026-10-25 — пользователь + координатор (Opus, local) — урок 08 Enemy Spawner
Сделано:
- Урок 08 перенесён пользователем (коммит `b01d2a7`, ветка `feature/enemy-spawner`): `EnemySpawner` (`ISaveProgress`, `KillData.ClearedSpawners`), `UniqueId` + `UniqueIdEditor`, гизмо `EnemySpawnerEditor`, `MonsterTypeId` (`Skeleton = 0`), `GameFactory.Register` публичный и в `IGameFactory`, `InitSpawners()` в `LoadSceneState` (переименован из `LoadLevelState`).
- Редактор: тег `EnemySpawner`, префаб `Resources/Enemies/EnemySpawner.prefab` (Id пустой), один спавнер в `SampleScene` (`SampleScene_<guid>`), `SaveTrigger` придвинут (x 3.79). Проверено по файлам; сборка успешна по `Editor.log`.
- Документы: правило «Редактор — часть задачи», `Ai.Architecture/editor-setup.md`, карта курса и отступления обновлены.
Осталось:
- `Spawn()` пустой, `isEmpty` никто не ставит — урок 09 (Static data) и 11 (Spawner rework).
- Скелет пока стоит на сцене руками; хак `ActorUI.Start` остаётся до спавна через фабрику.
- `UniqueIdEditor` использует устаревший `FindObjectsOfType` (`CS0618`) — заменить по решению пользователя.
- Хвосты боя: кнопка атаки в Simulator, смерть игрока во время удара (Play Mode).
Файлы: `Ai.Architecture/editor-setup.md`, `Ai.Architecture/course-map.md`, бриф `Ai.Tasks/2026-10-24-enemy-spawner.md` (локально, не в git).
Риски / не трогать:
- `Tools → Build Demo Level Geometry` не запускать — удаляет `NavMeshSurface`.
- WIP Enemy/Aggro не трогать; уроки делает пользователь, агент помогает по просьбе.

### 2026-10-24 (вечер) — координатор (Opus, local) — бой игрока доведён
Сделано:
- Удар с ПК работает и бьёт Skeleton; враг умирает (падает, стоит, исчезает через 3 с). Проверено пользователем в Play Mode.
- Мобильный HUD: группа `Mobile` (`MobileOnly`), кнопка атаки `OnScreenButton` → `<Gamepad>/buttonWest` → `Attack`.
- `Tools → Setup → Player Attack`: событие Attack в `PlayerInput`, состояние `Hit` в `FreeHandController`.
- Фиксы: маска Hittable, высота точки удара (уровень на y≈2), смерть игрока/врага не срывается триггерами из Any State, `EnemyDeath` останавливает врага и удаляет через 3 с.
Осталось:
- Проверить кнопку в Simulator и смерть игрока во время удара; полоска HP Skeleton (`blank`, `HpBar.ImageCurrent`, `HpCamera`).
- Заглушки анимаций Skeleton (`death_test`, `Damage`) заменить настоящими клипами.
- Коммиты по задачам — по просьбе пользователя.
Файлы: `Ai.Tasks/2026-10-24-player-combat.md` (Итог), `Ai.Issues/bugs/2026-10-24-player-attack-not-firing.md`, `Ai.Architecture/course-deviations.md`.
Риски / не трогать:
- WIP Enemy/Aggro (`Aggro`, `Follow`, `AgentMoveToPlayer` и др.) не менялся — только выключается из `EnemyDeath`.
- Unity CLI — через свою связанную сессию-worker; чужой worker (сессии «Main old») не использовать.

### 2026-10-24 — координатор (Haiku) — бой игрока, handoff
Сделано:
- Урон игроку работает (слой `Player` на префабе, триггер `Damage`). Полоска HUD (`PlayerHudBuilder`, спрайт `UISprite`, 300×40, отступ 30/−40).
- `PlayerVisual` разбит на `PlayerFacing` и `PlayerAnimator`, старый класс удалён, ссылки в префабе назначены.
- Смерть: Rigidbody kinematic при смерти (падение устранено).
- Ввод: действие `Attack` (мышь для редактора), `InputReceiver.OnAttack`, `IInputService.AttackButtonUp()`.
- `PlayerAttack` и событие `OnAttack` на кадре 5 клипа `Hit` (`Hit.FBX.meta`).
- `IHealth` — интерфейс пользователя (`Game.Scripts.Enemy`); мой дубликат удалён.
- Пакет `com.unity.pipeline` 0.8.0-exp.1 (реестр); старая копия в `D:\Unity\Backups`.
Осталось:
- Коллайдер тела у `Skeleton` (слой `Hittable`), маска `Hittable` в `PlayerAttack`.
- Состояние `Hit` с переходом по триггеру `Attack` в `FreeHandController` (сейчас `Attack` → `Fire`).
- Мобильная экранная кнопка атаки, урон по врагам (TODO), `using UnityEditor;` в `EnemyAttack.cs`.
- Коммиты: разобрать `git status` (сгенерированные файлы), закоммитить по задачам.
Файлы: `Ai.Tasks/2026-10-24-player-combat.md`, `Ai.Issues/bugs/2026-10-24-*`, `Ai.Issues/features/2026-10-24-player-attack-input.md`.
Риски / не трогать:
- Не трогать WIP Enemy/Aggro другого агента. Не коммитить без просьбы.

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
