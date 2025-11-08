# 📊 PHÂN TÍCH LOGIC HỆ THỐNG INVENTORY

## 🔍 FLOW CHART - Quy trình nhặt đồ vật

```
┌─────────────────────────────────────────────────────────────┐
│                    PLAYER NHẶT VẬT THỂ                       │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│  1. SelectionManager.Update()                                │
│     - Raycast từ camera                                      │
│     - Phát hiện InteractableObject                           │
│     - Hiển thị UI "[E] Nhặt {itemName}"                      │
└─────────────────────────────────────────────────────────────┘
                              │
                         Nhấn phím E
                              ▼
┌─────────────────────────────────────────────────────────────┐
│  2. InteractableObject.Pickup()                              │
│     ✓ Kiểm tra InventorySystem.Instance != null             │
│     ✓ Kiểm tra itemUIPrefab != null                         │
│     ✓ Gọi: InventorySystem.AddItemToInventory()             │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│  3. InventorySystem.AddItemToInventory()                     │
│     - Duyệt qua _slots (internal list)                      │
│     - Tìm slot trống đầu tiên (childCount == 0)             │
│     - Instantiate itemUIPrefab vào slot                      │
│     - Thêm CanvasGroup, DragDrop, ItemData                  │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│  4. Item xuất hiện trong Inventory                           │
│     ✓ Destroy vật thể 3D trong world                        │
│     ✓ Item UI hiện trong slot đầu tiên trống                │
└─────────────────────────────────────────────────────────────┘
```

---

## ❌ VẤN ĐỀ HIỆN TẠI

### Vấn đề 1: _slots list TRỐNG hoặc THIẾU
```
InventorySystem.Start()
    │
    ├─> PopulateSlotList(force: false)
    │   └─> Tìm slots theo Tag "Slot" hoặc tên
    │
    └─> RefreshSlotsCache()
        └─> Chuyển slotList → _slots (internal)
```

**Nếu _slots.Count = 0:**
```
AddItemToInventory() 
  └─> foreach (var slot in _slots)  // KHÔNG CHẠY VÌ LIST TRỐNG!
      └─> return false
```

**Nếu _slots.Count = 7 nhưng thực tế có 24 slots:**
```
AddItemToInventory()
  └─> Chỉ duyệt 7 slots đầu
  └─> Sau 7 items → "⚠️ Inventory is full!"
  └─> 17 slots còn lại BỊ BỎ QUA
```

---

## ✅ GIẢI PHÁP

### Root Cause:
**slotList không chứa đủ tất cả slots → _slots thiếu → AddItemToInventory fail**

### Fix:

#### 1. Đảm bảo slotList có đủ slots:
```csharp
// Trong InventorySystem.Start()
PopulateSlotList(force: false);  // Tìm slots
RefreshSlotsCache();             // Chuyển sang _slots

// Debug log
Debug.Log($"📦 Slot List Size: {slotList.Count}");
Debug.Log($"📊 Internal _slots Count: {_slots.Count}");
```

#### 2. Slots phải có Tag "Slot" hoặc tên chứa "slot":
```
bangchua (GameObject)
├── Slot1 (Tag: "Slot") ✅
├── Slot2 (Tag: "Slot") ✅
├── Slot3 (Tag: "Slot") ✅
├── ...
└── Slot24 (Tag: "Slot") ✅
```

#### 3. Rebuild Slot List:
```
InventorySystem → Click phải → "Rebuild Slot List (by Tag)"
```

---

## 🔍 KIỂM TRA LOGIC TỪNG BƯỚC

### Bước 1: Kiểm tra InventorySystem.Instance
```csharp
if (InventorySystem.Instance == null) 
{
    Debug.LogError("❌ InventorySystem.Instance = NULL!");
    // FIX: Đảm bảo có GameObject với InventorySystem trong scene
}
```

### Bước 2: Kiểm tra inventoryScreenUI
```csharp
if (inventoryScreenUI == null) 
{
    Debug.LogError("❌ inventoryScreenUI = NULL!");
    // FIX: Gán bangchua vào field "Inventory Screen UI"
}
```

### Bước 3: Kiểm tra slotList
```csharp
if (slotList.Count == 0) 
{
    Debug.LogError("❌ slotList TRỐNG!");
    // FIX: Gán Tag "Slot" cho các slot và Rebuild
}
else if (slotList.Count < 24) 
{
    Debug.LogWarning($"⚠️ slotList chỉ có {slotList.Count} slots (cần 24)!");
    // FIX: Rebuild Slot List
}
```

### Bước 4: Kiểm tra _slots
```csharp
if (_slots.Count == 0) 
{
    Debug.LogError("❌ _slots TRỐNG!");
    // FIX: RefreshSlotsCache() trong Start()
}
```

### Bước 5: Kiểm tra itemUIPrefab
```csharp
if (itemUIPrefab == null) 
{
    Debug.LogError("❌ Item không có UI Prefab!");
    // FIX: Gán prefab vào InteractableObject.itemUIPrefab
}
```

---

## 📝 CÁCH DEBUG ĐÚNG

### Debug trong Play Mode:

```csharp
// 1. Khi Start game
void Start() 
{
    Debug.Log("━━━━━ INVENTORY SYSTEM START ━━━━━");
    Debug.Log($"Inventory Screen UI: {(inventoryScreenUI ? inventoryScreenUI.name : "NULL")}");
    Debug.Log($"Slot List Count: {slotList.Count}");
    
    PopulateSlotList(force: false);
    Debug.Log($"After Populate: {slotList.Count} slots");
    
    RefreshSlotsCache();
    Debug.Log($"Internal _slots Count: {_slots.Count}");
    Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
}

// 2. Khi nhặt vật thể
public bool AddItemToInventory(GameObject itemUIPrefab, string itemID, string itemName)
{
    Debug.Log($"🎯 Trying to add: {itemName}");
    Debug.Log($"   _slots.Count = {_slots.Count}");
    
    if (!itemUIPrefab) 
    {
        Debug.LogError($"   ❌ itemUIPrefab = NULL!");
        return false;
    }

    foreach (var slot in _slots)
    {
        Debug.Log($"   Checking slot: {slot.name}, Empty: {slot.transform.childCount == 0}");
        
        if (!slot) continue;
        if (slot.transform.childCount == 0)
        {
            Debug.Log($"   ✅ Found empty slot: {slot.name}");
            // ... instantiate item
            return true;
        }
    }

    Debug.LogError($"   ❌ No empty slot found! (Checked {_slots.Count} slots)");
    return false;
}
```

---

## 🎯 CHECKLIST SỬA LỖI

### Setup Scene:
- [ ] **InventorySystem** có trong scene (attached vào GameObject)
- [ ] **bangchua** được gán vào "Inventory Screen UI"
- [ ] **bangchua** có 24 child slots
- [ ] Mỗi slot có Tag = "Slot" HOẶC tên chứa "slot"

### Setup Code:
- [ ] **PopulateSlotList()** được gọi trong Start()
- [ ] **RefreshSlotsCache()** được gọi sau PopulateSlotList()
- [ ] **_slots.Count** > 0 sau khi Start()

### Setup Item:
- [ ] **InteractableObject** có trên vật thể 3D
- [ ] **itemUIPrefab** đã được gán (không null)
- [ ] **itemUIPrefab** có Image, CanvasGroup, DragDrop

### Test Flow:
1. [ ] Start game → Console: "✅ Đã tìm thấy X slots"
2. [ ] Nhấn T → Xem debug info
3. [ ] Nhặt vật thể → Console: "✅ Added {name} to Slot"
4. [ ] Mở inventory (I) → Thấy item trong slot
5. [ ] Nhặt thêm → Item vào slot tiếp theo

---

## 💡 TÓM TẮT LOGIC

```
ĐÚNG:
Player nhấn E 
  → InteractableObject.Pickup() 
  → InventorySystem.AddItemToInventory() 
  → Duyệt _slots (24 slots)
  → Tìm slot trống
  → Instantiate itemUIPrefab vào slot
  → ✅ Item hiện trong inventory

SAI (Hiện tại):
Player nhấn E 
  → InteractableObject.Pickup() 
  → InventorySystem.AddItemToInventory() 
  → Duyệt _slots (0 hoặc 7 slots)  ❌
  → Không tìm thấy slot / Đầy sau 7 items
  → ❌ Item KHÔNG vào inventory
```

---

## 🔧 SCRIPT KIỂM TRA NHANH

Thêm vào InventorySystem để debug:

```csharp
[ContextMenu("Check Everything")]
private void CheckEverything()
{
    Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━");
    Debug.Log("🔍 FULL SYSTEM CHECK");
    Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━");
    
    // 1. InventorySystem
    Debug.Log($"1. InventorySystem.Instance: {(Instance != null ? "✅" : "❌ NULL")}");
    
    // 2. UI Reference
    Debug.Log($"2. inventoryScreenUI: {(inventoryScreenUI != null ? inventoryScreenUI.name : "❌ NULL")}");
    
    // 3. Slot List
    Debug.Log($"3. slotList.Count: {slotList.Count}");
    if (slotList.Count == 0) 
        Debug.LogError("   ❌ SLOT LIST TRỐNG!");
    
    // 4. Internal Slots
    Debug.Log($"4. _slots.Count: {_slots.Count}");
    if (_slots.Count == 0) 
        Debug.LogError("   ❌ _SLOTS TRỐNG! Gọi RefreshSlotsCache()");
    
    // 5. Empty Slots
    int empty = GetEmptySlotCount();
    Debug.Log($"5. Empty slots: {empty}/{_slots.Count}");
    
    // 6. Is Full
    Debug.Log($"6. isFull: {isFull}");
    
    Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━");
}
```

---

**Vấn đề chính: slotList.Count = 7 → _slots.Count = 7 → Chỉ nhận được 7 items!**

**Giải pháp: Rebuild Slot List để có đủ 24 slots! 🎯**
