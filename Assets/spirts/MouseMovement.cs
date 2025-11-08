using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    float xRotation = 0f;
    float yRotation = 0f;

    void Start()
    {
        // Khóa con trỏ chuột vào giữa màn hình và ẩn nó đi
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Điều khiển xoay quanh trục X (nhìn lên và xuống)
        xRotation -= mouseY;

        // Giới hạn góc xoay để không bị xoay quá mức (giống ngoài đời thật)
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Điều khiển xoay quanh trục Y (nhìn trái và phải)
        yRotation += mouseX;

        // Áp dụng hai phép xoay
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
