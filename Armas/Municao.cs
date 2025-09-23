using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Municao : MonoBehaviour
{
    [SerializeField]private GameObject imagemDeImpacto;

    void Start()
    {
        Destroy(gameObject,10f);
    }

    
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Parede":
                ContactPoint contac = collision.contacts[0];
                GameObject bulletHole = Instantiate(imagemDeImpacto, contac.point + (contac.normal * 0.01f), Quaternion.FromToRotation(Vector3.up, contac.normal));
                Destroy(bulletHole, 5f);
                Destroy(gameObject);
                break;
            case "Chao":
                ContactPoint contac2 = collision.contacts[0];
                GameObject bulletHole2 = Instantiate(imagemDeImpacto, contac2.point + (contac2.normal * 0.01f), Quaternion.FromToRotation(Vector3.up, contac2.normal));
                Destroy(gameObject);
                break;
            case "Inimigo":
                Destroy(gameObject);
                break;
                case"InimigoCabeça":
                Destroy(gameObject);
                break;
        }
    }

}
