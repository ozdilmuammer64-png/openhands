#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace KnightOnline
{
    public class AutoSetup : EditorWindow
    {
        [MenuItem("Tools/Knight Online 2D/Setup Game")]
        public static void ShowWindow()
        {
            GetWindow<AutoSetup>("Knight Online 2D");
        }
        
        void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.Label("🎮 Knight Online 2D", EditorStyles.boldLabel);
            GUILayout.Label("Tek tıkla kurulum", EditorStyles.miniLabel);
            GUILayout.Space(10);
            
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("🚀 OYUNU KUR", GUILayout.Height(50)))
            {
                SetupGame();
            }
            GUI.backgroundColor = Color.white;
            GUILayout.Space(5);
            
            if (GUILayout.Button("🗑️ Temizle"))
            {
                ClearScene();
            }
        }
        
        void SetupGame()
        {
            ClearScene();
            
            SetupCamera();
            SetupLight();
            SetupGround();
            SetupPlayer();
            SetupMonsters();
            SetupUI();
            
            Selection.activeGameObject = GameObject.Find("Player");
            EditorUtility.DisplayDialog("Tamamlandı!", "Oyun kuruldu! Play moduna geçin.", "Tamam");
            Debug.Log("✅ Oyun kuruldu!");
        }
        
        void SetupCamera()
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                GameObject go = new GameObject("Main Camera");
                cam = go.AddComponent<Camera>();
                go.tag = "MainCamera";
            }
            
            cam.transform.position = new Vector3(0, 0, -10);
            cam.orthographic = true;
            cam.orthographicSize = 5;
            cam.backgroundColor = new Color(0.5f, 0.7f, 1f);
            
            Debug.Log("✅ Kamera kuruldu (Orthographic)");
        }
        
        void SetupLight()
        {
            GameObject go = GameObject.Find("Directional Light");
            if (go == null)
            {
                go = new GameObject("Directional Light");
                Light l = go.AddComponent<Light>();
                l.type = LightType.Directional;
            }
            go.transform.rotation = Quaternion.Euler(50, -30, 0);
        }
        
        void SetupGround()
        {
            // Arkaplan
            GameObject bg = new GameObject("Background");
            SpriteRenderer bgSr = bg.AddComponent<SpriteRenderer>();
            bgSr.sprite = Sprite.Create(
                Texture2D.whiteTexture,
                new Rect(0, 0, 1, 1),
                new Vector2(0.5f, 0.5f)
            );
            bgSr.color = new Color(0.4f, 0.6f, 0.4f);
            bg.transform.localScale = new Vector3(50, 50, 1);
            bg.transform.position = Vector3.zero;
            bgSr.sortingOrder = -100;
            
            // Zemin çizgisi
            GameObject ground = new GameObject("Ground");
            SpriteRenderer sr = ground.AddComponent<SpriteRenderer>();
            sr.sprite = Sprite.Create(
                Texture2D.whiteTexture,
                new Rect(0, 0, 1, 1),
                new Vector2(0.5f, 0.5f)
            );
            sr.color = new Color(0.3f, 0.5f, 0.2f);
            ground.transform.localScale = new Vector3(50, 1, 1);
            ground.transform.position = new Vector3(0, -3, 0);
            sr.sortingOrder = -50;
            
            Debug.Log("✅ Zemin kuruldu");
        }
        
        void SetupPlayer()
        {
            GameObject player = new GameObject("Player");
            player.transform.position = new Vector3(0, 0, 0);
            
            // Sprite
            SpriteRenderer sr = player.AddComponent<SpriteRenderer>();
            sr.sprite = CreateColoredSprite(new Color(0.2f, 0.5f, 1f)); // Mavi
            sr.sortingOrder = 10;
            
            // Rigidbody
            Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.freezeRotation = true;
            
            // Collider
            BoxCollider2D col = player.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.8f, 1f);
            
            // Scripts
            player.AddComponent<PlayerController>();
            player.AddComponent<SkillSystem>();
            player.AddComponent<UIManager>();
            
            player.tag = "Player";
            
            Debug.Log("✅ Oyuncu kuruldu");
        }
        
        void SetupMonsters()
        {
            CreateMonster("Goblin", new Vector3(5, 0, 0), Color.green, 80, 10);
            CreateMonster("Orc", new Vector3(-5, 0, 0), new Color(0.4f, 0.6f, 0.3f), 100, 15);
            CreateMonster("Skeleton", new Vector3(8, 0, 0), Color.white, 60, 8);
            CreateMonster("Dragon", new Vector3(0, 5, 0), Color.red, 300, 30);
        }
        
        void CreateMonster(string name, Vector3 pos, Color color, int health, int damage)
        {
            GameObject go = new GameObject(name);
            go.transform.position = pos;
            
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CreateColoredSprite(color);
            sr.sortingOrder = 5;
            
            Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.freezeRotation = true;
            
            BoxCollider2D col = go.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.8f, 1f);
            
            MonsterController mc = go.AddComponent<MonsterController>();
            mc.monsterName = name;
            mc.maxHealth = health;
            mc.currentHealth = health;
            mc.attackDamage = damage;
            
            Debug.Log($"✅ {name} (Can: {health})");
        }
        
        void SetupUI()
        {
            // Canvas
            GameObject canvasGo = new GameObject("Canvas");
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasGo.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            // Health Bar Panel
            CreateHealthBar(canvasGo.transform);
            
            // Mana Bar Panel
            CreateManaBar(canvasGo.transform);
            
            // Skill Bar
            CreateSkillBar(canvasGo.transform);
            
            Debug.Log("✅ UI kuruldu");
        }
        
        void CreateHealthBar(Transform parent)
        {
            GameObject panel = new GameObject("HealthBarPanel");
            panel.transform.parent = parent;
            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(20, -20);
            rt.sizeDelta = new Vector2(250, 30);
            
            // Background
            GameObject bg = new GameObject("Background");
            bg.transform.parent = panel.transform;
            Image bgImg = bg.AddComponent<Image>();
            bgImg.color = Color.black;
            RectTransform bgRT = bg.GetComponent<RectTransform>();
            bgRT.anchorMin = Vector2.zero;
            bgRT.anchorMax = Vector2.one;
            bgRT.sizeDelta = Vector2.zero;
            
            // Fill
            GameObject fill = new GameObject("Fill");
            fill.transform.parent = panel.transform;
            Image fillImg = fill.AddComponent<Image>();
            fillImg.color = Color.red;
            RectTransform fillRT = fill.GetComponent<RectTransform>();
            fillRT.anchorMin = Vector2.zero;
            fillRT.anchorMax = new Vector2(1, 1);
            fillRT.sizeDelta = new Vector2(-4, -4);
            fillRT.anchoredPosition = Vector2.zero;
            
            // Text
            GameObject textGo = new GameObject("Text");
            textGo.transform.parent = panel.transform;
            textGo.AddComponent<TextMeshProUGUI>().text = "100/100";
            TextMeshProUGUI txt = textGo.GetComponent<TextMeshProUGUI>();
            txt.alignment = TextAlignmentOptions.Center;
            txt.color = Color.white;
            RectTransform txtRT = textGo.GetComponent<RectTransform>();
            txtRT.anchorMin = Vector2.zero;
            txtRT.anchorMax = Vector2.one;
            txtRT.sizeDelta = Vector2.zero;
        }
        
        void CreateManaBar(Transform parent)
        {
            GameObject panel = new GameObject("ManaBarPanel");
            panel.transform.parent = parent;
            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(20, -60);
            rt.sizeDelta = new Vector2(250, 20);
            
            // Background
            GameObject bg = new GameObject("Background");
            bg.transform.parent = panel.transform;
            Image bgImg = bg.AddComponent<Image>();
            bgImg.color = new Color(0, 0, 0, 0.5f);
            RectTransform bgRT = bg.GetComponent<RectTransform>();
            bgRT.anchorMin = Vector2.zero;
            bgRT.anchorMax = Vector2.one;
            bgRT.sizeDelta = Vector2.zero;
            
            // Fill
            GameObject fill = new GameObject("Fill");
            fill.transform.parent = panel.transform;
            Image fillImg = fill.AddComponent<Image>();
            fillImg.color = Color.blue;
            RectTransform fillRT = fill.GetComponent<RectTransform>();
            fillRT.anchorMin = Vector2.zero;
            fillRT.anchorMax = new Vector2(1, 1);
            fillRT.sizeDelta = new Vector2(-4, -4);
            fillRT.anchoredPosition = Vector2.zero;
            
            // Text
            GameObject textGo = new GameObject("Text");
            textGo.transform.parent = panel.transform;
            textGo.AddComponent<TextMeshProUGUI>().text = "100/100";
            TextMeshProUGUI txt = textGo.GetComponent<TextMeshProUGUI>();
            txt.alignment = TextAlignmentOptions.Center;
            txt.color = Color.white;
            RectTransform txtRT = textGo.GetComponent<RectTransform>();
            txtRT.anchorMin = Vector2.zero;
            txtRT.anchorMax = Vector2.one;
            txtRT.sizeDelta = Vector2.zero;
        }
        
        void CreateSkillBar(Transform parent)
        {
            GameObject panel = new GameObject("SkillBarPanel");
            panel.transform.parent = parent;
            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0);
            rt.anchorMax = new Vector2(0.5f, 0);
            rt.pivot = new Vector2(0.5f, 0);
            rt.anchoredPosition = new Vector2(0, 20);
            rt.sizeDelta = new Vector2(350, 60);
            
            // 5 skill slot oluştur
            for (int i = 0; i < 5; i++)
            {
                GameObject slot = new GameObject($"Skill{i + 1}");
                slot.transform.parent = panel.transform;
                RectTransform slotRT = slot.AddComponent<RectTransform>();
                slotRT.anchorMin = new Vector2(0, 0);
                slotRT.anchorMax = new Vector2(0, 0);
                slotRT.pivot = new Vector2(0, 0);
                slotRT.anchoredPosition = new Vector2(i * 70, 0);
                slotRT.sizeDelta = new Vector2(60, 60);
                
                // Background
                GameObject bg = new GameObject("BG");
                bg.transform.parent = slot.transform;
                Image bgImg = bg.AddComponent<Image>();
                bgImg.color = new Color(0.2f, 0.2f, 0.2f);
                RectTransform bgRT = bg.GetComponent<RectTransform>();
                bgRT.anchorMin = Vector2.zero;
                bgRT.anchorMax = Vector2.one;
                bgRT.sizeDelta = Vector2.zero;
                
                // Hotkey text
                GameObject keyText = new GameObject("Hotkey");
                keyText.transform.parent = slot.transform;
                keyText.AddComponent<TextMeshProUGUI>().text = (i + 1).ToString();
                TextMeshProUGUI keyTxt = keyText.GetComponent<TextMeshProUGUI>();
                keyTxt.alignment = TextAlignmentOptions.BottomRight;
                keyTxt.fontSize = 14;
                keyTxt.color = Color.white;
                RectTransform keyRT = keyText.GetComponent<RectTransform>();
                keyRT.anchorMin = Vector2.zero;
                keyRT.anchorMax = Vector2.one;
                keyRT.sizeDelta = new Vector2(-5, -5);
            }
        }
        
        Sprite CreateColoredSprite(Color color)
        {
            Texture2D tex = new Texture2D(64, 64);
            Color[] colors = new Color[64 * 64];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = color;
            tex.SetPixels(colors);
            tex.Apply();
            
            return Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
        }
        
        void ClearScene()
        {
            string[] names = { "Player", "Goblin", "Orc", "Skeleton", "Dragon", "Ground", "Background", "Canvas", "Main Camera" };
            foreach (string n in names)
            {
                GameObject go = GameObject.Find(n);
                if (go != null) DestroyImmediate(go);
            }
            
            foreach (MonsterController m in FindObjectsOfType<MonsterController>())
                DestroyImmediate(m.gameObject);
            
            Debug.Log("🗑️ Sahne temizlendi");
        }
    }
}
#endif
