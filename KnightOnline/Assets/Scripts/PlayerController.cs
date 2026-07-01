using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace KnightOnline
{
    public enum PlayerState { Idle, Walking, Running, Attacking, Casting, Dead }
    public enum PlayerClass { Warrior, Mage, Rogue, Priest }
    
    public class PlayerController : MonoBehaviour, IDamageable
    {
        [Header("Player Info")]
        public string playerName = "Knight";
        public PlayerClass playerClass = PlayerClass.Warrior;
        public int level = 1;
        public int experience = 0;
        public int experienceToNextLevel = 100;
        public int gold = 0;
        
        [Header("Stats")]
        public float maxHealth = 100f;
        public float currentHealth = 100f;
        public float maxMana = 100f;
        public float currentMana = 100f;
        public float maxStamina = 100f;
        public float currentStamina = 100f;
        
        [Header("Combat Stats")]
        public int strength = 10;
        public int dexterity = 10;
        public int intelligence = 10;
        public int constitution = 10;
        public int minDamage = 5;
        public int maxDamage = 10;
        public float attackSpeed = 1f;
        public float castSpeed = 1f;
        public float criticalChance = 5f;
        public float blockChance = 10f;
        public float dodgeChance = 5f;
        public float armor = 0f;
        
        [Header("Movement")]
        public float moveSpeed = 5f;
        public float runSpeed = 8f;
        public float rotationSpeed = 10f;
        public float jumpForce = 5f;
        
        [Header("Combat")]
        public float attackRange = 2f;
        public float attackCooldown = 1f;
        public LayerMask enemyLayer;
        public GameObject attackEffect;
        
        [Header("Components")]
        public Camera playerCamera;
        public Transform attackPoint;
        public GameObject healthBarPrefab;
        
        [Header("Animation")]
        public Animator animator;
        public RuntimeAnimatorController warriorAnimator;
        public RuntimeAnimatorController mageAnimator;
        public RuntimeAnimatorController rogueAnimator;
        public RuntimeAnimatorController priestAnimator;
        
        // Private variables
        private CharacterController characterController;
        private PlayerState currentState = PlayerState.Idle;
        private Vector3 moveDirection;
        private float gravity = -20f;
        private float verticalVelocity;
        private float lastAttackTime;
        private bool isGrounded;
        private bool isRunning;
        private bool isLocked;
        private Transform lockedTarget;
        private bool isDead;
        
        // Mana regeneration
        private float manaRegenRate = 5f;
        private float staminaRegenRate = 10f;
        private float lastManaRegen;
        private float lastStaminaRegen;
        
        // Experience multipliers per level
        private int[] expTable = { 0, 100, 250, 500, 850, 1300, 1900, 2700, 3700, 5000,
                                  6600, 8500, 10800, 13500, 16700, 20500, 25000, 30300, 36500, 43800,
                                  52000, 61400, 72200, 84600, 98800, 115000, 133300, 153800, 176800, 202500 };
        
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public float CurrentMana => currentMana;
        public float MaxMana => maxMana;
        public float CurrentStamina => currentStamina;
        public float MaxStamina => maxStamina;
        public float CurrentExperience => experience;
        public float ExperienceToNextLevel => experienceToNextLevel;
        public PlayerState CurrentState => currentState;
        public bool IsDead => isDead;
        
        // Events
        public System.Action<float, float> OnHealthChanged;
        public System.Action<float, float> OnManaChanged;
        public System.Action<float, float> OnStaminaChanged;
        public System.Action<int, int> OnExperienceGained;
        public System.Action<int> OnLevelUp;
        public System.Action OnDeath;
        public System.Action<Transform> OnTargetLocked;
        
        void Start()
        {
            characterController = GetComponent<CharacterController>();
            if (characterController == null)
            {
                characterController = gameObject.AddComponent<CharacterController>();
            }
            
            SetupClassStats();
            SetupAnimator();
            CreateHealthBar();
            
            if (playerCamera == null)
            {
                GameObject cam = new GameObject("PlayerCamera");
                cam.transform.parent = transform;
                cam.transform.localPosition = new Vector3(0, 2, -5);
                playerCamera = cam.AddComponent<Camera>();
                playerCamera.nearClipPlane = 0.1f;
            }
        }
        
        void SetupClassStats()
        {
            switch (playerClass)
            {
                case PlayerClass.Warrior:
                    strength = 15;
                    constitution = 12;
                    minDamage = 8;
                    maxDamage = 15;
                    armor = 10;
                    maxHealth = 150;
                    currentHealth = maxHealth;
                    maxMana = 50;
                    currentMana = maxMana;
                    break;
                case PlayerClass.Mage:
                    intelligence = 18;
                    constitution = 6;
                    minDamage = 12;
                    maxDamage = 20;
                    maxHealth = 80;
                    currentHealth = maxHealth;
                    maxMana = 200;
                    currentMana = maxMana;
                    break;
                case PlayerClass.Rogue:
                    dexterity = 18;
                    strength = 8;
                    minDamage = 6;
                    maxDamage = 12;
                    maxHealth = 100;
                    currentHealth = maxHealth;
                    maxMana = 80;
                    currentMana = maxMana;
                    criticalChance = 15f;
                    dodgeChance = 15f;
                    break;
                case PlayerClass.Priest:
                    intelligence = 15;
                    constitution = 10;
                    minDamage = 5;
                    maxDamage = 10;
                    maxHealth = 120;
                    currentHealth = maxHealth;
                    maxMana = 150;
                    currentMana = maxMana;
                    break;
            }
        }
        
        void SetupAnimator()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
                if (animator == null)
                {
                    animator = gameObject.AddComponent<Animator>();
                }
            }
            
            RuntimeAnimatorController controller = playerClass switch
            {
                PlayerClass.Warrior => warriorAnimator,
                PlayerClass.Mage => mageAnimator,
                PlayerClass.Rogue => rogueAnimator,
                PlayerClass.Priest => priestAnimator,
                _ => warriorAnimator
            };
            
            if (controller != null)
            {
                animator.runtimeAnimatorController = controller;
            }
        }
        
        void CreateHealthBar()
        {
            if (healthBarPrefab != null)
            {
                GameObject healthBar = Instantiate(healthBarPrefab, transform);
                healthBar.transform.localPosition = Vector3.up * 2.5f;
            }
        }
        
        void Update()
        {
            if (isDead) return;
            
            CheckGroundStatus();
            HandleMovement();
            HandleRotation();
            HandleRunning();
            HandleLockOn();
            HandleAttack();
            HandleManaRegeneration();
            HandleStaminaRegeneration();
            UpdateUI();
        }
        
        void CheckGroundStatus()
        {
            isGrounded = characterController.isGrounded;
            if (isGrounded && verticalVelocity < 0)
            {
                verticalVelocity = -2f;
            }
        }
        
        void HandleMovement()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            
            Vector3 inputDirection = new Vector3(horizontal, 0, vertical).normalized;
            
            if (inputDirection.magnitude > 0.1f)
            {
                float currentSpeed = isRunning ? runSpeed : moveSpeed;
                
                Vector3 cameraForward = playerCamera.transform.forward;
                Vector3 cameraRight = playerCamera.transform.right;
                cameraForward.y = 0;
                cameraRight.y = 0;
                cameraForward.Normalize();
                cameraRight.Normalize();
                
                moveDirection = cameraForward * inputDirection.z + cameraRight * inputDirection.x;
                moveDirection *= currentSpeed;
                
                characterController.Move(moveDirection * Time.deltaTime);
                
                UpdateState(isRunning ? PlayerState.Running : PlayerState.Walking);
                animator.SetFloat("Speed", currentSpeed);
            }
            else
            {
                characterController.Move(Vector3.zero);
                UpdateState(PlayerState.Idle);
                animator.SetFloat("Speed", 0);
            }
            
            // Jump
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                verticalVelocity = jumpForce;
                animator.SetTrigger("Jump");
            }
            
            verticalVelocity += gravity * Time.deltaTime;
            characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
        }
        
        void HandleRotation()
        {
            if (lockedTarget != null)
            {
                Vector3 direction = lockedTarget.position - transform.position;
                direction.y = 0;
                if (direction.magnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
            }
            else
            {
                float mouseX = Input.GetAxisRaw("Mouse X");
                transform.Rotate(Vector3.up * mouseX * rotationSpeed * Time.deltaTime * 100);
            }
        }
        
        void HandleRunning()
        {
            if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0 && moveDirection.magnitude > 0)
            {
                isRunning = true;
                currentStamina -= staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Max(0, currentStamina);
            }
            else
            {
                isRunning = false;
            }
            
            animator.SetBool("IsRunning", isRunning);
        }
        
        void HandleLockOn()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (lockedTarget != null)
                {
                    UnlockTarget();
                }
                else
                {
                    LockNearestEnemy();
                }
            }
        }
        
        void LockNearestEnemy()
        {
            Collider[] enemies = Physics.OverlapSphere(transform.position, 15f, enemyLayer);
            float closestDistance = Mathf.Infinity;
            Transform closest = null;
            
            foreach (Collider enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = enemy.transform;
                }
            }
            
            if (closest != null)
            {
                lockedTarget = closest;
                isLocked = true;
                OnTargetLocked?.Invoke(lockedTarget);
            }
        }
        
        void UnlockTarget()
        {
            lockedTarget = null;
            isLocked = false;
        }
        
        void HandleAttack()
        {
            if (Input.GetMouseButtonDown(0) && currentState != PlayerState.Attacking)
            {
                if (Time.time - lastAttackTime >= attackCooldown / attackSpeed)
                {
                    StartCoroutine(PerformAttack());
                }
            }
            
            if (Input.GetMouseButtonDown(1) && lockedTarget != null)
            {
                // Right click skill
                SkillSystem.Instance.CastSkill(1);
            }
        }
        
        System.Collections.IEnumerator PerformAttack()
        {
            UpdateState(PlayerState.Attacking);
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
            
            yield return new WaitForSeconds(0.2f);
            
            // Find target
            Transform target = lockedTarget;
            if (target == null)
            {
                Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);
                if (hits.Length > 0)
                {
                    target = hits[0].transform;
                }
            }
            
            if (target != null)
            {
                IDamageable damageable = target.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    int damage = CalculateDamage();
                    bool isCritical = Random.Range(0, 100) < criticalChance;
                    if (isCritical) damage = Mathf.RoundToInt(damage * 1.5f);
                    
                    damageable.TakeDamage(damage, isCritical);
                    
                    if (attackEffect != null)
                    {
                        Instantiate(attackEffect, target.position, Quaternion.identity);
                    }
                }
            }
            
            yield return new WaitForSeconds(attackCooldown / attackSpeed - 0.2f);
            if (currentState == PlayerState.Attacking)
            {
                UpdateState(PlayerState.Idle);
            }
        }
        
        int CalculateDamage()
        {
            int baseDamage = Random.Range(minDamage, maxDamage + 1);
            float strengthBonus = 1 + (strength * 0.05f);
            return Mathf.RoundToInt(baseDamage * strengthBonus);
        }
        
        void HandleManaRegeneration()
        {
            if (Time.time - lastManaRegen >= 1f)
            {
                if (currentMana < maxMana)
                {
                    float regenAmount = manaRegenRate * (1 + intelligence * 0.1f);
                    currentMana = Mathf.Min(maxMana, currentMana + regenAmount);
                }
                lastManaRegen = Time.time;
            }
        }
        
        void HandleStaminaRegeneration()
        {
            if (!isRunning && Time.time - lastStaminaRegen >= 1f)
            {
                if (currentStamina < maxStamina)
                {
                    currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegenRate);
                }
                lastStaminaRegen = Time.time;
            }
        }
        
        void UpdateUI()
        {
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            OnManaChanged?.Invoke(currentMana, maxMana);
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        }
        
        void UpdateState(PlayerState newState)
        {
            if (currentState != newState)
            {
                currentState = newState;
                animator.SetInteger("State", (int)newState);
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
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        public void TakeDamage(int damage, bool isCritical)
        {
            TakeDamage((float)damage);
            if (isCritical)
            {
                // Show critical damage indicator
                Debug.Log($"CRITICAL HIT! {damage}");
            }
        }
        
        public void Heal(float amount)
        {
            if (isDead) return;
            
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
        
        public void RestoreMana(float amount)
        {
            if (isDead) return;
            
            currentMana = Mathf.Min(maxMana, currentMana + amount);
            OnManaChanged?.Invoke(currentMana, maxMana);
        }
        
        void Die()
        {
            isDead = true;
            UpdateState(PlayerState.Dead);
            animator.SetTrigger("Death");
            OnDeath?.Invoke();
            
            // Respawn after 5 seconds
            StartCoroutine(RespawnAfterDelay(5f));
        }
        
        System.Collections.IEnumerator RespawnAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Respawn();
        }
        
        void Respawn()
        {
            isDead = false;
            currentHealth = maxHealth;
            currentMana = maxMana;
            currentStamina = maxStamina;
            transform.position = Vector3.zero;
            UpdateState(PlayerState.Idle);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            OnManaChanged?.Invoke(currentMana, maxMana);
        }
        
        public void GainExperience(int amount)
        {
            experience += amount;
            OnExperienceGained?.Invoke(experience, experienceToNextLevel);
            
            while (experience >= experienceToNextLevel && level < expTable.Length - 1)
            {
                LevelUp();
            }
        }
        
        void LevelUp()
        {
            level++;
            experience -= experienceToNextLevel;
            experienceToNextLevel = expTable[Mathf.Min(level, expTable.Length - 1)];
            
            // Increase stats
            maxHealth += 10 + constitution;
            maxMana += 5 + intelligence;
            currentHealth = maxHealth;
            currentMana = maxMana;
            
            minDamage += 1;
            maxDamage += 2;
            
            OnLevelUp?.Invoke(level);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            OnManaChanged?.Invoke(currentMana, maxMana);
            
            Debug.Log($"Level Up! Now level {level}");
        }
        
        public void AddGold(int amount)
        {
            gold += amount;
        }
        
        public bool SpendGold(int amount)
        {
            if (gold >= amount)
            {
                gold -= amount;
                return true;
            }
            return false;
        }
    }
}
