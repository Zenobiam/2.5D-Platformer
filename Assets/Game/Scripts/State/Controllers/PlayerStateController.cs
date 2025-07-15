using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    #region Player variables

    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _groundCheckDistance = 0.6f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private bool _isGrounded;
    private Vector2 _moveDirection;
    private bool _isFalling;
    private float _jumpCount = 1;
    private bool _isWalk;

    [Header("Other scripts")]
    private PlayerInputController _inputController;
    private PlayerVisual _playerVisual;

    [Header("Components")]
    private BoxCollider _boxCollider;
    private Rigidbody _rb;

    #endregion

    #region Access Fields

        #region Movement
        public Vector2 MoveDirection => _moveDirection;
        public bool IsGrounded
        {
            get { return _isGrounded; }
            set { _isGrounded = value; }
        }
        // public bool IsJumpPressed => _inputController.IsJumpPressed; // Вынести в отдельную логику? типо просчет всех нажатий и выдача IsJump вместо IsJumpPressed
        // public bool IsRunPressed => _inputController.IsRunPressed; // Вынести в отдельную логику? типо просчет всех нажатий и выдача IsRun вместо IsRunPressed
        // public bool IsCrouchPressed => _inputController.IsCrouchPressed; // Вынести в отдельную логику? типо просчет всех нажатий и выдача IsCrouch вместо IsCrouchPressed
        public float MoveSpeed
        {
            get { return _moveSpeed; }
            set { _moveSpeed = value; }
        }
        public float JumpForce
        {
            get { return _jumpForce; }
            set { _jumpForce = value; }
        }
        public float GroundCheckDistance => _groundCheckDistance;
        public bool IsFalling => _isFalling;
        public bool IsWalk => _isWalk;
        public float JumpCount
        {
            get { return _jumpCount; }
            set { _jumpCount = value; }
        }

    #endregion

    #region Components
        public BoxCollider BoxCollider => _boxCollider;
        public Rigidbody Rigidbody => _rb;

        #endregion

        #region Other scripts
        public PlayerInputController InputController => _inputController;
        public PlayerVisual PlayerVisual => _playerVisual;

        #endregion

    #endregion


    #region StateMachine variables

    [Header("StateMachine variables")]
    private PlayerBaseState _currentState;
    private PlayerStateFactory _states;

    public PlayerBaseState CurrentState 
    { 
        get { return _currentState; } 
        set { _currentState = value; } 
    }

    #endregion

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _inputController = GetComponent<PlayerInputController>();
        _playerVisual = GetComponentInChildren<PlayerVisual>();
        _boxCollider = GetComponentInChildren<BoxCollider>();
        // Фиксируем вращение Rigidbody
        _rb.freezeRotation = true;
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        // _groundLayer = ((int)Layers.Ground);

        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();        
    }

    // Update is called once per frame
    void Update()
    {
        _currentState.UpdateStates();
        CheckGrounded();
        HandleMovement();
        UpdateVisuals();
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        _currentState.OnCollisionEnter();
    }

    #region Player Methods

    private void CheckGrounded()
    {
        if (_boxCollider == null)
        {
            Debug.LogError("BoxCollider is not assigned");
            return;
        }

        // Создаем область проверки под игроком
        Vector3 checkPosition = _boxCollider.bounds.center - new Vector3(0, _boxCollider.bounds.extents.y, 0);
        Vector3 checkSize = new Vector3(_boxCollider.bounds.size.x * 0.9f, 0.1f, _boxCollider.bounds.size.z);

        // Проверяем наличие земли в области
        _isGrounded = Physics.CheckBox(
            checkPosition,           // Центр проверяемой области
            checkSize * 0.5f,        // Половина размера области (CheckBox использует половину размера)
            Quaternion.identity,     // Без поворота
            _groundLayer             // Слой земли
        );
    }

    private void HandleMovement()
    {
        Vector2 moveInput = _inputController.MoveInput;
        _moveDirection = moveInput.normalized;
        Vector2 movement = _moveDirection * _moveSpeed;
        _rb.linearVelocity = new Vector2(movement.x, _rb.linearVelocity.y);

        // Debug.Log($"moveInput - {moveInput}, _moveDirection - {_moveDirection}");

        _isWalk = moveInput.x != 0; // Здесть может быть бага с анимациями и не только
    }

    private void UpdateVisuals()
    {
        if (_playerVisual != null)
        {
            _playerVisual.UpdateVisuals(new Vector3(_moveDirection.x, 0, 0), _isGrounded);
        }
    }
    #endregion
}
