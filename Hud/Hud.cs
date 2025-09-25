using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    [Header("Scripts")]
    public static Hud instance;
    private CharacterStats playerStats;

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
        playerStats = Player.instance.stats;
        qtdMunicaoGo.SetActive(false);
        playerStats.OnVidaMudou += AtualizarBarraDeVida;
        playerStats.OnEstaminaMudou += AtualizarBarraDeEstamina;
        playerStats.OnFomeMudou += AtualizarBarraDeFome;
        playerStats.OnSedeMudou += AtualizarBarraDeSede;
    }

    void Update()
    {
        AtualizarPorcentagemDeFomeSede();
    }

    //E chamado quando o objeto e destruido
    private void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnVidaMudou -= AtualizarBarraDeVida;
            playerStats.OnEstaminaMudou -= AtualizarBarraDeEstamina;
            playerStats.OnFomeMudou -= AtualizarBarraDeFome;
            playerStats.OnSedeMudou -= AtualizarBarraDeSede;
        }
    }

    public void AtualizarPorcentagemDeFomeSede()
    {
        QtdDeFomeAtual.text = "" + Mathf.RoundToInt(Player.instance.stats.fomeAtual) + "%"; //ARREDONDA PARA O NUMERO INTEIRO MAIS PROXIMO
        QtdDeSedeAtual.text = "" + Mathf.RoundToInt(Player.instance.stats.sedeAtual) + "%";
    }

    public void AtualizarContadorDeMuniçao(int municao, int municaoParaRecarregar)
    {
        QtdDeMuniçaoAtual.text = "" + municao;
        QtdDeMuniçaoTotal.text = "" + municaoParaRecarregar;

    }

    #region AtualizarStatus

    private void AtualizarBarraDeVida(float atual, float maximo)
    {
        barraDeVida.maxValue = maximo;
        barraDeVida.value = atual;
    }

    private void AtualizarBarraDeEstamina(float atual, float maximo)
    {
        barraDeEstamina.maxValue = maximo;
        barraDeEstamina.value = atual;
    }

    private void AtualizarBarraDeFome(float atual, float maximo)
    {
        barraDeFome.maxValue = maximo;
        barraDeFome.value = atual;
        QtdDeFomeAtual.text = "" + Mathf.RoundToInt(atual) + "%";
    }

    private void AtualizarBarraDeSede(float atual, float maximo)
    {
        barraDeSede.maxValue = maximo;
        barraDeSede.value = atual;
        QtdDeSedeAtual.text = "" + Mathf.RoundToInt(atual) + "%";
    }
    #endregion

    public void DesativarHud()
    {
        foreach (GameObject paineis in PaineisDesativar)
        {
            paineis.SetActive(false);
        }
    }
}
