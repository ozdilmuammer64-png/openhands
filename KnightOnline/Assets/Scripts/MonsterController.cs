using UnityEngine;

namespace KnightOnline
{
    public class MonsterController : MonoBehaviour, IDamageable
    {
        [Header("Stats")]
        public string monsterName = "Goblin";
        public int maxHealth = 100;
        public int currentHealth = 100;
        public int attackDamage = 10;
        
        [Header("Movement")]
        public float moveSpeed = 2f;
        public float sightRange = 5f;
        public float attackRange = 1f;
        public float attackCooldown = 1f;
        
        [Header("Patrol")]
        public bool canPatrol = true;
        public float patrolRadius = 3f;
        public float patrolWaitTime = 2f;
        
        // Events
        public System.Action<int, int> OnHealthChanged;
        public System.Action OnDeath;
        
        Transform player;
        Rigidbody2D rb;
        Vector2 spawnPoint;
        bool isDead = false;
        bool facingRight = true;
        float lastAttackTime = 0;
        float patrolTimer = 0;
        Vector2 patrolTarget;
        
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.freezeRotation = true;
        }
        
        void Start()
        {
            currentHealth = maxHealth;
            spawnPoint = transform.position;
            
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
            
            if (canPatrol) SetNewPatrolTarget();
        }
        
        void Update()
        {
            if (isDead) return;
            
            if (player != null)
            {
                float dist = Vector2.Distance(transform.position, player.position);
                
                if (dist <= sightRange)
                {
                    // Oyuncuyu takip et
                    ChasePlayer();
                    
                    // Saldırı menzilinde mi?
                    if (dist <= attackRange)
                    {
                        Attack();
                    }
                }
                else
                {
                    // Devriye gez
                    Patrol();
                }
            }
            else
            {
                Patrol();
            }
        }
        
        void ChasePlayer()
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.velocity = dir * moveSpeed;
            
            // Yöne döndür
            if (dir.x > 0.1f && !facingRight) Flip();
            else if (dir.x < -0.1f && facingRight) Flip();
        }
        
        void Patrol()
        {
            patrolTimer -= Time.deltaTime;
            
            if (patrolTimer <= 0)
            {
                // Hedefe git
                Vector2 dir = (patrolTarget - (Vector2)transform.position).normalized;
                rb.velocity = dir * moveSpeed * 0.5f;
                
                // Yöne döndür
                if (dir.x > 0.1f && !facingRight) Flip();
                else if (dir.x < -0.1f && facingRight) Flip();
                
                // Hedefe vardı mı?
                if (Vector2.Distance(transform.position, patrolTarget) < 0.5f)
                {
                    SetNewPatrolTarget();
                }
            }
        }
        
        void SetNewPatrolTarget()
        {
            patrolTarget = spawnPoint + (Vector2)Random.insideUnitCircle * patrolRadius;
            patrolTimer = patrolWaitTime;
        }
        
        void Attack()
        {
            rb.velocity = Vector2.zero;
            
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                
                IDamageable dmg = player.GetComponent<IDamageable>();
                if (dmg != null)
                {
                    dmg.TakeDamage(attackDamage);
                    Debug.Log($"💀 {monsterName} saldırdı! {attackDamage} hasar");
                }
            }
        }
        
        void Flip()
        {
            facingRight = !facingRight;
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        }
        
        public void TakeDamage(int damage)
        {
            TakeDamage(damage, false);
        }
        
        public void TakeDamage(int damage, bool isCritical)
        {
            if (isDead) return;
            
            currentHealth -= damage;
            if (currentHealth < 0) currentHealth = 0;
            
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            
            string crit = isCritical ? " KRITIK!" : "";
            Debug.Log($"💥 {monsterName} hasar aldı! {damage}{crit} (Can: {currentHealth}/{maxHealth})");
            
            // Hasar efekti
            StartCoroutine(DamageEffect());
            
            if (currentHealth <= 0) Die();
        }
        
        public void TakeDamage(float damage)
        {
            TakeDamage((int)damage, false);
        }
        
        System.Collections.IEnumerator DamageEffect()
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color original = sr.color;
                sr.color = Color.red;
                yield return new WaitForSeconds(0.1f);
                sr.color = original;
            }
        }
        
        void Die()
        {
            if (isDead) return;
            isDead = true;
            
            rb.velocity = Vector2.zero;
            Debug.Log($"☠️ {monsterName} öldü!");
            OnDeath?.Invoke();
            
            // 3 saniye sonra yeniden doğ
            Invoke(nameof(Respawn), 3f);
        }
        
        void Respawn()
        {
            currentHealth = maxHealth;
            isDead = false;
            transform.position = spawnPoint;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            Debug.Log($"🔄 {monsterName} yeniden doğdu!");
        }
    }
}
