using UnityEngine;
using System;

[Serializable]
public class GameData
{
    public PlayerData playerData;
    public string currentArea;
    public DateTime saveTime;

    public GameData()
    {
        playerData = new PlayerData();
        currentArea = "Village";
        saveTime = DateTime.Now;
    }
}

[Serializable]
public class PlayerData
{
    public string playerName;
    public string race;
    public string classType;
    public int level;
    public float xp;
    public float xpToLevel;
    public float health;
    public float maxHealth;
    public float mana;
    public float maxMana;
    public float attackDamage;
    public float magicPower;
    public int strength;
    public int dexterity;
    public int intelligence;
    public int vitality;
    public int gold;
    public Vector3 position;
    public InventoryData inventory;
    public EquipmentData equipment;

    public PlayerData()
    {
        playerName = "Player";
        race = "Human";
        classType = "Warrior";
        level = 1;
        xp = 0;
        xpToLevel = 100;
        maxHealth = 100;
        health = 100;
        maxMana = 50;
        mana = 50;
        attackDamage = 10;
        magicPower = 5;
        strength = 10;
        dexterity = 10;
        intelligence = 10;
        vitality = 10;
        gold = 100;
        position = new Vector3(0, 0, 0);
        inventory = new InventoryData();
        equipment = new EquipmentData();
    }
}

[Serializable]
public class InventoryData
{
    public ItemData[] items;
    public InventoryData()
    {
        items = new ItemData[24];
    }
}

[Serializable]
public class EquipmentData
{
    public ItemData head;
    public ItemData body;
    public ItemData feet;
    public ItemData weapon;
    public ItemData ring;

    public EquipmentData()
    {
        head = null;
        body = null;
        feet = null;
        weapon = null;
        ring = null;
    }
}

[Serializable]
public class ItemData
{
    public string itemId;
    public string itemName;
    public string icon;
    public string slot;
    public string quality;
    public int count;
    public float healthBonus;
    public float manaBonus;
    public float attackBonus;
    public float magicBonus;
    public float strengthBonus;
    public float dexterityBonus;

    public ItemData()
    {
        count = 1;
        quality = "Normal";
    }
}

[Serializable]
public class EnemyData
{
    public string enemyType;
    public string enemyName;
    public float health;
    public float maxHealth;
    public float attackDamage;
    public int xpReward;
    public int goldReward;
    public Vector3 position;
}

[CreateAssetMenu(fileName = "GameSettings", menuName = "MedievalRealm/GameSettings")]
public class GameSettings : ScriptableObject
{
    public int maxLevel = 100;
    public float baseXpToLevel = 100f;
    public float xpMultiplier = 1.2f;
    public float baseHealthPerVitality = 10f;
    public float baseManaPerIntelligence = 3f;
    public float baseAttackPerStrength = 2f;
    public float baseMagicPerIntelligence = 2f;
    public float critChancePerDexterity = 0.5f;
    public float moveSpeedPerDexterity = 0.1f;
}