using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Hiển thị tooltip khi hover vào item
/// Attach vào mỗi Item UI Prefab
/// </summary>
[RequireComponent(typeof(ItemData))]
public class ItemTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Tooltip Settings")]
    [SerializeField] private GameObject tooltipPrefab;
    [SerializeField] private Vector2 tooltipOffset = new Vector2(10, -10);

    private ItemData itemData;
    private GameObject tooltipInstance;
    private Canvas canvas;

    private void Awake()
    {
        itemData = GetComponent<ItemData>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Không hiển thị tooltip khi đang kéo
        if (DragDrop.itemBeingDragged != null) return;

        ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip();
    }

    private void ShowTooltip()
    {
        if (itemData == null || tooltipPrefab == null) return;

        // Tạo tooltip
        if (tooltipInstance == null)
        {
            tooltipInstance = Instantiate(tooltipPrefab, canvas.transform);
        }

        // Đặt vị trí
        var rt = tooltipInstance.GetComponent<RectTransform>();
        if (rt != null)
        {
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.mousePosition,
                canvas.worldCamera,
                out mousePos
            );
            rt.anchoredPosition = mousePos + tooltipOffset;
        }

        // Cập nhật nội dung
        var texts = tooltipInstance.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length > 0)
        {
            texts[0].text = $"<b>{itemData.itemName}</b>";
            if (texts.Length > 1 && !string.IsNullOrEmpty(itemData.itemDescription))
            {
                texts[1].text = itemData.itemDescription;
            }
        }

        tooltipInstance.SetActive(true);
    }

    private void HideTooltip()
    {
        if (tooltipInstance != null)
        {
            tooltipInstance.SetActive(false);
        }
    }

    private void OnDisable()
    {
        HideTooltip();
    }

    private void OnDestroy()
    {
        if (tooltipInstance != null)
        {
            Destroy(tooltipInstance);
        }
    }
}
