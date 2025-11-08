using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody;   // kéo thả Player vào đây
    public float mouseSensitivity = 100f;

    float xRotation = 0f; // pitch

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // pitch (nhìn lên/xuống) trên Camera
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // yaw (quay trái/phải) trên thân Player
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
