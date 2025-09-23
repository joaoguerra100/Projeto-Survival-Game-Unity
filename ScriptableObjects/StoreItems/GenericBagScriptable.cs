using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public abstract class GenericBagScriptable : ScriptableObject
{
    #region  Declaration

    private bool usedOrganizeBtSizePryority = false;
    [SerializeField] private Sprite icon;
    [SerializeField] private string title;
    [SerializeField] protected int maxColum; // QUANTIDADE MAXIMA DE COLUNAS
    [SerializeField] protected int maxRow; // QUANTIDADE MAXIMA DE LINHAS
    [SerializeField] protected int currentSlotUse; // QUANTIDADE DE SLOT ESTA SENDO USADO
    [SerializeField][Range(0.1f, 50f)] protected float weigthLimited; // QUANTIDADE MAXIMA DE PESO CARREGADO
    [SerializeField] private float currentWeigthUse; //QUANTIDADE DE PESO QUE ESTA NA BAG
    [SerializeField] protected bool autoOrganze; //BOTAO DE AUTO ORGANIZAR O INVENTARIO
    public List<FireWeaponInstance> weaponList;
    public List<InventoryItem> inventoryItemsList;
    public MatrixUtility matrix;

    #endregion

    #region  Getting Setting

    public Sprite Icon { get => icon; }
    public string Title { get => title; }
    public int MaxRow { get => maxRow; }
    public int MaxColum { get => maxColum; }
    public int SlotLimited { get => maxRow * maxColum; }
    public int CurrentSlotUse { get => currentSlotUse; }
    public float WeigthLimited { get => weigthLimited; }
    public float CurrentWeigthUse { get => currentWeigthUse; }
    public bool UsedOrganizeBtSizePryority { get => usedOrganizeBtSizePryority; set => usedOrganizeBtSizePryority = value; }


    #endregion

    #region Methods

    protected virtual void OnEnable()
    {
        ResetBag();
    }

    public virtual void ResetBag()
    {
        weaponList = new List<FireWeaponInstance>();
        inventoryItemsList = new List<InventoryItem>();
        currentSlotUse = 0;
        currentWeigthUse = 0;
    }

    protected virtual bool SlotCapacityValidation(GenericItemScriptable item)
    {
        if (item.itemSize.x *item.itemSize.y == 2 || item.itemSize.x *item.itemSize.y == 3 || item.itemSize.x *item.itemSize.y == 5)
        {

            if (item.itemSize.x *item.itemSize.y > maxColum && item.itemSize.x *item.itemSize.y > maxRow)
            {

                return false;
            }
            else
            {

                return true;
            }

        }
        return true;
    }

    protected virtual bool SizeWeightNumberValidation(GenericItemScriptable item, int number, bool isNewItem)
    {
        //ITEM NOVO
        if (isNewItem)
        {
            if (CurrentSlotUse + item.itemSize.x *item.itemSize.y <= SlotLimited)  // caso tenha a quatidade de slots necessarios
            {
                if (currentWeigthUse + item.WeigthPerItem * number <= weigthLimited)// caso nao tenha dado o peso excedido
                {
                    bool resultAdd = item.Add(number);

                    if (resultAdd)
                    {
                        UpdateSizeAndWeigth();
                        return true;
                    }
                    else
                    {
                        Debug.LogWarning("Excedeu a quantitade limite");
                    }
                }
                else // caso tenha dado o peso excedido
                {
                    bool resultAdd = item.Add(number);

                    if (resultAdd)
                    {
                        UpdateSizeAndWeigth();
                        InventoryManagerController.instance.UpdateCurrentWeigthChar();
                        return true;
                    }
                    else
                    {
                        Debug.LogWarning("Excedeu a quantitade limite");
                    }
                    UpdateSizeAndWeigth();
                    //Debug.LogWarning("Excedeu o peso limite");
                }
            }
            else
            {
                Debug.LogWarning("Excedeu o slot limite");
            }

        }
        //ITEM JA EXISTENTE
        else // caso nao tenha a quatidade de slots necessarios
        {
            if (currentWeigthUse + item.WeigthPerItem + number <= weigthLimited) // caso nao tenha dado o peso excedido
            {
                bool resultAdd = item.Add(number);
                if (resultAdd)
                {
                    UpdateSizeAndWeigth();
                    return true;
                }
                else
                {
                    UpdateSizeAndWeigth();
                    Debug.LogWarning("Excedeu a quantitade limite");
                }
            }
            else // caso tenha dado o peso excedido
            {
                bool resultAdd = item.Add(number);

                if (resultAdd)
                {
                    UpdateSizeAndWeigth();
                    InventoryManagerController.instance.UpdateCurrentWeigthChar();
                    return true;
                }
                else
                {

                    Debug.LogWarning("Excedeu a quantitade limite");
                }
                UpdateSizeAndWeigth();
                //Debug.LogWarning("Excedeu o peso limite");
            }
        }

        return false;
    }

    public virtual void UpdateSizeAndWeigth()
    {
        currentSlotUse = 0;
        currentWeigthUse = 0;

        foreach (var item in inventoryItemsList)
        {
            currentSlotUse += item.baseItemData.itemSize.x * item.baseItemData.itemSize.y;
            currentWeigthUse += item.baseItemData.TotalWeigthPerItem;
        }
    }

    public virtual bool AddItem(GenericItemScriptable item, int number,WeaponItemScriptable weaponData)
    {
        // Verifica se já existe pelo menos uma pilha do item
        InventoryItem itemIstance = new InventoryItem
        {
            baseItemData = item
        };

        var existingItems = inventoryItemsList.Where(i => i.baseItemData.Id == item.Id).ToList();

        int remainingToAdd = number;
        if (existingItems.Count > 0)
            {
                // Tenta adicionar na(s) pilha(s) existente(s)
                foreach (var existingItem in existingItems)
                {
                    int spaceLeft = existingItem.baseItemData.LimitedNumber - existingItem.baseItemData.CurrentNumber;

                    if (spaceLeft > 0)
                    {
                        int amountToAdd = Mathf.Min(spaceLeft, remainingToAdd);

                        if (!SizeWeightNumberValidation(existingItem.baseItemData, amountToAdd, false))
                        {
                            // Se não passou na validação, não adiciona mais aqui
                            return false;
                        }

                        remainingToAdd -= amountToAdd;

                        if (remainingToAdd <= 0)
                            return true; // Adicionou tudo com sucesso
                    }
                    // Se espaço zerado, pula para próxima pilha
                }

                // Se sobrou quantidade para adicionar, cria novas pilhas
                while (remainingToAdd > 0)
                {
                    if (!SlotCapacityValidation(item))
                        return false;

                    var freeSpaces = matrix.LookForFreeArea2(item.itemSize.x, item.itemSize.y);
                    if (freeSpaces.Count == 0)
                    {
                        if (autoOrganze)
                        {
                            OrganizeBySizePriority();
                            freeSpaces = matrix.LookForFreeArea2(item.itemSize.x, item.itemSize.y);
                            if (freeSpaces.Count == 0)
                            {
                                Debug.LogWarning("Não há espaço mesmo após organizar.");
                                return false;
                            }
                        }
                        else
                        {
                            Debug.LogWarning("Não há espaço suficiente para adicionar o item.");
                            return false;
                        }
                    }

                    // Instancia um novo item para nova pilha
                    GenericItemScriptable newStack = Instantiate(item);
                    int amountToAdd = Mathf.Min(newStack.LimitedNumber, remainingToAdd);

                    if (!SizeWeightNumberValidation(newStack, amountToAdd, true))
                    {
                        return false;
                    }

                    matrix.SetItem(freeSpaces, newStack.Id);
                    if (item is WeaponItemScriptable)
                    {
                        CreateInventoryItemFromWeapon(weaponData,number);
                    }
                    else
                    {
                        CreateInventoryItemFromGenericItem(item,number);
                    }
                    UpdateSizeAndWeigth();

                    remainingToAdd -= amountToAdd;
                }

                return true;
            }
            else
            {
                // Item não existe no inventário, adiciona normalmente
                if (!SlotCapacityValidation(item))
                    return false;

                var freeSpaces = matrix.LookForFreeArea2(item.itemSize.x, item.itemSize.y);
                if (freeSpaces.Count == 0)
                {
                    if (autoOrganze)
                    {
                        OrganizeBySizePriority();
                        freeSpaces = matrix.LookForFreeArea2(item.itemSize.x, item.itemSize.y);
                        if (freeSpaces.Count == 0)
                        {
                            Debug.LogWarning("Não há espaço mesmo após organizar.");
                            return false;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Não há espaço suficiente para adicionar o item.");
                        return false;
                    }
                }

                int amountToAdd = Mathf.Min(item.LimitedNumber, remainingToAdd);

                if (SizeWeightNumberValidation(item, amountToAdd, true))
                {
                    matrix.SetItem(freeSpaces, item.Id);
                    if (item is WeaponItemScriptable)
                    {
                        CreateInventoryItemFromWeapon(weaponData,number);
                    }
                    else
                    {
                        CreateInventoryItemFromGenericItem(item,number);
                    }
                    UpdateSizeAndWeigth();
                    remainingToAdd -= amountToAdd;

                    // Se ainda sobrou quantidade, cria novas pilhas (recursivo simplificado)
                    while (remainingToAdd > 0)
                    {
                        GenericItemScriptable newStack = Instantiate(item);
                        int amount = Mathf.Min(newStack.LimitedNumber, remainingToAdd);

                        freeSpaces = matrix.LookForFreeArea2(item.itemSize.x, item.itemSize.y);
                        if (freeSpaces.Count == 0)
                            return true; // Parar se não houver espaço

                        if (SizeWeightNumberValidation(newStack, amount, true))
                        {
                            matrix.SetItem(freeSpaces, newStack.Id);
                            if (item is WeaponItemScriptable)
                            {
                                CreateInventoryItemFromWeapon(weaponData,number);
                            }
                            else
                            {
                                CreateInventoryItemFromGenericItem(item,number);
                            }
                            UpdateSizeAndWeigth();
                            remainingToAdd -= amount;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    return true;
                }

                return false;
            }
    }

    public virtual bool UseItem(int id, int value)
    {
        InventoryItem item = FindInventoryItemByID(id);
        if (item != null) // CASO O ITEM EXISTA
        {
            if (item.baseItemData.Use(value)) // CASO O ITEM CONSIGA SER USADO
            {
                UpdateSizeAndWeigth();
                return true;
            }
            else // CASO O ITEM NAO CONSIGA SER USADO
            {
                Debug.LogWarning("Nao tem quantidade suficiente");
            }
        }
        else // CASO O ITEM NAO EXISTA
        {
            Debug.LogWarning("Nenhum item foi econtrado com este ID");
        }
        return false;
    }

    public virtual bool UseAmmunition(TipoDeMunicao tipoDeMunicao, int value)
    {
        //InventoryItem item = FindItemByTipoDeMunicao(tipoDeMunicao);
        InventoryItem item = BuscarItemDeMunicaoEmTodasAsBags(tipoDeMunicao);
        if (item != null) // CASO O ITEM EXISTA
        {
            if (item.baseItemData.UseMunicao(value)) // CASO O ITEM CONSIGA SER USADO
            {
                UpdateSizeAndWeigth();
                return true;
            }
            else // CASO O ITEM NAO CONSIGA SER USADO
            {
                Debug.LogWarning("Nao tem quantidade suficiente");
            }
        }
        else // CASO O ITEM NAO EXISTA
        {
            Debug.LogWarning("Nenhum item foi econtrado com este tipoDeMunicao");
        }
        return false;
    }

    public bool RemoverItem(int id,string idUnico)
    {
        InventoryItem item = FindInventoryItemByID(id);
        FireWeaponInstance fireWeaponInstance = FindWeaponInstanceByStringID(idUnico);
        if (item != null)
        {
            matrix.ClearItemOnMatrix(id);
            inventoryItemsList.Remove(item);
            if (fireWeaponInstance != null)
            {
                weaponList.Remove(fireWeaponInstance);
            }
            UpdateSizeAndWeigth();
            return true;
        }
        return false;
    }

    public bool RemoveItem(InventoryItem item)
    {
        if (item != null)
        {
            matrix.ClearItemOnMatrix(item.baseItemData.Id);
            inventoryItemsList.Remove(item);
            UpdateSizeAndWeigth();
            return true;
        }
        return false;
    }

    public bool DropeItem(int id)
    {
        InventoryItem item = FindInventoryItemByID(id);
        if (item != null)
        {
            if (item.baseItemData.IsDroppable)
            {
                item.baseItemData.Reset(); // faz a quantidade atual de itens ser 0
                RemoveItem(item);
                return true;
            }
        }
        return false;
    }

    public virtual void OrganizeBySizePriority()
    {
        usedOrganizeBtSizePryority = true;
        List<InventoryItem> temporaryList = inventoryItemsList.OrderByDescending(x => x.baseItemData.Label).ToList(); //Organiza por ordem decrescente o tipo de item
        ResetBag();
        matrix.PopulateMatrix();

        foreach (var item in temporaryList)
        {
            FireWeaponInstance fireWeaponInstance = FindWeaponInstanceByStringID(item.instanceID);
            AddItem(item.baseItemData, 0,fireWeaponInstance.weaponData);
        }
    }

    #endregion

    #region FindID

    public InventoryItem BuscarItemDeMunicaoEmTodasAsBags(TipoDeMunicao tipo)
    {
        foreach (var bag in InventoryManagerController.instance.TodasAsBags())
        {
            InventoryItem item = bag.FindItemByTipoDeMunicao(tipo);
            if (item != null)
            {
                //Debug.Log("Encontrou munição correta em uma das bags: " + item.baseItemData.name);
                return item;
            }
        }

        Debug.LogWarning("Nenhuma munição com o tipo correto foi encontrada.");
        return null;
    }

    public InventoryItem FindItemByTipoDeMunicao(TipoDeMunicao tipoDeMunicao)
    {
        InventoryItem itemResult = inventoryItemsList.Find(obj => obj.baseItemData.TipoDeMunicao == tipoDeMunicao && obj.baseItemData.TipoDeItem == ItemType.MUNICAO);
        return itemResult;
    }
    public FireWeaponInstance FindWeaponInstanceByStringID(string id)
    {
        return weaponList.FirstOrDefault(x => x.instanceID == id);
    }

    public InventoryItem FindInventoryItemByStringID(string id)
    {
        return inventoryItemsList.FirstOrDefault(x => x.instanceID == id);
    }
    public InventoryItem FindInventoryItemByID(int id)
    {
        return inventoryItemsList.FirstOrDefault(x => x.baseItemData.Id == id);
    }

    #endregion

    #region InventoryGenericList

    public InventoryItem CreateInventoryItemFromGenericItem(GenericItemScriptable itemSO,int valor)
    {
        InventoryItem item = new InventoryItem
        {
            baseItemData = itemSO,
            bagOwner = this,
            quantidade = valor
        };
        if (item.baseItemData.itemSize.x > item.baseItemData.itemSize.y)
        {
            item.isRotated = true;
        }
        else if (item.baseItemData.itemSize.x <= item.baseItemData.itemSize.y)
        {
            item.isRotated = false;
        }
        inventoryItemsList.Add(item);
        //Debug.Log($"Coletou item: {itemSO.Label} com a qtd de: {itemSO.CurrentNumber}.");
        return item;
    }

    #endregion

    #region WeaponList

    public InventoryItem CreateInventoryItemFromWeapon(WeaponItemScriptable weaponSO, int valor)
    {
        string id = System.Guid.NewGuid().ToString();

        FireWeaponInstance novaInstancia = new FireWeaponInstance(weaponSO, id);

        // Adiciona essa instância única à bag
        weaponList.Add(novaInstancia);

        InventoryItem item = new InventoryItem
        {
            baseItemData = weaponSO,
            instanceID = id,
            bagOwner = this,
            quantidade = valor
        };
        if (item.baseItemData.itemSize.x > item.baseItemData.itemSize.y)
        {
            item.isRotated = true;
        }
        else if (item.baseItemData.itemSize.x <= item.baseItemData.itemSize.y)
        {
            item.isRotated = false;
        }
        inventoryItemsList.Add(item);
        //Debug.Log($"Coletou arma: {weaponSO.nome} com {novaInstancia.currentAmmo} balas no pente. com {novaInstancia.instanceID}");
        return item;
    }

    public bool DropeWeaponInstanceItem(string index)
    {
        FireWeaponInstance itemWeapon = FindWeaponInstanceByStringID(index);
        InventoryItem item = FindInventoryItemByStringID(index);

        if (item != null)
        {
            if (item.baseItemData.IsDroppable)
            {
                item.baseItemData.Reset(); // faz a quantidade atual de itens ser 0
                RemoveWeaponInstanceItem(itemWeapon,item);
                //Debug.Log($"Removeu arma: {item.Label} Com ID: {itemWeapon.instanceID}");
                return true;
            }
        }
        return false;
    }

    public bool RemoveWeaponInstanceItem(FireWeaponInstance itemWeapon,InventoryItem item)
    {
        if (item != null)
        {
            matrix.ClearItemOnMatrix(item.baseItemData.Id);
            weaponList.Remove(itemWeapon);
            inventoryItemsList.Remove(item);
            UpdateSizeAndWeigth();
            //Debug.Log($"Removeu arma: {item.Label} Com ID: {itemWeapon.instanceID}");
            return true;
        }
        return false;
    }

    #endregion

    #region RetornarLista

    public List<Vector2> FindCellById(int id)
    {
        return matrix.FindLocationById(id);
    }

    public List<InventoryItem> ReturnFullListInventoryItem()
    {
        return inventoryItemsList;
    }

    #endregion

}
