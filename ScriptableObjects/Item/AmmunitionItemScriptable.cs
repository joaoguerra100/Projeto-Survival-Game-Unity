using UnityEngine;

[CreateAssetMenu(fileName = "NewAmmunitionItem", menuName = "Inventory/items/NewAmmunitionItem")]
public class AmmunitionItemScriptable : GenericItemScriptable
{
    public override ItemType GetItemType()
    {
        return TipoDeItem;
    }

}