using UnityEngine;

[CreateAssetMenu(fileName = "NewConsumableItem", menuName = "Inventory/items/NewConsumableItem")]
public class ConsumableItemScriptable : GenericItemScriptable
{
    public override ItemType GetItemType()
    {
        return TipoDeItem;
    }
}
