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
| `LoadLevelState` | `LoadSceneState` (переименован в уроке 08, 2026-10-25) |
| `LoadLevelState.InitGameWorld` | `LoadSceneState.SetupGameEnvironment` |
| `EnemySpawner.Slain` | `EnemySpawner.isEmpty` |
| `Monster*` (урок 09): `MonsterStaticData`, `MonsterTypeId` (`Lich`, `Golem = 10`), `IGameFactory.CreateMonster`, `IStaticDataService.LoadMonsters` / `ForMonster` | `Enemy*`: `EnemyStaticData`, `EnemyTypeId` (`Skeleton = 0`, `Golem = 10`), `CreateEnemy`, `LoadEnemies` / `ForEnemy` |
| `Resources/Static Data/Monsters` | `Resources/StaticData/Enemies` |
| `RotateToHero` | `RotateToPlayer` |
| `Attack` (враг): `Damage`, `Cleavage`, `EffectiveDistance` | `EnemyAttack`: те же поля (опечатка `EffecticveDistance` исправлена в уроке 09) |
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
| `Logic/EnemySpawner.cs` (до урока 11), `Logic/UniqueId.cs` | `UniqueId` — `BaseLogic/UniqueId.cs` (`Game.Scripts.BaseLogic`, перенесён пользователем в уроке 11; путь = namespace); `EnemySpawner` в уроке 11 переименован курсом в `SpawnPoint` (+ `SpawnMarker` на сцене); у нас — новый `SpawnPoint.cs`, старый удалён |
| `Services/Randomizer/IRandomService.cs`, `RandomService.cs` (урок 10, в видео не пишется — есть только в итоге) | `Services/Randomizer/Interfaces/IRandomService.cs`, `Services/Randomizer/RandomService.cs` |
| `Enemy/LootPiece.cs`, `Enemy/LootSpawner.cs` (урок 10) | `BaseLogic/LootLogic/LootPiece.cs`, `EnemyLootSpawner.cs` (`Game.Scripts.BaseLogic.LootLogic`). Папка не `Loot`: namespace `….BaseLogic.Loot` перекрывал класс данных `Data.Loot` — приходилось писать `Data.Loot` (решение пользователя 2026-10-27) |
| `UI/LootCounter.cs` + `Resources/Hud/Loot.prefab` (вложен в `Hud`), иконка `Hud/CFX3_T_Skull.png` (урок 10) | `UI/LootCounter.cs`; объект `HUD/LootCounter` собирает `Tools → UI → Build Player HUD`; иконка `Resources/Prefabs/Player/CFX3_T_Skull.png` (тот же guid) |
| `Data/DictionaryData_Base.cs`, `LootPieceData.cs`, `LootPieceDataDictionary.cs`; `LootData.LootPiecesOnScene`; `UniqueId.GenerateId()`; `LoadLevelState.InitLootPieces` (урок 10, задание «10 WireUpTestAndTask») | те же имена в `Data/`; `UniqueId.GenerateId()` (его же вызывает `UniqueIdEditor`); `LoadSceneState.InitLootPieces` |
| `StaticData/LevelStaticData.cs`, `EnemySpawnerStaticData.cs`; `Logic/EnemySpawners/SpawnMarker.cs`, `SpawnPoint.cs`; `Editor/LevelStaticDataEditor.cs`, `SpawnMarkerEditor.cs` (урок 11) | `StaticData/` те же; `BaseLogic/EnemySpawners/` (`Game.Scripts.BaseLogic.EnemySpawners`); `Game/Editor/` |
| `Data/SerializableDictionary.cs` (урок 11, было `DictionaryData_Base`) | `Data/SerializableDictionary.cs` |
| `IStaticDataService.Load()`, `ForLevel`; `Resources/Static Data/Levels` | `Load()`, `ForLevel`; `Resources/StaticData/Levels` |
| `IGameFactory.CreateSpawner`; `Register` — private (урок 11) | то же |
| `Resources/UI/{Window,Shop,ShopItem}.prefab`, паки `Plugins/{Simple Fantasy GUI,Free RPG Icons,GUI CARTOON}` (урок 12) | `Resources/UI/` — те же имена и иерархия (собирает `ShopWindowBuilder`); паки — `ThirdParty/{Simple Fantasy GUI,RPG icons,2D Casual UI}` |
| `StaticData/MonsterStaticData.cs` | `StaticData/EnemyStaticData.cs` (`Game.Scripts.StaticData`) |
| `Services/StaticData/IStaticDataService.cs`, `StaticDataService.cs` | `Services/StaticData/Interfaces/IStaticDataService.cs`, `Services/StaticData/StaticDataService.cs` |
| `Resources/Enemies/Golem/` (пак) + `Golem.prefab` (Variant от `BaseEnemy`) | `Resources/Enemies/Golem/` (пак без курсовых `Golem.prefab` и `OverrideController`) + `Resources/Enemies/Golem.prefab` (Variant от `Enemy`, `Tools → Setup → Enemy Variant: Golem`) |
| `StaticData/` | `StaticData/` (`Game.Scripts.StaticData`): `EnemyStaticData`, `EnemyTypeId` |
| `Resources/Loot/` (урок 10: `Loot`, `PickupPopup`, `LootPickupFX`, `CFX3_MagicAura_D_Runic`) | `Resources/Loot/` — те же файлы и guid (перенесено 2026-10-25, только ассеты) |
| `Resources/Levels/Graveyard/raw/mobile_models/mobile_skull_03.fbx` + `Materials/cemetery_halloween_atlas.mat` + `textures/cemetery_halloween_atlas.tga` | `Resources/Loot/Skull/` (атлас — `.png`, тот же guid) |
| `Plugins/JMO Assets/Cartoon FX/` (часть: 4 материала, 4 текстуры, `CFX_Mesh_Circular1.5`, `CFX_AutoDestructShuriken.cs`) | `Resources/Effects/JMO Assets/Cartoon FX/` — та же структура подпапок |
| `UI/` | `UI/` (+ наш `MobileOnly`) |

## Пройденные уроки

| Урок | Статус | Задача |
|---|---|---|
| 01–04 | перенесено | — |
| 05 Enemy | перенесено (WIP Aggro другого агента) | `Ai.Issues/features/2026-09-19-aggro-wiring.md` |
| 06–07 Fighting | перенесено | `Ai.Tasks/2026-10-24-player-combat.md` |
| 08 Enemy Spawner | перенесено (2026-10-25, `b01d2a7`) | `Ai.Tasks/2026-10-24-enemy-spawner.md` (локально) |
| 09 Static data | перенесено (2026-10-25) + новый враг Golem | `Ai.Tasks/2026-10-25-static-data.md` (локально) |
| 10 Loot | перенесено (2026-10-27): лут с врагов, `RandomService`, счётчик на HUD, сохранение лута на сцене; души сохраняются при подборе (отступление) | `Ai.Tasks/2026-10-27-loot-hud.md`, `2026-10-27-loot-save.md` (локально) |
| 11 Spawner rework | код перенесён (2026-10-28, `e066983`), **Play Mode не проверен** | `Ai.Tasks/2026-10-28-spawner-rework.md` (локально) |
| 12 UI part 1 | перенесено (2026-10-30): окно магазина `Resources/UI/{Window,ShopItem,Shop}` — `Tools → UI → Build Shop Window`; паки UI в `ThirdParty/` | `Ai.Tasks/2026-10-30-ui-shop-layout.md` (локально) |
