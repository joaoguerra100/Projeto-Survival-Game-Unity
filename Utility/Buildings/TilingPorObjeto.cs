using UnityEngine;

// Coloque este script em cada objeto de chão que precisa de um Tiling diferente.
[ExecuteInEditMode] // Isso faz o script rodar no editor, para você ver o resultado na hora!
public class TilingPorObjeto : MonoBehaviour
{
    // Crie uma variável pública para você poder ajustar o Tiling no Inspector da Unity.
    public Vector2 tilingDesejado = new Vector2(1, 1);

    // Variáveis internas para checar se houve mudança
    private Vector2 tilingAnterior;
    private Renderer rend;

    void Start()
    {
        // Pega o componente Renderer do objeto
        rend = GetComponent<Renderer>();
        AplicarTiling();
        tilingAnterior = tilingDesejado;
    }
    
    // Roda sempre que algo for atualizado no Inspector
    void Update()
    {
        // Se estamos no editor e não no jogo rodando
        #if UNITY_EDITOR
        // Checa se o valor do tiling mudou no inspector
        if (tilingDesejado != tilingAnterior)
        {
            AplicarTiling();
            tilingAnterior = tilingDesejado;
        }
        #endif
    }

    void AplicarTiling()
    {
        // Importante: Isso cria uma instância do material para este objeto específico.
        // O material original não é modificado.
        if (rend != null)
        {
            // Pega o material próprio deste objeto (cria uma instância se não tiver)
            Material materialProprio = rend.material;
            
            // Aplica o Tiling
            materialProprio.mainTextureScale = tilingDesejado;
        }
    }
}