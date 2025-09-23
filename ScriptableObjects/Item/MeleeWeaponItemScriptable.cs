using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMeleeWeaponItem", menuName = "Inventory/items/NewMeleeWeaponItem")]
public class MeleeWeaponItemScriptable : GenericItemScriptable
{
    [SerializeField] private List<GenericActionScriptable> actionEquipList;
    public ItemEquip tipoEquip;
    public string nome => Label;

    public float danoArma;
    public float custoEstaminaAtackFraco;
    public float custoEstaminaAtackForte;

    public override void ActionEquipAndUnequipListDispatch()
    {
        actionManagerEvt = new ActionManagerEvent();
        actionManagerEvt.DispatchAllGenericActionListEvent(actionEquipList);
    }

    public override ItemType GetItemType()
    {
        return TipoDeItem;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
