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

        private float _currentRotation = 0f;
        private float _targetRotation = 0f;

        private void Start()
        {
            // Устанавливаем начальное направление
            _currentRotation = defaultFacingRight ? 0f : 180f;
            _targetRotation = _currentRotation;
            UpdateModelRotation();
        }

        public void UpdateVisuals(Vector3 moveDirection, bool isGrounded)
        {
            // Определяем направление поворота
            if (moveDirection.x > 0.1f)
            {
                _targetRotation = 0f; // Смотрим вправо
            }
            else if (moveDirection.x < -0.1f)
            {
                _targetRotation = 180f; // Смотрим влево
            }

            // Плавный поворот
            _currentRotation = Mathf.LerpAngle(_currentRotation, _targetRotation, rotationSpeed * Time.deltaTime);
            UpdateModelRotation();

            // Обновление анимаций
            if (animator != null)
            {
                float moveSpeed = Mathf.Abs(moveDirection.x);
                animator.SetFloat("MoveSpeed", moveSpeed);
                animator.SetBool("IsGrounded", isGrounded);
            }
        }

        private void UpdateModelRotation()
        {
            modelTransform.rotation = Quaternion.Euler(0f, _currentRotation, 0f);
        }
    }
}