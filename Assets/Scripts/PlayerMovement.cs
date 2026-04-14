using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private float _speed = 5f;
    private Rigidbody2D _rigidbody;
    private Vector2 _movementInput;
    private float camHeight, camWidth;
    private Vector2 _playerExtends;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
        _playerExtends = GetComponent<SpriteRenderer>().bounds.extents;
    }

    private void FixedUpdate()
    {
        Vector2 newPos = _rigidbody.position + _movementInput * _speed * Time.fixedDeltaTime;

        newPos.x = Mathf.Clamp(newPos.x, -camWidth + _playerExtends.x, camWidth - _playerExtends.x);
        newPos.y = Mathf.Clamp(newPos.y, -camHeight + _playerExtends.y, camHeight - _playerExtends.y);

        _rigidbody.MovePosition(newPos);
    }

    private void OnMove(InputValue inputValue)
    {
        _movementInput = inputValue.Get<Vector2>();
    }
}