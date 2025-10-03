using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfigController : MonoBehaviour
{
    public ScrollRect myScrollRect;
    public List<GameObject> paineisDeConfig;
    public GameObject painelInicial;

    [Header("Dropdowns de Opções")]
    public TMP_Dropdown dropdownData;
    public TMP_Dropdown dropdownTemperatura;

    void OnEnable()
    {
        if (painelInicial != null)
        {
            SwitchToPanel(painelInicial);
        }

        AtualizarOpcoesUI();
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
            if (painelParaMostrar.name == "OpçõesPainel")
            {
                AtualizarOpcoesUI();
            }
        }
    }

    IEnumerator ScrollToTopNextFrame()
    {
        yield return new WaitForEndOfFrame();
        myScrollRect.verticalNormalizedPosition = 1f;
    }

    void AtualizarOpcoesUI()
    {
        if (SettingsManager.instance != null)
        {
            if (dropdownData != null)
            {
                dropdownData.SetValueWithoutNotify((int)SettingsManager.instance.currentDateFormat);
            }

            if (dropdownTemperatura != null)
            {
                dropdownTemperatura.SetValueWithoutNotify((int)SettingsManager.instance.currentTempUnit);
            }
        }
    }
}
