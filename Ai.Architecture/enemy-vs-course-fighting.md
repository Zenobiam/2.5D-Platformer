# Enemy: наш код vs курс 06 Fighting

**Дата:** 2026-09-19  
**Статус:** только сравнение (код игры не менялся)  
**Наш путь:** `Assets/Game/Scripts/Enemy/`  
**Курс:** `…\06 Fighting\knowledge-is-power-master\src\KnowledgeIsPower\Assets\CodeBase\Enemy`  
**Связанное:** `Ai.Issues/2026-09-19-aggro-wiring.md`

## Важно про источник курса

В zip/CodeBase урока **06 Fighting** в папке `Enemy` лежат **только** aggro + движение + аниматор.  
Классов `Attack`, `EnemyHealth`, `EnemyDeath`, hitbox/overlap **нет** ни в распакованной папке, ни внутри `knowledge-is-power-master.zip`.

Темы атаки/HP есть в **видео** (part 1: Attack animation / Hitboxes / AttackRange / Construct attack; part 2 в `07 Fighting 2`: HeroHealth / EnemyHealth / IHealth / EnemyDeath), но в скачанном CodeBase `Enemy` для 06 они не попали. Ниже gap’ы по Fighting разделены: «есть в CodeBase Enemy» vs «ожидается по видеоурокам».

---

## 1. Инвентарь файлов

### Наш `Assets/Game/Scripts/Enemy/` (7 `.cs`)

| Файл | Роль |
|---|---|
| `Aggro.cs` | зона → вкл/выкл follow + cooldown |
| `TriggerObserver.cs` | события OnTriggerEnter/Exit |
| `AgentMoveToPlayer.cs` | NavMesh → к игроку (`Follow`) |
| `AnimateAlongAgent.cs` | скорость агента → Animator |
| `EnemyAnimator.cs` | параметры аниматора + state reader |
| `Follow.cs` | абстрактная база |
| `RotateToHero.cs` | поворот к игроку (`Follow`) |

### Курс `CodeBase/Enemy` (7 `.cs`) — **тот же набор имён**

| Файл | Роль |
|---|---|
| `Aggro.cs` | зона → вкл/выкл follow + cooldown |
| `TriggerObserver.cs` | события OnTriggerEnter/Exit |
| `AgentMoveToPlayer.cs` | NavMesh → к герою (`Follow`) |
| `AnimateAlongAgent.cs` | скорость агента → Animator |
| `EnemyAnimator.cs` | параметры аниматора + state reader |
| `Follow.cs` | абстрактная база |
| `RotateToHero.cs` | поворот к герою (`Follow`) |

**Вывод по инвентарю:** 1:1 по именам файлов. Расхождений «файл есть / файла нет» внутри папки Enemy **нет**.

Связанное у нас (не в Enemy, но для Fighting):  
`Animation/Interfaces/AnimatorState.cs`, `IAnimationStateReader`, `AnimatorStateReporter` — аналоги `CodeBase/Logic/*`.

---

## 2. Что совпадает с курсом

- **Состав стека:** TriggerObserver → Aggro → Follow-наследники + AnimateAlongAgent + EnemyAnimator.
- **`Follow`:** пустой `abstract class Follow : MonoBehaviour`.
- **`TriggerObserver`:** почти идентичен (`RequireComponent(Collider)`, события Enter/Exit).
- **`AgentMoveToPlayer`:** та же идея — ждать `PlayerCreated`/`HeroCreated`, в `Update` ставить `Agent.destination`, порог `MinimalDistance = 1`, `SqrMagnitudeTo`.
- **`AnimateAlongAgent`:** те же пороги `MinimalVelocity = 0.1f` и условие `velocity > min && remainingDistance > radius`.
- **`RotateToHero`:** логика LookAt по XZ + `Quaternion.Lerp` с `Speed * deltaTime` — совпадает (нейминг Player vs Hero / ServiceLocator vs AllServices).
- **Префаб курса (Lich):** Aggro.Cooldown = 3, Follow → `AgentMoveToPlayer` — тот же паттерн проводки, что мы целимся в aggro-wiring.

---

## 3. Чего нет у нас / чего нет в CodeBase курса

### В CodeBase `Enemy` (06) у курса есть, у нас — с расхождениями (не «отсутствие файла»)

См. §4 (поведение) и §5 (приоритеты выравнивания).

### В CodeBase `Enemy` (06) **нет ни у курса, ни у нас** (но есть в видео Fighting)

| Тема (видео) | Ожидаемый артефакт (по урокам) | Статус у нас |
|---|---|---|
| Attack animation / Construct attack | `Attack.cs` (cooldown, range, PlayAttack, overlap) | нет |
| Hitboxes / Overlap / Visual debug | hitbox + Physics.Overlap* / debug | нет |
| AttackRange | дистанция атаки + переключение move↔rotate | нет |
| EnemyHealth / IHealth (part 2) | HP, TakeDamage, событие | нет |
| EnemyDeath (part 2) | смерть, отключение AI, анимация Die | нет |
| BaseEnemy / Construct (part 2) | сборка врага / Init | нет |

`EnemyAnimator` у нас уже имеет **заготовки** `PlayLeft/Right/SplashAttack`, `PlayDamage`, `PlayDeath`, но **нет вызывающего геймплей-кода** (Attack/Health).

### У нас есть «лишнее» относительно курса (не баг)

- `AgentMoveToPlayer.EnsureOnNavMesh()` — наш pragmatic fix (Warp при старте вне NavMesh); в курсе этого нет.
- `EnemyAnimator.OnAnimatorMove()` пустой — чтобы root motion не конфликтовал с NavMeshAgent; в курсе нет.
- Параметры аниматора заточены под **Yurowm**-контроллер (Left/Right/Splash), а не под Lich курса (`Attack_1`, `IsMoving`).

---

## 4. Поведенческие различия

### Aggro (главное расхождение)

| | Курс | Мы |
|---|---|---|
| Тип Follow | `Follow` (база) | `Follow` (база) — как в курсе, чтобы вешать chase или `RotateToHero` |
| Флаг `_hasAggroTarget` | ставится в `SwitchFollowOn/Off` | на Enter=true, на Exit=false **до** cooldown (= «игрок в зоне») |
| Enter при уже aggro | `if (_hasAggroTarget) return` | всегда StopCoroutine + Follow on (**намеренный** фикс re-enter) |
| После cooldown | всегда `SwitchFollowOff()` | `if (_hasAggroTarget) On else Off` |
| WaitForSeconds | кэш в `Start` | кэш в `Start` |
| Unsubscribe | `OnDestroy` снимает события | `OnDestroy` снимает события |
| Старт | `Follow.enabled = false` inline | `SwitchFollowOff()` |

**Смысл:**

- Курс держит `_hasAggroTarget == true` весь cooldown после Exit → повторный Enter во время cooldown **игнорируется** (ранний return) и **не стопает** корутину → follow может выключиться даже если игрок снова в зоне. Это нюанс/дыра курса.
- Наша версия на Exit сразу сбрасывает флаг и при повторном Enter **отменяет** cooldown → re-aggro работает предсказуемее, но **не байт-в-байт** как CodeBase.
- Тип поля `Follow`: совпадает с курсом — в Inspector можно повесить `AgentMoveToPlayer` (преследование) или `RotateToHero` (смотрит, не бежит). Семантика cooldown у нас с фиксом re-enter (см. `internal/aggro-cooldown-fix.md`).

### Move (`AgentMoveToPlayer`)

- Нейминг: `Player` / `IGameFactory.PlayerGameObject` vs `Hero` / `HeroGameObject`.
- DI: `ServiceLocator` vs `AllServices`.
- У нас доп. `EnsureOnNavMesh`.
- Иначе логика destination совпадает.

### Animate (`AnimateAlongAgent`)

- Условие ShouldMove — одинаковое.
- Курс: `[SerializeField] private` Agent/Animator; у нас: `public` (как в Aggro/курсе-стиле public Inspector).
- Поведение Move/StopMoving **разное на уровне аниматора** (см. ниже).

### EnemyAnimator + AnimatorState

| | Курс | Мы |
|---|---|---|
| Движение | `IsMoving` bool + `Speed` float | только `Speed` float (0..1) |
| Атака | один `Attack_1` → `PlayAttack()` | Left / Right / Splash triggers |
| Урон/смерть | `Hit` / `Die` | `Damage` / `Death` |
| State enum | Idle, Attack, Walking, Died | Locomotion, Left/Right/SplashAttack, Damage, Death |
| State hashes | idle / attack01 / Move / die | совпадают с именами триггеров Yurowm |

**Итог:** API аниматора **не совместим** с курсовым Lich без адаптации. Заготовки атаки у нас есть, но под другой контроллер.

### RotateToHero

- Код почти 1:1; у нас пока нет геймплей-переключения (Attack ещё нет). В префабе Lich курса в этом snapshot **RotateToHero тоже не навешан** — только код готов.

---

## 5. Gap list (догнать Fighting) — только предложения

### P0 — выровнять то, что уже в CodeBase Enemy

1. **Aggro (сделано частично):** поле `Follow` (база) + OnDestroy/кэш WaitForSeconds. Cooldown — **намеренный** re-enter-fix (не early-return курса); см. `internal/aggro-cooldown-fix.md`.
2. **Проводка префаба:** TriggerObserver на AggroZone, Aggro.Follow → `AgentMoveToPlayer` *или* `RotateToHero`, Cooldown, стартовый Follow off.
3. **Не ломать EnsureOnNavMesh** без причины — это наш фикс окружения, не часть курса.

### P1 — содержимое видео 06 Fighting (Attack), которого нет в zip Enemy

4. **`Attack`:** cooldown атаки, проверка дистанции, `EnemyAnimator.PlayAttack` (или наши Left/Right/Splash), overlap/hitbox по герою.
5. **Переключение Follow:** в радиусе атаки — выкл `AgentMoveToPlayer`, вкл `RotateToHero` (поле Aggro/`Follow` как база или отдельные ссылки в Attack).
6. **Свести Animator API:** либо адаптировать Yurowm-триггеры под один `PlayAttack()`, либо оставить мультиатаку, но дать Attack одну точку вызова.

### P2 — видео 07 Fighting 2 + полировка

7. **`IHealth` / EnemyHealth / TakeDamage** + реакция аниматора (`PlayDamage` / Hit).
8. **`EnemyDeath`:** PlayDeath, отключение Aggro/Agent/Attack, коллайдеры.
9. **Unsubscribe / жизненный цикл** везде по образцу курса (`OnDestroy`).
10. Позже: Construct/BaseEnemy, UI HP bar — когда дойдём до спавна (урок ~09 по Info.txt курса).

---

## 6. Предложения (без реализации)

- Считать **06 Fighting CodeBase Enemy** закрытым эталоном для **агро+chase+animate**, не для боя: боя в этой папке zip просто нет.
- Для атаки/HP ориентироваться на **видео 06/07** + при появлении CodeBase из Fighting 2 / более поздних уроков — переснять diff.
- Aggro: либо «strict course», либо «course + fixed re-enter»; не смешивать молча.
- Аниматор: не копировать Lich-параметры слепо — зафиксировать контракт под выбранный enemy-rig (Yurowm).
- `RotateToHero` уже есть — включать в контур только вместе с Attack/range.

---

## Ссылки

- Курс Enemy: `G:\MediaGetDownloads\RealyGood\…\06 Fighting\knowledge-is-power-master\src\KnowledgeIsPower\Assets\CodeBase\Enemy`
- Наш Enemy: `Assets/Game/Scripts/Enemy/`
- Копия в Project store: `docs/enemy-vs-course-fighting.md`
