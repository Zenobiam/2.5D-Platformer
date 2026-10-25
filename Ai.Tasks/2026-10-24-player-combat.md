# 2026-10-24 — Бой игрока: атака, урон, смерть (handoff)

Статус: почти готово — бой с ПК и смерть врага проверены в Play Mode; осталось проверить экранную кнопку, смерть игрока во время удара и закоммитить (см. «Open (актуально)»)
Исполнитель: координатор (Opus) + `unity-reader` (Haiku, `local_d6a7af1a-2068-4e0f-8011-33108e0fc865`) для команд Unity CLI
Ветка: `cursor/enemy-follow-aggro-ba30` (есть незакоммиченные изменения, коммитов ещё не было)

## Goal

Довести бой игрока по курсу (уроки 6–7, мобильная разработка): атака игрока по кнопке, урон врагу через `IHealth`, полоска HP врага, смерть игрока без падения сквозь уровень.

## Constraints

- Идём по курсу. Не вводить Zenject/VContainer и новые интерфейсы сверх курса без просьбы (см. `Ai.Context/preferences.md`).
- Курс мобильный: нельзя опираться только на мышь и клавиатуру; экранные кнопки и производительность важны.
- Не коммитить без явной просьбы пользователя.
- Не трогать WIP Enemy/Aggro другого агента без согласования.

## Paths

- Игрок: `Assets/Game/Scripts/Player/` (`PlayerAttack.cs`, `PlayerAnimator.cs`, `PlayerFacing.cs`, `PlayerHealth.cs`, `PlayerDeath.cs`, `PlayerMovementController.cs`)
- Враг: `Assets/Game/Scripts/Enemy/` (`EnemyHealth.cs`, `EnemyAnimator.cs`, `EnemyAttack.cs`, `IHealth.cs` — интерфейс пользователя)
- Ввод: `Assets/Game/Scripts/Services/Input/` и `Assets/PlayerControls.inputactions`
- HUD: `Assets/Game/Scripts/UI/` и `Assets/Game/Editor/PlayerHudBuilder.cs`
- Анимации: `Assets/Resources/Yurowm/Animations/Hit.FBX`, `Assets/Resources/Yurowm/Controllers/FreeHandController.controller`, `Assets/Resources/Enemies/Skeleton/`

## DoD

1. Игрок атакует по кнопке (пока мышь в редакторе, потом экранная кнопка) и проигрывает клип `Hit`.
2. Событие `OnAttack` на кадре 0:05 находит врагов в зоне перед игроком и вызывает `TakeDamage` у `IHealth` врага.
3. Полоска HP врага (или HUD) меняется при ударе.
4. Смерть игрока: анимация `Death`, игрок не падает сквозь уровень, модель видна.
5. Консоль без ошибок в Play Mode (включая `function not found` и `Parameter does not exist`).

## Done (уже сделано и проверено в Play Mode)

- Урон игроку работает: два удара по 10 HP, 100 → 90 → 80, полоска HUD меняется (`fill` 0.9 → 0.8).
- Анимация получения урона: триггер `Damage` из `FreeHandController` (клипы `Damage1–3`).
- `PlayerVisual` разбит на `PlayerFacing` (поворот) и `PlayerAnimator` (`Speed`, `Damage`, `Death`, `Attack`, `IsAttacking`). Старый `PlayerVisual.cs` удалён; ссылки в префабе назначены.
- Слой `Player` у игрока; слой `Hittable` (индекс 15) создан пользователем.
- Смерть: Rigidbody переводится в kinematic (`PlayerDeath.Die`), падение устранено.
- Событие `OnAttack` добавлено в `Hit.FBX.meta` на кадре 5 (0.1667 с).
- Действие `Attack` в `PlayerControls.inputactions` (левая кнопка мыши), `InputReceiver.OnAttack`, `IInputService.AttackButtonUp()`, `InputService.AttackButtonUp()`.
- `PlayerAttack` на `CyborgGirl`: `Update` → `PlayAttack` по отпусканию кнопки, `OnAttack` → `Hit()` + урон через `IHealth.TakeDamage`.
- `EnemyAnimator.PlayHit()` (бывший `PlayDamage`) вызывается из `EnemyHealth.TakeDamage`.
- `IHealth` — интерфейс пользователя в `Game.Scripts.Enemy`; мой дубликат удалён; `PlayerHealth` и `ActorUI` используют его.
- Пакет `com.unity.pipeline` обновлён до `0.8.0-exp.1` (из реестра). Старая встроенная копия 0.7.0-exp.1 перенесена в `D:\Unity\Backups\com.unity.pipeline-0.7.0-exp.1` (в git это удаление).
- Временные логи `[TEMP-DEBUG]` удалены.

## Open (актуально, 2026-10-24 вечер)

1. **Экранная кнопка атаки в Simulator** — кнопка видна; удар кнопкой после `Tools → Setup → Player Attack` не проверен. Play Mode запускать из вкладки Simulator.
2. **Смерть игрока во время удара** — исправлено (`PlayerAnimator.PlayDeath` сбрасывает `Attack`), в Play Mode не проверено.
3. **Полоска HP Skeleton** — `blank.png` скопирован в `Assets/Game/Visuals/UI/`; пользователь настраивает Image (Filled) и `HpBar.ImageCurrent` в префабе; `HpCamera` повесить на Canvas полоски. Подтвердить, что полоска убывает без `NullReferenceException`.
   - **Проверено по файлу 2026-10-25** (`Enemy.prefab`): `Canvas` World Space + `HpCamera`, `HealthBar/HpBar.ImageCurrent` назначен, `Current` — Image Filled (Horizontal). В Play Mode не проверено.
4. **Анимации Skeleton** — временные клипы `Damage.anim` (наклон 20°) и `Death.anim` (падение 90°) по узлу `DeformationSystem` из `Tools → Setup → Skeleton Animations`; проверено в Play Mode. Состояние `Death` → `Death.anim` (`death_test` не используется). Write Defaults у `Death`/`Damage` выключен. Настоящие клипы под риг Skeleton — отдельная задача на ассеты.
5. **Commit** — разобрать `git status` по `Knowledge/conventions/git-hygiene.md`, коммиты по задачам, только по просьбе пользователя.

## Исходный список Open (закрыт, см. «Итог»)

1. **Коллайдер тела у `Skeleton`.** Сейчас только два триггера (`AttackZone`, `AggroZone`). Добавить `CapsuleCollider` (не trigger) на корень `Skeleton`, слой `Hittable`, без изменения дочерних объектов.
2. **`PlayerAttack.Hittable Mask`** в Inspector на `CyborgGirl` выставить `Hittable`. Проверить, что в `PlayerAttack` поле маски назначено (сейчас в коде слой берётся через `LayerMask.GetMask("Hittable")`).
3. **Состояние `Hit` в `FreeHandController`.** Создать состояние с клипом `Hit` и переход по триггеру `Attack` (сейчас `Attack` ведёт в `Fire`, стрельба). Имя состояния должно быть `Hit`, иначе `PlayerAnimator.IsAttacking` не сработает.
4. **Проверить `EnemyHealth`:** по коду он уже реализует `IHealth` (пользовательский интерфейс). Сверить, что `Current`/`Max` — свойства с `get; set`, а не поля, и что `TakeDamage` работает (после удара полоска врага меняется).
5. **Мобильный ввод.** Экранная кнопка атаки в HUD, которая вызывает `InputReceiver.OnAttack`-подобное нажатие; экранные кнопки движения/прыжка по `JoystickArea`. Мышь оставить только для редактора.
6. **Удалить `using UnityEditor;`** из `Assets/Game/Scripts/Enemy/EnemyAttack.cs` (в runtime-коде ломает сборку).
7. **Проверка смерти.** Сверить, что после фикса kinematic модель не исчезает (если исчезает, проверить `Update When Offscreen` у `SkinnedMeshRenderer` на `CyborgGirl` и `Glass`).
8. **Commit.** Разобрать `git status`: убрать из индекса сгенерированные файлы (см. `Knowledge/conventions/git-hygiene.md`), закоммитить по задачам отдельными коммитами.

## Не трогать

- Логику save/load и DI без просьбы.
- Enemy/Aggro WIP другого агента.
- `Assets/Resources/Yurowm/*` (ассеты пакета) — только чтение; события добавлять через импорт FBX.

## Проверка

- `unity command console` и `unity command console_status` после Play Mode (читать `[TEMP-DEBUG]`, ошибки CS, `function not found`, `Parameter ... does not exist`).
- `unity command get_scene_hierarchy` для проверки объектов.
- Ручная проверка в Play Mode: удар по врагу, полоска, смерть, экранная кнопка (после появления).

## Итог

### 2026-10-24 — сессия координатора (Opus)

**Проверено в Play Mode пользователем:** удар мышью (Game) проигрывает `Hit`, `OnAttack` бьёт Skeleton (`hits=1`), враг умирает: падает, не преследует, исчезает через 3 с. Ошибок компиляции нет.

**Исходный Open:** п.1 (HitBox), п.4 (`EnemyHealth`), п.6 (`using UnityEditor`) — уже были сделаны пользователем; п.2 — маска задаётся в коде; п.3 — сделан скриптом; п.5 — кнопка атаки сделана; п.7 — смерть игрока работала, доп. фикс при атаке; п.8 — открыт.

**Код**
- `PlayerAttack`: маска `1 << LayerMask.NameToLayer("Hittable")` (была `GetMask` со сдвигом → слой Default); убраны `[SerializeField]` маски и неиспользуемый `_radius`; `StartPoint().y` от позиции игрока (уровень на y≈2, курсовая формула уводила сферу под пол); `PhysicsDebug.DrawDebug` как в курсе.
- `PlayerAnimator.PlayDeath`: `ResetTrigger(Attack)` — отложенный Attack через `Any State → Hit` уводил из Death.
- `EnemyAnimator.PlayDeath`: сброс `LeftAttack/RightAttack/SplashAttack/Damage` (у Skeleton всё из Any State).
- `EnemyDeath`: `OnDeath?.Invoke()`, `Destroy(gameObject)` через 3 с (как в курсе); `StopBehaviour()` — выключает `EnemyAttack`, все `Follow`, `AnimateAlongAgent`, `NavMeshAgent`, коллайдеры детей. Файлы WIP Aggro/Follow не менялись.
- UI: `MobileOnly` (группа `Mobile` по `UnityEngine.Device.Application.isMobilePlatform`), `HpCamera` (поворот полоски к камере, курсовой `LookAtCamera`).

**Editor-скрипты**
- `PlayerHudBuilder` — группа `Mobile`: `AttackButton` (`OnScreenButton` → `<Gamepad>/buttonWest`), перенос `JoyStickArea`.
- `SkeletonAnimationsSetup` (**Tools → Setup → Skeleton Animations**) — кривые `Damage.anim`/`Death.anim` по `DeformationSystem` от позы из `Skeleton.fbx`, `Death` → `Death.anim`.
- `EnemyPrefabsBuilder` (разовый, после использования удалён вместе с исходным `Base Enemy.prefab`) — разделил врага как в курсе (Base + Lich): `Assets/Resources/Enemies/Enemy.prefab` (логика, Animator без Avatar, зоны, HitBox, HP) и Variant `Assets/Resources/Enemies/Skeleton.prefab` (кости и меш прямо под корнем, Avatar + `Skeleton.controller`). Новый враг — Variant от `Enemy.prefab`. Проверено в Play Mode, скелет в `SampleScene` заменён.
- `PlayerAttackSetup` (**Tools → Setup → Player Attack**) — событие `Player/Attack` → `InputReceiver.OnAttack` в `PlayerInput` (`InputHandler`); состояние `Hit` и переходы `Any State → Hit` (Attack), `Hit → Idle` (Exit Time 0.9) в `FreeHandController`.

**Ассеты / ввод**
- `PlayerControls.inputactions`: `Attack ← <Gamepad>/buttonWest` (группа Gamepad) — экранная кнопка и геймпад.
- `Hit.FBX.meta`: событие `OnAttack` на 0.2125 (было `NewEvent` на 0).
- `Assets/Game/Visuals/UI/blank.png` (+ .meta) из курса, урок 06 — только для полоски врага; у игрока `UISprite`.

**Документы:** `Ai.Context/agent-playbook.md` (сессии и Unity-worker, формат отчёта), `Ai.Context/preferences.md` + `.cursor/rules/course-reference.mdc` (архитектура из курса, API Unity 6), `Ai.Architecture/course-deviations.md`, `Ai.Issues/bugs/2026-10-24-player-attack-not-firing.md`.

**Предложено, не сделано:** события клипов Editor-скриптом (`ModelImporter.clipAnimations`); сгенерированный `PlayerControls` вместо `PlayerInput` + `InputReceiver`; `EditorSceneManager.playModeStartScene` = `InitialScene`.
