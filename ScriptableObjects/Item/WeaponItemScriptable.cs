using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponItem", menuName = "Inventory/items/NewWeaponItem")]
public class WeaponItemScriptable : GenericItemScriptable
{
    [SerializeField] private List<GenericActionScriptable> actionEquipList;
    public ItemEquip tipoEquip;
    public string nome => Label;

    [Header("Tipo De Arma")]
    public ModoDeTiro modoDeTiro;
    public bool podeTrocarModoTiro;
    public bool sniper;
    public TipoDeRecarga tipoDeRecarga;
    public TipoDeMunicao tipoDeMunicao => TipoDeMunicao;

    [Header("Configuraçao Dos Disparos")]
    public float fireRateSingle;
    public float fireRateAutomatic;
    public float bulletRange;
    public int danoMunicao;

    [Header("Muniçao")]
    public int municaoMaxima;
    public int municaoParaRecarregar;

    [Header("Tempo Para Carregar Bala Por Bala")]
    public float tempoCarregarInicio;
    public float tempoCarregarLoop;
    public float tempoCarregarFim;
    



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

public enum TipoDeRecarga
{
    Cartucho, // Recarrega tudo de uma vez
    BalaPorBala // Recarrega uma por uma
}
