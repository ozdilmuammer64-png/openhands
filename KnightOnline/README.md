# 🎮 Knight Online Benzeri Oyun - Unity Projesi

## 📖 Proje Hakkında

Bu proje, Unity 2022.3 LTS kullanılarak geliştirilen Knight Online tarzı bir Action MMORPG oyunudur. Tüm scriptler ve sistemler sıfırdan yazılmıştır.

## 🚀 Kurulum

### Unity'de Açma

1. Unity 2022.3 LTS veya üstü sürümünü indirin
2. Unity Hub'ı açın
3. "Open" butonuna tıklayın
4. `KnightOnline` klasörünü seçin
5. Projeyi açın

### Otomatik Sahne Kurulumu

1. Unity açıldıktan sonra menüden: **Tools > Knight Online > Setup Game** seçin
2. Açılan pencerede **"OYUNU KUR (TAM KURULUM)"** butonuna tıklayın
3. Tüm sahne, UI, objeler ve referanslar otomatik olarak kurulacak!

## 🎮 Kontroller

### Hareket
| Tuş | Aksiyon |
|-----|---------|
| WASD | Hareket |
| Mouse | Bakış Yönü |
| Sol Shift | Koşma |
| Space | Zıplama |
| Tab | Hedef Kilitme |
| E | Etkileşim |

### Saldırı
| Tuş | Aksiyon |
|-----|---------|
| Sol Mouse | Normal Saldırı |
| Sağ Mouse | Yetenek Saldırısı |
| 1-9 | Yetenekler |

### UI
| Tuş | Panel |
|-----|-------|
| I | Envanter |
| C | Karakter |
| K | Yetenekler |
| Q | Görevler |
| M | Harita |
| ESC | Ayarlar / Kapatma |

## 📁 Proje Yapısı

```
KnightOnline/
├── Assets/
│   ├── Scripts/           # Tüm oyun scriptleri
│   │   ├── PlayerController.cs      # Oyuncu kontrolü
│   │   ├── MonsterController.cs     # Canavar AI
│   │   ├── NPCController.cs        # NPC sistemi
│   │   ├── SkillSystem.cs          # Yetenek sistemi
│   │   ├── InventorySystem.cs      # Envanter sistemi
│   │   ├── QuestSystem.cs          # Görev sistemi
│   │   ├── UIManager.cs            # UI yönetimi
│   │   ├── SaveSystem.cs           # Kayıt sistemi
│   │   ├── AudioSystem.cs          # Ses sistemi
│   │   ├── CameraController.cs     # Kamera kontrolü
│   │   └── ...
│   ├── Editor/             # Editor scriptleri
│   │   └── AutoSetup.cs    # Otomatik kurulum scripti
│   ├── Prefabs/           # Prefab dosyaları
│   ├── Scenes/            # Sahne dosyaları
│   └── UI/                # UI elemanları
└── ProjectSettings/       # Unity proje ayarları
```

## ⚔️ Oyun Özellikleri

### Oyuncu Sistemi
- 4 sınıf (Warrior, Mage, Rogue, Priest)
- Seviye sistemi ve deneyim
- Can, mana, stamina yönetimi
- Hasar hesaplama ve kritik vuruşlar
- Otomatik hedefleme sistemi

### Canavar Sistemi
- Normal, Elite, Boss ve Guardian tipi canavarlar
- AI tabanlı patrol ve chase sistemi
- Canavar türlerine göre istatistikler
- Yeniden doğma sistemi

### Yetenek Sistemi
- 10 slot yetenek barı
- Yetenek cooldown sistemi
- Yetenek türleri (Passive, Active, Buff, Debuff)
- Mana ve stamina maliyeti

### Envanter Sistemi
- 24 slot envanter
- Ekipman sistemi
- Nadirlik sistemi (Common, Uncommon, Rare, Epic, Legendary)
- Item stacking
- Alışveriş sistemi

### Görev Sistemi
- Görev türleri (Kill, Collect, Explore, Escort, Talk)
- Görev takip sistemi
- Ödül sistemi

### UI Sistemi
- HUD (Sağlık, mana, EXP barı)
- Minimap
- Hedef bilgi paneli
- Bildirim sistemi
- Ölüm ve yeniden doğma ekranı

### Kayıt Sistemi
- Otomatik kayıt
- Manuel kayıt (F5)
- Çoklu kayıt desteği
- Ayarları kaydetme

## 🔧 Editor Script Kullanımı

### Tam Kurulum
```
Tools > Knight Online > Setup Game > "OYUNU KUR (TAM KURULUM)"
```

### Parçalı Kurulum
```
Tools > Knight Online > Setup Game > "Sadece UI"  - Sadece UI
Tools > Knight Online > Setup Game > "Sadece Oyuncu" - Sadece oyuncu
Tools > Knight Online > Setup Game > "Sadece Çevre" - Sadece çevre
```

### Sahneyi Sıfırlama
```
Tools > Knight Online > Setup Game > "Temizle"
```

## 📝 Script API Örnekleri

### Oyuncu Hasar Alma
```csharp
IDamageable damageable = target.GetComponent<IDamageable>();
if (damageable != null)
{
    damageable.TakeDamage(100);
}
```

### Envantere Eşya Ekleme
```csharp
InventorySystem.Instance.AddItem(itemData, quantity);
```

### Yetenek Kullanma
```csharp
SkillSystem.Instance.CastSkill(slotIndex);
```

### Görev İlerletme
```csharp
QuestSystem.Instance.UpdateKillProgress("Goblin");
```

## 🎯 Sonraki Geliştirmeler

- [ ] Animasyon kontrolleri
- [ ] Harita sistemi
- [ ] Guild sistemi
- [ ] PvP sistemi
- [ ] Daha fazla canavar türü
- [ ] Dükkan UI'ı
- [ ] Banka UI'ı
- [ ] Sezgisel minimap

## 📜 Lisans

Bu proje eğitim amaçlı oluşturulmuştur. Ticari kullanım için geliştirilmeye ihtiyaç duyar.

## 🤝 Katkıda Bulunma

1. Fork yapın
2. Feature branch oluşturun
3. Değişikliklerinizi commit edin
4. Pull request açın

---

**Geliştirici:** OpenHands Agent  
**Tarih:** 2026
