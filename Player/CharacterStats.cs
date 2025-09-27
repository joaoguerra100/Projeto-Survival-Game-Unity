using System;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    #region Variaveis
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

    [Header("Debufs")]
    public bool sangrando;
    public bool pernaQuebrada;

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
    }


    void Update()
    {
        if (PauseController.instance.pause == true | Player.instance.morte == true) { return; }
        GerenciarEstamina();
        AplicarPenalidadesDeStatus();
        AplicarPenalidadesMovimento();
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
            float multiplicadorRegen = 1f; // Começa com regeneração normal (100%)

            if (fomeAtual < 30f || sedeAtual < 30f)
            {
                // Se FOME OU SEDE estiverem baixas, a regeneração é penalizada
                multiplicadorRegen = 0.5f; // Regenera com 50% da velocidade
            }

            if (fomeAtual < 10f || sedeAtual < 10f)
            {
                // Se FOME OU SEDE estiverem críticas, a regeneração é zerada
                multiplicadorRegen = 0f; // Não regenera nada
            }
            // --------------------------------------------------------

            // Define a taxa de recuperação base (parado ou andando)
            float taxaRecuperacaoBase = (Player.instance.movimentosJogador.magnitude < 0.1f) ? recupStaminaParado : recupStaminaAndando;

            if (estaminaAtual < estaminaMax)
            {
                // APLICA O MULTIPLICADOR AQUI!
                estaminaAtual += taxaRecuperacaoBase * multiplicadorRegen * Time.deltaTime;
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
}
