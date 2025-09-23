using System.Collections.Generic;
using System.IO.Enumeration;
using UnityEngine;

[CreateAssetMenu(fileName = "NewClothingItem", menuName = "Inventory/items/NewClothingItem")]
public class ClothingItemScriptable : GenericItemScriptable //Referente no video ao ArmorItemScriptable
{
    [SerializeField]private List<GenericActionScriptable> actionEquipList;

    public override void ActionEquipAndUnequipListDispatch()
    {
        actionManagerEvt = new ActionManagerEvent();
        actionManagerEvt.DispatchAllGenericActionListEvent(actionEquipList);
    }

    public override ItemType GetItemType()
    {
        return TipoDeItem;
    }
}
