using UnityEngine;
using System;

public class LootManager : MonoBehaviour
{
    public static LootManager Instance { get; private set; }

    [Header("Loot Tables")]
    public LootDrop[] commonLoot;
    public LootDrop[] rareLoot;
    public LootDrop[] epicLoot;
    public LootDrop[] legendaryLoot;

    [Header("Drop Rates")]
    [Range(0, 100)]
    public float commonDropRate = 60f;
    [Range(0, 100)]
    public float rareDropRate = 25f;
    [Range(0, 100)]
    public float epicDropRate = 12f;
    [Range(0, 100)]
    public float legendaryDropRate = 3f;

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

    public GameObject GetRandomLoot()
    {
        // Determine rarity
        float roll = UnityEngine.Random.Range(0f, 100f);
        LootDrop[] selectedLoot = null;

        if (roll < legendaryDropRate && legendaryLoot.Length > 0)
        {
            selectedLoot = legendaryLoot;
        }
        else if (roll < legendaryDropRate + epicDropRate && epicLoot.Length > 0)
        {
            selectedLoot = epicLoot;
        }
        else if (roll < legendaryDropRate + epicDropRate + rareDropRate && rareLoot.Length > 0)
        {
            selectedLoot = rareLoot;
        }
        else
        {
            selectedLoot = commonLoot;
        }

        if (selectedLoot == null || selectedLoot.Length == 0)
        {
            selectedLoot = commonLoot;
        }

        LootDrop drop = selectedLoot[UnityEngine.Random.Range(0, selectedLoot.Length)];
        
        if (drop.lootPrefab != null)
        {
            return drop.lootPrefab;
        }

        return null;
    }

    public ItemData GenerateRandomItem()
    {
        ItemData item = new ItemData();
        
        float roll = UnityEngine.Random.Range(0f, 100f);
        string quality = "Common";

        if (roll < legendaryDropRate)
        {
            quality = "Legendary";
            item.attackBonus = UnityEngine.Random.Range(25, 40);
            item.magicBonus = UnityEngine.Random.Range(20, 35);
        }
        else if (roll < legendaryDropRate + epicDropRate)
        {
            quality = "Epic";
            item.attackBonus = UnityEngine.Random.Range(15, 25);
            item.magicBonus = UnityEngine.Random.Range(10, 20);
        }
        else if (roll < legendaryDropRate + epicDropRate + rareDropRate)
        {
            quality = "Rare";
            item.attackBonus = UnityEngine.Random.Range(8, 15);
            item.magicBonus = UnityEngine.Random.Range(5, 12);
        }
        else
        {
            quality = "Common";
            item.attackBonus = UnityEngine.Random.Range(3, 8);
            item.magicBonus = UnityEngine.Random.Range(2, 6);
        }

        item.quality = quality;
        
        string[] itemTypes = { "Sword", "Staff", "Helmet", "Armor", "Boots", "Ring" };
        item.itemId = itemTypes[UnityEngine.Random.Range(0, itemTypes.Length)];
        item.itemName = item.itemId;
        item.slot = item.itemId.ToLower();

        return item;
    }
}

[System.Serializable]
public class LootDrop
{
    public string itemName;
    public GameObject lootPrefab;
    public Sprite icon;
    public int dropWeight = 1;
}