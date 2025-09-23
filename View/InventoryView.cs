using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : MonoBehaviour
{

    #region Declaration

    [Header("Declaration")]
    public GenericBagScriptable[] currentBag;
    private bool visiblePanel;

    private string s = "/";
    private string w = "Kg";

    [SerializeField] private GameObject inventoryGo;
    [SerializeField] public GameObject[] slotGroup;
    [SerializeField] private GameObject[] slotGo;
    [SerializeField] private GameObject[] complexSlotGroup;
    public GameObject[] complexSlotGo;
    private int currentBagIndex;

    #endregion

    #region TitlePanel

    [Header("Mochila")]
    [SerializeField] private Image[] bagIcon;
    [SerializeField] private TextMeshProUGUI[] bagSlot;
    [SerializeField] private TextMeshProUGUI bagWeigth;

    [Header("Peso do item")]
    [SerializeField] private TextMeshProUGUI itemCurrentWeight;
    [SerializeField] private TextMeshProUGUI itemMaxWeight;

    #endregion

    #region DescriptionPanel

    [Header("Descriçao painel")]
    [SerializeField] private TextMeshProUGUI itemTex;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    #endregion

    #region DetailPanel

    [Header("Detlhe painel")]
    [SerializeField] private Image itemIconDeitalPanel;
    [SerializeField] private TextMeshProUGUI currentNumberText;
    [SerializeField] private TextMeshProUGUI maxNumberText;
    [SerializeField] private TextMeshProUGUI totalWeightPerItemNumberText;

    #endregion

    #region Designer and Slot Control

    [SerializeField] private List<GameObject> simpleSlotList;
    public static InventoryView instance;

    #endregion

    #region Getting Setting

    public bool VisiblePanel { get => visiblePanel; set => visiblePanel = value;}
    public int CurrentBagIndex => currentBagIndex;

    #endregion

    #region Methods

    void OnEnable()
    {
        visiblePanel = inventoryGo.activeSelf;
    }

    void Awake()
    {

        instance = this;
        inventoryGo.SetActive(true);
    }

    public void Iniciate(GenericBagScriptable bag, int bagIndex)
    {
        currentBag[bagIndex] = bag;
        currentBagIndex = bagIndex;
        SlotAndGridUpdate(currentBag[bagIndex].MaxRow, currentBag[bagIndex].MaxColum, bagIndex);
        BagWeigthAndSlotUpdate();
    }

    private void SlotAndGridUpdate(int maxRow, int maxColumn, int bagIndex)
    {
        int r = 0; // linha
        int c = 0; //coluna
        int totalSlots = maxRow * maxColumn;

        for (int i = 0; i < totalSlots; i++)
        {
            GameObject currentSlotGo = Instantiate(slotGo[bagIndex]);
            currentSlotGo.GetComponent<SimpleSlotView>().coodinate = new Vector2(r, c); // PASSA AS CORDENADAS QUE SERA INSTANCIADO
            currentSlotGo.tag = "SlotSimple";
            c++;
            if (c == maxColumn) // QUANDO MINHA COLUNA CHEGAR AO MAXIMO VAI ZERAR E ADICIONAR LINHAS
            {
                c = 0;
                r++;
            }

            currentSlotGo.transform.SetParent(slotGroup[bagIndex].gameObject.transform);
        }
        GridLayoutGroup grid = slotGroup[bagIndex].GetComponent<GridLayoutGroup>();
        if (grid != null)
        {
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = maxColumn;
        }
    }
    /* Segunda forma de ajustar, porem nao e usado o gridlayoutGroup
    private void SlotAndGridUpdate(int maxRow, int maxColumn, int bagIndex)
    {
        float spacing = 5f; // ajuste o espaçamento como desejar
        float size = cellSize[bagIndex];

        int r = 0;
        int c = 0;

        for (int i = 0; i < maxRow * maxColumn; i++)
        {
            GameObject currentSlotGo = Instantiate(slotGo[bagIndex]);
            currentSlotGo.tag = "SlotSimple";
            currentSlotGo.GetComponent<SimpleSlotView>().coodinate = new Vector2(r, c);

            RectTransform rt = currentSlotGo.GetComponent<RectTransform>();
            rt.SetParent(slotGroup[bagIndex].transform, false);
            rt.localPosition = new Vector3(c * (size + spacing), -r * (size + spacing), 0);

            c++;
            if (c == maxColumn)
            {
                c = 0;
                r++;
            }
        }
    }*/

    void RemoveAllSimpleSlot() //FUNCAO DE LIMPEZA DOS SLOTS
    {
        GameObject[] resultSimpleSlotGo = GameObject.FindGameObjectsWithTag("SlotSimple");

        foreach (var item in resultSimpleSlotGo)
        {
            Destroy(item);
        }
    }

    public void BagWeigthAndSlotUpdate()
    {
        float somaPesoMaximo = 0;
        foreach (GenericBagScriptable bag in currentBag)
        {
            if (bag != null) // checa se não é nulo
                somaPesoMaximo += bag.CurrentWeigthUse;
        }

        bagWeigth.text = somaPesoMaximo.ToString() + w;
        //float maxWeigth = currentBag[currentBagIndex].WeigthLimited;
        //string weigth  = currentBag[currentBagIndex].CurrentWeigthUse.ToString() + s + maxWeigth.ToString() + w;
        

        int maxSlot = currentBag[CurrentBagIndex].SlotLimited;
        bagIcon[CurrentBagIndex].sprite = currentBag[CurrentBagIndex].Icon;
        string slot = currentBag[CurrentBagIndex].Title + "(" + currentBag[CurrentBagIndex].CurrentSlotUse.ToString() + s + maxSlot.ToString() + ")";
        bagSlot[CurrentBagIndex].text = slot;
    }
    
    public void UpdateDiscriptionAndDetailPanel(InventoryItem item)
    {
        if(visiblePanel)
        {
            //Description panel
            itemTex.text = "Item:" + item.baseItemData.name;
            typeText.text = "Type: " + item.baseItemData.GetItemType();
            descriptionText.text = item.baseItemData.Description;

            //DetalPanel

            itemIconDeitalPanel.sprite = item.baseItemData.IconHorizontal;

            currentNumberText.text = item.baseItemData.CurrentNumber.ToString();
            maxNumberText.text = s + item.baseItemData.LimitedNumber.ToString();
            totalWeightPerItemNumberText.text = item.baseItemData.TotalWeigthPerItem.ToString() + w;
        }
    }

    public void UpdateAllItems(List<ItemComBag> itemComBagList)
    {
        if (!visiblePanel) return;

        RemoveAllComplexSlot();
        RemoveAllSimpleSlot();

        for (int i = 0; i < itemComBagList.Count; i++)
        {
            BuildCompexSlot(itemComBagList[i], i);
        }

        BagWeigthAndSlotUpdate();
    }

    public void BuildCompexSlot(ItemComBag itemComBag, int inventoryIndex)
    {
        InventoryItem itemData = itemComBag.item;
        GenericBagScriptable bag = itemComBag.bag;
        int bagIndex = InventoryManagerController.instance.GetBagIndex(bag);
        List<Vector2> cellList = bag.FindCellById(itemData.baseItemData.Id);
        if (cellList == null || cellList.Count == 0)
        {
            Debug.LogWarning($"[InventoryView] Nenhuma célula encontrada para o item com ID: {itemData.baseItemData.Id}. Ignorando slot.");
            return;
        }

        // Pega dados do grid
        GridLayoutGroup grid = slotGroup[bagIndex].GetComponent<GridLayoutGroup>();
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
            size = new Vector2((cellSize.x + spacing.x) * factor.y + cellSize.x,(cellSize.y + spacing.y) * factor.x + cellSize.y);

            //if (size.x == 0) size = new Vector2(cellSize[bagIndex], size.y);
            //if (size.y == 0) size = new Vector2(size.x, cellSize[bagIndex]);
        }

        // CRIAÇÃO DO SLOT
        GameObject obj = Instantiate(complexSlotGo[bagIndex]);
        obj.transform.SetParent(complexSlotGroup[bagIndex].transform, false);
        obj.GetComponent<RectTransform>().localPosition = localPos;
        obj.GetComponent<RectTransform>().sizeDelta = size;
        obj.GetComponent<ComplexSlotView>().ItemView = itemData;
        obj.GetComponent<ComplexSlotView>().UpdateIcon();
        obj.GetComponent<ComplexSlotView>().UpdateText();

        var slotView = obj.GetComponent<ComplexSlotView>();
        slotView.Setup(itemData, inventoryIndex,currentBag[bagIndex]);

        obj.tag = "ComplexSlot";
        obj.name = itemData.baseItemData.name + "_Clone";
    }

    void RemoveAllComplexSlot() //FUNCAO DE LIMPEZA DOS SLOTS
    {
        GameObject[] resultComplexSlotGo = GameObject.FindGameObjectsWithTag("ComplexSlot");

        foreach (var item in resultComplexSlotGo)
        {
            Destroy(item);
        }
    }

    public void AtualizarReferenciasDasBags(List<GenericBagScriptable> bags)
    {
        for (int i = 0; i < bags.Count; i++)
        {
            var slotParent = slotGroup[i].transform.parent;
            var complexParent = complexSlotGroup[i].transform.parent;

            var slotIdentifier = slotParent.GetComponent<ComplexSlotGroupIdentifier>();
            var complexIdentifier = complexParent.GetComponent<ComplexSlotGroupIdentifier>();

            if (slotIdentifier != null)
                slotIdentifier.bag = bags[i];
            else
                Debug.LogWarning($"[InventoryView] slotGroup[{i}] não possui ComplexSlotGroupIdentifier no parent.");

            if (complexIdentifier != null)
                complexIdentifier.bag = bags[i];
            else
                Debug.LogWarning($"[InventoryView] complexSlotGroup[{i}] não possui ComplexSlotGroupIdentifier no parent.");
        }
    }

    public void ShowAndHide()
    {
        visiblePanel = !visiblePanel;
        inventoryGo.SetActive(visiblePanel);
    }

    #endregion
}
