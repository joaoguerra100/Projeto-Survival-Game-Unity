using TMPro;
using UnityEngine;

public class WorldTimeManager : MonoBehaviour
{
    #region Declaration

    public static WorldTimeManager instance;

    [Header("Referencias")]
    public Light diretionalLight;
    public Gradient lightColorOverTime;
    public AnimationCurve lightIntensityOverTime;

    [Header("Configuraçoes de RelogioTxt")]
    public TextMeshProUGUI clockUITxt;
    public TextMeshProUGUI dateUITxt;
    public TextMeshProUGUI temperaturaUITxt;
    [Header("Configuraçoes de tempo")]
    [Range(0f, 24f)][SerializeField] private float horaAtual;
    [SerializeField] private float duracaoDoDiaEmMinutos = 40f;

    [Header("Boolenas de ajuda")]
    public bool tempoParado;
    public bool isNight;

    private float timeMultiplier;

    #endregion


    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        timeMultiplier = 24f / (duracaoDoDiaEmMinutos * 60f); //Calcula quantas horas passam por segundo
    }

    void Update()
    {
        if (tempoParado) return;
        UpdateTime();
        UpdateLighting();
        UpdateUI();
        UpdateNight();
    }

    void UpdateTime()
    {
        horaAtual += Time.deltaTime * timeMultiplier;
        if (horaAtual >= 24f)
        {
            horaAtual -= 24f;
            SeasonManager.instance.AvancarDia();
        }
    }

    public void UpdateLighting()
    {
        float timeNormalized = horaAtual / 24f;

        //Rotaciona a luza direcional(sol)
        diretionalLight.transform.rotation = Quaternion.Euler((timeNormalized * 360f) - 90f, 170f, 0);

        //Ajusta cor e intensidade da luz ao longo do tempo
        diretionalLight.color = lightColorOverTime.Evaluate(timeNormalized);
        diretionalLight.intensity = lightIntensityOverTime.Evaluate(timeNormalized);
    }

    void UpdateUI()
    {
        if (clockUITxt != null)
        {
            int hours = Mathf.FloorToInt(horaAtual);
            int minutes = Mathf.FloorToInt((horaAtual - hours) * 60f);
            clockUITxt.text = $"{hours:00}:{minutes:00}";
        }

        if (dateUITxt != null && SeasonManager.instance != null)
        {
            int diaDoMes = SeasonManager.instance.diaAtualDaEstacao;
            int numeroDoMes = (int)SeasonManager.instance.estacaoAtual + 1;

            // Verifica a configuração salva no SettingsManager
            if (SettingsManager.instance.currentDateFormat == SettingsManager.DateFormat.DiaMes)
            {
                dateUITxt.text = $"{diaDoMes:00}/{numeroDoMes:00}"; // Formato: 01/03
            }
            else // Se for MesDia
            {
                dateUITxt.text = $"{numeroDoMes:00}/{diaDoMes:00}"; // Formato: 03/01
            }
        }

        if (temperaturaUITxt != null && ClimaManager.instance != null)
        {
            // Pega a temperatura atual, que está sempre em Celsius no ClimaManager
            float celsius = ClimaManager.instance.temperaturaAtual;
            
            // Verifica a configuração salva no SettingsManager
            if (SettingsManager.instance.currentTempUnit == SettingsManager.TempUnit.Celsius)
            {
                temperaturaUITxt.text = $"{celsius:F1}°C"; // Exibe em Celsius
            }
            else // Se for Fahrenheit
            {
                // Converte Celsius para Fahrenheit
                float fahrenheit = (celsius * 9 / 5f) + 32;
                temperaturaUITxt.text = $"{fahrenheit:F1}°F"; // Exibe em Fahrenheit
            }
        }
    }

    void UpdateNight()
    {
        if (horaAtual >= 18f || horaAtual <= 6f)
        {
            isNight = true;
        }
        else
        {
            isNight = false;
        }
    }

    public float GetHoraAtual()
    {
        return horaAtual;
    }
}
