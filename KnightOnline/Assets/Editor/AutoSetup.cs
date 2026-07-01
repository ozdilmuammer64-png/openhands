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
            player.AddComponent<UIManager>();
            
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
            string[] names = { "Player", "Goblin", "Orc", "Skeleton", "Dragon", "Ground", "Background", "Main Camera" };
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
