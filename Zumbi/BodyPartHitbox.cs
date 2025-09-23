using UnityEngine;



public class BodyPartHitbox : MonoBehaviour
{

    public ParteDoCorpo parteDoCorpo;
    public Zumbi zumbi;

    void Start()
    {

    }


    void Update()
    {

    }
}

public enum ParteDoCorpo
{
    CABECA,
    BRACO_ESQUERDO,
    BRACO_DIREITO,
    PERNA_ESQUERDA,
    PERNA_DIREITA
}
