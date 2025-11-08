# 📦 Hướng Dẫn Setup Inventory System

## ❌ Lỗi: "Vật thể không có itemUIPrefab"

### Nguyên nhân
Khi bạn thấy lỗi này:
```
❌ Vật thể không có itemUIPrefab! Không thể thêm vào inventory.
```

Điều đó có nghĩa là GameObject trong scene có script `InteractableObject` nhưng **chưa được gán UI Prefab** để hiển thị trong inventory.

### ✅ Cách khắc phục

#### Bước 1: Tạo UI Prefab cho vật phẩm (nếu chưa có)

1. Tạo một GameObject mới trong Canvas
2. Thêm component `Image` hoặc `RawImage`
3. Chỉnh kích thước phù hợp (ví dụ: 64x64)
4. Thêm component `DragDrop` (để có thể kéo thả)
5. Thêm component `CanvasGroup` (để hỗ trợ kéo thả)
6. Kéo GameObject này vào thư mục Prefabs để tạo prefab
7. Xóa GameObject khỏi Canvas

#### Bước 2: Gán UI Prefab cho InteractableObject

1. Chọn GameObject vật phẩm trong Hierarchy (ví dụ: Apple, Sword, Potion...)
2. Tìm component `Interactable Object` trong Inspector
3. Trong phần **"Thông tin vật phẩm"**:
   - Điền `Item Name` (tên hiển thị)
   - Điền `Item ID` (hoặc để trống để tự động tạo)
   - **Kéo UI Prefab vào field `Item UI Prefab`** ⭐
4. Lưu scene

#### Bước 3: Kiểm tra setup

Trong Inspector của GameObject vật phẩm, kiểm tra:
- ✅ `Item Name` đã điền
- ✅ `Item UI Prefab` đã gán
- ✅ Có ít nhất 1 Collider (với Is Trigger = true)
- ✅ Có thêm 1 Collider (với Is Trigger = false) cho raycast

### 🎮 Cách hoạt động

1. **InteractableObject** (3D GameObject trong scene):
   - Chứa thông tin vật phẩm
   - Detect player với Trigger Collider
   - Detect raycast với Normal Collider
   - Reference đến UI Prefab

2. **UI Prefab** (2D UI Element):
   - Hiển thị trong Inventory UI
   - Có thể kéo thả giữa các slot
   - Được tạo runtime khi nhặt vật phẩm

### 🔧 Tính năng mới

**Validation tự động:**
- ⚠️ Editor warning nếu thiếu UI Prefab
- ❌ Runtime error với hướng dẫn chi tiết
- 🔴 Visual indicator (red cube) trong Scene view

**UI Feedback:**
- Hiển thị "[E] Nhặt {tên}" nếu setup đúng
- Hiển thị "[!] {tên} (THIẾU SETUP)" bằng màu đỏ nếu thiếu prefab

### 📝 Ví dụ cấu trúc thư mục

```
Assets/
├── Prefabs/
│   ├── Items/           (3D models - InteractableObject)
│   │   ├── Apple.prefab
│   │   └── Sword.prefab
│   └── ItemUI/          (2D UI - UI Prefabs)
│       ├── AppleUI.prefab
│       └── SwordUI.prefab
└── Scenes/
    └── GameScene.unity
```

### 🐛 Troubleshooting

**Q: Tôi đã gán prefab nhưng vẫn báo lỗi?**
- A: Kiểm tra xem prefab có đúng là UI element không (có RectTransform)
- A: Thử xóa và gán lại prefab

**Q: Làm sao biết GameObject nào thiếu prefab?**
- A: Chọn GameObject trong Hierarchy, nếu field "Item UI Prefab" trống = thiếu
- A: Trong Scene view, GameObject có vẽ red cube = thiếu prefab

**Q: Có thể dùng chung 1 UI prefab cho nhiều vật phẩm không?**
- A: Có, nhưng nên tạo riêng để dễ phân biệt bằng icon/màu sắc

### 📚 Related Files

- `InteractableObject.cs` - Script chính cho vật phẩm nhặt được
- `InventorySystem.cs` - Quản lý inventory
- `InventorySlot.cs` - Ô chứa item trong inventory
- `DragDrop.cs` - Xử lý kéo thả item
- `SelectionManager.cs` - Detect và hiển thị UI tương tác

---

**Cập nhật:** Code đã được cải thiện với validation tốt hơn và error messages chi tiết hơn.
