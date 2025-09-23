using System.Collections.Generic;
using UnityEngine;

public class ItemView : MonoBehaviour
{
    #region Declaration
    [SerializeField]private int number;
    [SerializeField]private GenericItemScriptable item;
    [SerializeField]private WeaponItemScriptable weaponData;
    public InventoryItem inventoryItem;
    [SerializeField] private List<GenericActionScriptable> actionList;
    private ActionManagerEvent actionManagerEvt;
    #endregion

    #region Getting Setting
    public int Number { get => number; set => number = value;}
    public GenericItemScriptable Item { get => item;}
    public WeaponItemScriptable WeaponData { get => weaponData;}
    #endregion

    #region Methods

    void Awake()
    {
        string id = System.Guid.NewGuid().ToString();
        inventoryItem = new InventoryItem
        {
            baseItemData = item,
            instanceID = id,
            quantidade = Number,
            weaponItemScriptable = weaponData
        };
    }

    public void Coletar()
    {
        bool result = InventoryManagerController.instance.AdicionarItemEmAlgumaBag(item, number, false, WeaponData);

        if (result)
        {
            actionManagerEvt = new ActionManagerEvent();
            actionManagerEvt.DispatchAllGenericActionListEvent(actionList);
            Destroy(this.gameObject);
        }
    }

    #endregion
}
