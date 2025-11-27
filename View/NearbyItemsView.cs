using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NearbyItemsView : MonoBehaviour
{
    #region Declaration
    [Header("Scripts")]
    public static NearbyItemsView intance;

    [Header("Loot")]
    [SerializeField] private float raioDeLoot;
    [SerializeField] private LayerMask lootLayerMask;
    [SerializeField] private List<InventoryItem> itensDropados;

    [Header("Painel")]
    [SerializeField] private GameObject coletarPainel;
    [SerializeField] public GameObject slotGroup;
    [SerializeField] public GameObject ComplexSlotGroup;
    [SerializeField] private GameObject complexSlotGroupPrefab;

    [Header("Matrix")]
    [SerializeField] private int maxCollum;
    [SerializeField] private int maxRow;
    public MatrixUtility matrix;

    [Header("Boleanas de ajuda")]
    public bool ligarGzimos;
    public bool visiblePanel;

    #endregion

    void Awake()
    {
        intance = this;
        coletarPainel.SetActive(true);
    }

    void Start()
    {
        itensDropados = new List<InventoryItem>();
    }

    void OnEnable()
    {
        visiblePanel = coletarPainel.activeSelf;
        ResetBag();
    }

    void OnDisable()
    {
        ResetBag();
    }

    void ResetBag()
    {
        matrix = new MatrixUtility(maxCollum, maxRow, "Bag");
        itensDropados.Clear();
        RemoveAllComplexSlot();
    }

    public void AtualizarItensPertos()
    {
        if (!visiblePanel) return;
        ResetBag();

        Collider[] hits = Physics.OverlapSphere(Player.instance.transform.position, raioDeLoot, lootLayerMask);

        HashSet<string> processados = new HashSet<string>();

        foreach (var hit in hits)
        {
            var lootItem = hit.GetComponentInParent<ItemView>();
            if (lootItem != null && lootItem.inventoryItem != null && lootItem.Item != null)
            {
                if (processados.Contains(lootItem.inventoryItem.instanceID)) continue;

                var freeSpaces = matrix.LookForFreeArea2(lootItem.Item.itemSize.x, lootItem.Item.itemSize.y);
                if (freeSpaces != null && freeSpaces.Count > 0)
                {
                    matrix.SetItem(freeSpaces, lootItem.Item.Id);
                    itensDropados.Add(lootItem.inventoryItem);
                    processados.Add(lootItem.inventoryItem.instanceID);
                }
            }
            else
            {
                Debug.LogWarning($"ItemView nulo ou não inicializado em {hit.name}");
            }
        }
        BuildCompexSlot(itensDropados);
    }

    void UpdateAllItems()
    {
        RemoveAllComplexSlot();
        BuildCompexSlot(itensDropados);
    }

    void RemoveAllComplexSlot()
    {
        foreach (Transform child in ComplexSlotGroup.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void BuildCompexSlot(List<InventoryItem> items)
    {
        int i = 0;

        foreach (var itemData in items)
        {
            List<Vector2> cellList = FindCellById(itemData.baseItemData.Id);
            if (cellList == null || cellList.Count == 0)
            {
                Debug.LogWarning($"[InventoryView] Nenhuma célula encontrada para o item com ID: {itemData.baseItemData.Id}. Ignorando slot.");
                continue;
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
            obj.SetActive(true);
            obj.transform.SetParent(ComplexSlotGroup.transform, false);
            obj.transform.localScale = Vector3.one;

            var rect = obj.GetComponent<RectTransform>();
            rect.localPosition = localPos;
            rect.sizeDelta = size;

            var complexSlot = obj.GetComponent<ComplexSlotView>();
            complexSlot.Setup(itemData, i, null);
            complexSlot.ItemView = itemData;
            complexSlot.UpdateIcon();
            complexSlot.UpdateText();

            i++;

            /* obj.tag = "ComplexSlotLoot"; */
            obj.name = itemData.baseItemData.name + "_LootUI";
        }
    }

    public bool RemoverItemLooteado(string idUnico)
    {
        InventoryItem item = FindInventoryItemByStringID(idUnico);
        if (item != null)
        {
            //Debug.Log($"Removendo item com ID: {idUnico}");
            itensDropados.Remove(item);
            matrix.ClearItemOnMatrix(item.baseItemData.Id);

            var itemViews = FindObjectsByType<ItemView>(FindObjectsSortMode.None);
            foreach (var view in itemViews)
            {
                if (view.inventoryItem.instanceID == idUnico)
                {
                    Destroy(view.gameObject);
                    break;
                }
            }
            UpdateAllItems();
            return true;
        }
        return false;
    }

    /* IEnumerator DestruirEAtualizar(ItemView view)
    {
        yield return new WaitForEndOfFrame();
        Destroy(view.gameObject);
        yield return null;
        UpdateAllItems();
    } */

    #region Methods de procura
    List<Vector2> FindCellById(int id)
    {
        return matrix.FindLocationById2(id);
    }

    InventoryItem FindInventoryItemByStringID(string id)
    {
        return itensDropados.FirstOrDefault(x => x.instanceID == id);
    }

    #endregion

    public void ShowAndHide()
    {
        visiblePanel = !visiblePanel;
        if (coletarPainel != null) coletarPainel.SetActive(visiblePanel);

        if (visiblePanel)
        {
            AtualizarItensPertos();
        }
    }

    void OnDrawGizmos()
    {
        if (ligarGzimos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(Player.instance.transform.position, raioDeLoot);
        }
    }

}
