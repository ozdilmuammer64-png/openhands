using UnityEngine;
using UnityEngine.AI;

namespace KnightOnline
{
    public class MonsterController : MonoBehaviour, IDamageable
    {
        [Header("Stats")]
        public string monsterName = "Goblin";
        public int maxHealth = 100;
        public int currentHealth = 100;
        public int attackDamage = 10;
        
        [Header("Settings")]
        public float moveSpeed = 2f;
        public float attackRange = 2f;
        public float sightRange = 10f;
        public float attackCooldown = 1f;
        
        [Header("Patrol")]
        public bool canPatrol = true;
        public float patrolRadius = 5f;
        
        // Events
        public System.Action<int, int> OnHealthChanged;
        public System.Action OnDeath;
        
        private NavMeshAgent navAgent;
        private Transform player;
        private float lastAttackTime = 0;
        private Vector3 spawnPoint;
        private bool isDead = false;
        
        void Start()
        {
            navAgent = GetComponent<NavMeshAgent>();
            if (navAgent == null)
                navAgent = gameObject.AddComponent<NavMeshAgent>();
            
            navAgent.speed = moveSpeed;
            navAgent.stoppingDistance = attackRange;
            
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            spawnPoint = transform.position;
            
            // NavMesh kontrolü
            if (!navAgent.isOnNavMesh)
            {
                Debug.LogWarning($"⚠️ {monsterName} NavMesh üzerinde değil! NavMesh bake edilmeli.");
            }
            
            // Rastgele patrol başlat
            if (canPatrol)
            {
                InvokeRepeating(nameof(Patrol), 2f, 5f);
            }
        }
        
        void Update()
        {
            if (isDead) return;
            
            if (player != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);
                
                // Oyuncu görüş alanında mı?
                if (distanceToPlayer <= sightRange)
                {
                    // Saldırı menzilinde mi?
                    if (distanceToPlayer <= attackRange)
                    {
                        Attack();
                    }
                    else
                    {
                        ChasePlayer();
                    }
                }
                else
                {
                    // Oyuncuyu kaybetti, idle
                    StopChase();
                }
            }
        }
        
        void ChasePlayer()
        {
            if (navAgent != null && navAgent.isOnNavMesh)
            {
                navAgent.SetDestination(player.position);
                navAgent.stoppingDistance = attackRange;
                
                // Yöne döndür
                Vector3 dir = player.position - transform.position;
                dir.y = 0;
                if (dir.magnitude > 0.1f)
                    transform.rotation = Quaternion.LookRotation(dir);
            }
        }
        
        void StopChase()
        {
            if (navAgent != null)
            {
                navAgent.ResetPath();
            }
        }
        
        void Patrol()
        {
            if (isDead) return;
            if (player != null && Vector3.Distance(transform.position, player.position) <= sightRange) return;
            
            if (navAgent != null && navAgent.isOnNavMesh && canPatrol)
            {
                Vector3 randomPoint = spawnPoint + Random.insideUnitSphere * patrolRadius;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 2f, NavMesh.AllAreas))
                {
                    navAgent.SetDestination(hit.position);
                }
            }
        }
        
        void Attack()
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                
                // Oyuncuya hasar ver
                IDamageable damageable = player.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(attackDamage, false);
                    Debug.Log($"💀 {monsterName}, oyuncuya {attackDamage} hasar verdi!");
                }
                
                // Yöne döndür
                Vector3 dir = player.position - transform.position;
                dir.y = 0;
                if (dir.magnitude > 0.1f)
                    transform.rotation = Quaternion.LookRotation(dir);
            }
        }
        
        public void TakeDamage(int damage, bool isCritical)
        {
            if (isDead) return;
            
            currentHealth -= damage;
            if (currentHealth < 0) currentHealth = 0;
            
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            
            string critText = isCritical ? " KRITIK!" : "";
            Debug.Log($"💥 {monsterName} {damage} hasar yedi!{critText} (Can: {currentHealth}/{maxHealth})");
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        void Die()
        {
            if (isDead) return;
            isDead = true;
            
            Debug.Log($"☠️ {monsterName} öldü!");
            OnDeath?.Invoke();
            
            // Yeniden doğ
            Invoke(nameof(Respawn), 3f);
        }
        
        void Respawn()
        {
            currentHealth = maxHealth;
            isDead = false;
            transform.position = spawnPoint;
            transform.position += Vector3.up; // Yerden biraz yukarı
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            Debug.Log($"🔄 {monsterName} yeniden doğdu!");
        }
    }
}
