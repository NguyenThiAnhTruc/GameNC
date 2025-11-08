using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// InventorySlot - Ô trong bảng inventory của nhân vật
/// Xử lý việc thả item vào ô và hoán đổi giữa các ô
/// </summary>
public class InventorySlot : MonoBehaviour, IDropHandler
{
    public GameObject Item
    {
        get
        {
            if (transform.childCount > 0)
                return transform.GetChild(0).gameObject;
            return null;
        }
    }

    public bool IsEmpty => transform.childCount == 0;

    public void OnDrop(PointerEventData eventData)
    {
        if (DragDrop.itemBeingDragged == null)
            return;

        Debug.Log($"📦 Item được thả vào InventorySlot: {gameObject.name}");

        // Gọi InventorySystem để xử lý swap/move
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.HandleItemDrop(this);
        }
        else
        {
            Debug.LogError("❌ Không tìm thấy InventorySystem!");
        }
    }
}
