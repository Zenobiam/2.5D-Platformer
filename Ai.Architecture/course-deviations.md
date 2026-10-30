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
| 2026-10-25 | Материалы лута | Cartoon FX на built-in `Particles/Additive` и `Additive (Soft)`, череп на `Mobile/Diffuse` | `URP/Particles/Unlit` (Additive) и `URP/Simple Lit`, переводит `Tools → Setup → Loot Materials to URP` | Проект на URP, built-in шейдеры розовые, штатный конвертер legacy-частицы не берёт. Минус: `Additive (Soft)` (`One OneMinusSrcColor`) заменён обычным Additive — вид может слегка отличаться |
| 2026-10-25 | Атлас черепа | `cemetery_halloween_atlas.tga`, 37.7 МБ без сжатия | `cemetery_halloween_atlas.png` без потерь, 13 МБ, тот же guid и настройки импорта | Меньше репозиторий и LFS; в билде размер одинаковый (Unity сжимает по своим настройкам) |
| 2026-10-27 | Отписка счётчика лута | `LootCounter` подписывается на `LootData.Changed` и не отписывается | `LootCounter.OnDestroy` отписывается (решение пользователя) | `WorldData` живёт в прогрессе дольше HUD: без отписки после перезагрузки сцены событие получает уничтоженный счётчик → `MissingReferenceException`. Так же курс делает в `ActorUI.OnDestroy` |
| 2026-10-27 | HUD счётчика лута | Отдельный префаб `Hud/Loot.prefab`, вложенный в `Hud` | Дочерний объект `HUD/LootCounter`, собирает `Tools → UI → Build Player HUD` | Наш HUD целиком собирает Editor-скрипт; вложенный префаб пришлось бы собирать и подключать отдельно |
| 2026-10-27 | Сохранение душ | Прогресс (в том числе `LootData.Collected` и лут на сцене) пишется только на `SaveTrigger` | `LootPiece` при подборе вызывает `SaveProgress()` (сервис — из `ServiceLocator`, как в `SaveTrigger`) | Решение пользователя: души не теряются при выходе. Минус: `SaveProgress` пишет весь прогресс, в том числе позицию игрока — каждый подбор работает как чекпоинт |
| 2026-10-28 | Сбор спавнеров уровня | `LevelStaticDataEditor` (Collect): `FindObjectsOfType<SpawnMarker>()` | `FindObjectsByType<SpawnMarker>(FindObjectsSortMode.None)` | В Unity 6 `FindObjectsOfType` устарел (`CS0618`); порядок не нужен — сортировка не делается |
| 2026-10-30 | Прокрутка колесом в магазине | `ScrollRect.scrollSensitivity` = 1 (мобилка — листают пальцем) | 50 (`ShopWindowBuilder.WheelScrollSensitivity`) | Пока основная платформа ПК (`Ai.Decisions/2026-10-30-pc-first.md`): `InputSystemUIInputModule` даёт 6 единиц на щелчок колеса — с 1 это ~130 щелчков на ленту |
| 2026-10-30 | Вёрстка окна магазина | `Content` прокрутки растянут на `Viewport`; Play — картинка `GUI CARTOON`; иконки `Free RPG Icons` | `Content` прижат влево (anchors x 0, pivot x 0); Play — круг + треугольник `2D Casual UI`; иконки `RPG icons/512X512`; `Generate Physics Shape` выкл у всех UI-спрайтов | `ContentSizeFitter` по ширине с растянутыми якорями считает ширину неверно; паков `GUI CARTOON` и `Free RPG Icons` у нас нет |
| — | NavMesh | Запекание в окне Navigation | `NavMeshSurface` из пакета AI Navigation | Встроенное запекание в Unity 6 устарело |

## Кандидаты (не сделано, только по задаче)

| Тема | Курс | Unity 6 | Статус |
|---|---|---|---|
| Ввод через сгенерированный класс | — | `PlayerControls` (Generate C# Class) в `InputService` вместо `PlayerInput` + `InputReceiver` | предложено, ждёт задачи |
| Поиск объектов | `FindObjectOfType` / `FindObjectsOfType` | `FindFirstObjectByType` / `FindObjectsByType(FindObjectsSortMode.None)` | `UniqueIdEditor` (урок 08) оставлен как в курсе — `FindObjectsOfType`, предупреждение `CS0618`; заменить по решению пользователя |
| Скорость Rigidbody | `Rigidbody.velocity` | `Rigidbody.linearVelocity` | в коде не используется (`NavMeshAgent.velocity` не затронут) |
| Текст UI | `UnityEngine.UI.Text` | TextMeshPro | когда дойдём до текста в UI |
