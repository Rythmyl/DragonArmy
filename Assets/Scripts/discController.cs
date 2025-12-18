using UnityEngine;

public class discController : MonoBehaviour
{
    public float rotationSpeed = 100f;

    void Update()
    {
        float horizontalInput = 0f;

        if (Input.GetKey(KeyCode.D))
            horizontalInput = 1f; 
        else if (Input.GetKey(KeyCode.A))
            horizontalInput = -1f;

        transform.Rotate(0f, 0f, horizontalInput * rotationSpeed * Time.deltaTime);
    }
}