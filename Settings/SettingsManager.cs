using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;

    public enum DateFormat { DiaMes, MesDia }
    public enum TempUnit { Celsius, Fahrenheit }

    public DateFormat currentDateFormat;
    public TempUnit currentTempUnit;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    /* Metodos dos botoes  */
    public void SetDateFormat(int formatIndex)
    {
        currentDateFormat = (DateFormat)formatIndex;
        SaveSettings();
    }

    public void SetTemperaturaUnit(int unitIndex)
    {
        currentTempUnit = (TempUnit)unitIndex;
        SaveSettings();
    }

    /* Logicas de salvar e carregar */
    void SaveSettings()
    {
        PlayerPrefs.SetInt("DateFormat", (int)currentDateFormat);
        PlayerPrefs.SetInt("TempUnit", (int)currentTempUnit);
        PlayerPrefs.Save();
    }
    void LoadSettings()
    {
        // Carrega o formato de data, usando Dia/Mês (0) como padrão se nada for encontrado
        currentDateFormat = (DateFormat)PlayerPrefs.GetInt("DateFormat", 0);
        // Carrega a unidade de temperatura, usando Celsius (0) como padrão
        currentTempUnit = (TempUnit)PlayerPrefs.GetInt("TempUnit", 0);
    }
}
