using System.Collections;
using UnityEngine;

public class ArmaMelee : MonoBehaviour
{
    [Header("Referencias")]
    public MeleeWeaponItemScriptable weaponData;
    private Animator anim;

    [Header("Ataque Carregado")]
    [SerializeField] private float holdThreshold = 0.6f; // Tempo segurando para atacar forte
    private float holdTime = 0;
    private bool isHolding;
    [HideInInspector] public bool isAttacking = false;

    [Header("Controle de ataque")]
    private bool podeBater = true;

    [Header("Configuração do Dano")]
    [SerializeField] private Transform attackPoint; // lugar aonde sera o ponto de impacto
    [SerializeField] private float attackRadius = 0.8f; //Raio de detecçao
    [SerializeField] private LayerMask enemyLayer;

    [Header("SomFx")]
    private AudioSource audioSourceArma;
    [SerializeField] private AudioClip atackFx;

    void OnEnable()
    {
        if (Hud.instance != null)
        {
            Hud.instance.qtdMunicaoGo.SetActive(false);
        }

        isAttacking = false;
        isHolding = false;
    }

    void Awake()
    {
        anim = GetComponent<Animator>();
        audioSourceArma = GetComponent<AudioSource>();
    }

    public void HandleAttackInput()
    {
        if (isAttacking) return; // bloqueia novos ataques enquanto estiver atacando

        if (Input.GetKeyDown(InputManager.instance.attackKey))
        {
            isHolding = true;
            holdTime = 0f;
        }
        if (isHolding && Input.GetKey(InputManager.instance.attackKey))
        {
            holdTime += Time.deltaTime;
        }
        if (Input.GetKeyUp(InputManager.instance.attackKey))
        {
            isHolding = false;
            PerformAttack();
        }
    }

    void PerformAttack()
    {
        // Ataque Forte
        if (holdTime >= holdThreshold)
        {
            if (Player.instance.stats.estaminaAtual >= weaponData.custoEstaminaAtackForte)
            {
                isAttacking = true;
                PlayerBracos.instance.PrepararVelocidadeDeAtaque();
                PlayerBracos.instance.anim.SetTrigger("AtaqueForte");
                //Player.instance.anim.SetTrigger("AtaqueForte");
                Player.instance.stats.UsarStamina(weaponData.custoEstaminaAtackForte);
            }
        }
        // Ataque Fraco
        else
        {
            if (podeBater && Player.instance.stats.estaminaAtual >= weaponData.custoEstaminaAtackFraco)
            {
                isAttacking = true;
                PlayerBracos.instance.PrepararVelocidadeDeAtaque();
                PlayerBracos.instance.anim.SetTrigger("AtaqueFraco");
                //Player.instance.anim.SetTrigger("AtaqueFraco");
                Player.instance.stats.UsarStamina(weaponData.custoEstaminaAtackFraco);
            }
        }
    }

    #region Animation Event

    public void AplicarDano()
    {
        if (audioSourceArma != null && atackFx != null)
        {
            audioSourceArma.PlayOneShot(atackFx);
        }

        //Detecta todos os inimigos dentro do raio de ataque
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRadius, enemyLayer);

        System.Collections.Generic.List<Zumbi> zumbisAtingidos = new System.Collections.Generic.List<Zumbi>();

        foreach (Collider enemycollider in hitEnemies)
        {
            BodyPartHitbox hitbox = enemycollider.GetComponent<BodyPartHitbox>();
            if (hitbox != null && !zumbisAtingidos.Contains(hitbox.zumbi))
            {
                // Adiciona a IA na lista, para não bater duas vezes
                zumbisAtingidos.Add(hitbox.zumbi);

                // Calcula o dano (exatamente como você fez)
                int danoParaAplicar = (int)weaponData.danoArma;
                AnimatorStateInfo stateInfo = PlayerBracos.instance.anim.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName("AtaqueForte"))
                {
                    danoParaAplicar = (int)(weaponData.danoArma * 1.5f);
                }

                hitbox.healthScript.ReceberDano(danoParaAplicar);
                hitbox.zumbi.ReceberHit(hitbox.parteDoCorpo);
            }
        }
    }

    public void EventoFimAtaque()
    {
        isAttacking = false;
        holdTime = 0f;

        //Restaura a valocidade do animator no final do atack
        PlayerBracos.instance.ResetarVelocidadeAnimacao();
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
