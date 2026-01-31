using UnityEngine;

public class TestMovement : MonoBehaviour
{

    public float _moveSpeed = 1.0f;
    public float _movementSpeedX, _movementSpeedY;

    public Rigidbody2D _rigidbody;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _movementSpeedX = Input.GetAxisRaw("Horizontal") * _moveSpeed;
        _movementSpeedY = Input.GetAxisRaw("Vertical") * _moveSpeed;
        _rigidbody.linearVelocity = new Vector2(_movementSpeedX, _movementSpeedY); 
    }
}
