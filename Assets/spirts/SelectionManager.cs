using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionManager : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float maxDistance = 7f;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private QueryTriggerInteraction triggerRule = QueryTriggerInteraction.Collide;
    [SerializeField] private bool rayFromScreenCenter = true;

    [Header("UI")]
    [SerializeField] private GameObject interaction_Info_UI;
    [SerializeField] private TextMeshProUGUI uiTextTMP;

    private Camera cam;
    private InteractableObject currentTarget;

    void Awake()
    {
        cam = Camera.main;
        if (!uiTextTMP && interaction_Info_UI)
            uiTextTMP = interaction_Info_UI.GetComponentInChildren<TextMeshProUGUI>(true);

        HideUI();
    }

    void Update()
    {
        if (!cam) { cam = Camera.main; if (!cam) return; }

        Ray ray = rayFromScreenCenter
            ? cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0))
            : cam.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance, interactableMask, triggerRule))
        {
            Debug.Log($"🎯 Raycast hit: {hit.transform.name} on layer {LayerMask.LayerToName(hit.transform.gameObject.layer)}");
            
            var interactable = hit.transform.GetComponent<InteractableObject>();
            if (interactable)
            {
                // Kiểm tra xem vật phẩm có thể nhặt được không
                bool canPickup = interactable.CanBePickedUp();
                
                if (canPickup)
                {
                    ShowUI(interactable.itemName, true);
                }
                else
                {
                    // Hiển thị cảnh báo nếu vật phẩm không có setup đầy đủ
                    ShowUI(interactable.itemName, false);
                }
                
                currentTarget = interactable;
                
                // Kiểm tra phím E khi nhìn vào vật thể
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log($"🔵 Nhấn E - Player trong vùng: {interactable.IsPlayerInRange()}");
                    
                    if (!canPickup)
                    {
                        Debug.LogError($"❌ Không thể nhặt '{interactable.itemName}' - vật phẩm chưa được setup đầy đủ! (Thiếu itemUIPrefab)", interactable);
                        return;
                    }
                    
                    if (interactable.IsPlayerInRange())
                    {
                        interactable.Pickup();
                    }
                    else
                    {
                        Debug.LogWarning("⚠️ Player chưa vào vùng trigger! Hãy lại gần hơn.");
                    }
                }
                return;
            }
            else
            {
                Debug.LogWarning($"⚠️ {hit.transform.name} không có InteractableObject script!");
            }
        }

        HideUI();
        currentTarget = null;
    }

    private void ShowUI(string itemName, bool canPickup = true)
    {
        if (interaction_Info_UI)
        {
            interaction_Info_UI.SetActive(true);
            if (uiTextTMP)
            {
                if (canPickup)
                {
                    uiTextTMP.text = $"[E] Nhặt {itemName}";
                    uiTextTMP.color = Color.white;
                }
                else
                {
                    uiTextTMP.text = $"[!] {itemName} (THIẾU SETUP)";
                    uiTextTMP.color = Color.red;
                }
            }
        }
    }

    private void HideUI()
    {
        if (interaction_Info_UI)
            interaction_Info_UI.SetActive(false);
    }
}
