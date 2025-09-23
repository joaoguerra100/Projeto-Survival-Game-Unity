using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    [Header("Scripts")]
    public static Hud instance;

    [Header("Muniçao Txt")]
    public TMP_Text QtdDeMuniçaoAtual;
    public TMP_Text QtdDeMuniçaoTotal;

    [Header("Sliders")]
    [SerializeField] private Slider barraDeFome;
    [SerializeField] private Slider barraDeSede;
    [SerializeField] private Slider barraDeVida;
    [SerializeField] private Slider barraDeEstamina;

    [Header("Fome E Sede Txt")]
    public TMP_Text QtdDeFomeAtual;
    public TMP_Text QtdDeSedeAtual;

    [Header("Paineis")]
    public GameObject qtdMunicaoGo;
    [SerializeField] private GameObject[] PaineisDesativar;

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        qtdMunicaoGo.SetActive(false);
    }

    void Update()
    {
        AtualizarPorcentagemDeFomeSede();
    }

    public void AtualizarPorcentagemDeFomeSede()
    {
        QtdDeFomeAtual.text = "" + Mathf.RoundToInt(Player.instance.fomeAtual) + "%"; //ARREDONDA PARA O NUMERO INTEIRO MAIS PROXIMO
        QtdDeSedeAtual.text = "" + Mathf.RoundToInt(Player.instance.sedeAtual) + "%";
    }

    public void AtualizarContadorDeMuniçao(int municao, int municaoParaRecarregar)
    {
        QtdDeMuniçaoAtual.text = "" + municao;
        QtdDeMuniçaoTotal.text = "" + municaoParaRecarregar;

    }

    public void ChangeSlider(SliderType sliderType, float valueAtual, float valueMax)
    {
        switch (sliderType)
        {
            case SliderType.VIDA:
                barraDeVida.value = valueAtual;
                barraDeVida.maxValue = valueMax;
                break;

            case SliderType.ESTAMINA:
                barraDeEstamina.value = valueAtual;
                barraDeEstamina.maxValue = valueMax;
                break;

            case SliderType.FOME:
                barraDeFome.value = valueAtual;
                barraDeFome.maxValue = valueMax;
                break;

            case SliderType.SEDE:
                barraDeSede.value = valueAtual;
                barraDeSede.maxValue = valueMax;
                break;
        }
    }

    public void DesativarHud()
    {
        foreach (GameObject paineis in PaineisDesativar)
        {
            paineis.SetActive(false);
        }
    }
}
