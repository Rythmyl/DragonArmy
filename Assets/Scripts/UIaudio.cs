using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class UIAudio : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerDownHandler
{
    private void OnEnable()
    {
        Graphic graphic = GetComponent<Graphic>();
        if (graphic != null)
        {
            graphic.SetAllDirty();
            CanvasRenderer canvasRenderer = graphic.canvasRenderer;
            if (canvasRenderer != null)
                canvasRenderer.cull = false;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (audioManager.Instance != null)
            audioManager.Instance.PlayUIHover();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (audioManager.Instance != null)
            audioManager.Instance.PlayUIClick();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (audioManager.Instance != null)
            audioManager.Instance.PlayUIClick();
    }
}