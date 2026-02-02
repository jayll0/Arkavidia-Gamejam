using UnityEngine;
using UnityEngine.AI;

public class FrogAnimationScript : MonoBehaviour
{

    [Header("Animator")]
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private NavMeshAgent _agent;

    private float _movementThreshold = 0.1f;
    private bool _facingRight = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = _agent.velocity;
        
        bool _isMoving = velocity.magnitude > _movementThreshold;
        _animator.SetBool("IsMoving", _isMoving);

        if (velocity.x > _movementThreshold && !_facingRight)
        {
            Flip();
        } 
        else if (velocity.x < -_movementThreshold && _facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        _facingRight = !_facingRight;
        _spriteRenderer.flipX = !_facingRight;
    }
}
