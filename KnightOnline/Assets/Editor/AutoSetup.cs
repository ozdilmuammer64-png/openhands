#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

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
            cam.orthographicSize = 8;
            cam.backgroundColor = new Color(0.5f, 0.7f, 1f);
            
            // CameraFollow ekle
            CameraFollow cf = cam.GetComponent<CameraFollow>();
            if (cf == null) cf = cam.gameObject.AddComponent<CameraFollow>();
            cf.offset = new Vector3(0, 0, -10);
            
            Debug.Log("✅ Kamera kuruldu");
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
            GameObject bg = new GameObject("Background");
            SpriteRenderer bgSr = bg.AddComponent<SpriteRenderer>();
            bgSr.sprite = CreateColoredSprite(new Color(0.4f, 0.6f, 0.4f));
            bg.transform.localScale = new Vector3(100, 100, 1);
            bg.transform.position = Vector3.zero;
            bgSr.sortingOrder = -100;
            
            Debug.Log("✅ Zemin kuruldu");
        }
        
        void SetupPlayer()
        {
            GameObject player = new GameObject("Player");
            player.transform.position = new Vector3(0, 0, 0);
            
            SpriteRenderer sr = player.AddComponent<SpriteRenderer>();
            sr.sprite = CreateColoredSprite(new Color(0.2f, 0.5f, 1f));
            sr.sortingOrder = 10;
            
            Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.freezeRotation = true;
            
            BoxCollider2D col = player.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.8f, 1f);
            
            player.AddComponent<PlayerController>();
            player.AddComponent<SkillSystem>();
            
            player.tag = "Player";
            
            Debug.Log("✅ Oyuncu kuruldu");
        }
        
        void SetupMonsters()
        {
            CreateMonster("Goblin", new Vector3(8, 0, 0), Color.green, 80, 10);
            CreateMonster("Orc", new Vector3(-8, 0, 0), new Color(0.4f, 0.6f, 0.3f), 100, 15);
            CreateMonster("Skeleton", new Vector3(12, 0, 0), Color.white, 60, 8);
            CreateMonster("Dragon", new Vector3(0, 8, 0), Color.red, 300, 30);
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
            
            Debug.Log($"✅ {name}");
        }
        
        void SetupUI()
        {
            // Canvas
            GameObject canvasObj = new GameObject("Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // UIManager
            UIManager ui = canvasObj.AddComponent<UIManager>();
            
            // === HEALTH BAR ===
            CreateHealthBar(canvasObj.transform, ui);
            
            // === MANA BAR ===
            CreateManaBar(canvasObj.transform, ui);
            
            // === SKILL BAR ===
            CreateSkillBar(canvasObj.transform, ui);
            
            Debug.Log("✅ UI kuruldu");
        }
        
        void CreateHealthBar(Transform parent, UIManager ui)
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
            GameObject bg = CreateUIObject("BG", panel.transform);
            Image bgImg = bg.AddComponent<Image>();
            bgImg.color = new Color(0, 0, 0, 0.5f);
            RectTransform bgRT = bg.GetComponent<RectTransform>();
            bgRT.anchorMin = Vector2.zero;
            bgRT.anchorMax = Vector2.one;
            bgRT.sizeDelta = Vector2.zero;
            
            // Fill
            GameObject fillObj = CreateUIObject("Fill", panel.transform);
            Image fillImg = fillObj.AddComponent<Image>();
            fillImg.color = Color.red;
            RectTransform fillRT = fillObj.GetComponent<RectTransform>();
            fillRT.anchorMin = Vector2.zero;
            fillRT.anchorMax = new Vector2(1, 1);
            fillRT.sizeDelta = new Vector2(-4, -4);
            fillRT.anchoredPosition = Vector2.zero;
            
            // Text
            GameObject txtObj = CreateUIObject("Text", panel.transform);
            Text txt = txtObj.AddComponent<Text>();
            txt.text = "100/100";
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
            txt.fontStyle = FontStyle.Bold;
            RectTransform txtRT = txtObj.GetComponent<RectTransform>();
            txtRT.anchorMin = Vector2.zero;
            txtRT.anchorMax = Vector2.one;
            txtRT.sizeDelta = Vector2.zero;
            
            ui.healthFill = fillImg;
            ui.healthText = txt;
        }
        
        void CreateManaBar(Transform parent, UIManager ui)
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
            GameObject bg = CreateUIObject("BG", panel.transform);
            Image bgImg = bg.AddComponent<Image>();
            bgImg.color = new Color(0, 0, 0, 0.5f);
            RectTransform bgRT = bg.GetComponent<RectTransform>();
            bgRT.anchorMin = Vector2.zero;
            bgRT.anchorMax = Vector2.one;
            bgRT.sizeDelta = Vector2.zero;
            
            // Fill
            GameObject fillObj = CreateUIObject("Fill", panel.transform);
            Image fillImg = fillObj.AddComponent<Image>();
            fillImg.color = Color.blue;
            RectTransform fillRT = fillObj.GetComponent<RectTransform>();
            fillRT.anchorMin = Vector2.zero;
            fillRT.anchorMax = new Vector2(1, 1);
            fillRT.sizeDelta = new Vector2(-4, -4);
            fillRT.anchoredPosition = Vector2.zero;
            
            // Text
            GameObject txtObj = CreateUIObject("Text", panel.transform);
            Text txt = txtObj.AddComponent<Text>();
            txt.text = "100/100";
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
            txt.fontStyle = FontStyle.Bold;
            RectTransform txtRT = txtObj.GetComponent<RectTransform>();
            txtRT.anchorMin = Vector2.zero;
            txtRT.anchorMax = Vector2.one;
            txtRT.sizeDelta = Vector2.zero;
            
            ui.manaFill = fillImg;
            ui.manaText = txt;
        }
        
        void CreateSkillBar(Transform parent, UIManager ui)
        {
            GameObject panel = new GameObject("SkillBarPanel");
            panel.transform.parent = parent;
            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0);
            rt.anchorMax = new Vector2(0.5f, 0);
            rt.pivot = new Vector2(0.5f, 0);
            rt.anchoredPosition = new Vector2(0, 20);
            rt.sizeDelta = new Vector2(400, 80);
            
            // Arrays for skill UI
            Image[] hotkeyBGs = new Image[5];
            Text[] hotkeyTexts = new Text[5];
            Text[] nameTexts = new Text[5];
            
            for (int i = 0; i < 5; i++)
            {
                GameObject slot = CreateUIObject($"Skill{i + 1}", panel.transform);
                RectTransform slotRT = slot.GetComponent<RectTransform>();
                slotRT.anchorMin = new Vector2(0, 0);
                slotRT.anchorMax = new Vector2(0, 0);
                slotRT.pivot = new Vector2(0, 0);
                slotRT.anchoredPosition = new Vector2(i * 80, 0);
                slotRT.sizeDelta = new Vector2(70, 70);
                
                // Background
                Image bg = slot.AddComponent<Image>();
                bg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
                hotkeyBGs[i] = bg;
                
                // Hotkey text
                GameObject keyTxt = CreateUIObject("Hotkey", slot.transform);
                Text kt = keyTxt.AddComponent<Text>();
                kt.text = (i + 1).ToString();
                kt.fontSize = 16;
                kt.alignment = TextAnchor.MiddleCenter;
                kt.color = Color.white;
                RectTransform ktRT = keyTxt.GetComponent<RectTransform>();
                ktRT.anchorMin = Vector2.zero;
                ktRT.anchorMax = Vector2.one;
                ktRT.sizeDelta = Vector2.zero;
                hotkeyTexts[i] = kt;
                
                // Skill name
                GameObject nameTxt = CreateUIObject("Name", slot.transform);
                Text nt = nameTxt.AddComponent<Text>();
                nt.text = "Skill";
                nt.fontSize = 10;
                nt.alignment = TextAnchor.LowerCenter;
                nt.color = Color.yellow;
                RectTransform ntRT = nameTxt.GetComponent<RectTransform>();
                ntRT.anchorMin = new Vector2(0, 0);
                ntRT.anchorMax = new Vector2(1, 0);
                ntRT.sizeDelta = new Vector2(0, 20);
                ntRT.anchoredPosition = new Vector2(0, 5);
                nameTexts[i] = nt;
            }
            
            ui.skillHotkeys = hotkeyTexts;
            ui.skillNames = nameTexts;
        }
        
        GameObject CreateUIObject(string name, Transform parent)
        {
            GameObject go = new GameObject(name);
            go.transform.parent = parent;
            go.AddComponent<RectTransform>();
            return go;
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
            string[] names = { "Player", "Goblin", "Orc", "Skeleton", "Dragon", "Ground", "Background", "Main Camera", "Canvas" };
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
