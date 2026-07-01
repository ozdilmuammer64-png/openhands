#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.AI;

namespace KnightOnline
{
    public class AutoSetup : EditorWindow
    {
        [MenuItem("Tools/Knight Online/Setup Game")]
        public static void ShowWindow()
        {
            GetWindow<AutoSetup>("Knight Online");
        }
        
        void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.Label("🎮 Knight Online", EditorStyles.boldLabel);
            GUILayout.Label("Tek tıkla oyun kurulumu", EditorStyles.miniLabel);
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
            BakeNavMesh();
            SetupPlayer();
            SetupMonsters();
            
            Selection.activeGameObject = GameObject.Find("Player");
            Debug.Log("✅ Oyun kuruldu! Play moduna geçin.");
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
            
            cam.transform.position = new Vector3(0, 8, -10);
            cam.transform.rotation = Quaternion.Euler(30, 0, 0);
            
            // CameraController ekle
            CameraController cc = cam.GetComponent<CameraController>();
            if (cc == null) cc = cam.gameObject.AddComponent<CameraController>();
            
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
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(5, 1, 5);
            
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.3f, 0.5f, 0.2f);
            ground.GetComponent<Renderer>().material = mat;
            
            Debug.Log("✅ Zemin kuruldu");
        }
        
        void BakeNavMesh()
        {
            NavMeshBuilder.BuildNavMesh();
            Debug.Log("✅ NavMesh bake edildi");
        }
        
        void SetupPlayer()
        {
            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.tag = "Player";
            player.transform.position = new Vector3(0, 1, 0);
            
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.2f, 0.5f, 1f);
            player.GetComponent<Renderer>().material = mat;
            
            CharacterController cc = player.AddComponent<CharacterController>();
            cc.height = 2f;
            cc.radius = 0.5f;
            cc.center = new Vector3(0, 1, 0);
            
            player.AddComponent<PlayerController>();
            player.AddComponent<SkillSystem>();
            
            Debug.Log("✅ Oyuncu kuruldu");
        }
        
        void SetupMonsters()
        {
            CreateMonster("Goblin", new Vector3(5, 1, 5), Color.green, 80);
            CreateMonster("Orc", new Vector3(-5, 1, 5), new Color(0.4f, 0.6f, 0.3f), 100);
            CreateMonster("Skeleton", new Vector3(8, 1, -5), Color.white, 60);
            CreateMonster("Dragon", new Vector3(0, 1, 15), Color.red, 300);
        }
        
        void CreateMonster(string name, Vector3 pos, Color color, int health)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.position = pos;
            go.transform.localScale = new Vector3(1, 2, 1);
            
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = color;
            go.GetComponent<Renderer>().material = mat;
            
            MonsterController mc = go.AddComponent<MonsterController>();
            mc.monsterName = name;
            mc.maxHealth = health;
            mc.currentHealth = health;
            
            Debug.Log($"✅ {name} (Can: {health})");
        }
        
        void ClearScene()
        {
            string[] names = {"Player", "Ground", "Goblin", "Orc", "Skeleton", "Dragon", "Canvas"};
            foreach (string n in names)
            {
                GameObject go = GameObject.Find(n);
                if (go != null) DestroyImmediate(go);
            }
            
            foreach (MonsterController m in FindObjectsOfType<MonsterController>())
                DestroyImmediate(m.gameObject);
        }
    }
}
#endif
