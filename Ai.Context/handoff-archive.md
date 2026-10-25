# Архив handoff

Предыдущие записи из `project-context.md` § 6, новые сверху. Исторические записи не переписываются: имена и пути в них — на момент записи.

```text
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
