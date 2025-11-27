using UnityEngine;

public class CorpseTimer : MonoBehaviour
{
    public float tempoDeDecomposicao = 12000f;

    void Start()
    {
        Destroy(gameObject, tempoDeDecomposicao);
    }
}
