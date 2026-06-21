using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _cameraEdgePadding = 0f;
    [SerializeField] private float attackAnimationLength = 0.4f;
    [SerializeField] private float minimumAttackDuration = 0.05f;
    [SerializeField] private string isMovingParameterName = "IsMoving";
    [SerializeField] private string attackTriggerName = "Attack";
    [SerializeField] private string attackSpeedParameterName = "AttackSpeed";

    private float _speed = 5f;

    private Rigidbody2D _rigidbody;

    // NEU: Zugriff auf den SpriteRenderer,
    // damit wir den Charakter nach links/rechts spiegeln können.
    private SpriteRenderer _spriteRenderer;

    // NEU: Zugriff auf den Animator,
    // damit wir zwischen Idle- und Run-Animation wechseln können.
    private Animator _animator;
    private int _isMovingHash;
    private int _attackTriggerHash;
    private int _attackSpeedHash;
    private bool _hasIsMovingParameter;
    private bool _hasAttackTrigger;
    private bool _hasAttackSpeedParameter;

    private Vector2 _movementInput;
    private float camHeight, camWidth;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        // NEU: SpriteRenderer vom Player holen.
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // NEU: Animator vom Player holen.
        _animator = GetComponent<Animator>();
        CacheAnimatorParameters();
    }

    void Start()
    {
        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
    }

    private void FixedUpdate()
    {
        Vector2 newPos = _rigidbody.position + _movementInput * _speed * Time.fixedDeltaTime;

        newPos.x = Mathf.Clamp(newPos.x, -camWidth + _cameraEdgePadding, camWidth - _cameraEdgePadding);
        newPos.y = Mathf.Clamp(newPos.y, -camHeight + _cameraEdgePadding, camHeight - _cameraEdgePadding);

        _rigidbody.MovePosition(newPos);
    }

    // NEU: Update läuft jeden Frame.
    // Hier kümmern wir uns um Animation und Blickrichtung.
    private void Update()
    {
        // NEU: Prüft, ob der Player sich bewegt.
        // sqrMagnitude ist > 0, wenn eine Bewegungseingabe vorhanden ist.
        bool isMoving = _movementInput.sqrMagnitude > 0.01f;

        // NEU: Gibt dem Animator Bescheid:
        // true  = Run Animation
        // false = Idle Animation
        //
        // Wichtig: Im Animator muss ein Bool-Parameter
        // mit exakt dem Namen "IsMoving" existieren.
        if (_animator != null && _hasIsMovingParameter)
        {
            _animator.SetBool(_isMovingHash, isMoving);
        }

        // NEU: Wenn der Player nach rechts läuft,
        // schaut der Sprite nach rechts.
        if (_movementInput.x > 0.01f)
        {
            _spriteRenderer.flipX = false;
        }
        // NEU: Wenn der Player nach links läuft,
        // wird der Sprite horizontal gespiegelt.
        else if (_movementInput.x < -0.01f)
        {
            _spriteRenderer.flipX = true;
        }
    }

    private void OnMove(InputValue inputValue)
    {
        _movementInput = inputValue.Get<Vector2>();
    }

    // Arduino-Input
    public void SetMovementInput(Vector2 input)
    {
        _movementInput = input;
    }

    public float Attack(float attackSpeed)
    {
        float safeAttackSpeed = Mathf.Max(0.01f, attackSpeed);

        if (_animator == null || !_hasAttackTrigger)
        {
            return 0f;
        }

        float attackDuration = Mathf.Max(minimumAttackDuration, 1f / safeAttackSpeed);
        float attackLength = Mathf.Max(0.01f, attackAnimationLength);

        if (_hasAttackSpeedParameter)
        {
            _animator.SetFloat(_attackSpeedHash, attackLength / attackDuration);
        }

        _animator.ResetTrigger(_attackTriggerHash);
        _animator.SetTrigger(_attackTriggerHash);

        return _hasAttackSpeedParameter ? attackDuration : attackLength;
    }

    private void CacheAnimatorParameters()
    {
        if (_animator == null)
        {
            return;
        }

        _isMovingHash = Animator.StringToHash(isMovingParameterName);
        _attackTriggerHash = Animator.StringToHash(attackTriggerName);
        _attackSpeedHash = Animator.StringToHash(attackSpeedParameterName);

        foreach (AnimatorControllerParameter parameter in _animator.parameters)
        {
            if (parameter.nameHash == _isMovingHash && parameter.type == AnimatorControllerParameterType.Bool)
            {
                _hasIsMovingParameter = true;
            }
            else if (parameter.nameHash == _attackTriggerHash && parameter.type == AnimatorControllerParameterType.Trigger)
            {
                _hasAttackTrigger = true;
            }
            else if (parameter.nameHash == _attackSpeedHash && parameter.type == AnimatorControllerParameterType.Float)
            {
                _hasAttackSpeedParameter = true;
            }
        }
    }
}
