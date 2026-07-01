using UnityEngine;
using System.Collections.Generic;

namespace KnightOnline
{
    public class SkillSystem : MonoBehaviour
    {
        public static SkillSystem Instance { get; private set; }
        
        [Header("Skills")]
        public List<Skill> skills = new List<Skill>();
        
        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        
        void Start()
        {
            SetupDefaultSkills();
        }
        
        void SetupDefaultSkills()
        {
            skills.Clear();
            
            // 1 - Güçlü Vuruş
            skills.Add(new Skill {
                name = "Power Strike",
                damage = 30,
                manaCost = 10,
                cooldown = 1f,
                description = "Güçlü bir saldırı"
            });
            
            // 2 - Ateş Topu
            skills.Add(new Skill {
                name = "Fireball",
                damage = 50,
                manaCost = 20,
                cooldown = 2f,
                description = "Ateş topu fırlat"
            });
            
            // 3 - Şifa
            skills.Add(new Skill {
                name = "Heal",
                healAmount = 40,
                manaCost = 15,
                cooldown = 5f,
                description = "Kendini iyileştir"
            });
            
            // 4 - Şimşek
            skills.Add(new Skill {
                name = "Lightning",
                damage = 80,
                manaCost = 25,
                cooldown = 3f,
                description = "Yıldırım çarpması"
            });
            
            // 5 - Buz
            skills.Add(new Skill {
                name = "Frost",
                damage = 25,
                manaCost = 15,
                cooldown = 2f,
                description = "Buz saldırısı"
            });
            
            Debug.Log($"✅ {skills.Count} yetenek yüklendi!");
        }
        
        void Update()
        {
            // Cooldown güncelle
            foreach (Skill s in skills)
            {
                if (s.currentCooldown > 0)
                    s.currentCooldown -= Time.deltaTime;
            }
            
            // Tuş kontrolü
            if (Input.GetKeyDown(KeyCode.Alpha1)) UseSkill(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) UseSkill(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) UseSkill(2);
            if (Input.GetKeyDown(KeyCode.Alpha4)) UseSkill(3);
            if (Input.GetKeyDown(KeyCode.Alpha5)) UseSkill(4);
        }
        
        public void UseSkill(int index)
        {
            if (index >= skills.Count) return;
            
            Skill skill = skills[index];
            
            // Cooldown kontrolü
            if (skill.currentCooldown > 0)
            {
                Debug.Log($"⏳ {skill.name} bekliyor ({skill.currentCooldown:F1}s)");
                return;
            }
            
            // Mana kontrolü
            PlayerController player = GetComponent<PlayerController>();
            if (player == null) return;
            
            if (player.currentMana < skill.manaCost)
            {
                Debug.Log($"❌ Mana yok! (Gerekli: {skill.manaCost}, Mevcut: {player.currentMana})");
                return;
            }
            
            // Mana harca
            player.UseMana((int)skill.manaCost);
            skill.currentCooldown = skill.cooldown;
            
            Debug.Log($"🔥 {skill.name}!");
            
            // Hasar yeteneği
            if (skill.damage > 0)
            {
                Transform target = FindNearestEnemy();
                if (target != null)
                {
                    IDamageable dmg = target.GetComponent<IDamageable>();
                    if (dmg != null)
                    {
                        dmg.TakeDamage(skill.damage);
                        Debug.Log($"💥 {target.name} -> {skill.damage} hasar!");
                    }
                }
            }
            
            // Şifa yeteneği
            if (skill.healAmount > 0)
            {
                player.Heal((int)skill.healAmount);
                Debug.Log($"💚 +{skill.healAmount} can!");
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
                float d = Vector2.Distance(transform.position, m.transform.position);
                if (d < closest && d <= 15f)
                {
                    closest = d;
                    result = m.transform;
                }
            }
            return result;
        }
        
        public Skill GetSkill(int index)
        {
            if (index >= 0 && index < skills.Count)
                return skills[index];
            return null;
        }
    }
    
    [System.Serializable]
    public class Skill
    {
        public string name = "Skill";
        public string description = "";
        public int damage = 0;
        public int healAmount = 0;
        public int manaCost = 10;
        public float cooldown = 1f;
        [HideInInspector] public float currentCooldown = 0;
    }
}
