using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class GuardianMovement : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("MapBoundary")]
    [SerializeField] private BoxCollider2D _mapBoundary;
    [SerializeField] private float _padding = 0.5f;

    [Header("Component")]
    [SerializeField] private AnimationController _animationController;

    private Vector2 movementInput;
    private Vector2 lastMoveDirection;
    private float minX, maxX, minY, maxY;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        if (_mapBoundary != null)
        {
            Bounds boundary = _mapBoundary.bounds;

            minX = boundary.min.x + _padding;
            maxX = boundary.max.x - _padding;
            minY = boundary.min.y + _padding;
            maxY = boundary.max.y - _padding;
        }
    }

    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        movementInput = new Vector2(inputX, inputY).normalized;

        _animationController.HandleAnimationAndFlip(inputX, inputY);
    }

    void FixedUpdate()
    {
        Vector2 newPosition = rb.position + movementInput * moveSpeed * Time.fixedDeltaTime;

        if (_mapBoundary != null)
        {
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        }

        rb.MovePosition(newPosition);
    }

    
}