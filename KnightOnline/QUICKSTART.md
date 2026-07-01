# HIZLI BAŞLANGIÇ REHBERİ
## Knight Online Benzeri Oyun

---

## 🎯 ADIM 1: Unity'i Açın

1. Unity 2022.3 LTS veya üstü sürümünü açın
2. **"Open"** butonuna tıklayın
3. `KnightOnline` klasörünü seçin
4. Projeyi açın

---

## ⚙️ ADIM 2: Oyunu Kurun (TEK TIK!)

1. Unity menüsünde **Tools** menüsüne gidin
2. **Knight Online** > **Setup Game** seçin
3. **"OYUNU KUR (TAM KURULUM)"** butonuna tıklayın
4. Kurulum tamamlandığında **Play** butonuna basın!

---

## 🎮 ADIM 3: Oynayın!

### Temel Kontroller:
```
Hareket:     WASD
Bakış:       Mouse
Koşma:       Sol Shift
Zıplama:     Space
Saldırı:     Sol Mouse
Hedef Kilitle: Tab
Etkileşim:   E
Envanter:    I
Karakter:    C
Yetenekler: K
Görevler:    Q
Harita:      M
Ayarlar:    ESC
```

---

## 📦 Kurulum Ne Yapar?

✅ **Sahne:** Zemin, ışıklar, gökyüzü
✅ **Oyuncu:** Kontrol edilebilir karakter
✅ **Canavarlar:** 5 normal canavar + 1 Boss
✅ **NPC'ler:** Tüccar, Görev Verici, Eğitmen, Bankacı
✅ **UI:** Sağlık barı, mana barı, yetenek barı, minimap
✅ **Sistemler:** Envanter, Yetenek, Görev, Kayıt
✅ **Referanslar:** Tüm scriptler birbirine bağlı

---

## 🔧 Özelleştirme

### Yeni Canavar Ekleme:
```csharp
// MonsterController bileşenini bir Cube/Plane/Sphere'e ekleyin
MonsterController monster = gameObject.AddComponent<MonsterController>();
monster.monsterName = "Yeni Canavar";
monster.monsterType = MonsterType.Normal;
monster.level = 5;
monster.maxHealth = 100;
monster.minDamage = 10;
monster.maxDamage = 20;
```

### Yeni Yetenek Ekleme:
```csharp
// SkillData ScriptableObject oluşturun
// Inspector'dan yetenek ayarlarını yapın
// SkillSystem.Instance.LearnSkill(skillData);
```

### Yeni Eşya Ekleme:
```csharp
// ItemData ScriptableObject oluşturun
// InventorySystem.Instance.AddItem(itemData, quantity);
```

---

## 🐛 Sorun Giderme

### "Player not found" hatası:
- AutoSetup scriptini tekrar çalıştırın

### Script hataları:
- Tüm scriptlerin `Assembly Definition` dosyasına eklendiğinden emin olun

### Referans hataları:
- **Tools > Knight Online > Setup Game** ile referansları yeniden bağlayın

---

## 📞 Destek

Unity Console'da sarı/kırmızı uyarılar görürseniz:
1. Unity'i kapatıp açın
2. **Tools > Knight Online > Setup Game** çalıştırın
3. **File > Save Project** yapın

---

**İYİ OYUNLAR! ⚔️🎮**
