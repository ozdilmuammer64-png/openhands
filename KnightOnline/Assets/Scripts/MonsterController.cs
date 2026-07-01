using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

namespace KnightOnline
{
    public enum MonsterType { Normal, Elite, Boss, Guardian }
    public enum MonsterState { Idle, Patrol, Chase, Attack, Hurt, Dead }
    
    public class MonsterController : MonoBehaviour, IDamageable
    {
        void Awake()
        {
            // Nesneyi yukarı kaldır (yerden)
            Vector3 pos = transform.position;
            pos.y = 1f; // Yerden 1 birim yukarıda
            transform.position = pos;
        }
        
        [Header("Monster Info")]
        public string monsterName = "Goblin";
        public MonsterType monsterType = MonsterType.Normal;
        public int level = 1;
        public int experienceReward = 10;
        public int goldRewardMin = 5;
        public int goldRewardMax = 15;
        
        [Header("Stats")]
        public float maxHealth = 50f;
        public float currentHealth = 50f;
        public int minDamage = 5;
        public int maxDamage = 10;
        public float attackSpeed = 1f;
        public float attackRange = 1.5f;
        public float attackCooldown = 1.5f;
        public float armor = 2f;
        public float criticalChance = 3f;
        
        [Header("AI Settings")]
        public float detectionRange = 10f;
        public float chaseRange = 15f;
        public float patrolRange = 5f;
        public float patrolSpeed = 2f;
        public float chaseSpeed = 4f;
        public float idleTime = 2f;
        
        [Header("Components")]
        public Animator animator;
        public Collider attackCollider;
        public GameObject deathEffect;
        public GameObject hitEffect;
        public GameObject healthBarPrefab;
        public Transform healthBarPosition;
        
        [Header("Patrol Points")]
        public List<Transform> patrolPoints = new List<Transform>();
        public bool randomPatrol = true;
        
        // Private variables
        private MonsterState currentState = MonsterState.Idle;
        private NavMeshAgent navMeshAgent;
        private Transform target;
        private Vector3 spawnPoint;
        private Vector3 currentPatrolPoint;
        private float lastAttackTime;
        private float lastStateChangeTime;
        private float stateTimer;
        private int currentPatrolIndex;
        private bool isDead;
        private bool isHurt;
        private CanvasGroup healthBarCanvas;
        
        // Drops
        [Header("Drops")]
        public List<DropItem> possibleDrops = new List<DropItem>();
        public int goldDropChance = 80;
        
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsDead => isDead;
        
        // Events
        public System.Action<MonsterController> OnDeath;
        public System.Action<float, float> OnHealthChanged;
        
        void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            if (navMeshAgent == null)
            {
                navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            }
            
            spawnPoint = transform.position;
            currentHealth = maxHealth;
            
            SetupByType();
            CreateHealthBar();
            StartIdle();
        }
        
        void SetupByType()
        {
            switch (monsterType)
            {
                case MonsterType.Normal:
                    navMeshAgent.speed = patrolSpeed;
                    break;
                case MonsterType.Elite:
                    maxHealth *= 2f;
                    currentHealth = maxHealth;
                    minDamage *= 2;
                    maxDamage *= 2;
                    navMeshAgent.speed = patrolSpeed * 1.2f;
                    gameObject.name = $"[Elite] {monsterName}";
                    break;
                case MonsterType.Boss:
                    maxHealth *= 5f;
                    currentHealth = maxHealth;
                    minDamage *= 3;
                    maxDamage *= 3;
                    experienceReward *= 5;
                    goldRewardMin *= 10;
                    goldRewardMax *= 10;
                    navMeshAgent.speed = patrolSpeed * 0.9f;
                    gameObject.name = $"[BOSS] {monsterName}";
                    break;
                case MonsterType.Guardian:
                    maxHealth *= 3f;
                    currentHealth = maxHealth;
                    minDamage *= 2;
                    maxDamage *= 2;
                    detectionRange *= 1.5f;
                    gameObject.name = $"[Guardian] {monsterName}";
                    break;
            }
            
            // Scale with level
            float levelMultiplier = 1f + (level - 1) * 0.15f;
            maxHealth *= levelMultiplier;
            currentHealth = maxHealth;
            minDamage = Mathf.RoundToInt(minDamage * levelMultiplier);
            maxDamage = Mathf.RoundToInt(maxDamage * levelMultiplier);
            experienceReward = Mathf.RoundToInt(experienceReward * levelMultiplier);
        }
        
        void CreateHealthBar()
        {
            if (healthBarPrefab != null)
            {
                GameObject healthBarObj = Instantiate(healthBarPrefab, transform);
                healthBarObj.transform.localPosition = healthBarPosition != null ? 
                    healthBarPosition.localPosition : Vector3.up * 2f;
                healthBarCanvas = healthBarObj.GetComponent<CanvasGroup>();
            }
        }
        
        void Update()
        {
            if (isDead) return;
            
            switch (currentState)
            {
                case MonsterState.Idle:
                    HandleIdle();
                    break;
                case MonsterState.Patrol:
                    HandlePatrol();
                    break;
                case MonsterState.Chase:
                    HandleChase();
                    break;
                case MonsterState.Attack:
                    HandleAttack();
                    break;
            }
            
            CheckForTarget();
        }
        
        void CheckForTarget()
        {
            if (currentState == MonsterState.Attack || currentState == MonsterState.Chase) return;
            
            Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange, LayerMask.GetMask("Player"));
            if (hits.Length > 0)
            {
                target = hits[0].transform;
                ChangeState(MonsterState.Chase);
            }
        }
        
        void StartIdle()
        {
            ChangeState(MonsterState.Idle);
            stateTimer = Random.Range(idleTime * 0.5f, idleTime * 1.5f);
        }
        
        void HandleIdle()
        {
            stateTimer -= Time.deltaTime;
            animator.SetFloat("Speed", 0);
            
            if (stateTimer <= 0)
            {
                if (ShouldPatrol())
                {
                    StartPatrol();
                }
                else
                {
                    stateTimer = Random.Range(idleTime * 0.5f, idleTime * 1.5f);
                }
            }
        }
        
        bool ShouldPatrol()
        {
            return patrolPoints.Count > 0 || patrolRange > 0;
        }
        
        void StartPatrol()
        {
            if (navMeshAgent == null || !navMeshAgent.isOnNavMesh || !navMeshAgent.isActiveAndEnabled)
            {
                StartIdle();
                return;
            }
            
            ChangeState(MonsterState.Patrol);
            
            if (patrolPoints.Count > 0)
            {
                if (randomPatrol)
                {
                    currentPatrolIndex = Random.Range(0, patrolPoints.Count);
                }
                else
                {
                    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
                }
                currentPatrolPoint = patrolPoints[currentPatrolIndex].position;
            }
            else
            {
                currentPatrolPoint = spawnPoint + new Vector3(
                    Random.Range(-patrolRange, patrolRange),
                    0,
                    Random.Range(-patrolRange, patrolRange)
                );
            }
            
            navMeshAgent.SetDestination(currentPatrolPoint);
            navMeshAgent.speed = patrolSpeed;
        }
        
        void HandlePatrol()
        {
            if (navMeshAgent == null || !navMeshAgent.isOnNavMesh || !navMeshAgent.isActiveAndEnabled)
            {
                StartIdle();
                return;
            }
            
            if (navMeshAgent.remainingDistance < 0.5f)
            {
                StartIdle();
                return;
            }
            
            animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
        }
        
        void HandleChase()
        {
            if (navMeshAgent == null || !navMeshAgent.isOnNavMesh || !navMeshAgent.isActiveAndEnabled)
            {
                StartIdle();
                return;
            }
            
            if (target == null)
            {
                StartIdle();
                return;
            }
            
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            
            if (distanceToTarget > chaseRange)
            {
                StartIdle();
                target = null;
                return;
            }
            
            if (distanceToTarget <= attackRange)
            {
                ChangeState(MonsterState.Attack);
                return;
            }
            
            navMeshAgent.SetDestination(target.position);
            navMeshAgent.speed = chaseSpeed;
            navMeshAgent.stoppingDistance = attackRange - 0.5f;
            
            animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
            
            // Look at target
            Vector3 direction = target.position - transform.position;
            direction.y = 0;
            if (direction.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(direction),
                    5f * Time.deltaTime
                );
            }
        }
        
        void HandleAttack()
        {
            if (target == null || target.GetComponent<PlayerController>()?.IsDead == true)
            {
                StartIdle();
                return;
            }
            
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            
            if (distanceToTarget > attackRange * 1.5f)
            {
                ChangeState(MonsterState.Chase);
                return;
            }
            
            // Face target
            Vector3 direction = target.position - transform.position;
            direction.y = 0;
            if (direction.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(direction),
                    10f * Time.deltaTime
                );
            }
            
            animator.SetFloat("Speed", 0);
            
            // Attack
            if (Time.time - lastAttackTime >= attackCooldown / attackSpeed)
            {
                StartCoroutine(PerformAttack());
            }
        }
        
        System.Collections.IEnumerator PerformAttack()
        {
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
            
            yield return new WaitForSeconds(0.3f);
            
            if (target != null)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (distanceToTarget <= attackRange)
                {
                    IDamageable damageable = target.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        int damage = CalculateDamage();
                        bool isCritical = Random.Range(0, 100) < criticalChance;
                        if (isCritical) damage = Mathf.RoundToInt(damage * 1.5f);
                        
                        damageable.TakeDamage(damage, isCritical);
                    }
                }
            }
            
            yield return new WaitForSeconds(attackCooldown / attackSpeed - 0.3f);
        }
        
        int CalculateDamage()
        {
            int baseDamage = Random.Range(minDamage, maxDamage + 1);
            float levelBonus = 1f + (level - 1) * 0.1f;
            return Mathf.RoundToInt(baseDamage * levelBonus);
        }
        
        void ChangeState(MonsterState newState)
        {
            if (currentState != newState)
            {
                currentState = newState;
                animator.SetInteger("State", (int)newState);
                lastStateChangeTime = Time.time;
            }
        }
        
        public void TakeDamage(float damage)
        {
            if (isDead) return;
            
            float reducedDamage = damage * (1 - armor / 100f);
            currentHealth -= reducedDamage;
            currentHealth = Mathf.Max(0, currentHealth);
            
            animator.SetTrigger("Hurt");
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            
            if (hitEffect != null)
            {
                GameObject hit = Instantiate(hitEffect, transform.position + Vector3.up, Quaternion.identity);
                Destroy(hit, 2f);
            }
            
            if (currentHealth <= 0)
            {
                Die();
            }
            else if (currentState != MonsterState.Attack && currentState != MonsterState.Chase)
            {
                // Aggro
                if (target != null)
                {
                    ChangeState(MonsterState.Chase);
                }
            }
        }
        
        public void TakeDamage(int damage, bool isCritical)
        {
            TakeDamage((float)damage);
        }
        
        void Die()
        {
            isDead = true;
            ChangeState(MonsterState.Dead);
            animator.SetTrigger("Death");
            
            navMeshAgent.enabled = false;
            
            // Drop loot
            DropLoot();
            
            // Give experience
            if (target != null)
            {
                PlayerController player = target.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.GainExperience(experienceReward);
                }
            }
            
            OnDeath?.Invoke(this);
            
            // Respawn
            StartCoroutine(RespawnAfterDelay(GameSettings.Instance.monsterRespawnTime));
            
            if (deathEffect != null)
            {
                Instantiate(deathEffect, transform.position + Vector3.up, Quaternion.identity);
            }
        }
        
        void DropLoot()
        {
            // Drop gold
            if (Random.Range(0, 100) < goldDropChance)
            {
                int goldAmount = Random.Range(goldRewardMin, goldRewardMax + 1);
                if (target != null)
                {
                    PlayerController player = target.GetComponent<PlayerController>();
                    if (player != null)
                    {
                        player.AddGold(goldAmount);
                    }
                }
            }
            
            // Drop items
            foreach (DropItem drop in possibleDrops)
            {
                if (Random.Range(0, 100) < drop.dropChance)
                {
                    GameObject item = Instantiate(drop.itemPrefab, transform.position + Vector3.up, Quaternion.identity);
                    ItemPickup pickup = item.AddComponent<ItemPickup>();
                    pickup.Initialize(drop.itemData);
                }
            }
        }
        
        System.Collections.IEnumerator RespawnAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            isDead = false;
            currentHealth = maxHealth;
            transform.position = spawnPoint;
            navMeshAgent.enabled = true;
            
            gameObject.SetActive(true);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            StartIdle();
        }
        
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseRange);
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spawnPoint, patrolRange);
        }
    }
    
    [System.Serializable]
    public class DropItem
    {
        public GameObject itemPrefab;
        public ItemData itemData;
        [Range(0, 100)]
        public float dropChance = 10f;
    }
}
