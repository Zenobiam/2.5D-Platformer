using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundCheckDistance = 0.6f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float playerHeight = 1f; // Высота модели игрока

    private BoxCollider boxCollider;

    private Rigidbody rb;
    private PlayerInputController inputController;
    private PlayerVisual playerVisual;
    [SerializeField] private bool isGrounded;
    private Vector2 moveDirection;

    public Vector2 MoveDirection => moveDirection;
    public bool IsGrounded => isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputController = GetComponent<PlayerInputController>();
        playerVisual = GetComponentInChildren<PlayerVisual>();
        boxCollider = GetComponentInChildren<BoxCollider>();
        // Фиксируем вращение Rigidbody
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Update()
    {
        CheckGrounded();
        HandleMovement();
        HandleJump();
        UpdateVisuals();
    }

    private void CheckGrounded()
    {
        if (boxCollider == null)
        {
            Debug.LogError("BoxCollider is not assigned");
            return;
        }

        // Создаем область проверки под игроком
        Vector3 checkPosition = boxCollider.bounds.center - new Vector3(0, boxCollider.bounds.extents.y, 0);
        Vector3 checkSize = new Vector3(boxCollider.bounds.size.x * 0.9f, 0.1f, boxCollider.bounds.size.z);

        // Проверяем наличие земли в области
        isGrounded = Physics.CheckBox(
            checkPosition,           // Центр проверяемой области
            checkSize * 0.5f,        // Половина размера области (CheckBox использует половину размера)
            Quaternion.identity,     // Без поворота
            groundLayer             // Слой земли
        );
    }

    private void HandleMovement()
    {
        Vector2 moveInput = inputController.MoveInput;
        moveDirection = moveInput.normalized;
        Vector2 movement = moveDirection * moveSpeed;
        rb.linearVelocity = new Vector2(movement.x, rb.linearVelocity.y);
    }

    private void HandleJump()
    {
        if (inputController.IsJumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void UpdateVisuals()
    {
        if (playerVisual != null)
        {
            playerVisual.UpdateVisuals(new Vector3(moveDirection.x, 0, 0), isGrounded);
        }
    }

    // Визуализация в редакторе
    private void OnDrawGizmos()
    {
        if (boxCollider == null) return;

        // Рисуем область проверки
        Vector3 checkPosition = boxCollider.bounds.center - new Vector3(0, boxCollider.bounds.extents.y, 0);
        Vector3 checkSize = new Vector3(boxCollider.bounds.size.x * 0.9f, 0.1f, boxCollider.bounds.size.z);

        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireCube(checkPosition, checkSize);
    }
} 