using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawnManager : MonoBehaviour
{
    [Header("Scripts")]
    public static ZombieSpawnManager Instance;

    [Header("Referencias")]
    private List<SpawnZone> zonasProximas = new List<SpawnZone>();
    private List<SpawnZone> zonasPonderadas = new List<SpawnZone>();
    private Collider[] collidersDeZona = new Collider[50];
    private int zumbisAtuais = 0;
    private Camera mainCamera;
    private Plane[] cameraFrustumPlanes;

    [Header("Configurações de Spawn")]
    public string zumbiPoolTag = "Zumbi";
    [HideInInspector]public Transform jogador;
    public int maxZumbisNoMundo;
    public float tempoDeTentativa = 2.0f; // a cada quantos segundos vai tentar spawnar um zumbi
    public int tentativasPorCiclo = 5;

    [Header("Regras de Distância ")]
    public float raioDeSeguranca = 50.0f;
    public float raioDeAtivacao = 150.0f;

    [Header("Controle de Zonas")]
    public LayerMask spawnZoneLayer;
    [Tooltip("O raio que o gerente usa para 'encontrar' zonas. Deve ser maior que o raioDeAtivacao.")]
    public float raioDeBuscaDeZonas = 200.0f;

    [Header("Controle de Densidade")]
    public int quantidadeZumbiBaixa = 100;
    public int quantidadeZumbiMedia = 200;
    public int quantidadeZumbiAlta = 300;

    [Header("Peso da Densidade (Chance)")]
    public int pesoBaixo = 1;
    public int pesoMedio = 3;
    public int pesoAlto = 5;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        jogador = GameObject.FindGameObjectWithTag("Player").transform;
        
        if (jogador == null)
        {
            Debug.LogError("O ZombieSpawnManager precisa da referência do Jogador!");
            return;
        }

        mainCamera = jogador.GetComponentInChildren<Camera>();
        if (mainCamera == null)
        {
            Debug.LogError("O objeto 'Jogador' não tem um componente Câmera!");
        }

        if (SettingsManager.instance != null)
        {
            switch (SettingsManager.instance.currentZombieDensity)
            {
                case SettingsManager.ZombieDensity.Baixa: maxZumbisNoMundo = quantidadeZumbiBaixa; break;
                case SettingsManager.ZombieDensity.Media: maxZumbisNoMundo = quantidadeZumbiMedia; break;
                case SettingsManager.ZombieDensity.Alta: maxZumbisNoMundo = quantidadeZumbiAlta; break;
            }
        }
        else
        { maxZumbisNoMundo = 150; }

        // Chama 'AtualizarZonasProximas' imediatamente (0) e depois a cada 5s
        InvokeRepeating(nameof(AtualizarZonasProximas), 0f, 5.0f);

        // Chama 'TentarSpawnarZumbi' após 1s e depois a cada 'tempoDeTentativa'
        InvokeRepeating(nameof(TentarSpawnarZumbi), 1f, tempoDeTentativa);
    }

    void AtualizarZonasProximas()
    {
        zonasProximas.Clear();
        zonasPonderadas.Clear();

        int count = Physics.OverlapSphereNonAlloc(jogador.position, raioDeBuscaDeZonas, collidersDeZona, spawnZoneLayer, QueryTriggerInteraction.Collide);

        for (int i = 0; i < count; i++)
        {
            // Checa se o collider encontrado tem o script SpawnZone
            if (collidersDeZona[i].TryGetComponent<SpawnZone>(out SpawnZone zona))
            {
                zonasProximas.Add(zona);
                int peso = pesoMedio;
                switch (zona.densidadeDaZona)
                {
                    case SpawnZone.ZonaDensidade.Baixa: peso = pesoBaixo; break;
                    case SpawnZone.ZonaDensidade.Media: peso = pesoMedio; break;
                    case SpawnZone.ZonaDensidade.Alta: peso = pesoAlto; break;
                }

                // Adiciona a zona na lista de apostas X vezes (baseado no peso)
                for (int j = 0; j < peso; j++)
                {
                    zonasPonderadas.Add(zona);
                }
            }
        }
    }

    void TentarSpawnarZumbi()
    {
        if (zumbisAtuais >= maxZumbisNoMundo) { return; }
        if (zonasPonderadas.Count == 0) { return; }

        // garante que ele sempre sabe para onde a câmera está olhando
        cameraFrustumPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

        for (int i = 0; i < tentativasPorCiclo; i++)
        {
            SpawnZone zonaEscolhida = zonasPonderadas[Random.Range(0, zonasPonderadas.Count)];

            if (zonaEscolhida.EstaCheia())
            {
                continue;
            }

            if(!zonaEscolhida.PodeRespawnar())
            {
                continue;
            }

            // CHECA VISIBILIDADE: Esta zona está na tela?
            Collider zonaCollider = zonaEscolhida.GetComponent<Collider>();
            if (GeometryUtility.TestPlanesAABB(cameraFrustumPlanes, zonaCollider.bounds))
            {
                continue;
            }

            // Pede um ponto aleatório DENTRO daquela zona
            Vector3 pontoAleatorio = zonaEscolhida.ObterPontoAleatorioDentroDaZona();
            // O ponto aleatório está no NavMesh?
            if (NavMesh.SamplePosition(pontoAleatorio, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                Vector3 pontoNoNavMesh = hit.position;

                // O ponto é seguro para spawnar?
                float distanciaDoJogador = Vector3.Distance(pontoNoNavMesh, jogador.position);
                if (distanciaDoJogador > raioDeSeguranca && distanciaDoJogador < raioDeAtivacao)
                {
                    // *** AQUI VOCÊ PODE ADICIONAR A LÓGICA DE DENSIDADE ***
                    // Ex: if (zonaEscolhida.densidadeDaZona == ZonaDensidade.Baixa && Random.value < 0.3f) ...
                    // Por enquanto, vamos spawnar de qualquer jeito.

                    GameObject zumbiNovo = ObjectPoolController.instance.SpawnZumbiFromPool(zumbiPoolTag, pontoNoNavMesh, Quaternion.identity);
                    if (zumbiNovo != null)
                    {
                        // 9. ATUALIZA OS CONTADORES
                        zumbiNovo.GetComponent<ZombieHealth>().SetMinhaZona(zonaEscolhida);
                        zonaEscolhida.AdicionarMorador();
                        zumbisAtuais++;

                        return;
                    }
                }
            }
        }
    }

    public void RegistrarMorteDeZumbi()
    {
        zumbisAtuais--;
    }
}
