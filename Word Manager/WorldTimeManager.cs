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
    public TextMeshProUGUI clockUITxt;

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
            horaAtual -= 24f;
    }

    void UpdateLighting()
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
}
