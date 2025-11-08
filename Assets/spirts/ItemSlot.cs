using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
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
        GameObject droppedItem = DragDrop.itemBeingDragged;
        
        if (droppedItem == null) return;

        Transform originalSlot = DragDrop.itemBeingDraggedFrom;
        if (originalSlot == null) return;

        Debug.Log($"📦 Item {droppedItem.name} được thả vào ItemSlot: {gameObject.name}");

        // Kiểm tra xem item có đang được kéo từ InventorySlot không
        bool isFromInventory = originalSlot.GetComponent<InventorySlot>() != null;

        // Nếu ô này trống, đặt item vào
        if (IsEmpty)
        {
            PlaceItemInSlot(droppedItem);
            Debug.Log($"✅ Đã đặt {droppedItem.name} vào slot trống {gameObject.name}");
        }
        // Nếu ô đã có item, hoán đổi
        else
        {
            SwapWithExistingItem(droppedItem, originalSlot);
            Debug.Log($"🔄 Hoán đổi {droppedItem.name} với {Item.name}");
        }
    }

    /// <summary>
    /// Đặt item vào slot trống
    /// </summary>
    private void PlaceItemInSlot(GameObject item)
    {
        item.transform.SetParent(transform);
        
        RectTransform rt = item.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;
        }
        else
        {
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// Hoán đổi với item đã có trong slot
    /// </summary>
    private void SwapWithExistingItem(GameObject droppedItem, Transform originalSlot)
    {
        GameObject existingItem = Item;

        // Đặt item đang kéo vào slot này
        PlaceItemInSlot(droppedItem);

        // Đặt item cũ vào slot gốc
        existingItem.transform.SetParent(originalSlot);
        
        RectTransform rt = existingItem.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;
        }
        else
        {
            existingItem.transform.localPosition = Vector3.zero;
            existingItem.transform.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// Xóa item khỏi slot này
    /// </summary>
    public void ClearSlot()
    {
        if (Item != null)
        {
            Destroy(Item);
        }
    }
}
