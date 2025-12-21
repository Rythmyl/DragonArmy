using UnityEngine;

public class BombInputManager : MonoBehaviour
{
    public BombThrower turret1;
    public BombThrower turret2;

    public GameObject bombF;
    public GameObject bombG;
  
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.F))
        {
            turret1.ThrowBomb(bombF);
        }

        if(Input.GetKeyUp(KeyCode.G))
        {
            turret2.ThrowBomb(bombG);
        }
    }
}
