# Knight Online Benzeri Oyun - Proje Manifestosu

## 📋 Proje Özeti
Bu proje Unity 2022.3 LTS ile geliştirilen, Knight Online tarzı bir Action MMORPG oyunudur.

## 📁 Oluşturulan Dosyalar

### Script Dosyaları (13 adet, ~4600+ satır kod)

| Script | Açıklama | Satır |
|--------|----------|-------|
| PlayerController.cs | Oyuncu kontrolü, sınıflar, istatistikler | ~500 |
| MonsterController.cs | Canavar AI, patrol, chase, attack | ~450 |
| NPCController.cs | NPC sistemi, diyaloglar | ~250 |
| SkillSystem.cs | Yetenek sistemi, cooldown, efektler | ~400 |
| InventorySystem.cs | Envanter, ekipman, item yönetimi | ~450 |
| QuestSystem.cs | Görev sistemi, objectives | ~400 |
| UIManager.cs | UI yönetimi, HUD, paneller | ~500 |
| SaveSystem.cs | Kayıt sistemi, otomatik kayıt | ~450 |
| AudioSystem.cs | Ses sistemi, müzik, efektler | ~250 |
| CameraController.cs | Kamera kontrolü, zoom, shake | ~200 |
| GameSettings.cs | Oyun ayarları | ~100 |
| IDamageable.cs | Hasar alma arayüzü | ~10 |
| ItemPickup.cs | Eşya toplama | ~150 |

### Editor Script
| Script | Açıklama |
|--------|----------|
| AutoSetup.cs | Tek tıkla sahne, obje, UI ve referans kuran ana script |

## 🎮 Özellikler

### ✅ Tamamlanan Özellikler
- [x] Oyuncu kontrol sistemi (WASD + Mouse)
- [x] 4 oyuncu sınıfı (Warrior, Mage, Rogue, Priest)
- [x] Seviye ve deneyim sistemi
- [x] Can, mana, stamina yönetimi
- [x] Canavar AI sistemi (Idle, Patrol, Chase, Attack)
- [x] Canavar türleri (Normal, Elite, Boss, Guardian)
- [x] Yetenek sistemi (10 slot, cooldown)
- [x] Envanter sistemi (24 slot, ekipman)
- [x] Nadirlik sistemi (Common - Legendary)
- [x] Görev sistemi (Kill, Collect, Explore, Talk)
- [x] NPC sistemi (Merchant, QuestGiver, Trainer, Banker)
- [x] UI sistemi (HUD, minimap, sağlık barı)
- [x] Kayıt sistemi (Auto-save, manual save)
- [x] Ses sistemi (Müzik, efektler)
- [x] Kamera sistemi (Zoom, shake, follow)
- [x] Otomatik hedefleme (Tab tuşu)

### 🔧 Editor Script Özellikleri
- [x] Tek tıkla tam kurulum
- [x] Sadece UI kurulumu
- [x] Sadece Oyuncu kurulumu
- [x] Sadece Çevre kurulumu
- [x] Referans otomatik bağlama
- [x] Sahne temizleme

## 🚀 Kurulum Adımları

1. Unity 2022.3 LTS açın
2. `KnightOnline` klasörünü seçin
3. **Tools > Knight Online > Setup Game** açın
4. **"OYUNU KUR (TAM KURULUM)"** tıklayın
5. Play tuşuna basın!

## 🎯 Kontroller

```
Hareket:        WASD
Bakış:          Mouse
Koşma:          Left Shift
Zıplama:        Space
Saldırı:        Sol Mouse
Yetenek:        Sağ Mouse
Hedef Kilit:    Tab
Etkileşim:      E
Envanter:       I
Karakter:       C
Yetenekler:     K
Görevler:       Q
Harita:         M
Kayıt:          F5
Yükle:          F9
Ayarlar:        ESC
```

## 📊 Proje İstatistikleri

- **Toplam Script:** 13
- **Toplam Kod Satırı:** ~4600+
- **Editor Script:** 1 (Ana kurulum scripti)
- **Prefabs:** Hazır (AutoSetup ile oluşturulacak)
- **UI Elementleri:** Hazır (AutoSetup ile oluşturulacak)

## 🎨 Renk Kodlaması (Nadirlik)

| Nadirlik | Renk | Hex |
|----------|------|-----|
| Common | Beyaz | #FFFFFF |
| Uncommon | Yeşil | #00FF00 |
| Rare | Mavi | #0077FF |
| Epic | Mor | #9900FF |
| Legendary | Turuncu | #FF8000 |

## 📝 Sonraki Adımlar

1. Animasyonları ekleyin (Animator Controller)
2. Sprite/texture dosyalarını ekleyin
3. Harita tasarımını yapın
4. Ses dosyalarını ekleyin
5. Test senaryoları oluşturun

---

**Geliştirme Tarihi:** 2026-07-01  
**Unity Sürümü:** 2022.3 LTS  
**Script Dili:** C#  
**Toplam Geliştirme Süresi:** ~2 saat
