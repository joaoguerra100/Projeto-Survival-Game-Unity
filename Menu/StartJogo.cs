using UnityEngine;
using UnityEngine.SceneManagement;

public class StartJogo : MonoBehaviour
{
    public int numeroFase;

    public void ButaoStar()
    {
        Invoke("LoadJogo", 1f);
    }

    void LoadJogo()
    {
        SceneManager.LoadScene(numeroFase);
    }
}
