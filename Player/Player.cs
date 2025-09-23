using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region VARIAVEIS

    [Header("Scripts")]
    public static Player instance;

    [Header("Referencias")]
    private CharacterController characterController;
    [HideInInspector] public Animator anim;

    [Header("Cameras")]
    private Transform myCamera;

    [Header("Vida")]
    public float vidaMaxima;
    public float vidaAtual;
    [HideInInspector] public bool morte;
    public bool sangrando;
    private bool travado = false;
    private float tempoTravado = 0f;

    [Header("Estamina")]
    public float estaminaMax;
    public float estaminaAtual;

    [Header("Fome")]
    public float fomeMaxima;
    public float fomeAtual;

    [Header("Sede")]
    public float sedeMaxima;
    public float sedeAtual;

    [Header("Movimentaçao")]
    public float velocidadeJogador;
    public float multiplicadorVelCorrida;
    [HideInInspector] public float velocidadeAtual;
    [HideInInspector] public Vector3 movimentosJogador;
    private float inputX, inputZ;
    private bool correndo = false;

    [Header("Pulo")]
    public float alturaDoPulo;
    private float gravidade = -9.81f;
    private float velocidadeVertical;
    [HideInInspector] public bool estaNochao;
    [SerializeField] private Transform verificadorChao;
    [SerializeField] private LayerMask layerPular;

    [Header("Agaichado")]
    [HideInInspector] public bool agaichado;

    [Header("ColetaConteiner")]
    public LayerMask layerDoContainer;
    private float holdThreshold = 0.5f;
    private float holdTime = 0f;
    private bool isHolding;
    [HideInInspector]public bool conteinerAberto;
    [HideInInspector]public ItemColetaObjetoView currentContainerAberto;

    [Header("Coleta")]
    private float raycastDistance = 2f;
    [SerializeField] private LayerMask layerMaskColetar;
    [SerializeField] private Transform alturaColeta;

    [Header("Interaçao")]
    [SerializeField] private LayerMask camadaInterativa;
    private InterfaceInteracao objetoAtual;

    [Header("Melle")]
    [HideInInspector] public bool isAttacking = false;

    [Header("Painel")]
    [SerializeField] private GameObject coletarPainel;

    [Header("Boleanas de bloqueio")] // bloquear açoes quando:
    [HideInInspector] public bool bloqueioControle; // quando estou tomando hit
    public bool trocaAnimator; // quando estou trocando de arma

    [Header("NoClip")]
    private bool isNoClip = false;
    private float noClipSpeed = 10f;

    #endregion


    #region METHODOS
    void Awake()
    {
        instance = this;
        characterController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        myCamera = Camera.main.transform; // pegamos referencia da nossa camera princial
    }

    void Start()
    {
        //podeBater = true;
        coletarPainel.SetActive(false);

        //ATRIBUIÇOES:
        vidaAtual = vidaMaxima;
        estaminaAtual = estaminaMax;
        fomeAtual = fomeMaxima;
        sedeAtual = sedeMaxima;

    }

    void Update()
    {
        MetodoMorte();
        if (PauseController.instance.pause == true | morte == true) { return; }
        //Cheat
        if (Input.GetKeyDown(KeyCode.N))
        {
            isNoClip = !isNoClip;
            characterController.enabled = !isNoClip;
        }

        if (isNoClip)
        {
            HandleNoClip();
            return;
        }
        //Fim Cheat
        if (travado)
        {
            tempoTravado -= Time.deltaTime;
            if (tempoTravado <= 0)
                travado = false;

            return;
        }
        EfeitosNegativos();
        velocidadeAtual = velocidadeJogador;
        Movimentacao();
        AplicarPenalidades();
        MoverJogador();
        Agaichar();
        Coletar();
        Interagir();
        AbrindoPainelColeta();
        if (bloqueioControle == false && trocaAnimator == false)
        {
            //BaterMelee();
            Pular();
        }
        Animacoes();
    }
    #endregion

    #region Movimentaçao

    void Movimentacao()
    {
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, myCamera.eulerAngles.y, transform.eulerAngles.z); // faz com que a a frente do personagem seja igual a aonde a camera esta apontada
        movimentosJogador = new Vector3(inputX, 0, inputZ).normalized;
        movimentosJogador = transform.TransformDirection(movimentosJogador); // FAZ ESTE VETOR TRABALHAR NO MESMO EIXO QUE O MEU TRANSFORM
    }

    void MoverJogador()
    {

        if (Input.GetKey(KeyCode.LeftShift) && movimentosJogador.magnitude > 0.1f && estaminaAtual > 0)
        {
            correndo = true;
            velocidadeAtual = velocidadeAtual * multiplicadorVelCorrida;
            UsarStaminaContinuo(5f);
        }
        else
        {
            correndo = false;

            if (movimentosJogador.magnitude < 0.1f)
                RecuperarStamina(6f);  // Recupera mais rápido parado
            else
                RecuperarStamina(3f);  // Recupera menos andando
        }

        if (movimentosJogador.magnitude >= 0.1f)
        {
            characterController.Move(movimentosJogador * velocidadeAtual * Time.deltaTime);

        }
    }

    void Pular()
    {
        estaNochao = Physics.CheckSphere(verificadorChao.position, 0.3f, layerPular);

        if (Input.GetKeyDown(KeyCode.Space) && estaNochao && trocaAnimator == false && bloqueioControle == false)
        {
            velocidadeVertical = Mathf.Sqrt(alturaDoPulo * -2f * gravidade);
        }

        if (estaNochao && velocidadeVertical < 0)
        {
            velocidadeVertical = -1f;
        }

        velocidadeVertical += gravidade * Time.deltaTime;

        characterController.Move(new Vector3(0, velocidadeVertical, 0) * Time.deltaTime);
    }

    void Agaichar()
    {
        if (Input.GetKeyDown(KeyCode.C) && estaNochao == true)
        {
            agaichado = !agaichado;
        }
    }

    #endregion

    #region Estamina

    void UsarStaminaContinuo(float taxaPorSegundo) //Para açoes continuas como correr
    {
        if (estaminaAtual > 0)
        {
            estaminaAtual -= taxaPorSegundo * Time.deltaTime;
            estaminaAtual = Mathf.Max(estaminaAtual, 0); // Impede valores negativos
        }
    }

    public void UsarStamina(float quantidade) //Para açoes pontuais como ataques
    {
        estaminaAtual -= quantidade;
        estaminaAtual = Mathf.Max(estaminaAtual, 0);
    }

    public void RecuperarStamina(float taxaRecuperacao)
    {
        if (!correndo /*&& !HandCombat.instance.isAttacking && HandCombat.instance.idCombo == 0*/)
        {
            estaminaAtual += taxaRecuperacao * Time.deltaTime;
            estaminaAtual = Mathf.Min(estaminaAtual, estaminaMax);
        }
    }

    float CalcularEstaminaMaxima(int nivel)
    {
        int estaminaBase = 100;
        float incrementoBase = 10f;
        return estaminaBase + incrementoBase * Mathf.Pow(1.15f, nivel - 1);
    }

    void AplicarPenalidades()
    {
        float percentual = estaminaAtual / estaminaMax;

        if (percentual >= 0.5f)
        {
            velocidadeAtual = velocidadeJogador;
            anim.SetBool("Cansado", false);
            //tempoAtaqueAtual = tempoAtaqueBase;
            // Sem penalidade
        }
        else if (percentual >= 0.3f)
        {
            velocidadeAtual = velocidadeJogador * 0.9f;
            anim.SetBool("Cansado", false);
            //tempoAtaqueAtual = tempoAtaqueBase;
        }
        else if (percentual >= 0.1f)
        {
            velocidadeAtual = velocidadeJogador * 0.75f;
            anim.SetBool("Cansado", false);
            //tempoAtaqueAtual = tempoAtaqueBase * 1.3f;
        }
        else if (percentual > 0f)
        {
            velocidadeAtual = velocidadeJogador * 0.5f;
            anim.SetBool("Cansado", true);
            //tempoAtaqueAtual = tempoAtaqueBase * 1.7f;
        }
        else // estamina zero
        {
            velocidadeAtual = 0f;
            anim.SetBool("Cansado", true);
            //tempoAtaqueAtual = tempoAtaqueBase * 2.5f;
        }
    }


    #endregion

    #region Danos

    void MetodoMorte()
    {
        if (vidaAtual <= 0 && morte == false)
        {
            morte = true;
            anim.SetTrigger("Morte");
            PlayerBracos.instance.anim.SetTrigger("Morte");
            //AudioManager.instance.MetodoSomFxVozes(AudioManager.instance.fxVozMorte);
            //StopAllCoroutines();
            //Destroy(gameObject, 5);
        }
    }

    public void DanosPlayer(int quant)
    {
        if (vidaAtual >= 1)
        {
            vidaAtual -= quant;
            //AudioManager.instance.MetodoSomFxVozes(AudioManager.instance.fxVozDanos);
            anim.SetTrigger("Danos");
            PlayerBracos.instance.anim.SetTrigger("Danos");
            bloqueioControle = true;
        }
    }

    void EfeitosNegativos()
    {
        if (sangrando)
        {
            vidaAtual -= 0.6f * Time.deltaTime;
        }
    }

    public void TravarMovimento(float tempo)
    {
        travado = true;
        tempoTravado = tempo;
    }

    #endregion

    #region Coleta
    void AbrindoPainelColeta()
    {
        Ray ray = new Ray(myCamera.position, myCamera.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, layerDoContainer))
        {
            if (hit.collider.CompareTag("Container") && hit.collider.TryGetComponent(out ItemColetaObjetoView containerView))
            {
                // Se mudou de container, fecha o antigo
                if (currentContainerAberto != null && currentContainerAberto != containerView)
                {
                    currentContainerAberto.itensNoArmazemPainel.SetActive(false);
                    currentContainerAberto.MostarCirculo(false);
                    currentContainerAberto.AtualizarCirculo(0);
                    isHolding = false;
                    holdTime = 0f;
                }

                currentContainerAberto = containerView;

                if (containerView.itensNoArmazemPainel.activeSelf == true) return;
                if (InventoryView.instance.VisiblePanel == true) return;

                containerView.MostarBotaoInteraçao(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (containerView.lootGerado)
                    {
                        containerView.MostrarLootExistente();
                        conteinerAberto = true;
                        InventoryManagerController.instance.ShowAndHide();
                        PlayerBracos.instance.ShowAndHideCrossHair();
                        containerView.MostarCirculo(false);
                        containerView.AtualizarCirculo(0);
                    }
                    else
                    {
                        isHolding = true;
                        holdTime = 0f;
                        containerView.MostarCirculo(true);
                    }
                }

                // Está segurando E
                if (isHolding && Input.GetKey(KeyCode.E) && !containerView.lootGerado)
                {
                    holdTime += Time.deltaTime;
                    float progresso = holdTime / holdThreshold;
                    containerView.AtualizarCirculo(progresso);
                    currentContainerAberto.MostarBotaoInteraçao(false);
                    containerView.isSearching = true;
                }

                // Soltou o E antes de completar
                if (isHolding && Input.GetKeyUp(KeyCode.E))
                {
                    isHolding = false;
                    holdTime = 0f;
                    containerView.MostarCirculo(false);
                    containerView.AtualizarCirculo(0);
                    containerView.isSearching = false;
                    currentContainerAberto.MostarBotaoInteraçao(true);
                }

                // Completou o tempo de hold
                if (holdTime >= holdThreshold && containerView.lootGerado == false)
                {
                    conteinerAberto = true;
                    InventoryManagerController.instance.ShowAndHide();
                    PlayerBracos.instance.ShowAndHideCrossHair();
                    isHolding = false;
                    holdTime = 0f;
                    containerView.isSearching = false;
                    containerView.MostarCirculo(false);
                    containerView.AtualizarCirculo(0);

                    // Sempre limpa a UI antes
                    containerView.ResetBag();

                    if (!containerView.lootGerado)
                    {
                        containerView.GerarLoot();
                    }
                    else
                    {
                        containerView.MostrarLootExistente();
                    }
                }
            }
            else
            {
                FecharCirculo();
            }
        }
        else
        {
            FecharCirculo();
        }
    }

    void FecharCirculo()
    {
        if (currentContainerAberto != null)
        {
            currentContainerAberto.MostarCirculo(false);
            currentContainerAberto.AtualizarCirculo(0);
            currentContainerAberto.MostarBotaoInteraçao(false);
        }

        conteinerAberto = false;
        isHolding = false;
        holdTime = 0f;
    }


    void Coletar()
    {
        Ray ray = new Ray(myCamera.position, myCamera.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, layerMaskColetar))
        {
            coletarPainel.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                ItemView item = hit.collider.GetComponent<ItemView>();
                if (item != null)
                {
                    // Calcula a diferença de altura entre o item e o personagem
                    float alturaItem = hit.point.y;
                    float alturaPersonagem = alturaColeta.transform.position.y;

                    float diferencaAltura = alturaItem - alturaPersonagem;

                    // Decide a animação com base na diferença de altura
                    if (diferencaAltura < -0.5f) // ajustável: -0.5f para itens bem baixos
                    {
                        // Toca animação de agachar e pegar
                        anim.SetTrigger("ColetarAgachado");
                        PlayerBracos.instance.anim.SetTrigger("ColetarAgachado");
                    }
                    else
                    {
                        // Toca animação de pegar normal
                        anim.SetTrigger("ColetarNormal");
                        PlayerBracos.instance.anim.SetTrigger("ColetarNormal");

                    }

                    item.Coletar();

                }
            }
        }
        else
        {
            coletarPainel.SetActive(false);
        }
    }

    #endregion

    #region Interaçao
    void Interagir()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, camadaInterativa))
        {
            InterfaceInteracao novaInterfaceInteracao = hit.collider.GetComponentInParent<InterfaceInteracao>();

            if (novaInterfaceInteracao != null)
            {
                // Se mudou de objeto, desliga o botão da anterior
                if (objetoAtual != null && objetoAtual != novaInterfaceInteracao)
                {
                    objetoAtual.MostarBotaoInteraçao(false);
                }

                objetoAtual = novaInterfaceInteracao;
                objetoAtual.MostarBotaoInteraçao(true);

                if (Input.GetKeyDown(objetoAtual.BotaoDeInteracao))
                {
                    objetoAtual.Interagir();
                }
            }

            else if (objetoAtual != null)
            {
                objetoAtual.MostarBotaoInteraçao(false);
                objetoAtual = null;
            }
        }
        else
        {
            // Se não estiver olhando pra nada, esconde o botão da última interagida
            if (objetoAtual != null)
            {
                objetoAtual.MostarBotaoInteraçao(false);
                objetoAtual = null;
            }
        }
    }

    #endregion

    #region Animaçoes
    private void Animacoes()
    {
        PlayerBracos.instance.anim.SetFloat("Vertical", inputZ);
        PlayerBracos.instance.anim.SetBool("Correndo", correndo);
        PlayerBracos.instance.anim.SetBool("Agaichado", agaichado);



        anim.SetFloat("Horizontal", inputX);
        anim.SetFloat("Vertical", inputZ);
        anim.SetBool("Agaichado", agaichado);
        anim.SetBool("Correndo", correndo);


    }

    #endregion

    #region Melee

   /*  private void BaterMelee()
    {
        if (isAttacking) return; // bloqueia novos ataques enquanto estiver atacando

        if (PlayerBracos.instance.anim.runtimeAnimatorController == PlayerBracos.instance.controllerMelee)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                isHolding = true;
                holdTime = 0;
            }
            else if (Input.GetButtonDown("Fire2"))
            {
                anim.SetTrigger("Bater2");
                isAttacking = true;
                //StartCoroutine(TempoParaBater());
            }

            if (isHolding && Input.GetButton("Fire1"))
            {
                holdTime += Time.deltaTime;
            }

            if (Input.GetButtonUp("Fire1"))
            {
                if (holdTime >= holdThreshold) // Ataque forte
                {
                    anim.SetTrigger("AtaqueForte");
                    isAttacking = true;
                }
                else // Ataque normal
                {
                    anim.SetTrigger("AtaqueFraco");
                    isAttacking = true;
                }

                isHolding = false;
            }
        }
    } */

    #endregion

    #region AnimationEvent

    public void EndAttack()
    {
        isAttacking = false;
    }

    public void ComboHandIniciar()
    {
        HandCombat.instance.ComboIniciar();
    }
    public void ComboHandCancelar()
    {
        HandCombat.instance.ComboCancelado();
    }
    
    public void PararSangramento()
    {
        if (sangrando)
        {
            sangrando = false;
        }
    }

    #endregion

    #region IEnumerator

    /*IEnumerator TempoParaBater()
    {
        podeBater = false;
        yield return new WaitForSeconds(1f);
        podeBater = true;
    }*/
    #endregion

    #region NoClip

    void HandleNoClip()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float upDown = 0f;

        if (Input.GetKey(KeyCode.E)) upDown = 1f;
        if (Input.GetKey(KeyCode.Q)) upDown = -1f;

        Vector3 move = transform.right * horizontal + transform.forward * vertical + Vector3.up * upDown;
        transform.position += move * noClipSpeed * Time.deltaTime;
    }


    #endregion
}

