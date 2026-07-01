using UnityEngine;

namespace KnightOnline
{
    public class GameSettings : MonoBehaviour
    {
        public static GameSettings Instance { get; private set; }
        
        [Header("Game Settings")]
        public string serverName = "Knight Online Server";
        public int maxPlayers = 100;
        public float gameTime = 0f;
        public bool isPaused = false;
        
        [Header("Player Settings")]
        public float playerMoveSpeed = 5f;
        public float playerRotationSpeed = 10f;
        public float attackRange = 2f;
        public float attackCooldown = 1f;
        
        [Header("Monster Settings")]
        public float monsterRespawnTime = 30f;
        public int monsterDamage = 10;
        public float monsterDetectionRange = 10f;
        
        [Header("Skill Settings")]
        public float skillCooldownMultiplier = 1f;
        public float skillDamageMultiplier = 1f;
        
        [Header("UI Settings")]
        public Color healthBarColor = Color.red;
        public Color manaBarColor = Color.blue;
        public Color expBarColor = Color.yellow;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void SaveSettings()
        {
            PlayerPrefs.SetFloat("PlayerMoveSpeed", playerMoveSpeed);
            PlayerPrefs.SetFloat("AttackRange", attackRange);
            PlayerPrefs.Save();
        }
        
        public void LoadSettings()
        {
            if (PlayerPrefs.HasKey("PlayerMoveSpeed"))
            {
                playerMoveSpeed = PlayerPrefs.GetFloat("PlayerMoveSpeed");
                attackRange = PlayerPrefs.GetFloat("AttackRange");
            }
        }
    }
}
