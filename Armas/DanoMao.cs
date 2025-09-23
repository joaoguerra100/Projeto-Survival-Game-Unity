using System.Collections;
using UnityEngine;

public class DanoMao : MonoBehaviour
{
   private bool colidiu;
    [SerializeField]private float intervaloDeDanos = 0.5f;
    private float ramdom;

    void OnTriggerEnter(Collider collision)
    {
        if (colidiu == false && collision.gameObject.CompareTag("Inimigo"))
        {
            StartCoroutine(PodeDano());
            ramdom = Random.Range(0, 100);
            if (ramdom >= 75)
            {
                Animator anim = collision.GetComponentInParent<Animator>();
                if (anim != null)
                {
                    anim.SetFloat("HitIndex", 0f);
                }
            }
            collision.gameObject.SendMessage("Danos", HandCombat.instance.danos, SendMessageOptions.DontRequireReceiver);
        }
    }

    IEnumerator PodeDano()
    {
        colidiu = true;
        yield return new WaitForSeconds(intervaloDeDanos);
        colidiu = false;
    }
}
