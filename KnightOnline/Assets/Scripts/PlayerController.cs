using UnityEngine;
using KnightOnline;

public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public int maxHealth = 100;
    public int currentHealth = 100;
    public int attackDamage = 20;
    public float moveSpeed = 5f;
    
    [Header("Components")]
    CharacterController cc;
    CameraController camController;
    
    [Header("Settings")]
    public float attackRange = 2.5f;
    public float attackCooldown = 0.5f;
    
    // Events
    public System.Action<int, int> OnHealthChanged;
    public System.Action OnDeath;
    
    float lastAttackTime = 0;
    Vector3 moveDirection;
    
    void Start()
    {
        cc = GetComponent<CharacterController>();
        if (cc == null) cc = gameObject.AddComponent<CharacterController>();
        
        // Kamerayı bul ve ayarla
        Camera cam = Camera.main;
        if (cam != null)
        {
            camController = cam.GetComponent<CameraController>();
            if (camController != null)
            {
                camController.target = transform;
            }
        }
        
        Debug.Log("✅ Oyuncu hazır!");
    }
    
    void Update()
    {
        HandleMovement();
        HandleAttack();
    }
    
    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(h, 0, v).normalized;
        
        if (dir.magnitude >= 0.1f)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                Vector3 forward = cam.transform.forward;
                Vector3 right = cam.transform.right;
                forward.y = 0; right.y = 0;
                forward.Normalize(); right.Normalize();
                moveDirection = (forward * v + right * h) * moveSpeed;
            }
            else
            {
                moveDirection = dir * moveSpeed;
            }
            
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
        else
        {
            moveDirection = Vector3.zero;
        }
        
        cc.Move(moveDirection * Time.deltaTime);
    }
    
    void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0))
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
        Debug.Log("⚔️ Saldırı!");
        
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up;
        if (Physics.Raycast(origin, transform.forward, out hit, attackRange))
        {
            IDamageable dmg = hit.collider.GetComponent<IDamageable>();
            if (dmg != null)
            {
                dmg.TakeDamage(attackDamage, false);
                Debug.Log($"💥 {hit.collider.name} -> {attackDamage} hasar!");
            }
        }
    }
    
    public void TakeDamage(int dmg, bool crit)
    {
        if (currentHealth <= 0) return;
        
        currentHealth -= dmg;
        if (currentHealth < 0) currentHealth = 0;
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"💔 Oyuncu {dmg} hasar! (Can: {currentHealth}/{maxHealth})");
        
        if (currentHealth <= 0) Die();
    }
    
    public void TakeDamage(float dmg) => TakeDamage((int)dmg, false);
    
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    void Die()
    {
        Debug.Log("☠️ Oyuncu öldü!");
        OnDeath?.Invoke();
        Invoke(nameof(Respawn), 3f);
    }
    
    void Respawn()
    {
        currentHealth = maxHealth;
        transform.position = new Vector3(0, 1, 0);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log("🔄 Yeniden doğdu!");
    }
}
