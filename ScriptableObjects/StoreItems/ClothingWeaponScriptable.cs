using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewClothingWeapon", menuName = "Inventory/StoreItems/NewClothingWeapon", order = 2)]
public class ClothingWeaponScriptable : ScriptableObject
{

    #region Declaration

    [SerializeField][Range(1, 10)] private int slotNumber; // DEPOIS TROCAR ESTE NUMERO PELO O NUMERO DE SLOTS QUE TERA DE EQUIPAR
    [SerializeField] private List<RulerScriptable> ruleList;
    [SerializeField] private Dictionary<int, InventoryItem> itemsDictionary;
    [SerializeField] private float currentWeigUse;

    #endregion

    #region Getting Setting

    public float CurrentWeigUse { get => currentWeigUse; }
    public Dictionary<int, InventoryItem> ItemsDictionary { get => itemsDictionary; }
    public int SlotNumber { get => slotNumber; }

    #endregion

    #region Methods

    void OnEnable()
    {
        itemsDictionary = new Dictionary<int, InventoryItem>();
        currentWeigUse = 0;
    }

    public InventoryItem GetEquippedItemByType(ItemType tipo)
    {
        foreach (var pair in itemsDictionary)
        {
            if (pair.Value.baseItemData.TipoDeItem == tipo)
            return pair.Value;
        }

        return null;
    }

    public List<string> GetIdsFromItemsClothingWeaponDictionary()
    {
        List<string> resultIds = new List<string>();

        foreach (var item in itemsDictionary)
        {
            resultIds.Add(item.Value.instanceID);
        }

        return resultIds;
    }

    public bool AddItem(int index, InventoryItem item)
    {
        //ITEM EXISTE
        if (itemsDictionary.ContainsValue(item))
        {
            Debug.LogWarning(item.baseItemData.name + "NAO PODER SER ADICIONADO, ELE JA EXISTE");
            return false;
        }

        //NOVO ITEM
        else
        {
            if (CheckAllRules(index, item))
            {
                itemsDictionary.Add(index, item);
                UpdateTotalWeigth();
                return true;
            }
        }

        return false;
    }

    private bool CheckAllRules(int index, InventoryItem item)
    {
        try
        {
            if (ruleList.Count != 0)
            {
                foreach (var rule in ruleList)
                {
                    if (rule.Validate(index, item))
                    {
                        return true;
                    }
                }
            }
        }
        catch (System.Exception)
        {
            Debug.LogWarning("NAO TEM NENHUMA REGRA, ENTAO NENHUM ITEM FOI PERMITIDO ");
        }

        return false;
    }

    /*public GenericItemScriptable GetItemById(int id)
    {
        var resultElement = itemsDictionary.First(element => element.Value.Id == id);
        return resultElement.Value;
    }*/
    public InventoryItem GetItemByStringId(string id)
    {
        foreach (var item in itemsDictionary)
        {
            if (item.Value.instanceID == id)
            return item.Value;
        }
        return null;
    }


    /*public GenericItemScriptable GetItemByIndex(int index)
    {
        var resultElement = itemsDictionary.First(element => element.Key == index);
        return resultElement.Value;
    }*/
    public InventoryItem GetItemByIndex(int index)
    {
        if (itemsDictionary.TryGetValue(index, out InventoryItem result))
        {
            return result;
        }

        return null;
    }

    public bool RemoveItemById(string id)
    {
        try
        {
            var resultElement = itemsDictionary.First(element => element.Value.instanceID == id);
            if (!resultElement.Equals(null))
            {
                itemsDictionary.Remove(resultElement.Key);
                UpdateTotalWeigth();
                return true;
            }
        }
        catch (System.Exception)
        {
            Debug.LogWarning("NAO TEM NENHUM ITEM COM ESTE VALOR");
        }

        return false;
    }

    private void UpdateTotalWeigth()
    {
        currentWeigUse = 0;

        if (itemsDictionary.Count != 0)
        {
            foreach (var item in itemsDictionary)
            {
                currentWeigUse += item.Value.baseItemData.TotalWeigthPerItem;
            }
        }
    }

    #endregion

}
