using System.Linq;
using UnityEngine;

public class PlayerBracos : MonoBehaviour
{
    #region Variaveis
    [Header("Scripts")]
    public static PlayerBracos instance;
    private Arma arma;
    private ArmaMelee armaMelee;

    [Header("Referencias")]
    [HideInInspector] public Animator anim;

    [Header("Modo De Tiro")]
    [HideInInspector] public bool carregandoArma;

    [Header("Animator")]
    public RuntimeAnimatorController[] controllerMelee;
    public RuntimeAnimatorController controllerDesarmado;
    public RuntimeAnimatorController[] controllerArmaDeFogo;

    [Header("Paineis")]
    public GameObject crosshairPanel;
    public bool visiblePanel;

    [Header("ModoDesarmado")]
    [HideInInspector] public bool estaDesarmado;

    #endregion

    #region Methods

    void Awake()
    {
        visiblePanel = true;
        visiblePanel = crosshairPanel.activeSelf;
        instance = this;
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        arma = FindAnyObjectByType<Arma>();
        armaMelee = FindAnyObjectByType<ArmaMelee>();
    }

    void Update()
    {
        if (Player.instance.morte) { return; }
        //Chutar();
        Cheats();
        if (PodeAgirComArma())
        {
            bool isMeleeController = controllerMelee.Contains(anim.runtimeAnimatorController);
            bool isFirearmController = controllerArmaDeFogo.Contains(anim.runtimeAnimatorController);
            if (arma != null && isFirearmController)
            {
                arma.TrocarModoDeTiro();
                arma.CarregarArma();
                if (carregandoArma == false)
                {
                    arma.Atirar();
                }
            }
            else if (armaMelee != null && isMeleeController)
            {
                armaMelee.HandleAttackInput();
            }
        }
    }
    #endregion

    public void ShowAndHideCrossHair()
    {
        visiblePanel = !visiblePanel;
        crosshairPanel.SetActive(visiblePanel);
    }

    void Cheats()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (arma != null)
                arma.ligarGzimos = !arma.ligarGzimos;

            else if (armaMelee != null)
            {

            }
        }

        if (Input.GetKeyDown(KeyCode.K) && arma != null)
        {
            arma.weaponData.municaoParaRecarregar += 30;
        }
    }

    private bool PodeAgirComArma()
    {
        return !estaDesarmado &&
               !PauseController.instance.visiblePanel &&
               !InventoryView.instance.VisiblePanel &&
               Player.instance.bloqueioControle == false &&
               Player.instance.trocaAnimator == false;
    }

    private void Chutar()
    {
        if (Input.GetKeyDown(KeyCode.F) && PodeChutar())
        {
            anim.SetTrigger("Chutar");
            Player.instance.anim.SetTrigger("Chutar");
            HandCombat.instance.isAttacking = true;
        }
    }

    private bool PodeChutar()
    {
        return Player.instance.estaNochao && Player.instance.bloqueioControle == false && Player.instance.trocaAnimator == false
        && carregandoArma == false && HandCombat.instance.isAttacking == false;
    }

    public void SetarArma(Arma novaArma)
    {
        arma = novaArma;
        armaMelee = null;
        estaDesarmado = false;
    }

    public void SetarArmaMelee(ArmaMelee novaArmaMelee)
    {
        armaMelee = novaArmaMelee;
        arma = null;
        estaDesarmado = false;
    }

    public void PrepararVelocidadeDeAtaque()
    {
        if (armaMelee == null) return;

        // --- 1. Multiplicador de Estamina ---
        // Mapeia a estamina atual (0 a 100) para uma faixa de velocidade (ex: 60% a 100%)
        // Com estamina cheia, a velocidade é normal (1.0). Com estamina vazia, é 40% (0.4).
        float minSpeedPorEstamina = 0.4f;
        float maxSpeedPorEstamina = 1.0f;
        float porcentagemEstamina = Player.instance.stats.estaminaAtual / Player.instance.stats.estaminaMax;
        float multEstamina = Mathf.Lerp(minSpeedPorEstamina, maxSpeedPorEstamina, porcentagemEstamina);

        // --- 2. Multiplicador de Peso da Arma ---
        // A velocidade é o inverso do peso. Peso 1 = vel. 1. Peso 2 = vel. 0.5. Peso 0.8 = vel. 1.25.
        float multPeso = 1f / armaMelee.weaponData.pesoArma;

        // --- 3. Multiplicador de Força ---
        // Ex: A cada 10 pontos de força, ganha 10% de velocidade. Força base 10 = 1.0.
        //para cada ponto de força acima de 10, você ganha 1% de bônus de velocidade. 1 de força e igual a 1%
        //A força 10 e o numero "padrao" onde nao tem buff e nem debuff
        float multForca = 1f + ((Player.instance.stats.forca - 10f) * 0.01f);

        // --- Cálculo Final ---
        float velocidadeFinal = multEstamina * multPeso * multForca;

        // Garante que a velocidade não seja negativa
        if (velocidadeFinal < 0.1f)
        {
            velocidadeFinal = 0.1f;
        }

        // Aplica a velocidade ao animator!
        anim.speed = velocidadeFinal;
    }

    public void ResetarVelocidadeAnimacao()
    {
        // Restaura a velocidade para o padrão
        anim.speed = 1f;
    }

    #region Desarmado

    public void Desarmar()
    {
        arma = null;
        armaMelee = null;
        anim.runtimeAnimatorController = controllerDesarmado;
        estaDesarmado = true;
        crosshairPanel.SetActive(true);
    }

    #endregion

    #region Animation Event

    public void RecarregarMunicao()
    {
        arma.VerificarMunicao();
    }
    public void AdicionarBalaPorBala()
    {
        arma.AdicionarUmaBala();
    }

    public void TerminandoAçaoCarregarArma()
    {
        carregandoArma = false;
    }

    public void TrocarAnimator()
    {
        Player.instance.trocaAnimator = false;
    }

    public void EventoFimAtaque()
    {
        armaMelee.EventoFimAtaque();
    }

    public void AplicarDano()
    {
        armaMelee.AplicarDano();
    }


    #endregion

    #region FX

    public void CarregarFx()
    {
        arma.audioSourceArma.clip = arma.reloadFx;
        arma.audioSourceArma.PlayOneShot(arma.reloadFx);
    }
    public void EngatilharFx()
    {
        arma.audioSourceArma.clip = arma.engatilharFx;
        arma.audioSourceArma.PlayOneShot(arma.engatilharFx);
    }

    public void MirarFX()
    {
        AudioManager.instance.TocarFx(AudioManager.instance.mirar);
    }

    #endregion
}
