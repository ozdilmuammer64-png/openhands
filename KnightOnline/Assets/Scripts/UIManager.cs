using UnityEngine;

namespace KnightOnline
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        
        PlayerController player;
        
        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        
        void Start()
        {
            player = FindObjectOfType<PlayerController>();
            
            if (player != null)
            {
                player.OnHealthChanged += OnHealthChanged;
                player.OnManaChanged += OnManaChanged;
            }
            
            Debug.Log("✅ UI Hazır!");
        }
        
        void OnHealthChanged(int current, int max)
        {
            Debug.Log($"❤️ Can: {current}/{max}");
        }
        
        void OnManaChanged(int current, int max)
        {
            Debug.Log($"💙 Mana: {current}/{max}");
        }
    }
}
