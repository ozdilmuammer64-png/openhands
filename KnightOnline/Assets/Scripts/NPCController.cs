using UnityEngine;
using System.Collections.Generic;

namespace KnightOnline.Scripts
{
    public enum NPCType { Merchant, QuestGiver, Trainer, Banker, Guard, Info, Boss }
    
    [CreateAssetMenu(fileName = "New NPC", menuName = "Knight Online/NPC")]
    public class NPCData : ScriptableObject
    {
        public string npcName = "NPC Name";
        public NPCType npcType = NPCType.Info;
        public Sprite portrait;
        public Sprite npcIcon;
        
        [Header("Dialogue")]
        public List<DialogueLine> dialogueLines = new List<DialogueLine>();
        public List<DialogueOption> dialogueOptions = new List<DialogueOption>();
        
        [Header("Services")]
        public bool canTrade = false;
        public bool canRepair = false;
        public bool canRefine = false;
        public bool canCraft = false;
        public bool canIdentify = false;
        
        [Header("Shop")]
        public List<ItemData> shopItems = new List<ItemData>();
        public float sellPriceMultiplier = 0.5f;
        
        [Header("Visual")]
        public Color nameColor = Color.white;
        public bool showGreetingBubble = true;
        public GameObject[] equipmentPrefabs;
    }
    
    [System.Serializable]
    public class DialogueLine
    {
        public string text = "Hello traveler!";
        public AudioClip voiceLine;
    }
    
    [System.Serializable]
    public class DialogueOption
    {
        public string optionText = "Continue";
        public int nextLineIndex = 0;
        public bool isQuest = false;
        public QuestData linkedQuest;
        public int goldCost = 0;
        public List<ItemData> requiredItems = new List<ItemData>();
        public System.Action onSelect;
    }
    
    public class NPCController : MonoBehaviour, IInteractable
    {
        [Header("NPC Data")]
        public NPCData npcData;
        
        [Header("Components")]
        public Animator animator;
        public Collider interactionCollider;
        public Canvas interactionPrompt;
        public GameObject shopUI;
        
        [Header("Animation")]
        public bool facePlayer = true;
        public float rotationSpeed = 5f;
        
        [Header("Effects")]
        public GameObject greetingEffect;
        public bool canInteract = true;
        
        private Transform player;
        private bool isPlayerNear;
        private bool isDialogueOpen;
        private CanvasGroup promptCanvas;
        
        public System.Action<NPCData> OnInteract;
        public System.Action OnDialogueStart;
        public System.Action OnDialogueEnd;
        
        void Start()
        {
            SetupNPC();
            CreateInteractionPrompt();
        }
        
        void SetupNPC()
        {
            if (gameObject.GetComponent<Collider>() == null)
            {
                interactionCollider = gameObject.AddComponent<SphereCollider>();
                (interactionCollider as SphereCollider).radius = 1.5f;
                (interactionCollider as SphereCollider).isTrigger = true;
            }
            
            gameObject.layer = LayerMask.NameToLayer("NPC");
            
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }
        
        void CreateInteractionPrompt()
        {
            if (interactionPrompt == null)
            {
                GameObject promptObj = new GameObject("InteractionPrompt");
                promptObj.transform.parent = transform;
                promptObj.transform.localPosition = Vector3.up * 2.5f;
                
                Canvas canvas = promptObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldToScreenPoint;
                canvas.worldCamera = Camera.main;
                
                promptObj.AddComponent<UnityEngine.UI.CanvasScaler>();
                promptObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                
                GameObject textObj = new GameObject("PromptText");
                textObj.transform.parent = promptObj.transform;
                
                UnityEngine.UI.Text text = textObj.AddComponent<UnityEngine.UI.Text>();
                text.text = "Press [E] to interact";
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.fontSize = 14;
                text.alignment = TextAnchor.MiddleCenter;
                text.color = Color.yellow;
                
                RectTransform rect = textObj.GetComponent<RectTransform>();
                rect.localPosition = Vector3.zero;
                rect.sizeDelta = new Vector2(150, 30);
                
                promptCanvas = promptObj.AddComponent<CanvasGroup>();
                promptCanvas.alpha = 0;
                
                interactionPrompt = promptObj.GetComponent<Canvas>();
            }
        }
        
        void Update()
        {
            if (!canInteract) return;
            
            CheckPlayerDistance();
            HandleInteraction();
            HandleFacePlayer();
        }
        
        void CheckPlayerDistance()
        {
            if (player == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    player = playerObj.transform;
                }
            }
            
            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.position);
                isPlayerNear = distance < 3f;
                
                if (promptCanvas != null)
                {
                    promptCanvas.alpha = isPlayerNear && !isDialogueOpen ? 1 : 0;
                }
            }
        }
        
        void HandleInteraction()
        {
            if (isPlayerNear && Input.GetKeyDown(KeyCode.E) && !isDialogueOpen)
            {
                Interact();
            }
        }
        
        void HandleFacePlayer()
        {
            if (facePlayer && player != null && isPlayerNear)
            {
                Vector3 direction = player.position - transform.position;
                direction.y = 0;
                if (direction.magnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
            }
        }
        
        public void Interact()
        {
            if (!canInteract) return;
            
            isDialogueOpen = true;
            OnInteract?.Invoke(npcData);
            OnDialogueStart?.Invoke();
            
            // Play greeting animation
            animator?.SetTrigger("Greet");
            
            // Show greeting effect
            if (greetingEffect != null && npcData.showGreetingBubble)
            {
                GameObject effect = Instantiate(greetingEffect, transform.position + Vector3.up * 2.5f, Quaternion.identity);
                Destroy(effect, 2f);
            }
            
            // Open dialogue UI
            UIManager.Instance?.OpenNPCDialogue(this);
        }
        
        public void CloseDialogue()
        {
            isDialogueOpen = false;
            OnDialogueEnd?.Invoke();
        }
        
        public void OpenShop()
        {
            if (npcData.canTrade)
            {
                UIManager.Instance?.OpenShop(this);
            }
        }
        
        public void AcceptQuest(QuestData quest)
        {
            QuestSystem.Instance?.AcceptQuest(quest);
        }
        
        public void OpenRepair()
        {
            if (npcData.canRepair)
            {
                UIManager.Instance?.OpenRepair(this);
            }
        }
        
        public List<ItemData> GetShopItems()
        {
            return npcData.shopItems;
        }
        
        public int GetItemSellPrice(ItemData item)
        {
            return Mathf.RoundToInt(item.buyPrice * npcData.sellPriceMultiplier);
        }
        
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 3f);
        }
    }
    
    public interface IInteractable
    {
        void Interact();
    }
}
