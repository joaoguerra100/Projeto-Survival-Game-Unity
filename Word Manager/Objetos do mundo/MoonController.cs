using UnityEngine;

public class MoonController : MonoBehaviour
{
    [Header("Referencias")]
    private MeshRenderer meshRenderer;
    private Quaternion rotacaoAlvo;

    [Header("Configurações de rotação por fase")]
    public float rotacaoLuaNova = 0f;
    public float rotacaoLuaCrescente = 9f;
    public float rotacaoLuaCheia = 180f;
    public float rotacaoLuaMinguante = 270f;
    public float velocidadeTransicao = 2f;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogError("MoonController: MeshRenderer não encontrado neste GameObject!");
        }
    }
    void Start()
    {
        AtualizarRotacaoAlvoPelaFase();
        transform.localRotation = rotacaoAlvo;
    }


    void Update()
    {
        bool ehNoite = WorldTimeManager.instance != null && WorldTimeManager.instance.isNight;
        if (meshRenderer != null)
        {
            meshRenderer.enabled = ehNoite;
        }

        if (ehNoite)
        {
            AtualizarRotacaoAlvoPelaFase();

            transform.localRotation = Quaternion.Slerp(transform.localRotation, rotacaoAlvo, Time.deltaTime * velocidadeTransicao);
        }
    }

    void AtualizarRotacaoAlvoPelaFase()
    {
        if (SeasonManager.instance == null) return;

        float targetYRotation = 0f;
        SeasonManager.FaseDaLua fase = SeasonManager.instance.faseDaLuaAtual;

        switch (fase)
        {
            case SeasonManager.FaseDaLua.Nova:
                targetYRotation = rotacaoLuaNova;
                break;
            case SeasonManager.FaseDaLua.Crescente:
                targetYRotation = rotacaoLuaCrescente;
                break;
            case SeasonManager.FaseDaLua.Cheia:
                targetYRotation = rotacaoLuaCheia;
                break;
            case SeasonManager.FaseDaLua.Minguante:
                targetYRotation = rotacaoLuaMinguante;
                break;
        }
        rotacaoAlvo = Quaternion.Euler(transform.localEulerAngles.x, targetYRotation, transform.localEulerAngles.z);
    }
}
