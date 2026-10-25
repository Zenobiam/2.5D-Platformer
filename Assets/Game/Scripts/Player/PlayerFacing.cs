using UnityEngine;

namespace Game.Scripts.Player
{
    public class PlayerFacing : MonoBehaviour
    {
        [SerializeField] private Transform modelTransform;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private bool defaultFacingRight = true;

        private float _currentRotation;

        // Куда смотрит игрок: +1 вправо (+X), -1 влево (-X). Камера и атака читают это значение, а не поворот модели.
        public int Direction { get; private set; }
        public Vector3 Forward => new Vector3(Direction, 0f, 0f);

        // Awake, а не Start: камера получает игрока сразу после создания, до его Start.
        private void Awake()
        {
            Direction = defaultFacingRight ? 1 : -1;
            _currentRotation = TargetRotation();
        }

        public void Face(Vector3 moveDirection)
        {
            if (moveDirection.x > 0.1f)
                Direction = 1;
            else if (moveDirection.x < -0.1f)
                Direction = -1;

            // Плавный поворот
            _currentRotation = Mathf.LerpAngle(_currentRotation, TargetRotation(), rotationSpeed * Time.deltaTime);
            UpdateModelRotation();
        }

        // 2.5D: модель смотрит вдоль ±X (камера с -Z), не в ±Z
        private float TargetRotation() =>
            Direction * 90f;

        private void UpdateModelRotation()
        {
            if (modelTransform != null)
                modelTransform.rotation = Quaternion.Euler(0f, _currentRotation, 0f);
        }
    }
}
