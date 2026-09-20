using UnityEngine;

namespace Game.Scripts.Player
{
    public class PlayerVisual : MonoBehaviour
    {
        [Header("Visual Settings")] [SerializeField]
        private Transform modelTransform;

        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private Animator animator;
        [SerializeField] private bool defaultFacingRight = true;

        private static readonly int SpeedHash = Animator.StringToHash("Speed");

        private float _currentRotation = 0f;
        private float _targetRotation = 0f;

        private void Start()
        {
            // 2.5D: модель смотрит вдоль ±X (камера с -Z), не в ±Z
            _currentRotation = defaultFacingRight ? 90f : -90f;
            _targetRotation = _currentRotation;
            UpdateModelRotation();
        }

        // CC двигает root — не даём root motion клипам сдвигать CharacterController.
        private void OnAnimatorMove()
        {
        }

        public void UpdateVisuals(Vector3 moveDirection, bool isGrounded)
        {
            // Определяем направление поворота
            if (moveDirection.x > 0.1f)
            {
                _targetRotation = 90f; // Вправо (+X)
            }
            else if (moveDirection.x < -0.1f)
            {
                _targetRotation = -90f; // Влево (-X)
            }

            // Плавный поворот
            _currentRotation = Mathf.LerpAngle(_currentRotation, _targetRotation, rotationSpeed * Time.deltaTime);
            UpdateModelRotation();

            // Yurowm FreeHand: Speed 0 idle / ~0.5 walk / ~1 run
            if (animator != null)
            {
                float moveSpeed = Mathf.Abs(moveDirection.x);
                float speedParam = moveSpeed > 0.1f ? 1f : 0f;
                animator.SetFloat(SpeedHash, speedParam);
            }
        }

        private void UpdateModelRotation()
        {
            if (modelTransform != null)
                modelTransform.rotation = Quaternion.Euler(0f, _currentRotation, 0f);
        }
    }
}