using UnityEngine;

namespace KnightOnline
{
    public class PlayerController : MonoBehaviour, IDamageable
    {
        [Header("Stats")]
        public int maxHealth = 100;
        public int currentHealth = 100;
        public int attackDamage = 20;
        public float moveSpeed = 5f;
        
        [Header("Mana")]
        public int maxMana = 100;
        public int currentMana = 100;
        public float manaRegenRate = 5f;
        
        [Header("Combat")]
        public float attackRange = 1.5f;
        public float attackCooldown = 0.3f;
        
        // Events
        public System.Action<int, int> OnHealthChanged;
        public System.Action<int, int> OnManaChanged;
        public System.Action OnDeath;
        
        Rigidbody2D rb;
        Vector2 moveDirection;
        float lastAttackTime = 0;
        float lastManaRegenTime = 0;
        bool facingRight = true;
        
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
            currentMana = maxMana;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            OnManaChanged?.Invoke(currentMana, maxMana);
        }
        
        void Update()
        {
            HandleMovement();
            HandleAttack();
            HandleManaRegen();
            HandleSkills();
        }
        
        void HandleMovement()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            
            moveDirection = new Vector2(h, v).normalized;
            rb.velocity = moveDirection * moveSpeed;
            
            // Yöne göre döndür
            if (h > 0 && !facingRight) Flip();
            else if (h < 0 && facingRight) Flip();
        }
        
        void Flip()
        {
            facingRight = !facingRight;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);
        }
        
        void HandleAttack()
        {
            if (Input.GetMouseButtonDown(0)) // Sol tık
            {
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    PerformAttack();
                    lastAttackTime = Time.time;
                }
            }
        }
        
        void PerformAttack()
        {
            Vector2 attackDir = facingRight ? Vector2.right : Vector2.left;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, attackDir, attackRange);
            
            if (hit.collider != null)
            {
                IDamageable dmg = hit.collider.GetComponent<IDamageable>();
                if (dmg != null)
                {
                    dmg.TakeDamage(attackDamage);
                    Debug.Log($"⚔️ Saldırı! {hit.collider.name} -> {attackDamage} hasar");
                }
            }
        }
        
        void HandleManaRegen()
        {
            if (Time.time - lastManaRegenTime >= 1f)
            {
                currentMana = Mathf.Min(currentMana + (int)manaRegenRate, maxMana);
                OnManaChanged?.Invoke(currentMana, maxMana);
                lastManaRegenTime = Time.time;
            }
        }
        
        void HandleSkills()
        {
            // 1-5 tuşları ile yetenekler
            if (Input.GetKeyDown(KeyCode.Alpha1)) SkillSystem.Instance?.UseSkill(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SkillSystem.Instance?.UseSkill(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SkillSystem.Instance?.UseSkill(2);
            if (Input.GetKeyDown(KeyCode.Alpha4)) SkillSystem.Instance?.UseSkill(3);
            if (Input.GetKeyDown(KeyCode.Alpha5)) SkillSystem.Instance?.UseSkill(4);
        }
        
        public void TakeDamage(int damage)
        {
            TakeDamage(damage, false);
        }
        
        public void TakeDamage(int damage, bool isCritical)
        {
            if (currentHealth <= 0) return;
            
            currentHealth -= damage;
            if (currentHealth < 0) currentHealth = 0;
            
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            
            string crit = isCritical ? " KRITIK!" : "";
            Debug.Log($"💔 Hasar aldı! {damage}{crit} (Can: {currentHealth}/{maxHealth})");
            
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
        
        public void Heal(int amount)
        {
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            Debug.Log($"💚 İyileşti! +{amount} (Can: {currentHealth}/{maxHealth})");
        }
        
        public void UseMana(int amount)
        {
            currentMana -= amount;
            if (currentMana < 0) currentMana = 0;
            OnManaChanged?.Invoke(currentMana, maxMana);
        }
        
        void Die()
        {
            Debug.Log("☠️ Oyuncu öldü!");
            OnDeath?.Invoke();
            
            // 3 saniye sonra yeniden doğ
            Invoke(nameof(Respawn), 3f);
        }
        
        void Respawn()
        {
            currentHealth = maxHealth;
            transform.position = Vector3.zero;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            Debug.Log("🔄 Yeniden doğdu!");
        }
    }
}
