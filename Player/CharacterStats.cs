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

    }

    #endregion

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
}
