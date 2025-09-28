using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Scripts")]
    public static AudioManager instance;

    #region Variaveis Sons Ambiente

    [Header("Controladores de Ambiente")]
    public AudioSource sourceAmbiente;
    public AudioSource sourceEfeitosAmbiente;
    private Coroutine fadeCoroutine;

    [Header("Sons de Chuva")]
    public AudioClip somChuvaFraca;
    public AudioClip somChuvaMedia;
    public AudioClip somChuvaForte;

    [Header("Sons de Tempestade")]
    public AudioClip[] sonsDeTrovao;

    [Header("Sons de Vento")]
    public AudioClip[] sonsVentoLoop;
    public AudioClip[] sonsRajadaDeVento;
    private Coroutine rajadasCoroutine;

    #endregion

    [Header("ControladoresHud")]
    public AudioSource sourceFxHud;

    [Header("ControladoresPlayer")]
    public AudioSource somFxVozes;
    public AudioSource efeitoSonoro;
    public AudioSource musica;

    [Header("FxMenu")]
    public AudioClip fxPause;
    public AudioClip[] fxGameOver;
    public AudioClip fxConfirmar;
    public AudioClip fxCancelar;
    public AudioClip fxBtnMove;

    [Header("FxArmasGenerico")]
    public AudioClip semMunicao;
    public AudioClip mirar;
    public AudioClip capsuleEjeting;
    public AudioClip trocarModoDeTiro;

    [Header("FxVozesPlayer")]
    public AudioClip fxVozMorte;
    public AudioClip fxVozDanos;
    public AudioClip fxVozPulo;

    #region Metodos iniciais

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        if (instance == this)
        {
            StartCoroutine(GerenciarLoopsDeVento());

            // Inicia a rotina que vai tocar rajadas de vento aleatórias
            rajadasCoroutine = StartCoroutine(RajadasDeVentoCoroutine());
        }
        DontDestroyOnLoad(this.gameObject);
    }

    #endregion
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

    #region Som Ambiente

    public void TocarSomDeAmbiente(AudioClip clip, float targetVolume)
    {
        // Se já existe uma transição ocorrendo, pare-a
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        // Inicia a nova transição de áudio
        fadeCoroutine = StartCoroutine(FadeAudioSource(clip, targetVolume));
    }

    private IEnumerator FadeAudioSource(AudioClip clip, float targetVolume)
    {
        float fadeDuration = 2.0f; // Duração da transição em segundos
        float startVolume = sourceAmbiente.volume;

        // Se o som já está tocando e é o mesmo, apenas ajusta o volume
        if (sourceAmbiente.isPlaying && sourceAmbiente.clip == clip)
        {
            float timer = 0f;
            while (timer < fadeDuration)
            {
                sourceAmbiente.volume = Mathf.Lerp(startVolume, targetVolume, timer / fadeDuration);
                timer += Time.deltaTime;
                yield return null;
            }
            sourceAmbiente.volume = targetVolume;
        }
        else // Se é um som novo ou estava parado, faz fade out do antigo e fade in do novo
        {
            // Fade out do som atual (se houver)
            float timer = 0f;
            while (timer < fadeDuration / 2)
            {
                sourceAmbiente.volume = Mathf.Lerp(startVolume, 0f, timer / (fadeDuration / 2));
                timer += Time.deltaTime;
                yield return null;
            }
            sourceAmbiente.volume = 0f;
            sourceAmbiente.Stop();

            // Se o novo clip não for nulo, começa o fade in
            if (clip != null)
            {
                sourceAmbiente.clip = clip;
                sourceAmbiente.Play();

                timer = 0f;
                while (timer < fadeDuration / 2)
                {
                    sourceAmbiente.volume = Mathf.Lerp(0f, targetVolume, timer / (fadeDuration / 2));
                    timer += Time.deltaTime;
                    yield return null;
                }
                sourceAmbiente.volume = targetVolume;
            }
        }
        fadeCoroutine = null;
    }

    public AudioClip EscolherSomDeTrovaoAleatorio()
    {
        if (sonsDeTrovao.Length == 0)
        {
            Debug.LogWarning("Não há sons de trovão configurados no AudioManager.");
            return null;
        }
        // Escolhe um índice aleatório do array
        int indexAleatorio = Random.Range(0, sonsDeTrovao.Length);
        // Retorna o AudioClip daquela posição
        return sonsDeTrovao[indexAleatorio];
    }

    #endregion

    #region Ventos
    public void AjustarVentoDeEstacao(float volumeAlvo)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        StartCoroutine(FadeVolume(sourceAmbiente, volumeAlvo, 5.0f)); // Fade de 5 segundos
    }

    private IEnumerator RajadasDeVentoCoroutine()
    {
        // Loop infinito que toca rajadas de vento
        while (true)
        {
            // 1. Espera um tempo aleatório (ex: entre 10 e 30 segundos)
            float espera = Random.Range(10f, 30f);
            yield return new WaitForSeconds(espera);

            // 2. Verifica se há sons de rajada para tocar
            if (sonsRajadaDeVento.Length > 0)
            {
                // 3. Escolhe uma rajada aleatória
                AudioClip rajada = sonsRajadaDeVento[Random.Range(0, sonsRajadaDeVento.Length)];

                // 4. Toca a rajada no AudioSource secundário, com um volume também aleatório
                sourceEfeitosAmbiente.PlayOneShot(rajada, Random.Range(0.6f, 1.0f));
            }
        }
    }

    private IEnumerator FadeVolume(AudioSource source, float targetVolume, float duration)
    {
        float startVolume = source.volume;
        float timer = 0f;

        while (timer < duration)
        {
            source.volume = Mathf.Lerp(startVolume, targetVolume, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        source.volume = targetVolume;
    }

    private IEnumerator GerenciarLoopsDeVento()
    {
        // Espera um frame para garantir que o SeasonManager já tenha iniciado
        yield return null;

        // Loop infinito para tocar os sons de vento um após o outro
        while (true)
        {
            // Se não tiver sons de vento configurados, apenas espera
            if (sonsVentoLoop.Length == 0)
            {
                yield return new WaitForSeconds(1f);
                continue; // Pula para a próxima iteração do loop
            }

            // 1. Escolhe um som de vento aleatório do array
            AudioClip proximoVento = sonsVentoLoop[Random.Range(0, sonsVentoLoop.Length)];

            // 2. Define o som no AudioSource
            sourceAmbiente.clip = proximoVento;

            // 3. Toca o som (ele começará a tocar em loop por padrão)
            sourceAmbiente.Play();

            yield return new WaitForSeconds(proximoVento.length);
        }
    }
    #endregion
}
