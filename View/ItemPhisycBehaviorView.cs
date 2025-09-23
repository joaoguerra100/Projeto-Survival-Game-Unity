using System.Collections;
using UnityEngine;

public class ItemPhisycBehaviorView : MonoBehaviour
{
    [SerializeField]private float tempoParaColsiao = 1;

    void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(SetItemToPickUp());
    }

    IEnumerator SetItemToPickUp() // DEPOIS DE QUANTO TEMPO O ITEM VAI PODER SER PEGO
    {
        yield return new WaitForSeconds(tempoParaColsiao);

        Destroy(this.gameObject.GetComponentInChildren<Rigidbody>()); // DESTROI O RIGIDBODY DO FILHO DESTE OBJETO

        this.gameObject.transform.parent.gameObject.GetComponent<SphereCollider>().center = this.gameObject.transform.localPosition; // TRAZ O COLISOR ESFERICO PARA A POSIÇAO AONDE O ITEM CAIR

    }

    
}
