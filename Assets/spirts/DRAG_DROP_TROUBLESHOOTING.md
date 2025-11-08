# 🔧 Hướng Dẫn Sửa Lỗi Không Kéo Được Item

## ❌ Triệu chứng
- Bạn có item trong inventory
- Nhưng **không click** hoặc **không kéo** được item
- Item không phản hồi khi click chuột

---

## 🎯 5 Nguyên Nhân Chính

### 1️⃣ **Canvas THIẾU Graphic Raycaster** (90% trường hợp)

**Kiểm tra:**
```
Hierarchy → Canvas → Inspector → Tìm "Graphic Raycaster" component
```

**Nếu không có:**
```
1. Chọn Canvas
2. Add Component → UI → Graphic Raycaster
3. ✅ XONG! Thử lại ngay
```

---

### 2️⃣ **Item THIẾU Image Component**

**Kiểm tra:**
- Item có component **Image** không?
- Image có **Raycast Target = ✓** không?

**Sửa:**
```
1. Chọn Item prefab trong Project
2. Add Component → UI → Image
3. Kéo sprite vào Source Image
4. Bật ✓ Raycast Target
5. Save prefab
```

---

### 3️⃣ **Item THIẾU CanvasGroup**

**Sửa:**
```
1. Chọn item trong inventory (trong Play Mode)
2. Add Component → Canvas Group
3. Đặt:
   - Alpha = 1
   - ✓ Interactable
   - ✓ Block Raycasts
```

---

### 4️⃣ **Item THIẾU DragDrop Script**

**Sửa:**
```
1. Chọn item
2. Add Component → DragDrop
3. ✅ Done
```

---

### 5️⃣ **EventSystem bị tắt hoặc thiếu**

**Kiểm tra:**
```
Hierarchy → EventSystem → Inspector → ✓ enabled
```

**Nếu thiếu:**
```
GameObject → UI → Event System
```

---

## 🚀 CÁCH SỬA NHANH (Khuyên dùng!)

### Bước 1: Thêm Debug Helper
```
1. Chọn Canvas (hoặc GameObject bất kỳ)
2. Add Component → DragDropDebugHelper
```

### Bước 2: Chạy Game và Debug
```
1. Nhấn Play
2. Nhấn phím Y
3. Xem Console - nó sẽ chỉ rõ vấn đề!
```

### Bước 3: Auto Fix
```
1. Trong Inspector, tìm DragDropDebugHelper
2. Click chuột phải → "Auto Fix All Items"
3. ✅ Tất cả items sẽ được fix tự động!
```

---

## 📋 Checklist Hoàn Chỉnh

### Canvas Setup
- [ ] **Canvas** có component **Graphic Raycaster**
- [ ] Graphic Raycaster **enabled** = ✓
- [ ] Canvas Render Mode = Screen Space - Overlay (hoặc Camera)

### EventSystem Setup
- [ ] **EventSystem** có trong scene
- [ ] EventSystem **enabled** = ✓

### Mỗi Slot Setup
- [ ] Có component **InventorySlot**
- [ ] Có component **RectTransform**
- [ ] Tag = "Slot" (nếu dùng auto populate)

### Mỗi Item Setup
- [ ] Có component **RectTransform**
- [ ] Có component **Image**
  - [ ] Image.raycastTarget = ✓
  - [ ] Image có sprite
- [ ] Có component **CanvasGroup**
  - [ ] Alpha = 1
  - [ ] Block Raycasts = ✓
  - [ ] Interactable = ✓
- [ ] Có component **DragDrop**

---

## 🎮 Test Kéo Thả

Sau khi fix, test:

1. **Click vào item** → Con trỏ nên hiện
2. **Giữ chuột trái** → Item nên mờ đi (alpha = 0.7)
3. **Kéo chuột** → Item theo chuột
4. **Thả vào slot khác** → Item chuyển sang slot đó
5. **Thả vào slot có item** → 2 items hoán đổi
6. **Thả ra ngoài** → Item quay lại slot cũ

---

## 🐛 Debug Logs

### Khi click vào item (đúng):
```
🖱️ Bắt đầu kéo: ItemName
  → Item được kéo từ: Slot (0)
```

### Khi thả vào slot (đúng):
```
📦 Item được thả vào InventorySlot: Slot (1)
✅ Item ItemName đã được thả vào: Slot (1)
```

### Nếu không click được:
```
❌ [ItemName] THIẾU Image component!
❌ CANVAS THIẾU GRAPHIC RAYCASTER!
```

---

## 🔍 Commands Hữu Ích

### Debug trong Play Mode:
- **Phím T** → Debug Inventory Setup
- **Phím Y** → Debug Drag & Drop Setup

### Context Menu (Click phải script):
- **Debug Inventory Setup** → Kiểm tra tổng quát
- **Debug Drag & Drop Setup** → Kiểm tra chi tiết từng item
- **Auto Fix All Items** → Tự động fix tất cả
- **Rebuild Slot List** → Rebuild slot list theo tag

---

## 💡 Tips

### Prefab Setup (Khuyên dùng)
Để tránh phải fix từng item, setup đúng prefab từ đầu:

```
ItemUI.prefab
├── RectTransform       ✅
├── Image               ✅ (raycastTarget = true)
│   └── Sprite          ✅
├── CanvasGroup         ✅ (alpha=1, blocksRaycasts=true)
└── DragDrop            ✅
```

### Tạo Prefab Đúng:
```
1. Tạo Image trong Canvas
2. Setup tất cả components
3. Test kéo thả trong Play Mode
4. Nếu OK → Kéo ra Project để tạo prefab
5. Xóa khỏi Canvas
6. Gán prefab vào InteractableObject.itemUIPrefab
```

---

## 🎯 TÓM TẮT - Sửa Nhanh Nhất

```
1. Chọn Canvas → Add Component → Graphic Raycaster
2. Add Component → DragDropDebugHelper vào Canvas
3. Play → Nhấn Y → Xem Console
4. Click chuột phải DragDropDebugHelper → Auto Fix All Items
5. ✅ XONG!
```

---

**Nếu vẫn không được, check Console và gửi screenshot để tôi giúp cụ thể hơn! 🚀**
