# 🏰 Medieval Realm Online - MMORPG Prototip

## Konsept & Vizyon

Orta çağ fantasy dünyasında geçen, izometrik perspektifli, offline MMORPG deneyimi. Albion Online'ın pixel art estetiği ile Knight Online'ın PvP odaklı oynanışını birleştiren, tek kişilik ama canlı bir dünya hissi veren oyun.

## Tasarım Dili

### Estetik Yön
- **Albion Online tarzı**: 16-bit pixel art, parlak renkler, detaylı sprite'lar
- **Knight Online referansı**: Karanlık fantasy, gotik elementler

### Renk Paleti
- Primary: `#4a90d9` (Royal Blue - soylu/lonca)
- Secondary: `#2d2d2d` (Karanlık gri - UI arka plan)
- Accent: `#ffd700` (Altın - değerli eşyalar/XP)
- Background: `#1a1a2e` (Derin gece mavisi)
- Text: `#e8e8e8` (Açık gri)
- Health: `#e74c3c` (Kırmızı)
- Mana: `#3498db` (Mavi)

### Tipografi
- Ana font: "Press Start 2P" (pixel font)
- UI font: "Roboto Mono"
- Fallback: monospace

## Oyun Mekanikleri

### Karakter Sistemi
1. **Irklar**: İnsan, Ork, Elf, Demon
2. **Sınıflar**: Warrior, Mage, Rogue, Paladin
3. **Özellikler**: Strength, Dexterity, Intelligence, Vitality
4. **Level sistemi**: 1-100

### Lonca Sistemi
- Beyaz Lonca (Işık tarafı)
- Siyah Lonca (Karanlık tarafı)

### PvE İçerik
- Normal canavarlar (seviye 1-20)
- Elite canavarlar
- Boss canavarlar

### UI Bileşenleri
- Sağ üst: XP barı, level
- Sol üst: Can barı, mana barı
- Alt: Quick slot (1-8 tuşları)

## Kontroller

- WASD: Hareket
- Mouse sol tık: Saldırı/Skill
- 1-4: Skilller
- I: Envanter
- ESC: Menü
- Q: Normal saldırı

## Proje Yapısı

```
mmorpg-game/
├── index.html
├── SPEC.md
├── css/style.css
└── js/
    ├── main.js
    ├── game.js
    ├── player.js
    ├── enemy.js
    ├── combat.js
    ├── skills.js
    ├── inventory.js
    ├── renderer.js
    └── ui.js
```

## Çalışacak Özellikler (v1.0)

✅ Temel karakter hareketi
✅ 4-yönlü sprite animasyonları
✅ Düşman AI
✅ XP ve level sistemi
✅ 4 temel skill
✅ Envanter sistemi
✅ Lonca seçimi
✅ Shop sistemi
✅ Ana menü ve karakter oluşturma
✅ Kayıt sistemi