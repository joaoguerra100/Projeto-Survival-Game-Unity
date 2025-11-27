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
    public Material ceuMaterial;

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
    private bool eraNoite;

    [Header("Iluminação Noturna (Lua)")]
    public float intensidadeLuaNova = 0.01f;
    public float intensidadeLuaCrescente = 0.05f;
    public float intensidadeLuaMinguante = 0.05f;
    public float intensidadeLuaCheia = 0.12f;

    [Header("Referências Lua")]
    public Transform moonTransform;
    public float skyDistance = 1000f; // Distância da lua em relação ao centro do mundo
    private Transform cameraTransform;

    private float timeMultiplier;

    #endregion


    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        timeMultiplier = 24f / (duracaoDoDiaEmMinutos * 60f); //Calcula quantas horas passam por segundo
        cameraTransform = Camera.main.transform;
        UpdateNight();
        eraNoite = isNight;
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

    #region Controle da luz e tempo
    public void UpdateLighting()
    {
        float timeNormalized = horaAtual / 24f;
        Quaternion lightRotation = Quaternion.Euler((timeNormalized * 360f) - 90f, 170f, 0);

        // --- LUZ DIRECIONAL (SOL/LUA) ---
        diretionalLight.transform.rotation = lightRotation;
        Color lightColor = lightColorOverTime.Evaluate(timeNormalized);
        float finalIntensity = lightIntensityOverTime.Evaluate(timeNormalized);
        diretionalLight.color = lightColor; // Aplica a cor base

        // --- LÓGICA DA LUZ DA LUA ---
        float intensidadeLuzDaLua = 0f;
        SeasonManager.FaseDaLua fase = SeasonManager.instance.faseDaLuaAtual;
        if (isNight && SeasonManager.instance != null)
        {
            switch (fase)
            {
                case SeasonManager.FaseDaLua.Nova: intensidadeLuzDaLua = intensidadeLuaNova; break;
                case SeasonManager.FaseDaLua.Crescente: intensidadeLuzDaLua = intensidadeLuaCrescente; break;
                case SeasonManager.FaseDaLua.Cheia: intensidadeLuzDaLua = intensidadeLuaCheia; break;
                case SeasonManager.FaseDaLua.Minguante: intensidadeLuzDaLua = intensidadeLuaMinguante; break;
            }
            finalIntensity += intensidadeLuzDaLua;
        }
        diretionalLight.intensity = finalIntensity;

        //CONTROLE DO MATERIAL DO CÉU (Skybox/Procedural)
        ControleMaterialCel(timeNormalized);

        // --- CONTROLE DA NEBLINA ---
        ControleNeblina();

        // --- MOVIMENTO DA LUA FÍSICA ---
        MovimentoDaLua(lightRotation);
    }

    void ControleMaterialCel(float timeNormalized)
    {
        Color lightColor = lightColorOverTime.Evaluate(timeNormalized);
        SeasonManager.FaseDaLua fase = SeasonManager.instance.faseDaLuaAtual;
        if (ceuMaterial != null)
        {
            if (isNight)
            {
                float skyExposure;
                Color skyTint;
                switch (fase)
                {
                    case SeasonManager.FaseDaLua.Nova:
                        skyExposure = 0f;
                        skyTint = Color.black;
                        break;
                    case SeasonManager.FaseDaLua.Crescente:
                    case SeasonManager.FaseDaLua.Minguante:
                        skyExposure = 0.03f;
                        skyTint = new Color(0.04f, 0.04f, 0.1f); // Azul bem escuro
                        break;
                    case SeasonManager.FaseDaLua.Cheia:
                    default:
                        skyExposure = 0.1f;
                        skyTint = new Color(0.08f, 0.08f, 0.18f); // Azul noturno
                        break;
                }
                ceuMaterial.SetFloat("_Exposure", skyExposure);
                ceuMaterial.SetColor("_SkyTint", skyTint);
            }
            else
            {
                ceuMaterial.SetColor("_SkyTint", lightColor);
                float baseIntensityDay = lightIntensityOverTime.Evaluate(timeNormalized);
                ceuMaterial.SetFloat("_Exposure", Mathf.Clamp(baseIntensityDay, 0.5f, 1.5f));
            }
        }
    }

    void ControleNeblina()
    {
        SeasonManager.FaseDaLua fase = SeasonManager.instance.faseDaLuaAtual;
        if (isNight)
        {
            RenderSettings.fogColor = Color.black;
            switch (fase)
            {
                case SeasonManager.FaseDaLua.Nova:
                    RenderSettings.fogDensity = 0.06f; // Mais densa
                    break;
                case SeasonManager.FaseDaLua.Crescente:
                case SeasonManager.FaseDaLua.Minguante:
                    RenderSettings.fogDensity = 0.04f; // Média
                    break;
                case SeasonManager.FaseDaLua.Cheia:
                default:
                    RenderSettings.fogDensity = 0.025f; // Menos densa
                    break;
            }
        }
        else
        {
            RenderSettings.fogColor = new Color(0.5f, 0.5f, 0.5f);
            RenderSettings.fogDensity = 0.01f;
        }
    }

    void MovimentoDaLua(Quaternion lightRotation)
    {
        if (moonTransform != null && cameraTransform != null)
        {
            moonTransform.position = cameraTransform.position + (lightRotation * Vector3.forward * skyDistance);
            moonTransform.LookAt(cameraTransform.position);
        }
    }

    #endregion

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
        bool noiteAtual;

        if (horaAtual >= 18f || horaAtual <= 6f)
        {
            noiteAtual = true;
        }
        else
        {
            noiteAtual = false;
        }

        if (noiteAtual != isNight)
        {
            isNight = noiteAtual;
            AudioManager.instance.AtualizarSomAmbienteDetalhes();
        }
    }

    public float GetHoraAtual()
    {
        return horaAtual;
    }

    public float GetSegundosReaisPorDia()
    {
        return duracaoDoDiaEmMinutos * 60f;
    }
}
