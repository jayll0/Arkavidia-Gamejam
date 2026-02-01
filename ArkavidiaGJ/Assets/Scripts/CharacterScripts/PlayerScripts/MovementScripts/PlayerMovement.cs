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

    private Vector2 movementInput;
    private Vector2 lastMoveDirection; 
    private float  minX, maxX, minY, maxY;

    void Start()
    {
        // Otomatis cari komponen kalau lupa di-drag di inspector
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        // Buat Setting Map Boundary
        if (_mapBoundary != null)
        {
            Bounds boundary = _mapBoundary.bounds;

            minX = boundary.min.x + _padding;
            maxX = boundary.max.x - _padding;
            minY = boundary.min.y + _padding;
            maxY = boundary.max.y - _padding;
        }
    }

    // Input atau Logic disini
    void Update()
    {
        // 1. Menerima Input (WASD / Panah)
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        // 2. Normalisasi Vector (Supaya diagonal gak ngebut)
        movementInput = new Vector2(inputX, inputY).normalized;

        // 3. Mengatur Animasi & Arah Hadap (Visual)
        HandleAnimationAndFlip(inputX, inputY);
    }


    // Physics disini
    void FixedUpdate()
    {
        // 4. Menggerakkan Karakter (Physics)
        rb.linearVelocity = movementInput * moveSpeed; 
    }

    // Camera dan Posisi disini
    private void LateUpdate()
    {
        Vector2 position = transform.position;
        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);
        transform.position = position;
    }

    void HandleAnimationAndFlip(float x, float y)
    {
        bool isMoving = movementInput.magnitude > 0.01f;

        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
        }

        if (x > 0)
        {
            spriteRenderer.flipX = false; 
        }
        else if (x < 0)
        {
            spriteRenderer.flipX = true;  
        }
        
        if (isMoving)
        {
            lastMoveDirection = movementInput;
        }
    }
}