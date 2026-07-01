using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace KnightOnline
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        
        [Header("Health Bar")]
        public Image healthBarFill;
        public TextMeshProUGUI healthText;
        
        [Header("Mana Bar")]
        public Image manaBarFill;
        public TextMeshProUGUI manaText;
        
        [Header("Skill Icons")]
        public Image[] skillIconImages;
        public TextMeshProUGUI[] skillHotkeyTexts;
        public Image[] skillCooldownImages;
        public TextMeshProUGUI[] skillCooldownTexts;
        
        [Header("Notifications")]
        public TextMeshProUGUI notificationText;
        float notificationTimer = 0;
        
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
                player.OnHealthChanged += UpdateHealthBar;
                player.OnManaChanged += UpdateManaBar;
            }
            
            if (skillSystem != null)
            {
                UpdateSkillUI();
            }
        }
        
        void Update()
        {
            UpdateNotification();
            UpdateSkillCooldowns();
        }
        
        void UpdateHealthBar(int current, int max)
        {
            if (healthBarFill != null)
                healthBarFill.fillAmount = (float)current / max;
            
            if (healthText != null)
                healthText.text = $"{current}/{max}";
        }
        
        void UpdateManaBar(int current, int max)
        {
            if (manaBarFill != null)
                manaBarFill.fillAmount = (float)current / max;
            
            if (manaText != null)
                manaText.text = $"{current}/{max}";
        }
        
        void UpdateSkillUI()
        {
            if (skillSystem == null || skillSystem.skills == null) return;
            
            for (int i = 0; i < skillIconImages.Length && i < skillSystem.skills.Count; i++)
            {
                if (skillHotkeyTexts[i] != null)
                    skillHotkeyTexts[i].text = (i + 1).ToString();
                
                if (skillCooldownTexts[i] != null)
                    skillCooldownTexts[i].text = "";
            }
        }
        
        void UpdateSkillCooldowns()
        {
            if (skillSystem == null || skillSystem.skills == null) return;
            
            for (int i = 0; i < skillCooldownImages.Length && i < skillSystem.skills.Count; i++)
            {
                Skill skill = skillSystem.skills[i];
                
                if (skill.currentCooldown > 0)
                {
                    float fill = skill.currentCooldown / skill.cooldown;
                    if (skillCooldownImages[i] != null)
                        skillCooldownImages[i].fillAmount = fill;
                    
                    if (skillCooldownTexts[i] != null)
                        skillCooldownTexts[i].text = Mathf.CeilToInt(skill.currentCooldown).ToString();
                }
                else
                {
                    if (skillCooldownImages[i] != null)
                        skillCooldownImages[i].fillAmount = 0;
                }
            }
        }
        
        public void ShowNotification(string message, float duration = 2f)
        {
            if (notificationText != null)
            {
                notificationText.text = message;
                notificationTimer = duration;
            }
        }
        
        void UpdateNotification()
        {
            if (notificationTimer > 0)
            {
                notificationTimer -= Time.deltaTime;
                if (notificationTimer <= 0 && notificationText != null)
                {
                    notificationText.text = "";
                }
            }
        }
    }
}
