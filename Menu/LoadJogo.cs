using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadJogo : MonoBehaviour
{
    public int numeroFase;
    private float contagem;



    void Update()
    {
        contagem += Time.deltaTime;

        if (contagem >= 3f)
        {
            SceneManager.LoadScene(numeroFase);
        }
    }
}
