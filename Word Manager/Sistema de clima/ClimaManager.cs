using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class ClimaManager : MonoBehaviour
{
    [Header("Referencias")]
    public static ClimaManager instance;
    public Volume volumeGlobal;

    [Header("Referências de Céu")]
    public Material ceuMaterial;
    public ParticleSystem camadaDeNuvens;
    private ParticleSystem.EmissionModule nuvensEmission;
    private ParticleSystem.MainModule nuvensMain;

    #region Variaveis De Chuva

    [Header("Referências de Clima Chuva")]
    public GameObject rainEffectObject;
    public ParticleSystem rainParticleSystem;
    public ParticleSystem splashParticleSystem;
    private ParticleSystem.EmissionModule rainEmission;
    private ParticleSystem.VelocityOverLifetimeModule rainVelocity;
    private ParticleSystemRenderer rainRenderer;
    public enum RainIntensity { SemChuva = 0, Fraca = 1, Media = 2, Forte = 3 }
    public RainIntensity intensidadeAtualDaChuva;
    private Coroutine transicaoChuvaCoroutine;

    [Header("Probabilidades de Chuva Primavera")]
    [Range(0, 100)] public int chanceChuvaFracaPrimavera = 30;
    [Range(0, 100)] public int chanceChuvaMediaPrimavera = 15;
    [Range(0, 100)] public int chanceChuvaFortePrimavera = 5;

    [Header("Probabilidades de Chuva Verao")]
    [Range(0, 100)] public int chanceChuvaFracaVerao = 10;
    [Range(0, 100)] public int chanceChuvaMediaVerao = 2;
    [Range(0, 100)] public int chanceChuvaForteVerao = 1;
    [Header("Probabilidades de Chuva Outono")]// Estaçao de chuva
    [Range(0, 100)] public int chanceChuvaFracaOutono = 35;
    [Range(0, 100)] public int chanceChuvaMediaOutono = 25;
    [Range(0, 100)] public int chanceChuvaForteOutono = 15;

    [Header("Configuraçoes da chuva")]
    public bool chovendoNesteMomento { get; private set; }
    private float horaInicioChuva;
    private float horaFimChuva;

    [Header("Tempestade")]
    [SerializeField] private float minTempoEntreRaios = 8f;
    [SerializeField] private float maxTempoEntreRaios = 25f;
    private Coroutine tempestadeCoroutine;
    #endregion

    #region Variaveis De Neve

    [Header("Referencias de Clima de Neve")]
    public ParticleSystem snowParticleSystem;
    private ParticleSystem.EmissionModule snowEmission;
    private ParticleSystem.VelocityOverLifetimeModule snowVelocity;

    public enum NeveIntensidade
    {
        SemNeve = 0,
        Fraca = 1,
        Media = 2,
        Forte = 3,
    }

    public NeveIntensidade intensidadeAtualDaNeve;

    [Header("Probabilidades de Neve Inverno")]
    [Range(0, 100)] public int chanceNeveFracaInverno = 40;
    [Range(0, 100)] public int chanceNeveMediaInverno = 20;
    [Range(0, 100)] public int chanceNeveForteInverno = 10;

    [Header("Configuraçoes da neve")]
    public bool nevandoNesteMomento { get; private set; }
    private float horaInicioNeve;
    private float horaFimNeve;
    private Coroutine transicaoNeveCoroutine;

    #endregion

    #region Variaveis De Temperatura

    [Header("Temperatura")]
    public float temperaturaAtual;

    [Header("Temperaturas Base por Estação")]
    [SerializeField] private float temperaturaBasePrimavera = 16f;
    [SerializeField] private float temperaturaBaseVerao = 28f;
    [SerializeField] private float temperaturaBaseOutono = 12f;
    [SerializeField] private float temperaturaBaseInverno = -5f;

    [Header("Modificadores")]
    [SerializeField] private AnimationCurve modificadorTemperaturaPorHora;
    [SerializeField] private float modificadorChuva = -6f;
    [SerializeField] private float modificadorNeve = -10f;

    #endregion

    #region Variaveis de Vento

    [Header("Referencias de Vento")]
    public WindZone ventoGlobal;

    [Header("Força Base do Vento por Estação")]
    [Range(0f, 2f)] public float forcaVentoPrimavera = 0.6f;
    [Range(0f, 2f)] public float forcaVentoVerao = 0.2f; // verao com brisa leve
    [Range(0f, 2f)] public float forcaVentoOutono = 0.8f;
    [Range(0f, 2f)] public float forcaVentoInverno = 1.2f; // Inverno venta mais

    [Header("Força Extra durante tempestades")]
    [Range(0f, 5f)] public float forcaExtraTempestade = 3f; // Valor a ser somado durante chuva/neve forte
    [Range(0f, 5f)] public float turbulenciaExtraTempestade = 2.5f;

    #endregion

    #region Methods Iniciais

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Configuração da Chuva
        if (rainParticleSystem != null)
        {
            rainEmission = rainParticleSystem.emission;
            rainVelocity = rainParticleSystem.velocityOverLifetime;
            rainRenderer = rainParticleSystem.GetComponent<ParticleSystemRenderer>();

            rainParticleSystem.Stop(); // Garante que comece parado
        }
        if (splashParticleSystem != null)
        {
            splashParticleSystem.Stop();
        }

        // Configuração da Neve
        if (snowParticleSystem != null)
        {
            snowEmission = snowParticleSystem.emission;
            snowVelocity = snowParticleSystem.velocityOverLifetime;
            snowParticleSystem.Stop();
        }
        if (camadaDeNuvens != null)
        {
            nuvensEmission = camadaDeNuvens.emission;
            nuvensMain = camadaDeNuvens.main;
        }
        DeterminarClimaDoDia(); // Determina o clima para o primeiro dia do jogo
    }

    void Update()
    {
        VerificaChuva();
        VeririficaNeve();
        CalcularTemperaturaAtual();
        CalcularVentoAtual();
    }

    #endregion

    public void DeterminarClimaDoDia()
    {

        // Reseta o clima do dia anterior
        intensidadeAtualDaChuva = RainIntensity.SemChuva;
        intensidadeAtualDaNeve = NeveIntensidade.SemNeve;

        SeasonManager.Estacao estacao = SeasonManager.instance.estacaoAtual;
        //Debug.Log($"Dia {SeasonManager.instance.diaAtualDoAno}, Estação: {estacao}");
        //Verifica se a estação é Inverno.
        if (estacao == SeasonManager.Estacao.Inverno)
        {
            // Se for inverno, sorteia a chance de NEVAR
            int randomValue = Random.Range(0, 101);

            if (randomValue <= chanceNeveForteInverno) intensidadeAtualDaNeve = NeveIntensidade.Forte;
            else if (randomValue <= chanceNeveForteInverno + chanceNeveMediaInverno) intensidadeAtualDaNeve = NeveIntensidade.Media;
            else if (randomValue <= chanceNeveForteInverno + chanceNeveMediaInverno + chanceNeveFracaInverno) intensidadeAtualDaNeve = NeveIntensidade.Fraca;

            if (intensidadeAtualDaNeve != NeveIntensidade.SemNeve)
            {
                horaInicioNeve = Random.Range(0f, 24f);
                horaFimNeve = horaInicioNeve + Random.Range(4f, 12f); // Nevascas podem ser longas
                if (horaFimNeve >= 24f) horaFimNeve -= 24f;
                Debug.Log($"Clima do dia: Neve {intensidadeAtualDaNeve}. Começa às {horaInicioNeve:00}:00 e terminará às {horaFimNeve:00}:00.");
            }
            else
            {
                Debug.Log("Clima do dia: Tempo limpo no Inverno.");
            }
        }
        //Se nao e inverno cai na lojica de chuva
        else
        {
            // Define as chances de chuva para a estação atual
            int chanceFraca, chanceMedia, chanceForte;

            switch (estacao)
            {
                case SeasonManager.Estacao.Primavera:
                    chanceFraca = chanceChuvaFracaPrimavera;
                    chanceMedia = chanceChuvaMediaPrimavera;
                    chanceForte = chanceChuvaFortePrimavera;
                    break;
                case SeasonManager.Estacao.Verao:
                    chanceFraca = chanceChuvaFracaVerao;
                    chanceMedia = chanceChuvaMediaVerao;
                    chanceForte = chanceChuvaForteVerao;
                    break;
                case SeasonManager.Estacao.Outono:
                    chanceFraca = chanceChuvaFracaOutono;
                    chanceMedia = chanceChuvaMediaOutono;
                    chanceForte = chanceChuvaForteOutono;
                    break;
                default:
                    chanceFraca = 0; chanceMedia = 0; chanceForte = 0;
                    break;
            }

            int randomValue = Random.Range(0, 101);

            if (randomValue <= chanceForte) intensidadeAtualDaChuva = RainIntensity.Forte;
            else if (randomValue <= chanceForte + chanceMedia) intensidadeAtualDaChuva = RainIntensity.Media;
            else if (randomValue <= chanceForte + chanceMedia + chanceFraca) intensidadeAtualDaChuva = RainIntensity.Fraca;

            // Define um horário aleatório para a chuva, se houver
            if (intensidadeAtualDaChuva != RainIntensity.SemChuva)
            {
                // A chuva pode durar entre 2h e 8h
                horaInicioChuva = Random.Range(0f, 24f);
                horaFimChuva = Random.Range(horaInicioChuva + 2f, horaInicioChuva + 8f);
                // Garante que não termine depois da meia-noite (pode ajustar)
                if (horaFimChuva >= 24f) horaFimChuva -= 24f;
                Debug.Log($"Clima do dia: Chuva {intensidadeAtualDaChuva}. Começa às {horaInicioChuva:00}:00 e terminará às {horaFimChuva:00}:00.");
            }
            else
            {
                Debug.Log($"Clima do dia: Sem chuva na estação de {estacao}.");
            }
        }
    }

    #region Chuva

    void VerificaChuva()
    {
        // Esta primeira verificação continua igual. O código só roda se houver previsão de chuva para o dia.
        if (intensidadeAtualDaChuva != RainIntensity.SemChuva)
        {
            float horaAtual = WorldTimeManager.instance.GetHoraAtual();

            // Esta nova variável 'dentroDoHorario' vai guardar o resultado (verdadeiro ou falso)
            // se a chuva deve estar ativa agora ou não.
            bool dentroDoHorario;

            // Caso 1: A chuva termina no mesmo dia em que começa (ex: 14h às 18h)
            // Se a hora de início é MENOR que a hora de fim.
            if (horaInicioChuva < horaFimChuva)
            {
                dentroDoHorario = horaAtual >= horaInicioChuva && horaAtual < horaFimChuva;
            }
            // Caso 2: A chuva atravessa a meia-noite (ex: 22h às 2h)
            // Se a hora de início é MAIOR que a hora de fim.
            else
            {
                dentroDoHorario = horaAtual >= horaInicioChuva || horaAtual < horaFimChuva;
            }

            // Se for para chover neste horário...
            if (dentroDoHorario)
            {
                //e não está chovendo ainda, então LIGA a chuva.
                if (!chovendoNesteMomento)
                {
                    ChangeRainIntensity((int)intensidadeAtualDaChuva, true);
                    chovendoNesteMomento = true;
                }
            }
            // Se NÃO for para chover neste horário...
            else
            {
                //e a chuva está ligada, então DESLIGA a chuva.
                if (chovendoNesteMomento)
                {
                    ChangeRainIntensity(0, false);
                    chovendoNesteMomento = false;
                }
            }
        }
        // Se a previsão do dia é "Sem Chuva", garante que qualquer chuva que estivesse ativa seja desligada.
        else
        {
            if (chovendoNesteMomento)
            {
                ChangeRainIntensity(0, false);
                chovendoNesteMomento = false;
            }
        }
    }

    // O 'targetIntensity' pode ser 0 (parar), 1 (fraca), 2 (média), 3 (forte)
    public void ChangeRainIntensity(int targetIntensityEnum, bool activateEffect)
    {
        // Adicionado para parar transições antigas
        if (transicaoChuvaCoroutine != null)
        {
            StopCoroutine(transicaoChuvaCoroutine);
        }

        RainIntensity targetIntensity = (RainIntensity)targetIntensityEnum;

        Color corCeuAlvo = new Color(0.53f, 0.84f, 1f); // Cor de céu azul padrão
        float exposicaoAlvo = 1.3f; // Exposição padrão
        int emissaoNuvensAlvo = 5;
        Color corNuvensAlvo = new Color(0.59f, 0.6f, 0.6f); //#979898

        if (targetIntensity != RainIntensity.SemChuva)
        {
            // Se for chover, deixa o céu cinza e escuro
            corCeuAlvo = new Color(0.27f, 0.31f, 0.35f);
            exposicaoAlvo = 0.2f;
            emissaoNuvensAlvo = 50;
            corNuvensAlvo = new Color(0.31f, 0.33f, 0.37f);
        }
        // Inicia a nova coroutine para fazer a transição do céu
        StartCoroutine(TransicaoDeCeuCoroutine(corCeuAlvo, exposicaoAlvo, 8f));
        StartCoroutine(TransicaoDeNuvensCoroutine(emissaoNuvensAlvo, corNuvensAlvo, 8f));

        float targetRate = 0;
        float targetLengthScale = 0;
        float targetSpeedMin = 0, targetSpeedMax = 0;

        AudioClip somDaChuva = null;
        float volumeDaChuva = 0f;

        switch (targetIntensity)
        {
            // O caso SemChuva não muda
            case RainIntensity.SemChuva:
                targetRate = 0; targetLengthScale = 0; targetSpeedMin = 0; targetSpeedMax = 0;
                break;
            // Os outros casos agora definem um range de velocidade
            case RainIntensity.Fraca:
                targetRate = 500; targetLengthScale = 2; targetSpeedMin = -12; targetSpeedMax = -8;
                somDaChuva = AudioManager.instance.somChuvaFraca;
                volumeDaChuva = 0.4f;
                break;
            case RainIntensity.Media:
                targetRate = 2000; targetLengthScale = 3.5f; targetSpeedMin = -17; targetSpeedMax = -13;
                somDaChuva = AudioManager.instance.somChuvaMedia;
                volumeDaChuva = 0.7f;
                break;
            case RainIntensity.Forte:
                targetRate = 10000; targetLengthScale = 5; targetSpeedMin = -22; targetSpeedMax = -18;
                somDaChuva = AudioManager.instance.somChuvaForte;
                volumeDaChuva = 1.0f;
                break;
        }

        // Faz Os raios aparecerem
        if ((targetIntensity == RainIntensity.Media || targetIntensity == RainIntensity.Forte) && tempestadeCoroutine == null)
        {
            //inicia a tempestade.
            tempestadeCoroutine = StartCoroutine(TempestadeCoroutine());
        }
        // Se a chuva for Fraca ou Sem Chuva, E a coroutine estiver rodando...
        else if ((targetIntensity == RainIntensity.Fraca || targetIntensity == RainIntensity.SemChuva) && tempestadeCoroutine != null)
        {
            //para a tempestade.
            StopCoroutine(tempestadeCoroutine);
            tempestadeCoroutine = null; // Limpa a referência
        }

        AudioManager.instance.TocarSomDeAmbiente(somDaChuva, volumeDaChuva);

        if (rainParticleSystem == null) return;

        if (activateEffect)
        {
            if (!rainParticleSystem.isPlaying) rainParticleSystem.Play();
            if (splashParticleSystem != null && !splashParticleSystem.isPlaying) splashParticleSystem.Play();
        }

        // Inicia a nova coroutine com os novos parâmetros de velocidade
        transicaoChuvaCoroutine = StartCoroutine(TransitionRain(targetRate, targetSpeedMin, targetSpeedMax, targetLengthScale));
    }

    // Coroutine para a transição suave
    IEnumerator TransitionRain(float targetRate, float targetSpeedMin, float targetSpeedMax, float targetLengthScale)
    {
        float transitionDuration = 3.0f;
        float time = 0;

        // Lendo os valores iniciais corretamente do modo "Random Between Two Constants"
        float startRate = rainEmission.rateOverTime.constant;
        float startLengthScale = rainRenderer.lengthScale;
        float startSpeedMin = rainVelocity.y.constantMin; // <-- MUDANÇA AQUI
        float startSpeedMax = rainVelocity.y.constantMax; // <-- MUDANÇA AQUI

        while (time < transitionDuration)
        {
            float t = time / transitionDuration;

            rainEmission.rateOverTime = Mathf.Lerp(startRate, targetRate, t);
            rainRenderer.lengthScale = Mathf.Lerp(startLengthScale, targetLengthScale, t);

            var vel = rainVelocity; // Pega uma cópia do módulo
            // Interpola o valor mínimo e máximo
            float newMin = Mathf.Lerp(startSpeedMin, targetSpeedMin, t);
            float newMax = Mathf.Lerp(startSpeedMax, targetSpeedMax, t);
            // ATRIBUI UMA NOVA CURVA MIN/MAX COM OS VALORES ATUALIZADOS
            vel.y = new ParticleSystem.MinMaxCurve(newMin, newMax);
            // --- FIM DA CORREÇÃO ---

            time += Time.deltaTime;
            yield return null;
        }

        // Garante que os valores finais sejam exatos
        rainEmission.rateOverTime = targetRate;
        rainRenderer.lengthScale = targetLengthScale;
        var finalVel = rainVelocity;
        // Define o valor final usando a mesma estrutura MinMaxCurve
        finalVel.y = new ParticleSystem.MinMaxCurve(targetSpeedMin, targetSpeedMax);

        if (targetRate == 0)
        {
            rainParticleSystem.Stop();
            if (splashParticleSystem != null) splashParticleSystem.Stop();
        }
    }

    private IEnumerator TransicaoDeCeuCoroutine(Color corAlvo, float exposicaoAlvo, float duracao)
    {
        if (ceuMaterial == null) yield break;

        float tempo = 0;
        Color corInicial = ceuMaterial.GetColor("_SkyTint");
        float exposicaoInicial = ceuMaterial.GetFloat("_Exposure");

        while (tempo < duracao)
        {
            // Interpola a cor e a exposição ao longo do tempo
            ceuMaterial.SetColor("_SkyTint", Color.Lerp(corInicial, corAlvo, tempo / duracao));
            ceuMaterial.SetFloat("_Exposure", Mathf.Lerp(exposicaoInicial, exposicaoAlvo, tempo / duracao));

            tempo += Time.deltaTime;
            yield return null;
        }

        // Garante os valores finais
        ceuMaterial.SetColor("_SkyTint", corAlvo);
        ceuMaterial.SetFloat("_Exposure", exposicaoAlvo);
    }

    IEnumerator TransicaoDeNuvensCoroutine(int emissaoAlvo, Color corAlvo, float duracao)
    {
        if (camadaDeNuvens == null) yield break;

        float tempo = 0;
        float emissaoInicial = nuvensEmission.rateOverTime.constant;
        Color corInicial = nuvensMain.startColor.color;

        while (tempo < duracao)
        {
            float t = tempo / duracao;
            // Interpola a taxa de emissão (quantidade de nuvens)
            nuvensEmission.rateOverTime = Mathf.Lerp(emissaoInicial, emissaoAlvo, t);

            // Interpola a cor das nuvens
            var main = camadaDeNuvens.main;
            main.startColor = Color.Lerp(corInicial, corAlvo, t);

            tempo += Time.deltaTime;
            yield return null;
        }

        // Garante os valores finais
        nuvensEmission.rateOverTime = emissaoAlvo;
        var finalMain = camadaDeNuvens.main;
        finalMain.startColor = corAlvo;
    }

    #endregion

    #region Tempestade

    IEnumerator TempestadeCoroutine()
    {
        // Este loop continua enquanto a coroutine estiver ativa
        while (true)
        {
            // 1. Espera um tempo aleatório para o próximo raio
            float tempoDeEspera = Random.Range(minTempoEntreRaios, maxTempoEntreRaios);
            yield return new WaitForSeconds(tempoDeEspera);

            // 2. O clarão do raio acontece (efeito visual)
            StartCoroutine(EfeitoDeRaio());

            // 3. Escolhe um som de trovão aleatório
            AudioClip somDoTrovao = AudioManager.instance.EscolherSomDeTrovaoAleatorio();

            // 4. Espera um pequeno tempo para simular a distância (som viaja mais devagar que a luz)
            float delayDoSom = Random.Range(0.1f, 1.2f);
            yield return new WaitForSeconds(delayDoSom);

            // 5. Toca o som do trovão
            if (somDoTrovao != null)
            {
                AudioManager.instance.TocarFx(somDoTrovao);
            }
        }
    }

    IEnumerator EfeitoDeRaio()
    {
        float intensidadeOriginal = WorldTimeManager.instance.diretionalLight.intensity;
        float duracaoDoClarao = Random.Range(0.1f, 0.25f);

        // Aumenta a intensidade da luz para simular o clarão
        WorldTimeManager.instance.diretionalLight.intensity = intensidadeOriginal * 5f; // 5x mais forte

        yield return new WaitForSeconds(duracaoDoClarao);

        // Volta a intensidade ao normal
        WorldTimeManager.instance.diretionalLight.intensity = intensidadeOriginal;
    }

    #endregion

    #region Neve

    void VeririficaNeve()
    {
        if (intensidadeAtualDaNeve != NeveIntensidade.SemNeve)
        {
            float horaAtual = WorldTimeManager.instance.GetHoraAtual();
            bool dentroDoHorario;

            if (horaInicioNeve < horaFimNeve)
                dentroDoHorario = horaAtual >= horaInicioNeve && horaAtual < horaFimNeve;
            else
                dentroDoHorario = horaAtual >= horaInicioNeve || horaAtual < horaFimNeve;

            if (dentroDoHorario)
            {
                if (!nevandoNesteMomento)
                {
                    ChangeSnowIntensity((int)intensidadeAtualDaNeve, true);
                    nevandoNesteMomento = true;
                }
            }
            else
            {
                if (nevandoNesteMomento)
                {
                    ChangeSnowIntensity(0, false);
                    nevandoNesteMomento = false;
                }
            }
        }
    }

    public void ChangeSnowIntensity(int targetIntensityEnum, bool activateEffect)
    {
        if (transicaoNeveCoroutine != null) StopCoroutine(transicaoNeveCoroutine);

        NeveIntensidade targetIntensity = (NeveIntensidade)targetIntensityEnum;
        float targetRate = 0;
        float targetSpeedMin = 0, targetSpeedMax = 0;

        switch (targetIntensity)
        {
            case NeveIntensidade.Fraca:
                targetRate = 1000; targetSpeedMin = -2; targetSpeedMax = -1;
                break;
            case NeveIntensidade.Media:
                targetRate = 2500; targetSpeedMin = -3; targetSpeedMax = -2;
                break;
            case NeveIntensidade.Forte:
                targetRate = 5000; targetSpeedMin = -4; targetSpeedMax = -3;
                break;
        }

        if (activateEffect)
        {
            if (!snowParticleSystem.isPlaying) snowParticleSystem.Play();
        }

        transicaoNeveCoroutine = StartCoroutine(TransitionSnow(targetRate, targetSpeedMin, targetSpeedMax));
    }

    IEnumerator TransitionSnow(float targetRate, float targetSpeedMin, float targetSpeedMax)
    {
        float transitionDuration = 5.0f; // Transição da neve pode ser mais longa e suave
        float time = 0;

        float startRate = snowEmission.rateOverTime.constant;
        float startSpeedMin = snowVelocity.y.constantMin;
        float startSpeedMax = snowVelocity.y.constantMax;

        while (time < transitionDuration)
        {
            float t = time / transitionDuration;
            snowEmission.rateOverTime = Mathf.Lerp(startRate, targetRate, t);

            var vel = snowVelocity;
            float newMin = Mathf.Lerp(startSpeedMin, targetSpeedMin, t);
            float newMax = Mathf.Lerp(startSpeedMax, targetSpeedMax, t);
            vel.y = new ParticleSystem.MinMaxCurve(newMin, newMax);

            time += Time.deltaTime;
            yield return null;
        }

        snowEmission.rateOverTime = targetRate;
        var finalVel = snowVelocity;
        finalVel.y = new ParticleSystem.MinMaxCurve(targetSpeedMin, targetSpeedMax);

        if (targetRate == 0)
        {
            snowParticleSystem.Stop();
        }
    }

    #endregion

    #region Clima
    void CalcularTemperaturaAtual()
    {
        float tempBase = 0f;

        SeasonManager.Estacao estacao = SeasonManager.instance.estacaoAtual;
        switch (estacao)
        {
            case SeasonManager.Estacao.Primavera:
                tempBase = temperaturaBasePrimavera;
                break;
            case SeasonManager.Estacao.Verao:
                tempBase = temperaturaBaseVerao;
                break;
            case SeasonManager.Estacao.Outono:
                tempBase = temperaturaBaseOutono;
                break;
            case SeasonManager.Estacao.Inverno:
                tempBase = temperaturaBaseInverno;
                break;
        }

        float horaNormalizada = WorldTimeManager.instance.GetHoraAtual() / 24f;

        float modificadorHora = modificadorTemperaturaPorHora.Evaluate(horaNormalizada);

        float modificadorClima = 0f;
        if (chovendoNesteMomento)
        {
            modificadorClima = modificadorChuva;
        }
        else if (nevandoNesteMomento)
        {
            modificadorClima = modificadorNeve;
        }
    
        temperaturaAtual = tempBase + modificadorHora + modificadorClima;
    }
    #endregion

    #region Vento

    void CalcularVentoAtual()
    {
        if (ventoGlobal == null) return;

        // 1 Define uma força e turbulencia base com base na estaçao
        float forcaAlvo = 0f;
        float turbulenciaAlvo = 0.5f; // Uma turbulência base para o vento não ser tão reto

        SeasonManager.Estacao estacao = SeasonManager.instance.estacaoAtual;
        switch (estacao)
        {
            case SeasonManager.Estacao.Primavera:
                forcaAlvo = forcaVentoPrimavera;
                break;
            case SeasonManager.Estacao.Verao:
                forcaAlvo = forcaVentoVerao;
                break;
            case SeasonManager.Estacao.Outono:
                forcaAlvo = forcaVentoOutono;
                break;
            case SeasonManager.Estacao.Inverno:
                forcaAlvo = forcaVentoInverno;
                break;
        }

        // 2 verifica se ha uma tempestade apara adicionar força e turbulencia estra
        if (intensidadeAtualDaChuva == RainIntensity.Forte || intensidadeAtualDaNeve == NeveIntensidade.Forte)
        {
            forcaAlvo += forcaExtraTempestade;
            turbulenciaAlvo += turbulenciaExtraTempestade;
        }

        // 3. Aplica os valores suavemente ao Wind Zone para evitar mudanças bruscas
        // Usamos Lerp para que o vento aumente e diminua gradualmente
        ventoGlobal.windMain = Mathf.Lerp(ventoGlobal.windMain, forcaAlvo, Time.deltaTime * 0.5f);
        ventoGlobal.windTurbulence = Mathf.Lerp(ventoGlobal.windTurbulence, turbulenciaAlvo, Time.deltaTime * 0.5f);
    }

    #endregion
}
