using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Database/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public List<GenericItemScriptable> allItems;
    public List<WeaponItemScriptable> armasDeFogo;

    public WeaponItemScriptable GetWeaponByID(int id)
    {
        return armasDeFogo.Find(item => item.Id == id);
    }

     public GenericItemScriptable GetItemByID(int id)
    {
        return allItems.Find(item => item.Id == id);
    }
}
