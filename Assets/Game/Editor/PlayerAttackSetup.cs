using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Services.Input;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Editor
{
    // Подключает атаку игрока: событие Attack в PlayerInput и состояние Hit в FreeHandController.
    // Повторный запуск ничего не дублирует.
    public static class PlayerAttackSetup
    {
        private const string ScenePath = "Assets/Game/Scenes/SampleScene.unity";
        private const string InputHandlerName = "InputHandler";
        private const string AttackActionPath = "Player/Attack";

        private const string ControllerPath = "Assets/Resources/Yurowm/Controllers/FreeHandController.controller";
        private const string HitClipPath = "Assets/Resources/Yurowm/Animations/Hit.FBX";
        private const string HitStateName = "Hit";
        private const string IdleStateName = "Idle";
        private const string AttackTrigger = "Attack";

        [MenuItem("Tools/Setup/Player Attack")]
        public static void Setup()
        {
            if (Application.isPlaying)
            {
                Debug.LogError("Player Attack setup: выйдите из Play Mode.");
                return;
            }

            SetupInput();
            SetupAnimator();
        }

        private static void SetupInput()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            var scene = EditorSceneManager.OpenScene(ScenePath);

            GameObject inputHandler = GameObject.Find(InputHandlerName);
            if (inputHandler == null)
            {
                Debug.LogError($"Player Attack setup: в {ScenePath} нет объекта {InputHandlerName}.");
                return;
            }

            var playerInput = inputHandler.GetComponent<PlayerInput>();
            var receiver = inputHandler.GetComponent<InputReceiver>();
            InputAction attack = playerInput.actions.FindAction(AttackActionPath);
            if (attack == null)
            {
                Debug.LogError($"Player Attack setup: нет действия {AttackActionPath} в {playerInput.actions.name}.");
                return;
            }

            List<PlayerInput.ActionEvent> events = playerInput.actionEvents.ToList();
            PlayerInput.ActionEvent attackEvent = events.FirstOrDefault(e => e.actionId == attack.id.ToString());
            if (attackEvent == null)
            {
                attackEvent = new PlayerInput.ActionEvent(attack);
                events.Add(attackEvent);
            }

            if (!HasListener(attackEvent, receiver, nameof(InputReceiver.OnAttack)))
                UnityEventTools.AddPersistentListener(attackEvent, receiver.OnAttack);

            playerInput.actionEvents = events.ToArray();

            EditorUtility.SetDirty(playerInput);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"Player Attack setup: {AttackActionPath} → InputReceiver.OnAttack ({ScenePath}).");
        }

        private static void SetupAnimator()
        {
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
            AnimatorStateMachine root = controller.layers[0].stateMachine;

            AnimatorState idle = FindState(root, IdleStateName);
            if (idle == null)
            {
                Debug.LogError($"Player Attack setup: в {ControllerPath} нет состояния {IdleStateName}.");
                return;
            }

            AnimatorState hit = FindState(root, HitStateName);
            if (hit == null)
            {
                AnimationClip clip = AssetDatabase.LoadAllAssetsAtPath(HitClipPath)
                    .OfType<AnimationClip>()
                    .FirstOrDefault(c => c.name == HitStateName);
                if (clip == null)
                {
                    Debug.LogError($"Player Attack setup: в {HitClipPath} нет клипа {HitStateName}.");
                    return;
                }

                hit = root.AddState(HitStateName);
                hit.motion = clip;
            }

            if (root.anyStateTransitions.All(t => t.destinationState != hit))
            {
                AnimatorStateTransition toHit = root.AddAnyStateTransition(hit);
                toHit.AddCondition(AnimatorConditionMode.If, 0f, AttackTrigger);
                toHit.hasExitTime = false;
                toHit.duration = 0.1f;
                toHit.canTransitionToSelf = false;
            }

            if (hit.transitions.All(t => t.destinationState != idle))
            {
                AnimatorStateTransition toIdle = hit.AddTransition(idle);
                toIdle.hasExitTime = true;
                toIdle.exitTime = 0.9f;
                toIdle.duration = 0.1f;
            }

            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            Debug.Log($"Player Attack setup: Any State → {HitStateName} ({AttackTrigger}) → {IdleStateName} ({ControllerPath}).");
        }

        private static bool HasListener(PlayerInput.ActionEvent actionEvent, Object target, string methodName)
        {
            for (int i = 0; i < actionEvent.GetPersistentEventCount(); i++)
            {
                if (actionEvent.GetPersistentTarget(i) == target && actionEvent.GetPersistentMethodName(i) == methodName)
                    return true;
            }

            return false;
        }

        private static AnimatorState FindState(AnimatorStateMachine stateMachine, string name)
        {
            foreach (ChildAnimatorState child in stateMachine.states)
            {
                if (child.state.name == name)
                    return child.state;
            }

            foreach (ChildAnimatorStateMachine child in stateMachine.stateMachines)
            {
                AnimatorState state = FindState(child.stateMachine, name);
                if (state != null)
                    return state;
            }

            return null;
        }
    }
}
