using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Scripts")]
    public static AudioManager instance;

    [Header("Mixers")]
    public AudioMixer masterMixer;

    #region Variaveis Sons Ambiente

    [Header("Controladores de Ambiente")]
    public AudioSource sourceAmbiente;
    public AudioSource sourceEfeitosAmbiente;
    public AudioSource sourceAmbienteDetalhes;
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

    [Header("Controladores de Ambiente Detalhado")]
    private AudioClip clipAtualAmbienteDetalhes;
    private Coroutine fadeDetalhesCoroutine;

    [Header("Sons de Ambiente por Estação/Hora")]
    public AudioClip somPrimaveraDia;
    public AudioClip somPrimaveraNoite;
    public AudioClip somVeraoDia;
    public AudioClip somVeraoNoite;
    public AudioClip somOutonoDia;
    public AudioClip somOutonoNoite;
    public AudioClip somInvernoDia;  // Pode ser um som de vento mais sutil ou silêncio
    public AudioClip somInvernoNoite; // Silêncio ou vento ainda mais sutil

    #endregion

    #region Variaveis

    [Header("ControladoresHud")]
    public AudioSource sourceFxHud;

    [Header("ControladoresPlayer")]
    public AudioSource somFxVozes;
    public AudioSource efeitoSonoro;
    public AudioSource musica;

    [Header("FxMenu")]
    public AudioClip fxPause;
    public AudioClip[] fxGameOver;
    public AudioClip fxHoverSound;
    public AudioClip fxClickSound;
    public AudioClip fxCancelar;

    [Header("FxArmasGenerico")]
    public AudioClip semMunicao;
    public AudioClip mirar;
    public AudioClip capsuleEjeting;
    public AudioClip trocarModoDeTiro;

    [Header("FxVozesPlayer")]
    public AudioClip fxVozMorte;
    public AudioClip fxVozDanos;
    public AudioClip fxVozPulo;
    public AudioClip[] fxVozFrio; //0 tosse , 1 arrepio
    public AudioClip fxVozCalor;
    #endregion

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
            AtualizarSomAmbienteDetalhes();
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

    public IEnumerator TocarSomFrioComDelay()
    {
        // Toca um som de frio aleatório imediatamente
        if (fxVozFrio.Length > 0 && !somFxVozes.isPlaying)
        {
            AudioClip somRandomico = fxVozFrio[Random.Range(0, fxVozFrio.Length)];
            somFxVozes.PlayOneShot(somRandomico);
        }
        // Espera um tempo aleatório antes de permitir que o som toque novamente
        yield return new WaitForSeconds(Random.Range(8f, 15f));
        // Avisa ao CharacterStats que já pode pedir outro som de frio
        if (Player.instance != null) Player.instance.stats.podeTocarSomDeFrio = true;
    }

    public IEnumerator TocarSomCalorComDelay()
    {
        if (fxVozCalor != null && !somFxVozes.isPlaying)
        {
            somFxVozes.PlayOneShot(fxVozCalor);
        }
        yield return new WaitForSeconds(Random.Range(10f, 20f));
        if (Player.instance != null) Player.instance.stats.podeTocarSomDeCalor = true;
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

    public void TransicaoParaAmbiente(bool paraInterior)
    {
        if (masterMixer == null) return;

        AudioMixerSnapshot snapshotAlvo = masterMixer.FindSnapshot(paraInterior ? "Interior" : "Exterior");

        if (snapshotAlvo != null)
        {
            float tempoDeTransicao = 1.5f;
            snapshotAlvo.TransitionTo(tempoDeTransicao);
            //Debug.Log("Transição de áudio para " + snapshotAlvo.name);
        }
        else
        {
            Debug.LogError("Snapshot não encontrado: " + (paraInterior ? "Interior" : "Exterior"));
        }
    }

    public void AtualizarSomAmbienteDetalhes()
    {
        if (WorldTimeManager.instance == null || SeasonManager.instance == null) return;

        bool ehNoite = WorldTimeManager.instance.isNight;
        SeasonManager.Estacao estacao = SeasonManager.instance.estacaoAtual;
        AudioClip clipAlvo = null;

        // Seleciona o AudioClip correto com base na estação e hora
        switch (estacao)
        {
            case SeasonManager.Estacao.Primavera:
                clipAlvo = ehNoite ? somPrimaveraNoite : somPrimaveraDia;
                break;
            case SeasonManager.Estacao.Verao:
                clipAlvo = ehNoite ? somVeraoNoite : somVeraoDia;
                break;
            case SeasonManager.Estacao.Outono:
                clipAlvo = ehNoite ? somOutonoNoite : somOutonoDia;
                break;
            case SeasonManager.Estacao.Inverno:
                clipAlvo = ehNoite ? somInvernoNoite : somInvernoDia;
                break;
        }

        // Só faz a transição se o som alvo for diferente do que já está tocando
        if (clipAlvo != clipAtualAmbienteDetalhes && sourceAmbienteDetalhes != null)
        {
            // Para a transição anterior, se houver uma
            if (fadeDetalhesCoroutine != null)
            {
                StopCoroutine(fadeDetalhesCoroutine);
            }
            // Inicia a nova transição
            fadeDetalhesCoroutine = StartCoroutine(FadeAmbienteDetalhesCoroutine(clipAlvo));
        }
    }

    private IEnumerator FadeAmbienteDetalhesCoroutine(AudioClip clipAlvo)
    {
        float fadeDuration = 3.0f; // Duração da transição (ex: 3 segundos)
        float startVolume = sourceAmbienteDetalhes.volume;

        // 1. Fade out do som atual
        float timer = 0f;
        while (timer < fadeDuration / 2)
        {
            sourceAmbienteDetalhes.volume = Mathf.Lerp(startVolume, 0f, timer / (fadeDuration / 2));
            timer += Time.deltaTime;
            yield return null;
        }
        sourceAmbienteDetalhes.volume = 0f;
        sourceAmbienteDetalhes.Stop();

        // 2. Troca o clipe e começa o fade in
        clipAtualAmbienteDetalhes = clipAlvo; // Atualiza o clipe atual
        if (clipAlvo != null)
        {
            sourceAmbienteDetalhes.clip = clipAlvo;
            sourceAmbienteDetalhes.loop = true; // Garante que o som de ambiente toque em loop
            sourceAmbienteDetalhes.Play();

            // Pega o volume alvo (pode ser ajustado ou vir de configurações)
            float targetVolume = 1.0f; // Volume padrão para ambiente detalhado

            timer = 0f;
            while (timer < fadeDuration / 2)
            {
                sourceAmbienteDetalhes.volume = Mathf.Lerp(0f, targetVolume, timer / (fadeDuration / 2));
                timer += Time.deltaTime;
                yield return null;
            }
            sourceAmbienteDetalhes.volume = targetVolume;
        }
        else
        {
            // Se o clipAlvo for nulo (ex: silêncio no inverno), apenas mantém parado.
        }

        fadeDetalhesCoroutine = null;
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
