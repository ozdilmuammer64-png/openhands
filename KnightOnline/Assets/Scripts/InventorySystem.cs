using UnityEngine;
using System.Collections.Generic;

namespace KnightOnline.Scripts
{
    public enum ItemType { Weapon, Armor, Helmet, Shield, Boots, Gloves, Belt, Ring, Amulet, Potion, Scroll, Quest, Material, Gem }
    public enum ItemRarity { Common, Uncommon, Rare, Epic, Legendary }
    public enum EquipmentSlot { None, Weapon, Armor, Helmet, Shield, Boots, Gloves, Belt, Ring1, Ring2, Amulet, LeftRing, RightRing }
    
    [CreateAssetMenu(fileName = "New Item", menuName = "Knight Online/Item")]
    public class ItemData : ScriptableObject
    {
        public string itemName = "New Item";
        public string description = "Item description";
        public Sprite icon;
        public ItemType itemType = ItemType.Material;
        public ItemRarity rarity = ItemRarity.Common;
        
        [Header("Stack")]
        public bool stackable = false;
        public int maxStack = 99;
        
        [Header("Stats")]
        public int strengthBonus;
        public int dexterityBonus;
        public int intelligenceBonus;
        public int constitutionBonus;
        public float healthBonus;
        public float manaBonus;
        public int minDamageBonus;
        public int maxDamageBonus;
        public float armorBonus;
        public float attackSpeedBonus;
        public float castSpeedBonus;
        public float criticalChanceBonus;
        public float blockChanceBonus;
        public float dodgeChanceBonus;
        
        [Header("Combat")]
        public float damageBonusPercent;
        public float defenseBonusPercent;
        public float xpBonusPercent;
        public float goldBonusPercent;
        
        [Header("Usage")]
        public bool usable = false;
        public float healAmount;
        public float manaRestoreAmount;
        public float buffDuration;
        public string buffName;
        
        [Header("Requirements")]
        public int requiredLevel = 1;
        public PlayerClass requiredClass = PlayerClass.Warrior;
        public int requiredStrength;
        public int requiredDexterity;
        public int requiredIntelligence;
        
        [Header("Value")]
        public int buyPrice;
        public int sellPrice;
        
        [Header("Visual")]
        public GameObject worldModel;
        public Color rarityColor = Color.white;
        
        public string RarityColorHex
        {
            get
            {
                return rarity switch
                {
                    ItemRarity.Common => "#FFFFFF",
                    ItemRarity.Uncommon => "#00FF00",
                    ItemRarity.Rare => "#0077FF",
                    ItemRarity.Epic => "#9900FF",
                    ItemRarity.Legendary => "#FF8000",
                    _ => "#FFFFFF"
                };
            }
        }
    }
    
    public class InventoryItem
    {
        public ItemData data;
        public int quantity;
        public int slotIndex;
        public string uniqueId;
        
        public InventoryItem(ItemData itemData, int qty = 1)
        {
            data = itemData;
            quantity = qty;
            uniqueId = System.Guid.NewGuid().ToString();
        }
    }
    
    public class InventorySystem : MonoBehaviour
    {
        public static InventorySystem Instance { get; private set; }
        
        [Header("Inventory Settings")]
        public int inventorySize = 24;
        public int equipmentSlots = 12;
        public int bankSize = 100;
        
        [Header("Current Items")]
        public List<InventoryItem> inventory = new List<InventoryItem>();
        public List<InventoryItem> equipment = new List<InventoryItem>();
        public List<InventoryItem> bank = new List<InventoryItem>();
        
        [Header("Components")]
        public PlayerController player;
        
        // Events
        public System.Action OnInventoryChanged;
        public System.Action<InventoryItem> OnItemAdded;
        public System.Action<InventoryItem> OnItemRemoved;
        public System.Action<InventoryItem> OnItemEquipped;
        public System.Action<InventoryItem> OnItemUnequipped;
        
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
            if (player == null)
            {
                player = FindObjectOfType<PlayerController>();
            }
            
            InitializeEquipmentSlots();
            InitializeInventory();
        }
        
        void InitializeInventory()
        {
            for (int i = 0; i < inventorySize; i++)
            {
                inventory.Add(null);
            }
        }
        
        void InitializeEquipmentSlots()
        {
            for (int i = 0; i < equipmentSlots; i++)
            {
                equipment.Add(null);
            }
        }
        
        public bool AddItem(ItemData itemData, int quantity = 1)
        {
            if (itemData == null) return false;
            
            // Stack if stackable
            if (itemData.stackable)
            {
                int remainingQty = quantity;
                
                // Find existing stacks
                foreach (InventoryItem existing in inventory)
                {
                    if (existing != null && existing.data == itemData && existing.quantity < itemData.maxStack)
                    {
                        int spaceInStack = itemData.maxStack - existing.quantity;
                        int toAdd = Mathf.Min(spaceInStack, remainingQty);
                        existing.quantity += toAdd;
                        remainingQty -= toAdd;
                        
                        if (remainingQty <= 0)
                        {
                            OnInventoryChanged?.Invoke();
                            OnItemAdded?.Invoke(existing);
                            return true;
                        }
                    }
                }
                
                // Add new stacks
                while (remainingQty > 0)
                {
                    int slot = FindEmptySlot();
                    if (slot == -1)
                    {
                        Debug.Log("Inventory full!");
                        return false;
                    }
                    
                    int stackQty = Mathf.Min(remainingQty, itemData.maxStack);
                    InventoryItem newItem = new InventoryItem(itemData, stackQty);
                    newItem.slotIndex = slot;
                    inventory[slot] = newItem;
                    remainingQty -= stackQty;
                    
                    OnItemAdded?.Invoke(newItem);
                }
            }
            else
            {
                // Non-stackable items
                int slot = FindEmptySlot();
                if (slot == -1)
                {
                    Debug.Log("Inventory full!");
                    return false;
                }
                
                for (int i = 0; i < quantity; i++)
                {
                    slot = FindEmptySlot();
                    if (slot == -1)
                    {
                        Debug.Log("Inventory full!");
                        return false;
                    }
                    
                    InventoryItem newItem = new InventoryItem(itemData, 1);
                    newItem.slotIndex = slot;
                    inventory[slot] = newItem;
                    OnItemAdded?.Invoke(newItem);
                }
            }
            
            OnInventoryChanged?.Invoke();
            return true;
        }
        
        public bool RemoveItem(int slotIndex, int quantity = 1)
        {
            if (slotIndex < 0 || slotIndex >= inventory.Count) return false;
            
            InventoryItem item = inventory[slotIndex];
            if (item == null) return false;
            
            if (item.quantity <= quantity)
            {
                inventory[slotIndex] = null;
                OnItemRemoved?.Invoke(item);
                OnInventoryChanged?.Invoke();
                return true;
            }
            else
            {
                item.quantity -= quantity;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }
        
        public bool RemoveItem(InventoryItem item, int quantity = 1)
        {
            return RemoveItem(item.slotIndex, quantity);
        }
        
        int FindEmptySlot()
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }
        
        public InventoryItem GetItem(int slotIndex)
        {
            if (slotIndex >= 0 && slotIndex < inventory.Count)
            {
                return inventory[slotIndex];
            }
            return null;
        }
        
        public InventoryItem GetEquipment(EquipmentSlot slot)
        {
            int index = (int)slot - 1;
            if (index >= 0 && index < equipment.Count)
            {
                return equipment[index];
            }
            return null;
        }
        
        public bool EquipItem(int inventorySlot)
        {
            InventoryItem item = GetItem(inventorySlot);
            if (item == null) return false;
            
            if (!CanEquip(item.data)) return false;
            
            EquipmentSlot targetSlot = GetEquipmentSlotForItem(item.data);
            int targetIndex = (int)targetSlot - 1;
            
            // Unequip current item
            InventoryItem currentlyEquipped = equipment[targetIndex];
            if (currentlyEquipped != null)
            {
                UnequipItem(targetSlot);
            }
            
            // Equip new item
            equipment[targetIndex] = item;
            inventory[inventorySlot] = null;
            item.slotIndex = targetIndex;
            
            // Apply stats
            ApplyItemStats(item.data, true);
            
            OnItemEquipped?.Invoke(item);
            OnInventoryChanged?.Invoke();
            
            return true;
        }
        
        public bool UnequipItem(EquipmentSlot slot)
        {
            int index = (int)slot - 1;
            if (index < 0 || index >= equipment.Count) return false;
            
            InventoryItem item = equipment[index];
            if (item == null) return false;
            
            // Check inventory space
            if (FindEmptySlot() == -1)
            {
                Debug.Log("Inventory full!");
                return false;
            }
            
            // Remove stats
            ApplyItemStats(item.data, false);
            
            // Move to inventory
            equipment[index] = null;
            AddItem(item.data, item.quantity);
            
            OnItemUnequipped?.Invoke(item);
            OnInventoryChanged?.Invoke();
            
            return true;
        }
        
        bool CanEquip(ItemData item)
        {
            if (player == null) return false;
            
            if (player.level < item.requiredLevel)
            {
                Debug.Log($"Requires level {item.requiredLevel}");
                return false;
            }
            
            if (item.requiredClass != PlayerClass.Warrior && item.requiredClass != player.playerClass)
            {
                Debug.Log($"Class {item.requiredClass} required");
                return false;
            }
            
            if (player.strength < item.requiredStrength)
            {
                Debug.Log($"Requires {item.requiredStrength} Strength");
                return false;
            }
            
            if (player.dexterity < item.requiredDexterity)
            {
                Debug.Log($"Requires {item.requiredDexterity} Dexterity");
                return false;
            }
            
            if (player.intelligence < item.requiredIntelligence)
            {
                Debug.Log($"Requires {item.requiredIntelligence} Intelligence");
                return false;
            }
            
            return true;
        }
        
        EquipmentSlot GetEquipmentSlotForItem(ItemData item)
        {
            return item.itemType switch
            {
                ItemType.Weapon => EquipmentSlot.Weapon,
                ItemType.Armor => EquipmentSlot.Armor,
                ItemType.Helmet => EquipmentSlot.Helmet,
                ItemType.Shield => EquipmentSlot.Shield,
                ItemType.Boots => EquipmentSlot.Boots,
                ItemType.Gloves => EquipmentSlot.Gloves,
                ItemType.Belt => EquipmentSlot.Belt,
                ItemType.Ring => EquipmentSlot.Ring1,
                ItemType.Amulet => EquipmentSlot.Amulet,
                _ => EquipmentSlot.None
            };
        }
        
        void ApplyItemStats(ItemData item, bool equip)
        {
            int multiplier = equip ? 1 : -1;
            
            player.strength += item.strengthBonus * multiplier;
            player.dexterity += item.dexterityBonus * multiplier;
            player.intelligence += item.intelligenceBonus * multiplier;
            player.constitution += item.constitutionBonus * multiplier;
            
            player.maxHealth += item.healthBonus * multiplier;
            player.maxMana += item.manaBonus * multiplier;
            
            player.minDamage += item.minDamageBonus * multiplier;
            player.maxDamage += item.maxDamageBonus * multiplier;
            player.armor += item.armorBonus * multiplier;
            
            player.attackSpeed += item.attackSpeedBonus * multiplier;
            player.castSpeed += item.castSpeedBonus * multiplier;
            player.criticalChance += item.criticalChanceBonus * multiplier;
            player.blockChance += item.blockChanceBonus * multiplier;
            player.dodgeChance += item.dodgeChanceBonus * multiplier;
        }
        
        public bool UseItem(int slotIndex)
        {
            InventoryItem item = GetItem(slotIndex);
            if (item == null || !item.data.usable) return false;
            
            // Apply item effects
            if (item.data.healAmount > 0)
            {
                player.Heal(item.data.healAmount);
            }
            
            if (item.data.manaRestoreAmount > 0)
            {
                player.RestoreMana(item.data.manaRestoreAmount);
            }
            
            // Remove from inventory
            RemoveItem(slotIndex, 1);
            
            return true;
        }
        
        public bool SellItem(int slotIndex)
        {
            InventoryItem item = GetItem(slotIndex);
            if (item == null) return false;
            
            int sellValue = item.data.sellPrice * item.quantity;
            player.AddGold(sellValue);
            RemoveItem(slotIndex, item.quantity);
            
            return true;
        }
        
        public int GetTotalItemCount(ItemData itemData)
        {
            int count = 0;
            foreach (InventoryItem item in inventory)
            {
                if (item != null && item.data == itemData)
                {
                    count += item.quantity;
                }
            }
            return count;
        }
        
        public void SortInventory()
        {
            List<InventoryItem> items = new List<InventoryItem>();
            
            foreach (InventoryItem item in inventory)
            {
                if (item != null)
                {
                    items.Add(item);
                }
            }
            
            items.Sort((a, b) => 
            {
                // Sort by type first
                int typeCompare = ((int)a.data.itemType).CompareTo((int)b.data.itemType);
                if (typeCompare != 0) return typeCompare;
                
                // Then by rarity
                int rarityCompare = ((int)b.data.rarity).CompareTo((int)a.data.rarity);
                if (rarityCompare != 0) return rarityCompare;
                
                // Then by name
                return a.data.itemName.CompareTo(b.data.itemName);
            });
            
            for (int i = 0; i < inventory.Count; i++)
            {
                inventory[i] = null;
            }
            
            for (int i = 0; i < items.Count; i++)
            {
                inventory[i] = items[i];
                items[i].slotIndex = i;
            }
            
            OnInventoryChanged?.Invoke();
        }
    }
}
