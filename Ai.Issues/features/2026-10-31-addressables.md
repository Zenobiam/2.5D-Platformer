# 2026-10-31 — Addressables и переход между уровнями (урок 15 Addressables)

## Проблема

Префабы грузились `Resources.Load` по пути: всё содержимое `Resources` попадает в билд и в индекс, код привязан к папкам. Позиция игрока искалась на сцене `FindWithTag` в рантайме; перехода между уровнями не было.

## Что сделали

- Пакет Addressables 2.7.6 (курс — 1.16.16). `AssetsProvider`: `Load` по адресу и по `AssetReference` с кэшем завершённых загрузок, `Instantiate` через `InstantiateAsync`, `CleanUp` → `Addressables.Release` всех handle при смене уровня.
- Фабрики и `LoadSceneState` — `async`/`Task`; `WarmUp` лута и спавнера во время загрузки сцены; `SpawnPoint`, `EnemyLootSpawner` — `async void`; `LootCounter` обновляется в `Construct` (HUD создаётся асинхронно, `Start` может пройти раньше).
- `EnemyStaticData.PrefabReference` (`AssetReferenceGameObject`) вместо `GameObject Prefab`.
- `LevelStaticData`: `InitialPlayerPosition`, `LevelTransfer` (Collect по тегам `InitialPoint`, `LevelTransferInitialPoint`); `IGameStateMachine` как сервис; `LevelTransferTrigger`.
- Отступления: баг курса в `Load<T>(string)` (лишняя загрузка, не освобождалась) исправлен; префабы урока перенесены из `Resources` в `Game/Prefabs`; `TransferTo` — из `LevelStaticData`, а не из поля префаба; второй уровень — копия `SampleScene`.
- Editor: `Tools → Setup → Move Addressable Prefabs`, `Addressables`, `Second Level` (`AddressablesSetup.cs`).

## Что сломалось по дороге

- Точка перехода, созданная через GameObject → Create Empty, оказалась на y = −616 (в точке камеры Scene view) — найдено по YAML сцены до Play Mode.
- Collect меняет ассет в памяти: без File → Save Project `LevelStaticData` на диске остаётся старым.
- **Скелет на втором уровне висел в воздухе.** Причина: при копировании сцены `NavMeshSurface.OnValidate` обнуляет ссылку на NavMesh («Duplicating NavMeshSurface does not duplicate the referenced NavMesh data»), в `Editor.log` — `Failed to create agent because there is no valid NavMesh`. Исправление — Bake в копии; скрипт теперь об этом предупреждает.

## Вывод урока

Адрес вместо пути отвязывает код от папок, а счётчик ссылок Addressables требует парного `Release` — поэтому провайдер хранит все handle и чистит их при смене уровня. Асинхронное создание меняет порядок жизненного цикла: всё, что делалось в `Start` и зависит от `Construct`, переносится в `Construct`.

## Проверка

`0 error CS` (`Editor.log`); группы, адреса, guid, `PrefabReference`, сцены, Build Settings, `LevelStaticData`, NavMesh — по файлам; Play Mode — пользователь («Работает»): оба уровня, враги, лут, переход туда и обратно.

Бриф: `Ai.Tasks/2026-10-31-addressables.md` (локально).
