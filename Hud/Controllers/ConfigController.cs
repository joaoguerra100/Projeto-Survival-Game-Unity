using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigController : MonoBehaviour
{
    public ScrollRect myScrollRect;
    public List<GameObject> paineisDeConfig;
    public GameObject painelInicial;

    void OnEnable()
    {
        if (painelInicial != null)
        {
            SwitchToPanel(painelInicial);
        }
    }

    void Start()
    {

    }


    void Update()
    {

    }
    public void SwitchToPanel(GameObject painelParaMostrar)
    {
        foreach (GameObject painel in paineisDeConfig)
        {
            if (painel != null)
            {
                painel.SetActive(false);
            }
        }

        if (painelParaMostrar != null)
        {
            painelParaMostrar.SetActive(true);
            if (painelParaMostrar.name == "TeclasPainel" && myScrollRect != null)
            {
                StartCoroutine(ScrollToTopNextFrame());
            }
        }
    }
    
    IEnumerator ScrollToTopNextFrame()
    {
        yield return new WaitForEndOfFrame();
        myScrollRect.verticalNormalizedPosition = 1f;
    }
}
