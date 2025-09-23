using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShortCutView : MonoBehaviour
{
    #region  Declaration

    [SerializeField] private GameObject shortCutGo;
    [SerializeField] private GameObject slotSpecialGroup;
    [SerializeField] private GameObject specialSlotGo;

    private Dictionary<int, GameObject> specialSlotDictionary;

    public static ShortCutView instance;

    #endregion

    #region Methods

    void Awake()
    {
        instance = this;
    }

    public void IniciateShortCutSlots(int maxShortSlot, int startIndex)
    {
        for (int column = 0; column < maxShortSlot; column++)
        {
            GameObject resultGo = Instantiate(specialSlotGo);
            SimpleSlotView[] resultSimpleSlotViewList = resultGo.GetComponentsInChildren<SimpleSlotView>();

            if (!(resultSimpleSlotViewList[0].Equals(null)))
            {
                resultSimpleSlotViewList[0].coodinate = new Vector2(0, startIndex + column);
            }
            resultGo.transform.SetParent(slotSpecialGroup.gameObject.transform);
        }

        specialSlotDictionary = new Dictionary<int, GameObject>();
        GameObject[] resultList = GameObject.FindGameObjectsWithTag("SpecialSlot");
        int index = 0;

        foreach (var specialSlotGo in resultList)
        {
            specialSlotDictionary.Add(index, specialSlotGo);
            index++;
        }
    }

    public void RefreshSlotSystem(Dictionary<int, InventoryItem> itemsShortCutDictionary)
    {
        foreach (var item in specialSlotDictionary)
        {
            item.Value.GetComponent<DisplayItemBehaviorView>().TurnOff();
        }

        if (!(itemsShortCutDictionary.Equals(null)))
        {
            foreach (var item in itemsShortCutDictionary)
            {
                GameObject resultGo = SelectSpecialSlotByIndex(item.Key);
                resultGo.GetComponent<DisplayItemBehaviorView>().TurnOn();
                resultGo.GetComponentInChildren<ComplexSlotView>().ItemView = item.Value;
                resultGo.GetComponentInChildren<ComplexSlotView>().UpdateText();
                resultGo.GetComponentInChildren<ComplexSlotView>().UpdateIcon();
            }
        }
    }

    public void UpdateSlot(Dictionary<int, InventoryItem> itemShortCutDictionary, List<int> usedKeys)
    {
        foreach (var key in usedKeys)
        {
            //PROCURANDO UM SLOT POR UMA CHAVE QUE ESTA SENDO USADA
            var resultSpecialSlotDictionary = specialSlotDictionary.First(element => element.Key == key);
            resultSpecialSlotDictionary.Value.GetComponent<DisplayItemBehaviorView>().TurnOn();

            //USANDO ESTA MESMA CHAVE PARA ENCONTAR O ITEM
            var resultItemShortCutDictionary = itemShortCutDictionary.First(element => element.Key == key);
            InventoryItem resultItem = resultItemShortCutDictionary.Value;

            resultSpecialSlotDictionary.Value.GetComponentInChildren<ComplexSlotView>().ItemView = resultItem;
            resultSpecialSlotDictionary.Value.GetComponentInChildren<ComplexSlotView>().UpdateText();
            resultSpecialSlotDictionary.Value.GetComponentInChildren<ComplexSlotView>().UpdateIcon();
        }
    }

    private GameObject SelectSpecialSlotByIndex(int id)
    {
        try
        {
            var result = specialSlotDictionary.First(element => element.Key == id);
            if (!(result.Equals(null)))
            {
                return result.Value;
            }
        }
        catch (System.Exception)
        {

            Debug.LogWarning("NAO TEM SLOT COM ESTE VALOR");
        }

        return null;
    }

    #endregion
}
