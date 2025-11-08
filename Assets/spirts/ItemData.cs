using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Component lưu trữ và hiển thị thông tin item trong inventory
/// Attach vào mỗi Item UI Prefab
/// </summary>
public class ItemData : MonoBehaviour
{
    [Header("Thông tin Item")]
    [Tooltip("Tên hiển thị của item")]
    public string itemName = "Item";
    
    [Tooltip("ID duy nhất của item")]
    public string itemID = "";
    
    [Tooltip("Icon/Sprite của item")]
    public Sprite itemIcon;
    
    [Tooltip("Mô tả item")]
    [TextArea(2, 4)]
    public string itemDescription = "";
    
    [Tooltip("Số lượng item (nếu có thể stack)")]
    public int quantity = 1;

    [Header("UI References (Tự động tìm)")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI quantityText;

    private void Awake()
    {
        // Tự động tìm components nếu chưa gán
        if (iconImage == null)
            iconImage = GetComponent<Image>();
        
        if (nameText == null)
            nameText = GetComponentInChildren<TextMeshProUGUI>();
        
        // Cập nhật UI
        UpdateUI();
    }

    /// <summary>
    /// Khởi tạo item với thông tin
    /// </summary>
    public void Initialize(string name, string id, Sprite icon = null, int qty = 1)
    {
        itemName = name;
        itemID = id;
        itemIcon = icon;
        quantity = qty;
        
        UpdateUI();
    }

    /// <summary>
    /// Cập nhật hiển thị UI
    /// </summary>
    public void UpdateUI()
    {
        // Cập nhật icon
        if (iconImage != null && itemIcon != null)
        {
            iconImage.sprite = itemIcon;
            iconImage.enabled = true;
        }
        else if (iconImage != null)
        {
            // Nếu không có icon, hiển thị màu mặc định
            iconImage.enabled = true;
        }

        // Cập nhật tên
        if (nameText != null)
        {
            nameText.text = itemName;
        }

        // Cập nhật số lượng
        if (quantityText != null)
        {
            if (quantity > 1)
            {
                quantityText.text = quantity.ToString();
                quantityText.gameObject.SetActive(true);
            }
            else
            {
                quantityText.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Thay đổi số lượng
    /// </summary>
    public void SetQuantity(int newQuantity)
    {
        quantity = Mathf.Max(1, newQuantity);
        UpdateUI();
    }

    /// <summary>
    /// Thêm số lượng
    /// </summary>
    public void AddQuantity(int amount)
    {
        SetQuantity(quantity + amount);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Tự động cập nhật UI khi thay đổi trong Editor
        if (Application.isPlaying)
        {
            UpdateUI();
        }
    }
#endif
}
