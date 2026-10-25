# Отступления от курса (Unity 2019.3 → Unity 6)

**Принцип:** архитектура — из курса, API Unity — современные. Правило: [`../Ai.Context/preferences.md`](../Ai.Context/preferences.md) § «Курс 2021 vs наш Unity 6».

Курс k-syndicate записан в 2021 году на Unity 2019.3.4f1; проект — Unity 6 (`6000.0.60f1`). Сюда пишем каждое место, где код отличается от курса из-за версии Unity или платформы. Одна строка: «курс → у нас → почему».

## Сделано

| Дата | Тема | Курс | У нас | Почему |
|---|---|---|---|---|
| 2026-10-24 | Ввод | `UnityEngine.Input` + SimpleInput; два сервиса `StandaloneInputService` / `MobileInputService`, выбор по `Application.isEditor` | Input System: один `InputService`, устройства — привязками в `PlayerControls.inputactions` | Старый Input Manager — legacy; Input System описывает устройства схемой, код от устройства не зависит |
| 2026-10-24 | Экранные кнопки | Кнопки SimpleInput (`SimpleInput.GetButtonUp("Fire")`) | `OnScreenButton` (Input System) нажимает `<Gamepad>/buttonWest`, привязанную к `Attack` | Экранный контрол попадает в то же действие, что мышь и геймпад; без стороннего пакета |
| 2026-10-24 | Мобильный HUD | Экранные кнопки видны всегда | Группа `Mobile` в HUD с `MobileOnly`: видна по `UnityEngine.Device.Application.isMobilePlatform` | Device Simulator (Unity 2021+) подменяет только `UnityEngine.Device.*`; переключение ПК/мобилка в редакторе без смены платформы |
| 2026-10-24 | Точка удара игрока | `y = _characterController.center.y / 2` (абсолютная высота) | `y = transform.position.y + center.y / 2` (`PlayerAttack.StartPoint`) | В курсе пол на y = 0; у нас уровень на y ≈ 2, сфера уходила под пол, `hits = 0` |
| 2026-10-25 | Агент вне NavMesh | `AgentMoveToPlayer` только ставит `destination` (урок 09: `Construct` + `SetDestinationForAgent`) | + `EnsureOnNavMesh()` в `Start`: `NavMesh.SamplePosition` в радиусе 5 и `Agent.Warp` (добавлено 2026-09-19, `128414f`) | Уровень не на y = 0, враг появляется в позиции спавнера и может оказаться вне меша — тогда агент молча игнорирует `destination` (`Ai.Issues/bugs/2026-09-19-navmesh-status.md`). Минус: ошибка расстановки не видна |
| 2026-10-25 | Анимации нового врага | Общий `EnemyAnimatorController` + `AnimatorOverrideController` на монстра; префаб голема — Variant от курсового `BaseEnemy` | Override на базе `Skeleton.controller` (наш общий контроллер врага с параметрами `EnemyAnimator`), Variant от `Enemy.prefab` собирает `Tools → Setup → Enemy Variant: Golem`, кости под корнем | Наш `EnemyAnimator` вызывает параметры Skeleton (`RightAttack`, `Damage`, `Death`); у курса другой набор (`Attack_1`, `Hit`, `Die`); готовые префабы курса ссылаются на его guid |
| — | NavMesh | Запекание в окне Navigation | `NavMeshSurface` из пакета AI Navigation | Встроенное запекание в Unity 6 устарело |

## Кандидаты (не сделано, только по задаче)

| Тема | Курс | Unity 6 | Статус |
|---|---|---|---|
| Ввод через сгенерированный класс | — | `PlayerControls` (Generate C# Class) в `InputService` вместо `PlayerInput` + `InputReceiver` | предложено, ждёт задачи |
| Поиск объектов | `FindObjectOfType` / `FindObjectsOfType` | `FindFirstObjectByType` / `FindObjectsByType(FindObjectsSortMode.None)` | `UniqueIdEditor` (урок 08) оставлен как в курсе — `FindObjectsOfType`, предупреждение `CS0618`; заменить по решению пользователя |
| Скорость Rigidbody | `Rigidbody.velocity` | `Rigidbody.linearVelocity` | в коде не используется (`NavMeshAgent.velocity` не затронут) |
| Текст UI | `UnityEngine.UI.Text` | TextMeshPro | когда дойдём до текста в UI |
