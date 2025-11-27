using UnityEngine;

public class ZombieCulling : MonoBehaviour
{
    public float distanciaParaDesaparecer = 200f;
    public float tempoDeChecagem = 5f;
    public string zumbiPoolTag = "Zumbi";
    private Transform jogador;
    void Start()
    {
        if (ZombieSpawnManager.Instance != null)
        {
            jogador = ZombieSpawnManager.Instance.jogador;
        }
        else
        {
            Debug.LogError("Culling não achou o Jogador!");
            Destroy(this); // Desativa este script se não achar o jogador
            return;
        }
        InvokeRepeating(nameof(ChecarDistancia), Random.Range(0f, tempoDeChecagem), tempoDeChecagem);
    }

    void ChecarDistancia()
    {
        if (jogador == null) return;

        float distancia = Vector3.Distance(transform.position, jogador.position);

        if (distancia > distanciaParaDesaparecer)
        {
            ZombieSpawnManager.Instance.RegistrarMorteDeZumbi();

            ObjectPoolController.instance.ReturnToPool(zumbiPoolTag, gameObject);
        }
    }
}
