using System.Collections;
using UnityEngine;

public class ItemPhisycBehaviorView : MonoBehaviour
{
    [SerializeField] private float tempoParaColsiao = 1;
    private bool jaIniciouParada = false;
    void OnCollisionEnter(Collision collision)
    {
        if (jaIniciouParada) return;

        StartCoroutine(SetItemToPickUp());
    }

    IEnumerator SetItemToPickUp() // DEPOIS DE QUANTO TEMPO O ITEM VAI PODER SER PEGO
    {
        jaIniciouParada = true;

        yield return new WaitForSeconds(tempoParaColsiao);

        Rigidbody rb = gameObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            //Destroy(rb);
            rb.isKinematic = true;
        }
        else
        {
            Debug.LogWarning("Não foi encontrado um Rigidbody para destruir.", gameObject);
        }
    }
}
