using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Editor
{
    // Новый враг = Prefab Variant от Enemy.prefab (логика) + модель + override-контроллер на базе Skeleton.controller.
    // Модель раскладывается прямо под корень Variant'а (как у Skeleton): клипы пишут пути от корня модели,
    // а Animator висит на корне врага — иначе анимации не найдут кости.
    // Повторный запуск: Exit Time атак в общем контроллере выставляется в 1, override-контроллер пересобирается,
    // существующий Variant не трогается (удалить вручную, чтобы собрать заново).
    public static class EnemyVariantSetup
    {
        private const string EnemyBasePath = "Assets/Resources/Enemies/Enemy.prefab";
        private const string BaseControllerPath = "Assets/Resources/Enemies/Skeleton/FBX/Skeleton.controller";

        private const string GolemModelPath = "Assets/Resources/Enemies/Golem/Prefabs/PBR_Golem.prefab";
        private const string GolemClipsFolder = "Assets/Resources/Enemies/Golem/Animations";
        private const string GolemOverridePath = "Assets/Resources/Enemies/Golem/Golem.overrideController";
        private const string GolemVariantPath = "Assets/Resources/Enemies/Golem.prefab";

        // Клип Skeleton.controller → клип голема. RightAttack вызывает EnemyAnimator.PlayRightAttack(),
        // поэтому на него Attack01: только в нём есть оба события OnAttack и OnAttackEnded (в Attack02 нет OnAttackEnded).
        private static readonly Dictionary<string, string> GolemClips = new Dictionary<string, string>
        {
            ["IDLE"] = "Idle",
            ["Walk"] = "Walk",
            ["Run"] = "Walk",
            ["RightAttack"] = "Attack01",
            ["LeftAttack"] = "Attack02",
            ["SplashAttack"] = "Attack01",
            ["Damage"] = "GetHit",
            ["Death"] = "Die",
        };

        // Состояния атак общего контроллера: выход из них — только в конце клипа.
        // Иначе события позже точки выхода (OnAttackEnded у длинных клипов, например Attack01 голема) не вызываются.
        private static readonly string[] AttackStates = { "LeftAttack", "RightAttack", "SplashAttack" };
        private const float AttackExitTime = 1f;

        [MenuItem("Tools/Setup/Enemy Variant: Golem")]
        public static void SetupGolem()
        {
            if (Application.isPlaying)
            {
                Debug.LogError("Enemy variant setup: выйдите из Play Mode.");
                return;
            }

            SetAttackExitTime();

            AnimatorOverrideController overrideController = BuildOverrideController(GolemOverridePath, GolemClipsFolder, GolemClips);
            if (overrideController == null)
                return;

            CreateVariant(GolemModelPath, GolemVariantPath, overrideController);
            AssetDatabase.SaveAssets();
        }

        private static void SetAttackExitTime()
        {
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(BaseControllerPath);
            if (controller == null)
            {
                Debug.LogError($"Enemy variant setup: нет {BaseControllerPath}.");
                return;
            }

            var changed = new List<string>();
            foreach (ChildAnimatorState child in controller.layers[0].stateMachine.states)
            {
                if (System.Array.IndexOf(AttackStates, child.state.name) < 0)
                    continue;

                foreach (AnimatorStateTransition transition in child.state.transitions)
                {
                    if (!transition.hasExitTime || Mathf.Approximately(transition.exitTime, AttackExitTime))
                        continue;

                    string destination = transition.destinationState != null ? transition.destinationState.name : "Exit";
                    changed.Add($"{child.state.name} → {destination} ({transition.exitTime} → {AttackExitTime})");
                    transition.exitTime = AttackExitTime;
                    EditorUtility.SetDirty(transition);
                }
            }

            EditorUtility.SetDirty(controller);
            Debug.Log(changed.Count > 0
                ? $"Enemy variant setup: Exit Time атак в {BaseControllerPath}: {string.Join("; ", changed)}."
                : $"Enemy variant setup: Exit Time атак в {BaseControllerPath} уже {AttackExitTime}.");
        }

        private static AnimatorOverrideController BuildOverrideController(string path, string clipsFolder, Dictionary<string, string> clipMap)
        {
            var baseController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(BaseControllerPath);
            if (baseController == null)
            {
                Debug.LogError($"Enemy variant setup: нет {BaseControllerPath}.");
                return null;
            }

            var overrideController = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(path);
            if (overrideController == null)
            {
                overrideController = new AnimatorOverrideController(baseController);
                AssetDatabase.CreateAsset(overrideController, path);
            }
            else
            {
                overrideController.runtimeAnimatorController = baseController;
            }

            var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            overrideController.GetOverrides(overrides);

            for (int i = 0; i < overrides.Count; i++)
            {
                AnimationClip original = overrides[i].Key;
                if (!clipMap.TryGetValue(original.name, out string clipName))
                {
                    Debug.LogWarning($"Enemy variant setup: для клипа {original.name} нет замены — останется клип Skeleton.");
                    continue;
                }

                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{clipsFolder}/{clipName}.anim");
                if (clip == null)
                {
                    Debug.LogError($"Enemy variant setup: нет клипа {clipsFolder}/{clipName}.anim.");
                    continue;
                }

                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(original, clip);
            }

            overrideController.ApplyOverrides(overrides);
            EditorUtility.SetDirty(overrideController);
            Debug.Log($"Enemy variant setup: {path} — {overrides.Count} клипов от {BaseControllerPath}.");
            return overrideController;
        }

        private static void CreateVariant(string modelPath, string variantPath, RuntimeAnimatorController controller)
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(variantPath) != null)
            {
                Debug.Log($"Enemy variant setup: {variantPath} уже есть — не трогаю (удалите вручную, чтобы собрать заново).");
                return;
            }

            var enemyBase = AssetDatabase.LoadAssetAtPath<GameObject>(EnemyBasePath);
            var modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
            if (enemyBase == null || modelPrefab == null)
            {
                Debug.LogError($"Enemy variant setup: нет {EnemyBasePath} или {modelPath}.");
                return;
            }

            // Временная сцена: открытая сцена не помечается изменённой.
            Scene previewScene = EditorSceneManager.NewPreviewScene();
            var enemy = (GameObject)PrefabUtility.InstantiatePrefab(enemyBase, previewScene);
            var model = (GameObject)PrefabUtility.InstantiatePrefab(modelPrefab, previewScene);
            try
            {
                var modelAnimator = model.GetComponent<Animator>();
                Avatar avatar = modelAnimator != null ? modelAnimator.avatar : null;
                PrefabUtility.UnpackPrefabInstance(model, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

                // Кости и меш — прямо под корень врага, контейнер модели удаляем.
                Transform modelRoot = model.transform;
                modelRoot.SetPositionAndRotation(enemy.transform.position, enemy.transform.rotation);
                while (modelRoot.childCount > 0)
                    modelRoot.GetChild(0).SetParent(enemy.transform, worldPositionStays: true);

                var animator = enemy.GetComponent<Animator>();
                animator.runtimeAnimatorController = controller;
                animator.avatar = avatar;

                enemy.name = System.IO.Path.GetFileNameWithoutExtension(variantPath);
                PrefabUtility.SaveAsPrefabAsset(enemy, variantPath);
                Debug.Log($"Enemy variant setup: создан {variantPath} (Variant от {EnemyBasePath}), Avatar {(avatar != null ? avatar.name : "НЕТ")}. Проверьте размер HitBox, зон Aggro/Attack и высоту полоски HP.");
            }
            finally
            {
                EditorSceneManager.ClosePreviewScene(previewScene);
            }
        }
    }
}
