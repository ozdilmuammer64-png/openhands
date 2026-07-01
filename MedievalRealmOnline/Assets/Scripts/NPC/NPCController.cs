using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NPCController : MonoBehaviour
{
    [Header("NPC Info")]
    public string npcName = "NPC";
    public string npcTitle = "";
    public Sprite npcPortrait;

    [Header("Dialog")]
    public DialogData[] dialogs;

    [Header("Shop")]
    public bool hasShop = false;
    public ShopData shopData;

    [Header("Quest")]
    public bool hasQuest = false;
    public QuestData questData;

    [Header("Healer")]
    public bool isHealer = false;
    public int healCost = 20;

    [Header("Components")]
    private Animator animator;
    private NavMeshAgent agent;
    private Transform player;

    [Header("UI")]
    public GameObject interactPrompt;
    public GameObject dialogPanel;

    private bool canInteract;
    private bool isInDialog;

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }

        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }
    }

    void Update()
    {
        CheckPlayerDistance();
    }

    void CheckPlayerDistance()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            canInteract = distance <= 3f;

            if (interactPrompt != null)
            {
                interactPrompt.SetActive(canInteract && !isInDialog);
                
                // Face camera
                interactPrompt.transform.LookAt(Camera.main.transform);
            }
        }
    }

    void OnMouseDown()
    {
        if (canInteract && !isInDialog)
        {
            Interact();
        }
    }

    public void Interact()
    {
        if (isHealer)
        {
            OpenHealerDialog();
        }
        else if (hasShop)
        {
            OpenShop();
        }
        else
        {
            OpenDialog();
        }
    }

    void OpenDialog()
    {
        if (dialogs == null || dialogs.Length == 0) return;

        isInDialog = true;
        
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(true);
            // Setup dialog UI with dialogs[0]
            UIManager.Instance.ShowNotification($"Talking to {npcName}", NotificationType.Info);
        }
    }

    void OpenHealerDialog()
    {
        PlayerController playerCtrl = player?.GetComponent<PlayerController>();
        
        if (playerCtrl != null)
        {
            if (GameManager.Instance.currentGame.playerData.gold >= healCost)
            {
                GameManager.Instance.currentGame.playerData.gold -= healCost;
                playerCtrl.Heal(playerCtrl.GetMaxHealth());
                playerCtrl.RestoreMana(playerCtrl.GetMaxMana());
                
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowNotification("Tamamen iyileştirildin!", NotificationType.Success);
                }
            }
            else
            {
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowNotification("Yeterli altın yok!", NotificationType.Error);
                }
            }
        }
    }

    void OpenShop()
    {
        if (shopData == null) return;
        
        // Open shop UI
        UIManager.Instance.ShowNotification($"Opening {npcName}'s Shop", NotificationType.Info);
    }

    public void CloseDialog()
    {
        isInDialog = false;
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }
    }
}

[System.Serializable]
public class DialogData
{
    public string speakerName;
    [TextArea(2, 4)]
    public string dialogText;
    public DialogOption[] options;
}

[System.Serializable]
public class DialogOption
{
    public string optionText;
    public DialogAction action;
    public string nextDialogId;
}

[System.Serializable]
public class DialogAction
{
    public enum ActionType { None, OpenShop, GiveItem, StartQuest, EndDialog }
    public ActionType type;
    public string itemId;
    public int goldAmount;
}

[System.Serializable]
public class ShopData
{
    public string shopName;
    public ShopItem[] items;
}

[System.Serializable]
public class ShopItem
{
    public string itemId;
    public string itemName;
    public int price;
    public int stock = -1; // -1 = unlimited
}

[System.Serializable]
public class QuestData
{
    public string questId;
    public string questName;
    public string description;
    public QuestObjective[] objectives;
    public QuestReward reward;
}

[System.Serializable]
public class QuestObjective
{
    public string type; // "Kill", "Collect", "Talk", "Reach"
    public string targetId;
    public int count;
    public int currentProgress;
}

[System.Serializable]
public class QuestReward
{
    public int gold;
    public int xp;
    public string itemId;
}