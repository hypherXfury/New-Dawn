using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    float xRotation = 0f;

    public static MouseLook Instance; // optional singleton if needed

    void Awake()
    {
        Instance = this;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    public float GetCurrentXRotation() => xRotation;

    public void SetAdditionalRotation(float addX)
    {
        float newRot = Mathf.Clamp(xRotation + addX, -90f, 90f);
        transform.localRotation = Quaternion.Euler(newRot, 0f, 0f);
    }
}
