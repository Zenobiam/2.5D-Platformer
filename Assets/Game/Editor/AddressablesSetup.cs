using System.Collections.Generic;
using System.Linq;
using Game.Scripts.BaseLogic;
using Game.Scripts.Constants;
using Game.Scripts.StaticData;
using Game.Scripts.Triggers;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Game.Editor
{
    /// <summary>
    /// Урок 15 Addressables. Три шага по порядку:
    /// 1. Move Addressable Prefabs — префабы урока из Resources в Game/Prefabs (AssetDatabase.MoveAsset: guid тот же,
    ///    ссылки сцен и StaticData целы). Addressables не держит адресуемые ассеты в Resources (иначе — Resources_moved).
    ///    Пути — из плана переноса project-structure-review.md § 6.
    /// 2. Addressables — префаб LevelTransferTrigger, адреса AssetAddress в Default Local Group,
    ///    PrefabReference у EnemyStaticData. Нужны настройки Addressables (окно Groups → Create Addressables Settings).
    /// 3. Second Level — копия SampleScene (новые UniqueId), сцена в Build Settings, LevelStaticData с переходами A ↔ B.
    /// Повторный запуск безопасен: перенесённое, созданное и уже заполненное пропускается.
    /// </summary>
    public static class AddressablesSetup
    {
        private const string PrefabsFolder = "Assets/Game/Prefabs";

        private const string PlayerPath = PrefabsFolder + "/Player/Player.prefab";
        private const string HudPath = PrefabsFolder + "/Player/HUD.prefab";
        private const string UIRootPath = PrefabsFolder + "/UI/UIRoot.prefab";
        private const string LootPath = PrefabsFolder + "/Loot/Loot.prefab";
        private const string SpawnPointPath = PrefabsFolder + "/Enemies/SpawnPoint.prefab";
        private const string EnemyPath = PrefabsFolder + "/Enemies/Enemy.prefab";
        private const string SkeletonPath = PrefabsFolder + "/Enemies/Skeleton.prefab";
        private const string GolemPath = PrefabsFolder + "/Enemies/Golem.prefab";
        private const string LevelTransferTriggerPath = PrefabsFolder + "/Triggers/LevelTransferTrigger.prefab";

        /// <summary>
        /// Откуда → куда. Enemy.prefab — база Variant'ов Skeleton и Golem: адреса у него нет, переносим вместе с ними.
        /// </summary>
        private static readonly (string From, string To)[] Moves =
        {
            ("Assets/Resources/Prefabs/Player/Player.prefab", PlayerPath),
            ("Assets/Resources/Prefabs/Player/HUD.prefab", HudPath),
            ("Assets/Resources/UI/UIRoot.prefab", UIRootPath),
            ("Assets/Resources/Loot/Loot.prefab", LootPath),
            ("Assets/Resources/Enemies/SpawnPoint.prefab", SpawnPointPath),
            ("Assets/Resources/Enemies/Enemy.prefab", EnemyPath),
            ("Assets/Resources/Enemies/Skeleton.prefab", SkeletonPath),
            ("Assets/Resources/Enemies/Golem.prefab", GolemPath),
        };

        /// <summary>
        /// Адрес → префаб. UIRoot — адрес из UIFactory.UIRootPath.
        /// </summary>
        private static readonly (string Address, string Path)[] Entries =
        {
            (AssetAddress.PlayerPath, PlayerPath),
            (AssetAddress.HudPath, HudPath),
            ("UIRoot", UIRootPath),
            (AssetAddress.Loot, LootPath),
            (AssetAddress.Spawner, SpawnPointPath),
            (AssetAddress.LevelTransferTrigger, LevelTransferTriggerPath),
            ("Skeleton", SkeletonPath),
            ("Golem", GolemPath),
        };

        private static readonly Dictionary<EnemyTypeId, string> EnemyPrefabs = new Dictionary<EnemyTypeId, string>
        {
            [EnemyTypeId.Skeleton] = SkeletonPath,
            [EnemyTypeId.Golem] = GolemPath,
        };

        private const string EnemiesDataFolder = "Assets/Resources/StaticData/Enemies";

        private const string FirstScenePath = "Assets/Game/Scenes/SampleScene.unity";
        private const string SecondScenePath = "Assets/Game/Scenes/SecondLevel.unity";
        private const string FirstLevelDataPath = "Assets/Resources/StaticData/Levels/BasicLevel.asset";
        private const string SecondLevelDataPath = "Assets/Resources/StaticData/Levels/SecondLevel.asset";

        [MenuItem("Tools/Setup/Move Addressable Prefabs")]
        public static void MovePrefabs()
        {
            if (Application.isPlaying)
            {
                Debug.LogError("Move addressable prefabs: выйдите из Play Mode.");
                return;
            }

            foreach ((string from, string to) in Moves)
            {
                if (AssetDatabase.LoadAssetAtPath<GameObject>(to) != null)
                {
                    Debug.Log($"Move addressable prefabs: уже на месте {to}");
                    continue;
                }

                EnsureFolder(System.IO.Path.GetDirectoryName(to).Replace('\\', '/'));

                string error = AssetDatabase.MoveAsset(from, to);
                if (string.IsNullOrEmpty(error))
                    Debug.Log($"Move addressable prefabs: {from} → {to}");
                else
                    Debug.LogError($"Move addressable prefabs: {from} → {to}: {error}");
            }

            AssetDatabase.SaveAssets();
        }

        [MenuItem("Tools/Setup/Addressables")]
        public static void SetupAddressables()
        {
            if (Application.isPlaying)
            {
                Debug.LogError("Addressables setup: выйдите из Play Mode.");
                return;
            }

            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("Addressables setup: нет настроек — Window → Asset Management → Addressables → Groups → Create Addressables Settings.");
                return;
            }

            BuildLevelTransferTrigger();

            foreach ((string address, string path) in Entries)
                MarkAddressable(settings, address, path);

            foreach (EnemyStaticData enemyData in LoadAll<EnemyStaticData>(EnemiesDataFolder))
                SetPrefabReference(enemyData);

            AssetDatabase.SaveAssets();
        }

        [MenuItem("Tools/Setup/Second Level")]
        public static void SetupSecondLevel()
        {
            if (Application.isPlaying)
            {
                Debug.LogError("Second level: выйдите из Play Mode.");
                return;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(SecondScenePath) == null)
            {
                if (!AssetDatabase.CopyAsset(FirstScenePath, SecondScenePath))
                {
                    Debug.LogError($"Second level: не удалось скопировать {FirstScenePath} → {SecondScenePath}");
                    return;
                }

                RegenerateUniqueIds(SecondScenePath);
                Debug.Log($"Second level: {SecondScenePath} — копия {FirstScenePath}, UniqueId новые");
                // NavMeshSurface.OnValidate обнуляет ссылку на NavMesh в копии («Duplicating NavMeshSurface does not
                // duplicate the referenced NavMesh data») — без запекания агенты врагов не создаются и висят в точке спавна.
                Debug.LogWarning($"Second level: откройте {SecondScenePath}, у NavMeshSurface (DemoRoute) нажмите Bake и сохраните сцену.");
            }

            AddToBuildSettings(SecondScenePath);

            string firstScene = System.IO.Path.GetFileNameWithoutExtension(FirstScenePath);
            string secondScene = System.IO.Path.GetFileNameWithoutExtension(SecondScenePath);

            var firstLevel = AssetDatabase.LoadAssetAtPath<LevelStaticData>(FirstLevelDataPath);
            SetTransferTo(firstLevel, secondScene);

            var secondLevel = AssetDatabase.LoadAssetAtPath<LevelStaticData>(SecondLevelDataPath);
            if (secondLevel == null)
            {
                secondLevel = ScriptableObject.CreateInstance<LevelStaticData>();
                secondLevel.LevelKey = secondScene;
                secondLevel.EnemySpawners = new List<EnemySpawnerStaticData>();
                AssetDatabase.CreateAsset(secondLevel, SecondLevelDataPath);
                Debug.Log($"Second level: создан {SecondLevelDataPath} — откройте сцену {secondScene} и нажмите Collect");
            }

            SetTransferTo(secondLevel, firstScene);

            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Триггер перехода: BoxCollider isTrigger поперёк пути игрока (игрок идёт по X) и LevelTransferTrigger.
        /// TransferTo в префабе пустой — его ставит GameFactory из LevelStaticData.
        /// </summary>
        private static void BuildLevelTransferTrigger()
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(LevelTransferTriggerPath) != null)
                return;

            EnsureFolder(PrefabsFolder + "/Triggers");

            var root = new GameObject("LevelTransferTrigger");
            try
            {
                var collider = root.AddComponent<BoxCollider>();
                collider.isTrigger = true;
                collider.center = new Vector3(0, 1.5f, 0);
                collider.size = new Vector3(1, 3, 3);

                var trigger = root.AddComponent<LevelTransferTrigger>();
                trigger.Collider = collider;

                PrefabUtility.SaveAsPrefabAsset(root, LevelTransferTriggerPath);
                Debug.Log($"Addressables setup: создан {LevelTransferTriggerPath}");
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        private static void MarkAddressable(AddressableAssetSettings settings, string address, string path)
        {
            string guid = AssetDatabase.AssetPathToGUID(path);
            if (string.IsNullOrEmpty(guid))
            {
                Debug.LogError($"Addressables setup: нет {path} — сначала Tools → Setup → Move Addressable Prefabs.");
                return;
            }

            AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, settings.DefaultGroup);
            entry.address = address;
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
            Debug.Log($"Addressables setup: {path} → адрес «{address}» в {settings.DefaultGroup.Name}");
        }

        private static void SetPrefabReference(EnemyStaticData enemyData)
        {
            if (!EnemyPrefabs.TryGetValue(enemyData.EnemyTypeId, out string path))
            {
                Debug.LogError($"Addressables setup: нет префаба для {enemyData.EnemyTypeId} ({enemyData.name}).");
                return;
            }

            string guid = AssetDatabase.AssetPathToGUID(path);
            if (enemyData.PrefabReference != null && enemyData.PrefabReference.AssetGUID == guid)
                return;

            enemyData.PrefabReference = new AssetReferenceGameObject(guid);
            EditorUtility.SetDirty(enemyData);
            Debug.Log($"Addressables setup: {enemyData.name}.PrefabReference → {path}");
        }

        /// <summary>
        /// Копия сцены несёт те же UniqueId спавнеров: убитый враг на одном уровне считался бы убитым и на другом
        /// (KillData.ClearedSpawners хранит id). UniqueIdEditor ищет дубликаты только в открытой сцене — генерируем здесь.
        /// </summary>
        private static void RegenerateUniqueIds(string scenePath)
        {
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            try
            {
                foreach (UniqueId uniqueId in scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<UniqueId>(true)))
                {
                    uniqueId.GenerateId();
                    EditorUtility.SetDirty(uniqueId);
                }

                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
            finally
            {
                EditorSceneManager.CloseScene(scene, true);
            }
        }

        private static void AddToBuildSettings(string scenePath)
        {
            List<EditorBuildSettingsScene> scenes = EditorBuildSettings.scenes.ToList();
            if (scenes.Any(scene => scene.path == scenePath))
                return;

            scenes.Add(new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log($"Second level: {scenePath} добавлена в Build Settings");
        }

        private static void SetTransferTo(LevelStaticData levelData, string transferTo)
        {
            if (levelData == null)
                return;

            levelData.LevelTransfer ??= new LevelTransferStaticData(transferTo, Vector3.zero);
            if (!string.IsNullOrEmpty(levelData.LevelTransfer.TransferTo))
                return;

            levelData.LevelTransfer.TransferTo = transferTo;
            EditorUtility.SetDirty(levelData);
            Debug.Log($"Second level: {levelData.name}.LevelTransfer.TransferTo = {transferTo}");
        }

        private static IEnumerable<T> LoadAll<T>(string folder) where T : Object =>
            AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { folder })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<T>);

        private static void EnsureFolder(string folder)
        {
            if (AssetDatabase.IsValidFolder(folder))
                return;

            string parent = System.IO.Path.GetDirectoryName(folder).Replace('\\', '/');
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, System.IO.Path.GetFileName(folder));
        }
    }
}
