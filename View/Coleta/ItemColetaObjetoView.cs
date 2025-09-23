using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemColetaObjetoView : MonoBehaviour
{
    #region Declaration
    [Header("Referencias")]
    public Transform containerTransform;

    [Header("Loot")]
    [SerializeField] private LootTableScriptable lootTable;
    public List<InventoryItem> itensAtuais = new List<InventoryItem>();

    [Header("Painel")]
    public GameObject itensNoArmazemPainel;
    [SerializeField] public GameObject slotGroup;
    [SerializeField] public GameObject ComplexSlotGroup;
    [SerializeField] private GameObject complexSlotGroupPrefab;
    [SerializeField] private GameObject simpleslotPrefab;

    [Header("Configuração")]
    public int minItemsToSpawn;
    public int maxItemsToSpawn;

    [Header("UI Cirulo")]
    public Image holdImage;
    public Transform circuloTransform;
    public float offsetDistance;        // Distância da bolinha em relação ao container
    public float baseScale = 1f;                 // Tamanho "ideal" da bolinha
    public float scaleMultiplier = 1f;           // Fator para ajustar visualmente o tamanho final

    [Header("UI Botao de Interaçao")]
    public Image btnInteracao;
    public Transform btnInteracaoTransform;
    [SerializeField] private TextMeshProUGUI btnTxt;
    [SerializeField]private float viewportPosx;
    [SerializeField]private float viewportPosy;
    [SerializeField]private float offsetDistanceBtnInteracao;
    [SerializeField] private float baseScaleBtnInteracao;
    [SerializeField] private float scaleMultiplierBtnInteracao;

    [Header("Som")]
    public AudioClip procurando;
    public AudioClip trancado;
    [HideInInspector] public bool isSearching;
    private AudioSource searchAudioSource;

    [Header("Matrix")]
    [SerializeField] private int maxCollum;
    [SerializeField] private int maxRow;
    public MatrixUtility matrix;

    [Header("Boleanas de ajuda")]
    private bool isLoocked;
    private bool visiblePanel;
    [HideInInspector]public bool lootGerado = false;

    #endregion

    #region Metodos iniciais

    void Awake()
    {
        searchAudioSource = GetComponent<AudioSource>();
        matrix = new MatrixUtility(maxRow, maxCollum, "Bag");
    }
    void OnEnable()
    {
        visiblePanel = itensNoArmazemPainel.activeSelf;
    }

    void Update()
    {
        SeguirJogadorCirculo();
        SeguirJogadorBotao();
        AtualizarTexto();
        TocarSom();
    }

    #endregion

    #region Gerar Loot
    public void GerarLoot()
    {
        if (lootGerado) return;
        if (!visiblePanel) return;

        ResetBag();

        itensAtuais.Clear();  // limpa a lista antes de gerar

        var itensDropados = Random.Range(minItemsToSpawn, maxItemsToSpawn + 1);

        for (int i = 0; i < itensDropados; i++)
        {
            InventoryItem item = GetRandomItem();
            if (item != null)
            {
                var freeSpaces = matrix.LookForFreeArea2(item.baseItemData.itemSize.x, item.baseItemData.itemSize.y);
                matrix.SetItem(freeSpaces, item.baseItemData.Id);
                itensAtuais.Add(item);
                BuildCompexSlot(item);
            }
        }

        lootGerado = true;
    }

    InventoryItem GetRandomItem()
    {
        float totalChance = 0f;

        foreach (var item in lootTable.possiveisItens)
        {
            totalChance += item.baseItemData.DropChance;
        }

        float randomValue = Random.Range(0f, totalChance);
        float currentSum = 0f;

        foreach (var item in lootTable.possiveisItens)
        {
            currentSum += item.baseItemData.DropChance;
            if (randomValue <= currentSum)
            {
                return item;
            }
        }

        return null;
    }

    #endregion

    #region UI Circulo

    void SeguirJogadorCirculo()
    {
        if (!circuloTransform.gameObject.activeSelf) return;

        Transform cam = Camera.main.transform;

        // Olha para a câmera (manter sempre visível)
        circuloTransform.LookAt(cam.position);

        // Posição original com offset do container em direção à câmera
        Vector3 directionToCamera = (cam.position - containerTransform.position).normalized;
        Vector3 worldPosWithOffset = containerTransform.position + directionToCamera * offsetDistance;

        // Converte para viewport (0..1)
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(worldPosWithOffset);

        // Força x e y para centro da tela
        viewportPos.x = 0.5f;
        viewportPos.y = 0.5f;

        // Converte de volta para mundo, mantendo a profundidade (z)
        Vector3 finalWorldPos = Camera.main.ViewportToWorldPoint(viewportPos);

        // Posiciona a bolinha
        circuloTransform.position = finalWorldPos;

        // Ajusta a escala para manter tamanho constante na tela
        float distance = Vector3.Distance(cam.position, circuloTransform.position);
        float finalScale = baseScale * (distance * scaleMultiplier);
        circuloTransform.localScale = new Vector3(finalScale, finalScale, finalScale);
    }

    public void AtualizarCirculo(float porcentagem)
    {
        if (circuloTransform != null)
        {
            Image img = circuloTransform.GetComponent<Image>();
            if (img != null)
                img.fillAmount = porcentagem;
        }
    }

    public void MostarCirculo(bool mostrar)
    {
        if (circuloTransform != null)
            holdImage.gameObject.SetActive(mostrar);
    }

    #endregion

    #region UI Botao

    void SeguirJogadorBotao()
    {
        if (!btnInteracaoTransform.gameObject.activeSelf) return;

        Transform cam = Camera.main.transform;

        // Olha para a câmera (manter sempre visível)
        btnInteracaoTransform.rotation = Quaternion.LookRotation(btnInteracaoTransform.position - cam.position);

        // Posição original com offset do container em direção à câmera
        Vector3 directionToCamera = (cam.position - containerTransform.position).normalized;
        Vector3 worldPosWithOffset = containerTransform.position + directionToCamera * offsetDistanceBtnInteracao;

        // Converte para viewport (0..1)
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(worldPosWithOffset);

        // Força x e y para centro da tela
        viewportPos.x = viewportPosx;
        viewportPos.y = viewportPosy;

        // Converte de volta para mundo, mantendo a profundidade (z)
        Vector3 finalWorldPos = Camera.main.ViewportToWorldPoint(viewportPos);

        // Posiciona a bolinha
        btnInteracaoTransform.position = finalWorldPos;

        // Ajusta a escala para manter tamanho constante na tela
        float distance = Vector3.Distance(cam.position, btnInteracaoTransform.position);
        float finalScale = baseScaleBtnInteracao * (distance * scaleMultiplierBtnInteracao);
        btnInteracaoTransform.localScale = new Vector3(finalScale, finalScale, finalScale);
    }

    public void MostarBotaoInteraçao(bool mostrar)
    {
        if (btnInteracao != null)
            btnInteracao.gameObject.SetActive(mostrar);
    }

    void AtualizarTexto()
    {
        btnTxt.text = "Aperte E para interagir Com: " + lootTable.nomeContainer;
    }

    #endregion

    #region Musica

    void TocarSom()
    {
        if (isSearching)
        {
            if (!searchAudioSource.isPlaying)
            {
                searchAudioSource.clip = procurando;
                searchAudioSource.Play();
            }

        }
        else
        {
            if (searchAudioSource.isPlaying)
            {
                searchAudioSource.Stop();
            }
        }
    }
        

    #endregion

    #region GerarSlots
    public void MostrarLootExistente()
    {
        ResetBag();

        foreach (InventoryItem item in itensAtuais)
        {
            BuildCompexSlot(item);
        }
    }

    public void BuildCompexSlot(InventoryItem itemData)
    {
        int i = 0;

        List<Vector2> cellList = FindCellById(itemData.baseItemData.Id);
        if (cellList == null || cellList.Count == 0)
        {
            Debug.LogWarning($"[InventoryView] Nenhuma célula encontrada para o item com ID: {itemData.baseItemData.Id}. Ignorando slot.");
            return;
        }

        // Pega dados do grid
        GridLayoutGroup grid = slotGroup.GetComponent<GridLayoutGroup>();
        Vector2 cellSize = grid.cellSize;
        Vector2 spacing = grid.spacing;
        RectOffset padding = grid.padding;

        // Coordenada inicial
        Vector2Int coord = new Vector2Int((int)cellList[0].y, (int)cellList[0].x);
        float posX = padding.left + (cellSize.x + spacing.x) * coord.x;
        float posY = -(padding.top + (cellSize.y + spacing.y) * coord.y); // y negativo
        Vector3 localPos = new Vector3(posX, posY, 0f);

        // Tamanho do complex slot
        Vector2 factor = Vector2.one;
        Vector2 size = cellSize;

        if (cellList.Count > 1)
        {
            factor = cellList[cellList.Count - 1] - cellList[0];
            size = new Vector2((cellSize.x + spacing.x) * factor.y + cellSize.x, (cellSize.y + spacing.y) * factor.x + cellSize.y);
        }

        // CRIAÇÃO DO SLOT
        GameObject obj = Instantiate(complexSlotGroupPrefab);
        obj.transform.SetParent(ComplexSlotGroup.transform, false);
        var rect = obj.GetComponent<RectTransform>();
        rect.localPosition = localPos;
        rect.sizeDelta = size;
        var complexSlot = obj.GetComponent<ComplexSlotView>();
        complexSlot.ItemView = itemData;
        complexSlot.UpdateIcon();
        complexSlot.UpdateText();

        var slotView = obj.GetComponent<ComplexSlotView>();
        i++;
        slotView.Setup2(itemData, i);

        obj.tag = "ComplexSlotLoot";
        obj.name = itemData.baseItemData.name + "_Clone";

    }

    void BuildSimpleSlot(int maxRow, int maxColumn)
    {
        int r = 0; // linha
        int c = 0; //coluna
        int totalSlots = maxRow * maxColumn;

        for (int i = 0; i < totalSlots; i++)
        {
            GameObject currentSlotGo = Instantiate(simpleslotPrefab);
            currentSlotGo.GetComponent<SimpleSlotView>().coodinate = new Vector2(r, c); // PASSA AS CORDENADAS QUE SERA INSTANCIADO
            currentSlotGo.tag = "SlotSimpleContainer";
            c++;
            if (c == maxColumn) // QUANDO MINHA COLUNA CHEGAR AO MAXIMO VAI ZERAR E ADICIONAR LINHAS
            {
                c = 0;
                r++;
            }

            currentSlotGo.transform.SetParent(slotGroup.gameObject.transform);
        }
        GridLayoutGroup grid = slotGroup.GetComponent<GridLayoutGroup>();
        if (grid != null)
        {
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = maxColumn;
        }
    }

    void RemoveAllComplexSlot() //FUNCAO DE LIMPEZA DOS SLOTS
    {
        foreach (Transform child in ComplexSlotGroup.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void RemoveAllSimpleSlot() //FUNCAO DE LIMPEZA DOS SLOTS
    {
        foreach (Transform child in slotGroup.transform)
        {
            Destroy(child.gameObject);
        }
    }
    #endregion

    public void ResetBag()
    {
        RemoveAllComplexSlot();
        RemoveAllSimpleSlot();
        BuildSimpleSlot(maxRow, maxCollum);
    }

    public void ShowAndHide()
    {
        visiblePanel = !visiblePanel;
        itensNoArmazemPainel.SetActive(visiblePanel);
    }

    public bool RemoverItemDoContainer(string id, GameObject destruir)
    {
        InventoryItem item = FindInventoryItemByStringID(id);
        if (item != null)
        {
            matrix.ClearItemOnMatrix(item.baseItemData.Id);
            if (item.instanceID == id)
            {
                itensAtuais.Remove(item);
            }

            //Debug.Log($"Destruindo objeto físico do item {item.instanceID}");
            StartCoroutine(DestruirEAtualizar(destruir));
            return true;
        }
        return false;
    }

    IEnumerator DestruirEAtualizar(GameObject view)
    {
        yield return new WaitForEndOfFrame();
        Destroy(view.gameObject);
    }

    #region Methods de procura
    List<Vector2> FindCellById(int id)
    {
        return matrix.FindLocationById2(id);
    }

    InventoryItem FindInventoryItemByStringID(string id)
    {
        return itensAtuais.FirstOrDefault(x => x.instanceID == id);
    }
    
    #endregion
}
