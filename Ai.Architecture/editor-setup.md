# Что настроено в редакторе

Справочник того, что живёт не в коде, а в Unity. Сверено по YAML 2026-10-25 (ветка `feature/enemy-spawner`, `e045cb6`; урок 08 — `b01d2a7`): `ProjectSettings/*.asset`, `.prefab`, `.unity`, `.controller`. Обновлять тем же изменением, что и задачу (`Ai.Context/README.md` § «Что обновить»). Заменяет обзор [`gameobject-architecture-notes.md`](gameobject-architecture-notes.md) (2026-09-20).

Как читать: «зачем» — что делает код с этой настройкой; «если забыть» — как ломается (часто молча).

## 1. Настройки проекта

### Теги (`ProjectSettings/TagManager.asset`)

| Тег | Где стоит | Кто ищет | Если забыть |
|---|---|---|---|
| `InitialPoint` | `SampleScene` → `PlayerHandler` | `LoadSceneState`: `FindWithTag` → точка создания игрока | `NullReferenceException` при загрузке уровня |
| `EnemySpawner` | префаб `EnemySpawner` и его экземпляры на сцене | `LoadSceneState.InitSpawners()`: `FindGameObjectsWithTag` → `GameFactory.Register` | спавнер молча не регистрируется в прогрессе; если тега нет в проекте — `UnityException: Tag ... is not defined` |
| `LoadingCurtain` | префаб `Curtain` | — (тег курса, кодом не ищется) | — |
| `Player` (встроенный) | корень префаба `Player` | — | — |
| `Player Modell` | нигде (проверено по префабам и сценам) | — | лишний тег |
| `MainCamera` (встроенный) | `SampleScene`, `InitialScene` → `Main Camera` | `Camera.main` в `LoadSceneState.CameraFollow` | камера не следует за игроком |

### Слои и матрица коллизий (`TagManager.asset`, `DynamicsManager.asset`)

| # | Слой | Где стоит | Зачем |
|---|---|---|---|
| 10 | `Ground` | `Level`, `Level/DemoRoute/Ground` | земля уровня |
| 12 | `Player` | корень `Player` и коллайдеры | маска `Player` в `EnemyAttack` (`OverlapSphereNonAlloc`) — враг находит игрока |
| 13 | `Aggro` | **нигде** | — |
| 14 | `AttackZone` | **нигде** | — |
| 15 | `Hittable` | `Enemy/HitBox` (`BoxCollider`, не trigger) | маска `Hittable` в `PlayerAttack` — игрок находит врага |
| 16 | `Pickup` | корень `Loot/Loot` и `Skull_damaged` (индекс как в курсе, добавил пользователь 2026-10-25) | лут урока 10 |

Матрица: `Aggro` и `AttackZone` сталкиваются **только** с `Player`; `Hittable` — ни с чем (физическим запросам `Overlap*` матрица не нужна, они берут маску из кода). Остальные слои — по умолчанию.

> Расхождение: слои `Aggro` и `AttackZone` и матрица под них заведены (коммит `087fd9c`), но объекты `Enemy/AggroZone` и `Enemy/AttackZone` стоят на `Default`. Работает и так (`Default` сталкивается с `Player`), но триггеры зон ловят и всё остальное на `Default`. Решение — за пользователем.

### Сцены в билде (`EditorBuildSettings.asset`)

`InitialScene` (0) → `SampleScene` (1). Play Mode с `SampleScene` тоже работает: `GameRunner` на сцене создаёт bootstrapper.

## 2. Сцены

### `Assets/Game/Scenes/InitialScene.unity`

| Объект | Компоненты | Зачем |
|---|---|---|
| `Main Camera` | Camera, URP camera data | сцене нужна камера |
| `GameRunner` (префаб) | `GameRunner` | создаёт `GameBootstrapper` → `GameMain` → `GameStateMachine` |

### `Assets/Game/Scenes/SampleScene.unity`

| Объект | Компоненты, тег, слой | Зачем | Как сделано |
|---|---|---|---|
| `PlayerHandler` | только Transform, тег `InitialPoint` | точка появления игрока | руками |
| `Main Camera` | Camera, `CameraController` | `LoadSceneState` вызывает `Follow(player)` | руками |
| `InputHandler` | `PlayerInput` (Invoke Unity Events, карта `Player`), `InputReceiver` | события `Move/Jump/Run/Crouch/Attack` → `InputReceiver.OnMove/OnJump/OnRun/OnCrouch/OnAttack` | руками; `Attack` — `Tools → Setup → Player Attack` |
| `Level` → `DemoRoute` | `NavMeshSurface`; дети `Ground` (слой `Ground`), `Hazards`, `Decor` — `NavMeshModifier` | геометрия и навигация для врагов | геометрия — `Tools → Build Demo Level Geometry`; NavMesh — руками |
| `EnemySpawner` (префаб, x ≈ 5.5) | тег `EnemySpawner`, `EnemySpawner` (`EnemyTypeId = Skeleton`; в уроке 10 переключён с Golem обратно на Skeleton — у Golem нет `MinLoot/MaxLoot`), `UniqueId` (`Id = SampleScene_<guid>`) | точка спавна врага и запись «очищен» в прогрессе; с урока 09 создаёт врага | руками (урок 08) |
| `Skeleton` (префаб `Skeleton.prefab`) | враг | пока стоит на сцене руками; с урока 09 его создаёт спавнер | руками |
| `SaveTrigger` (префаб, x = 3.79) | `BoxCollider` trigger, `SaveTrigger` | сохранение прогресса при входе; придвинут к старту в уроке 08 | руками |
| `GameRunner` (префаб) | `GameRunner` | старт из `SampleScene` без `InitialScene` | руками |
| `Global Volume`, `Directional Light`, `EventSystem` (`InputSystemUIInputModule`) | стандарт URP / Input System | — | — |
| `-------Managers-----------`, `--------Core----------`, `-----------------------` | пустые | разделители в иерархии | — |

Игрок и HUD на сцене **не лежат**: их создаёт `GameFactory` из `Resources` в `LoadSceneState`.

## 3. Префабы

| Префаб (`Assets/Resources/…`) | Устройство | Зачем / неочевидное |
|---|---|---|
| `Prefabs/Player/Player` | корень: слой `Player`, `CharacterController`, `Rigidbody`, `PlayerMovementController`, `PlayerHealth`, `PlayerDeath`, `PlayerFacing`; модель — вложенный префаб `Yurowm/…/CyborgGirl` (на нём `Animator`, `PlayerAnimator`, `PlayerAttack`) | путь — `AssetPath.PlayerPath`; при смерти `Rigidbody` → kinematic |
| `Prefabs/Player/HUD` | Canvas, `ActorUI`; `HpBar` (`Background`, `Fill`); группа `Mobile` (`MobileOnly`): `JoyStickArea`, `AttackButton` (`OnScreenButton` → `<Gamepad>/buttonWest`); `LootCounter` справа сверху (`HorizontalLayoutGroup` управляет шириной детей; `LootCounter`; `Icon` — `Image` с `CFX3_T_Skull.png` + `LayoutElement` 100×100, `Counter` — `TextMeshProUGUI`, `LiberationSans SDF`, без переноса — длинное число растёт влево) — урок 10, 2026-10-27; цвет, размер и текст `Counter` и цвет `Icon` скрипт задаёт только при создании, правки в Inspector сохраняются | собирается `Tools → UI → Build Player HUD`; корень должен быть **активен**, иначе `GetComponentInChildren<ActorUI>()` = null |
| `Enemies/Enemy` | **логика врага без модели**: `Animator` (без Avatar), `EnemyAnimator`, `NavMeshAgent`, `AgentMoveToPlayer` (выключен, включает `Aggro`), `AnimateAlongAgent`, `Aggro`, `EnemyAttack`, `CheckAttackRange`, `EnemyHealth`, `EnemyDeath`, `ActorUI`; дети: `AggroZone` и `AttackZone` (`SphereCollider` trigger + `TriggerObserver`), `HitBox` (слой `Hittable`), `Canvas` (`HpCamera`) → `HealthBar` (`HpBar`: `Current`, `Back`), `LootSpawner` (`EnemyLootSpawner`, `EnemyDeath` назначен; урок 10) | база для всех врагов, как `BaseEnemy` в курсе |
| `Enemies/Golem` | **Prefab Variant** от `Enemy` (собирает `Tools → Setup → Enemy Variant: Golem`): `Hips` и `RockGolemMesh` под корнем, Animator — `Golem/Golem.overrideController` (база `Skeleton.controller`) + `FreeGolemMeshAvatar`; переопределено: `NavMeshAgent` Radius 0.8 / Stopping Distance 2, `AttackZone` 2.3, `AggroZone` 6, `HitBox` 3×3×3, `Canvas` y 4.36 | урок 09; что должно быть на новом враге — `<Knowledge>\mechanics\new-enemy.md` |
| `Enemies/Skeleton` | **Prefab Variant** от `Enemy`: добавлены кости и меш (`Geometry/geo/Skeleton`), Avatar и `Skeleton.controller` | новый враг = новый Variant от `Enemy` |
| `Enemies/EnemySpawner` | пустой объект: тег `EnemySpawner`, `EnemySpawner`, `UniqueId` с **пустым** `Id` | в Scene view — красная сфера 0.5 (`EnemySpawnerEditor`, `[DrawGizmo]`), в Game view не виден; `Id` генерирует `UniqueIdEditor` при выделении экземпляра на сцене (`<сцена>_<guid>`), ассет префаба пропускает. Не дублировать в Prefab Mode |
| `Loot/Loot` | из курса (урок 10): корень — слой `Pickup`, `BoxCollider` trigger, `LootPiece` (`LootModel` → `Skull_damaged`, `PickableFxPrefab` → `LootPickupFX`, `LootText`/`PickupPopup` → вложенный `PickupPopup`), `UniqueId` (`Id` пустой — id получает экземпляр: `EnemyLootSpawner` → `GenerateId()` или `LoadSceneState.InitLootPieces` из сохранения); дети `Skull_damaged` (`Skull/mobile_skull_03.fbx`), `PickupPopup`, `CFX3_MagicAura_D_Runic` | путь — `AssetPath.Loot`; курсовые `UniqueId`/`EndlessRotation` удалены пользователем при переносе, `UniqueId` наш добавлен 2026-10-27 (проверено по файлу); без него — NRE при выпадении лута |
| `Loot/PickupPopup` | TextMeshPro (3D) + `Animator` (`PickupPopup.controller`, клип `PickupPopup.anim`) | шрифт `LiberationSans SDF` — из TMP Essential Resources, без импорта текст пустой |
| `Loot/LootPickupFX`, `Loot/CFX3_MagicAura_D_Runic` | `ParticleSystem` (+ `CFX_AutoDestructShuriken` у вспышки) на legacy Cartoon FX из `Effects/JMO Assets/Cartoon FX/` | материалы переводит на URP `Tools → Setup → Loot Materials to URP`, иначе розовые |
| `UI/Window` | растянут на родителя: `Background` (`Inventory_Box`, Sliced), `Title_back` (`Title_Box`, Sliced) → `Text (TMP)`, `Close_button` (`Close_Icon` + `Button`) | базовое окно урока 12; `Close_button` → `WindowBase.CloseButton` в уроке 13. Собирает `Tools → UI → Build Shop Window` |
| `UI/ShopItem` | `Inventory_Slot` (Sliced) + `LayoutElement` 450×550; `Item` (иконка), `Button` (`Button.png`, Sliced) → `Price_container` (`HorizontalLayoutGroup`: череп + `Price_text`), `Count_text` | товар; иконка — у каждого экземпляра своя |
| `UI/Shop` | вложенный `Window`; `Content`: `AdsItem_container` (0–0.3: слот с иконкой и лентой FREE, `WatchAd_button` — лента + Play из `2D Casual UI` в рамке), `InApps_container` (0.3–1, **свой Canvas** + `GraphicRaycaster` → `Scroll` (`ScrollRect`) → `Viewport` (`Mask`) → `Content` (`HorizontalLayoutGroup` + `ContentSizeFitter`, прижат влево) → `ShopItem_1…4`), `Bottom_container` (0–0.1, подпись) | магазин урока 12; Raycast Target только у кнопок и прокрутки |
| `Prefabs/Triggers/SaveTrigger` | `BoxCollider` trigger, `SaveTrigger` | зелёный куб-гизмо в Scene view (`OnDrawGizmos`) |
| `Prefabs/UI/Curtain` | Canvas, `CanvasGroup`, `LoadingCurtain`, тег `LoadingCurtain` | шторка загрузки |
| `Prefabs/Infrastructure/GameBootstrapper`, `GameRunner` | один компонент | точка входа |

## 4. Animator

| Контроллер | Состояния | Параметры | Кто вызывает |
|---|---|---|---|
| `Yurowm/Controllers/FreeHandController` (игрок, ассет пакета) | Idle, Walk, Run, Sneak, CrouchingRun, Jump, Wary, Aiming, Fire, **Hit**, Damage1–3, Death, WeaponHands, Null | `Speed` (float), `Squat`, `Aiming` (bool), `Attack`, `Jump`, `Damage`, `Death` (trigger), `DamageID` (int) | `PlayerAnimator`; `Hit` и `Any State → Hit` (по `Attack`) добавил `Tools → Setup → Player Attack`; событие `OnAttack` — в импорте `Hit.FBX` |
| `Enemies/Skeleton/FBX/Skeleton.controller` | Locomotion (Blend Tree по `Speed`), LeftAttack, RightAttack, SplashAttack, Damage, Death — атаки, урон и смерть из `Any State` | `Speed`, `Death`, `Damage`, `SplashAttack`, `LeftAttack`, `RightAttack` | `EnemyAnimator`; клипы `Damage.anim`, `Death.anim` — заглушки из `Tools → Setup → Skeleton Animations` |

## 5. Editor-инструменты (`Tools → …`)

| Пункт меню | Скрипт | Что делает | Повторный запуск | Статус и рекомендация |
|---|---|---|---|---|
| `Tools/Clear Prefs` | `Tools.cs` | `PlayerPrefs.DeleteAll()` — стирает сохранение | безопасен | **оставить** (курс); нужен перед проверкой после изменения `PlayerProgress` |
| `Tools/UI/Build Player HUD` | `PlayerHudBuilder.cs` | собирает `HUD.prefab`: `HpBar`, `LootCounter` (урок 10), группа `Mobile`, `AttackButton` | find-or-create, безопасен | **оставить** — воспроизводит HUD |
| `Tools/Setup/Player Attack` | `PlayerAttackSetup.cs` | событие `Player/Attack` → `InputReceiver.OnAttack` в `PlayerInput`; состояние `Hit` в `FreeHandController` | не дублирует | **оставить** — показывает, как подключено; разовый, уже применён |
| `Tools/Setup/Enemy Variant: Golem` | `EnemyVariantSetup.cs` | Exit Time атак (`LeftAttack/RightAttack/SplashAttack → …`) в `Skeleton.controller` = 1; `Golem/Golem.overrideController` по таблице клипов; `Enemies/Golem.prefab` — Variant от `Enemy` с моделью под корнем | Exit Time и override — перезаписываются; Variant не трогается, если есть | **оставить** — шаблон для следующих врагов (`<Knowledge>\mechanics\new-enemy.md`) |
| `Tools/Setup/Loot Materials to URP` | `LootMaterialsSetup.cs` | 4 материала Cartoon FX (legacy `Particles/Additive`, `Additive (Soft)`) → `URP/Particles/Unlit` Additive; материал черепа (`Mobile/Diffuse`) → `URP/Simple Lit`; текстура и цвет переносятся | безопасен, перезаписывает | **оставить** — штатный конвертер legacy-частицы не переводит |
| `Tools/UI/Build Shop Window` | `ShopWindowBuilder.cs` | импорт спрайтов окна (9-slice курса на 6 спрайтах, `Generate Physics Shape` выкл); префабы `Resources/UI/Window`, `ShopItem`, `Shop` | безопасен: раскладка и компоненты — каждый раз, спрайт/цвет/текст — только при создании | **оставить** — воспроизводит UI урока 12 |
| `Tools/Setup/Skeleton Animations` | `SkeletonAnimationsSetup.cs` | кривые заглушек `Damage.anim`, `Death.anim`; `Death` → `Death.anim` | перезаписывает кривые | **временный**: удалить вместе с заглушками, когда появятся настоящие клипы Skeleton |
| `Tools/Build Demo Level Geometry` | `BuildDemoLevel.cs` | удаляет и заново строит `Level/DemoRoute` из тайлов AssetHunts | **разрушительный**: удаляет `DemoRoute` вместе с `NavMeshSurface` и `NavMeshModifier`, слой `Ground` не ставит — после запуска враги не ходят | **не запускать** без доработки; доработка — отдельная задача |
| `Tools/Level/Build Demo Geometry (SampleScene)` | `DemoLevelBuilder.cs` | то же самое, но строит `Level/DemoGeometry` | — | **не используется** (в сцене нет `DemoGeometry`) — дубликат, кандидат на удаление |
