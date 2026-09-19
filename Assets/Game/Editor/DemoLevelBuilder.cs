using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Editor
{
    public static class DemoLevelBuilder
    {
        private const string SampleScenePath = "Assets/Game/Scenes/SampleScene.unity";
        private const string KitRoot =
            "Assets/Resources/Environment/AssetHunts!/GameDev Starter Kit - Platformer/Asset";
        private const string GeometryRootName = "DemoGeometry";

        [MenuItem("Tools/Level/Build Demo Geometry (SampleScene)")]
        public static void BuildDemoGeometry()
        {
            var scene = EditorSceneManager.OpenScene(SampleScenePath, OpenSceneMode.Single);

            Transform levelRoot = FindOrCreateLevelRoot();
            ClearPreviousGeometry(levelRoot);

            Transform geometry = new GameObject(GeometryRootName).transform;
            geometry.SetParent(levelRoot, false);

            Transform ground = CreateChild(geometry, "Ground");
            Transform platforms = CreateChild(geometry, "Platforms");
            Transform hazards = CreateChild(geometry, "Hazards");
            Transform decor = CreateChild(geometry, "Decor");

            GameObject groundTile = LoadPrefab($"{KitRoot}/3D Tile/3D_Tile_Ground_01.prefab");
            GameObject slopeTile = LoadPrefab($"{KitRoot}/3D Tile/3D_Tile_Ground_Slope_01.prefab");
            GameObject tree = LoadPrefab($"{KitRoot}/Plant/Plant_Tree_01.prefab");
            GameObject rock = LoadPrefab($"{KitRoot}/Rock/Rock_A_01.prefab");
            GameObject fence = LoadPrefab($"{KitRoot}/Prop/Prop_Fence_A_01.prefab");
            GameObject spikes = LoadPrefab($"{KitRoot}/Obstacle/Obstacle_Wooden_Spike_01.prefab");
            GameObject sign = LoadPrefab($"{KitRoot}/Prop/Prop_Wooden_Sign_01.prefab");

            float tileWidth = EstimateWidth(groundTile, fallback: 2f);
            float x = 0f;

            // Start runway
            for (int i = 0; i < 6; i++)
            {
                Place(groundTile, ground, new Vector3(x, 0f, 0f));
                x += tileWidth;
            }

            // Small step up
            Place(groundTile, platforms, new Vector3(x, 1f, 0f));
            x += tileWidth;
            Place(groundTile, platforms, new Vector3(x, 1f, 0f));
            x += tileWidth;

            // Gap then continue
            x += tileWidth * 0.75f;
            for (int i = 0; i < 4; i++)
            {
                Place(groundTile, ground, new Vector3(x, 0f, 0f));
                x += tileWidth;
            }

            // Slope + upper ledge
            if (slopeTile != null)
            {
                Place(slopeTile, platforms, new Vector3(x, 0f, 0f));
                x += tileWidth;
            }

            for (int i = 0; i < 3; i++)
            {
                Place(groundTile, platforms, new Vector3(x, 2f, 0f));
                x += tileWidth;
            }

            // Drop back + hazard pocket
            for (int i = 0; i < 3; i++)
            {
                Place(groundTile, ground, new Vector3(x, 0f, 0f));
                x += tileWidth;
            }

            if (spikes != null)
                Place(spikes, hazards, new Vector3(x - tileWidth * 1.5f, 0.1f, 0f));

            for (int i = 0; i < 4; i++)
            {
                Place(groundTile, ground, new Vector3(x, 0f, 0f));
                x += tileWidth;
            }

            // Finish marker
            if (sign != null)
                Place(sign, decor, new Vector3(x - tileWidth, 0f, 0.4f));

            // Background decor (does not affect route)
            if (tree != null)
            {
                Place(tree, decor, new Vector3(tileWidth * 2f, 0f, 1.2f));
                Place(tree, decor, new Vector3(tileWidth * 10f, 0f, 1.4f));
            }

            if (rock != null)
                Place(rock, decor, new Vector3(tileWidth * 5f, 0f, 1.1f));

            if (fence != null)
            {
                Place(fence, decor, new Vector3(tileWidth * 3f, 0f, -1.1f));
                Place(fence, decor, new Vector3(tileWidth * 8f, 0f, -1.1f));
            }

            EnsureColliders(ground);
            EnsureColliders(platforms);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            Debug.Log($"[DemoLevelBuilder] Built '{GeometryRootName}' under Level. InitialPoint/managers untouched.");
        }

        private static Transform FindOrCreateLevelRoot()
        {
            GameObject existing = GameObject.Find("Level");
            if (existing != null)
                return existing.transform;

            return new GameObject("Level").transform;
        }

        private static void ClearPreviousGeometry(Transform levelRoot)
        {
            Transform old = levelRoot.Find(GeometryRootName);
            if (old == null)
                return;

            Object.DestroyImmediate(old.gameObject);
        }

        private static Transform CreateChild(Transform parent, string name)
        {
            var child = new GameObject(name).transform;
            child.SetParent(parent, false);
            return child;
        }

        private static GameObject LoadPrefab(string path)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
                Debug.LogWarning($"[DemoLevelBuilder] Missing prefab: {path}");
            return prefab;
        }

        private static void Place(GameObject prefab, Transform parent, Vector3 worldPosition)
        {
            if (prefab == null)
                return;

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.transform.SetParent(parent, true);
            instance.transform.position = worldPosition;
            instance.transform.rotation = Quaternion.identity;
        }

        private static float EstimateWidth(GameObject prefab, float fallback)
        {
            if (prefab == null)
                return fallback;

            Renderer renderer = prefab.GetComponentInChildren<Renderer>();
            if (renderer == null)
                return fallback;

            float width = renderer.bounds.size.x;
            return width > 0.1f ? width : fallback;
        }

        private static void EnsureColliders(Transform root)
        {
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child.GetComponent<Collider>() != null)
                    continue;

                MeshFilter filter = child.GetComponent<MeshFilter>();
                if (filter == null || filter.sharedMesh == null)
                    continue;

                // Only add to mesh objects that look like tiles/props with renderers
                if (child.GetComponent<Renderer>() == null)
                    continue;

                MeshCollider meshCollider = child.gameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = filter.sharedMesh;
                meshCollider.convex = false;
            }
        }
    }
}
