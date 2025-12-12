using UnityEngine;

public class BombSpawner : MonoBehaviour
{
    [Header("---BombSetting---")]
    public GameObject bombPrefab;
    public Transform placePoint;
    public float placeDistance = 3f;
    public int bombCount = 1; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            PlaceBomb();
        }
        
    }

    void PlaceBomb()
    {
        if (bombCount <= 0)
            return;

        bombCount--;

        Vector3 spawPos = transform.position + transform.forward * placeDistance;
        spawPos.y = transform.position.y;

        GameObject boomObject  = Instantiate(bombPrefab,spawPos,Quaternion.identity);

        boomObject.GetComponent<Bomb>().Plant();

      
    }

    
    
    public void AddBomb()
    {
        bombCount++;
    }
}
