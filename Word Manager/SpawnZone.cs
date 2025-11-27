using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    public enum ZonaTipo
    {
        Interior,
        Exterior
    }

    public enum ZonaDensidade
    {
        Baixa,
        Media,
        Alta
    }

    [Header("Configurações da Zona")]
    public ZonaTipo tipoDaZona;
    public ZonaDensidade densidadeDaZona;
    private BoxCollider zoneCollider;
    private float proximoRespawnPermitido = 0f;

    [Header("População")]
    public int populacaoDesejada = 10; // Quantos zumbis "moram" aqui
    [HideInInspector] public int populacaoAtual = 0;

    void Awake()
    {
        zoneCollider = GetComponent<BoxCollider>();
        if (zoneCollider == null)
        {
            Debug.LogError("SpawnZone " + gameObject.name + " está sem um BoxCollider!");
        }
    }

    public Vector3 ObterPontoAleatorioDentroDaZona()
    {
        if (zoneCollider == null) return Vector3.zero;

        Bounds bounds = zoneCollider.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);
        float y = bounds.center.y - bounds.extents.y;

        return new Vector3(x, y, z);
    }

    public void AdicionarMorador()
    {
        populacaoAtual++;
    }

    public void RemoverMorador()
    {
        if (populacaoAtual > 0)
        {
            populacaoAtual--;
        }

        if(populacaoAtual <= 0)
        {
            IniciarTimerDeRespawn();
        }
    }

    public bool EstaCheia()
    {
        return populacaoAtual >= populacaoDesejada;
    }

    public void IniciarTimerDeRespawn()
    {
        SettingsManager.RespawnTime respawnGlobal = SettingsManager.instance.currentRespawnTime;

        if (respawnGlobal == SettingsManager.RespawnTime.nenhum)
        {
            proximoRespawnPermitido = Time.time + 99999999f;
            return;
        }

        float diasParaRespawn = 0f;
        switch (respawnGlobal)
        {
            case SettingsManager.RespawnTime.UmDia: diasParaRespawn = 1f; break;
            case SettingsManager.RespawnTime.TresDias: diasParaRespawn = 3f; break;
            case SettingsManager.RespawnTime.CincoDias: diasParaRespawn = 5f; break;
            case SettingsManager.RespawnTime.SeteDias: diasParaRespawn = 7f; break;
        }

        float segundosReaisPorDia = WorldTimeManager.instance.GetSegundosReaisPorDia();

        float cooldownEmSegundosReais = diasParaRespawn * segundosReaisPorDia;

        proximoRespawnPermitido = Time.time + cooldownEmSegundosReais;
    }

    public bool PodeRespawnar()
    {
        return Time.time >= proximoRespawnPermitido;
    }
}
