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
    [SerializeField] private float duracaoDoDiaEmMinutos;

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
            SeasonManager.Estacao estacaoAtual = SeasonManager.instance.estacaoAtual;

            string numeroDoMes = "";
            string nomeDoMes = "";
            switch (estacaoAtual)
            {
                case SeasonManager.Estacao.Primavera:
                    numeroDoMes = "1";
                    nomeDoMes = "Primavera";
                    break;
                case SeasonManager.Estacao.Verao:
                    numeroDoMes = "2";
                    nomeDoMes = "Verão";
                    break;
                case SeasonManager.Estacao.Outono:
                    numeroDoMes = "3";
                    nomeDoMes = "Outono";
                    break;
                case SeasonManager.Estacao.Inverno:
                    numeroDoMes = "4";
                    nomeDoMes = "Inverno";
                    break;
            }
            dateUITxt.text = $"{diaDoMes:00}/{numeroDoMes:00}";
            //dateUITxt.text = $"Dia {diaDoMes:00} de {nomeDoMes}";
        }

        if (temperaturaUITxt != null)
        {
            // Formata para mostrar uma casa decimal e o símbolo de graus
            temperaturaUITxt.text = $"{ClimaManager.instance.temperaturaAtual:F1}°C";
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
