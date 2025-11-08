using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Script debug để kiểm tra tại sao không kéo được item
/// Attach vào Canvas hoặc bất kỳ GameObject nào
/// Nhấn phím Y trong Play Mode để kiểm tra
/// </summary>
public class DragDropDebugHelper : MonoBehaviour
{
    [Header("Nhấn phím Y trong Play Mode để debug Drag & Drop")]
    [SerializeField] private KeyCode debugKey = KeyCode.Y;

    void Update()
    {
        if (Input.GetKeyDown(debugKey))
        {
            DebugDragDropSetup();
        }
    }

    [ContextMenu("Debug Drag & Drop Setup")]
    public void DebugDragDropSetup()
    {
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log("🔍 KIỂM TRA DRAG & DROP SETUP");
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

        // 1. Kiểm tra Canvas
        var canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("❌ KHÔNG TÌM THẤY CANVAS!");
            return;
        }
        Debug.Log($"✅ Canvas: {canvas.name}");

        // 2. Kiểm tra Graphic Raycaster
        var raycaster = canvas.GetComponent<GraphicRaycaster>();
        if (raycaster == null)
        {
            Debug.LogError("❌ CANVAS THIẾU GRAPHIC RAYCASTER!");
            Debug.LogError("→ Chọn Canvas → Add Component → Graphic Raycaster");
            Debug.LogError("→ ĐÂY LÀ NGUYÊN NHÂN CHÍNH KHÔNG KÉO ĐƯỢC!");
        }
        else
        {
            Debug.Log($"✅ Graphic Raycaster: {(raycaster.enabled ? "Enabled" : "DISABLED!")}");
            if (!raycaster.enabled)
            {
                Debug.LogError("❌ Graphic Raycaster BỊ TẮT! Bật lên để kéo thả hoạt động.");
            }
        }

        // 3. Kiểm tra EventSystem
        var eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("❌ KHÔNG TÌM THẤY EVENT SYSTEM!");
            Debug.LogError("→ GameObject → UI → Event System");
            return;
        }
        Debug.Log($"✅ EventSystem: {eventSystem.name}");

        // 4. Kiểm tra tất cả items trong inventory
        if (InventorySystem.Instance == null)
        {
            Debug.LogWarning("⚠️ Không tìm thấy InventorySystem");
            return;
        }

        var inv = InventorySystem.Instance;
        Debug.Log($"\n📦 Kiểm tra items trong inventory:");

        int itemCount = 0;
        int itemsWithIssues = 0;

        foreach (var slotGO in inv.slotList)
        {
            if (slotGO == null) continue;
            
            if (slotGO.transform.childCount > 0)
            {
                var item = slotGO.transform.GetChild(0).gameObject;
                itemCount++;

                Debug.Log($"\n🔸 Item: {item.name} (Slot: {slotGO.name})");

                // Kiểm tra RectTransform
                var rt = item.GetComponent<RectTransform>();
                if (rt == null)
                {
                    Debug.LogError($"  ❌ THIẾU RectTransform!", item);
                    itemsWithIssues++;
                }
                else
                {
                    Debug.Log($"  ✅ RectTransform OK");
                }

                // Kiểm tra Image
                var img = item.GetComponent<Image>();
                if (img == null)
                {
                    Debug.LogError($"  ❌ THIẾU Image component!", item);
                    Debug.LogError($"     → Không thể click vào item!");
                    itemsWithIssues++;
                }
                else
                {
                    Debug.Log($"  ✅ Image: {(img.raycastTarget ? "Raycast Target = true" : "❌ Raycast Target = FALSE!")}");
                    if (!img.raycastTarget)
                    {
                        Debug.LogError($"     → Bật Raycast Target để click được!", item);
                        itemsWithIssues++;
                    }
                }

                // Kiểm tra CanvasGroup
                var cg = item.GetComponent<CanvasGroup>();
                if (cg == null)
                {
                    Debug.LogError($"  ❌ THIẾU CanvasGroup!", item);
                    Debug.LogError($"     → Add Component → Canvas Group", item);
                    itemsWithIssues++;
                }
                else
                {
                    Debug.Log($"  ✅ CanvasGroup: alpha={cg.alpha}, blocksRaycasts={cg.blocksRaycasts}");
                    if (!cg.blocksRaycasts)
                    {
                        Debug.LogWarning($"     ⚠️ blocksRaycasts = false (có thể không click được)");
                    }
                }

                // Kiểm tra DragDrop
                var dd = item.GetComponent<DragDrop>();
                if (dd == null)
                {
                    Debug.LogError($"  ❌ THIẾU DragDrop script!", item);
                    Debug.LogError($"     → Add Component → DragDrop", item);
                    itemsWithIssues++;
                }
                else
                {
                    Debug.Log($"  ✅ DragDrop script có");
                }
            }
        }

        Debug.Log($"\n📊 Tổng kết:");
        Debug.Log($"  - Tổng số items: {itemCount}");
        Debug.Log($"  - Items có vấn đề: {itemsWithIssues}");

        if (itemsWithIssues > 0)
        {
            Debug.LogError($"\n❌ CÓ {itemsWithIssues} ITEMS KHÔNG SETUP ĐÚNG!");
            Debug.LogError("→ Xem các lỗi trên và fix từng item.");
        }
        else if (itemCount > 0)
        {
            Debug.Log($"\n✅ TẤT CẢ ITEMS SETUP ĐÚNG!");
            
            if (raycaster == null)
            {
                Debug.LogError("\n❌ NHƯNG CANVAS THIẾU GRAPHIC RAYCASTER!");
                Debug.LogError("→ Đây là lý do không kéo được!");
            }
        }

        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
    }

    [ContextMenu("Auto Fix All Items")]
    public void AutoFixAllItems()
    {
        Debug.Log("🔧 BẮT ĐẦU AUTO FIX...");

        if (InventorySystem.Instance == null)
        {
            Debug.LogError("❌ Không tìm thấy InventorySystem!");
            return;
        }

        int fixedCount = 0;

        foreach (var slotGO in InventorySystem.Instance.slotList)
        {
            if (slotGO == null || slotGO.transform.childCount == 0) continue;

            var item = slotGO.transform.GetChild(0).gameObject;

            // Thêm CanvasGroup nếu thiếu
            var cg = item.GetComponent<CanvasGroup>();
            if (cg == null)
            {
                cg = item.AddComponent<CanvasGroup>();
                cg.blocksRaycasts = true;
                cg.alpha = 1f;
                Debug.Log($"✅ Đã thêm CanvasGroup vào {item.name}");
                fixedCount++;
            }

            // Thêm DragDrop nếu thiếu
            var dd = item.GetComponent<DragDrop>();
            if (dd == null)
            {
                item.AddComponent<DragDrop>();
                Debug.Log($"✅ Đã thêm DragDrop vào {item.name}");
                fixedCount++;
            }

            // Fix Image raycastTarget
            var img = item.GetComponent<Image>();
            if (img != null && !img.raycastTarget)
            {
                img.raycastTarget = true;
                Debug.Log($"✅ Đã bật raycastTarget cho {item.name}");
                fixedCount++;
            }
        }

        Debug.Log($"🎉 Đã fix {fixedCount} vấn đề!");
        Debug.Log("→ Chạy lại 'Debug Drag & Drop Setup' để kiểm tra");
    }
}
