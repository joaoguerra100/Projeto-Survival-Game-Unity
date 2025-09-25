using System.Collections;
using UnityEngine;

public class HandCombat : MonoBehaviour
{
    [Header("Scripts")]
    public static HandCombat instance;

    [Header("Ataque Combo")]
    [HideInInspector] public bool espearAtaque;
    [HideInInspector] public bool usarPunho;
    [HideInInspector] public int idCombo;

    [Header("Ataque Carregado")]
    private float holdThreshold = 0.6f;
    private float holdTime = 0;
    private bool isHolding;
    [HideInInspector] public bool isAttacking = false;

    [Header("Controle de ataque")]
    [HideInInspector] public int danos;
    private bool podeBater = true;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (Player.instance.morte) { return; }
        if (!Player.instance.bloqueioControle && !Player.instance.trocaAnimator && !PauseController.instance.visiblePanel && !InventoryView.instance.VisiblePanel)
        {
            Golpear();
        }

    }

    public void Golpear()
    {
        if (isAttacking) return; // bloqueia novos ataques enquanto estiver atacando
        if (PlayerBracos.instance.anim.runtimeAnimatorController == PlayerBracos.instance.controllerDesarmado)
        {
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
                if (holdTime >= holdThreshold)
                {
                    const float custoEstaminaForte = 15f;
                    if (Player.instance.stats.estaminaAtual >= custoEstaminaForte)
                    {
                        PlayerBracos.instance.anim.SetTrigger("AtaqueForte");
                        Player.instance.anim.SetTrigger("AtaqueForte");

                        danos = 16;
                        Player.instance.UsarStamina(custoEstaminaForte);

                        isAttacking = true;
                        StartCoroutine(TempoParaBater(2.5f));
                    }
                    return;
                }
                if (!usarPunho && !espearAtaque && podeBater)
                {
                    float[] danosCombo = { 6f, 6f, 10f };
                    float[] estaminaCombo = { 5f, 5f, 8f };

                    idCombo = Mathf.Clamp(idCombo + 1, 1, 3);
                    int index = idCombo - 1;

                    float custoEstamina = estaminaCombo[index];

                    if (Player.instance.stats.estaminaAtual >= custoEstamina)
                    {
                        usarPunho = true;
                        espearAtaque = true;

                        PlayerBracos.instance.anim.SetInteger("IdCombo", idCombo);
                        Player.instance.anim.SetInteger("IdCombo", idCombo);

                        danos = (int)danosCombo[index];
                        Player.instance.UsarStamina(custoEstamina);

                        if (idCombo >= 3)
                        {
                            StartCoroutine(TempoParaBater(1.2f));
                        }
                    }

                    else
                    {
                        // Se não tiver estamina suficiente, não aumenta o combo
                        idCombo = Mathf.Clamp(idCombo - 1, 0, 3);
                    }
                }
            }
        }
    }

    IEnumerator TempoParaBater(float tempo)
    {
        podeBater = false;
        yield return new WaitForSeconds(tempo);
        podeBater = true;
    }

    #region Animation Event

    public void ComboIniciar()
    {
        usarPunho = false;
        espearAtaque = false;
    }
    public void ComboCancelado()
    {
        usarPunho = false;
        espearAtaque = false;
        //tempoPulo = false;
        idCombo = 0;
        PlayerBracos.instance.anim.SetInteger("IdCombo", idCombo);
        Player.instance.anim.SetInteger("IdCombo", idCombo);
        StartCoroutine(TempoParaBater(0.8f));
    }

    public void EndAttack()
    {
        isAttacking = false;
    }
    #endregion
}
