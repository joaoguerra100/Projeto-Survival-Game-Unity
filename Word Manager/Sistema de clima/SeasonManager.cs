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

    [Header("Configuração das Estações")]
    public Estacao estacaoAtual;
    [SerializeField] private int diasPorEstacao = 10;

    [Header("Contadores")]
    public int diaAtualDoAno = 1;
    public int diaAtualDaEstacao = 1;

    [Header("Configuração do Vento por Estação")]
    [Range(0f, 1f)] public float volumeVentoPrimavera = 0.6f;
    [Range(0f, 1f)] public float volumeVentoVerao = 0.2f;
    [Range(0f, 1f)] public float volumeVentoOutono = 0.4f;
    [Range(0f, 1f)] public float volumeVentoInverno = 0.8f; // Mais vento no inverno

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        CalcularEstacaoInicial();

        AjustarVentoParaEstacaoAtual(); 
    }

    public void AvancarDia()
    {
        diaAtualDoAno++;
        diaAtualDaEstacao++;

        if (diaAtualDaEstacao > diasPorEstacao)
        {
            diaAtualDaEstacao = 1;
            //passa para a proxima estação
            estacaoAtual++;
            //Reseta para primeira estaçao caso acabe o ciclo
            if (estacaoAtual > Estacao.Inverno)
            {
                estacaoAtual = Estacao.Primavera;
            }
            AjustarVentoParaEstacaoAtual();
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
}
