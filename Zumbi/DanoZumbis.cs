using System.Collections;
using UnityEngine;

public class DanoZumbis : MonoBehaviour
{
    private Zumbi zumbi;
    private bool colidiu;
    [SerializeField] private int dano;
    [SerializeField]private float intervaloDeDanos = 1f;

    void Start()
    {
        zumbi = GetComponentInParent<Zumbi>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (colidiu == false && collision.gameObject.CompareTag("Player") && zumbi.isAttacking)
        {
            StartCoroutine(PodeDano());
            collision.gameObject.SendMessage("DanosPlayer", zumbi.danos, SendMessageOptions.DontRequireReceiver);
            Debug.Log(zumbi.danos);
        }
    }

    IEnumerator PodeDano()
    {
        colidiu = true;
        zumbi.isAttacking = false;
        yield return new WaitForSeconds(intervaloDeDanos);
        colidiu = false;
    }

}
