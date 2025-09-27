using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [Header("Scripts")]
    public static GameOverController instance;

    [Header("GameOver")]
    [SerializeField] private GameObject painelGameOver;
    [HideInInspector] public bool travarGameOver;
    [SerializeField] private GameObject btnGameOver;
    private int fase;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        painelGameOver.SetActive(false);
    }


    void Update()
    {
        GameOver();
    }

    void GameOver()
    {
        if (Player.instance.morte == true && travarGameOver == false)
        {
            travarGameOver = true;
            Cursor.lockState = CursorLockMode.None;
            AudioManager.instance.MetodoSomFxHud(AudioManager.instance.fxGameOver[Random.Range(0, AudioManager.instance.fxGameOver.Length)]);
            EventSystem.current.SetSelectedGameObject(btnGameOver);
            Hud.instance.DesativarHud();
            StartCoroutine(IEGameOver());
        }
        if (Input.GetButtonDown("Horizontal") && travarGameOver)
        {
            AudioManager.instance.MetodoSomFxHud(AudioManager.instance.fxBtnMove);
        }
    }

    public void BotaoGameOver()
    {
        EventSystem.current.SetSelectedGameObject(btnGameOver);
    }

    IEnumerator IEGameOver()
    {
        yield return new WaitForSeconds(6f);
        painelGameOver.SetActive(true);
        Time.timeScale = 0;
    }

    public void Confirmar()
    {
        AudioManager.instance.MetodoSomFxHud(AudioManager.instance.fxConfirmar);
    }

    public void Cancelar()
    {
        AudioManager.instance.MetodoSomFxHud(AudioManager.instance.fxCancelar);
    }

    public void Reiniciar()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }
    public void VoltarTelaInicial()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
