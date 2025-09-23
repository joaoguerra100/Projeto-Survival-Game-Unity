using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "NewBag", menuName = "Inventory/StoreItems/NewBag")]
public class BagScriptable : GenericBagScriptable
{

    #region Declaration

    [SerializeField][Range(0, 10)] protected int maxShortCutSlot;

    [SerializeField] private List<RulerScriptable> rulerList;
    [SerializeField] protected Dictionary<int, InventoryItem> itemShortCutDictionary;



    #endregion

    #region Getting Setting

    public int MaxShortCutSlot { get => maxShortCutSlot; }
    public Dictionary<int, InventoryItem> ItemShortCutDictionary { get => itemShortCutDictionary; }

    #endregion

    #region Methods

    protected override void OnEnable()
    {
        itemShortCutDictionary = new Dictionary<int, InventoryItem>();
        base.OnEnable();
    }

    public override void ResetBag()
    {
        base.ResetBag();
        matrix = new MatrixUtility(maxRow, maxColum, "Bag");
    }

    public override bool UseItem(int id, int value)
    {
        return base.UseItem(id, value);
    }

    public bool AddItemToShortCut(int index, InventoryItem item)
    {
        // Se o item já está no atalho (por valor), apenas alerta
        if (itemShortCutDictionary.ContainsValue(item))
        {
            Debug.LogWarning($"[SHORTCUT] O item '{item.baseItemData.name}' já está em um atalho.");
            return false;
        }

        // Verifica regras de negócio para esse índice
        if (!CheckAllRules(index, item))
        {
            Debug.LogWarning($"[SHORTCUT] Item '{item.baseItemData.name}' não passou nas regras de adição.");
            return false;
        }

        // Se já existe um item nesse índice, substitui
        if (itemShortCutDictionary.ContainsKey(index))
        {
            Debug.Log($"[SHORTCUT] Substituindo item do atalho {index}.");
            itemShortCutDictionary[index] = item; // sobrescreve
        }
        else
        {
            itemShortCutDictionary.Add(index, item); // adiciona novo
        }

        return true;
    }

    public void ClearShortcutDictionary()
    {
        if (itemShortCutDictionary != null)
            itemShortCutDictionary.Clear();
    }

    public bool ChangeItemShortCutPosition(InventoryItem item, int index)
    {
        if (itemShortCutDictionary.ContainsValue(item))
        {

            RemoveItemFromShortCutById(item.instanceID);
            AddItemToShortCut(index, item);
            return true;
        }

        return false;
    }

    public List<string> GetIdsFromItemsShortCutDictionary() // Talvez mudar futuramente(com o id unico gerado)
    {
        List<string> resultIds = new List<string>();

        foreach (var item in itemShortCutDictionary)
        {
            resultIds.Add(item.Value.instanceID);
        }

        return resultIds;
    }

    public List<int> GetUsedKeysFromShortCutDictionary()
    {
        List<int> resultKeys = new List<int>();

        foreach (var item in itemShortCutDictionary)
        {
            resultKeys.Add(item.Key);
        }

        return resultKeys;
    }

    public InventoryItem GetItemByIndexPosition(int index)
    {
        try
        {
            var result = itemShortCutDictionary.First(element => element.Key == index);

            if (!result.Value.Equals(null))
            {
                return result.Value;
            }
        }
        catch (System.Exception)
        {
            Debug.LogWarning("NAO TEM NENHUM ITEM COM ESTA CHAVE");
        }

        return null;
    }

    public int GetIndexByItem(InventoryItem item)
    {
        int resultIndex = -1;
        try
        {
            var result = itemShortCutDictionary.First(element => element.Value == item);
            if (!result.Value.Equals(null))
            {
                resultIndex = result.Key;
                return resultIndex;
            }
        }
        catch (System.Exception)
        {
            Debug.LogWarning("NAO TEM NENHUM ITEM COM ESTE VALOR");
        }

        return resultIndex;
    }

    public bool RemoveItemFromShortCutById(string id)
    {
        try
        {
            var resultItem = itemShortCutDictionary.First(element => element.Value.instanceID == id);

            if (!resultItem.Equals(null))
            {
                itemShortCutDictionary.Remove(resultItem.Key);
                return true;
            }

        }
        catch (System.Exception)
        {
            Debug.LogWarning("Nao nem tenhum item com este Id");
        }

        return false;
    }


    private bool CheckAllRules(int index, InventoryItem item)
    {
        if (rulerList.Count != 0)
        {
            foreach (var ruler in rulerList)
            {
                if (ruler.Validate(index, item))
                {
                    return true;
                }
            }
        }
        else
        {
            Debug.LogWarning("NAO TEM NENHUMA REGRA, OU ITEM QUE SEJA PERMITIDO");
        }

        return false;
    }

    #endregion
}

