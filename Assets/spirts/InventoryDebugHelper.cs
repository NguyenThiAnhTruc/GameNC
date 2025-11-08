using UnityEngine;

/// <summary>
/// Script debug để kiểm tra setup Inventory
/// Thêm vào GameObject có InventorySystem và nhấn phím T trong Play Mode
/// </summary>
public class InventoryDebugHelper : MonoBehaviour
{
    [Header("Nhấn phím T trong Play Mode để debug")]
    [SerializeField] private KeyCode debugKey = KeyCode.T;

    void Update()
    {
        if (Input.GetKeyDown(debugKey))
        {
            DebugInventorySetup();
        }
    }

    [ContextMenu("Debug Inventory Setup")]
    public void DebugInventorySetup()
    {
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log("🔍 KIỂM TRA INVENTORY SETUP");
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

        // 1. Kiểm tra InventorySystem
        if (InventorySystem.Instance == null)
        {
            Debug.LogError("❌ KHÔNG TÌM THẤY InventorySystem.Instance!");
            Debug.LogError("→ Hãy đảm bảo có GameObject với script InventorySystem trong scene");
            return;
        }
        Debug.Log("✅ InventorySystem.Instance tìm thấy");

        var inv = InventorySystem.Instance;

        // 2. Kiểm tra Inventory Screen UI
        if (inv.inventoryScreenUI == null)
        {
            Debug.LogError("❌ inventoryScreenUI = NULL!");
            Debug.LogError("→ Kéo Canvas/InventoryPanel vào field 'Inventory Screen UI'");
        }
        else
        {
            Debug.Log($"✅ inventoryScreenUI = {inv.inventoryScreenUI.name}");
        }

        // 3. Kiểm tra Slot List
        int slotCount = inv.slotList != null ? inv.slotList.Count : 0;
        if (slotCount == 0)
        {
            Debug.LogError("❌ slotList TRỐNG (Size = 0)!");
            Debug.LogError("→ CÁCH SỬA:");
            Debug.LogError("  1. Gán Tag 'Slot' cho tất cả slot");
            Debug.LogError("  2. Click chuột phải vào InventorySystem → 'Rebuild Slot List'");
            Debug.LogError("  3. HOẶC kéo tay từng slot vào Slot List");
        }
        else
        {
            Debug.Log($"✅ Slot List có {slotCount} slots");
            
            // Kiểm tra từng slot
            int nullCount = 0;
            for (int i = 0; i < inv.slotList.Count; i++)
            {
                if (inv.slotList[i] == null)
                {
                    Debug.LogWarning($"⚠️ Slot List[{i}] = NULL!");
                    nullCount++;
                }
            }
            
            if (nullCount > 0)
            {
                Debug.LogError($"❌ Có {nullCount} slot NULL trong Slot List!");
            }
        }

        // 4. Kiểm tra Empty Slots
        int emptySlots = inv.GetEmptySlotCount();
        Debug.Log($"📦 Số slot trống: {emptySlots}");

        // 5. Kiểm tra Is Full
        Debug.Log($"📊 Is Full: {inv.isFull}");
        Debug.Log($"📊 Is Open: {inv.isOpen}");

        // 6. Kiểm tra Canvas
        var canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("❌ Không tìm thấy Canvas trong scene!");
        }
        else
        {
            Debug.Log($"✅ Canvas: {canvas.name}");
            
            var raycaster = canvas.GetComponent<UnityEngine.UI.GraphicRaycaster>();
            if (raycaster == null)
            {
                Debug.LogError("❌ Canvas THIẾU Graphic Raycaster!");
                Debug.LogError("→ Add Component → Graphic Raycaster vào Canvas");
            }
            else
            {
                Debug.Log("✅ Canvas có Graphic Raycaster");
            }
        }

        // 7. Kiểm tra EventSystem
        var eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("❌ Không tìm thấy EventSystem!");
            Debug.LogError("→ GameObject → UI → Event System");
        }
        else
        {
            Debug.Log($"✅ EventSystem: {eventSystem.name}");
        }

        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log("🏁 KIỂM TRA HOÀN TẤT");
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
    }

    [ContextMenu("Debug All InteractableObjects")]
    public void DebugAllInteractableObjects()
    {
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log("🔍 KIỂM TRA TẤT CẢ VẬT PHẨM");
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

        var allItems = FindObjectsOfType<InteractableObject>();
        Debug.Log($"📦 Tìm thấy {allItems.Length} InteractableObjects");

        foreach (var item in allItems)
        {
            Debug.Log($"\n🔸 {item.gameObject.name}:");
            Debug.Log($"  - Item Name: {item.itemName}");
            Debug.Log($"  - Item ID: {item.itemID}");
            
            if (item.CanBePickedUp())
            {
                Debug.Log($"  ✅ Có thể nhặt được");
            }
            else
            {
                Debug.LogError($"  ❌ KHÔNG THỂ NHẶT - Thiếu UI Prefab!");
            }
        }

        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
    }
}
