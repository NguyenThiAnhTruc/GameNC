using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Canvas canvas;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    public static GameObject itemBeingDragged;
    public static Transform itemBeingDraggedFrom;

    private Vector3 startPosition;
    private Transform startParent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
        
        // Validation
        if (rectTransform == null)
        {
            Debug.LogError($"❌ [{gameObject.name}] THIẾU RectTransform! Item không thể kéo được.", this);
        }
        
        if (canvasGroup == null)
        {
            Debug.LogError($"❌ [{gameObject.name}] THIẾU CanvasGroup! Item không thể kéo được.", this);
            Debug.LogError($"→ Add Component → Canvas Group", this);
        }
        
        if (canvas == null)
        {
            Debug.LogError($"❌ [{gameObject.name}] Không tìm thấy Canvas! Item không thể kéo được.", this);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasGroup == null || canvas == null || rectTransform == null)
        {
            Debug.LogError($"❌ [{gameObject.name}] Không thể kéo - thiếu component!", this);
            return;
        }

        Debug.Log($"🖱️ Bắt đầu kéo: {gameObject.name}");

        // Hiện chuột trong khi kéo (đặc biệt nếu game ẩn cursor khi chơi)
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        startParent = transform.parent;
        startPosition = transform.position;

        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;

        // Đưa item lên Canvas gốc để hiển thị trên cùng
        transform.SetParent(canvas.transform, true);

        itemBeingDragged = gameObject;
        itemBeingDraggedFrom = startParent;
        
        Debug.Log($"  → Item được kéo từ: {startParent.name}");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rectTransform == null || canvas == null) return;
        
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup == null)
        {
            Debug.LogError($"❌ [{gameObject.name}] CanvasGroup null khi kết thúc kéo!", this);
            return;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Nếu không thả vào slot hợp lệ -> quay lại chỗ cũ
        if (transform.parent == canvas.transform)
        {
            Debug.Log($"↩️ Item {gameObject.name} quay lại slot cũ (không thả vào slot hợp lệ)");
            transform.position = startPosition;
            transform.SetParent(startParent);
        }
        else
        {
            Debug.Log($"✅ Item {gameObject.name} đã được thả vào: {transform.parent.name}");
        }

        itemBeingDragged = null;
        itemBeingDraggedFrom = null;
    }
}
