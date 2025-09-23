using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericItemScriptable : ScriptableObject
{
    #region Declaration

    [SerializeField]private int id;
    [HideInInspector]public bool Equipado = false;
    [SerializeField] private TipoDeMunicao tipoDeMunicao;
    [SerializeField] private ItemType tipoDeItem;
    [SerializeField]private Sprite iconHorizontal; // o sprite "deitado"
    [SerializeField]private Sprite iconVertical;   // o sprite "em pé"
    [SerializeField] private bool isDroppable;
    [SerializeField]private bool removeWhenNumberIsZero;
    [SerializeField]private bool isOnlyItem; // o item e unico, so pode ter um no inventario
    [SerializeField]private string label;  // ROTULO/NOME DO ITEM
    [SerializeField][TextArea]private string description; // TEXTO DE DESCRIÇAO DO ITEM
    [SerializeField]private int currentNumber; // QUANTIDADE ATUAL DE ITEMS
    [SerializeField]private int limitedNumber; //LIMITE DE UM TIPO DE ITEM NO INVENTARIO
    [SerializeField]private float weigthPerItem; //QUANTIDADE DE PESO POR ITEM
    [SerializeField]private float totalWeigthPerItem; // QUANTIDADE TOTAL DE TODO AQUELE STACK DO ITEM
    public Vector2Int itemSize;
    [SerializeField] private float dropChance;

    

    [SerializeField] private List<GenericActionScriptable> actionUseList;
    protected ActionManagerEvent actionManagerEvt;

    #endregion

    #region Getting Setting
    public int Id { get => id;}
    public ItemType TipoDeItem { get => tipoDeItem;}
    public abstract ItemType GetItemType();
    public Sprite IconHorizontal { get => iconHorizontal;}
    public Sprite IconVertical { get => iconVertical;}
    public bool IsDroppable { get => isDroppable; }
    public bool RemoveWhenNumberIsZero { get => removeWhenNumberIsZero;}
    public bool IsOnlyItem { get => isOnlyItem;}
    public string Label { get => label;}
    public string Description { get => description;}
    public int CurrentNumber { get => currentNumber;}
    public int LimitedNumber { get => limitedNumber;}
    public float WeigthPerItem { get => weigthPerItem;}
    public TipoDeMunicao TipoDeMunicao { get => tipoDeMunicao;}
    public float DropChance { get => dropChance;}
    public float TotalWeigthPerItem
    {
        get
        {
            UpdateWeigth();
            return totalWeigthPerItem;
        }
    }

    





    #endregion

    #region Methods

    private void OnEnable()
    {
        Reset();
        UpdateWeigth();
        Equipado = false;
    }

    public void Reset()
    {
        currentNumber = 0;
    }

    public bool Add(int value) // ADICIONA ITENS AO INVENTARIO
    {
        if(isOnlyItem) // SE FOR ITEM UNICO
        {
            currentNumber = 1;
            return true;
        }
        else // CASO NAO FOR ITEM UNICO
        {
            if(value + currentNumber <= limitedNumber) //SE O VALOR ADICIONADO MAIS O VALOR QUE EU JA POSSUO E MENOR OU IGUAL AO NUMERO LIMITE DE ITENS ELE ADICIONA O ITEM
            {
                currentNumber += value;
                UpdateWeigth();
                return true;
            }
        }
        return false;
    }

    private bool Subtract(int value) // REMOVE ITENS DO INVENTARIO
    {
        if(currentNumber - value >= 0) //SE O VALOR QUE ESTOU TIRANDO DO MEU INVENTARIO E MAIOR OU IGUAL A 0 PARA N TER ITEM NEGATIVO
        {
            currentNumber -= value;
            UpdateWeigth();
            return true;
        }
        return false;
    }

    public virtual bool Use(int value) // PODE SER ALTERADO EM OUTRO SCRIPT/ ELE USA O ITEM
    {
        if(isOnlyItem)
        {
            ActionUseListDispatch();
            return true;
        }
        else
        {
            if(Subtract(value)) // SUBTRAI UM ITEM PELO O VALOR PASSADO E CASO FOR VERDADEIRO USA O ITEM
            {
                ActionUseListDispatch();
                return true;
            }
        }
        return false;
    }

    public virtual bool UseMunicao(int value) // PODE SER ALTERADO EM OUTRO SCRIPT/ ELE USA O ITEM
    {
        if(Subtract(value)) // SUBTRAI UM ITEM PELO O VALOR PASSADO E CASO FOR VERDADEIRO USA O ITEM
        {

            return true;
        }
        return false;
    }

    public virtual void ActionUseListDispatch()
    {
        actionManagerEvt = new ActionManagerEvent();
        actionManagerEvt.DispatchAllGenericActionListEvent(actionUseList);
    }

    public virtual void ActionEquipAndUnequipListDispatch()
    {

    }

    private void UpdateWeigth()
    {
        totalWeigthPerItem = weigthPerItem * currentNumber;
    }

    #endregion
}
