using UnityEngine;

public class creditsMusic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
       

        if (audioManager.Instance != null)
        {
            
            audioManager.Instance.PlayCreditsMusic();
        }
    }
}
