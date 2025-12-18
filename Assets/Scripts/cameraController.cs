using UnityEngine;

public class cameraController : MonoBehaviour
{
    public Transform discTransform; 
    public float mouseSensitivity = 2f;

    private float xRotation = 90f;

    void Start()
    {
        transform.rotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, 0f, 90f);

        float discYRotation = discTransform.eulerAngles.y;

        transform.rotation = Quaternion.Euler(xRotation, discYRotation, 0f);
    }
}
