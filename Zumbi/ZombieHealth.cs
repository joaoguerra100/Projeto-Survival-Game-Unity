using UnityEngine;
using UnityEngine.AI;

public class ZombieHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxVida = 70;
    private int vidaAtual;

    [Header("Estado")]
    public bool estaMorto = false;
    private Zumbi aiScript;
    private NavMeshAgent agent;
    private Animator anim;
    private ZombieCulling cullingScript;
    public LootTableScriptable tabelaDeLoot;

    [Header("Configuração de Morte e Pool")]
    public float tempoDeDecomposicao = 300f; // 5 minutos
    public string zumbiPoolTag = "Zumbi";
    public GameObject prefabDoCadaver;
    [SerializeField] private float tempoParaSpawnarCadaver = 3.0f;

    private SpawnZone minhaZonaDeOrigem;

    void Awake()
    {
        aiScript = GetComponent<Zumbi>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        cullingScript = GetComponent<ZombieCulling>();
    }

    void OnEnable()
    {
        vidaAtual = maxVida;
        estaMorto = false;

        aiScript.enabled = true;
        agent.enabled = true;
        agent.isStopped = false;
        cullingScript.enabled = true;

        minhaZonaDeOrigem = null;
    }

    public void ReceberDano(int quantidade)
    {
        if (estaMorto) return; // Se já está morto, não faz nada

        vidaAtual -= quantidade;

        if (aiScript.enabled)
        {
            aiScript.ReagirAoHit();
        }

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    public void Morrer()
    {
        estaMorto = true;

        // 1. Avisa os sistemas
        ZombieSpawnManager.Instance.RegistrarMorteDeZumbi();
        if (minhaZonaDeOrigem != null)
        {
            minhaZonaDeOrigem.RemoverMorador();
        }

        // 2. Desliga a IA e toca animação
        aiScript.enabled = false;
        aiScript.StopAllCoroutines();
        agent.isStopped = true;
        agent.enabled = false;
        anim.SetTrigger("Morreu");
        cullingScript.enabled = false;

        // Espera a animaçao terminar para trocar pelo cadaver
        Invoke(nameof(TrnasformarEmCadaver), tempoParaSpawnarCadaver);
        
    }

    void TrnasformarEmCadaver()
    {
        // Cria o Cadaver
        GameObject cadaver = Instantiate(prefabDoCadaver, transform.position, transform.rotation);

        // Passa o loot
        LootCorpos scriptLoot = cadaver.GetComponent<LootCorpos>();
        if (scriptLoot != null && tabelaDeLoot != null)
        {
            scriptLoot.GerarLootInicial(tabelaDeLoot);
        }

        // Devolve o zumbi para o pool
        ObjectPoolController.instance.ReturnToPool(zumbiPoolTag, gameObject);
    }

    public void SetMinhaZona(SpawnZone zona)
    {
        minhaZonaDeOrigem = zona;
    }
}
