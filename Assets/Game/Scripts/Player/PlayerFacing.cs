using UnityEngine;

namespace Game.Scripts.Player
{
    public class PlayerFacing : MonoBehaviour
    {
        [SerializeField] private Transform modelTransform;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private bool defaultFacingRight = true;

        private float _currentRotation;
        private float _targetRotation;

        private void Start()
        {
            // 2.5D: модель смотрит вдоль ±X (камера с -Z), не в ±Z
            _currentRotation = defaultFacingRight ? 90f : -90f;
            _targetRotation = _currentRotation;
            // UpdateModelRotation();
        }

        public void Face(Vector3 moveDirection)
        {
            if (moveDirection.x > 0.1f)
                _targetRotation = 90f; // Вправо (+X)
            else if (moveDirection.x < -0.1f)
                _targetRotation = -90f; // Влево (-X)

            // Плавный поворот
            _currentRotation = Mathf.LerpAngle(_currentRotation, _targetRotation, rotationSpeed * Time.deltaTime);
            UpdateModelRotation();
        }

        private void UpdateModelRotation()
        {
            if (modelTransform != null)
                modelTransform.rotation = Quaternion.Euler(0f, _currentRotation, 0f);
        }
    }
}
