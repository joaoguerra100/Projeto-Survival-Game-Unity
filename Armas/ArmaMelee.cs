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

        if (Input.GetButtonDown("Fire1"))
        {
            isHolding = true;
            holdTime = 0f;
        }
        if (isHolding && Input.GetButton("Fire1"))
        {
            holdTime += Time.deltaTime;
        }
        if (Input.GetButtonUp("Fire1"))
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
            if (Player.instance.estaminaAtual >= weaponData.custoEstaminaAtackForte)
            {
                isAttacking = true;
                PlayerBracos.instance.anim.SetTrigger("AtaqueForte");
                Player.instance.anim.SetTrigger("AtaqueForte");
                Player.instance.UsarStamina(weaponData.custoEstaminaAtackForte);
            }
        }
        // Ataque Fraco
        else
        {
            if (podeBater && Player.instance.estaminaAtual >= weaponData.custoEstaminaAtackFraco)
            {
                isAttacking = true;
                PlayerBracos.instance.anim.SetTrigger("AtaqueFraco");
                Player.instance.anim.SetTrigger("AtaqueFraco");
                Player.instance.UsarStamina(weaponData.custoEstaminaAtackFraco);
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

        foreach (Collider enemycollider in hitEnemies)
        {
            Zumbi zumbis = enemycollider.GetComponent<Zumbi>();
            if (zumbis != null)
            {
                AnimatorStateInfo stateInfo = PlayerBracos.instance.anim.GetCurrentAnimatorStateInfo(0);

                if (stateInfo.IsName("AtaqueForte"))
                {
                    var danoArma = weaponData.danoArma * 1.5f;
                    zumbis.Danos((int)danoArma);
                }
                else
                {
                    zumbis.Danos((int)weaponData.danoArma);
                }
            }
        }
    }

    public void EventoFimAtaque()
    {
        isAttacking = false;
        holdTime = 0f;
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
