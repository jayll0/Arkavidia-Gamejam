using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class EnemyMovementScript : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] private float _moveRadius = 3f;
    [SerializeField] private float _idleTime = 8f;
    [SerializeField] private float _patrolSpeed = 1.5f;

    [Header("Player Detection")]
    [SerializeField] private Transform _player;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private float _detectionRadius;
    [SerializeField] private float _chaseSpeed = 3f;
    [SerializeField] private float _stopDistance = 8f;

    [Header("Patrol Setting")]
    [SerializeField] private float _stopPatrolDistance = 0.5f;

    [Header("Battle Setup")]
    [SerializeField] private List<Characters> _enemyTeam = new List<Characters>();
    [SerializeField] private int _questIndex = -1;
    [SerializeField] private float _battleTriggerDistance = 1.5f;
    [SerializeField] private bool _isBoss = false;

    private NavMeshAgent _agent;

    private float _timer;
    private bool _isIdle;
    private bool _isPlayerDetected = false;
    private bool _battleTriggered = false;

    private Vector3 _originalPosition;
    private Vector3 _targetPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Kalau quest enemy ini sudah selesai (sudah dikalahkan), hancurkan
        if (_questIndex >= 0 && EnemyScript.instance != null
            && EnemyScript.instance.IsQuestCompleted(_questIndex))
        {
            Destroy(gameObject);
            return;
        }

        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _agent.stoppingDistance = _stopPatrolDistance;

        _originalPosition = transform.position;

        if (_player == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                _player = player.transform;
            }
        }

        _agent.speed = _patrolSpeed;
        SetRandomDestination();
    }

    // Update is called once per frame
    void Update()
    {
        if (_player == null || _battleTriggered) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        // Trigger battle ketika cukup dekat
        if (distanceToPlayer <= _battleTriggerDistance)
        {
            TriggerBattle();
            return;
        }

        if (distanceToPlayer <= _detectionRadius)
        {
            _isPlayerDetected = true;
            _targetPosition = _player.position;
        }
        else if (distanceToPlayer >= _stopDistance)
        {
            _isPlayerDetected = false;
        }

        if (_isPlayerDetected)
        {
            ChasePlayer();
        }
        else
        {
            RandomMovement();
        }
    }

    // Mulai battle dengan player
    private void TriggerBattle()
    {
        if (_battleTriggered) return;
        _battleTriggered = true;

        if (EnemyScript.instance != null && _enemyTeam != null && _enemyTeam.Count > 0)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            EnemyScript.instance.SetCurrentEnemyQuestIndex(_questIndex);
            EnemyScript.instance.StartBattle(_enemyTeam, currentScene);
        }
        else
        {
            Debug.LogWarning("Tidak bisa mulai battle: EnemyScript atau enemyTeam belum di-set!");
            _battleTriggered = false;
        }
    }

    // Ngejar Player
    void ChasePlayer()
    {
        _agent.speed = _chaseSpeed;
        _agent.isStopped = false;

        if (_agent.isOnNavMesh)
        {
            _agent.SetDestination(_player.position);
        }

        _isIdle = false;
        _timer = 0;
    }

    // Jalan-Jalan Gabut
    void RandomMovement()
    {
        _agent.speed = _patrolSpeed;

        if(!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
            {
                if (!_isIdle)
                {
                    _isIdle = true;
                    _timer = 0;
                    _agent.isStopped = true;
                }
                else
                {
                    _timer += Time.deltaTime;

                    if (_timer >= _idleTime)
                    {
                        SetRandomDestination();
                        _isIdle = false;
                        _timer = 0;
                        _agent.isStopped = false;
                    }
                }
            }
        }
    }

    // Jalan Random Otomatis
    void SetRandomDestination()
    {
        // Cek posisi valid sampe 10 kali
        for (int i = 0; i < 10; i++)
        {
            Vector2 randomDirection = Random.insideUnitCircle * _moveRadius;
            Vector3 randomPosition = _originalPosition + new Vector3(randomDirection.x, randomDirection.y, 0);

            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomPosition, out hit, _moveRadius, NavMesh.AllAreas))
            {
                if (_agent.isOnNavMesh)
                {
                    _agent.SetDestination(hit.position);
                    return;
                }
            }
        }
    }


    // Ketika Player kedeteksi
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _isPlayerDetected = true;
        }
    }

    // Ketika Player keluar dari deteksi
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            float distance = Vector3.Distance(transform.position, _player.position);

            if (distance > _stopPatrolDistance)
            {
                _isPlayerDetected = false;
            }
        }
    }

    // Gizmos
    private void OnDrawGizmosSelected()
    {
        // Detection radius (merah)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);

        // Stop chase radius (orange)
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, _stopDistance);

        // Battle trigger radius (hijau)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _battleTriggerDistance);

        // Movement radius (biru)
        Gizmos.color = Color.blue;
        Vector3 center = Application.isPlaying ? _originalPosition : transform.position;
        Gizmos.DrawWireSphere(center, _moveRadius);

        // Path visualization
        if (Application.isPlaying && _agent != null && _agent.hasPath)
        {
            Gizmos.color = Color.yellow;
            Vector3[] path = _agent.path.corners;
            for (int i = 0; i < path.Length - 1; i++)
            {
                Gizmos.DrawLine(path[i], path[i + 1]);
            }
        }
    }
}
