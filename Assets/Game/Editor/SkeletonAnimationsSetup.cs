using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Game.Editor
{
    // Простые клипы Damage и Death для Skeleton, пока нет настоящих анимаций под его риг.
    // Анимируется узел DeformationSystem (родитель рига): его не трогает ни один другой клип.
    // Повторный запуск перезаписывает кривые, а не добавляет новые.
    public static class SkeletonAnimationsSetup
    {
        private const string FbxPath = "Assets/Resources/Enemies/Skeleton/FBX/Skeleton.fbx";
        private const string ControllerPath = "Assets/Resources/Enemies/Skeleton/FBX/Skeleton.controller";
        private const string DamageClipPath = "Assets/Resources/Enemies/Skeleton/Animations/Damage.anim";
        private const string DeathClipPath = "Assets/Resources/Enemies/Skeleton/Animations/Death.anim";

        private const string RigNodePath = "DeformationSystem";
        private const string DeathStateName = "Death";

        // Наклон корпуса при уроне и при смерти (градусы вокруг X узла рига).
        private const float DamageTilt = -20f;
        private const float DeathTilt = -90f;

        [MenuItem("Tools/Setup/Skeleton Animations")]
        public static void Setup()
        {
            if (Application.isPlaying)
            {
                Debug.LogError("Skeleton animations setup: выйдите из Play Mode.");
                return;
            }

            var fbx = AssetDatabase.LoadAssetAtPath<GameObject>(FbxPath);
            Transform rigNode = fbx != null ? fbx.transform.Find(RigNodePath) : null;
            if (rigNode == null)
            {
                Debug.LogError($"Skeleton animations setup: в {FbxPath} нет узла {RigNodePath}.");
                return;
            }

            Vector3 baseRotation = rigNode.localEulerAngles;

            var damage = AssetDatabase.LoadAssetAtPath<AnimationClip>(DamageClipPath);
            var death = AssetDatabase.LoadAssetAtPath<AnimationClip>(DeathClipPath);
            if (damage == null || death == null)
            {
                Debug.LogError($"Skeleton animations setup: нет клипов {DamageClipPath} или {DeathClipPath}.");
                return;
            }

            // Damage: быстрый наклон и возврат за 0.4 с.
            FillTilt(damage, baseRotation,
                new Keyframe(0f, 0f), new Keyframe(0.1f, DamageTilt), new Keyframe(0.4f, 0f));

            // Death: падение за 0.6 с, дальше поза держится до удаления врага.
            FillTilt(death, baseRotation,
                new Keyframe(0f, 0f), new Keyframe(0.6f, DeathTilt), new Keyframe(1.2f, DeathTilt));

            AssignDeathClip(death);

            AssetDatabase.SaveAssets();
            Debug.Log($"Skeleton animations setup: Damage ({DamageTilt}°) и Death ({DeathTilt}°) по узлу {RigNodePath}; состояние {DeathStateName} → {DeathClipPath}.");
        }

        private static void FillTilt(AnimationClip clip, Vector3 baseRotation, params Keyframe[] tiltKeys)
        {
            clip.ClearCurves();

            var tilt = new AnimationCurve();
            foreach (Keyframe key in tiltKeys)
                tilt.AddKey(key.time, baseRotation.x + key.value);

            float end = tiltKeys[tiltKeys.Length - 1].time;
            SetCurve(clip, "localEulerAnglesRaw.x", tilt);
            SetCurve(clip, "localEulerAnglesRaw.y", AnimationCurve.Constant(0f, end, baseRotation.y));
            SetCurve(clip, "localEulerAnglesRaw.z", AnimationCurve.Constant(0f, end, baseRotation.z));

            AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = false;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            EditorUtility.SetDirty(clip);
        }

        private static void SetCurve(AnimationClip clip, string property, AnimationCurve curve) =>
            AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve(RigNodePath, typeof(Transform), property), curve);

        private static void AssignDeathClip(AnimationClip death)
        {
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
            foreach (ChildAnimatorState child in controller.layers[0].stateMachine.states)
            {
                if (child.state.name != DeathStateName)
                    continue;

                child.state.motion = death;
                EditorUtility.SetDirty(controller);
                return;
            }

            Debug.LogError($"Skeleton animations setup: в {ControllerPath} нет состояния {DeathStateName}.");
        }
    }
}
