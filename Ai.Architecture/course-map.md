# Карта «курс → у нас»

Справочник для сверки с CodeBase курса k-syndicate. Сверено по коду 2026-10-24 (курс — CodeBase урока 09, у нас — `b51f2fa`). Отступления из-за Unity 6 — не здесь, а в [`course-deviations.md`](course-deviations.md).

**Обновлять тем же изменением**, что переносит класс из курса, переименовывает или перемещает наш файл.

## Где брать код урока

- Корень: `G:\MediaGetDownloads\RealyGood\k-syndicate.school - Архитектура мобильных игр на UNITY для профессионалов (2021)\`
- **Папка урока N содержит код на начало урока N. Итог урока N лежит в папке урока N+1.** Что добавил урок N — это `diff` папок N и N+1 (`diff -r -u --strip-trailing-cr -x '*.meta'`). Прецедент: в папке `08 Enemy Spawner` нет `EnemySpawner.cs`, он появляется в `09 Static data` (2026-10-24).
- Уроки: 01 Input, 02 States, 03 DI, 04 Progress, 05 Enemy, 06 Fighting, 07 Fighting 2, 08 Enemy Spawner, 09 Static data, 10 Loot, 11 Spawner rework, 12–13 UI, 14 Rewarded ads, 15 Addressables, 16 InApps, 17 Build.
- Сослаться на курс: `<урок>\…\CodeBase\<путь>` (например `09 Static data\…\CodeBase\Logic\EnemySpawner.cs`).

## Имена

| Курс | У нас |
|---|---|
| `Hero*` (классы, поля, события) | `Player*` (`HeroGameObject` → `PlayerGameObject`, `HeroCreated` → `PlayerCreated`, `CreateHero` → `CreatePlayer`, `HeroState` → `PlayerState`, `HeroStats` → `PlayerStats`) |
| `ISavedProgress`, `ISavedProgressReader` | `ISaveProgress`, `ISaveProgressReader` (оба в одном файле `ISaveProgress.cs`) |
| `IGameFactory.Cleanup()` | `CleanUp()` |
| `GameFactory.InstantiateRegistered` | `InstantiateAndRegister` |
| `LoadLevelState.InitGameWorld` | `SetupGameEnvironment` |
| `IAssetProvider` / `AssetProvider` | `IAssetsProvider` / `AssetsProvider` |
| `AllServices.Container` | `ServiceLocator.Container` (API тот же: `RegisterSingle`, `Single`) |
| `DataExtensions.AsVectorData` / `AsUnityVector` / `ToDeserialized` | `AsVector3Data` / `AsVector3` / `ToDeserializes` |
| `EnemyDeath.Happaned` | `EnemyDeath.OnDeath` |
| `Attack` (враг) | `EnemyAttack` |
| `CameraFollow` | `CameraController` (метод `Follow` тот же) |
| `LookAtCamera` | `HpCamera` |
| `Game` | `GameMain` |
| `Constants.Epsilon` | нет (не переносили) |

## Папки и namespace

Namespace = `Game.Scripts.<путь от Assets/Game/Scripts>`; интерфейсы в подпапке `Interfaces/` → `….Interfaces`. Editor-скрипты — `Assets/Game/Editor/`, `Game.Editor`.

| Курс `CodeBase/` | У нас `Assets/Game/Scripts/` |
|---|---|
| `CameraLogic/` | `Camera/` |
| `Data/` | `Data/`; `DataExtensions` → `Data/Extensions/` |
| `Editor/` | `Assets/Game/Editor/` |
| `Enemy/` | `Enemy/` |
| `Hero/` | `Player/` (`HeroMove` → `PlayerMovementController`, `HeroAnimator` → `PlayerAnimator` + `PlayerFacing`) |
| `Infrastructure/Game.cs`, `GameBootstrapper.cs`, `GameRunner.cs` | `Core/` |
| `Infrastructure/ICoroutineRunner.cs` | `Infrastructure/Interfaces/` |
| `Infrastructure/SceneLoader.cs`, `Logic/LoadingCurtain.cs` | `StateMachine/` |
| `Infrastructure/States/` | `StateMachine/` (`GameStateMachine`), `StateMachine/States/`, `StateMachine/Interfaces/IState.cs` |
| `Infrastructure/AssetManagement/AssetPath.cs` | `Constants/AssetPath.cs` |
| `Infrastructure/AssetManagement/AssetProvider.cs` | `Providers/` (+ `Providers/Interfaces/`) |
| `Infrastructure/Factory/` | `Factories/` (+ `Factories/Interfaces/`) |
| `Infrastructure/Services/AllServices.cs`, `IService.cs` | `Infrastructure/ServiceLocator.cs`, `Infrastructure/IService.cs` |
| `Infrastructure/Services/PersistentProgress/`, `SaveLoad/` | `Services/PersistentProgress/`, `Services/SaveLoad/` |
| `Services/Input/` | `Services/Input/` (`InputService` + `InputReceiver`, см. отступление «Ввод») |
| `Logic/AnimatorState`, `AnimatorStateReporter`, `IAnimationStateReader` | `Animation/` (+ `Animation/Interfaces/`) |
| `Logic/IHealth.cs` | `BaseLogic/Interfaces/IHealth.cs` (`Game.Scripts.BaseLogic.Interfaces`) |
| `Logic/SaveTrigger.cs` | `Triggers/SaveTrigger.cs` |
| `Logic/` (прочее: `EnemySpawner`, `UniqueId`) | `BaseLogic/` — план урока 08 |
| `StaticData/` | `StaticData/` — план урока 08 |
| `UI/` | `UI/` (+ наш `MobileOnly`) |

## Пройденные уроки

| Урок | Статус | Задача |
|---|---|---|
| 01–04 | перенесено | — |
| 05 Enemy | перенесено (WIP Aggro другого агента) | `Ai.Issues/features/2026-09-19-aggro-wiring.md` |
| 06–07 Fighting | перенесено | `Ai.Tasks/2026-10-24-player-combat.md` |
| 08 Enemy Spawner | бриф | `Ai.Tasks/2026-10-24-enemy-spawner.md` |
