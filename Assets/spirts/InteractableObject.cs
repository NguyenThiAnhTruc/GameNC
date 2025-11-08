using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("Thông tin vật phẩm")]
    public string itemName = "Vật thể";
    public string itemID = ""; // ID để phân biệt các loại vật phẩm
    
    [Tooltip("Prefab UI của item này (để hiển thị trong inventory)")]
    [SerializeField] private GameObject itemUIPrefab;

    [Header("Collider Settings")]
    [Tooltip("Collider để Raycast detect (Is Trigger = FALSE)")]
    [SerializeField] private Collider raycastCollider;
    
    [Tooltip("Collider để detect player trong vùng (Is Trigger = TRUE)")]
    [SerializeField] private Collider triggerCollider;

    private bool playerTrongVung = false;
    private Transform playerTransform;

    [Header("Detection Settings")]
    [SerializeField] private float pickupRadius = 2f;

    // Cờ để không hiển thị warning nhiều lần
    private bool hasShownPrefabWarning = false;

#if UNITY_EDITOR
    /// <summary>
    /// Validate trong Editor để cảnh báo sớm về missing prefab
    /// </summary>
    private void OnValidate()
    {
        if (itemUIPrefab == null)
        {
            Debug.LogWarning($"⚠️ [{gameObject.name}] CHƯA GÁN itemUIPrefab!\n" +
                            $"→ Hãy kéo prefab UI vào field 'Item UI Prefab' trong Inspector.\n" +
                            $"→ Vật phẩm này sẽ KHÔNG THỂ nhặt được nếu thiếu prefab!", this);
        }
        
        // Kiểm tra xem itemName có hợp lệ không
        if (string.IsNullOrWhiteSpace(itemName))
        {
            Debug.LogWarning($"⚠️ [{gameObject.name}] Item Name đang trống! Hãy đặt tên cho vật phẩm.", this);
        }
    }
#endif

    void Awake()
    {
        // Tự động tìm colliders nếu chưa assign
        if (!raycastCollider || !triggerCollider)
        {
            var colliders = GetComponents<Collider>();
            foreach (var col in colliders)
            {
                if (col.isTrigger)
                    triggerCollider = col;
                else
                    raycastCollider = col;
            }
        }
        
        // Tự động tạo ID nếu chưa có
        if (string.IsNullOrEmpty(itemID))
            itemID = itemName.ToLower().Replace(" ", "_");
    }

    void Start()
    {
        // Tìm player
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player)
            playerTransform = player.transform;
        
        // Cảnh báo runtime nếu thiếu itemUIPrefab
        if (itemUIPrefab == null && !hasShownPrefabWarning)
        {
            Debug.LogError($"❌ [{gameObject.name}] THIẾU itemUIPrefab!\n" +
                          $"→ Vật phẩm '{itemName}' sẽ KHÔNG THỂ nhặt được!\n" +
                          $"→ CÁCH SỬA: Chọn GameObject này trong Hierarchy, tìm field 'Item UI Prefab' trong Inspector và kéo prefab UI vào đó.\n" +
                          $"→ Prefab UI thường nằm trong thư mục Resources/Prefabs hoặc Assets/Prefabs.", this);
            hasShownPrefabWarning = true;
        }
        
        // Cảnh báo nếu không tìm thấy collider trigger
        if (triggerCollider == null)
        {
            Debug.LogWarning($"⚠️ [{gameObject.name}] Không tìm thấy Trigger Collider! Player có thể không vào được vùng nhặt.", this);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponentInParent<CharacterController>())
        {
            playerTrongVung = true;
            Debug.Log($"🟢 Player vào vùng nhặt của {itemName}");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponentInParent<CharacterController>())
        {
            playerTrongVung = false;
            Debug.Log($"🔴 Player rời vùng nhặt của {itemName}");
        }
    }

    void Update()
    {
        // Kiểm tra khoảng cách với player
        if (playerTransform && !playerTrongVung)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (distance <= pickupRadius)
            {
                playerTrongVung = true;
            }
        }

        if (playerTrongVung && Input.GetKeyDown(KeyCode.E))
        {
            Pickup();
        }
    }

    /// <summary>
    /// Nhặt vật phẩm và thêm vào inventory của nhân vật
    /// </summary>
    public void Pickup()
    {
        // Kiểm tra InventorySystem có tồn tại không
        if (InventorySystem.Instance == null)
        {
            Debug.LogError($"❌ [{gameObject.name}] Không tìm thấy InventorySystem trong scene!\n" +
                          $"→ Hãy đảm bảo có GameObject với script InventorySystem trong scene.", this);
            return;
        }

        // Kiểm tra có prefab UI không với hướng dẫn chi tiết
        if (itemUIPrefab == null)
        {
            Debug.LogError($"❌ Vật thể '{itemName}' ({gameObject.name}) không có itemUIPrefab! Không thể thêm vào inventory.\n" +
                          $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                          $"🔧 CÁCH SỬA LỖI NÀY:\n" +
                          $"  1. Chọn GameObject '{gameObject.name}' trong Hierarchy\n" +
                          $"  2. Tìm component 'Interactable Object' trong Inspector\n" +
                          $"  3. Kéo UI Prefab vào field 'Item UI Prefab' (trong mục 'Thông tin vật phẩm')\n" +
                          $"  4. UI Prefab thường có:\n" +
                          $"     - RectTransform component\n" +
                          $"     - Image hoặc Sprite Renderer\n" +
                          $"     - Có thể có DragDrop component\n" +
                          $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━", this);
            return;
        }

        // Thêm vào inventory UI
        bool added = InventorySystem.Instance.AddItemToInventory(itemUIPrefab, itemID, itemName);
        
        if (added)
        {
            Debug.Log($"✅ Đã nhặt '{itemName}' và thêm vào inventory");
            Destroy(gameObject); // Xóa vật phẩm khỏi thế giới game
        }
        else
        {
            Debug.Log($"❌ Inventory đã đầy! Không thể nhặt '{itemName}'");
        }
    }

    public bool IsPlayerInRange()
    {
        if (playerTrongVung)
            return true;

        if (playerTransform)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            return distance <= pickupRadius;
        }

        return false;
    }

    /// <summary>
    /// Kiểm tra xem vật phẩm này có thể nhặt được không (có đủ setup)
    /// </summary>
    public bool CanBePickedUp()
    {
        return itemUIPrefab != null && InventorySystem.Instance != null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
        
        // Vẽ cảnh báo nếu thiếu prefab
        if (itemUIPrefab == null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
        }
    }
}
