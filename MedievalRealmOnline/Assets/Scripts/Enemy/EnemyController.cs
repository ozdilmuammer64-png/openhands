using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Stats")]
    public string enemyName = "Enemy";
    public int enemyLevel = 1;
    public float maxHealth = 50f;
    public float currentHealth;
    public float attackDamage = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float moveSpeed = 3f;
    public int xpReward = 25;
    public int goldReward = 10;

    [Header("AI Settings")]
    public float chaseRange = 10f;
    public float patrolRadius = 5f;
    public float detectionAngle = 180f;

    [Header("Components")]
    private NavMeshAgent agent;
    private Animator animator;
    private Transform target;
    private Vector3 patrolCenter;
    private Vector3 patrolDestination;

    [Header("Effects")]
    public GameObject damageNumberPrefab;
    public GameObject deathEffect;
    public GameObject hitEffect;

    [Header("UI")]
    public Canvas healthBarCanvas;
    public UnityEngine.UI.Image healthBarFill;
    public Text healthText;

    // State
    private enum State { Idle, Patrol, Chase, Attack, Dead }
    private State currentState = State.Idle;

    private float lastAttackTime;
    private bool isDead;
    private float patrolTimer;
    private Vector3 lastKnownPosition;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        if (healthBarCanvas != null)
        {
            healthBarCanvas.worldCamera = Camera.main;
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
        patrolCenter = transform.position;
        SetState(State.Patrol);
    }

    void Update()
    {
        if (isDead) return;

        UpdateState();
        UpdateAnimations();
        UpdateHealthBar();
    }

    void UpdateState()
    {
        switch (currentState)
        {
            case State.Idle:
                HandleIdle();
                break;
            case State.Patrol:
                HandlePatrol();
                break;
            case State.Chase:
                HandleChase();
                break;
            case State.Attack:
                HandleAttack();
                break;
        }
    }

    void HandleIdle()
    {
        patrolTimer -= Time.deltaTime;
        if (patrolTimer <= 0)
        {
            SetState(State.Patrol);
        }

        // Look for player
        CheckForPlayer();
    }

    void HandlePatrol()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            // Pick new patrol destination
            Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
            randomDirection += patrolCenter;
            
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }

        // Look for player
        CheckForPlayer();
    }

    void HandleChase()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            SetState(State.Patrol);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        if (distanceToPlayer > chaseRange * 1.5f)
        {
            // Lost player
            lastKnownPosition = target.position;
            SetState(State.Idle);
            patrolTimer = 3f;
            return;
        }

        if (distanceToPlayer <= attackRange)
        {
            SetState(State.Attack);
            return;
        }

        // Chase player
        agent.SetDestination(target.position);
        agent.speed = moveSpeed * 1.2f;
    }

    void HandleAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        if (distanceToPlayer > attackRange)
        {
            SetState(State.Chase);
            return;
        }

        // Face player
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);

        // Attack
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            Attack();
        }
    }

    void CheckForPlayer()
    {
        if (target != null) return;

        Collider[] players = Physics.OverlapSphere(transform.position, chaseRange, LayerMask.GetMask("Player"));
        
        foreach (Collider col in players)
        {
            Vector3 direction = (col.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, direction);

            if (angle <= detectionAngle / 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + Vector3.up, direction, out hit, chaseRange))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        target = col.transform;
                        SetState(State.Chase);
                        return;
                    }
                }
            }
        }
    }

    void Attack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // Deal damage to player
        PlayerController player = target.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage(attackDamage);
        }

        // Hit effect
        if (hitEffect != null && target != null)
        {
            Instantiate(hitEffect, target.position + Vector3.up, Quaternion.identity);
        }
    }

    public void TakeDamage(float damage, bool isCrit)
    {
        if (isDead) return;

        currentHealth -= damage;

        // Show damage number
        if (damageNumberPrefab != null)
        {
            GameObject dmgNum = Instantiate(damageNumberPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
            DamageNumber dmgComponent = dmgNum.GetComponent<DamageNumber>();
            if (dmgComponent != null)
            {
                dmgComponent.SetDamage(damage, isCrit);
            }
        }

        // Hit effect
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        // Knockback
        if (target != null)
        {
            Vector3 knockbackDir = (transform.position - target.position).normalized;
            agent.Move(knockbackDir * 0.5f);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        SetState(State.Dead);

        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        // Death effect
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Award player
        if (target != null)
        {
            PlayerController player = target.GetComponent<PlayerController>();
            if (player != null)
            {
                player.GainXP(xpReward);
                player.GainGold(goldReward);
            }
        }

        // Drop loot
        DropLoot();

        // Destroy after delay
        Destroy(gameObject, 2f);
    }

    void DropLoot()
    {
        // 30% chance to drop an item
        if (Random.Range(0f, 1f) < 0.3f)
        {
            // Spawn random loot
            GameObject loot = LootManager.Instance.GetRandomLoot();
            if (loot != null)
            {
                Instantiate(loot, transform.position + Vector3.up, Quaternion.identity);
            }
        }
    }

    void SetState(State newState)
    {
        currentState = newState;

        switch (newState)
        {
            case State.Patrol:
                agent.isStopped = false;
                agent.speed = moveSpeed;
                break;
            case State.Chase:
                agent.isStopped = false;
                agent.speed = moveSpeed * 1.2f;
                break;
            case State.Attack:
                agent.isStopped = true;
                break;
            case State.Dead:
                agent.isStopped = true;
                GetComponent<Collider>().enabled = false;
                break;
        }
    }

    void UpdateAnimations()
    {
        if (animator == null) return;

        bool isMoving = agent.velocity.magnitude > 0.1f;
        animator.SetBool("IsMoving", isMoving);

        if (currentState == State.Attack)
        {
            animator.SetBool("IsAttacking", true);
        }
        else
        {
            animator.SetBool("IsAttacking", false);
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{Mathf.Floor(currentHealth)}/{maxHealth}";
        }

        // Face camera
        if (healthBarCanvas != null)
        {
            healthBarCanvas.transform.LookAt(Camera.main.transform);
            healthBarCanvas.transform.Rotate(0, 180, 0);
        }
    }

    public bool IsDead() => isDead;
}