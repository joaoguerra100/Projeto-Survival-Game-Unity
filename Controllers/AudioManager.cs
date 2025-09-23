using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Scripts")]
    public static AudioManager instance;

    [Header("ControladoresHud")]
    public AudioSource sourceFxHud;

    [Header("FxMenu")]
    public AudioClip fxPause;
    public AudioClip[] fxGameOver;
    public AudioClip fxConfirmar;
    public AudioClip fxCancelar;
    public AudioClip fxBtnMove;

    [Header("ControladoresPlayer")]
    public AudioSource somFxVozes;
    public AudioSource efeitoSonoro;
    public AudioSource musica;

    [Header("FxArmasGenerico")]
    public AudioClip semMunicao;
    public AudioClip mirar;
    public AudioClip capsuleEjeting;
    public AudioClip trocarModoDeTiro;

    [Header("FxVozesPlayer")]
    public AudioClip fxVozMorte;
    public AudioClip fxVozDanos;
    public AudioClip fxVozPulo;

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }


    void Update()
    {

    }
    //EFEITOS SONOROS HUD
    public void MetodoSomFxHud(AudioClip fxHud)
    {
        sourceFxHud.clip = fxHud;
        sourceFxHud.PlayOneShot(fxHud);
    }

    // EFEITOS SONOROS ARMAS
    public void TocarFx(AudioClip fx)
    {
        efeitoSonoro.clip = fx;
        efeitoSonoro.PlayOneShot(fx);
    }
    // EFEITOS SONOROS PLAYER
    public void MetodoSomFxVozes(AudioClip fxVoz)
    {
        somFxVozes.clip = fxVoz;
        somFxVozes.PlayOneShot(fxVoz);
    }
    
}
