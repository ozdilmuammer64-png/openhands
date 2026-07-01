#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace KnightOnline.Editor
{
    /// <summary>
    /// Knight Online - Tek tıkla sahne, obje, UI ve referansları kuran script
    /// Unity Editor'de Tools > Knight Online > Setup Game
    /// </summary>
    public class AutoSetup : EditorWindow
    {
        private bool setupComplete = false;
        
        [MenuItem("Tools/Knight Online/Setup Game")]
        public static void ShowWindow()
        {
            AutoSetup window = GetWindow<AutoSetup>("Knight Online Setup");
            window.minSize = new Vector2(400, 350);
            window.maxSize = new Vector2(600, 500);
        }
        
        [MenuItem("Tools/Knight Online/Setup Game", true)]
        public static bool ValidateShowWindow()
        {
            return !Application.isPlaying;
        }
        
        void OnGUI()
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("🎮 Knight Online Otomatik Kurulum", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Bu araç tüm bileşenleri tek tıkla kurar.\n\n" +
                "• Sahne kurulumu\n" +
                "• Oyuncu sistemi\n" +
                "• Canavar sistemi\n" +
                "• UI sistemi\n" +
                "• Referanslar",
                MessageType.Info);
            
            GUILayout.Space(10);
            
            if (setupComplete)
            {
                EditorGUILayout.HelpBox("✅ Kurulum tamamlandı!", MessageType.Info);
            }
            
            GUILayout.Space(10);
            
            GUI.backgroundColor = new Color(0.2f, 0.8f, 0.2f);
            if (GUILayout.Button("🚀 OYUNU KUR", GUILayout.Height(50)))
            {
                SetupGame();
            }
            GUI.backgroundColor = Color.white;
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("🗑️ Temizle"))
            {
                ClearScene();
            }
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("🔄 Referansları Yeniden Bağla"))
            {
                ConnectReferences();
            }
        }
        
        void SetupGame()
        {
            Debug.Log("🎮 Knight Online kurulumu başlıyor...");
            
            // 1. Sahne oluştur
            SetupScene();
            
            // 2. Çevre kur
            SetupEnvironment();
            
            // 3. Oyuncu kur
            SetupPlayer();
            
            // 4. UI kur
            SetupUI();
            
            // 5. Referansları bağla
            ConnectReferences();
            
            // 6. Örnek içerik
            CreateSampleContent();
            
            setupComplete = true;
            
            Debug.Log("✅ Kurulum tamamlandı!");
            EditorUtility.DisplayDialog("Kurulum Tamamlandı!", 
                "Kurulum başarılı!\n\nKontroller:\nWASD: Hareket\nMouse: Bakış\nSol Tık: Saldırı\nTab: Hedef Kilitleme\n1-9: Yetenekler", "Tamam");
        }
        
        void SetupScene()
        {
            Scene scene = SceneManager.GetActiveScene();
            if (scene.name == null || scene.name == "")
            {
                scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneGameMode.Single);
            }
            
            // Sahneyi temizle
            GameObject[] allObjects = scene.GetRootGameObjects();
            foreach (GameObject obj in allObjects)
            {
                if (obj.name != "GameManager")
                {
                    DestroyImmediate(obj);
                }
            }
        }
        
        void SetupEnvironment()
        {
            Debug.Log("🌍 Çevre kuruluyor...");
            
            // Ana ışık
            GameObject lightObj = GameObject.Find("Directional Light");
            if (lightObj == null)
            {
                lightObj = new GameObject("Directional Light");
                Light light = lightObj.AddComponent<Light>();
                light.type = LightType.Directional;
                light.intensity = 1.2f;
                light.color = new Color(1f, 0.98f, 0.9f);
                light.shadows = LightShadows.Soft;
                lightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            }
            
            // Ortam ışığı
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.4f, 0.45f, 0.55f);
            
            // Gökyüzü
            Camera.main.backgroundColor = new Color(0.5f, 0.7f, 1f);
            
            // Zemin
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(5, 1, 5);
            
            Material groundMat = new Material(Shader.Find("Standard"));
            groundMat.color = new Color(0.3f, 0.5f, 0.2f);
            ground.GetComponent<Renderer>().material = groundMat;
        }
        
        void SetupPlayer()
        {
            Debug.Log("👤 Oyuncu kuruluyor...");
            
            // Oyuncu nesnesi
            GameObject playerObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            playerObj.name = "Player";
            playerObj.tag = "Player";
            
            // Oyuncu materyali
            Material playerMat = new Material(Shader.Find("Standard"));
            playerMat.color = new Color(0.2f, 0.5f, 1f);
            playerObj.GetComponent<Renderer>().material = playerMat;
            
            // Character Controller ekle
            CharacterController charController = playerObj.AddComponent<CharacterController>();
            charController.height = 2f;
            charController.radius = 0.5f;
            charController.center = new Vector3(0, 1, 0);
            charController.stepOffset = 0.3f;
            charController.slopeLimit = 45f;
            
            // Player Controller ekle
            PlayerController playerController = playerObj.AddComponent<PlayerController>();
            playerController.playerName = "Knight";
            playerController.playerClass = PlayerClass.Warrior;
            playerController.moveSpeed = 5f;
            playerController.runSpeed = 8f;
            playerController.jumpForce = 8f;
            
            // Animator
            Animator animator = playerObj.AddComponent<Animator>();
            playerController.animator = animator;
            
            // Saldırı noktası
            GameObject attackPoint = new GameObject("AttackPoint");
            attackPoint.transform.parent = playerObj.transform;
            attackPoint.transform.localPosition = new Vector3(0, 1.5f, 1f);
            playerController.attackPoint = attackPoint.transform;
            
            // Kamera
            GameObject cameraObj = new GameObject("PlayerCamera");
            cameraObj.transform.parent = playerObj.transform;
            cameraObj.transform.localPosition = new Vector3(0, 2.5f, -5);
            Camera cam = cameraObj.AddComponent<Camera>();
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 1000;
            cam.depth = -1;
            playerController.playerCamera = cam;
            
            // Sağlık barı
            CreateHealthBar(playerObj, playerController);
            
            Debug.Log("✅ Oyuncu kuruldu");
        }
        
        void CreateHealthBar(GameObject parent, PlayerController controller)
        {
            GameObject healthBarCanvas = new GameObject("HealthBarCanvas");
            healthBarCanvas.transform.parent = parent.transform;
            healthBarCanvas.transform.localPosition = Vector3.up * 2.5f;
            
            Canvas canvas = healthBarCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            
            healthBarCanvas.AddComponent<UnityEngine.UI.CanvasScaler>();
            healthBarCanvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            // Arka plan
            GameObject bgObj = new GameObject("HealthBarBG");
            bgObj.transform.parent = healthBarCanvas.transform;
            UnityEngine.UI.Image bg = bgObj.AddComponent<UnityEngine.UI.Image>();
            bg.color = new Color(0.2f, 0.2f, 0.2f);
            RectTransform bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.sizeDelta = new Vector2(2, 0.2f);
            bgRect.localPosition = Vector3.zero;
            
            // Sağlık barı
            GameObject healthObj = new GameObject("HealthBar");
            healthObj.transform.parent = healthBarCanvas.transform;
            UnityEngine.UI.Image healthImg = healthObj.AddComponent<UnityEngine.UI.Image>();
            healthImg.color = Color.red;
            healthImg.type = UnityEngine.UI.Image.Type.Filled;
            healthImg.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
            RectTransform healthRect = healthObj.GetComponent<RectTransform>();
            healthRect.sizeDelta = new Vector2(1.9f, 0.15f);
            healthRect.localPosition = Vector3.zero;
            
            // İsim
            GameObject nameObj = new GameObject("NameLabel");
            nameObj.transform.parent = healthBarCanvas.transform;
            UnityEngine.UI.Text nameText = nameObj.AddComponent<UnityEngine.UI.Text>();
            nameText.text = controller.playerName;
            nameText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            nameText.fontSize = 12;
            nameText.color = Color.white;
            nameText.alignment = TextAnchor.MiddleCenter;
            RectTransform nameRect = nameObj.GetComponent<RectTransform>();
            nameRect.sizeDelta = new Vector2(2, 0.3f);
            nameRect.localPosition = new Vector3(0, 0.3f, 0);
        }
        
        void SetupUI()
        {
            Debug.Log("🖥️ UI kuruluyor...");
            
            // Ana Canvas
            GameObject mainCanvas = GameObject.Find("MainCanvas");
            if (mainCanvas == null)
            {
                mainCanvas = new GameObject("MainCanvas");
            }
            
            Canvas canvas = mainCanvas.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = mainCanvas.AddComponent<Canvas>();
            }
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            
            if (mainCanvas.GetComponent<UnityEngine.UI.CanvasScaler>() == null)
            {
                UnityEngine.UI.CanvasScaler scaler = mainCanvas.AddComponent<UnityEngine.UI.CanvasScaler>();
                scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f;
            }
            
            if (mainCanvas.GetComponent<UnityEngine.UI.GraphicRaycaster>() == null)
            {
                mainCanvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }
            
            // Event System
            EventSystem eventSystem = Object.FindObjectOfType<EventSystem>();
            if (eventSystem == null)
            {
                GameObject eventObj = new GameObject("EventSystem");
                eventSystem = eventObj.AddComponent<EventSystem>();
                eventObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
            
            // HUD Panel
            GameObject hudPanel = CreatePanel(mainCanvas, "HUDPanel");
            
            // Oyuncu bilgi paneli
            GameObject playerInfoPanel = CreatePanel(hudPanel, "PlayerInfoPanel");
            playerInfoPanel.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            playerInfoPanel.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
            playerInfoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(10, -10);
            playerInfoPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 160);
            
            // İsim
            CreateText(playerInfoPanel, "PlayerName", "Knight", 16, FontStyle.Bold);
            
            // Seviye
            CreateText(playerInfoPanel, "LevelText", "Lv.1", 16, FontStyle.Bold);
            
            // Sağlık barı
            CreateBar(playerInfoPanel, "HealthBar", Color.red, new Vector2(0, -30));
            
            // Mana barı
            CreateBar(playerInfoPanel, "ManaBar", Color.blue, new Vector2(0, -60));
            
            // Stamina barı
            CreateBar(playerInfoPanel, "StaminaBar", Color.green, new Vector2(0, -90));
            
            // EXP barı
            CreateBar(playerInfoPanel, "ExpBar", Color.yellow, new Vector2(0, -115));
            
            // Altın
            CreateText(playerInfoPanel, "GoldText", "💰 0", 14);
            
            // Yetenek barı
            GameObject skillBarPanel = CreatePanel(hudPanel, "SkillBarPanel");
            skillBarPanel.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0);
            skillBarPanel.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0);
            skillBarPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 10);
            skillBarPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 70);
            
            for (int i = 0; i < 9; i++)
            {
                float xPos = (i - 4) * 55;
                GameObject skillSlot = CreatePanel(skillBarPanel, $"SkillSlot_{i}");
                skillSlot.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0);
                skillSlot.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0);
                skillSlot.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, 0);
                skillSlot.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
                
                CreateText(skillSlot, "KeyText", (i + 1).ToString(), 10, FontStyle.Bold);
            }
            
            // Ölüm paneli
            GameObject deathPanel = CreatePanel(mainCanvas, "DeathPanel");
            deathPanel.SetActive(false);
            deathPanel.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            deathPanel.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            deathPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            deathPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 300);
            
            CreateText(deathPanel, "DeathTitle", "💀 ÖLDÜN!", 32, FontStyle.Bold);
            
            Debug.Log("✅ UI kuruldu");
        }
        
        GameObject CreatePanel(GameObject parent, string name)
        {
            GameObject panel = new GameObject(name);
            panel.transform.parent = parent.transform;
            
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(100, 100);
            
            return panel;
        }
        
        GameObject CreateText(GameObject parent, string name, string text, int fontSize = 12, FontStyle fontStyle = FontStyle.Normal)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.parent = parent.transform;
            
            UnityEngine.UI.Text textComponent = textObj.AddComponent<UnityEngine.UI.Text>();
            textComponent.text = text;
            textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            textComponent.fontSize = fontSize;
            textComponent.fontStyle = fontStyle;
            textComponent.color = Color.white;
            
            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(200, 30);
            
            return textObj;
        }
        
        void CreateBar(GameObject parent, string name, Color fillColor, Vector2 offset)
        {
            GameObject container = CreatePanel(parent, name + "Container");
            container.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            container.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
            container.GetComponent<RectTransform>().anchoredPosition = offset;
            container.GetComponent<RectTransform>().sizeDelta = new Vector2(240, 25);
            
            // Arkaplan
            GameObject bg = new GameObject("Background");
            bg.transform.parent = container.transform;
            UnityEngine.UI.Image bgImg = bg.AddComponent<UnityEngine.UI.Image>();
            bgImg.color = new Color(0.2f, 0.2f, 0.2f);
            RectTransform bgRect = bg.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            // Dolgu
            GameObject fill = new GameObject("Fill");
            fill.transform.parent = container.transform;
            UnityEngine.UI.Image fillImg = fill.AddComponent<UnityEngine.UI.Image>();
            fillImg.color = fillColor;
            fillImg.type = UnityEngine.UI.Image.Type.Filled;
            fillImg.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = new Vector2(2, 2);
            fillRect.offsetMax = new Vector2(-2, -2);
        }
        
        void ConnectReferences()
        {
            Debug.Log("🔗 Referanslar bağlanıyor...");
            
            // GameSettings
            GameObject gameSettingsObj = GameObject.Find("GameManager");
            if (gameSettingsObj == null)
            {
                gameSettingsObj = new GameObject("GameManager");
            }
            
            GameSettings gameSettings = gameSettingsObj.GetComponent<GameSettings>();
            if (gameSettings == null)
            {
                gameSettings = gameSettingsObj.AddComponent<GameSettings>();
            }
            
            // UI Manager
            UIManager uiManager = Object.FindObjectOfType<UIManager>();
            if (uiManager == null)
            {
                GameObject uiManagerObj = new GameObject("UIManager");
                uiManager = uiManagerObj.AddComponent<UIManager>();
            }
            
            // Player
            PlayerController player = Object.FindObjectOfType<PlayerController>();
            if (player != null)
            {
                SerializedObject serializedUI = new SerializedObject(uiManager);
                serializedUI.FindProperty("player").objectReferenceValue = player;
                serializedUI.ApplyModifiedProperties();
                
                if (player.playerCamera == null)
                {
                    Camera cam = Object.FindObjectOfType<Camera>();
                    if (cam != null)
                    {
                        SerializedObject serializedPlayer = new SerializedObject(player);
                        serializedPlayer.FindProperty("playerCamera").objectReferenceValue = cam;
                        serializedPlayer.ApplyModifiedProperties();
                    }
                }
            }
            
            // Skill System
            SkillSystem skillSystem = Object.FindObjectOfType<SkillSystem>();
            if (skillSystem == null)
            {
                GameObject skillSystemObj = new GameObject("SkillSystem");
                skillSystem = skillSystemObj.AddComponent<SkillSystem>();
            }
            
            // Inventory System
            InventorySystem inventorySystem = Object.FindObjectOfType<InventorySystem>();
            if (inventorySystem == null)
            {
                GameObject inventorySystemObj = new GameObject("InventorySystem");
                inventorySystem = inventorySystemObj.AddComponent<InventorySystem>();
            }
            
            // Quest System
            QuestSystem questSystem = Object.FindObjectOfType<QuestSystem>();
            if (questSystem == null)
            {
                GameObject questSystemObj = new GameObject("QuestSystem");
                questSystem = questSystemObj.AddComponent<QuestSystem>();
            }
            
            Debug.Log("✅ Referanslar bağlandı");
        }
        
        void CreateSampleContent()
        {
            Debug.Log("📦 Örnek içerik oluşturuluyor...");
            
            // Canavarlar
            CreateMonster("Goblin", new Vector3(10, 0, 5), Color.green);
            CreateMonster("Orc", new Vector3(-10, 0, 5), new Color(0.4f, 0.6f, 0.3f));
            CreateMonster("Skeleton", new Vector3(15, 0, -10), Color.white);
            
            // Boss
            CreateBoss("Dragon", new Vector3(0, 0, 30));
            
            // NPC
            CreateNPC("Merchant", "Tüccar", new Vector3(5, 0, -5), Color.yellow);
            CreateNPC("QuestGiver", "Görevci", new Vector3(-5, 0, -5), Color.green);
            
            // Oyuncu altın
            PlayerController player = Object.FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.gold = 1000;
            }
            
            Debug.Log("✅ Örnek içerik oluşturuldu");
        }
        
        void CreateMonster(string name, Vector3 position, Color color)
        {
            GameObject monster = GameObject.CreatePrimitive(PrimitiveType.Cube);
            monster.name = name;
            monster.transform.position = position;
            monster.transform.localScale = new Vector3(1, 2, 1);
            monster.layer = LayerMask.NameToLayer("Enemy");
            
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = color;
            monster.GetComponent<Renderer>().material = mat;
            
            MonsterController controller = monster.AddComponent<MonsterController>();
            controller.monsterName = name;
            controller.monsterType = MonsterType.Normal;
            controller.level = UnityEngine.Random.Range(1, 5);
            controller.maxHealth = 30 + (controller.level * 10);
            controller.currentHealth = controller.maxHealth;
            controller.minDamage = 3 + controller.level;
            controller.maxDamage = 5 + controller.level;
            controller.experienceReward = 10 + (controller.level * 5);
            
            Animator animator = monster.AddComponent<Animator>();
            controller.animator = animator;
            
            CreateHealthBar(monster, controller);
        }
        
        void CreateBoss(string name, Vector3 position)
        {
            GameObject boss = GameObject.CreatePrimitive(PrimitiveType.Cube);
            boss.name = name;
            boss.transform.position = position;
            boss.transform.localScale = new Vector3(3, 5, 3);
            boss.layer = LayerMask.NameToLayer("Enemy");
            
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.8f, 0.1f, 0.1f);
            boss.GetComponent<Renderer>().material = mat;
            
            MonsterController controller = boss.AddComponent<MonsterController>();
            controller.monsterName = name;
            controller.monsterType = MonsterType.Boss;
            controller.level = 10;
            controller.maxHealth = 500;
            controller.currentHealth = controller.maxHealth;
            controller.minDamage = 20;
            controller.maxDamage = 35;
            controller.experienceReward = 500;
            controller.goldRewardMin = 500;
            controller.goldRewardMax = 1000;
            
            Animator animator = boss.AddComponent<Animator>();
            controller.animator = animator;
            
            CreateHealthBar(boss, controller);
            
            Debug.Log("🗡️ Boss oluşturuldu: " + name);
        }
        
        void CreateNPC(string name, string displayName, Vector3 position, Color color)
        {
            GameObject npc = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            npc.name = name;
            npc.transform.position = position;
            npc.transform.localScale = new Vector3(1, 1.5f, 1);
            npc.layer = LayerMask.NameToLayer("NPC");
            
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = color;
            npc.GetComponent<Renderer>().material = mat;
            
            NPCController controller = npc.AddComponent<NPCController>();
            controller.npcData = ScriptableObject.CreateInstance<NPCData>();
            controller.npcData.npcName = displayName;
            controller.npcData.npcType = NPCType.Merchant;
            
            // İsim etiketi
            GameObject nameTagCanvas = new GameObject("NameTagCanvas");
            nameTagCanvas.transform.parent = npc.transform;
            nameTagCanvas.transform.localPosition = Vector3.up * 2.5f;
            
            Canvas canvas = nameTagCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            
            GameObject nameTag = new GameObject("NameTag");
            nameTag.transform.parent = nameTagCanvas.transform;
            
            UnityEngine.UI.Text text = nameTag.AddComponent<UnityEngine.UI.Text>();
            text.text = displayName;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 14;
            text.fontStyle = FontStyle.Bold;
            text.color = color;
            text.alignment = TextAnchor.MiddleCenter;
            
            RectTransform rect = nameTag.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(150, 30);
        }
        
        void ClearScene()
        {
            if (!EditorUtility.DisplayDialog("Sahneyi Temizle",
                "Tüm nesneleri silmek istediğinize emin misiniz?",
                "Evet", "İptal"))
            {
                return;
            }
            
            Scene scene = SceneManager.GetActiveScene();
            GameObject[] allObjects = scene.GetRootGameObjects();
            foreach (GameObject obj in allObjects)
            {
                DestroyImmediate(obj);
            }
            
            setupComplete = false;
            Debug.Log("🗑️ Sahne temizlendi!");
        }
    }
}
#endif
