using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Game.Editor
{
    public static class BuildDemoLevel
    {
        private const string SampleScenePath = "Assets/Game/Scenes/SampleScene.unity";
        private const string KitRoot =
            "Assets/Resources/Environment/AssetHunts!/GameDev Starter Kit - Platformer/Asset";
        private const string DemoRootName = "DemoRoute";

        private const string GroundTile = KitRoot + "/3D Tile/3D_Tile_Ground_01.prefab";
        private const string GroundTileAlt = KitRoot + "/3D Tile/3D_Tile_Ground_02.prefab";
        private const string SlopeTile = KitRoot + "/3D Tile/3D_Tile_Ground_Slope_01.prefab";
        private const string TreePrefab = KitRoot + "/Plant/Plant_Tree_01.prefab";
        private const string RockPrefab = KitRoot + "/Rock/Rock_A_01.prefab";
        private const string FencePrefab = KitRoot + "/Prop/Prop_Fence_A_01.prefab";
        private const string SpikePrefab = KitRoot + "/Obstacle/Obstacle_Wooden_Spike_01.prefab";
        private const string SignPrefab = KitRoot + "/Prop/Prop_Wooden_Sign_01.prefab";

        [MenuItem("Tools/Build Demo Level Geometry")]
        public static void Build()
        {
            var scene = EditorSceneManager.OpenScene(SampleScenePath, OpenSceneMode.Single);

            Transform levelRoot = FindOrCreateLevelRoot();
            ClearPreviousDemoRoute(levelRoot);

            Transform demoRoot = new GameObject(DemoRootName).transform;
            demoRoot.SetParent(levelRoot, false);

            Transform ground = CreateFolder(demoRoot, "Ground");
            Transform decor = CreateFolder(demoRoot, "Decor");
            Transform hazards = CreateFolder(demoRoot, "Hazards");

            // Simple left-to-right 2.5D route along X. Does not touch InitialPoint / managers.
            float x = -6f;
            float y = 0f;
            const float step = 2f;

            // Start platform
            for (int i = 0; i < 5; i++)
            {
                Place(GroundTile, ground, new Vector3(x, y, 0f));
                x += step;
            }

            // Small step up
            y = 1f;
            for (int i = 0; i < 3; i++)
            {
                Place(i % 2 == 0 ? GroundTile : GroundTileAlt, ground, new Vector3(x, y, 0f));
                x += step;
            }

            // Gap then landing
            x += step;
            y = 0f;
            for (int i = 0; i < 4; i++)
            {
                Place(GroundTile, ground, new Vector3(x, y, 0f));
                x += step;
            }

            // Slope climb
            Place(SlopeTile, ground, new Vector3(x, y, 0f));
            x += step;
            y = 1.5f;
            for (int i = 0; i < 3; i++)
            {
                Place(GroundTileAlt, ground, new Vector3(x, y, 0f));
                x += step;
            }

            // Finish pad
            y = 1.5f;
            for (int i = 0; i < 4; i++)
            {
                Place(GroundTile, ground, new Vector3(x, y, 0f));
                x += step;
            }

            float finishX = x - step;

            // Decor (background Z)
            Place(TreePrefab, decor, new Vector3(-4f, 0f, 2.5f));
            Place(TreePrefab, decor, new Vector3(8f, 0f, 2.5f));
            Place(RockPrefab, decor, new Vector3(2f, 0f, 2f));
            Place(FencePrefab, decor, new Vector3(12f, 0f, 1.5f));
            Place(SignPrefab, decor, new Vector3(finishX, 1.5f, 0.5f));

            // One hazard on the low section after the gap
            Place(SpikePrefab, hazards, new Vector3(8f, 0f, 0f));

            EnsureColliders(demoRoot);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            Debug.Log(
                $"Demo level geometry built under Level/{DemoRootName}. InitialPoint and other systems untouched.");
        }

        private static Transform FindOrCreateLevelRoot()
        {
            GameObject existing = GameObject.Find("Level");
            if (existing != null)
                return existing.transform;

            var created = new GameObject("Level");
            return created.transform;
        }

        private static void ClearPreviousDemoRoute(Transform levelRoot)
        {
            Transform previous = levelRoot.Find(DemoRootName);
            if (previous != null)
                Object.DestroyImmediate(previous.gameObject);

            // Remove old placeholder cubes under Level (not InitialPoint / managers).
            for (int i = levelRoot.childCount - 1; i >= 0; i--)
            {
                Transform child = levelRoot.GetChild(i);
                if (child.name.StartsWith("Cube"))
                    Object.DestroyImmediate(child.gameObject);
            }
        }

        private static Transform CreateFolder(Transform parent, string name)
        {
            var folder = new GameObject(name).transform;
            folder.SetParent(parent, false);
            return folder;
        }

        private static void Place(string prefabPath, Transform parent, Vector3 position)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null)
            {
                Debug.LogWarning($"Prefab not found: {prefabPath}");
                return;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            instance.transform.localPosition = position;
            instance.transform.localRotation = Quaternion.identity;
        }

        private static void EnsureColliders(Transform demoRoot)
        {
            Transform ground = demoRoot.Find("Ground");
            if (ground == null)
                return;

            foreach (Transform tile in ground)
            {
                if (tile.GetComponentInChildren<Collider>() != null)
                    continue;

                MeshFilter filter = tile.GetComponentInChildren<MeshFilter>();
                if (filter != null && filter.sharedMesh != null)
                {
                    var meshCollider = filter.gameObject.AddComponent<MeshCollider>();
                    meshCollider.sharedMesh = filter.sharedMesh;
                    continue;
                }

                tile.gameObject.AddComponent<BoxCollider>();
            }
        }
    }
}
