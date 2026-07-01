using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace KnightOnline
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        
        [Header("UI Panels")]
        public GameObject mainMenuPanel;
        public GameObject gameUI;
        public GameObject inventoryPanel;
        public GameObject characterPanel;
        public GameObject skillPanel;
        public GameObject questPanel;
        public GameObject mapPanel;
        public GameObject settingsPanel;
        public GameObject npcDialoguePanel;
        public GameObject shopPanel;
        public GameObject repairPanel;
        public GameObject deathPanel;
        
        [Header("HUD Elements")]
        public Text playerNameText;
        public Text levelText;
        public Image healthBar;
        public Image manaBar;
        public Image staminaBar;
        public Image expBar;
        public Text healthText;
        public Text manaText;
        public Text staminaText;
        public Text expText;
        public Text goldText;
        
        [Header("Skill Bar")]
        public List<Image> skillSlotImages = new List<Image>();
        public List<Text> skillSlotTexts = new List<Text>();
        public List<Image> skillCooldownOverlays = new List<Image>();
        
        [Header("Target Info")]
        public GameObject targetInfoPanel;
        public Text targetNameText;
        public Image targetHealthBar;
        public Text targetHealthText;
        public Image targetLevelBadge;
        
        [Header("Quest Tracker")]
        public GameObject questTrackerPanel;
        public Text trackedQuestName;
        public Text trackedQuestObjective;
        public Text trackedQuestProgress;
        
        [Header("Notifications")]
        public GameObject notificationPrefab;
        public Transform notificationParent;
        public List<GameObject> activeNotifications = new List<GameObject>();
        
        [Header("Damage Numbers")]
        public GameObject damageNumberPrefab;
        public Camera mainCamera;
        
        [Header("Components")]
        public PlayerController player;
        public EventSystem eventSystem;
        
        // Panel states
        private Dictionary<GameObject, bool> panelStates = new Dictionary<GameObject, bool>();
        private bool anyPanelOpen;
        
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
            SetupUI();
            InitializePanelStates();
            SubscribeToEvents();
            
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            
            if (player == null)
            {
                player = FindObjectOfType<PlayerController>();
            }
        }
        
        void Update()
        {
            HandleInput();
            UpdateHUD();
        }
        
        void HandleInput()
        {
            // Toggle panels with keyboard
            if (Input.GetKeyDown(KeyCode.I))
            {
                TogglePanel(inventoryPanel);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                TogglePanel(characterPanel);
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                TogglePanel(skillPanel);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                TogglePanel(questPanel);
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                TogglePanel(mapPanel);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseAllPanels();
                if (anyPanelOpen) CloseAllPanels();
                else OpenSettings();
            }
        }
        
        void SetupUI()
        {
            if (gameUI == null)
            {
                gameUI = CreateGameUI();
            }
            
            if (eventSystem == null)
            {
                EventSystem existingSystem = FindObjectOfType<EventSystem>();
                if (existingSystem == null)
                {
                    GameObject eventSys = new GameObject("EventSystem");
                    eventSystem = eventSys.AddComponent<EventSystem>();
                    eventSys.AddComponent<StandaloneInputModule>();
                }
                else
                {
                    eventSystem = existingSystem;
                }
            }
        }
        
        GameObject CreateGameUI()
        {
            GameObject canvasObj = new GameObject("GameUI");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            return canvasObj;
        }
        
        void InitializePanelStates()
        {
            GameObject[] panels = { inventoryPanel, characterPanel, skillPanel, questPanel, mapPanel, 
                                   settingsPanel, npcDialoguePanel, shopPanel, repairPanel, deathPanel };
            
            foreach (GameObject panel in panels)
            {
                if (panel != null)
                {
                    panelStates[panel] = panel.activeSelf;
                }
            }
        }
        
        void SubscribeToEvents()
        {
            if (player != null)
            {
                player.OnHealthChanged += UpdateHealthBar;
                player.OnManaChanged += UpdateManaBar;
                player.OnStaminaChanged += UpdateStaminaBar;
                player.OnExperienceGained += UpdateExpBar;
                player.OnLevelUp += OnPlayerLevelUp;
                player.OnDeath += OnPlayerDeath;
                player.OnTargetLocked += OnTargetLocked;
            }
            
            if (SkillSystem.Instance != null)
            {
                SkillSystem.Instance.OnSkillCast += OnSkillCast;
                SkillSystem.Instance.OnCooldownStart += OnSkillCooldownStart;
                SkillSystem.Instance.OnCooldownEnd += OnSkillCooldownEnd;
            }
        }
        
        void UpdateHUD()
        {
            if (player == null) return;
            
            if (playerNameText != null)
            {
                playerNameText.text = player.playerName;
            }
            
            if (levelText != null)
            {
                levelText.text = $"Lv.{player.level}";
            }
            
            if (goldText != null)
            {
                goldText.text = player.gold.ToString("N0");
            }
        }
        
        void UpdateHealthBar(float current, float max)
        {
            if (healthBar != null)
            {
                healthBar.fillAmount = current / max;
            }
            if (healthText != null)
            {
                healthText.text = $"{Mathf.RoundToInt(current)}/{Mathf.RoundToInt(max)}";
            }
        }
        
        void UpdateManaBar(float current, float max)
        {
            if (manaBar != null)
            {
                manaBar.fillAmount = current / max;
            }
            if (manaText != null)
            {
                manaText.text = $"{Mathf.RoundToInt(current)}/{Mathf.RoundToInt(max)}";
            }
        }
        
        void UpdateStaminaBar(float current, float max)
        {
            if (staminaBar != null)
            {
                staminaBar.fillAmount = current / max;
            }
            if (staminaText != null)
            {
                staminaText.text = $"{Mathf.RoundToInt(current)}/{Mathf.RoundToInt(max)}";
            }
        }
        
        void UpdateExpBar(int current, int needed)
        {
            if (expBar != null)
            {
                expBar.fillAmount = (float)current / needed;
            }
            if (expText != null)
            {
                expText.text = $"{current}/{needed}";
            }
        }
        
        void OnPlayerLevelUp(int newLevel)
        {
            ShowNotification($"LEVEL UP! You are now level {newLevel}!", NotificationType.LevelUp);
        }
        
        void OnPlayerDeath()
        {
            if (deathPanel != null)
            {
                deathPanel.SetActive(true);
            }
        }
        
        void OnTargetLocked(Transform target)
        {
            if (target == null)
            {
                if (targetInfoPanel != null)
                {
                    targetInfoPanel.SetActive(false);
                }
                return;
            }
            
            if (targetInfoPanel != null)
            {
                targetInfoPanel.SetActive(true);
                
                MonsterController monster = target.GetComponent<MonsterController>();
                if (monster != null)
                {
                    if (targetNameText != null)
                    {
                        targetNameText.text = monster.monsterName;
                    }
                    monster.OnHealthChanged += (current, max) =>
                    {
                        if (targetHealthBar != null)
                        {
                            targetHealthBar.fillAmount = current / max;
                        }
                        if (targetHealthText != null)
                        {
                            targetHealthText.text = $"{Mathf.RoundToInt(current)}/{Mathf.RoundToInt(max)}";
                        }
                    };
                }
            }
        }
        
        void OnSkillCast(SkillData skill)
        {
            ShowNotification($"Cast {skill.skillName}", NotificationType.Skill);
        }
        
        void OnSkillCooldownStart(SkillData skill, float cooldown)
        {
            // Update cooldown UI
        }
        
        void OnSkillCooldownEnd(SkillData skill)
        {
            // Update cooldown UI
        }
        
        public void TogglePanel(GameObject panel)
        {
            if (panel == null) return;
            
            bool isActive = panel.activeSelf;
            panel.SetActive(!isActive);
            panelStates[panel] = !isActive;
            
            anyPanelOpen = false;
            foreach (var state in panelStates)
            {
                if (state.Value)
                {
                    anyPanelOpen = true;
                    break;
                }
            }
            
            Time.timeScale = anyPanelOpen ? 0f : 1f;
        }
        
        public void OpenPanel(GameObject panel)
        {
            if (panel != null && !panel.activeSelf)
            {
                panel.SetActive(true);
                panelStates[panel] = true;
                anyPanelOpen = true;
                Time.timeScale = 0f;
            }
        }
        
        public void ClosePanel(GameObject panel)
        {
            if (panel != null && panel.activeSelf)
            {
                panel.SetActive(false);
                panelStates[panel] = false;
                
                anyPanelOpen = false;
                foreach (var state in panelStates)
                {
                    if (state.Value)
                    {
                        anyPanelOpen = true;
                        break;
                    }
                }
                
                if (!anyPanelOpen)
                {
                    Time.timeScale = 1f;
                }
            }
        }
        
        public void CloseAllPanels()
        {
            foreach (var state in panelStates)
            {
                if (state.Key != null)
                {
                    state.Key.SetActive(false);
                    panelStates[state.Key] = false;
                }
            }
            anyPanelOpen = false;
            Time.timeScale = 1f;
        }
        
        public void OpenNPCDialogue(NPCController npc)
        {
            if (npcDialoguePanel == null) return;
            
            OpenPanel(npcDialoguePanel);
            
            // Update dialogue UI with NPC data
            Text dialogueText = npcDialoguePanel.transform.Find("DialogueText")?.GetComponent<Text>();
            if (dialogueText != null && npc.npcData != null)
            {
                dialogueText.text = npc.npcData.dialogueLines.Count > 0 ? 
                    npc.npcData.dialogueLines[0].text : "Hello traveler!";
            }
        }
        
        public void CloseNPCDialogue()
        {
            ClosePanel(npcDialoguePanel);
        }
        
        public void OpenShop(NPCController npc)
        {
            if (shopPanel == null) return;
            OpenPanel(shopPanel);
            // Populate shop with NPC's items
        }
        
        public void CloseShop()
        {
            ClosePanel(shopPanel);
        }
        
        public void OpenRepair(NPCController npc)
        {
            if (repairPanel == null) return;
            OpenPanel(repairPanel);
        }
        
        public void CloseRepair()
        {
            ClosePanel(repairPanel);
        }
        
        public void OpenSettings()
        {
            OpenPanel(settingsPanel);
        }
        
        public void CloseSettings()
        {
            ClosePanel(settingsPanel);
        }
        
        public void RespawnPlayer()
        {
            if (deathPanel != null)
            {
                deathPanel.SetActive(false);
            }
            Time.timeScale = 1f;
            // Trigger player respawn
        }
        
        public void ShowNotification(string message, NotificationType type = NotificationType.Info)
        {
            if (notificationPrefab == null || notificationParent == null) return;
            
            GameObject notification = Instantiate(notificationPrefab, notificationParent);
            Text text = notification.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = message;
                
                Color color = type switch
                {
                    NotificationType.Info => Color.white,
                    NotificationType.Success => Color.green,
                    NotificationType.Warning => Color.yellow,
                    NotificationType.Error => Color.red,
                    NotificationType.LevelUp => new Color(1f, 0.84f, 0f),
                    NotificationType.Skill => new Color(0.5f, 0.8f, 1f),
                    NotificationType.Quest => new Color(1f, 0.5f, 0f),
                    _ => Color.white
                };
                text.color = color;
            }
            
            activeNotifications.Add(notification);
            
            // Auto destroy after delay
            Destroy(notification, 3f);
        }
        
        public void ShowDamageNumber(Vector3 position, int damage, bool isCritical, bool isHeal = false)
        {
            if (damageNumberPrefab == null || mainCamera == null) return;
            
            Vector3 screenPos = mainCamera.WorldToScreenPoint(position);
            if (screenPos.z < 0) return;
            
            GameObject dmgNum = Instantiate(damageNumberPrefab, screenPos, Quaternion.identity, notificationParent);
            Text text = dmgNum.GetComponent<Text>();
            
            if (text != null)
            {
                if (isHeal)
                {
                    text.text = $"+{damage}";
                    text.color = Color.green;
                }
                else
                {
                    text.text = isCritical ? $"CRIT! {damage}" : damage.ToString();
                    text.color = isCritical ? new Color(1f, 0.8f, 0f) : Color.red;
                    text.fontSize = isCritical ? 24 : 18;
                }
            }
            
            // Animate
            StartCoroutine(AnimateDamageNumber(dmgNum));
            
            Destroy(dmgNum, 1f);
        }
        
        System.Collections.IEnumerator AnimateDamageNumber(GameObject dmgNum)
        {
            float duration = 0.8f;
            float speed = 50f;
            Vector3 startPos = dmgNum.transform.position;
            
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                dmgNum.transform.position = startPos + Vector3.up * speed * Time.deltaTime;
                startPos = dmgNum.transform.position;
                
                Text text = dmgNum.GetComponent<Text>();
                if (text != null)
                {
                    Color c = text.color;
                    c.a = 1f - (t / duration);
                    text.color = c;
                }
                
                yield return null;
            }
        }
        
        public void UpdateQuestTracker(QuestData quest)
        {
            if (questTrackerPanel == null) return;
            
            ActiveQuest activeQuest = QuestSystem.Instance?.GetActiveQuest(quest);
            if (activeQuest == null)
            {
                questTrackerPanel.SetActive(false);
                return;
            }
            
            questTrackerPanel.SetActive(true);
            
            if (trackedQuestName != null)
            {
                trackedQuestName.text = quest.questName;
            }
            
            if (trackedQuestObjective != null && quest.objectives.Count > 0)
            {
                QuestObjective obj = quest.objectives[0];
                int target = obj.type == QuestType.Kill ? obj.killCount : obj.collectCount;
                int progress = QuestSystem.Instance.GetObjectiveProgress(activeQuest, obj.objectiveName);
                
                trackedQuestObjective.text = obj.objectiveName;
                trackedQuestProgress.text = $"{progress}/{target}";
            }
        }
    }
    
    public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Error,
        LevelUp,
        Skill,
        Quest,
        Loot
    }
}
