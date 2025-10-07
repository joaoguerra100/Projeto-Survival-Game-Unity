using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    #region Variaveis De Status

    [Header("Vida")]
    public float vidaMaxima;
    [SerializeField] private float _vidaAtual;
    public float vidaAtual
    {
        get { return _vidaAtual; }
        set
        {
            _vidaAtual = Mathf.Clamp(value, 0, vidaMaxima);
            OnVidaMudou?.Invoke(_vidaAtual, vidaMaxima);
        }
    }

    [Header("Velocidade")]
    public float velAndar;
    public float multiplicadorVelCorrida;
    private float velAndarOriginal;

    [Header("Atributos do Jogador")]
    public float forca = 10f;
    public float agilidade;

    [Header("Estamina")]
    public float estaminaMax;
    [SerializeField] private float _estaminaAtual;
    public float tempoAteRegenerar = 1.5f;
    [HideInInspector] public float contadorDelayRegen;
    private float multiplicadorRegenStamina;
    public float recupStaminaParado;
    public float recupStaminaAndando;
    public float estaminaAtual
    {
        get { return _estaminaAtual; }
        set
        {
            _estaminaAtual = Mathf.Clamp(value, 0, estaminaMax);
            OnEstaminaMudou?.Invoke(_estaminaAtual, estaminaMax);
        }
    }

    [Header("Fome")]
    public float fomeMaxima;
    [SerializeField] private float _fomeAtual;
    public float fomeAtual
    {
        get { return _fomeAtual; }
        set
        {
            _fomeAtual = Mathf.Clamp(value, 0, fomeMaxima);
            OnFomeMudou?.Invoke(_fomeAtual, fomeMaxima);
        }
    }

    [Header("Sede")]
    public float sedeMaxima;
    [SerializeField] private float _sedeAtual;
    public float sedeAtual
    {
        get { return _sedeAtual; }
        set
        {
            _sedeAtual = Mathf.Clamp(value, 0, sedeMaxima);
            OnSedeMudou?.Invoke(_sedeAtual, sedeMaxima);
        }
    }

    #endregion

    #region Variaveis de Clima

    [Header("Debufs")]
    public bool sangrando;
    public bool pernaQuebrada;

    [Header("Debuffs de Clima")]
    [SerializeField] private float nivelDeUmidade;
    [SerializeField] private float temperaturaCorporal = 37f;

    [Header("Configurações de Sobrevivência")]
    [SerializeField] private float zonaDeConfortoMin = 18f;
    [SerializeField] private float zonaDeConfortoMax = 28f;
    [SerializeField] private float taxaDeMudancaTemperatura = 0.1f;
    [SerializeField] private float taxaDeHomeostase = 0.5f;
    [SerializeField] private float taxaDeSecagem = 5f;
    [SerializeField] private float danoPorHipotermia = 0.5f;
    [SerializeField] private float danoPorCalor = 0.5f;

    [Header("Icones")]
    [SerializeField] private GameObject iconeFrio;
    [SerializeField] private GameObject iconeCalor;
    [SerializeField] private GameObject iconeMolhado;

    [Header("Boleanas de status")]
    private bool estaAbrigado = false;
    private bool pertoDeFonteDeCalor = false;
    private bool estaNaAgua = false;

    [Header("Controle de Sons de Status")]
    [HideInInspector] public bool podeTocarSomDeFrio = true;
    [HideInInspector] public bool podeTocarSomDeCalor = true;

    [Header("PosProcessamento")]
    private LensDistortion lensDistortion;
    private ChromaticAberration chromaticAberration;

    [Header("Efeitos Visuais")]
    [SerializeField] private Image statusOverlayImage;
    [SerializeField] private float fadeSpeed = 3f;

    #endregion

    #region Actions

    public event Action<float, float> OnVidaMudou;
    public event Action<float, float> OnEstaminaMudou;
    public event Action<float, float> OnFomeMudou;
    public event Action<float, float> OnSedeMudou;

    #endregion

    #region Methods inicias

    void Awake()
    {
        velAndarOriginal = velAndar;
        _vidaAtual = vidaMaxima;
        _estaminaAtual = estaminaMax;
        _fomeAtual = fomeMaxima;
        _sedeAtual = sedeMaxima;
    }

    void Start()
    {
        OnVidaMudou?.Invoke(_vidaAtual, vidaMaxima);
        OnEstaminaMudou?.Invoke(_estaminaAtual, estaminaMax);
        OnFomeMudou?.Invoke(_fomeAtual, fomeMaxima);
        OnSedeMudou?.Invoke(_sedeAtual, sedeMaxima);
        iconeCalor.SetActive(false);
        iconeFrio.SetActive(false);
        iconeMolhado.SetActive(false);

        if (ClimaManager.instance != null && ClimaManager.instance.volumeGlobal != null)
        {
            VolumeProfile profile = ClimaManager.instance.volumeGlobal.profile;
            profile.TryGet(out lensDistortion);
            profile.TryGet(out chromaticAberration);
        }
    }


    void Update()
    {
        if (PauseController.instance.pause == true | Player.instance.morte == true) { return; }
        GerenciarStatusDoClima();
        GerenciarEstamina();
        AplicarPenalidadesDeStatus();
        AplicarPenalidadesMovimento();

        GerenciarEfeitosPosProcessamento();
        GerenciarStatusOverlay();
    }

    #endregion

    #region Stamina

    void GerenciarEstamina()
    {
        if (Player.instance.correndo)
        {
            UsarStaminaContinuo(5f);
            return; // Não regenera enquanto corre
        }

        if (contadorDelayRegen > 0)
        {
            contadorDelayRegen -= Time.deltaTime;
        }
        else
        {
            // --- NOVO: Cálculo do Multiplicador de Regeneração ---
            multiplicadorRegenStamina = 1f; // Começa com regeneração normal (100%)

            if (fomeAtual < 30f || sedeAtual < 30f)
            {
                // Se FOME OU SEDE estiverem baixas, a regeneração é penalizada
                multiplicadorRegenStamina = 0.5f; // Regenera com 50% da velocidade
            }

            if (fomeAtual < 10f || sedeAtual < 10f)
            {
                // Se FOME OU SEDE estiverem críticas, a regeneração é zerada
                multiplicadorRegenStamina = 0f; // Não regenera nada
            }

            // Lógica de penalidade por umidade
            if (nivelDeUmidade > 75f)
            {
                multiplicadorRegenStamina *= 0.5f;
            }

            // Define a taxa de recuperação base (parado ou andando)
            float taxaRecuperacaoBase = (Player.instance.movimentosJogador.magnitude < 0.1f) ? recupStaminaParado : recupStaminaAndando;

            if (estaminaAtual < estaminaMax)
            {
                // APLICA O MULTIPLICADOR AQUI!
                estaminaAtual += taxaRecuperacaoBase * multiplicadorRegenStamina * Time.deltaTime;
                estaminaAtual = Mathf.Min(estaminaAtual, estaminaMax);
            }
        }
    }

    public void UsarStamina(float quantidade) //Para açoes pontuais como ataques
    {
        if (estaminaAtual > 0)
        {
            estaminaAtual -= quantidade;
            estaminaAtual = Mathf.Max(estaminaAtual, 0);
            contadorDelayRegen = tempoAteRegenerar;
        }
    }

    void UsarStaminaContinuo(float taxaPorSegundo) //Para açoes continuas como correr
    {
        if (estaminaAtual > 0)
        {
            estaminaAtual -= taxaPorSegundo * Time.deltaTime;
            estaminaAtual = Mathf.Max(estaminaAtual, 0); // Impede valores negativos
            contadorDelayRegen = tempoAteRegenerar;
        }
    }

    float CalcularEstaminaMaxima(int nivel)
    {
        int estaminaBase = 100;
        float incrementoBase = 10f;
        return estaminaBase + incrementoBase * Mathf.Pow(1.15f, nivel - 1);
    }

    /* public void RecuperarStamina(float taxaRecuperacao)
    {
        if (!Player.instance.correndo)
        {
            estaminaAtual += taxaRecuperacao * Time.deltaTime;
            estaminaAtual = Mathf.Min(estaminaAtual, estaminaMax);
        }
    } */

    #endregion

    #region Penalidades de Status

    void AplicarPenalidadesDeStatus()
    {
        // --- Penalidades por Fome Crítica ---
        if (fomeAtual < 10f)
        {
            // Perde vida continuamente
            vidaAtual -= 0.7f * Time.deltaTime;
        }

        // --- Penalidades por Sede Crítica ---
        if (sedeAtual < 10f)
        {
            // Perde vida continuamente (o dano se acumula se ambos estiverem críticos)
            vidaAtual -= 0.7f * Time.deltaTime;
        }

        // Garante que a vida não fique abaixo de zero por causa das penalidades
        vidaAtual = Mathf.Max(vidaAtual, 0);
    }

    void AplicarPenalidadesMovimento()
    {
        float percentual = estaminaAtual / estaminaMax;

        if (percentual >= 0.5f)
        {
            Player.instance.velocidadeAtual = velAndar;
        }
        else if (percentual >= 0.3f)
        {
            Player.instance.velocidadeAtual = velAndar * 0.9f;
        }
        else if (percentual >= 0.1f)
        {
            Player.instance.velocidadeAtual = velAndar * 0.75f;
        }
        else if (percentual > 0f)
        {
            Player.instance.velocidadeAtual = velAndar * 0.5f;
        }
        else // estamina zero
        {
            Player.instance.velocidadeAtual = velAndar * 0.3f;
        }
    }

    public void AplicarSangramento(bool estaSangrando)
    {
        sangrando = estaSangrando;
        /* if (estaSangrando) {
            AudioManager.instance.PlayHeartbeatSound();
            particleEffectSangue.Play();
        } else {
            AudioManager.instance.StopHeartbeatSound();
        } */
    }

    public void QuebrarPerna(bool estaQuebrada)
    {
        pernaQuebrada = estaQuebrada;
        if (estaQuebrada)
        {
            velAndar = velAndar / 2;
        }
        else
        {
            velAndar = velAndarOriginal;
        }
    }

    #endregion

    #region Clima

    void GerenciarStatusDoClima()
    {
        // --- COLETA DE DADOS ---
        ClimaManager clima = ClimaManager.instance;
        float temperaturaExterna = clima.temperaturaAtual;
        bool estaChovendoAgora = clima.chovendoNesteMomento;

        //LÓGICA DE UMIDADE
        if (estaChovendoAgora && !estaAbrigado)
        {
            nivelDeUmidade += (float)clima.intensidadeAtualDaChuva * 0.5f * Time.deltaTime;
        }
        else
        {
            float taxaDeSecagemAjustada = taxaDeSecagem;
            if (temperaturaExterna > 25f) taxaDeSecagemAjustada *= 2f;
            nivelDeUmidade -= taxaDeSecagemAjustada * Time.deltaTime;
        }
        //LÓGICA DE TEMPERATURA

        // 1. A FORÇA DO AMBIENTE: Tenta empurrar a temperatura para os extremos.
        float forcaDoAmbiente = 0f;

        // SÓ SOFRE COM O AMBIENTE SE NÃO ESTIVER ABRIGADO
        if (!estaAbrigado)
        {
            if (temperaturaExterna < zonaDeConfortoMin)
            {
                // Se está frio lá fora, a força é negativa (esfria o corpo).
                forcaDoAmbiente = (temperaturaExterna - zonaDeConfortoMin) * taxaDeMudancaTemperatura;
            }
            else if (temperaturaExterna > zonaDeConfortoMax)
            {
                // Se está quente lá fora, a força é positiva (esquenta o corpo).
                forcaDoAmbiente = (temperaturaExterna - zonaDeConfortoMax) * taxaDeMudancaTemperatura;
            }

            // Modificadores: Estar molhado piora o frio.
            if (nivelDeUmidade > 50f && forcaDoAmbiente < 0)
            {
                forcaDoAmbiente *= 3f; // Efeito do frio 3x mais forte.
            }
        }

        // 2. A FORÇA DO CORPO (HOMEOSTASE): Sempre tenta voltar para 37°C.
        float forcaHomeostase = (37f - temperaturaCorporal) * taxaDeHomeostase;

        // OVERRIDE DA FONTE DE CALOR
        if (pertoDeFonteDeCalor)
        {
            // Se está perto do fogo, a força de homeostase fica muito mais forte,
            // aquecendo o corpo rapidamente de volta para 37°C.
            forcaHomeostase *= 10f; // Se aquece 10x mais rápido

            // E também seca as roupas muito mais rápido
            nivelDeUmidade -= taxaDeSecagem * 5f * Time.deltaTime;
        }

        nivelDeUmidade = Mathf.Clamp(nivelDeUmidade, 0, 100);

        // OVERRIDE DA ÁGUA
        if (estaNaAgua)
        {
            // A água força a temperatura corporal a se igualar à temperatura externa
            // de forma MUITO mais agressiva do que o ar.
            // Simulamos isso aplicando uma força de resfriamento extra e muito forte.

            // A água só vai te esfriar se ela for mais fria que seu corpo.
            if (temperaturaExterna < temperaturaCorporal)
            {
                // A "força" do frio da água é 15x mais forte que a do ar.
                forcaDoAmbiente = (temperaturaExterna - temperaturaCorporal) * taxaDeMudancaTemperatura * 15f;
            }
        }

        // 3. CÁLCULO FINAL: A mudança na temperatura é a soma das duas forças.
        // Elas lutam uma contra a outra.
        float mudancaLiquida = forcaDoAmbiente + forcaHomeostase;

        // Aplica a mudança final.
        temperaturaCorporal += mudancaLiquida * Time.deltaTime;

        // Limita a temperatura a valores fisiológicos.
        temperaturaCorporal = Mathf.Clamp(temperaturaCorporal, 32f, 42f);

        GerenciarIcones();
        //APLICAÇÃO DOS DEBUFFS
        DebuffsCalor();
        DebuffsFrio();
    }

    void DebuffsCalor()
    {
        if (temperaturaCorporal >= 42f)
        {
            // NÍVEL 3: CRÍTICO (Dano na vida)
            float severidade = temperaturaCorporal - 42f;
            vidaAtual -= danoPorCalor * severidade * Time.deltaTime;
            TocarSomDeStatus("Calor");
            if (iconeCalor != null) iconeCalor.SetActive(true);
        }
        else if (temperaturaCorporal > 40f) // Acima de 40°C
        {
            // NÍVEL 2: PERIGOSO (Perde sede e fome)
            float severidade = temperaturaCorporal - 40f;
            sedeAtual -= 5f * severidade * Time.deltaTime;
            fomeAtual -= 1f * severidade * Time.deltaTime;
            TocarSomDeStatus("Calor");
            if (iconeCalor != null) iconeCalor.SetActive(true);
        }
        else if (temperaturaCorporal > 38f) // Acima de 38°C
        {
            // NÍVEL 1: ALERTA (Apenas um aviso)
            if (iconeCalor != null) iconeCalor.SetActive(true);
            //Debug.Log("Alerta: Calor excessivo!");
        }
        else
        {
            podeTocarSomDeCalor = true;
        }
    }
    void DebuffsFrio()
    {
        if (temperaturaCorporal < 33f)
        {
            float severidade = 33f - temperaturaCorporal;
            vidaAtual -= danoPorHipotermia * severidade * Time.deltaTime;
            fomeAtual -= 1.2f * severidade * Time.deltaTime;
            TocarSomDeStatus("Frio");
            if (iconeFrio != null) iconeFrio.SetActive(true);
        }
        else if (temperaturaCorporal < 35f)
        {
            multiplicadorRegenStamina *= 0.3f;
            TocarSomDeStatus("Frio");
            if (iconeFrio != null) iconeFrio.SetActive(true);
        }
        else if (temperaturaCorporal < 36.5f)
        {
            multiplicadorRegenStamina *= 0.7f;
            TocarSomDeStatus("Frio");
            if (iconeFrio != null) iconeFrio.SetActive(true);
        }
        else
        {
            podeTocarSomDeFrio = true;
        }
    }

    void GerenciarIcones()
    {
        if (iconeCalor != null) iconeCalor.SetActive(false);
        if (iconeFrio != null) iconeFrio.SetActive(false);

        if (iconeMolhado != null)
        {
            iconeMolhado.SetActive(nivelDeUmidade > 30f);
        }
    }

    void TocarSomDeStatus(string tipo)
    {
        if (tipo == "Frio" && podeTocarSomDeFrio)
        {
            StartCoroutine(AudioManager.instance.TocarSomFrioComDelay());
            podeTocarSomDeFrio = false; // Impede que o som toque de novo imediatamente
        }
        else if (tipo == "Calor" && podeTocarSomDeCalor)
        {
            StartCoroutine(AudioManager.instance.TocarSomCalorComDelay());
            podeTocarSomDeCalor = false;
        }
    }

    #endregion

    #region Gerenciamento de efeitos

    void GerenciarEfeitosPosProcessamento()
    {
        if (lensDistortion == null || chromaticAberration == null) return;

        float distortionAlvo = 0f;
        float chromaticAlvo = 0f;

        // Logica de calor
        if (temperaturaCorporal > 39f)
        {
            // Mapeia a temperatura de 39°C (sem efeito) para 42°C (efeito máximo)
            float intensidaCalor = 1 - ((temperaturaCorporal - 39f) / (42f - 39f));
            intensidaCalor = Mathf.Clamp01(intensidaCalor);

            distortionAlvo = intensidaCalor * -0.3f;
            chromaticAlvo = intensidaCalor * 0.4f;
        }
        // APLICAÇÃO SUAVE DOS VALORES
        // Usamos Lerp para que os efeitos apareçam e desapareçam gradualmente, não de forma instantânea.
        float velocidadeDeTransicao = Time.deltaTime * 2f;
        lensDistortion.intensity.value = Mathf.Lerp(lensDistortion.intensity.value, distortionAlvo, velocidadeDeTransicao);
        chromaticAberration.intensity.value = Mathf.Lerp(chromaticAberration.intensity.value, chromaticAlvo, velocidadeDeTransicao);
    }

    void GerenciarStatusOverlay()
    {
        if (statusOverlayImage == null) return;

        Color targetColor = Color.clear;
        float targetAlpha = 0f;

        // Logica de calor
        if (temperaturaCorporal > 38f)
        {
            float intensidade = 0f;
            if (temperaturaCorporal >= 42f) intensidade = 1f;
            else if (temperaturaCorporal > 40f) intensidade = (temperaturaCorporal - 40f) / 2f;
            else intensidade = (temperaturaCorporal - 38f) / 2f;

            targetColor = Color.Lerp(new Color(1f, 0.5f, 0f), Color.red, intensidade); // Laranja para Vermelho
            targetAlpha = intensidade * 0.3f; // Alpha máximo de 30% para calor
        }
        // Logica de Frio
        if (temperaturaCorporal < 36.5f)
        {
            float intensidade = 0f;
            if (temperaturaCorporal >= 33f) intensidade = 1f;
            else if (temperaturaCorporal > 35f) intensidade = (35f - temperaturaCorporal) / 2f;
            else intensidade = (36.5f - temperaturaCorporal) / 1.5f;

            targetColor = Color.Lerp(new Color(0.5f, 0.7f, 1f), Color.blue, intensidade); // Azul claro para Azul escuro
            targetAlpha = intensidade * 0.3f; // Alpha máximo de 30% para frio
        }
        // Logica de pouca vida
        // Se a vida estiver muito baixa, sobrescreve o resto com vermelho intenso
        if (vidaAtual < vidaMaxima * 0.25f) // Abxaido de 25% de vida
        {
            float intensidadeDano = 1 - (vidaAtual / (vidaMaxima * 0.25f));
            targetColor = Color.Lerp(Color.red, new Color(0.5f, 0f, 0f), intensidadeDano); // Vermelho para Vermelho Escuro
            targetAlpha = Mathf.Lerp(0.2f, 0.6f, intensidadeDano); // Alpha de 20% a 60%
        }
        // APLICAÇÃO SUAVE NO OVERLAY
        statusOverlayImage.color = Color.Lerp(statusOverlayImage.color, targetColor, Time.deltaTime * fadeSpeed);
        // O Alpha é controlado pela cor, então apenas defina o targetAlpha
        Color currentColor = statusOverlayImage.color;
        currentColor.a = Mathf.Lerp(currentColor.a, targetAlpha, Time.deltaTime * fadeSpeed);
        statusOverlayImage.color = currentColor;
    }

    #endregion

    #region OnTrigger
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Abrigo"))
        {
            estaAbrigado = true;
            AudioManager.instance.TransicaoParaAmbiente(true);
        }
        if (other.CompareTag("FonteDeCalor"))
        {
            pertoDeFonteDeCalor = true;
        }
        if (other.CompareTag("Agua"))
        {
            estaNaAgua = true;
        }
        if (other.CompareTag("Fogo"))
        {
            vidaAtual -= 0.5f * Time.deltaTime;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Abrigo"))
        {
            estaAbrigado = false;
            AudioManager.instance.TransicaoParaAmbiente(false);
        }
        if (other.CompareTag("FonteDeCalor"))
        {
            pertoDeFonteDeCalor = false;
        }
        if (other.CompareTag("Agua"))
        {
            estaNaAgua = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Fogo"))
        {
            float danoPorSegundo = 25f;
            vidaAtual -= danoPorSegundo * Time.deltaTime;
        }
        if (other.CompareTag("Agua"))
        {
            nivelDeUmidade += 20f * Time.deltaTime;
        }
    }

    #endregion

    #region Metodos de ajuda

    public float GetTemperaturaCorporal()
    {
        return temperaturaCorporal;
    }

    public void SetTemperaturaCorporal(float valor)
    {
        temperaturaCorporal = valor;
    }

    public float GetNivelDeUmidade()
    {
        return nivelDeUmidade;
    }

    public void SetNivelDeUmidade(float valor)
    {
        nivelDeUmidade = valor;
    }


    #endregion
}
