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
    [HideInInspector] public CharacterStats stats;

    [Header("Cameras")]
    private Transform myCamera;

    [Header("Vida")]
    [HideInInspector] public bool morte;
    private bool travado = false;
    private float tempoTravado = 0f;

    [Header("Movimentaçao")]
    public float velocidadeAtual;
    [HideInInspector] public Vector3 movimentosJogador;
    private float inputX, inputZ;
    [HideInInspector]public bool correndo = false;

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
    [HideInInspector] public bool conteinerAberto;
    [HideInInspector] public ItemColetaObjetoView currentContainerAberto;

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
        stats = GetComponent<CharacterStats>();
        anim = GetComponent<Animator>();
        myCamera = Camera.main.transform; // pegamos referencia da nossa camera princial
    }

    void Start()
    {
        //podeBater = true;
        coletarPainel.SetActive(false);
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
        velocidadeAtual = stats.velAndar;
        Movimentacao();
        MoverJogador();
        Agaichar();
        Coletar();
        Interagir();
        AbrindoPainelColeta();
        if (bloqueioControle == false && trocaAnimator == false)
        {
            Pular();
        }
        Animacoes();
    }
    #endregion

    #region Movimentaçao

    void Movimentacao()
    {
        inputX = InputManager.instance.HorizontalAxis;
        inputZ = InputManager.instance.VerticalAxis;
        
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, myCamera.eulerAngles.y, transform.eulerAngles.z); // faz com que a a frente do personagem seja igual a aonde a camera esta apontada
        movimentosJogador = new Vector3(inputX, 0, inputZ).normalized;
        movimentosJogador = transform.TransformDirection(movimentosJogador); // FAZ ESTE VETOR TRABALHAR NO MESMO EIXO QUE O MEU TRANSFORM
    }

    void MoverJogador()
    {
        correndo = Input.GetKey(InputManager.instance.runKey) && movimentosJogador.magnitude > 0.1f && stats.estaminaAtual > 0;

        if (correndo)
        {
            velocidadeAtual = velocidadeAtual * stats.multiplicadorVelCorrida;
        }

        if (movimentosJogador.magnitude >= 0.1f)
        {
            characterController.Move(movimentosJogador * velocidadeAtual * Time.deltaTime);
        }
    }

    void Pular()
    {
        estaNochao = Physics.CheckSphere(verificadorChao.position, 0.3f, layerPular);

        if (Input.GetKeyDown(InputManager.instance.jumpKey) && estaNochao && trocaAnimator == false && bloqueioControle == false)
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
        if (Input.GetKeyDown(InputManager.instance.agaichadoKey) && estaNochao == true)
        {
            agaichado = !agaichado;
        }
    }

    #endregion

    #region Danos

    void MetodoMorte()
    {
        if (stats.vidaAtual <= 0 && morte == false)
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
        if (stats.vidaAtual >= 1)
        {
            stats.vidaAtual -= quant;
            //AudioManager.instance.MetodoSomFxVozes(AudioManager.instance.fxVozDanos);
            anim.SetTrigger("Danos");
            PlayerBracos.instance.anim.SetTrigger("Danos");
            bloqueioControle = true;
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

                if (Input.GetKeyDown(InputManager.instance.collectKey))
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
                if (isHolding && Input.GetKey(InputManager.instance.collectKey) && !containerView.lootGerado)
                {
                    holdTime += Time.deltaTime;
                    float progresso = holdTime / holdThreshold;
                    containerView.AtualizarCirculo(progresso);
                    currentContainerAberto.MostarBotaoInteraçao(false);
                    containerView.isSearching = true;
                }

                // Soltou o E antes de completar
                if (isHolding && Input.GetKeyUp(InputManager.instance.collectKey))
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
            if (Input.GetKeyDown(InputManager.instance.collectKey))
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
        bool estaCansado = stats.estaminaAtual < (stats.estaminaMax * 0.1f);

        PlayerBracos.instance.anim.SetFloat("Vertical", inputZ);
        PlayerBracos.instance.anim.SetBool("Correndo", correndo);
        PlayerBracos.instance.anim.SetBool("Agaichado", agaichado);

        anim.SetFloat("Horizontal", inputX);
        anim.SetFloat("Vertical", inputZ);
        anim.SetBool("Agaichado", agaichado);
        anim.SetBool("Correndo", correndo);
        anim.SetBool("Cansado", estaCansado);
    }

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
        stats.AplicarSangramento(false);
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

