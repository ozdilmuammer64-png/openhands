using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float rotationSpeed = 10f;
    public float gravity = -20f;

    [Header("Combat Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 0.5f;
    public LayerMask enemyLayer;

    [Header("Components")]
    private CharacterController characterController;
    private Animator animator;
    private Camera mainCamera;

    [Header("UI Reference")]
    public Image healthBarFill;
    public Image manaBarFill;
    public Text healthText;
    public Text manaText;

    [Header("Particle Effects")]
    public GameObject hitEffect;
    public GameObject levelUpEffect;

    // Player Stats
    private float currentHealth;
    private float maxHealth;
    private float currentMana;
    private float maxMana;
    private float attackDamage;
    private float magicPower;
    private float critChance;
    private float currentXp;
    private float xpToLevel;
    private int currentLevel;
    private int gold;

    // Movement
    private Vector3 velocity;
    private float verticalVelocity;
    private bool isGrounded;
    private Vector3 moveDirection;

    // Combat
    private float lastAttackTime;
    private bool isAttacking;

    // Skills
    private float[] skillCooldowns = new float[4];
    private bool[] skillReady = new bool[4];

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    void Start()
    {
        InitializePlayer();
    }

    void Update()
    {
        HandleMovement();
        HandleCombat();
        HandleSkills();
        UpdateUI();
    }

    void InitializePlayer()
    {
        if (GameManager.Instance != null && GameManager.Instance.currentGame != null)
        {
            PlayerData pd = GameManager.Instance.currentGame.playerData;
            
            currentHealth = pd.health;
            maxHealth = pd.maxHealth;
            currentMana = pd.mana;
            maxMana = pd.maxMana;
            attackDamage = pd.attackDamage;
            magicPower = pd.magicPower;
            currentXp = pd.xp;
            xpToLevel = pd.xpToLevel;
            currentLevel = pd.level;
            gold = pd.gold;

            transform.position = pd.position;

            GameManager.Instance.currentGame.playerData.health = currentHealth;
            GameManager.Instance.currentGame.playerData.mana = currentMana;
        }
        else
        {
            currentHealth = 100;
            maxHealth = 100;
            currentMana = 50;
            maxMana = 50;
            attackDamage = 10;
            magicPower = 5;
            currentXp = 0;
            xpToLevel = 100;
            currentLevel = 1;
            gold = 100;
        }
    }

    #region Movement

    void HandleMovement()
    {
        isGrounded = characterController.isGrounded;
        
        if (isGrounded)
        {
            verticalVelocity = -2f;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            // Calculate movement relative to camera
            Vector3 camForward = mainCamera.transform.forward;
            Vector3 camRight = mainCamera.transform.right;
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 targetDirection = camForward * moveDirection.z + camRight * moveDirection.x;
            
            // Rotate player towards movement direction
            if (targetDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // Apply movement
            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
            characterController.Move(targetDirection * currentSpeed * Time.deltaTime);

            // Animation
            if (animator != null)
            {
                animator.SetFloat("Speed", currentSpeed);
            }
        }
        else
        {
            if (animator != null)
            {
                animator.SetFloat("Speed", 0);
            }
        }

        // Apply gravity
        verticalVelocity += gravity * Time.deltaTime;
        velocity.y = verticalVelocity;
        characterController.Move(velocity * Time.deltaTime);
    }

    #endregion

    #region Combat

    void HandleCombat()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            TryAttack();
        }
    }

    void TryAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        lastAttackTime = Time.time;
        isAttacking = true;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // Raycast to find enemy
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackRange * 10, enemyLayer))
        {
            EnemyController enemy = hit.collider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                // Calculate damage with crit
                bool isCrit = Random.Range(0f, 100f) < critChance;
                float damage = isCrit ? attackDamage * 1.5f : attackDamage;
                
                enemy.TakeDamage(damage, isCrit);
                
                // Show hit effect
                if (hitEffect != null)
                {
                    Instantiate(hitEffect, hit.point, Quaternion.identity);
                }
            }
        }

        Invoke("ResetAttack", 0.3f);
    }

    void ResetAttack()
    {
        isAttacking = false;
    }

    #endregion

    #region Skills

    void HandleSkills()
    {
        for (int i = 0; i < 4; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                UseSkill(i);
            }

            // Update cooldowns
            if (skillCooldowns[i] > 0)
            {
                skillCooldowns[i] -= Time.deltaTime;
                if (skillCooldowns[i] <= 0)
                {
                    skillCooldowns[i] = 0;
                    skillReady[i] = true;
                }
            }
        }
    }

    void UseSkill(int skillIndex)
    {
        if (!skillReady[skillIndex]) return;

        // Find nearest enemy
        EnemyController nearestEnemy = FindNearestEnemy();

        switch (skillIndex)
        {
            case 0: // Basic attack skill
                if (currentMana >= 10)
                {
                    currentMana -= 10;
                    skillCooldowns[0] = 2f;
                    skillReady[0] = false;

                    if (nearestEnemy != null)
                    {
                        float damage = magicPower + 20;
                        nearestEnemy.TakeDamage(damage, false);
                        if (animator != null) animator.SetTrigger("Skill1");
                    }
                }
                break;

            case 1: // Dash/Clear
                if (currentMana >= 15)
                {
                    currentMana -= 15;
                    skillCooldowns[1] = 3f;
                    skillReady[1] = false;

                    if (nearestEnemy != null)
                    {
                        float damage = magicPower + 30;
                        nearestEnemy.TakeDamage(damage, false);
                        transform.position = nearestEnemy.transform.position;
                        if (animator != null) animator.SetTrigger("Skill2");
                    }
                }
                break;

            case 2: // AOE
                if (currentMana >= 25)
                {
                    currentMana -= 25;
                    skillCooldowns[2] = 5f;
                    skillReady[2] = false;

                    Collider[] enemies = Physics.OverlapSphere(transform.position, 5f, enemyLayer);
                    foreach (Collider c in enemies)
                    {
                        EnemyController enemy = c.GetComponent<EnemyController>();
                        if (enemy != null)
                        {
                            enemy.TakeDamage(magicPower + 40, false);
                        }
                    }
                    if (animator != null) animator.SetTrigger("Skill3");
                }
                break;

            case 3: // Ultimate
                if (currentMana >= 30)
                {
                    currentMana -= 30;
                    skillCooldowns[3] = 10f;
                    skillReady[3] = false;

                    if (nearestEnemy != null)
                    {
                        float damage = magicPower + 80;
                        nearestEnemy.TakeDamage(damage, true);
                        if (animator != null) animator.SetTrigger("Skill4");
                    }
                }
                break;
        }
    }

    EnemyController FindNearestEnemy()
    {
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        EnemyController nearest = null;
        float minDist = Mathf.Infinity;

        foreach (EnemyController enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy;
            }
        }

        return nearest;
    }

    #endregion

    #region Damage & Death

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        UpdateUI();
    }

    void Die()
    {
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }

        enabled = false;
    }

    #endregion

    #region Experience & Leveling

    public void GainXP(int amount)
    {
        currentXp += amount;
        
        if (currentXp >= xpToLevel)
        {
            LevelUp();
        }

        UpdateUI();
    }

    void LevelUp()
    {
        currentLevel++;
        currentXp -= xpToLevel;
        xpToLevel = Mathf.Floor(100 * Mathf.Pow(1.2f, currentLevel));

        // Increase stats
        maxHealth += 20;
        maxMana += 10;
        currentHealth = maxHealth;
        currentMana = maxMana;
        attackDamage += 2;
        magicPower += 1;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.LevelUp();
        }

        // Level up effect
        if (levelUpEffect != null)
        {
            Instantiate(levelUpEffect, transform.position + Vector3.up * 2, Quaternion.identity);
        }

        UpdateUI();
    }

    public void GainGold(int amount)
    {
        gold += amount;
        UpdateUI();
    }

    #endregion

    #region Healing

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateUI();
    }

    public void RestoreMana(float amount)
    {
        currentMana = Mathf.Min(maxMana, currentMana + amount);
        UpdateUI();
    }

    #endregion

    #region UI Update

    void UpdateUI()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }

        if (manaBarFill != null)
        {
            manaBarFill.fillAmount = currentMana / maxMana;
        }

        if (healthText != null)
        {
            healthText.text = $"{Mathf.Floor(currentHealth)}/{maxHealth}";
        }

        if (manaText != null)
        {
            manaText.text = $"{Mathf.Floor(currentMana)}/{maxMana}";
        }

        // Update game data
        if (GameManager.Instance != null && GameManager.Instance.currentGame != null)
        {
            GameManager.Instance.currentGame.playerData.health = currentHealth;
            GameManager.Instance.currentGame.playerData.mana = currentMana;
            GameManager.Instance.currentGame.playerData.xp = currentXp;
            GameManager.Instance.currentGame.playerData.level = currentLevel;
            GameManager.Instance.currentGame.playerData.gold = gold;
            GameManager.Instance.currentGame.playerData.position = transform.position;
        }
    }

    #endregion

    #region Getters

    public float GetHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetMana() => currentMana;
    public float GetMaxMana() => maxMana;
    public int GetLevel() => currentLevel;
    public float GetXP() => currentXp;
    public float GetXPToLevel() => xpToLevel;
    public int GetGold() => gold;
    public float GetAttackDamage() => attackDamage;
    public float GetMagicPower() => magicPower;

    #endregion
}