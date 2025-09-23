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
        /* arma = FindAnyObjectByType<Arma>();
        armaMelee = FindAnyObjectByType<ArmaMelee>(); */
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
