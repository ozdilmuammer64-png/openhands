#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace KnightOnline.Editor
{
    /// <summary>
    /// Knight Online - Tüm sahneyi, objeleri, UI'ı ve referansları tek tıkla otomatik kuran script
    /// Unity Editor'de Tools > Knight Online > Setup Game yolunu kullanın
    /// </summary>
    public class AutoSetup : EditorWindow
    {
        private bool setupComplete = false;
        private Vector2 scrollPosition;
        
        [MenuItem("Tools/Knight Online/Setup Game", false, 1)]
        public static void ShowWindow()
        {
            AutoSetup window = GetWindow<AutoSetup>("Knight Online Setup");
            window.minSize = new Vector2(400, 300);
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
                "Bu araç Knight Online benzeri bir oyun için gerekli tüm bileşenleri tek tıkla kurar.\n\n" +
                "• Sahne kurulumu\n" +
                "• Oyuncu sistemi\n" +
                "• Düşman ve canavar sistemi\n" +
                "• NPC sistemi\n" +
                "• Envanter sistemi\n" +
                "• Yetenek sistemi\n" +
                "• UI sistemi\n" +
                "• Işık ve çevre\n" +
                "• Referans bağlantıları",
                MessageType.Info);
            
            GUILayout.Space(10);
            
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Kurulum Durumu", EditorStyles.boldLabel);
            
            if (setupComplete)
            {
                GUI.backgroundColor = Color.green;
                EditorGUILayout.HelpBox("✅ Kurulum tamamlandı!", MessageType.Info);
                GUI.backgroundColor = Color.white;
            }
            else
            {
                EditorGUILayout.HelpBox("⏳ Kurulum yapılmadı", MessageType.Warning);
            }
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(10);
            
            GUI.backgroundColor = new Color(0.2f, 0.8f, 0.2f);
            if (GUILayout.Button("🚀 OYUNU KUR (TAM KURULUM)", GUILayout.Height(50)))
            {
                SetupGame();
            }
            GUI.backgroundColor = Color.white;
            
            GUILayout.Space(10);
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Sadece UI"))
            {
                SetupUIOnly();
            }
            if (GUILayout.Button("Sadece Oyuncu"))
            {
                SetupPlayerOnly();
            }
            if (GUILayout.Button("Sadece Çevre"))
            {
                SetupEnvironmentOnly();
            }
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("🗑️ Temizle (Sahneyi Sıfırla"))
            {
                ClearScene();
            }
            
            GUILayout.Space(10);
            
            if (GUI.changed)
            {
                EditorUtility.SetDirty(this);
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
            
            // 6. Örnek içerik ekle
            CreateSampleContent();
            
            setupComplete = true;
            
            Debug.Log("✅ Knight Online kurulumu tamamlandı!");
            EditorUtility.DisplayDialog("Kurulum Tamamlandı!", 
                "Knight Online oyunu başarıyla kuruldu!\n\n" +
                "Kontroller:\n" +
                "- WASD: Hareket\n" +
                "- Mouse: Bakış yönü\n" +
                "- Sol Tık: Saldırı\n" +
                "- Tab: Hedef kilitleme\n" +
                "- 1-9: Yetenekler\n" +
                "- I: Envanter\n" +
                "- C: Karakter\n" +
                "- K: Yetenekler\n" +
                "- Q: Görevler", "Tamam");
        }
        
        void SetupScene()
        {
            Debug.Log("📦 Sahne kuruluyor...");
            
            // Mevcut sahneyi temizle
            ClearSceneObjects();
            
            // Yeni sahne oluştur veya mevcut olanı kullan
            Scene scene = SceneManager.GetActiveScene();
            if (scene.name == null || scene.name == "")
            {
                scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneGameMode.Single);
            }
        }
        
        void ClearSceneObjects()
        {
            // Sahnedeki tüm nesneleri temizle (parent'lı olanlar hariç)
            GameObject[] allObjects = SceneManager.GetActiveScene().GetRootGameObjects();
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
            RenderSettings.skybox = null;
            Camera.main.backgroundColor = new Color(0.5f, 0.7f, 1f);
            
            // Zemin
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(5, 1, 5);
            
            // Zemin materyali
            Material groundMat = new Material(Shader.Find("Standard"));
            groundMat.color = new Color(0.3f, 0.5f, 0.2f);
            ground.GetComponent<Renderer>().material = groundMat;
            
            // Dünya manager
            GameObject worldManager = new GameObject("WorldManager");
            
            // Zaman sistemi
            GameObject timeSystem = new GameObject("TimeSystem");
            timeSystem.transform.parent = worldManager.transform;
            timeSystem.AddComponent<TimeSystem>();
            
            // Spawn alanları
            CreateSpawnAreas(worldManager);
            
            // Region bölgeleri
            CreateRegions(worldManager);
        }
        
        void CreateSpawnAreas(GameObject parent)
        {
            GameObject spawnAreas = new GameObject("SpawnAreas");
            spawnAreas.transform.parent = parent.transform;
            
            // Oyuncu spawn
            GameObject playerSpawn = GameObject.CreatePrimitive(PrimitiveType.Cube);
            playerSpawn.name = "PlayerSpawn";
            playerSpawn.transform.parent = spawnAreas.transform;
            playerSpawn.transform.position = new Vector3(0, 0.5f, 0);
            playerSpawn.transform.localScale = new Vector3(2, 1, 2);
            playerSpawn.GetComponent<Collider>().isTrigger = true;
            
            Material spawnMat = new Material(Shader.Find("Standard"));
            spawnMat.color = new Color(0, 0.5f, 1, 0.3f);
            playerSpawn.GetComponent<Renderer>().material = spawnMat;
        }
        
        void CreateRegions(GameObject parent)
        {
            string[] regionNames = { "Town", "Forest", "Dungeon", "BossArena" };
            Color[] regionColors = { 
                new Color(0.8f, 0.6f, 0.4f, 0.3f),
                new Color(0.2f, 0.6f, 0.2f, 0.3f),
                new Color(0.4f, 0.3f, 0.3f, 0.3f),
                new Color(0.8f, 0.2f, 0.2f, 0.3f)
            };
            Vector3[] positions = {
                new Vector3(0, 0.1f, 0),
                new Vector3(30, 0.1f, 0),
                new Vector3(-30, 0.1f, 0),
                new Vector3(0, 0.1f, 50)
            };
            
            for (int i = 0; i < regionNames.Length; i++)
            {
                GameObject region = GameObject.CreatePrimitive(PrimitiveType.Quad);
                region.name = $"Region_{regionNames[i]}";
                region.transform.parent = parent.transform;
                region.transform.position = positions[i];
                region.transform.rotation = Quaternion.Euler(90, 0, 0);
                region.transform.localScale = new Vector3(40, 40, 1);
                region.GetComponent<Collider>().isTrigger = true;
                
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = regionColors[i];
                region.GetComponent<Renderer>().material = mat;
                
                region.AddComponent<RegionTrigger>().regionName = regionNames[i];
            }
        }
        
        void SetupPlayer()
        {
            Debug.Log("👤 Oyuncu kuruluyor...");
            
            // Oyuncu nesnesi
            GameObject playerObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            playerObj.name = "Player";
            playerObj.tag = "Player";
            playerObj.layer = LayerMask.NameToLayer("Player");
            
            // Oyuncu materyali
            Material playerMat = new Material(Shader.Find("Standard"));
            playerMat.color = new Color(0.2f, 0.5f, 1f);
            playerObj.GetComponent<Renderer>().material = playerMat;
            
            // Collider ayarları
            CapsuleCollider playerCollider = playerObj.GetComponent<CapsuleCollider>();
            playerCollider.height = 2f;
            playerCollider.radius = 0.5f;
            playerCollider.center = new Vector3(0, 1, 0);
            
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
            
            // Player Input Manager
            playerObj.AddComponent<PlayerInputManager>();
            
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
            Image bg = bgObj.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.2f, 0.2f);
            RectTransform bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.sizeDelta = new Vector2(2, 0.2f);
            bgRect.localPosition = Vector3.zero;
            
            // Sağlık barı
            GameObject healthObj = new GameObject("HealthBar");
            healthObj.transform.parent = healthBarCanvas.transform;
            Image healthImg = healthObj.AddComponent<Image>();
            healthImg.color = Color.red;
            healthImg.type = Image.Type.Filled;
            healthImg.fillMethod = Image.FillMethod.Horizontal;
            RectTransform healthRect = healthObj.GetComponent<RectTransform>();
            healthRect.sizeDelta = new Vector2(1.9f, 0.15f);
            healthRect.localPosition = Vector3.zero;
            
            // İsim etiketi
            GameObject nameObj = new GameObject("NameLabel");
            nameObj.transform.parent = healthBarCanvas.transform;
            Text nameText = nameObj.AddComponent<Text>();
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
            EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
            if (eventSystem == null)
            {
                GameObject eventObj = new GameObject("EventSystem");
                eventSystem = eventObj.AddComponent<EventSystem>();
                eventObj.AddComponent<StandaloneInputModule>();
            }
            
            // HUD Panel
            GameObject hudPanel = CreatePanel(mainCanvas, "HUDPanel", new Vector2(0.5f, 0.5f), new Vector2(0, 0), new Vector2(1, 1));
            
            // Oyuncu bilgi paneli (sol üst)
            GameObject playerInfoPanel = CreatePanel(hudPanel, "PlayerInfoPanel", new Vector2(0, 1), new Vector2(10, -10), new Vector2(250, 150));
            
            // İsim
            CreateText(playerInfoPanel, "PlayerName", "Knight", new Vector2(0, 1), new Vector2(5, -5), new Vector2(120, 20), 16, FontStyle.Bold);
            
            // Seviye
            CreateText(playerInfoPanel, "LevelText", "Lv.1", new Vector2(1, 1), new Vector2(-5, -5), new Vector2(100, 20), 16, FontStyle.Bold, TextAnchor.MiddleRight);
            
            // Sağlık barı
            GameObject healthBarContainer = CreatePanel(playerInfoPanel, "HealthBarContainer", new Vector2(0, 1), new Vector2(0, -30), new Vector2(240, 25));
            CreateHealthBarUI(healthBarContainer, "HealthBar", Color.red);
            
            // Mana barı
            GameObject manaBarContainer = CreatePanel(playerInfoPanel, "ManaBarContainer", new Vector2(0, 1), new Vector2(0, -60), new Vector2(240, 25));
            CreateHealthBarUI(manaBarContainer, "ManaBar", Color.blue);
            
            // Stamina barı
            GameObject staminaBarContainer = CreatePanel(playerInfoPanel, "StaminaBarContainer", new Vector2(0, 1), new Vector2(0, -90), new Vector2(240, 20));
            CreateHealthBarUI(staminaBarContainer, "StaminaBar", Color.green);
            
            // EXP barı
            GameObject expBarContainer = CreatePanel(playerInfoPanel, "ExpBarContainer", new Vector2(0, 1), new Vector2(0, -115), new Vector2(240, 15));
            CreateHealthBarUI(expBarContainer, "ExpBar", Color.yellow);
            
            // Altın
            CreateText(playerInfoPanel, "GoldText", "💰 0", new Vector2(0, 1), new Vector2(0, -135), new Vector2(150, 20), 14);
            
            // Yetenek barı (alt)
            GameObject skillBarPanel = CreatePanel(hudPanel, "SkillBarPanel", new Vector2(0.5f, 0), new Vector2(0, 10), new Vector2(500, 70));
            
            for (int i = 0; i < 9; i++)
            {
                float xPos = (i - 4) * 55;
                GameObject skillSlot = CreatePanel(skillBarPanel, $"SkillSlot_{i}", new Vector2(0.5f, 0), new Vector2(xPos, 0), new Vector2(50, 50));
                CreateImage(skillSlot, "SkillIcon", new Color(0.3f, 0.3f, 0.3f));
                
                GameObject keyText = CreateText(skillSlot, "KeyText", (i + 1).ToString(), new Vector2(0.5f, 0.5f), new Vector2(0, -15), new Vector2(20, 20), 10, FontStyle.Bold, TextAnchor.MiddleCenter, Color.yellow);
                
                GameObject cooldownOverlay = CreatePanel(skillSlot, "CooldownOverlay", new Vector2(0.5f, 0.5f), new Vector2(0, 0), new Vector2(50, 50));
                Image cooldownImg = cooldownOverlay.AddComponent<Image>();
                cooldownImg.color = new Color(0, 0, 0, 0.7f);
                cooldownImg.fillAmount = 0;
            }
            
            // Hedef bilgi paneli (üst orta)
            GameObject targetPanel = CreatePanel(hudPanel, "TargetInfoPanel", new Vector2(0.5f, 1), new Vector2(0, -10), new Vector2(300, 60));
            targetPanel.SetActive(false);
            
            CreateText(targetPanel, "TargetName", "Hedef", new Vector2(0.5f, 1), new Vector2(0, -5), new Vector2(200, 20), 14, FontStyle.Bold, TextAnchor.MiddleCenter);
            
            GameObject targetHealthContainer = CreatePanel(targetPanel, "TargetHealthContainer", new Vector2(0.5f, 1), new Vector2(0, -30), new Vector2(280, 20));
            CreateHealthBarUI(targetHealthContainer, "TargetHealth", Color.red);
            
            // Mini harita (sağ üst)
            GameObject minimapPanel = CreatePanel(hudPanel, "MinimapPanel", new Vector2(1, 1), new Vector2(-10, -10), new Vector2(150, 150));
            CreateImage(minimapPanel, "MinimapBackground", new Color(0.1f, 0.1f, 0.1f, 0.8f));
            
            // Bildirimler
            GameObject notificationPanel = CreatePanel(hudPanel, "NotificationPanel", new Vector2(0.5f, 0.5f), new Vector2(0, 100), new Vector2(400, 50));
            
            // Ölüm paneli
            GameObject deathPanel = CreatePanel(mainCanvas, "DeathPanel", new Vector2(0.5f, 0.5f), new Vector2(0, 0), new Vector2(400, 300));
            deathPanel.SetActive(false);
            
            GameObject deathTitle = CreateText(deathPanel, "DeathTitle", "💀 ÖLDÜN!", new Vector2(0.5f, 0.7f), new Vector2(0, 0), new Vector2(300, 50), 32, FontStyle.Bold, TextAnchor.MiddleCenter, Color.red);
            CreateButton(deathPanel, "RespawnButton", "Yeniden Doğ", new Vector2(0.5f, 0.3f), new Vector2(0, 0), new Vector2(150, 50), () => {
                Debug.Log("Yeniden doğma butonu tıklandı!");
            });
            
            // XP Gain gösterge paneli
            GameObject xpGainPanel = CreatePanel(hudPanel, "XPGainPanel", new Vector2(0.5f, 0), new Vector2(0, 100), new Vector2(300, 40));
            
            Debug.Log("✅ UI kuruldu");
        }
        
        GameObject CreatePanel(GameObject parent, string name, Vector2 anchor, Vector2 offset, Vector2 size)
        {
            GameObject panel = new GameObject(name);
            panel.transform.parent = parent.transform;
            
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = offset;
            rect.sizeDelta = size;
            
            return panel;
        }
        
        GameObject CreateImage(GameObject parent, string name, Color color)
        {
            GameObject imgObj = new GameObject(name);
            imgObj.transform.parent = parent.transform;
            
            Image img = imgObj.AddComponent<Image>();
            img.color = color;
            
            RectTransform rect = imgObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(100, 100);
            
            return imgObj;
        }
        
        GameObject CreateText(GameObject parent, string name, string text, Vector2 anchor, Vector2 offset, Vector2 size, int fontSize = 12, FontStyle fontStyle = FontStyle.Normal, TextAnchor alignment = TextAnchor.MiddleLeft, Color color = default)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.parent = parent.transform;
            
            Text textComponent = textObj.AddComponent<Text>();
            textComponent.text = text;
            textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            textComponent.fontSize = fontSize;
            textComponent.fontStyle = fontStyle;
            textComponent.alignment = alignment;
            textComponent.color = color == default ? Color.white : color;
            
            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = offset;
            rect.sizeDelta = size;
            
            return textObj;
        }
        
        GameObject CreateButton(GameObject parent, string name, string text, Vector2 anchor, Vector2 offset, Vector2 size, UnityEngine.Events.UnityAction onClick)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.parent = parent.transform;
            
            Button button = buttonObj.AddComponent<Button>();
            button.onClick.AddListener(onClick);
            
            RectTransform rect = buttonObj.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = offset;
            rect.sizeDelta = size;
            
            // Arkaplan
            Image bg = buttonObj.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.4f, 0.2f);
            
            // Text
            GameObject textObj = new GameObject("Text");
            textObj.transform.parent = buttonObj.transform;
            Text textComponent = textObj.AddComponent<Text>();
            textComponent.text = text;
            textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            textComponent.fontSize = 14;
            textComponent.color = Color.white;
            textComponent.alignment = TextAnchor.MiddleCenter;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            return buttonObj;
        }
        
        void CreateHealthBarUI(GameObject parent, string name, Color fillColor)
        {
            // Arkaplan
            GameObject bg = new GameObject("Background");
            bg.transform.parent = parent.transform;
            Image bgImg = bg.AddComponent<Image>();
            bgImg.color = new Color(0.2f, 0.2f, 0.2f);
            RectTransform bgRect = bg.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            // Dolgu
            GameObject fill = new GameObject("Fill");
            fill.transform.parent = parent.transform;
            Image fillImg = fill.AddComponent<Image>();
            fillImg.color = fillColor;
            fillImg.type = Image.Type.Filled;
            fillImg.fillMethod = Image.FillMethod.Horizontal;
            fillImg.fillOrigin = (int)Image.OriginHorizontal.Left;
            
            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = new Vector2(2, 2);
            fillRect.offsetMax = new Vector2(-2, -2);
        }
        
        void ConnectReferences()
        {
            Debug.Log("🔗 Referanslar bağlanıyor...");
            
            // GameSettings singleton
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
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager == null)
            {
                GameObject uiManagerObj = new GameObject("UIManager");
                uiManager = uiManagerObj.AddComponent<UIManager>();
            }
            
            // Player Controller
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                // UI Manager'a player bağla
                SerializedObject serializedUI = new SerializedObject(uiManager);
                serializedUI.FindProperty("player").objectReferenceValue = player;
                serializedUI.ApplyModifiedProperties();
                
                // Player'a camera bağla
                if (player.playerCamera == null)
                {
                    Camera cam = FindObjectOfType<Camera>();
                    if (cam != null)
                    {
                        SerializedObject serializedPlayer = new SerializedObject(player);
                        serializedPlayer.FindProperty("playerCamera").objectReferenceValue = cam;
                        serializedPlayer.ApplyModifiedProperties();
                    }
                }
            }
            
            // Skill System
            SkillSystem skillSystem = FindObjectOfType<SkillSystem>();
            if (skillSystem == null)
            {
                GameObject skillSystemObj = new GameObject("SkillSystem");
                skillSystem = skillSystemObj.AddComponent<SkillSystem>();
            }
            
            // Inventory System
            InventorySystem inventorySystem = FindObjectOfType<InventorySystem>();
            if (inventorySystem == null)
            {
                GameObject inventorySystemObj = new GameObject("InventorySystem");
                inventorySystem = inventorySystemObj.AddComponent<InventorySystem>();
            }
            
            // Quest System
            QuestSystem questSystem = FindObjectOfType<QuestSystem>();
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
            
            // Örnek canavarlar
            CreateSampleMonsters();
            
            // Örnek NPC'ler
            CreateSampleNPCs();
            
            // Örnek envanter öğeleri
            CreateSampleItems();
            
            // Örnek yetenekler
            CreateSampleSkills();
            
            Debug.Log("✅ Örnek içerik oluşturuldu");
        }
        
        void CreateSampleMonsters()
        {
            GameObject monsterSpawner = new GameObject("MonsterSpawner");
            
            string[] monsterTypes = { "Goblin", "Orc", "Skeleton", "Wolf", "Troll" };
            Vector3[] spawnPositions = {
                new Vector3(10, 0, 5),
                new Vector3(-10, 0, 5),
                new Vector3(15, 0, -10),
                new Vector3(-15, 0, -10),
                new Vector3(0, 0, 20)
            };
            
            for (int i = 0; i < monsterTypes.Length; i++)
            {
                CreateMonster(monsterTypes[i], spawnPositions[i], monsterSpawner.transform);
            }
            
            // Boss
            CreateBoss("Dragon", new Vector3(0, 0, 40), monsterSpawner.transform);
        }
        
        void CreateMonster(string name, Vector3 position, Transform parent)
        {
            GameObject monster = GameObject.CreatePrimitive(PrimitiveType.Cube);
            monster.name = name;
            monster.transform.parent = parent;
            monster.transform.position = position;
            monster.transform.localScale = new Vector3(1, 2, 1);
            monster.layer = LayerMask.NameToLayer("Enemy");
            
            Material mat = new Material(Shader.Find("Standard"));
            Color monsterColor = name switch
            {
                "Goblin" => new Color(0.3f, 0.7f, 0.3f),
                "Orc" => new Color(0.4f, 0.6f, 0.3f),
                "Skeleton" => new Color(0.9f, 0.9f, 0.9f),
                "Wolf" => new Color(0.5f, 0.4f, 0.3f),
                "Troll" => new Color(0.2f, 0.5f, 0.3f),
                _ => Color.gray
            };
            mat.color = monsterColor;
            monster.GetComponent<Renderer>().material = mat;
            
            // Monster Controller
            MonsterController controller = monster.AddComponent<MonsterController>();
            controller.monsterName = name;
            controller.monsterType = MonsterType.Normal;
            controller.level = Random.Range(1, 5);
            controller.maxHealth = 30 + (controller.level * 10);
            controller.currentHealth = controller.maxHealth;
            controller.minDamage = 3 + controller.level;
            controller.maxDamage = 5 + controller.level;
            controller.experienceReward = 10 + (controller.level * 5);
            controller.goldRewardMin = 5 + controller.level;
            controller.goldRewardMax = 15 + controller.level;
            
            // Animator
            Animator animator = monster.AddComponent<Animator>();
            controller.animator = animator;
            
            // Sağlık barı
            CreateHealthBar(monster, controller);
        }
        
        void CreateBoss(string name, Vector3 position, Transform parent)
        {
            GameObject boss = GameObject.CreatePrimitive(PrimitiveType.Cube);
            boss.name = name;
            boss.transform.parent = parent;
            boss.transform.position = position;
            boss.transform.localScale = new Vector3(3, 5, 3);
            boss.layer = LayerMask.NameToLayer("Enemy");
            
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.8f, 0.1f, 0.1f);
            boss.GetComponent<Renderer>().material = mat;
            
            // Boss aura efekti
            GameObject aura = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            aura.name = "BossAura";
            aura.transform.parent = boss.transform;
            aura.transform.localScale = Vector3.one * 1.5f;
            aura.transform.localPosition = Vector3.zero;
            
            Material auraMat = new Material(Shader.Find("Standard"));
            auraMat.color = new Color(1f, 0, 0, 0.3f);
            auraMat.SetFloat("_Mode", 3);
            aura.GetComponent<Renderer>().material = auraMat;
            
            DestroyImmediate(aura.GetComponent<Collider>());
            
            // Monster Controller
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
            controller.detectionRange = 20f;
            controller.chaseRange = 30f;
            
            // Animator
            Animator animator = boss.AddComponent<Animator>();
            controller.animator = animator;
            
            // Sağlık barı
            CreateHealthBar(boss, controller);
            
            Debug.Log($"🗡️ Boss oluşturuldu: {name}");
        }
        
        void CreateSampleNPCs()
        {
            // Ticaret NPC
            CreateNPC("Merchant", "Tüccar", NPCType.Merchant, new Vector3(5, 0, -5), Color.yellow);
            
            // Görev NPC
            CreateNPC("QuestGiver", "Görev Verici", NPCType.QuestGiver, new Vector3(-5, 0, -5), Color.green);
            
            // Eğitmen
            CreateNPC("Trainer", "Eğitmen", NPCType.Trainer, new Vector3(0, 0, -8), Color.cyan);
            
            // Bankacı
            CreateNPC("Banker", "Bankacı", NPCType.Banker, new Vector3(8, 0, -5), Color.magenta);
        }
        
        void CreateNPC(string name, string displayName, NPCType type, Vector3 position, Color color)
        {
            GameObject npc = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            npc.name = name;
            npc.transform.position = position;
            npc.transform.localScale = new Vector3(1, 1.5f, 1);
            npc.layer = LayerMask.NameToLayer("NPC");
            
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = color;
            npc.GetComponent<Renderer>().material = mat;
            
            // NPC Controller
            NPCController controller = npc.AddComponent<NPCController>();
            controller.npcData = ScriptableObject.CreateInstance<NPCData>();
            controller.npcData.npcName = displayName;
            controller.npcData.npcType = type;
            
            // NPC tipine göre ayarlar
            switch (type)
            {
                case NPCType.Merchant:
                    controller.npcData.canTrade = true;
                    break;
                case NPCType.QuestGiver:
                    controller.npcData.canTrade = false;
                    break;
                case NPCType.Trainer:
                    controller.npcData.canTrade = false;
                    break;
                case NPCType.Banker:
                    controller.npcData.canTrade = false;
                    break;
            }
            
            // İsim etiketi
            CreateNPCNameTag(npc, displayName, color);
        }
        
        void CreateNPCNameTag(GameObject npc, string name, Color color)
        {
            GameObject nameTagCanvas = new GameObject("NameTagCanvas");
            nameTagCanvas.transform.parent = npc.transform;
            nameTagCanvas.transform.localPosition = Vector3.up * 2.5f;
            
            Canvas canvas = nameTagCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            
            GameObject nameTag = new GameObject("NameTag");
            nameTag.transform.parent = nameTagCanvas.transform;
            
            Text text = nameTag.AddComponent<Text>();
            text.text = name;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 14;
            text.fontStyle = FontStyle.Bold;
            text.color = color;
            text.alignment = TextAnchor.MiddleCenter;
            
            RectTransform rect = nameTag.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(150, 30);
        }
        
        void CreateSampleItems()
        {
            Debug.Log("🎒 Örnek eşyalar oluşturuluyor...");
            
            // Bu öğeler oyun başında envantere eklenebilir
            string[] itemNames = { "Health Potion", "Mana Potion", "Iron Sword", "Leather Armor", "Gold Ring" };
            int[] itemPrices = { 50, 75, 500, 300, 1000 };
            
            // Başlangıç envanteri
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.gold = 1000;
                Debug.Log("💰 Oyuncuya 1000 altın verildi");
            }
        }
        
        void CreateSampleSkills()
        {
            Debug.Log("⚔️ Örnek yetenekler oluşturuluyor...");
            
            SkillSystem skillSystem = FindObjectOfType<SkillSystem>();
            if (skillSystem != null)
            {
                // Yetenekler runtime'da eklenebilir
                Debug.Log("SkillSystem hazır");
            }
        }
        
        // Parçalı kurulum metodları
        void SetupUIOnly()
        {
            SetupUI();
            ConnectReferences();
            Debug.Log("✅ Sadece UI kuruldu!");
        }
        
        void SetupPlayerOnly()
        {
            SetupPlayer();
            Debug.Log("✅ Sadece Oyuncu kuruldu!");
        }
        
        void SetupEnvironmentOnly()
        {
            SetupEnvironment();
            Debug.Log("✅ Sadece Çevre kuruldu!");
        }
        
        void ClearScene()
        {
            bool confirm = EditorUtility.DisplayDialog("Sahneyi Temizle",
                "Sahnede kurulan tüm nesneleri silmek istediğinize emin misiniz?",
                "Evet, Sil", "İptal");
            
            if (!confirm) return;
            
            ClearSceneObjects();
            setupComplete = false;
            Debug.Log("🗑️ Sahne temizlendi!");
        }
    }
    
    // Bölge tetikleyicisi
    public class RegionTrigger : MonoBehaviour
    {
        public string regionName;
        public Color regionColor = Color.green;
        
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log($"🏰 {regionName} bölgesine girdiniz!");
            }
        }
    }
    
    // Zaman sistemi
    public class TimeSystem : MonoBehaviour
    {
        public float gameTime;
        public float dayLength = 600f; // 10 dakika = 1 gün
        
        void Update()
        {
            gameTime += Time.deltaTime;
            float dayProgress = (gameTime % dayLength) / dayLength;
            
            // Gün ışığını güncelle
            float sunAngle = dayProgress * 360f;
            Light sun = GameObject.Find("Directional Light")?.GetComponent<Light>();
            if (sun != null)
            {
                sun.transform.rotation = Quaternion.Euler(50f + sunAngle * 0.5f, -30f, 0f);
                
                // Gece/gündüz renk değişimi
                if (dayProgress < 0.25f || dayProgress > 0.75f)
                {
                    sun.intensity = 0.3f;
                    RenderSettings.ambientLight = new Color(0.1f, 0.1f, 0.2f);
                }
                else
                {
                    sun.intensity = 1.2f;
                    RenderSettings.ambientLight = new Color(0.4f, 0.45f, 0.55f);
                }
            }
        }
    }
    
    // Oyuncu giriş yönetimi
    public class PlayerInputManager : MonoBehaviour
    {
        void Update()
        {
            // Global input handling
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // Pause menu
            }
        }
    }
}
#endif
