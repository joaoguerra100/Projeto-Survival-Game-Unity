using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public GenericItemScriptable baseItemData;  // O ScriptableObject base
    public WeaponItemScriptable weaponItemScriptable;
    public GenericBagScriptable bagOwner;
    public string instanceID;                   // Apenas usado se for arma
    public bool isRotated;
    public int quantidade;
}
