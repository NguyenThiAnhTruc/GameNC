# 📦 Hướng Dẫn Hiển Thị Tên và Icon Item trong Inventory

## 🎯 Mục tiêu
Khi nhặt vật thể (ví dụ: Stone), trong inventory sẽ hiển thị:
- ✅ **Icon** của Stone
- ✅ **Tên** "Stone" 
- ✅ **Số lượng** (nếu có nhiều)
- ✅ **Tooltip** khi hover chuột (optional)

---

## 🔧 Cách 1: Tạo Item UI Prefab Đơn Giản (Chỉ Icon)

### Bước 1: Tạo UI trong Canvas
```
1. Hierarchy → Canvas → Click phải → UI → Image
2. Đặt tên: "StoneUI"
3. Set kích thước: Width = 64, Height = 64
```

### Bước 2: Setup Image Component
```
1. Chọn StoneUI
2. Inspector → Image component:
   - Source Image: [Kéo sprite icon của Stone vào đây]
   - Color: Trắng (255, 255, 255, 255)
   - Raycast Target: ✓ (BẮT BUỘC để click được)
   - Preserve Aspect: ✓ (Optional, giữ tỷ lệ icon)
```

### Bước 3: Add Components cần thiết
```
1. Add Component → Canvas Group
   - Alpha: 1
   - Interactable: ✓
   - Block Raycasts: ✓

2. Add Component → DragDrop

3. Add Component → ItemData (MỚI!)
   - Item Name: "Stone"
   - Item ID: "stone" (hoặc để trống, tự động tạo)
   - Item Icon: [Cùng sprite như Image]
```

### Bước 4: Tạo Prefab
```
1. Kéo StoneUI từ Hierarchy vào thư mục Project/Prefabs
2. Xóa StoneUI khỏi Canvas
3. ✅ Prefab đã sẵn sàng!
```

### Bước 5: Gán vào InteractableObject
```
1. Chọn Stone1 trong scene (GameObject 3D)
2. Inspector → InteractableObject component
3. Kéo prefab "StoneUI" vào field "Item UI Prefab"
4. ✅ XONG!
```

---

## 🎨 Cách 2: Tạo Item UI Prefab Đẹp Hơn (Icon + Tên + Số lượng)

### Cấu trúc UI:
```
StoneUI (Image - Background)
├── Icon (Image - Sprite của Stone)
├── NameText (TextMeshPro - "Stone")
└── QuantityText (TextMeshPro - "x5" góc dưới phải)
```

### Bước 1: Tạo Panel Background
```
1. Canvas → UI → Image
2. Đặt tên: "StoneUI"
3. Size: 64x64
4. Image: Set màu background (ví dụ: xám đậm #444444)
5. Image Type: Sliced (nếu dùng sprite border)
```

### Bước 2: Tạo Icon
```
1. Click phải StoneUI → UI → Image
2. Đặt tên: "Icon"
3. Size: 48x48 (nhỏ hơn background một chút)
4. Anchor: Center-Center
5. Source Image: [Sprite icon của Stone]
6. Preserve Aspect: ✓
```

### Bước 3: Tạo Name Text (Optional)
```
1. Click phải StoneUI → UI → TextMeshPro - Text
2. Đặt tên: "NameText"
3. Anchor: Bottom-Center
4. Size: 64x16
5. Position Y: -8
6. Text: "Stone"
7. Font Size: 10
8. Alignment: Center-Center
9. Color: Trắng
10. Outline: Đen (độ dày 0.2 để dễ đọc)
```

### Bước 4: Tạo Quantity Text (Optional)
```
1. Click phải StoneUI → UI → TextMeshPro - Text
2. Đặt tên: "QuantityText"
3. Anchor: Bottom-Right
4. Size: 24x16
5. Position: X = -4, Y = 4
6. Text: "x5"
7. Font Size: 12
8. Alignment: Right-Bottom
9. Color: Trắng
10. Outline: Đen
```

### Bước 5: Add Components
```
Chọn StoneUI (root), add:
1. Canvas Group (alpha=1, interactable=✓, blockRaycasts=✓)
2. DragDrop
3. ItemData:
   - Item Name: "Stone"
   - Item ID: "stone"
   - Item Icon: [Cùng sprite với Icon]
   - Description: "Đá để xây dựng"
   - Quantity: 1
   
4. References trong ItemData:
   - Icon Image: [Kéo child "Icon" vào đây]
   - Name Text: [Kéo child "NameText" vào đây]
   - Quantity Text: [Kéo child "QuantityText" vào đây]
```

### Bước 6: Tạo Prefab
```
1. Kéo StoneUI vào thư mục Prefabs
2. Xóa khỏi Canvas
3. Gán vào Stone1 → ItemUIPrefab
```

---

## 🎯 Auto Setup (Khuyên dùng!)

Hệ thống đã được cập nhật để **tự động** thêm ItemData khi nhặt vật phẩm!

### Các bước tối thiểu:
```
1. Tạo UI prefab với Image (có sprite)
2. Add Canvas Group và DragDrop
3. Gán vào InteractableObject
4. ✅ ItemData sẽ tự động được thêm khi nhặt!
```

Khi nhặt, hệ thống sẽ tự động:
- ✅ Thêm ItemData component
- ✅ Set itemName từ InteractableObject
- ✅ Set itemID từ InteractableObject
- ✅ Set icon từ Image.sprite
- ✅ Đặt tên GameObject = "Stone (stone)"

---

## 📝 Template Prefab Structure

### Tối thiểu (Chỉ Icon):
```
StoneUI
├── RectTransform (64x64)
├── Image (raycastTarget = ✓)
├── CanvasGroup
├── DragDrop
└── ItemData (tự động thêm)
```

### Đầy đủ (Icon + Text):
```
StoneUI
├── RectTransform (64x64)
├── Image (background)
├── CanvasGroup
├── DragDrop
├── ItemData
│   ├── Icon Image → Icon (child)
│   ├── Name Text → NameText (child)
│   └── Quantity Text → QuantityText (child)
├── Icon (Image - child)
├── NameText (TMP - child, optional)
└── QuantityText (TMP - child, optional)
```

---

## 🎨 Ví dụ Sprites/Icons

### Tìm sprite icons:
```
1. Tải icon pack miễn phí từ:
   - Kenny.nl (Game Icons)
   - Itch.io (Free Assets)
   - Unity Asset Store (Free 2D Assets)

2. Import vào Unity:
   - Assets/Sprites/Icons/stone_icon.png
   
3. Texture Settings:
   - Texture Type: Sprite (2D and UI)
   - Sprite Mode: Single
   - Pixels Per Unit: 64 (hoặc 100)
   - Filter Mode: Point (pixel art) hoặc Bilinear
   - Compression: None (chất lượng cao)
```

---

## 🐛 Troubleshooting

### ❌ Icon không hiển thị
**Nguyên nhân:**
- Image.sprite = null
- Image.enabled = false
- Image.color.alpha = 0

**Sửa:**
```
1. Chọn prefab
2. Image component → Source Image → [Chọn sprite]
3. Color → Alpha = 255
4. ✓ Enabled
```

### ❌ Tên không hiển thị
**Nguyên nhân:**
- ItemData.nameText = null
- Text không có font
- Text color = transparent

**Sửa:**
```
1. Chọn prefab
2. ItemData → Name Text → [Kéo TextMeshPro child vào]
3. TextMeshPro → Font Asset → [Chọn font]
4. Color → White
```

### ❌ Item hiển thị sai tên
**Kiểm tra:**
```
1. InteractableObject.itemName = "Stone" ✓
2. ItemData sẽ tự động copy tên này
3. Console log: "✅ Added Stone to Slot (0)"
```

---

## 💡 Tips Nâng Cao

### 1. Tạo nhiều variants:
```
StoneUI_Common.prefab   (màu xám)
StoneUI_Rare.prefab     (màu xanh)
StoneUI_Epic.prefab     (màu tím)
```

### 2. Thêm animation:
```
Add Component → Animator
Tạo animation: Hover_Scale (phóng to 10% khi hover)
```

### 3. Thêm border theo rarity:
```
Background Image:
- Common: Viền xám
- Rare: Viền xanh
- Epic: Viền tím
```

### 4. Stack items:
```
Khi nhặt cùng loại item:
- Tăng ItemData.quantity
- Hiển thị "x5" ở góc
```

---

## 📋 Checklist Setup Hoàn Chỉnh

### UI Prefab:
- [ ] Image component với sprite
- [ ] Image.raycastTarget = ✓
- [ ] RectTransform size phù hợp
- [ ] CanvasGroup (alpha=1, blocksRaycasts=✓)
- [ ] DragDrop component
- [ ] ItemData component (có thể auto)

### InteractableObject (3D GameObject):
- [ ] Item Name đã điền
- [ ] Item UI Prefab đã gán
- [ ] Có Collider trigger

### Test:
- [ ] Nhặt được vật phẩm
- [ ] Icon hiển thị trong inventory
- [ ] Tên hiển thị đúng
- [ ] Kéo thả được giữa các slot
- [ ] Console không có lỗi

---

**Sau khi setup xong, nhặt vật phẩm và xem Console để debug! 🎯**

## Console logs mẫu:
```
✅ Đã thêm CanvasGroup vào Stone
✅ Đã thêm DragDrop vào Stone
✅ Đã thêm ItemData vào Stone
✅ Added Stone to Slot (0) (DragDrop ready: True)
✅ Đã nhặt 'Stone' và thêm vào inventory
```
