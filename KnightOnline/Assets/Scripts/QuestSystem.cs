using UnityEngine;
using System.Collections.Generic;

namespace KnightOnline
{
    public enum QuestType { Kill, Collect, Explore, Escort, Talk }
    public enum QuestDifficulty { Easy, Medium, Hard, Elite }
    public enum QuestStatus { Locked, Available, Accepted, Completed, TurnedIn }
    
    [CreateAssetMenu(fileName = "New Quest", menuName = "Knight Online/Quest")]
    public class QuestData : ScriptableObject
    {
        public string questName = "New Quest";
        public string description = "Quest description";
        [TextArea(2, 4)]
        public string objective = "Complete the objective";
        public QuestType questType = QuestType.Kill;
        public QuestDifficulty difficulty = QuestDifficulty.Easy;
        
        [Header("Requirements")]
        public int requiredLevel = 1;
        public List<QuestData> prerequisites = new List<QuestData>();
        
        [Header("Objectives")]
        public List<QuestObjective> objectives = new List<QuestObjective>();
        public int totalObjectiveCount = 1;
        
        [Header("Rewards")]
        public int experienceReward = 100;
        public int goldReward = 50;
        public List<ItemData> itemRewards = new List<ItemData>();
        public int skillPointReward = 0;
        
        [Header("Dialogue")]
        public string acceptDialogue = "I need your help with something.";
        public string inProgressDialogue = "Are you making progress?";
        public string completeDialogue = "Thank you for your help!";
        
        [Header("Settings")]
        public bool repeatable = false;
        public float repeatCooldown = 0f;
        public bool shareable = false;
        public bool timelimited = false;
        public float timeLimit = 0f;
    }
    
    [System.Serializable]
    public class QuestObjective
    {
        public string objectiveName = "Kill enemies";
        public QuestType type = QuestType.Kill;
        
        [Header("Kill Objective")]
        public string targetEnemyName;
        public int killCount = 10;
        
        [Header("Collect Objective")]
        public ItemData collectItem;
        public int collectCount = 5;
        
        [Header("Explore Objective")]
        public Vector3 exploreLocation;
        public float exploreRadius = 5f;
        
        [Header("Talk Objective")]
        public NPCData talkToNPC;
        
        [Header("Escort Objective")]
        public string escortNPCName;
        public Vector3 destination;
        
        [Header("Progress")]
        public int currentProgress = 0;
    }
    
    public class ActiveQuest
    {
        public QuestData data;
        public QuestStatus status;
        public float startTime;
        public float remainingTime;
        public Dictionary<string, int> objectiveProgress = new Dictionary<string, int>();
        
        public ActiveQuest(QuestData questData)
        {
            data = questData;
            status = QuestStatus.Accepted;
            startTime = Time.time;
            remainingTime = questData.timeLimit;
            
            foreach (QuestObjective obj in questData.objectives)
            {
                objectiveProgress[obj.objectiveName] = 0;
            }
        }
        
        public bool IsComplete()
        {
            foreach (QuestObjective obj in data.objectives)
            {
                int progress;
                if (!objectiveProgress.TryGetValue(obj.objectiveName, out progress))
                {
                    return false;
                }
                
                int targetCount = obj.type == QuestType.Kill ? obj.killCount : obj.collectCount;
                if (progress < targetCount)
                {
                    return false;
                }
            }
            return true;
        }
        
        public float GetProgress()
        {
            int total = 0;
            int completed = 0;
            
            foreach (QuestObjective obj in data.objectives)
            {
                total++;
                int targetCount = obj.type == QuestType.Kill ? obj.killCount : obj.collectCount;
                int progress;
                if (objectiveProgress.TryGetValue(obj.objectiveName, out progress) && progress >= targetCount)
                {
                    completed++;
                }
            }
            
            return total > 0 ? (float)completed / total : 0f;
        }
    }
    
    public class QuestSystem : MonoBehaviour
    {
        public static QuestSystem Instance { get; private set; }
        
        [Header("Quest Lists")]
        public List<QuestData> allQuests = new List<QuestData>();
        public List<ActiveQuest> activeQuests = new List<ActiveQuest>();
        public List<QuestData> completedQuests = new List<QuestData>();
        
        [Header("Quest Tracking")]
        public QuestData trackedQuest;
        public bool showQuestMarkers = true;
        
        [Header("Components")]
        public PlayerController player;
        public Transform questMarkerParent;
        
        // Events
        public System.Action<QuestData> OnQuestAccepted;
        public System.Action<QuestData> OnQuestCompleted;
        public System.Action<QuestData> OnQuestTurnedIn;
        public System.Action<QuestData, string, int, int> OnObjectiveProgress;
        
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
        }
        
        void Update()
        {
            UpdateTimedQuests();
        }
        
        void UpdateTimedQuests()
        {
            foreach (ActiveQuest quest in activeQuests)
            {
                if (quest.data.timelimited && quest.remainingTime > 0)
                {
                    quest.remainingTime -= Time.deltaTime;
                    
                    if (quest.remainingTime <= 0)
                    {
                        FailQuest(quest);
                    }
                }
            }
        }
        
        public List<QuestData> GetAvailableQuests()
        {
            List<QuestData> available = new List<QuestData>();
            
            foreach (QuestData quest in allQuests)
            {
                if (CanAcceptQuest(quest))
                {
                    available.Add(quest);
                }
            }
            
            return available;
        }
        
        public bool CanAcceptQuest(QuestData quest)
        {
            // Check if already accepted or completed
            if (activeQuests.Exists(q => q.data == quest)) return false;
            if (completedQuests.Contains(quest) && !quest.repeatable) return false;
            
            // Check level requirement
            if (player != null && player.level < quest.requiredLevel) return false;
            
            // Check prerequisites
            foreach (QuestData prereq in quest.prerequisites)
            {
                if (!completedQuests.Contains(prereq))
                {
                    return false;
                }
            }
            
            return true;
        }
        
        public bool AcceptQuest(QuestData quest)
        {
            if (!CanAcceptQuest(quest)) return false;
            
            ActiveQuest newQuest = new ActiveQuest(quest);
            activeQuests.Add(newQuest);
            
            OnQuestAccepted?.Invoke(quest);
            
            Debug.Log($"Quest accepted: {quest.questName}");
            return true;
        }
        
        public bool CompleteQuest(QuestData quest)
        {
            ActiveQuest activeQuest = activeQuests.Find(q => q.data == quest);
            if (activeQuest == null) return false;
            
            if (!activeQuest.IsComplete()) return false;
            
            activeQuest.status = QuestStatus.Completed;
            
            OnQuestCompleted?.Invoke(quest);
            
            Debug.Log($"Quest completed: {quest.questName}");
            return true;
        }
        
        public bool TurnInQuest(QuestData quest)
        {
            ActiveQuest activeQuest = activeQuests.Find(q => q.data == quest);
            if (activeQuest == null) return false;
            
            if (activeQuest.status != QuestStatus.Completed) return false;
            
            // Give rewards
            if (player != null)
            {
                player.GainExperience(quest.experienceReward);
                player.AddGold(quest.goldReward);
                
                foreach (ItemData item in quest.itemRewards)
                {
                    InventorySystem.Instance?.AddItem(item);
                }
            }
            
            activeQuest.status = QuestStatus.TurnedIn;
            activeQuests.Remove(activeQuest);
            
            if (!quest.repeatable)
            {
                completedQuests.Add(quest);
            }
            
            OnQuestTurnedIn?.Invoke(quest);
            
            Debug.Log($"Quest turned in: {quest.questName}");
            return true;
        }
        
        public void FailQuest(ActiveQuest quest)
        {
            activeQuests.Remove(quest);
            Debug.Log($"Quest failed: {quest.data.questName}");
        }
        
        public void UpdateQuestProgress(QuestType type, string targetName, int amount = 1)
        {
            foreach (ActiveQuest quest in activeQuests)
            {
                if (quest.status != QuestStatus.Accepted) continue;
                
                foreach (QuestObjective objective in quest.data.objectives)
                {
                    if (objective.type != type) continue;
                    
                    bool matches = false;
                    
                    switch (type)
                    {
                        case QuestType.Kill:
                            matches = objective.targetEnemyName == targetName;
                            break;
                        case QuestType.Collect:
                            matches = objective.collectItem != null && objective.collectItem.itemName == targetName;
                            break;
                        case QuestType.Explore:
                            matches = objective.objectiveName == targetName;
                            break;
                        case QuestType.Talk:
                            matches = objective.talkToNPC != null && objective.talkToNPC.npcName == targetName;
                            break;
                    }
                    
                    if (matches)
                    {
                        int currentProgress;
                        quest.objectiveProgress.TryGetValue(objective.objectiveName, out currentProgress);
                        
                        int targetCount = type == QuestType.Kill ? objective.killCount : objective.collectCount;
                        int newProgress = Mathf.Min(currentProgress + amount, targetCount);
                        
                        quest.objectiveProgress[objective.objectiveName] = newProgress;
                        
                        OnObjectiveProgress?.Invoke(quest.data, objective.objectiveName, newProgress, targetCount);
                        
                        if (quest.IsComplete())
                        {
                            CompleteQuest(quest.data);
                        }
                        
                        break;
                    }
                }
            }
        }
        
        public void UpdateKillProgress(string enemyName)
        {
            UpdateQuestProgress(QuestType.Kill, enemyName, 1);
        }
        
        public void UpdateCollectProgress(ItemData item)
        {
            UpdateQuestProgress(QuestType.Collect, item.itemName, 1);
        }
        
        public void UpdateExploreProgress(string locationName)
        {
            UpdateQuestProgress(QuestType.Explore, locationName, 1);
        }
        
        public void UpdateTalkProgress(NPCData npc)
        {
            UpdateQuestProgress(QuestType.Talk, npc.npcName, 1);
        }
        
        public void TrackQuest(QuestData quest)
        {
            trackedQuest = quest;
        }
        
        public void StopTracking()
        {
            trackedQuest = null;
        }
        
        public ActiveQuest GetActiveQuest(QuestData quest)
        {
            return activeQuests.Find(q => q.data == quest);
        }
        
        public bool HasQuest(QuestData quest)
        {
            return activeQuests.Exists(q => q.data == quest);
        }
        
        public int GetObjectiveProgress(ActiveQuest quest, string objectiveName)
        {
            int progress;
            quest.objectiveProgress.TryGetValue(objectiveName, out progress);
            return progress;
        }
        
        public int GetObjectiveTarget(QuestObjective objective)
        {
            return objective.type == QuestType.Kill ? objective.killCount : objective.collectCount;
        }
    }
}
