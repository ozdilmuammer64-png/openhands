using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD References")]
    public Image healthBarFill;
    public Image manaBarFill;
    public Image xpBarFill;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI xpText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI goldText;

    [Header("Skill Bar")]
    public Image[] skillCooldownImages;
    public TextMeshProUGUI[] skillCooldownTexts;
    public Image[] skillIconImages;
    public Sprite defaultSkillIcon;

    [Header("Notification")]
    public GameObject notificationPrefab;
    public Transform notificationContainer;

    [Header("Death Screen")]
    public GameObject deathScreen;
    public Button respawnButton;

    [Header("Level Up Effect")]
    public GameObject levelUpEffectPrefab;
    public Transform levelUpEffectPosition;

    [Header("Item Pickup")]
    public GameObject itemPickupNotification;
    public TextMeshProUGUI itemPickupName;
    public Image itemPickupIcon;

    [Header("Pause Menu")]
    public GameObject pauseMenu;

    [Header("Inventory Panel")]
    public GameObject inventoryPanel;
    public Transform inventoryGrid;
    public GameObject inventorySlotPrefab;

    private PlayerController player;

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
        player = FindObjectOfType<PlayerController>();

        if (deathScreen != null)
            deathScreen.SetActive(false);

        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        // Setup skill icons
        SetupSkillBar();
    }

    void Update()
    {
        UpdateHUD();
        UpdateSkillCooldowns();

        // Toggle inventory
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    void SetupSkillBar()
    {
        if (skillIconImages != null)
        {
            for (int i = 0; i < skillIconImages.Length; i++)
            {
                if (skillIconImages[i] == null && defaultSkillIcon != null)
                {
                    skillIconImages[i].sprite = defaultSkillIcon;
                }
            }
        }
    }

    void UpdateHUD()
    {
        if (player == null) return;

        float healthPercent = player.GetHealth() / player.GetMaxHealth();
        float manaPercent = player.GetMana() / player.GetMaxMana();
        float xpPercent = player.GetXP() / player.GetXPToLevel();

        if (healthBarFill != null)
            healthBarFill.fillAmount = Mathf.Lerp(healthBarFill.fillAmount, healthPercent, Time.deltaTime * 5);

        if (manaBarFill != null)
            manaBarFill.fillAmount = Mathf.Lerp(manaBarFill.fillAmount, manaPercent, Time.deltaTime * 5);

        if (xpBarFill != null)
            xpBarFill.fillAmount = Mathf.Lerp(xpBarFill.fillAmount, xpPercent, Time.deltaTime * 5);

        if (healthText != null)
            healthText.text = $"{Mathf.Floor(player.GetHealth())}/{Mathf.Floor(player.GetMaxHealth())}";

        if (manaText != null)
            manaText.text = $"{Mathf.Floor(player.GetMana())}/{Mathf.Floor(player.GetMaxMana())}";

        if (xpText != null)
            xpText.text = $"{Mathf.Floor(player.GetXP())}/{Mathf.Floor(player.GetXPToLevel())}";

        if (levelText != null)
            levelText.text = $"Lv.{player.GetLevel()}";

        if (goldText != null)
            goldText.text = $"💰 {player.GetGold()}";
    }

    void UpdateSkillCooldowns()
    {
        if (player == null) return;

        // Update based on player skill cooldowns
        // This would be connected to PlayerController's skill system
    }

    public void ShowDeathScreen()
    {
        if (deathScreen != null)
        {
            deathScreen.SetActive(true);
        }
    }

    public void HideDeathScreen()
    {
        if (deathScreen != null)
        {
            deathScreen.SetActive(false);
        }
    }

    public void RespawnPlayer()
    {
        HideDeathScreen();
        
        if (player != null)
        {
            player.Heal(player.GetMaxHealth() / 2);
            player.transform.position = Vector3.zero;
        }
    }

    public void ShowLevelUpEffect()
    {
        if (levelUpEffectPrefab != null)
        {
            Vector3 pos = levelUpEffectPosition != null ? levelUpEffectPosition.position : player.transform.position + Vector3.up * 2;
            Instantiate(levelUpEffectPrefab, pos, Quaternion.identity);
        }

        ShowNotification("🎉 SEVİYE ATLADI!", NotificationType.LevelUp);
    }

    public void ShowNotification(string message, NotificationType type = NotificationType.Info)
    {
        if (notificationPrefab == null) return;

        GameObject notification = Instantiate(notificationPrefab, notificationContainer);
        NotificationUI notificationUI = notification.GetComponent<NotificationUI>();
        
        if (notificationUI != null)
        {
            notificationUI.Setup(message, type);
        }

        Destroy(notification, 3f);
    }

    public void ShowItemPickup(ItemData item)
    {
        if (itemPickupNotification != null)
        {
            if (itemPickupName != null && item != null)
            {
                itemPickupName.text = $"🎁 {item.itemName}";
            }

            itemPickupNotification.SetActive(true);
            Invoke("HideItemPickup", 2f);
        }
    }

    void HideItemPickup()
    {
        if (itemPickupNotification != null)
        {
            itemPickupNotification.SetActive(false);
        }
    }

    public void ToggleInventory()
    {
        if (inventoryPanel != null)
        {
            bool isOpen = inventoryPanel.activeSelf;
            inventoryPanel.SetActive(!isOpen);
            
            if (!isOpen)
            {
                UpdateInventoryUI();
            }
        }
    }

    void UpdateInventoryUI()
    {
        if (inventoryGrid == null || inventorySlotPrefab == null) return;

        // Clear existing slots
        foreach (Transform child in inventoryGrid)
        {
            Destroy(child.gameObject);
        }

        // Create slots for each inventory item
        if (GameManager.Instance != null && GameManager.Instance.currentGame != null)
        {
            ItemData[] items = GameManager.Instance.currentGame.playerData.inventory.items;
            
            for (int i = 0; i < items.Length; i++)
            {
                GameObject slot = Instantiate(inventorySlotPrefab, inventoryGrid);
                InventorySlotUI slotUI = slot.GetComponent<InventorySlotUI>();
                
                if (slotUI != null)
                {
                    slotUI.Setup(items[i], i);
                }
            }
        }
    }

    public void OpenPauseMenu()
    {
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void ClosePauseMenu()
    {
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}

public enum NotificationType
{
    Info,
    Warning,
    Error,
    Success,
    LevelUp,
    Item
}

public class NotificationUI : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public Image backgroundImage;

    public void Setup(string message, NotificationType type)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }

        if (backgroundImage != null)
        {
            switch (type)
            {
                case NotificationType.LevelUp:
                    backgroundImage.color = new Color(1f, 0.84f, 0f); // Gold
                    break;
                case NotificationType.Item:
                    backgroundImage.color = new Color(0.6f, 0.3f, 0.8f); // Purple
                    break;
                case NotificationType.Success:
                    backgroundImage.color = new Color(0.2f, 0.8f, 0.2f); // Green
                    break;
                case NotificationType.Error:
                    backgroundImage.color = new Color(0.9f, 0.2f, 0.2f); // Red
                    break;
                default:
                    backgroundImage.color = new Color(0.2f, 0.4f, 0.8f); // Blue
                    break;
            }
        }
    }
}

public class InventorySlotUI : MonoBehaviour
{
    public Image itemIcon;
    public TextMeshProUGUI itemCount;
    public TextMeshProUGUI slotIndex;
    public Button slotButton;

    private int slotIndexValue;
    private ItemData currentItem;

    public void Setup(ItemData item, int index)
    {
        slotIndexValue = index;
        currentItem = item;

        if (slotIndex != null)
        {
            slotIndex.text = (index + 1).ToString();
        }

        if (item != null)
        {
            // itemIcon.sprite = item.icon;
            if (itemCount != null)
            {
                itemCount.text = item.count > 1 ? item.count.ToString() : "";
            }
        }
        else
        {
            itemIcon.sprite = null;
            itemIcon.color = Color.clear;
            if (itemCount != null)
            {
                itemCount.text = "";
            }
        }

        if (slotButton != null)
        {
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(OnSlotClicked);
        }
    }

    void OnSlotClicked()
    {
        if (currentItem != null)
        {
            InventoryManager.Instance.UseItem(slotIndexValue);
            UIManager.Instance.ToggleInventory();
            UIManager.Instance.ToggleInventory();
        }
    }
}