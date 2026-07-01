using UnityEngine;
using UnityEngine.UI;

namespace KnightOnline
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        
        [Header("Health Bar")]
        public Image healthFill;
        public Text healthText;
        
        [Header("Mana Bar")]
        public Image manaFill;
        public Text manaText;
        
        [Header("Skill Bar")]
        public Image[] skillIcons;
        public Text[] skillHotkeys;
        public Image[] skillCooldownFills;
        public Text[] skillCooldownTexts;
        public Text[] skillNames;
        
        PlayerController player;
        SkillSystem skillSystem;
        
        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        
        void Start()
        {
            player = FindObjectOfType<PlayerController>();
            skillSystem = FindObjectOfType<SkillSystem>();
            
            if (player != null)
            {
                player.OnHealthChanged += UpdateHealth;
                player.OnManaChanged += UpdateMana;
            }
            
            UpdateHealth(player?.currentHealth ?? 100, player?.maxHealth ?? 100);
            UpdateMana(player?.currentMana ?? 100, player?.maxMana ?? 100);
            UpdateSkillUI();
        }
        
        void Update()
        {
            UpdateSkillCooldowns();
        }
        
        void UpdateHealth(int current, int max)
        {
            if (healthFill != null)
                healthFill.fillAmount = (float)current / max;
            
            if (healthText != null)
                healthText.text = $"{current}/{max}";
        }
        
        void UpdateMana(int current, int max)
        {
            if (manaFill != null)
                manaFill.fillAmount = (float)current / max;
            
            if (manaText != null)
                manaText.text = $"{current}/{max}";
        }
        
        void UpdateSkillUI()
        {
            if (skillSystem == null || skillSystem.skills == null) return;
            
            for (int i = 0; i < skillSystem.skills.Count; i++)
            {
                if (i >= skillHotkeys?.Length) break;
                
                Skill skill = skillSystem.skills[i];
                
                if (skillHotkeys != null && skillHotkeys[i] != null)
                    skillHotkeys[i].text = (i + 1).ToString();
                
                if (skillNames != null && i < skillNames.Length && skillNames[i] != null)
                    skillNames[i].text = skill.name;
                
                if (skillCooldownTexts != null && i < skillCooldownTexts.Length && skillCooldownTexts[i] != null)
                    skillCooldownTexts[i].text = "";
                
                if (skillCooldownFills != null && i < skillCooldownFills.Length && skillCooldownFills[i] != null)
                    skillCooldownFills[i].fillAmount = 0;
            }
        }
        
        void UpdateSkillCooldowns()
        {
            if (skillSystem == null || skillSystem.skills == null) return;
            
            for (int i = 0; i < skillSystem.skills.Count; i++)
            {
                Skill skill = skillSystem.skills[i];
                
                if (skill.currentCooldown > 0)
                {
                    float fill = skill.currentCooldown / skill.cooldown;
                    
                    if (skillCooldownFills != null && i < skillCooldownFills.Length && skillCooldownFills[i] != null)
                        skillCooldownFills[i].fillAmount = fill;
                    
                    if (skillCooldownTexts != null && i < skillCooldownTexts.Length && skillCooldownTexts[i] != null)
                        skillCooldownTexts[i].text = Mathf.CeilToInt(skill.currentCooldown).ToString();
                }
                else
                {
                    if (skillCooldownFills != null && i < skillCooldownFills.Length && skillCooldownFills[i] != null)
                        skillCooldownFills[i].fillAmount = 0;
                    
                    if (skillCooldownTexts != null && i < skillCooldownTexts.Length && skillCooldownTexts[i] != null)
                        skillCooldownTexts[i].text = "";
                }
            }
        }
    }
}
