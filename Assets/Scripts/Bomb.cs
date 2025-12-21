using UnityEngine;

public class Bomb : MonoBehaviour
{

    public float lifetime = 3f;

    private void Start()
    {
        Destroy(gameObject,lifetime);
    }

}
