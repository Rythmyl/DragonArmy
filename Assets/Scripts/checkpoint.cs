using UnityEngine;
using System.Collections;

public class checkpoint : MonoBehaviour
{
    [SerializeField] Renderer model;
    Color colorOrig;

    void Start()
    {
        colorOrig = model.material.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rythmyl"))
        {
            // Update the checkpoint position in gamemanager
            gamemanager.instance.UpdateCheckpoint(transform.position);
            StartCoroutine(feedback());
        }
    }

    IEnumerator feedback()
    {
        model.material.color = Color.red;
        gamemanager.instance.checkpointPopup.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        gamemanager.instance.checkpointPopup.SetActive(false);
        model.material.color = colorOrig;
    }
}
