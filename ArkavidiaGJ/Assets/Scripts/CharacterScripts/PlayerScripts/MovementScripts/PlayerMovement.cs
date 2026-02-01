using UnityEngine;

public class GuardianMovement : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f; // Kecepatan jalan biasa

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;    // Masukkan Animator di sini lewat Inspector
    public SpriteRenderer spriteRenderer; // Untuk membalik gambar kiri/kanan

    private Vector2 movementInput;
    private Vector2 lastMoveDirection; // Berguna kalau nanti mau bikin fitur Dash/Attack arah terakhir

    void Start()
    {
        // Otomatis cari komponen kalau lupa di-drag di inspector
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

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

    void FixedUpdate()
    {
        // 4. Menggerakkan Karakter (Physics)
        // Pakai .velocity supaya gerakannya "Snappy" (Langsung jalan/berhenti)
        // CATATAN: Kalau pakai Unity 6, ganti .velocity jadi .linearVelocity
        rb.linearVelocity = movementInput * moveSpeed; 
    }

    void HandleAnimationAndFlip(float x, float y)
    {
        // Cek apakah karakter sedang bergerak?
        bool isMoving = movementInput.magnitude > 0.01f;

        // Kirim data ke Animator (kamu perlu buat parameter "IsMoving" di Animator Controller)
        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
        }

        // Logic membalik badan (Flip Sprite) ala Guardian Tales
        // Kalau tekan Kanan (x > 0), hadap kanan.
        // Kalau tekan Kiri (x < 0), hadap kiri.
        if (x > 0)
        {
            spriteRenderer.flipX = false; // Hadap kanan (sesuaikan dengan gambar aslimu)
        }
        else if (x < 0)
        {
            spriteRenderer.flipX = true;  // Hadap kiri
        }
        
        // Simpan arah terakhir (berguna untuk logic skill nanti)
        if (isMoving)
        {
            lastMoveDirection = movementInput;
        }
    }
}