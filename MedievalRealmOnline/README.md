# 🏰 Medieval Realm Online - Unity MMORPG

## ⚡ HIZLI KURULUM

Bu proje Unity 2022.3.10f1 içindir. Tüm scriptler hazır!

### Adım 1: Projeyi Aç
1. Unity Hub'ı açın
2. **Open** → `MedievalRealmOnline` klasörünü seçin

### Adım 2: Sahneleri Oluştur
Unity'de şunları yapın:

```
1. File → New Scene → "MainMenu" olarak kaydet
2. File → New Scene → "GameScene" olarak kaydet
```

### Adım 3: GameScene'i Ayarla

**Zorunlu Adımlar:**

1. **Player Objesi:**
   - 3D Object → Capsule oluşturun
   - Tag: Player yapın
   - Component ekle: CharacterController
   - Script ekle: `PlayerController.cs`

2. **Kamika:**
   - Main Camera'ya `CameraController.cs` ekleyin
   - Target: Player

3. **Yüzey:**
   - 3D Object → Plane oluşturun (100x100 boyutunda)
   - Üzerine NavMesh Bake edin (Window → AI → Navigation)

4. **GameManager:**
   - Boş obje oluşturun → `GameManager.cs` ekleyin

5. **UI Canvas:**
   - GameScene'de Canvas oluşturun
   - Yukarıdaki UI yapısını kurun (detaylar aşağıda)

### Adım 4: Build
1. File → Build Settings
2. MainMenu ve GameScene'i ekleyin
3. **Build And Run**

---

## Detaylı Kurulum Rehberi

### Adım 1: Unity Proje Dosyalarını Açma

1. Unity Hub'ı açın
2. **Open** butonuna tıklayın
3. `MedievalRealmOnline` klasörünü seçin
4. Unity editör açılacak (2022.3.10f1 versiyonu)

### Adım 2: Sahne Oluşturma

Unity'de şu sahneleri oluşturun:

#### 2.1 MainMenu Sahnesi
1. **File > New Scene** ile yeni sahne oluşturun
2. **MainMenu** olarak kaydedin

#### 2.2 GameScene Sahnesi
1. **File > New Scene** ile yeni sahne oluşturun
2. **GameScene** olarak kaydedin

### Adım 3: MainMenu Sahne Kurulumu

```
MainMenu Sahnesi Hiyerarşisi:
─────────────────────────────
─ Main Camera
─ Canvas
│   └─ MainMenuPanel
│       ├─ TitleText (TextMeshPro)
│       ├─ NewGameButton
│       ├─ ContinueButton
│       ├─ OptionsButton
│       └─ QuitButton
─ EventSystem
```

**Canvas Ayarları:**
- Render Mode: Screen Space - Overlay
- Canvas Scaler: Scale With Screen Size

### Adım 4: GameScene Sahne Kurulumu

```
GameScene Sahne Hiyerarşisi:
─────────────────────────────
─ Main Camera (tag: MainCamera)
│   └─ CameraController.cs
─ Directional Light
─ Ground (Plane, boyut: 100x100)
─ Player (küp veya karakter modeli)
│   ├─ PlayerController.cs
│   ├─ CharacterController
│   ├─ Animator
│   └─ Tag: Player
─ GameManager (boş obje)
│   └─ GameManager.cs
─ UIManager (boş obje)
│   └─ UIManager.cs
─ AudioManager (boş obje)
│   └─ AudioManager.cs
─ LootManager (boş obje)
│   └─ LootManager.cs
─ InventoryManager (boş obje)
│   └─ InventoryManager.cs

— EnemySpawnPoints —
│   ├─ EnemySpawn1 (Enemies/Slime)
│   ├─ EnemySpawn2 (Enemies/Goblin)
│   └─ EnemySpawn3 (Enemies/Skeleton)

— NPCs —
│   ├─ Merchant (NPC/ShopKeeper)
│   ├─ Healer (NPC)
│   └─ GuildMaster (NPC)
```

### Adım 5: Player Hazırlığı

1. **3D Object > Capsule** oluşturun (veya karakter modeli)
2. **Tag**: Player olarak ayarlayın
3. **Layer**: Create Layer "Player" olarak ekleyin

**Player Inspector Ayarları:**
```
Player GameObject:
├── Transform
│   Position: (0, 1, 0)
│   Scale: (1, 1, 1)
├── Character Controller
│   Height: 2
│   Radius: 0.5
│   Center: (0, 1, 0)
├── Animator (Animator component olmalı)
├── PlayerController.cs (script)
│   ├── Move Speed: 5
│   ├── Sprint Speed: 8
│   ├── Attack Range: 2
│   ├── Enemy Layer: Enemy
│   ├── Health Bar Fill: [UI Image]
│   ├── Mana Bar Fill: [UI Image]
│   └── Health/Mana Text: [UI Text]
```

### Adım 6: Enemy Hazırlığı

1. **3D Object > Capsule** oluşturun
2. **Layer**: Create Layer "Enemy" olarak ekleyin
3. Layer'ı Enemy yapın

**Enemy Inspector Ayarları:**
```
Enemy GameObject:
├── Transform
├── Nav Mesh Agent
│   Speed: 3
│   Stopping Distance: 1.5
├── Animator
├── EnemyController.cs (script)
│   ├── Enemy Name: "Slime"
│   ├── Enemy Level: 1
│   ├── Max Health: 30
│   ├── Attack Damage: 5
│   ├── XP Reward: 15
│   ├── Gold Reward: 5
│   └── Health Bar Fill: [UI Image]
└── Canvas (Health Bar için)
    └── HealthBarFill [UI Image]
```

### Adım 7: UI Canvas Kurulumu

**GameScene Canvas:**
```
Canvas (Screen Space - Overlay)
├── HealthBarPanel (SOL ÜST)
│   ├── HealthBarBackground (Image)
│   │   └── HealthBarFill (Image)
│   └── HealthText (TextMeshPro)
├── ManaBarPanel
│   ├── ManaBarBackground
│   │   └── ManaBarFill
│   └── ManaText
├── XPBarPanel (SAĞ ÜST)
│   ├── XPBarBackground
│   │   └── XPBarFill
│   ├── LevelText
│   └── XPText
├── GoldDisplay (SAĞ ÜST)
│   └── GoldText
├── SkillBar (ALT ORTA)
│   ├── SkillSlot1 (Image + Text)
│   ├── SkillSlot2
│   ├── SkillSlot3
│   └── SkillSlot4
├── Minimap (SOL ALT)
│   └── RawImage
└── NotificationArea (SAĞ ÜST KÖŞE)
    └── Container for notifications
```

### Adım 8: Build Settings

1. **File > Build Settings** açın
2. Sahneleri ekleyin:
   - MainMenu (index 0)
   - GameScene (index 1)
3. Platform: PC, Mac & Linux Standalone
4. **Build And Run**

## 🎮 Kontroller

| Tuş | Aksiyon |
|-----|---------|
| WASD | Hareket |
| Sol Tık | Saldırı |
| 1-4 | Skilller |
| E | Eşya al |
| I | Envanter |
| ESC | Menü |

## 📁 Proje Yapısı

```
MedievalRealmOnline/
├── Assets/
│   ├── Scripts/
│   │   ├── Game/
│   │   │   ├── GameManager.cs
│   │   │   ├── GameData.cs
│   │   │   ├── SaveSystem.cs
│   │   │   ├── DamageNumber.cs
│   │   │   └── CameraController.cs
│   │   ├── Player/
│   │   │   ├── PlayerController.cs
│   │   │   ├── PlayerMovement.cs
│   │   │   └── CharacterCreator.cs
│   │   ├── Enemy/
│   │   │   └── EnemyController.cs
│   │   ├── UI/
│   │   │   ├── UIManager.cs
│   │   │   └── MainMenu.cs
│   │   ├── Items/
│   │   │   ├── LootManager.cs
│   │   │   └── ItemPickup.cs
│   │   ├── NPC/
│   │   │   └── NPCController.cs
│   │   └── Audio/
│   │       └── AudioManager.cs
│   ├── Prefabs/
│   ├── Scenes/
│   └── Sprites/
└── ProjectSettings/
```

## 🔧 Yapılacaklar Listesi

- [x] Ana menü sistemi
- [x] Karakter oluşturma
- [x] Oyuncu kontrolörü
- [x] Düşman AI
- [x] UI sistemi
- [x] Kayıt sistemi
- [ ] Animasyonlar (Animator Controller)
- [ ] Ses efektleri
- [ ] Daha fazla düşman türü
- [ ] Lonca sistemi
- [ ] Görev sistemi

## ⚠️ Önemli Notlar

1. **TextMeshPro**: TextMeshPro paketini Window > Package Manager'dan yükleyin
2. **NavMesh**: Window > AI > Navigation'dan NavMesh bake edin
3. **Layer Mask**: Player ve Enemy layer'ları oluşturun
4. **Physics Layer**: Collision matrix'i ayarlayın

## 🚀 İlk Test

1. Play butonuna basın
2. Ana menüde "Yeni Oyun" tıklayın
3. Karakter oluşturun
4. WASD ile hareket edin
5. Düşmanlara tıklayarak saldırın
6. 1-4 tuşları ile skill kullanın

---

**Kolay gelsin! ⚔️**