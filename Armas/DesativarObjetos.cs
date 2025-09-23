using UnityEngine;

public class DesativarObjetos : MonoBehaviour
{
    public float tempo = 5f;

    void OnEnable() // ESTA FUNÇAO E CHAMADA QUANDO O OBJETO E ABILITADO E ATIVO
    {
        CancelInvoke(); // CANVELA QUALQUER INVOKE QUE AINDA N FOI CHAMADO (ELE E USADO PARA QUE QUANDO O OBJETO FOR REATIVADO DNOVO, SEJA DESATIVADO CHAMANDO DENOVO A FUNÇAO)
        Invoke("Desativar", tempo); // INVOCA UMA FUNÇAO DEPOIS DE UM TEMPO
    }

    void Desativar()
    {
        gameObject.SetActive(false);
    }
    
}
