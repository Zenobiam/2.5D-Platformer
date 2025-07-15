using UnityEngine;

// [RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    //[Header("Movement Settings")]
    //[SerializeField] private float _moveSpeed = 5f;
    //[SerializeField] private float _jumpForce = 5f;
    //[SerializeField] private float _groundCheckDistance = 0.6f;
    //[SerializeField] private LayerMask _groundLayer;
    //[SerializeField] private float _playerHeight = 1f; // Высота модели игрока

    //private BoxCollider _boxCollider;

    //private Rigidbody _rb;
    //private PlayerInputController _inputController;
    //private PlayerVisual _playerVisual;
    //[SerializeField] private bool _isGrounded;
    //private Vector2 _moveDirection;

    //public Vector2 MoveDirection => _moveDirection;
    //public bool IsGrounded => _isGrounded;

    private void Awake()
    {
        //_rb = GetComponent<Rigidbody>();
        //_inputController = GetComponent<PlayerInputController>();
        //_playerVisual = GetComponentInChildren<PlayerVisual>();
        //_boxCollider = GetComponentInChildren<BoxCollider>();
        //// Фиксируем вращение Rigidbody
        //_rb.freezeRotation = true;
        //_rb.constraints = RigidbodyConstraints.FreezeRotation;

        //_groundLayer = LayerMask.NameToLayer("Ground");
    }

    private void Update()
    {
        //CheckGrounded();
        //HandleMovement();
        //HandleJump();
        //UpdateVisuals();
    }

    //private void CheckGrounded()
    //{
    //    if (_boxCollider == null)
    //    {
    //        Debug.LogError("BoxCollider is not assigned");
    //        return;
    //    }

    //    // Создаем область проверки под игроком
    //    Vector3 checkPosition = _boxCollider.bounds.center - new Vector3(0, _boxCollider.bounds.extents.y, 0);
    //    Vector3 checkSize = new Vector3(_boxCollider.bounds.size.x * 0.9f, 0.1f, _boxCollider.bounds.size.z);

    //    // Проверяем наличие земли в области
    //    _isGrounded = Physics.CheckBox(
    //        checkPosition,           // Центр проверяемой области
    //        checkSize * 0.5f,        // Половина размера области (CheckBox использует половину размера)
    //        Quaternion.identity,     // Без поворота
    //        _groundLayer             // Слой земли
    //    );
    //}

    //private void HandleMovement()
    //{
    //    Vector2 moveInput = _inputController.MoveInput;
    //    _moveDirection = moveInput.normalized;
    //    Vector2 movement = _moveDirection * _moveSpeed;
    //    _rb.linearVelocity = new Vector2(movement.x, _rb.linearVelocity.y);
    //}

    //private void HandleJump()
    //{
    //    if (_inputController.IsJumpPressed && _isGrounded)
    //    {
    //        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
    //    }
    //}

    //private void UpdateVisuals()
    //{
    //    if (_playerVisual != null)
    //    {
    //        _playerVisual.UpdateVisuals(new Vector3(_moveDirection.x, 0, 0), _isGrounded);
    //    }
    //}

    // Визуализация в редакторе
    //private void OnDrawGizmos()
    //{
    //    if (_boxCollider == null) return;

    //    // Рисуем область проверки
    //    Vector3 checkPosition = _boxCollider.bounds.center - new Vector3(0, _boxCollider.bounds.extents.y, 0);
    //    Vector3 checkSize = new Vector3(_boxCollider.bounds.size.x * 0.9f, 0.1f, _boxCollider.bounds.size.z);

    //    Gizmos.color = _isGrounded ? Color.green : Color.red;
    //    Gizmos.DrawWireCube(checkPosition, checkSize);
    //}
} 