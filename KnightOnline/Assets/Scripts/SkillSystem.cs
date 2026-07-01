using UnityEngine;
using System.Collections.Generic;

namespace KnightOnline
{
    public class SkillSystem : MonoBehaviour
    {
        public static SkillSystem Instance { get; private set; }
        
        [Header("Skills")]
        public List<Skill> skills = new List<Skill>();
        
        [Header("Mana")]
        public int maxMana = 100;
        public int currentMana = 100;
        public float manaRegenRate = 10f;
        
        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        
        void Start()
        {
            SetupDefaultSkills();
        }
        
        void Update()
        {
            // Mana yenile
            currentMana = Mathf.Min(currentMana + manaRegenRate * Time.deltaTime, maxMana);
            
            // Cooldown güncelle
            foreach (Skill s in skills)
            {
                if (s.currentCooldown > 0)
                    s.currentCooldown -= Time.deltaTime;
            }
            
            // Tuş kontrolü (1-5)
            if (Input.GetKeyDown(KeyCode.Alpha1)) CastSkill(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) CastSkill(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) CastSkill(2);
            if (Input.GetKeyDown(KeyCode.Alpha4)) CastSkill(3);
            if (Input.GetKeyDown(KeyCode.Alpha5)) CastSkill(4);
        }
        
        void SetupDefaultSkills()
        {
            skills.Clear();
            
            skills.Add(new Skill { name = "Power Strike", damage = 30, manaCost = 10, cooldown = 1f });
            skills.Add(new Skill { name = "Fireball", damage = 50, manaCost = 20, cooldown = 2f });
            skills.Add(new Skill { name = "Heal", healAmount = 40, manaCost = 15, cooldown = 5f });
            skills.Add(new Skill { name = "Smite", damage = 80, manaCost = 25, cooldown = 3f });
            skills.Add(new Skill { name = "Frost", damage = 25, manaCost = 15, cooldown = 2f });
            
            Debug.Log($"✅ {skills.Count} yetenek yüklendi!");
        }
        
        void CastSkill(int index)
        {
            if (index >= skills.Count) return;
            
            Skill skill = skills[index];
            
            if (skill.currentCooldown > 0)
            {
                Debug.Log($"⏳ {skill.name} bekliyor!");
                return;
            }
            
            if (currentMana < skill.manaCost)
            {
                Debug.Log($"❌ Mana yok! ({currentMana}/{skill.manaCost})");
                return;
            }
            
            currentMana -= (int)skill.manaCost;
            skill.currentCooldown = skill.cooldown;
            
            Debug.Log($"🔥 {skill.name}!");
            
            // Hasar ver
            if (skill.damage > 0)
            {
                Transform target = FindNearestEnemy();
                if (target != null)
                {
                    IDamageable dmg = target.GetComponent<IDamageable>();
                    if (dmg != null)
                    {
                        dmg.TakeDamage(skill.damage, false);
                        Debug.Log($"💥 {target.name} -> {skill.damage} hasar!");
                    }
                }
            }
            
            // Şifa ver
            if (skill.healAmount > 0)
            {
                PlayerController pc = GetComponent<PlayerController>();
                if (pc != null)
                {
                    pc.Heal((int)skill.healAmount);
                    Debug.Log($"💚 +{skill.healAmount} can!");
                }
            }
        }
        
        Transform FindNearestEnemy()
        {
            MonsterController[] monsters = FindObjectsOfType<MonsterController>();
            float closest = float.MaxValue;
            Transform result = null;
            
            foreach (MonsterController m in monsters)
            {
                if (m == null) continue;
                float d = Vector3.Distance(transform.position, m.transform.position);
                if (d < closest && d <= 15f)
                {
                    closest = d;
                    result = m.transform;
                }
            }
            return result;
        }
    }
    
    [System.Serializable]
    public class Skill
    {
        public string name = "Skill";
        public int damage = 0;
        public float healAmount = 0;
        public float manaCost = 10;
        public float cooldown = 1f;
        [HideInInspector] public float currentCooldown = 0;
    }
}
