using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private Transform modelTransform;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private Animator animator;
    [SerializeField] private bool defaultFacingRight = true;

    private float currentRotation = 0f;
    private float targetRotation = 0f;

    private void Start()
    {
        // Устанавливаем начальное направление
        currentRotation = defaultFacingRight ? 0f : 180f;
        targetRotation = currentRotation;
        UpdateModelRotation();
    }

    public void UpdateVisuals(Vector3 moveDirection, bool isGrounded)
    {
        // Определяем направление поворота
        if (moveDirection.x > 0.1f)
        {
            targetRotation = 0f; // Смотрим вправо
        }
        else if (moveDirection.x < -0.1f)
        {
            targetRotation = 180f; // Смотрим влево
        }

        // Плавный поворот
        currentRotation = Mathf.LerpAngle(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
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
        modelTransform.rotation = Quaternion.Euler(0f, currentRotation, 0f);
    }
} 