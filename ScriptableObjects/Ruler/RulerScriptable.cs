using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRuler", menuName = "Inventory/Ruler/NewRuler")]
public class RulerScriptable : ScriptableObject
{

    #region Declaration

    [SerializeField] private int idSelected;
    [SerializeField] private bool IgnoreAllSlotFilter;
    [SerializeField] private bool useIdAndIgnoreAllItemTypeFilter;
    [SerializeField] private SlotFilterScriptable slotFilter;
    [SerializeField] private ItemTypeFilterScriptable itemTypeFilter;

    #endregion

    #region Methods

    public bool Validate(int index, InventoryItem item)
    {
        try
        {
            List<int> resultIdenxList = slotFilter.GetAllIndex();
            List<ItemType> resultItemTypeList = itemTypeFilter.ItemType;

            if (resultItemTypeList.Count == 0)
            {
                Debug.LogWarning("THERE IS NO ItemType IN ItemTypeFilter");
            }

            if (resultIdenxList.Contains(index) || IgnoreAllSlotFilter)
            {
                if (!useIdAndIgnoreAllItemTypeFilter)
                {
                    foreach (var itemTypeInList in resultItemTypeList)
                    {
                        if (itemTypeInList == item.baseItemData.GetItemType())
                        {
                            return true;
                        }
                    }
                }

                //IGNORE ALL ITEM TYPE
                else
                {
                    if (idSelected == item.baseItemData.Id)
                    {
                        return true;
                    }
                }
            }
        }
        catch (System.Exception)
        {
            Debug.LogWarning("There is no Filters in SlotFilter / ItemTypeFilter or both of them");
        }

        return false;
    }

    
    #endregion
}
