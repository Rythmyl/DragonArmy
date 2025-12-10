using UnityEngine;

public class BuffPickup : MonoBehaviour
{

    private buffs.BuffType buffType;
    private buffs buffS;

    [SerializeField] float lifetime = 15f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        
    }

    public void Initialize(buffs.BuffType type, buffs power)
    {
        buffType = type;
        buffS = power;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rythmyl"))
        { 
            buffs playerBuff = other.GetComponent<buffs>();

            if (playerBuff != null)
            { 
                playerBuff.ApplyBuff(buffType);
                Destroy(gameObject);
            }
        }
    }
}
