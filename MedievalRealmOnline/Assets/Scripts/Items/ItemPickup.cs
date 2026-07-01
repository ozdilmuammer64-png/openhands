using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemPickup : MonoBehaviour
{
    public ItemData itemData;
    public float pickupRange = 2f;

    [Header("UI")]
    public GameObject promptUI;
    public TextMeshProUGUI itemNameText;
    public Image itemIcon;

    private bool canPickup;

    void Start()
    {
        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }
    }

    void Update()
    {
        // Check distance to player
        if (GameManager.Instance != null && GameManager.Instance.currentGame != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                canPickup = distance <= pickupRange;

                if (promptUI != null)
                {
                    promptUI.SetActive(canPickup);
                    
                    // Face camera
                    promptUI.transform.LookAt(Camera.main.transform);
                }

                if (canPickup && Input.GetKeyDown(KeyCode.E))
                {
                    PickupItem(player.GetComponent<PlayerController>());
                }
            }
        }
    }

    void PickupItem(PlayerController player)
    {
        if (itemData == null) return;

        // Add to inventory
        bool added = InventoryManager.Instance.AddItem(itemData);

        if (added)
        {
            // Show pickup notification
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowItemPickup(itemData);
            }

            Destroy(gameObject);
        }
    }

    public void SetItem(ItemData data)
    {
        itemData = data;
        
        if (itemNameText != null && data != null)
        {
            itemNameText.text = data.itemName;
        }

        if (itemIcon != null && data != null)
        {
            // itemIcon.sprite = data.icon;
        }
    }
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public int maxInventorySlots = 24;
    private ItemData[] inventory;
    private GameObject[] inventoryUI;

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

        inventory = new ItemData[maxInventorySlots];
    }

    void Start()
    {
        // Load inventory from save
        if (GameManager.Instance != null && GameManager.Instance.currentGame != null)
        {
            inventory = GameManager.Instance.currentGame.playerData.inventory.items;
        }
    }

    public bool AddItem(ItemData item)
    {
        // Find empty slot or stack
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null)
            {
                inventory[i] = item;
                UpdateUI();
                SaveInventory();
                return true;
            }
        }

        Debug.Log("Inventory full!");
        return false;
    }

    public bool RemoveItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventory.Length) return false;
        
        if (inventory[slotIndex] != null)
        {
            inventory[slotIndex] = null;
            UpdateUI();
            SaveInventory();
            return true;
        }
        return false;
    }

    public bool UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventory.Length) return false;
        
        ItemData item = inventory[slotIndex];
        if (item == null) return false;

        // Check if consumable
        if (item.itemId.Contains("potion"))
        {
            PlayerController player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
            if (player != null)
            {
                if (item.itemId.Contains("health"))
                {
                    player.Heal(50);
                }
                else if (item.itemId.Contains("mana"))
                {
                    player.RestoreMana(30);
                }

                item.count--;
                if (item.count <= 0)
                {
                    RemoveItem(slotIndex);
                }
                else
                {
                    SaveInventory();
                }
                return true;
            }
        }

        return false;
    }

    public ItemData GetItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventory.Length) return null;
        return inventory[slotIndex];
    }

    void UpdateUI()
    {
        if (inventoryUI == null || inventoryUI.Length == 0) return;

        for (int i = 0; i < Mathf.Min(inventory.Length, inventoryUI.Length); i++)
        {
            if (inventoryUI[i] != null && inventory[i] != null)
            {
                // Update slot UI
                // inventoryUI[i].GetComponent<InventorySlot>().SetItem(inventory[i]);
            }
        }
    }

    void SaveInventory()
    {
        if (GameManager.Instance != null && GameManager.Instance.currentGame != null)
        {
            GameManager.Instance.currentGame.playerData.inventory.items = inventory;
        }
    }
}