using UnityEngine;
using System;

namespace KnightOnline.Scripts
{
    [Serializable]
    public class GameSaveData
    {
        public PlayerSaveData playerData;
        public InventorySaveData inventoryData;
        public QuestSaveData questData;
        public SkillSaveData skillData;
        public WorldSaveData worldData;
        public SettingsSaveData settingsData;
        public DateTime saveTime;
        public string saveVersion;
    }
    
    [Serializable]
    public class PlayerSaveData
    {
        public string playerName;
        public int playerClass;
        public int level;
        public int experience;
        public int gold;
        
        public float maxHealth;
        public float currentHealth;
        public float maxMana;
        public float currentMana;
        public float maxStamina;
        public float currentStamina;
        
        public int strength;
        public int dexterity;
        public int intelligence;
        public int constitution;
        
        public Vector3 position;
        public Quaternion rotation;
    }
    
    [Serializable]
    public class InventorySaveData
    {
        public List<ItemSlotSave> inventory;
        public List<ItemSlotSave> equipment;
    }
    
    [Serializable]
    public class ItemSlotSave
    {
        public string itemName;
        public int quantity;
        public int slotIndex;
    }
    
    [Serializable]
    public class QuestSaveData
    {
        public List<string> completedQuests;
        public List<ActiveQuestSave> activeQuests;
    }
    
    [Serializable]
    public class ActiveQuestSave
    {
        public string questName;
        public Dictionary<string, int> objectiveProgress;
    }
    
    [Serializable]
    public class SkillSaveData
    {
        public List<string> learnedSkills;
        public List<float> skillCooldowns;
    }
    
    [Serializable]
    public class WorldSaveData
    {
        public List<MonsterSpawnSave> monsterSpawns;
        public List<PickupSave> pickups;
        public float gameTime;
    }
    
    [Serializable]
    public class MonsterSpawnSave
    {
        public string monsterName;
        public Vector3 position;
        public bool isAlive;
        public float health;
    }
    
    [Serializable]
    public class PickupSave
    {
        public string itemName;
        public Vector3 position;
        public bool isPickedUp;
    }
    
    [Serializable]
    public class SettingsSaveData
    {
        public float masterVolume;
        public float musicVolume;
        public float sfxVolume;
        public float mouseSensitivity;
        public bool invertedY;
        public int qualityLevel;
        public bool fullscreen;
        public int resolutionWidth;
        public int resolutionHeight;
    }
    
    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance { get; private set; }
        
        [Header("Save Settings")]
        public string saveFileName = "KnightOnlineSave";
        public int maxAutoSaves = 3;
        public float autoSaveInterval = 300f; // 5 minutes
        
        [Header("Current Save")]
        public GameSaveData currentSave;
        
        private float lastAutoSave;
        private bool isSaving;
        
        void Awake()
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
        
        void Update()
        {
            // Auto save
            if (Time.time - lastAutoSave >= autoSaveInterval)
            {
                if (ShouldAutoSave())
                {
                    AutoSave();
                    lastAutoSave = Time.time;
                }
            }
            
            // Manual save
            if (Input.GetKeyDown(KeyCode.F5))
            {
                SaveGame();
            }
            
            // Manual load
            if (Input.GetKeyDown(KeyCode.F9))
            {
                LoadGame();
            }
        }
        
        bool ShouldAutoSave()
        {
            // Don't auto save if in main menu or cutscene
            PlayerController player = FindObjectOfType<PlayerController>();
            return player != null && !player.IsDead;
        }
        
        public void SaveGame(string saveName = null)
        {
            if (isSaving) return;
            StartCoroutine(SaveGameCoroutine(saveName));
        }
        
        System.Collections.IEnumerator SaveGameCoroutine(string saveName)
        {
            isSaving = true;
            Debug.Log("💾 Oyun kaydediliyor...");
            
            GameSaveData saveData = new GameSaveData
            {
                saveTime = DateTime.Now,
                saveVersion = "1.0.0"
            };
            
            // Save player data
            saveData.playerData = SavePlayerData();
            
            // Save inventory
            saveData.inventoryData = SaveInventoryData();
            
            // Save quests
            saveData.questData = SaveQuestData();
            
            // Save skills
            saveData.skillData = SaveSkillData();
            
            // Save world
            saveData.worldData = SaveWorldData();
            
            // Save settings
            saveData.settingsData = SaveSettingsData();
            
            // Convert to JSON
            string json = JsonUtility.ToJson(saveData, true);
            
            // Generate filename
            string fileName = string.IsNullOrEmpty(saveName) ? 
                $"{saveFileName}_{DateTime.Now:yyyyMMdd_HHmmss}.json" : 
                $"{saveFileName}_{saveName}.json";
            
            // Save to file
            string path = Application.persistentDataPath + "/" + fileName;
            System.IO.File.WriteAllText(path, json);
            
            Debug.Log($"✅ Oyun kaydedildi: {fileName}");
            
            UIManager.Instance?.ShowNotification("Oyun kaydedildi!", NotificationType.Success);
            
            yield return new WaitForSeconds(0.5f);
            isSaving = false;
        }
        
        PlayerSaveData SavePlayerData()
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player == null) return null;
            
            return new PlayerSaveData
            {
                playerName = player.playerName,
                playerClass = (int)player.playerClass,
                level = player.level,
                experience = player.experience,
                gold = player.gold,
                
                maxHealth = player.maxHealth,
                currentHealth = player.currentHealth,
                maxMana = player.maxMana,
                currentMana = player.currentMana,
                maxStamina = player.maxStamina,
                currentStamina = player.currentStamina,
                
                strength = player.strength,
                dexterity = player.dexterity,
                intelligence = player.intelligence,
                constitution = player.constitution,
                
                position = player.transform.position,
                rotation = player.transform.rotation
            };
        }
        
        InventorySaveData SaveInventoryData()
        {
            InventorySaveData data = new InventorySaveData
            {
                inventory = new List<ItemSlotSave>(),
                equipment = new List<ItemSlotSave>()
            };
            
            InventorySystem inventory = FindObjectOfType<InventorySystem>();
            if (inventory == null) return data;
            
            foreach (InventoryItem item in inventory.inventory)
            {
                if (item != null && item.data != null)
                {
                    data.inventory.Add(new ItemSlotSave
                    {
                        itemName = item.data.name,
                        quantity = item.quantity,
                        slotIndex = item.slotIndex
                    });
                }
            }
            
            foreach (InventoryItem item in inventory.equipment)
            {
                if (item != null && item.data != null)
                {
                    data.equipment.Add(new ItemSlotSave
                    {
                        itemName = item.data.name,
                        quantity = item.quantity,
                        slotIndex = item.slotIndex
                    });
                }
            }
            
            return data;
        }
        
        QuestSaveData SaveQuestData()
        {
            QuestSaveData data = new QuestSaveData
            {
                completedQuests = new List<string>(),
                activeQuests = new List<ActiveQuestSave>()
            };
            
            QuestSystem quests = FindObjectOfType<QuestSystem>();
            if (quests == null) return data;
            
            foreach (QuestData quest in quests.completedQuests)
            {
                data.completedQuests.Add(quest.name);
            }
            
            foreach (ActiveQuest active in quests.activeQuests)
            {
                data.activeQuests.Add(new ActiveQuestSave
                {
                    questName = active.data.name,
                    objectiveProgress = new Dictionary<string, int>(active.objectiveProgress)
                });
            }
            
            return data;
        }
        
        SkillSaveData SaveSkillData()
        {
            SkillSaveData data = new SkillSaveData
            {
                learnedSkills = new List<string>(),
                skillCooldowns = new List<float>()
            };
            
            SkillSystem skills = FindObjectOfType<SkillSystem>();
            if (skills == null) return data;
            
            foreach (LearnedSkill skill in skills.learnedSkills)
            {
                if (skill.data != null)
                {
                    data.learnedSkills.Add(skill.data.name);
                    data.skillCooldowns.Add(skill.currentCooldown);
                }
            }
            
            return data;
        }
        
        WorldSaveData SaveWorldData()
        {
            WorldSaveData data = new WorldSaveData
            {
                monsterSpawns = new List<MonsterSpawnSave>(),
                pickups = new List<PickupSave>(),
                gameTime = GameSettings.Instance?.gameTime ?? 0
            };
            
            // Save monster states
            MonsterController[] monsters = FindObjectsOfType<MonsterController>();
            foreach (MonsterController monster in monsters)
            {
                data.monsterSpawns.Add(new MonsterSpawnSave
                {
                    monsterName = monster.monsterName,
                    position = monster.transform.position,
                    isAlive = !monster.IsDead,
                    health = monster.CurrentHealth
                });
            }
            
            // Save pickup states
            ItemPickup[] pickups = FindObjectsOfType<ItemPickup>();
            foreach (ItemPickup pickup in pickups)
            {
                data.pickups.Add(new PickupSave
                {
                    itemName = pickup.itemData?.name ?? "",
                    position = pickup.transform.position,
                    isPickedUp = false
                });
            }
            
            return data;
        }
        
        SettingsSaveData SaveSettingsData()
        {
            SettingsSaveData data = new SettingsSaveData
            {
                masterVolume = AudioSystem.Instance?.masterVolume ?? 1f,
                musicVolume = AudioSystem.Instance?.musicVolume ?? 0.7f,
                sfxVolume = AudioSystem.Instance?.sfxVolume ?? 1f,
                qualityLevel = QualitySettings.GetQualityLevel(),
                fullscreen = Screen.fullscreen
            };
            
            Resolution res = Screen.currentResolution;
            data.resolutionWidth = res.width;
            data.resolutionHeight = res.height;
            
            return data;
        }
        
        public void LoadGame(string saveName = null)
        {
            string fileName = string.IsNullOrEmpty(saveName) ? 
                GetLatestSaveFile() : 
                $"{saveFileName}_{saveName}.json";
            
            if (string.IsNullOrEmpty(fileName))
            {
                Debug.LogWarning("Kayıtlı oyun bulunamadı!");
                UIManager.Instance?.ShowNotification("Kayıtlı oyun bulunamadı!", NotificationType.Warning);
                return;
            }
            
            string path = Application.persistentDataPath + "/" + fileName;
            if (!System.IO.File.Exists(path))
            {
                Debug.LogWarning($"Kayıt dosyası bulunamadı: {path}");
                return;
            }
            
            string json = System.IO.File.ReadAllText(path);
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
            
            if (saveData == null)
            {
                Debug.LogError("Kayıt verisi yüklenemedi!");
                return;
            }
            
            StartCoroutine(LoadGameCoroutine(saveData));
        }
        
        System.Collections.IEnumerator LoadGameCoroutine(GameSaveData saveData)
        {
            Debug.Log("📂 Oyun yükleniyor...");
            
            // Load player data
            if (saveData.playerData != null)
            {
                LoadPlayerData(saveData.playerData);
            }
            
            // Load inventory
            if (saveData.inventoryData != null)
            {
                LoadInventoryData(saveData.inventoryData);
            }
            
            // Load quests
            if (saveData.questData != null)
            {
                LoadQuestData(saveData.questData);
            }
            
            // Load skills
            if (saveData.skillData != null)
            {
                LoadSkillData(saveData.skillData);
            }
            
            // Load settings
            if (saveData.settingsData != null)
            {
                LoadSettingsData(saveData.settingsData);
            }
            
            Debug.Log("✅ Oyun yüklendi!");
            UIManager.Instance?.ShowNotification("Oyun yüklendi!", NotificationType.Success);
            
            yield return null;
        }
        
        void LoadPlayerData(PlayerSaveData data)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player == null) return;
            
            player.playerName = data.playerName;
            player.playerClass = (PlayerClass)data.playerClass;
            player.level = data.level;
            player.experience = data.experience;
            player.gold = data.gold;
            
            player.maxHealth = data.maxHealth;
            player.currentHealth = data.currentHealth;
            player.maxMana = data.maxMana;
            player.currentMana = data.currentMana;
            player.maxStamina = data.maxStamina;
            player.currentStamina = data.currentStamina;
            
            player.strength = data.strength;
            player.dexterity = data.dexterity;
            player.intelligence = data.intelligence;
            player.constitution = data.constitution;
            
            player.transform.position = data.position;
            player.transform.rotation = data.rotation;
        }
        
        void LoadInventoryData(InventorySaveData data)
        {
            // Implementation for loading inventory
        }
        
        void LoadQuestData(QuestSaveData data)
        {
            // Implementation for loading quests
        }
        
        void LoadSkillData(SkillSaveData data)
        {
            // Implementation for loading skills
        }
        
        void LoadSettingsData(SettingsSaveData data)
        {
            if (AudioSystem.Instance != null)
            {
                AudioSystem.Instance.SetMasterVolume(data.masterVolume);
                AudioSystem.Instance.SetMusicVolume(data.musicVolume);
                AudioSystem.Instance.SetSFXVolume(data.sfxVolume);
            }
            
            QualitySettings.SetQualityLevel(data.qualityLevel);
            Screen.fullscreen = data.fullscreen;
        }
        
        string GetLatestSaveFile()
        {
            string[] files = System.IO.Directory.GetFiles(Application.persistentDataPath, $"{saveFileName}_*.json");
            
            if (files.Length == 0) return null;
            
            // Sort by date and get latest
            Array.Sort(files);
            return System.IO.Path.GetFileName(files[files.Length - 1]);
        }
        
        void AutoSave()
        {
            SaveGame("AutoSave");
        }
        
        public bool HasSaveFile()
        {
            string latestSave = GetLatestSaveFile();
            return !string.IsNullOrEmpty(latestSave);
        }
        
        public string[] GetAllSaveFiles()
        {
            return System.IO.Directory.GetFiles(Application.persistentDataPath, $"{saveFileName}_*.json");
        }
        
        public void DeleteSave(string saveName)
        {
            string fileName = $"{saveFileName}_{saveName}.json";
            string path = Application.persistentDataPath + "/" + fileName;
            
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                Debug.Log($"🗑️ Kayıt silindi: {fileName}");
            }
        }
    }
}
