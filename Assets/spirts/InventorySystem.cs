using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    // ======= Các field công khai để Inspector hiện đúng như ảnh =======
    public GameObject inventoryScreenUI;                       // Inventory Screen UI
    public List<GameObject> slotList = new List<GameObject>(); // Slot List (Size, Element 0..)
    public List<string> itemList = new List<string>();         // Item List (chỉ để xem)
    public bool isOpen;                                        // Is Open (checkbox)
    public bool isFull;                                        // Is Full (checkbox)

    // Giống tutorial (placeholder cho mở rộng equip)
    public GameObject itemToAdd;
    public GameObject whatSlotToEquip;

    // ======= Runtime / nội bộ =======
    private readonly List<InventorySlot> _slots = new List<InventorySlot>();
    private readonly Dictionary<string, int> _dict = new Dictionary<string, int>();
    public static InventorySystem Instance { get; private set; }
    public bool IsSecondPanelOpen { get; private set; }

    // Các tuỳ chọn thêm (không bắt buộc kéo vào)
    [Header("Optional / Advanced")]
    [SerializeField] private GameObject secondPanelUI;
    [SerializeField] private Transform slotsParent;
    [SerializeField] private KeyCode toggleInventoryKey = KeyCode.I;
    [SerializeField] private KeyCode toggleSecondPanelKey = KeyCode.B;
    [SerializeField] private bool controlCursorWhenOpen = true;
    [SerializeField] private bool pauseTimeWhenOpen = false;

    // ================= Unity =================
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        SetInventoryActive(false);
        SetSecondPanelActive(false);

        // Nếu chưa kéo tay Slot List → tự populate theo Tag="Slot"
        PopulateSlotList(force: false);

        RefreshSlotsCache();
        RefreshIsFullFlag();
        RefreshItemListForInspector();
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleInventoryKey)) ToggleInventory();
        if (Input.GetKeyDown(toggleSecondPanelKey)) ToggleSecondPanel();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Không ghi đè nếu bạn đã kéo tay slotList
        PopulateSlotList(force: false);
        RefreshSlotsCache();
        RefreshIsFullFlag();
        RefreshItemListForInspector();
    }
#endif

    // ================= Public controls =================
    public void ToggleInventory()
    {
        SetInventoryActive(!isOpen);
        if (isOpen && IsSecondPanelOpen) SetSecondPanelActive(false);
    }

    public void ToggleSecondPanel()
    {
        SetSecondPanelActive(!IsSecondPanelOpen);
        if (IsSecondPanelOpen && isOpen) SetInventoryActive(false);
    }

    public void CloseAllPanels()
    {
        SetInventoryActive(false);
        SetSecondPanelActive(false);
    }

    // ================= UI helpers =================
    private void SetInventoryActive(bool active)
    {
        isOpen = active;
        if (inventoryScreenUI) inventoryScreenUI.SetActive(active);

        bool anyOpen = isOpen || IsSecondPanelOpen;
        if (controlCursorWhenOpen)
        {
            Cursor.visible = anyOpen;
            Cursor.lockState = anyOpen ? CursorLockMode.None : CursorLockMode.Locked;
        }
        if (pauseTimeWhenOpen) Time.timeScale = anyOpen ? 0f : 1f;

        if (active) RefreshSlotsCache();
    }

    private void SetSecondPanelActive(bool active)
    {
        IsSecondPanelOpen = active;
        if (secondPanelUI) secondPanelUI.SetActive(active);

        bool anyOpen = isOpen || IsSecondPanelOpen;
        if (controlCursorWhenOpen)
        {
            Cursor.visible = anyOpen;
            Cursor.lockState = anyOpen ? CursorLockMode.None : CursorLockMode.Locked;
        }
        if (pauseTimeWhenOpen) Time.timeScale = anyOpen ? 0f : 1f;
    }

    private void RefreshSlotsCache()
    {
        _slots.Clear();

        // Ưu tiên slotList bạn kéo tay / hoặc đã Populate theo Tag
        if (slotList != null && slotList.Count > 0)
        {
            foreach (var go in slotList)
            {
                if (!go) continue;
                var s = go.GetComponent<InventorySlot>();
                if (!s) s = go.AddComponent<InventorySlot>(); // đảm bảo có component
                _slots.Add(s);
            }
        }
        else
        {
            // Không có slotList → tự quét InventorySlot con
            if (!slotsParent && inventoryScreenUI) slotsParent = inventoryScreenUI.transform;
            if (slotsParent) _slots.AddRange(slotsParent.GetComponentsInChildren<InventorySlot>(true));
        }

        RefreshIsFullFlag();
    }

    private void RefreshIsFullFlag()
    {
        int empty = GetEmptySlotCount();
        isFull = (_slots.Count > 0 && empty == 0);
    }

    private void RefreshItemListForInspector()
    {
        itemList.Clear();
        foreach (var kv in _dict) itemList.Add($"{kv.Key} x{kv.Value}");
    }

    // ================= Populate theo Tag (đúng tutorial) =================
    /// <summary>
    /// Điền Slot List bằng các child của inventoryScreenUI có Tag = "Slot".
    /// force=false: chỉ chạy khi slotList đang trống; force=true: luôn rebuild.
    /// </summary>
    private void PopulateSlotList(bool force)
    {
        if (!inventoryScreenUI) return;
        if (!force && slotList != null && slotList.Count > 0) return;

        if (slotList == null) slotList = new List<GameObject>();
        slotList.Clear();

        // Cách 1: Tìm theo Tag "Slot"
        int foundByTag = 0;
        foreach (Transform child in inventoryScreenUI.transform)
        {
            if (child.CompareTag("Slot"))
            {
                slotList.Add(child.gameObject);
                foundByTag++;
            }
        }

        // Cách 2: Nếu không tìm thấy theo tag, tìm theo tên "Slot"
        if (foundByTag == 0)
        {
            Debug.LogWarning("⚠️ Không tìm thấy slot nào có Tag 'Slot'! Đang tìm theo tên...");

            foreach (Transform child in inventoryScreenUI.transform)
            {
                // Tìm tất cả GameObject có tên chứa "Slot" hoặc "slot"
                if (child.name.ToLower().Contains("slot"))
                {
                    slotList.Add(child.gameObject);
                }
            }

            if (slotList.Count > 0)
            {
                Debug.Log($"✅ Đã tìm thấy {slotList.Count} slots theo tên!");
                Debug.LogWarning("💡 TIP: Gán Tag 'Slot' cho các ô để tránh lỗi trong tương lai!");
            }
            else
            {
                Debug.LogError($"❌ KHÔNG TÌM THẤY SLOT NÀO trong '{inventoryScreenUI.name}'!");
                Debug.LogError("→ Các slots phải:");
                Debug.LogError("  1. Là child trực tiếp của inventoryScreenUI");
                Debug.LogError("  2. Có Tag = 'Slot' HOẶC tên chứa 'Slot'");
            }
        }
        else
        {
            Debug.Log($"✅ Đã tìm thấy {foundByTag} slots theo Tag 'Slot'");
        }
    }

    [ContextMenu("Rebuild Slot List (by Tag)")]
    private void RebuildSlotListByTag()
    {
        PopulateSlotList(force: true);
        RefreshSlotsCache();
        Debug.Log($"🔄 Rebuilt Slot List: {slotList.Count} slots found");
    }

    [ContextMenu("Debug Slot List")]
    private void DebugSlotList()
    {
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log("🔍 DEBUG SLOT LIST");
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━");

        if (inventoryScreenUI == null)
        {
            Debug.LogError("❌ inventoryScreenUI = NULL!");
            return;
        }

        Debug.Log($"📦 Inventory Screen UI: {inventoryScreenUI.name}");
        Debug.Log($"📊 Slot List Size: {slotList.Count}");
        Debug.Log($"📊 Internal _slots Count: {_slots.Count}");

        Debug.Log($"\n🔸 Chi tiết Slot List:");
        for (int i = 0; i < slotList.Count; i++)
        {
            if (slotList[i] == null)
            {
                Debug.LogWarning($"  [{i}] = NULL!");
            }
            else
            {
                string tag = slotList[i].tag;
                int childCount = slotList[i].transform.childCount;
                Debug.Log($"  [{i}] {slotList[i].name} (Tag: {tag}, Children: {childCount})");
            }
        }

        Debug.Log($"\n🔸 Children của '{inventoryScreenUI.name}':");
        int childIndex = 0;
        foreach (Transform child in inventoryScreenUI.transform)
        {
            string hasSlotScript = child.GetComponent<InventorySlot>() != null ? "✅" : "❌";
            Debug.Log($"  [{childIndex}] {child.name} (Tag: {child.tag}) {hasSlotScript}");
            childIndex++;
        }

        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━");
    }

    [ContextMenu("Check Full System")]
    private void CheckFullSystem()
    {
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log("🔍 KIỂM TRA TOÀN BỘ HỆ THỐNG");
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        
        // 1. InventorySystem Instance
        if (Instance == null)
            Debug.LogError("❌ 1. InventorySystem.Instance = NULL!");
        else if (Instance != this)
            Debug.LogError("❌ 1. InventorySystem.Instance != this!");
        else
            Debug.Log("✅ 1. InventorySystem.Instance OK");
        
        // 2. UI Reference
        if (inventoryScreenUI == null)
            Debug.LogError("❌ 2. inventoryScreenUI = NULL! → Gán bangchua vào đây");
        else
            Debug.Log($"✅ 2. inventoryScreenUI = {inventoryScreenUI.name}");
        
        // 3. Slot List
        Debug.Log($"📊 3. slotList.Count = {slotList.Count}");
        if (slotList.Count == 0)
            Debug.LogError("❌ 3. SLOT LIST TRỐNG! → Rebuild Slot List");
        else if (slotList.Count < 24)
            Debug.LogWarning($"⚠️ 3. Slot List chỉ có {slotList.Count} slots (có thể thiếu)");
        else
            Debug.Log($"✅ 3. Slot List có {slotList.Count} slots");
        
        // 4. Internal _slots
        Debug.Log($"📊 4. _slots.Count = {_slots.Count}");
        if (_slots.Count == 0)
            Debug.LogError("❌ 4. _SLOTS TRỐNG! → Gọi RefreshSlotsCache()");
        else
            Debug.Log($"✅ 4. _slots có {_slots.Count} slots");
        
        // 5. Empty Slots
        int empty = GetEmptySlotCount();
        Debug.Log($"📊 5. Số slot trống = {empty}/{_slots.Count}");
        
        // 6. Is Full
        Debug.Log($"📊 6. isFull = {isFull}");
        if (isFull && empty > 0)
            Debug.LogError("❌ 6. isFull = true nhưng còn slot trống!");
        
        // 7. Items in inventory
        Debug.Log($"📦 7. Số loại item: {_dict.Count}");
        foreach (var kv in _dict)
            Debug.Log($"   - {kv.Key}: x{kv.Value}");
        
        Debug.Log("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        
        // Kết luận
        if (inventoryScreenUI != null && slotList.Count > 0 && _slots.Count > 0)
        {
            Debug.Log("✅ HỆ THỐNG OK - Có thể nhặt vật thể!");
        }
        else
        {
            Debug.LogError("❌ HỆ THỐNG CÓ LỖI - Xem các lỗi trên!");
            Debug.LogError("→ CÁCH SỬA:");
            if (inventoryScreenUI == null)
                Debug.LogError("  1. Gán bangchua vào 'Inventory Screen UI'");
            if (slotList.Count == 0)
                Debug.LogError("  2. Click phải → 'Rebuild Slot List (by Tag)'");
            if (_slots.Count == 0)
                Debug.LogError("  3. Gọi RefreshSlotsCache() trong Start()");
        }
        
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
    }

    // ================= Drag & Drop =================
    public void HandleItemDrop(InventorySlot targetSlot)
    {
        if (targetSlot == null || DragDrop.itemBeingDragged == null) return;

        Transform draggedItem = DragDrop.itemBeingDragged.transform;
        Transform originSlot = DragDrop.itemBeingDraggedFrom;
        if (originSlot == targetSlot.transform) return;

        Transform existing = targetSlot.transform.childCount > 0
            ? targetSlot.transform.GetChild(0) : null;

        if (existing == null) { MoveItemToSlot(draggedItem, targetSlot.transform); return; }
        SwapItems(originSlot, targetSlot.transform, draggedItem, existing);
    }

    private void MoveItemToSlot(Transform item, Transform slot)
    {
        item.SetParent(slot);
        (item as RectTransform).anchoredPosition = Vector2.zero;
        item.localScale = Vector3.one;
        RefreshIsFullFlag();
    }

    private void SwapItems(Transform slotA, Transform slotB, Transform itemA, Transform itemB)
    {
        itemA.SetParent(slotB);
        (itemA as RectTransform).anchoredPosition = Vector2.zero;

        itemB.SetParent(slotA);
        (itemB as RectTransform).anchoredPosition = Vector2.zero;

        itemA.localScale = Vector3.one;
        itemB.localScale = Vector3.one;
    }

    // ================= Inventory API (hai cách thêm item) =================

    /// <summary>
    /// Cách 1 (hiện đại): Thêm bằng prefab UI đã có (kéo vào tham số).
    /// </summary>
    public bool AddItemToInventory(GameObject itemUIPrefab, string itemID, string itemName)
    {
        if (!itemUIPrefab) return false;

        foreach (var slot in _slots)
        {
            if (!slot) continue;
            if (slot.transform.childCount == 0)
            {
                var go = Instantiate(itemUIPrefab, slot.transform);
                var rt = go.GetComponent<RectTransform>();
                if (rt)
                {
                    rt.anchoredPosition = Vector2.zero;
                    rt.localScale = Vector3.one;
                }
                else
                {
                    // Nếu không có RectTransform, thêm warning
                    Debug.LogWarning($"⚠️ Item '{itemName}' không có RectTransform! Có thể không hiển thị đúng trong UI.");
                }

                // ===== QUAN TRỌNG: Đảm bảo có CanvasGroup =====
                var canvasGroup = go.GetComponent<CanvasGroup>();
                if (!canvasGroup)
                {
                    canvasGroup = go.AddComponent<CanvasGroup>();
                    Debug.Log($"✅ Đã thêm CanvasGroup vào {itemName}");
                }
                canvasGroup.blocksRaycasts = true;
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;

                // ===== QUAN TRỌNG: Đảm bảo có DragDrop =====
                var dragDrop = go.GetComponent<DragDrop>();
                if (!dragDrop)
                {
                    dragDrop = go.AddComponent<DragDrop>();
                    Debug.Log($"✅ Đã thêm DragDrop vào {itemName}");
                }

                // ===== QUAN TRỌNG: Đảm bảo có Image để raycast =====
                var image = go.GetComponent<UnityEngine.UI.Image>();
                if (!image)
                {
                    Debug.LogWarning($"⚠️ Item '{itemName}' không có Image component! Có thể không click được.");
                    Debug.LogWarning($"→ Thêm Image component vào prefab '{itemUIPrefab.name}' và set sprite.");
                }
                else
                {
                    // Đảm bảo Image có thể raycast
                    image.raycastTarget = true;
                }

                // ===== MỚI: Setup ItemData để hiển thị thông tin =====
                var itemData = go.GetComponent<ItemData>();
                if (itemData == null)
                {
                    itemData = go.AddComponent<ItemData>();
                    Debug.Log($"✅ Đã thêm ItemData vào {itemName}");
                }

                // Khởi tạo thông tin item
                itemData.Initialize(itemName, itemID, image != null ? image.sprite : null);

                // Đặt tên GameObject cho dễ debug
                go.name = $"{itemName} ({itemID})";

                if (!string.IsNullOrEmpty(itemID))
                {
                    if (_dict.ContainsKey(itemID)) _dict[itemID]++;
                    else _dict[itemID] = 1;
                }

                RefreshItemListForInspector();
                RefreshIsFullFlag();
                Debug.Log($"✅ Added {itemName} to {slot.name} (DragDrop ready: {dragDrop != null})");
                return true;
            }
        }

        Debug.LogWarning("⚠️ Inventory is full!");
        RefreshIsFullFlag();
        return false;
    }

    /// <summary>
    /// Cách 2 (đúng ảnh/tutorial): Thêm bằng tên prefab trong thư mục Resources.
    /// Ví dụ: Assets/Resources/Wood.prefab → AddToInventory("Wood")
    /// </summary>
    public void AddToInventory(string itemName)
    {
        if (isFull)
        {
            Debug.Log("The inventory is full");
            return;
        }

        // Tìm slot trống theo thứ tự (giống video)
        whatSlotToEquip = FindNextEmptySlot();
        if (whatSlotToEquip == null)
        {
            Debug.Log("No empty slot found!");
            return;
        }

        // Load prefab UI từ Resources
        GameObject loadedItem = Resources.Load<GameObject>(itemName);
        if (loadedItem == null)
        {
            Debug.LogError($"❌ Item '{itemName}' not found in Resources!");
            return;
        }

        // Instantiate giống ảnh: vị trí/rotation của slot + parent = slot
        itemToAdd = Instantiate(
            loadedItem,
            whatSlotToEquip.transform.position,
            whatSlotToEquip.transform.rotation,
            whatSlotToEquip.transform
        );

        // Chỉnh RectTransform cho UI
        var rt = itemToAdd.GetComponent<RectTransform>();
        if (rt)
        {
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;
        }

        // Bảo đảm kéo-thả
        if (!itemToAdd.GetComponent<CanvasGroup>())
            itemToAdd.AddComponent<CanvasGroup>();
        if (!itemToAdd.GetComponent<DragDrop>())
            itemToAdd.AddComponent<DragDrop>();

        Debug.Log($"✅ Added {itemName} via Resources");
        RefreshIsFullFlag();
        RefreshItemListForInspector();
    }

    /// <summary>API tương thích tutorial cũ.</summary>
    public bool AddItemToFirstEmptySlot(GameObject itemUIPrefab)
        => AddItemToInventory(itemUIPrefab, "", "Item");

    public void RemoveItemAtSlot(InventorySlot slot)
    {
        if (!slot) return;
        for (int i = slot.transform.childCount - 1; i >= 0; i--)
            Destroy(slot.transform.GetChild(i).gameObject);

        RefreshIsFullFlag();
        RefreshItemListForInspector();
    }

    public bool HasItem(string itemID) => _dict.ContainsKey(itemID) && _dict[itemID] > 0;
    public int GetItemCount(string itemID) => _dict.ContainsKey(itemID) ? _dict[itemID] : 0;

    public int GetEmptySlotCount()
    {
        int c = 0;
        foreach (var s in _slots) if (s && s.transform.childCount == 0) c++;
        return c;
    }

    // ====== Helpers cho tutorial ======
    private GameObject FindNextEmptySlot()
    {
        // Duyệt theo thứ tự slotList như trong video
        foreach (var s in slotList)
        {
            if (s != null && s.transform.childCount == 0)
                return s;
        }
        return null;
    }
}
