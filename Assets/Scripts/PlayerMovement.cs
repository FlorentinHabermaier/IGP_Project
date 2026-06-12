using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
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
    private Vector2 _playerExtends;

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

        // GEÄNDERT: Vorher war hier GetComponent<SpriteRenderer>().
        // Jetzt nutzen wir die gespeicherte Variable _spriteRenderer.
        _playerExtends = _spriteRenderer.bounds.extents;
    }

    private void FixedUpdate()
    {
        Vector2 newPos = _rigidbody.position + _movementInput * _speed * Time.fixedDeltaTime;

        newPos.x = Mathf.Clamp(newPos.x, -camWidth + _playerExtends.x, camWidth - _playerExtends.x);
        newPos.y = Mathf.Clamp(newPos.y, -camHeight + _playerExtends.y, camHeight - _playerExtends.y);

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
}