using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCreator : MonoBehaviour
{
    [Header("UI References")]
    public GameObject creationPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI statPointsText;

    [Header("Race Selection")]
    public GameObject racePanel;
    public Button[] raceButtons;

    [Header("Class Selection")]
    public GameObject classPanel;
    public Button[] classButtons;

    [Header("Stats Panel")]
    public TextMeshProUGUI strValueText;
    public TextMeshProUGUI dexValueText;
    public TextMeshProUGUI intValueText;
    public TextMeshProUGUI vitValueText;
    public TextMeshProUGUI strBonusText;
    public TextMeshProUGUI dexBonusText;
    public TextMeshProUGUI intBonusText;
    public TextMeshProUGUI vitBonusText;

    [Header("Name Input")]
    public TMP_InputField nameInput;

    [Header("Create Button")]
    public Button createButton;

    [Header("Race/Class Info")]
    public TextMeshProUGUI selectedRaceText;
    public TextMeshProUGUI selectedClassText;
    public TextMeshProUGUI classDescriptionText;

    // Selection
    private string selectedRace = "Human";
    private string selectedClass = "Warrior";
    private int availablePoints = 10;

    private int strBase = 10;
    private int dexBase = 10;
    private int intBase = 10;
    private int vitBase = 10;

    // Race bonuses
    private readonly int[] raceStrBonus = { 0, 3, -1, 1 };    // Human, Orc, Elf, Demon
    private readonly int[] raceDexBonus = { 0, -1, 3, 1 };
    private readonly int[] raceIntBonus = { 0, -1, 1, 3 };
    private readonly int[] raceVitBonus = { 0, 2, -1, -2 };

    // Class data
    private readonly string[] classDescriptions = {
        "Yakın dövüş ustası. Yüksek hasar, orta dayanıklılık.",
        "Uzakdan büyü saldırıları. Düşük dayanıklılık, yüksek büyü gücü.",
        "Hızlı ve gizli. Orta hasar, yüksek çeviklik.",
        "Tank ve şifacı. Yüksek dayanıklılık, orta hasar."
    };

    void Start()
    {
        SetupButtons();
        UpdateUI();
    }

    void SetupButtons()
    {
        // Race buttons
        for (int i = 0; i < raceButtons.Length; i++)
        {
            int index = i;
            raceButtons[i].onClick.AddListener(() => SelectRace(index));
        }

        // Class buttons
        for (int i = 0; i < classButtons.Length; i++)
        {
            int index = i;
            classButtons[i].onClick.AddListener(() => SelectClass(index));
        }

        // Stat buttons
        createButton.onClick.AddListener(CreateCharacter);

        // Name input listener
        nameInput.onValueChanged.AddListener(delegate { ValidateInput(); });
    }

    void SelectRace(int index)
    {
        string[] races = { "Human", "Orc", "Elf", "Demon" };
        selectedRace = races[index];
        UpdateUI();
    }

    void SelectClass(int index)
    {
        string[] classes = { "Warrior", "Mage", "Rogue", "Paladin" };
        selectedClass = classes[index];
        UpdateUI();
    }

    public void IncreaseStat(string stat)
    {
        if (availablePoints <= 0) return;

        int total = strBase + dexBase + intBase + vitBase;
        if (total >= 40) return;

        switch (stat)
        {
            case "Str": strBase++; break;
            case "Dex": dexBase++; break;
            case "Int": intBase++; break;
            case "Vit": vitBase++; break;
        }

        availablePoints--;
        UpdateUI();
    }

    public void DecreaseStat(string stat)
    {
        int minValue = 1;

        switch (stat)
        {
            case "Str":
                if (strBase > minValue) { strBase--; availablePoints++; }
                break;
            case "Dex":
                if (dexBase > minValue) { dexBase--; availablePoints++; }
                break;
            case "Int":
                if (intBase > minValue) { intBase--; availablePoints++; }
                break;
            case "Vit":
                if (vitBase > minValue) { vitBase--; availablePoints++; }
                break;
        }

        UpdateUI();
    }

    void ValidateInput()
    {
        string name = nameInput.text.Trim();
        createButton.interactable = (name.Length >= 2 && name.Length <= 15);
    }

    void UpdateUI()
    {
        // Stats
        int raceIndex = GetRaceIndex();
        
        strValueText.text = (strBase + raceStrBonus[raceIndex]).ToString();
        dexValueText.text = (dexBase + raceDexBonus[raceIndex]).ToString();
        intValueText.text = (intBase + raceIntBonus[raceIndex]).ToString();
        vitValueText.text = (vitBase + raceVitBonus[raceIndex]).ToString();

        // Bonuses
        strBonusText.text = raceStrBonus[raceIndex] >= 0 ? $"+{raceStrBonus[raceIndex]}" : raceStrBonus[raceIndex].ToString();
        dexBonusText.text = raceDexBonus[raceIndex] >= 0 ? $"+{raceDexBonus[raceIndex]}" : raceDexBonus[raceIndex].ToString();
        intBonusText.text = raceIntBonus[raceIndex] >= 0 ? $"+{raceIntBonus[raceIndex]}" : raceIntBonus[raceIndex].ToString();
        vitBonusText.text = raceVitBonus[raceIndex] >= 0 ? $"+{raceVitBonus[raceIndex]}" : raceVitBonus[raceIndex].ToString();

        // Points
        statPointsText.text = $"Kalan Puan: {availablePoints}";

        // Selection
        selectedRaceText.text = $"Irk: {selectedRace}";
        selectedClassText.text = $"Sınıf: {selectedClass}";
        classDescriptionText.text = classDescriptions[GetClassIndex()];
    }

    int GetRaceIndex()
    {
        switch (selectedRace)
        {
            case "Orc": return 1;
            case "Elf": return 2;
            case "Demon": return 3;
            default: return 0;
        }
    }

    int GetClassIndex()
    {
        switch (selectedClass)
        {
            case "Mage": return 1;
            case "Rogue": return 2;
            case "Paladin": return 3;
            default: return 0;
        }
    }

    void CreateCharacter()
    {
        string playerName = nameInput.text.Trim();
        if (playerName.Length < 2) return;

        // Create new game data
        GameData newGame = new GameData();
        PlayerData pd = newGame.playerData;

        int raceIndex = GetRaceIndex();
        int classIndex = GetClassIndex();

        // Set basic info
        pd.playerName = playerName;
        pd.race = selectedRace;
        pd.classType = selectedClass;
        pd.level = 1;
        pd.xp = 0;
        pd.xpToLevel = 100;
        pd.gold = 100;

        // Set stats with race bonuses
        pd.strength = strBase + raceStrBonus[raceIndex];
        pd.dexterity = dexBase + raceDexBonus[raceIndex];
        pd.intelligence = intBase + raceIntBonus[raceIndex];
        pd.vitality = vitBase + raceVitBonus[raceIndex];

        // Calculate derived stats based on class
        CalculateClassStats(pd, classIndex);

        // Set spawn position
        pd.position = new Vector3(0, 1, 0);

        // Give starting items
        GiveStartingItems(pd);

        // Set current game
        GameManager.Instance.currentGame = newGame;
        GameManager.Instance.StartNewGame();
    }

    void CalculateClassStats(PlayerData pd, int classIndex)
    {
        switch (classIndex)
        {
            case 0: // Warrior
                pd.maxHealth = 150 + pd.vitality * 10;
                pd.maxMana = 30 + pd.intelligence * 3;
                pd.attackDamage = 15 + pd.strength * 2;
                pd.magicPower = 5 + pd.intelligence;
                break;

            case 1: // Mage
                pd.maxHealth = 80 + pd.vitality * 10;
                pd.maxMana = 100 + pd.intelligence * 3;
                pd.attackDamage = 5 + pd.strength;
                pd.magicPower = 25 + pd.intelligence * 2;
                break;

            case 2: // Rogue
                pd.maxHealth = 100 + pd.vitality * 10;
                pd.maxMana = 40 + pd.intelligence * 3;
                pd.attackDamage = 20 + pd.strength * 2;
                pd.magicPower = 8 + pd.intelligence;
                break;

            case 3: // Paladin
                pd.maxHealth = 130 + pd.vitality * 10;
                pd.maxMana = 60 + pd.intelligence * 3;
                pd.attackDamage = 12 + pd.strength * 2;
                pd.magicPower = 12 + pd.intelligence * 2;
                break;
        }

        pd.health = pd.maxHealth;
        pd.mana = pd.maxMana;
    }

    void GiveStartingItems(PlayerData pd)
    {
        // Health potion
        pd.inventory.items[0] = new ItemData
        {
            itemId = "potion_health",
            itemName = "Can İksiri",
            icon = "❤️",
            count = 3
        };

        // Mana potion
        pd.inventory.items[1] = new ItemData
        {
            itemId = "potion_mana",
            itemName = "Mana İksiri",
            icon = "💙",
            count = 3
        };
    }
}