using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;

    public enum DateFormat { DiaMes, MesDia }
    public enum TempUnit { Celsius, Fahrenheit }
    public enum ZombieDensity { Baixa, Media, Alta }
    public enum RespawnTime {nenhum, UmDia, TresDias, CincoDias, SeteDias}

    public DateFormat currentDateFormat;
    public TempUnit currentTempUnit;
    public ZombieDensity currentZombieDensity;
    public RespawnTime currentRespawnTime;

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

    public void SetZombieDensity(int densityIndex)
    {
        currentZombieDensity = (ZombieDensity)densityIndex;
        SaveSettings();
    }

    public void SetRespawnTime(int timeIndex)
    {
        currentRespawnTime = (RespawnTime)timeIndex;
        SaveSettings();
    }

    /* Logicas de salvar e carregar */
    void SaveSettings()
    {
        PlayerPrefs.SetInt("DateFormat", (int)currentDateFormat);
        PlayerPrefs.SetInt("TempUnit", (int)currentTempUnit);
        PlayerPrefs.SetInt("ZombieDensity", (int)currentZombieDensity);
        PlayerPrefs.SetInt("RespawnTime", (int)currentRespawnTime);
        PlayerPrefs.Save();
    }
    void LoadSettings()
    {
        // Carrega o formato de data, usando Dia/Mês (0) como padrão se nada for encontrado
        currentDateFormat = (DateFormat)PlayerPrefs.GetInt("DateFormat", 0);
        // Carrega a unidade de temperatura, usando Celsius (0) como padrão
        currentTempUnit = (TempUnit)PlayerPrefs.GetInt("TempUnit", 0);
        // Carrega a densidade, usando "Media" (1) como padrão
        currentZombieDensity = (ZombieDensity)PlayerPrefs.GetInt("ZombieDensity", 1);
        // Carrega o tempo de respawn, usando "TresDias" (2) como padrão
        currentRespawnTime = (RespawnTime)PlayerPrefs.GetInt("RespawnTime", 2);
    }
}
