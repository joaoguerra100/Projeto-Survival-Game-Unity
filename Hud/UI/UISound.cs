using UnityEngine;
using UnityEngine.EventSystems;

public class UISound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (AudioManager.instance.fxHoverSound != null && AudioManager.instance != null)
        {
            AudioManager.instance.MetodoSomFxHud(AudioManager.instance.fxHoverSound);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (AudioManager.instance.fxClickSound != null && AudioManager.instance != null)
        {
            AudioManager.instance.MetodoSomFxHud(AudioManager.instance.fxClickSound);
        }
    }
}
