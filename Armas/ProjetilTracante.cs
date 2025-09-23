using UnityEngine;

public class ProjetilTracante : MonoBehaviour
{

    public float velocidade = 100f;
    private Vector3 direcao;
    private float tempoVida = 2f;
    private float timer;

    void Update()
    {
        transform.position += direcao * velocidade * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= tempoVida)
        {
            RetornarParaPool();
        }
    }

    public void Tracante(Vector3 dir)
    {
        direcao = dir.normalized;
        timer = 0f;

        TrailRenderer trail = GetComponent<TrailRenderer>();
        if (trail != null)
        {
            trail.Clear();
        }
    }

    void RetornarParaPool()
    {
        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        RetornarParaPool();
    }


}
