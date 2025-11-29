using UnityEngine;

public class cameraController : MonoBehaviour
{
    [Header("----- Camera Settings -----")]
    [Range(1, 100)] [SerializeField] int sens;

    [Header("----- Vertical Clamp -----")]
    [Range(-90, 0)] [SerializeField] int lockVertMin;
    [Range(0, 90)] [SerializeField] int lockVertMax;

    [Header("----- Options -----")]
    [SerializeField] bool invertY;

    float camRotX;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sens * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sens * Time.deltaTime;

        if (invertY)
            camRotX += mouseY;
        else
            camRotX -= mouseY;

        camRotX = Mathf.Clamp(camRotX, lockVertMin, lockVertMax);

        transform.localRotation = Quaternion.Euler(camRotX, 0, 0);
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
