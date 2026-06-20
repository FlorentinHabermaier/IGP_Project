using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _cameraEdgePadding = 0f;

    private float _speed = 5f;

    private Rigidbody2D _rigidbody;

    // NEU: Zugriff auf den SpriteRenderer,
    // damit wir den Charakter nach links/rechts spiegeln können.
    private SpriteRenderer _spriteRenderer;

    // NEU: Zugriff auf den Animator,
    // damit wir zwischen Idle- und Run-Animation wechseln können.
    private Animator _animator;

    private Vector2 _movementInput;
    private float camHeight, camWidth;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        // NEU: SpriteRenderer vom Player holen.
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // NEU: Animator vom Player holen.
        _animator = GetComponent<Animator>();
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
        _animator.SetBool("IsMoving", isMoving);

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

    // Hook for future attack animations. attackSpeed is passed in so the
    // animation can later be played faster or slower per hit.
    public void Attack(float attackSpeed)
    {
    }
}
