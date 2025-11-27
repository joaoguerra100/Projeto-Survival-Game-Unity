using UnityEngine;

public class SeasonManager : MonoBehaviour
{
    [Header("Referencias")]
    public static SeasonManager instance;
    public enum Estacao
    {
        Primavera, //Tempo normal
        Verao, //Calor
        Outono, // Chuva
        Inverno // Frio
    }

    public enum FaseDaLua
    {
        Nova,
        Crescente,
        Cheia,
        Minguante
    }

    [Header("Configuração das Estações")]
    public Estacao estacaoAtual;
    [SerializeField] private int diasPorEstacao = 15;
    private Estacao estacaoAnterior;

    [Header("Contadores")]
    public int diaAtualDoAno = 1;
    public int diaAtualDaEstacao = 1;

    [Header("Configuração do Vento por Estação")]
    [Range(0f, 1f)] public float volumeVentoPrimavera = 0.5f;
    [Range(0f, 1f)] public float volumeVentoVerao = 0.2f;
    [Range(0f, 1f)] public float volumeVentoOutono = 0.6f;
    [Range(0f, 1f)] public float volumeVentoInverno = 0.8f; // Mais vento no inverno

    [Header("Ciclo Lunar")]
    public FaseDaLua faseDaLuaAtual;
    [SerializeField] private int diasDoCicloLunar = 28; // Duração de um ciclo completo da lua

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        CalcularEstacaoInicial();
        CalcularFaseDaLua();
        AjustarVentoParaEstacaoAtual();
        estacaoAnterior = estacaoAtual;
    }

    public void AvancarDia()
    {
        diaAtualDoAno++;
        diaAtualDaEstacao++;

        bool mudouDeEstacao = false;

        if (diaAtualDaEstacao > diasPorEstacao)
        {
            diaAtualDaEstacao = 1;
            estacaoAtual++;
            if (estacaoAtual > Estacao.Inverno)
            {
                estacaoAtual = Estacao.Primavera;
            }
            mudouDeEstacao = true;
            AjustarVentoParaEstacaoAtual();
        }

        CalcularFaseDaLua();
        if (mudouDeEstacao)
        {
            AjustarVentoParaEstacaoAtual();
            AudioManager.instance.AtualizarSomAmbienteDetalhes();
        }
        // Avisa o ClimaManager para decidir o clima do novo dia
        ClimaManager.instance.DeterminarClimaDoDia();
    }

    void AjustarVentoParaEstacaoAtual()
    {
        float volumeAlvo = 0f;
        switch (estacaoAtual)
        {
            case Estacao.Primavera:
                volumeAlvo = volumeVentoPrimavera;
                break;
            case Estacao.Verao:
                volumeAlvo = volumeVentoVerao;
                break;
            case Estacao.Outono:
                volumeAlvo = volumeVentoOutono;
                break;
            case Estacao.Inverno:
                volumeAlvo = volumeVentoInverno;
                break;
        }
        AudioManager.instance.AjustarVentoDeEstacao(volumeAlvo);
    }


    void CalcularEstacaoInicial()
    {
        // Lógica simples para definir a estação se o jogo começar em um dia diferente de 1
        int estacaoIndex = (diaAtualDoAno / diaAtualDaEstacao) % 4;
        estacaoAtual = (Estacao)estacaoIndex;
    }

    void CalcularFaseDaLua()
    {
        // Usa o operador de módulo (%) para encontrar o dia atual dentro do ciclo lunar
        int diaNoCiclo = (diaAtualDoAno - 1) % diasDoCicloLunar;

        // Define a fase com base no dia do ciclo (dividindo 28 dias em 4 fases)
        if (diaNoCiclo < 7)
        {
            faseDaLuaAtual = FaseDaLua.Nova; // 1ª semana: Lua Nova (escuridão)
        }
        else if (diaNoCiclo < 14)
        {
            faseDaLuaAtual = FaseDaLua.Crescente; // 2ª semana: Crescente
        }
        else if (diaNoCiclo < 21)
        {
            faseDaLuaAtual = FaseDaLua.Cheia; // 3ª semana: Lua Cheia (claridade máxima)
        }
        else
        {
            faseDaLuaAtual = FaseDaLua.Minguante; // 4ª semana: Minguante
        }

        //Debug.Log($"Dia do ano: {diaAtualDoAno}. A fase da lua agora é: {faseDaLuaAtual}");
    }
}
