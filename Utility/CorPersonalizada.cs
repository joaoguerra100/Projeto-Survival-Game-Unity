using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class CorPersonalizada : MonoBehaviour
{
    [Header("Cor da casa")]
    public Color corDaCasa = Color.white;

    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        MaterialPropertyBlock block = new MaterialPropertyBlock();

        rend.GetPropertyBlock(block);
        block.SetColor("_Color", corDaCasa); // "_Color" precisa existir no shader
        rend.SetPropertyBlock(block);
    }
}
