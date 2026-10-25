# Архитектура игровых объектов (GO / Prefab / Scene)

> **Заменено (2026-10-25)** справочником [`editor-setup.md`](editor-setup.md): там актуальное состояние после уроков 05–07 (слои, `Enemy`/`Skeleton` Variant, HUD `Mobile`, Tools). Этот файл — снимок на 2026-09-20, для истории.

**Дата:** 2026-09-20  
**Область:** префабы Player / Skeleton / SaveTrigger + ключевые GO SampleScene  
**Ограничения разбора:** только YAML/prefab text + связанные скрипты; код и префабы не менялись. Сверка с курсом 08 Fighting — вне скоупа.

---

## 1. SampleScene — каркас

### Корни сцены

| GO | Роль |
|---|---|
| `PlayerHandler` | точка спавна игрока, tag `InitialPoint` (только Transform) |
| `Main Camera` | URP camera + `CameraController` (`_followTarget` пуст в сцене → вешается в `LoadLevelState`) |
| `InputHandler` | `PlayerInput` + `InputReceiver` (сцена, не префаб игрока) |
| `Level` → `DemoRoute` | геометрия + `NavMeshSurface` (baked: `NavMesh-DemoRoute 1.asset`) |
| `DemoRoute` → `Ground` / `Hazards` / `Decor` | тайлы/пропсы AssetHunts + `NavMeshModifier` |
| `Skeleton` (prefab instance) | враг, root сцены, pos ≈ (7.83, 2, 0) |
| `SaveTrigger` (prefab instance) | сейв-зона, root сцены, pos ≈ (11.88, 2.62, −1) |
| `GameRunner` (prefab instance) | страховка bootstrap, если зашли в сцену без Initial |
| `EventSystem`, `Directional Light`, `Global Volume` | стандартный рантайм |
| `-------Managers-----------`, `--------Core----------`, `-----------------------` | **пустые** организаторы (детей нет) |

### Кто создаётся когда

| Объект | Источник |
|---|---|
| Player, HUD | runtime: `GameFactory` из Resources (`AssetPath`) в `LoadLevelState` |
| Skeleton, SaveTrigger, уровень, input, camera | лежат в SampleScene |
| GameBootstrapper / Curtain | через `GameRunner` / bootstrap-поток (InitialScene почти пустая) |

Асимметрия: игрок — factory; враг и триггер сейва — scene-placed. Для учебного демо ок, но паттерн спавна не единый.

---

## 2. Player.prefab

### Иерархия

```
Player                          ← логический корень
├── CharacterController
├── Rigidbody                   ← см. «хрупкое»
├── PlayerMovementController    (MovementSpeed = 4)
└── CyborgGirl (nested PrefabInstance → Yurowm CyborgGirl)
    ├── Animator (controller подменён на игровой)
    ├── PlayerVisual (added)
    │     SerializeField: modelTransform, animator, rotationSpeed, defaultFacingRight
    └── [removed] Yurowm Actions + Yurowm PlayerController
```

### Кто корень логики

**`Player` (root)** — движение, прогресс (`ISaveProgress`), CharacterController.  
**`CyborgGirl`** — только визуал/аниматор: `PlayerAnimator` (Animator-параметры, событие `OnAttack`) и `PlayerAttack` (кнопка атаки, поиск целей). Поворот модели по ±X делает `PlayerFacing` на корне `Player`.

Связи: `PlayerMovementController` берёт CC через `GetComponent`, visual через `GetComponentInChildren` — SerializeField почти нет (кроме публичного `MovementSpeed`). Visual сам держит ссылки на Transform/Animator через Inspector.

### Хорошо

- Чёткое разделение **logic root vs visual child**.
- Yurowm demo-скрипты (`Actions`, `PlayerController`) **сняты** с instance (`m_RemovedComponents`) — логика не живёт на чужом ассете.
- Root motion глушится (`OnAnimatorMove` пустой в `PlayerAnimator`).
- Tag `Player` на корне.

### Плохо / хрупко

- На корне одновременно **CharacterController и Rigidbody** (`UseGravity=1`, `IsKinematic=0`). Код RB не использует; CC сам двигает тело. Классический конфликт физики / лишний компонент.
- Nested dependency на Yurowm-префаб: смена/реимпорт исходника требует сохранить removed components + наш Animator controller override.
- `Camera.main` и static `InputReceiver` — не битые ссылки на префабе, но жёсткая привязка к сцене.
- HUD-префаб по умолчанию `m_IsActive: 0` — инстанс из factory тоже стартует выключенным (если ничего не активирует).

---

## 3. Skeleton.prefab

### Иерархия

```
Skeleton  (= корень FBX IDLE.fbx, PrefabInstance)   ← логика + визуал на одном GO
├── Animator (controller → Skeleton)
├── EnemyAnimator
├── NavMeshAgent (speed 3.5, stoppingDistance 1, height 2, radius 0.2)
├── AgentMoveToPlayer : Follow   [enabled = 0 на префабе]
│     SerializeField/public: Agent → NavMeshAgent
├── AnimateAlongAgent
│     Agent → NavMeshAgent; Animator → EnemyAnimator
├── Aggro
│     TriggerObserver → AggroZone.TriggerObserver
│     Follow → AgentMoveToPlayer
└── AggroZone (child, local y=1)
    ├── SphereCollider isTrigger, radius 3
    └── TriggerObserver
```

### Кто корень логики

**Тот же GO, что и модель FBX** — Agent, Aggro, AnimateAlongAgent, EnemyAnimator висят на визуальном корне.  
`AggroZone` вынесен в child (правильно для радиуса), но `Aggro` / `Follow` остаются на root модели.

Цепочка: `TriggerObserver` → `Aggro` включает/выключает `Follow` (`AgentMoveToPlayer`) → Agent идёт к `GameFactory.PlayerGameObject` → `AnimateAlongAgent` пишет скорость в `EnemyAnimator`.

### Хорошо

- Aggro вынесен в отдельный trigger-child + тонкий `TriggerObserver` (события).
- Follow по умолчанию **выключен** — idle, пока не вошли в зону.
- `AnimateAlongAgent` ссылается на `EnemyAnimator`, не на сырой Animator — единая точка анимационных API.
- Root motion глушится в `EnemyAnimator.OnAnimatorMove`.
- Agent умеет `Warp` на NavMesh, если спавн чуть мимо (`EnsureOnNavMesh`).
- Сцена: `NavMeshSurface` на `DemoRoute`, bake есть.

### Плохо / хрупко

- **Логика наложена на FBX/визуал** — в отличие от Player. Реимпорт модели / смена скелета рискует для added components и ссылок.
- Нет отдельного empty «Enemy root» с child Visual — хуже копируемый паттерн.
- `Aggro.Cooldown` в YAML **не сериализован** → runtime `0` (мгновенный cooldown после Exit).
- `TriggerObserver` / `SaveTrigger` **не фильтруют** по tag/layer — любой collider включает aggro / сейв.
- `RotateToHero` есть в коде, на префабе **нет** — мёртвый/отложенный кусок.
- Враг не через `GameFactory` — нет общего lifecycle/progress registration с игроком.
- Player = CharacterController; Enemy = NavMeshAgent — осознанно разные системы (2.5D vs AI), но стык по высоте/оси Z нужно держать в голове (платформер + bake).

---

## 4. SaveTrigger.prefab

```
SaveTrigger
├── BoxCollider isTrigger (1×1×3)
└── SaveTrigger
      public Collider: BoxCollider (для gizmos)
```

- Без детей, без Rigidbody (триггер ловит CC игрока — нормально).
- `OnTriggerEnter` → `ISaveLoadService.SaveProgress()` → `SetActive(false)`.
- Нет проверки, что вошёл именно Player.
- Позиция в префабе и в сцене различаются (сцена оверрайдит) — ожидаемо для instance.

**Вердикт:** простой и читаемый demo-триггер; хрупкость — отсутствие фильтра и одноразовость без респавна.

---

## 5. Инфраструктурные префабы (кратко)

| Prefab | Состав |
|---|---|
| `GameBootstrapper` | один MB: ссылка на `LoadingCurtainPrefab` |
| `GameRunner` | один MB: ссылка на `BootstrapperPrefab` |
| `HUD` | Canvas + JoyStickArea; **inactive** в ассете |
| `Curtain` | loading UI (через bootstrap) |

Чистые, тонкие — хороший образец «один GO = одна инфраструктурная роль».

---

## 6. Сводка: хорошо / плохо / хрупко

### Хорошо

1. Player: logic root + nested visual, Yurowm-логика снята.
2. Enemy: AggroZone child + Observer events + Follow enable/disable.
3. Инфра-префабы (Bootstrapper/Runner) минимальны и с явными SerializeField на префабы.
4. Камера: target не захардкожен в сцене — `Follow` из `LoadLevelState`.
5. `PlayerHandler` + tag `InitialPoint` — понятная точка спавна без логики на маркере.
6. NavMesh bake привязан к `DemoRoute` / Level-иерархии уровня.

### Плохо / хрупко

1. Skeleton: gameplay-компоненты на корне FBX (overlay на визуале).
2. Player: лишний non-kinematic Rigidbody рядом с CharacterController.
3. Разный spawn path: Player/HUD factory vs Enemy/SaveTrigger scene.
4. Триггеры без фильтра tag/layer (aggro и save).
5. `Aggro.Cooldown` фактически 0; `RotateToHero` не подключён.
6. Пустые organizer-GO и HUD inactive — шум / сюрприз при инстансе.
7. Input через static `InputReceiver` на сценном GO — работает, но тестируемость и «префаб самодостаточен» слабее, чем у Player.

---

## 7. Observations / Risks / Proposals

### Observations

- Паттерн Player (пустой logic root + visual prefab) и паттерн Skeleton (logic on model) **не согласованы**.
- Aggro ↔ Follow через `enabled` — простой и прозрачный state без отдельной FSM на враге.
- CharacterController (player) vs NavMeshAgent (enemy) — две модели движения в одной сцене; для 2.5D платформера это ожидаемо на текущем этапе.

### Risks

- Реимпорт Yurowm / Skeleton FBX может затронуть overrides и added components.
- Rigidbody на Player может дать неожиданные physics callbacks / drift.
- Любой moving collider в AggroZone / SaveTrigger даст ложные срабатывания.
- Неактивный HUD-префаб → UI может «пропасть» после CreateHud, если нет явного Activate.

### Proposals (только предложения, не делать в этом таске)

1. Убрать Rigidbody с Player **или** сделать kinematic и задокументировать зачем он нужен (триггеры и так видят CC).
2. Вынести Skeleton logic на empty root, FBX оставить child Visual (как Player) — единый копируемый шаблон.
3. Сериализовать `Aggro.Cooldown` в префабе; при необходимости фильтр `other.CompareTag("Player")` в Observer / SaveTrigger.
4. Позже: спавн врагов через `GameFactory` + `AssetPath`, когда понадобится lifecycle как у Player.
5. Включить HUD в префабе или активировать после `CreateHud`.
6. Подключить или удалить `RotateToHero`, чтобы не копился dead code.
7. Почистить пустые `Managers` / `Core` / `---` из SampleScene или реально наполнить их.

---

## Источники

- `Assets/Resources/Prefabs/Player/Player.prefab`
- `Assets/Resources/Enemies/Skeleton/Prefab/Skeleton.prefab`
- `Assets/Resources/Prefabs/Triggers/SaveTrigger.prefab`
- `Assets/Game/Scenes/SampleScene.unity`
- Скрипты: `PlayerMovementController`, `PlayerFacing`, `PlayerAnimator`, `PlayerAttack`, `PlayerHealth`, `PlayerDeath`, `Aggro`, `AgentMoveToPlayer`, `AnimateAlongAgent`, `TriggerObserver`, `EnemyAnimator`, `SaveTrigger`, `GameFactory`, `LoadLevelState`
