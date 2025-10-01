using TMPro;
using UnityEngine;

public class OptionsMenuUI : MonoBehaviour
{
    public TMP_Dropdown dropdownData;
    public TMP_Dropdown dropdownTemperatura;

    void OnEnable()
    {
        if (SettingsManager.instance != null)
        {
            AtualizarUI();
        }
    }

    public void AtualizarUI()
    {
        if (dropdownData != null)
        {
            // Define o valor do dropdown para o índice salvo (0 ou 1)
            // O .SetValueWithoutNotify impede que o evento OnValueChanged seja disparado desnecessariamente
            dropdownData.SetValueWithoutNotify((int)SettingsManager.instance.currentDateFormat);
        }
        if (dropdownTemperatura != null)
        {
            dropdownTemperatura.SetValueWithoutNotify((int)SettingsManager.instance.currentTempUnit);
        }
    }


}
