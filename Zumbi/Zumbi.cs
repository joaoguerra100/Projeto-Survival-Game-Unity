using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Zumbi : MonoBehaviour
{
    #region Variaveis
    [Header("Scripts")]
    private ControladorAudioZumbi zumbiSom;
    private ZombieHealth healthScript;

    [Header("Referencias")]
    private Animator anim;
    private NavMeshAgent navMesh;
    private AudioSource[] sons;
    private Transform player;

    [Header("Randomizaçao")]
    private int randomicoPatrulha;
    private int randomicoEscolha;
    private int randomicoTapa;
    private int randomicoAlerta;

    [Header("Boleanas De IA")]
    public bool acertou;
    public bool alerta;
    public bool avistou;
    public bool patrulha;
    public bool procurarSom;
    public bool travarUpdate;

    [Header("Raios De Visao")]
    [SerializeField] private Vector3 raioPatrulha;
    [SerializeField] private float raioAlerta;
    [SerializeField] private float raioAtaque;
    [SerializeField] private float raioFugir;

    [Header("Movimentaçao")]
    [SerializeField] private float velocidadeRotacao;
    [SerializeField] private float moverFrente;
    private float temporariaDist;

    [Header("Ataques")]
    [SerializeField] private Transform pontoDeMordida; // ponto onde o zumbi deve estar para iniciar a animação
    [HideInInspector] public int danos;
    [HideInInspector] public bool isAttacking;
    private bool deveResetarStopDistance = true;

    [Header("PerseguirSom")]
    private Vector3 lastHeardPosition;


    #endregion

    #region Methodos

    void Awake()
    {
        sons = GetComponentsInChildren<AudioSource>();
        anim = GetComponent<Animator>();
        navMesh = GetComponent<NavMeshAgent>();
        zumbiSom = GetComponent<ControladorAudioZumbi>();
        healthScript = GetComponent<ZombieHealth>();

        foreach (var hitbox in GetComponentsInChildren<BodyPartHitbox>())
        {
            hitbox.zumbi = this;
            hitbox.healthScript = this.healthScript;
        }
    }

    void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // Reseta as booleanas da IA para o estado "vivo" padrão
        patrulha = true;
        alerta = false;
        acertou = false;
        avistou = false;
        procurarSom = false;
        travarUpdate = false;
        isAttacking = false;

        anim.SetFloat("IdleIndex", Random.Range(0, 3));
        anim.SetFloat("AndarIndex", Random.Range(0, 4));
    }

    void Update()
    {
        if (healthScript.estaMorto) { return; }
        Animacoes();
        if (acertou == true | avistou == true) { MetodoOlhar(); }
        ControladorBatalha();
        ControladorPatrulha();
        ControladorAlerta();
        MetodoDistancia();
        PerderDeVista();

    }

    #endregion

    #region Hit

    public void ReagirAoHit()
    {
        StopCoroutine(IeAtacarTapa());
        StopCoroutine(IEAtacarMordida());
        acertou = true; // Faz o zumbi te ver

        if (navMesh.enabled && navMesh.isOnNavMesh)
        {
            StartCoroutine(PararAndar());
        }
    }

    public void ReceberHit(ParteDoCorpo part)
    {
        anim.SetTrigger("Hit");
        switch (part)
        {
            case ParteDoCorpo.CABECA:
                anim.SetFloat("HitIndex", 0f);
                break;
            case ParteDoCorpo.BRACO_ESQUERDO:
                anim.SetFloat("HitIndex", 1f);
                break;
            case ParteDoCorpo.BRACO_DIREITO:
                anim.SetFloat("HitIndex", 2f);
                break;
            case ParteDoCorpo.PERNA_DIREITA:
                anim.SetFloat("HitIndex", 3f);
                break;
            case ParteDoCorpo.PERNA_ESQUERDA:
                anim.SetFloat("HitIndex", 4f);
                break;
        }
    }

    IEnumerator PararAndar()
    {
        float speed = gameObject.GetComponent<NavMeshAgent>().speed;
        gameObject.GetComponent<NavMeshAgent>().speed = 0;
        yield return new WaitForSeconds(2f);
        gameObject.GetComponent<NavMeshAgent>().speed = speed;
    }

    #endregion

    #region Patrulha

    void ControladorPatrulha()
    {
        //PATRULHA
        if (patrulha && navMesh.remainingDistance <= navMesh.stoppingDistance && !alerta && !avistou && !acertou && !procurarSom)
        {
            StartCoroutine(IePatrulha());
        }
    }

    IEnumerator IePatrulha()
    {
        yield return new WaitForSeconds(3f);
        randomicoPatrulha = Random.Range(0, 100);

        if (randomicoPatrulha <= 50 && navMesh.remainingDistance <= navMesh.stoppingDistance && patrulha == true)
        {
            MetodoPatrulhar();
        }
        else
        {
            yield return new WaitForSeconds(3f);
        }
    }

    void MetodoPatrulhar()
    {
        Vector3 temporarioPos = transform.position + new Vector3(Random.Range(-raioPatrulha.x, raioPatrulha.x), Random.Range(-raioPatrulha.y, raioPatrulha.y), Random.Range(-raioPatrulha.z, raioPatrulha.z));
        navMesh.destination = temporarioPos;
    }

    #endregion

    #region Ataque

    void ControladorBatalha()
    {
        if (avistou == true | acertou == true && travarUpdate == false)
        {
            procurarSom = false;
            navMesh.updateRotation = true;
            patrulha = false;
            alerta = false;

            StopCoroutine(IeAlerta());
            StopCoroutine(IePatrulha());

            navMesh.destination = player.position;
            MetodoOlhar();

            if (navMesh.remainingDistance <= navMesh.stoppingDistance)
            {
                temporariaDist = Vector3.Distance(transform.position, player.position);

                if (temporariaDist <= raioAtaque)
                {
                    // Atacar só se ainda não estiver atacando
                    if (!travarUpdate)
                    {
                        MetodoEscolha();
                    }
                }
            }
        }

        // Resetar travarUpdate somente se o player realmente saiu do raio
        if (travarUpdate)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            if (dist > raioAtaque + 1f) // um buffer pra não ficar piscando
            {
                //Debug.Log("Resetando travarUpdate: player fora do alcance.");
                StopAllCoroutines();
                anim.SetInteger("IdBatalha", 0);
                ResetarStopDistance();
                travarUpdate = false;
            }
        }

        else
        {
            patrulha = true;
        }
    }

    void MetodoEscolha()
    {
        randomicoEscolha = Random.Range(0, 100);
        if (randomicoEscolha <= 50)
        {
            navMesh.stoppingDistance = 1.5f;
            raioAtaque = 1.5f;
            deveResetarStopDistance = false;
            StartCoroutine(IEAtacarMordida());
        }
        else
        {
            navMesh.stoppingDistance = 0.7f;
            raioAtaque = 0.7f;
            deveResetarStopDistance = true;
            StartCoroutine(IeAtacarTapa());
        }
    }

    IEnumerator IEAtacarMordida()
    {
        travarUpdate = true;
        yield return new WaitForSeconds(2.30f);
        // RECUA levemente se estiver muito colado no player
        if (Vector3.Distance(transform.position, player.position) < 1.4f)
        {
            Vector3 direcao = (transform.position - player.position).normalized;
            navMesh.Move(direcao * 0.5f); // Ajuste o valor se necessário
        }
        anim.SetInteger("IdBatalha", 2);
        PlayerBracos.instance.anim.SetTrigger("LevarMordida");
        Player.instance.anim.SetTrigger("LevarMordida");
        isAttacking = true;
        Player.instance.TravarMovimento(2f);
        danos = 15;
        navMesh.updateRotation = false;
        navMesh.isStopped = true;
        anim.applyRootMotion = true;
        yield return new WaitForSeconds(2.30f);
        navMesh.updateRotation = true;
        navMesh.isStopped = false;
        anim.applyRootMotion = false;
        anim.SetInteger("IdBatalha", 0);
        travarUpdate = false;
    }

    IEnumerator IeAtacarTapa()
    {
        travarUpdate = true;
        yield return new WaitForSeconds(2.15f);
        randomicoTapa = Random.Range(0, 3);
        anim.SetInteger("IdBatalha", 1);
        anim.SetFloat("TapaIndex", randomicoTapa);
        isAttacking = true;
        danos = 8;
        ChanceDeSangramento(20f);
        yield return new WaitForSeconds(2.15f);
        if (deveResetarStopDistance)
        {
            ResetarStopDistance();
        }
        anim.SetInteger("IdBatalha", 0);
        travarUpdate = false;
    }

    void ChanceDeSangramento(float chancePorcentual)
    {
        float sorteio = Random.Range(0, 100);
        if (sorteio <= chancePorcentual)
        {
            Player.instance.stats.AplicarSangramento(true);
        }
    }
    void ChanceDeMordida()
    {
        randomicoEscolha = Random.Range(0, 100);
    }

    void ResetarStopDistance()
    {
        navMesh.stoppingDistance = 1.5f;
        raioAtaque = 1.5f;
    }

    #endregion

    #region Alerta

    void ControladorAlerta()
    {
        //ALERTA
        if (alerta == true && avistou == false && acertou == false)
        {
            MetodoAlerta();
        }
    }

    void MetodoAlerta()
    {
        gameObject.GetComponent<NavMeshAgent>().isStopped = true;
        //StopAllCoroutines();
        StartCoroutine(IeAlerta());
    }

    IEnumerator IeAlerta()
    {
        yield return new WaitForSeconds(5f);
        randomicoAlerta = Random.Range(0, 100);
        if (randomicoAlerta <= 50)
        {
            MetodoOlhar();
            yield return new WaitForSeconds(0.1f);
        }

        else
        {
            yield return new WaitForSeconds(5f);
            StopAllCoroutines();
        }
    }

    #endregion

    #region Olhar/Distancia

    //OLHAR
    void MetodoOlhar()
    {
        Vector3 direcao = (player.position - transform.position).normalized;
        Quaternion rotacao = Quaternion.LookRotation(direcao);
        Quaternion atual = transform.localRotation;
        transform.localRotation = Quaternion.Lerp(atual, rotacao, velocidadeRotacao * Time.deltaTime);

        /*if (atual.y < 0f && avistou == false && acertou == false && alerta == true)
        {
            anim.SetBool("GiroEsq", true);
            anim.SetBool("GiroDir", false);
        }
        else if (atual.y > 0f && avistou == false && acertou == false && alerta == true)
        {
            anim.SetBool("GiroDir", true);
            anim.SetBool("GiroEsq", false);
        }*/
    }

    void MetodoDistancia()
    {
        temporariaDist = Vector3.Distance(transform.position, player.position);

        if (temporariaDist <= raioAlerta && avistou == false)
        {
            alerta = true;
        }
        else
        {
            alerta = false;
            gameObject.GetComponent<NavMeshAgent>().isStopped = false;

        }
    }

    #endregion

    #region PerseguirSom

    public void InvestigarArea(Vector3 position)
    {
        procurarSom = true;
        if (!avistou && !acertou && procurarSom)
        {
            patrulha = false;
            lastHeardPosition = position;
            navMesh.destination = lastHeardPosition;
        }
    }

    #endregion

    #region Ontrigger

    //AVISTOU
    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            avistou = true;
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            avistou = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            avistou = false;
        }
    }

    #endregion

    #region Animation Event

    /*public void MoverParaFrente()
    {
        if (navMesh.remainingDistance < moverFrente)
        {
            Vector3 distancia = transform.position + transform.forward * 4f;
            NavMesh.SamplePosition(distancia, out NavMeshHit hit, 4f, NavMesh.AllAreas);
            navMesh.SetDestination(hit.position);
        }
    }*/
    public void MoverParaTras()
    {
        Vector3 distancia = transform.position - transform.forward * 3f;
        NavMesh.SamplePosition(distancia, out NavMeshHit hit, 3f, NavMesh.AllAreas);
        navMesh.SetDestination(hit.position);
        Quaternion rotacaoOriginal = transform.rotation;
        navMesh.SetDestination(hit.position);
        transform.rotation = rotacaoOriginal; // Reinforce rotation if needed*/
        navMesh.updateRotation = false;
    }

    public void SomVozIdleFx()
    {
        zumbiSom.TocarIdle();
    }

    public void SomVozAttackMaoFx()
    {
        zumbiSom.TocarAttackMao();
    }

    /*public void SomVozAttackMordidaFx()
    {
        zumbiSom.TocarAttackMordida();
    }*/

    public void SomVozHitFx()
    {
        zumbiSom.TocarDamage();
    }

    public void SomVozMorteFx()
    {
        zumbiSom.TocarDeath();
    }

    public void PararSomFx()
    {
        foreach (AudioSource audio in sons)
        {
            if (audio.isPlaying)
            {
                audio.Stop();
            }
        }
    }

    #endregion

    void Animacoes()
    {
        anim.SetFloat("Velodidade", navMesh.velocity.magnitude);
        anim.SetBool("Alerta", alerta);
    }

    void PerderDeVista()
    {
        float distanciaReal = Vector3.Distance(transform.position, player.position);

        if (distanciaReal >= raioFugir)
        {
            acertou = false;
            avistou = false;
            alerta = false;
            patrulha = true;
            navMesh.isStopped = false;
            //Debug.Log("Jogador saiu do alcance, zumbi voltando à patrulha.");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raioAlerta);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, raioFugir);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, raioPatrulha * 2f);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, raioAtaque);
    }
}
