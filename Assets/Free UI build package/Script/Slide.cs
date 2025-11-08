using UnityEngine;
using UnityEngine.UI;

public class Slide : MonoBehaviour
{
    private Image filler;
    public Slider slider;

    void Start()
    {
        // Tìm Image nằm trong Fill Area (theo cấu trúc mặc định của Slider)
        filler = slider.fillRect.GetComponent<Image>();
    }

    void Update()
    {
        if (filler != null && slider != null)
            filler.fillAmount = slider.value;
    }
}
