using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemTypeFilter", menuName = "Inventory/Filters/NewItemFilter", order = 2)]
public class ItemTypeFilterScriptable : ScriptableObject
{
    #region  Declaration

    [SerializeField] private List<ItemType> itemType;



    #endregion

    #region Getting Setting

    public List<ItemType> ItemType { get => itemType;}
    
    #endregion

}
