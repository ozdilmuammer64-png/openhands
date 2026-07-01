using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KnightOnline
{
    public enum SkillType { Passive, Active, Buff, Debuff }
    public enum SkillTarget { Self, SingleEnemy, MultipleEnemies, Area, Ally, SelfArea }
    public enum SkillElement { Physical, Fire, Ice, Lightning, Holy, Dark, None }
    
    [CreateAssetMenu(fileName = "New Skill", menuName = "Knight Online/Skill")]
    public class SkillData : ScriptableObject
    {
        public string skillName = "New Skill";
        public string description = "Skill description";
        public Sprite icon;
        public SkillType skillType = SkillType.Active;
        public SkillTarget targetType = SkillTarget.SingleEnemy;
        public SkillElement element = SkillElement.Physical;
        
        [Header("Requirements")]
        public int levelRequired = 1;
        public int skillPointCost = 1;
        public float manaCost = 20f;
        public float staminaCost = 0f;
        
        [Header("Damage")]
        public int baseDamage = 10;
        public float damageMultiplier = 1f;
        public float criticalMultiplier = 1.5f;
        
        [Header("Healing")]
        public float healAmount = 0f;
        public float healOverTime = 0f;
        public float healDuration = 0f;
        
        [Header("Buff/Debuff")]
        public float buffDuration = 5f;
        public float buffStrengthBonus = 0f;
        public float buffDamageReduction = 0f;
        public float buffSpeedMultiplier = 1f;
        
        [Header("Range & Cooldown")]
        public float range = 5f;
        public float cooldown = 5f;
        public float castTime = 0f;
        public float areaRadius = 0f;
        
        [Header("Effects")]
        public GameObject castEffect;
        public GameObject impactEffect;
        public AudioClip castSound;
        public AudioClip impactSound;
        
        [Header("Animation")]
        public string animationTrigger = "Cast";
        
        [Header("Visual")]
        public Color skillColor = Color.white;
        public bool showProjectile = false;
        public float projectileSpeed = 10f;
    }
    
    public class SkillSystem : MonoBehaviour
    {
        public static SkillSystem Instance { get; private set; }
        
        [Header("Skill Hotkeys")]
        public KeyCode[] hotkeyCodes = new KeyCode[]
        {
            KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5,
            KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0
        };
        
        [Header("Learned Skills")]
        public List<LearnedSkill> learnedSkills = new List<LearnedSkill>();
        public List<SkillData> availableSkills = new List<SkillData>();
        
        [Header("Active Buffs")]
        private List<ActiveBuff> activeBuffs = new List<ActiveBuff>();
        
        [Header("Components")]
        public PlayerController player;
        public Transform castPoint;
        public GameObject projectilePrefab;
        public LayerMask enemyLayer;
        public LayerMask allyLayer;
        
        // Events
        public System.Action<SkillData> OnSkillCast;
        public System.Action<SkillData, float> OnCooldownStart;
        public System.Action<SkillData> OnCooldownEnd;
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        void Start()
        {
            if (player == null)
            {
                player = FindObjectOfType<PlayerController>();
            }
            
            if (castPoint == null && player != null)
            {
                castPoint = player.attackPoint;
            }
        }
        
        void Update()
        {
            HandleHotkeys();
            UpdateBuffs();
        }
        
        void HandleHotkeys()
        {
            for (int i = 0; i < hotkeyCodes.Length; i++)
            {
                if (Input.GetKeyDown(hotkeyCodes[i]))
                {
                    CastSkill(i);
                }
            }
        }
        
        public void CastSkill(int slotIndex)
        {
            if (slotIndex >= learnedSkills.Count) return;
            
            LearnedSkill skill = learnedSkills[slotIndex];
            if (skill == null || !skill.data) return;
            
            // Check cooldown
            if (skill.currentCooldown > 0) return;
            
            // Check mana
            if (player.currentMana < skill.data.manaCost) return;
            
            // Check stamina
            if (player.currentStamina < skill.data.staminaCost) return;
            
            // Consume resources
            player.currentMana -= skill.data.manaCost;
            player.currentStamina -= skill.data.staminaCost;
            
            // Start cooldown
            skill.currentCooldown = skill.data.cooldown;
            OnCooldownStart?.Invoke(skill.data, skill.data.cooldown);
            
            // Cast the skill
            StartCoroutine(CastSkillRoutine(skill.data));
        }
        
        System.Collections.IEnumerator CastSkillRoutine(SkillData skill)
        {
            // Cast time
            if (skill.castTime > 0)
            {
                player.GetComponent<Animator>()?.SetTrigger(skill.animationTrigger);
                yield return new WaitForSeconds(skill.castTime);
            }
            
            // Find targets
            List<Transform> targets = FindTargets(skill);
            
            // Apply effects
            foreach (Transform target in targets)
            {
                ApplySkillEffect(skill, target);
            }
            
            // Self area effect
            if (skill.targetType == SkillTarget.SelfArea)
            {
                ApplySkillEffect(skill, transform);
            }
            
            OnSkillCast?.Invoke(skill);
        }
        
        List<Transform> FindTargets(SkillData skill)
        {
            List<Transform> targets = new List<Transform>();
            Vector3 center = skill.targetType == SkillTarget.Self || 
                            skill.targetType == SkillTarget.SelfArea ? 
                            transform.position : 
                            (player.lockedTarget ? player.lockedTarget.position : transform.position + transform.forward * skill.range);
            
            switch (skill.targetType)
            {
                case SkillTarget.Self:
                case SkillTarget.SelfArea:
                    targets.Add(transform);
                    break;
                    
                case SkillTarget.SingleEnemy:
                    if (player.lockedTarget != null)
                    {
                        targets.Add(player.lockedTarget);
                    }
                    else
                    {
                        Collider[] hits = Physics.OverlapSphere(transform.position, skill.range, enemyLayer);
                        if (hits.Length > 0) targets.Add(hits[0].transform);
                    }
                    break;
                    
                case SkillTarget.MultipleEnemies:
                    Collider[] multiHits = Physics.OverlapSphere(transform.position, skill.range, enemyLayer);
                    foreach (Collider hit in multiHits)
                    {
                        targets.Add(hit.transform);
                    }
                    break;
                    
                case SkillTarget.Ally:
                    Collider[] allyHits = Physics.OverlapSphere(transform.position, skill.range, allyLayer);
                    foreach (Collider hit in allyHits)
                    {
                        if (hit.transform != transform)
                        {
                            targets.Add(hit.transform);
                        }
                    }
                    break;
                    
                case SkillTarget.Area:
                    Collider[] areaHits = Physics.OverlapSphere(center, skill.areaRadius, enemyLayer);
                    foreach (Collider hit in areaHits)
                    {
                        targets.Add(hit.transform);
                    }
                    break;
            }
            
            return targets;
        }
        
        void ApplySkillEffect(SkillData skill, Transform target)
        {
            if (target == null) return;
            
            IDamageable damageable = target.GetComponent<IDamageable>();
            
            // Damage
            if (damageable != null && skill.baseDamage > 0)
            {
                int damage = Mathf.RoundToInt(skill.baseDamage * skill.damageMultiplier * (1 + player.intelligence * 0.1f));
                bool isCritical = Random.Range(0, 100) < player.criticalChance;
                if (isCritical) damage = Mathf.RoundToInt(damage * skill.criticalMultiplier);
                
                damageable.TakeDamage(damage, isCritical);
            }
            
            // Healing
            PlayerController targetPlayer = target.GetComponent<PlayerController>();
            if (targetPlayer != null)
            {
                if (skill.healAmount > 0)
                {
                    targetPlayer.Heal(skill.healAmount);
                }
                
                if (skill.healOverTime > 0)
                {
                    StartCoroutine(HealOverTime(targetPlayer, skill.healOverTime, skill.healDuration));
                }
                
                // Buffs
                if (skill.skillType == SkillType.Buff)
                {
                    ApplyBuff(targetPlayer, skill);
                }
            }
            
            // Effects
            if (skill.impactEffect != null)
            {
                Instantiate(skill.impactEffect, target.position, Quaternion.identity);
            }
            
            // Sound
            if (skill.impactSound != null)
            {
                AudioSource.PlayClipAtPoint(skill.impactSound, target.position);
            }
        }
        
        System.Collections.IEnumerator HealOverTime(PlayerController player, float healAmount, float duration)
        {
            float interval = 1f;
            float totalTicks = duration / interval;
            float healPerTick = healAmount / totalTicks;
            
            for (int i = 0; i < totalTicks; i++)
            {
                player.Heal(healPerTick);
                yield return new WaitForSeconds(interval);
            }
        }
        
        void ApplyBuff(PlayerController target, SkillData skill)
        {
            // Remove existing buff of same type
            activeBuffs.RemoveAll(b => b.skillName == skill.skillName && b.target == target.transform);
            
            ActiveBuff buff = new ActiveBuff
            {
                skillName = skill.skillName,
                target = target.transform,
                duration = skill.buffDuration,
                remainingTime = skill.buffDuration,
                damageBonus = skill.buffStrengthBonus,
                damageReduction = skill.buffDamageReduction,
                speedMultiplier = skill.buffSpeedMultiplier
            };
            
            activeBuffs.Add(buff);
        }
        
        void UpdateBuffs()
        {
            for (int i = activeBuffs.Count - 1; i >= 0; i--)
            {
                ActiveBuff buff = activeBuffs[i];
                buff.remainingTime -= Time.deltaTime;
                
                if (buff.remainingTime <= 0)
                {
                    RemoveBuff(buff);
                    activeBuffs.RemoveAt(i);
                }
            }
            
            // Update cooldowns
            foreach (LearnedSkill skill in learnedSkills)
            {
                if (skill.currentCooldown > 0)
                {
                    skill.currentCooldown -= Time.deltaTime;
                    if (skill.currentCooldown <= 0)
                    {
                        skill.currentCooldown = 0;
                        OnCooldownEnd?.Invoke(skill.data);
                    }
                }
            }
        }
        
        void RemoveBuff(ActiveBuff buff)
        {
            Debug.Log($"Buff {buff.skillName} expired on {buff.target.name}");
        }
        
        public float GetBuffValue(PlayerController player, string buffName, BuffType type)
        {
            foreach (ActiveBuff buff in activeBuffs)
            {
                if (buff.target == player.transform && buff.skillName == buffName)
                {
                    switch (type)
                    {
                        case BuffType.DamageBonus: return buff.damageBonus;
                        case BuffType.DamageReduction: return buff.damageReduction;
                        case BuffType.SpeedMultiplier: return buff.speedMultiplier;
                    }
                }
            }
            return 0f;
        }
        
        public void LearnSkill(SkillData skill)
        {
            if (skill == null) return;
            if (learnedSkills.Count >= 10) return;
            
            // Check if already learned
            if (learnedSkills.Exists(s => s.data == skill)) return;
            
            LearnedSkill newSkill = new LearnedSkill
            {
                data = skill,
                currentCooldown = 0
            };
            
            learnedSkills.Add(newSkill);
            Debug.Log($"Learned skill: {skill.skillName}");
        }
        
        public void UpgradeSkill(int slotIndex)
        {
            if (slotIndex >= learnedSkills.Count) return;
            // Skill upgrade logic
        }
        
        public float GetSkillCooldown(int slotIndex)
        {
            if (slotIndex >= learnedSkills.Count) return 0;
            return learnedSkills[slotIndex].currentCooldown;
        }
        
        public float GetSkillCooldownPercent(int slotIndex)
        {
            if (slotIndex >= learnedSkills.Count || learnedSkills[slotIndex].data == null) return 0;
            return learnedSkills[slotIndex].currentCooldown / learnedSkills[slotIndex].data.cooldown;
        }
    }
    
    [System.Serializable]
    public class LearnedSkill
    {
        public SkillData data;
        public float currentCooldown;
        public int level = 1;
    }
    
    public class ActiveBuff
    {
        public string skillName;
        public Transform target;
        public float duration;
        public float remainingTime;
        public float damageBonus;
        public float damageReduction;
        public float speedMultiplier;
    }
    
    public enum BuffType { DamageBonus, DamageReduction, SpeedMultiplier }
}
