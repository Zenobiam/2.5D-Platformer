# Что настроено в редакторе

Справочник того, что живёт не в коде, а в Unity. Сверено по YAML 2026-10-25 (ветка `feature/enemy-spawner`, `e045cb6`): `ProjectSettings/*.asset`, `.prefab`, `.unity`, `.controller`. Обновлять тем же изменением, что и задачу (`Ai.Context/README.md` § «Что обновить»). Заменяет обзор [`gameobject-architecture-notes.md`](gameobject-architecture-notes.md) (2026-09-20).

Как читать: «зачем» — что делает код с этой настройкой; «если забыть» — как ломается (часто молча).

## 1. Настройки проекта

### Теги (`ProjectSettings/TagManager.asset`)

| Тег | Где стоит | Кто ищет | Если забыть |
|---|---|---|---|
| `InitialPoint` | `SampleScene` → `PlayerHandler` | `LoadLevelState`: `FindWithTag` → точка создания игрока | `NullReferenceException` при загрузке уровня |
| `LoadingCurtain` | префаб `Curtain` | — (тег курса, кодом не ищется) | — |
| `Player` (встроенный) | корень префаба `Player` | — | — |
| `Player Modell` | нигде (проверено по префабам и сценам) | — | лишний тег |
| `MainCamera` (встроенный) | `SampleScene`, `InitialScene` → `Main Camera` | `Camera.main` в `LoadLevelState.CameraFollow` | камера не следует за игроком |

### Слои и матрица коллизий (`TagManager.asset`, `DynamicsManager.asset`)

| # | Слой | Где стоит | Зачем |
|---|---|---|---|
| 10 | `Ground` | `Level`, `Level/DemoRoute/Ground` | земля уровня |
| 12 | `Player` | корень `Player` и коллайдеры | маска `Player` в `EnemyAttack` (`OverlapSphereNonAlloc`) — враг находит игрока |
| 13 | `Aggro` | **нигде** | — |
| 14 | `AttackZone` | **нигде** | — |
| 15 | `Hittable` | `Enemy/HitBox` (`BoxCollider`, не trigger) | маска `Hittable` в `PlayerAttack` — игрок находит врага |

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
| `Main Camera` | Camera, `CameraController` | `LoadLevelState` вызывает `Follow(player)` | руками |
| `InputHandler` | `PlayerInput` (Invoke Unity Events, карта `Player`), `InputReceiver` | события `Move/Jump/Run/Crouch/Attack` → `InputReceiver.OnMove/OnJump/OnRun/OnCrouch/OnAttack` | руками; `Attack` — `Tools → Setup → Player Attack` |
| `Level` → `DemoRoute` | `NavMeshSurface`; дети `Ground` (слой `Ground`), `Hazards`, `Decor` — `NavMeshModifier` | геометрия и навигация для врагов | геометрия — `Tools → Build Demo Level Geometry`; NavMesh — руками |
| `Skeleton` (префаб `Skeleton.prefab`) | враг | пока стоит на сцене руками; с урока 09 его создаёт спавнер | руками |
| `SaveTrigger` (префаб) | `BoxCollider` trigger, `SaveTrigger` | сохранение прогресса при входе | руками |
| `GameRunner` (префаб) | `GameRunner` | старт из `SampleScene` без `InitialScene` | руками |
| `Global Volume`, `Directional Light`, `EventSystem` (`InputSystemUIInputModule`) | стандарт URP / Input System | — | — |
| `-------Managers-----------`, `--------Core----------`, `-----------------------` | пустые | разделители в иерархии | — |

Игрок и HUD на сцене **не лежат**: их создаёт `GameFactory` из `Resources` в `LoadLevelState`.

## 3. Префабы

| Префаб (`Assets/Resources/…`) | Устройство | Зачем / неочевидное |
|---|---|---|
| `Prefabs/Player/Player` | корень: слой `Player`, `CharacterController`, `Rigidbody`, `PlayerMovementController`, `PlayerHealth`, `PlayerDeath`, `PlayerFacing`; модель — вложенный префаб `Yurowm/…/CyborgGirl` (на нём `Animator`, `PlayerAnimator`, `PlayerAttack`) | путь — `AssetPath.PlayerPath`; при смерти `Rigidbody` → kinematic |
| `Prefabs/Player/HUD` | Canvas, `ActorUI`; `HpBar` (`Background`, `Fill`); группа `Mobile` (`MobileOnly`): `JoyStickArea`, `AttackButton` (`OnScreenButton` → `<Gamepad>/buttonWest`) | собирается `Tools → UI → Build Player HUD`; корень должен быть **активен**, иначе `GetComponentInChildren<ActorUI>()` = null |
| `Enemies/Enemy` | **логика врага без модели**: `Animator` (без Avatar), `EnemyAnimator`, `NavMeshAgent`, `AgentMoveToPlayer` (выключен, включает `Aggro`), `AnimateAlongAgent`, `Aggro`, `EnemyAttack`, `CheckAttackRange`, `EnemyHealth`, `EnemyDeath`, `ActorUI`; дети: `AggroZone` и `AttackZone` (`SphereCollider` trigger + `TriggerObserver`), `HitBox` (слой `Hittable`), `Canvas` (`HpCamera`) → `HealthBar` (`HpBar`: `Current`, `Back`) | база для всех врагов, как `BaseEnemy` в курсе |
| `Enemies/Skeleton` | **Prefab Variant** от `Enemy`: добавлены кости и меш (`Geometry/geo/Skeleton`), Avatar и `Skeleton.controller` | новый враг = новый Variant от `Enemy` |
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
| `Tools/UI/Build Player HUD` | `PlayerHudBuilder.cs` | собирает `HUD.prefab`: `HpBar`, группа `Mobile`, `AttackButton` | find-or-create, безопасен | **оставить** — воспроизводит HUD |
| `Tools/Setup/Player Attack` | `PlayerAttackSetup.cs` | событие `Player/Attack` → `InputReceiver.OnAttack` в `PlayerInput`; состояние `Hit` в `FreeHandController` | не дублирует | **оставить** — показывает, как подключено; разовый, уже применён |
| `Tools/Setup/Skeleton Animations` | `SkeletonAnimationsSetup.cs` | кривые заглушек `Damage.anim`, `Death.anim`; `Death` → `Death.anim` | перезаписывает кривые | **временный**: удалить вместе с заглушками, когда появятся настоящие клипы Skeleton |
| `Tools/Build Demo Level Geometry` | `BuildDemoLevel.cs` | удаляет и заново строит `Level/DemoRoute` из тайлов AssetHunts | **разрушительный**: удаляет `DemoRoute` вместе с `NavMeshSurface` и `NavMeshModifier`, слой `Ground` не ставит — после запуска враги не ходят | **не запускать** без доработки; доработка — отдельная задача |
| `Tools/Level/Build Demo Geometry (SampleScene)` | `DemoLevelBuilder.cs` | то же самое, но строит `Level/DemoGeometry` | — | **не используется** (в сцене нет `DemoGeometry`) — дубликат, кандидат на удаление |
