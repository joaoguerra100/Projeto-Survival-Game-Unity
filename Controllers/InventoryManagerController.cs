using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InventoryManagerController : MonoBehaviour
{
    #region Declaration

    [Header("Scripts")]
    public static InventoryManagerController instance;
    public FireWeaponInstance equipedWeapon;

    [Header("Referencia")]
    public ScrollRect myScrollRect;

    [Header("Dropar Item")]
    [SerializeField] private GameObject playerGo; // GAMEOBJECT DO PLAYER
    [SerializeField] private GameObject directionLaunchGo; // DIREÇAO DE LANÇAMENTO DO GAMEOBJECT
    [SerializeField] private GameObject placeToDrop; // LOCAL AONDE ELE SERA DROPADO
    [SerializeField] private List<GameObject> itemListGo;

    [Header("Mochila")]
    public GenericBagScriptable vesteBag;
    public GenericBagScriptable blusaBag;
    public GenericBagScriptable calcaBag;
    public GenericBagScriptable mochilaBag;
    [SerializeField] public ClothingWeaponScriptable currentClothingWeapon;

    [Header("Animation")]
    [SerializeField] private AnimationCurve curveRate;

    [Header("Actions")]
    [SerializeField] private ItemActionPanelView itemActionPanelView;

    #endregion

    #region Methods Iniciais

    void Awake()
    {
        instance = this;
    }

    void OnDisable()
    {
        if (InputManager.instance != null)
        {
            InputManager.instance.OnInventoryKeyPressed -= HandleInventoryToggle;
            InputManager.instance.OnShortcut1Pressed -= HandleShortcut1;
            InputManager.instance.OnShortcut2Pressed -= HandleShortcut2;
            InputManager.instance.OnShortcut3Pressed -= HandleShortcut3;
            InputManager.instance.OnShortcut4Pressed -= HandleShortcut4;
            InputManager.instance.OnShortcut5Pressed -= HandleShortcut5;
            
        }
    }

    void Start()
    {
        if (InputManager.instance != null)
        {
            // Sintoniza no anúncio "OnInventoryKeyPressed" e define qual método executar
            InputManager.instance.OnInventoryKeyPressed += HandleInventoryToggle;

            // Sintoniza nos atalhos
            InputManager.instance.OnShortcut1Pressed += HandleShortcut1;
            InputManager.instance.OnShortcut2Pressed += HandleShortcut2;
            InputManager.instance.OnShortcut3Pressed += HandleShortcut3;
            InputManager.instance.OnShortcut4Pressed += HandleShortcut4;
            InputManager.instance.OnShortcut5Pressed += HandleShortcut5;
        }
        else
        {
            Debug.LogError("InputManager.instance não encontrado! O sistema de input não funcionará.");
        }

        ClothingWeaponView.instance.Iniciate(currentClothingWeapon);
        IniciarSlotsBags();

        int globalShortcutIndex = 0;
        foreach (var bag in TodasAsBagScriptable())
        {
            ShortCutView.instance.IniciateShortCutSlots(bag.MaxShortCutSlot, globalShortcutIndex);
            globalShortcutIndex += bag.MaxShortCutSlot;
        }


        ShowAndHide();

    }

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.G)) // espawnar itens
        {
            BuildMeshModel(1, 1);
        }*/
    }

    #endregion

    #region Metodos de eventos

    private void HandleInventoryToggle()
    {
        ShowAndHide();
        PlayerBracos.instance.ShowAndHideCrossHair();
        Player.instance.conteinerAberto = false;

        if (InventoryView.instance.VisiblePanel == false)
        {
            EsconderItemAction();
        }
    }

    private void HandleShortcut1()
    {
        UseShortCutToEquipItem(0, 5);
    }
    private void HandleShortcut2()
    {
        UseShortCutToEquipItem(1, 5);
    }
    private void HandleShortcut3()
    {
        UseShortCutToEquipItem(2, 5);
    }
    private void HandleShortcut4()
    {
        UseShortCutToEquipItem(3, 5);
    }
    private void HandleShortcut5()
    {
        UseShortCutToEquipItem(4, 5);
    }

    #endregion

    #region Methods

    /*public bool AddItemToCurrentBag(GenericItemScriptable newItem, int number, bool exceptionCase, WeaponItemScriptable weaponData)
            {
                bool result = false;

                //VERIFICAR SE E UMA exceptionCase
                if (!InventoryView.instance.VisiblePanel || exceptionCase)
                {
                    result = currentBag.AddItem(newItem, number, weaponData);

                    if (result)
                    {
                        List<InventoryItem> inventoryItems = currentBag.ReturnFullListInventoryItem();
                        //InventoryView.instance.UpdateAllItems(inventoryItems);
                    }

                    if (currentBag.UsedOrganizeBtSizePryority)
                    {
                        List<InventoryItem> inventoryItems = currentBag.ReturnFullListInventoryItem();
                        //InventoryView.instance.UpdateAllItems(inventoryItems);
                        currentBag.UsedOrganizeBtSizePryority = false;
                    }
                    //RefreshShortCutViewByItem(newItem);
                }
                UpdateCurrentWeigthChar(); //ATIVAR CASO QUEIRA QUE DIMINUA A VELOCIDADE DO PLAYER ANTES DE ULTRAPASSAR O LIMITE
                return result;
            }*/

    /*public void OrganizeItem()
    {
        currentBag.OrganizeBySizePriority();
        List<InventoryItem> inventoryItems = currentBag.ReturnFullListInventoryItem();
        //InventoryView.instance.UpdateAllItems(inventoryItems);
        StartCoroutine(RefreshShortCutView());
        currentBag.UsedOrganizeBtSizePryority = false;
    }*/

    public bool RemoveItem(GenericBagScriptable origin, int id, string idUnico)
    {
        bool result = false;

        if (origin != null && id >= 0)
        {
            result = origin.RemoverItem(id, idUnico);
            RemoveItemFromShortCut(idUnico);
            StartCoroutine(RefreshInventoryView());
        }
        UpdateCurrentWeigthChar(); //ATIVAR CASO QUEIRA QUE DIMINUA A VELOCIDADE DO PLAYER ANTES DE ULTRAPASSAR O LIMITE

        return result;
    }

    public bool DropItem(InventoryItem item, GenericBagScriptable bag)
    {
        bool result = false;
        int currentNumber;

        if (item.baseItemData.Id >= 0)
        {
            currentNumber = item.baseItemData.CurrentNumber;
            result = bag.DropeItem(item.baseItemData.Id);
            if (result)
            {
                EsconderItemAction();
                StartCoroutine(RefreshInventoryView());
                UpdateCurrentWeigthChar(); //ATIVAR CASO QUEIRA QUE DIMINUA A VELOCIDADE DO PLAYER ANTES DE ULTRAPASSAR O LIMITE
                BuildMeshModel(item.baseItemData.Id, currentNumber);
                return true;
            }

            else
            {
                EsconderItemAction();
                StartCoroutine(RefreshInventoryView());
                UpdateCurrentWeigthChar(); //ATIVAR CASO QUEIRA QUE DIMINUA A VELOCIDADE DO PLAYER ANTES DE ULTRAPASSAR O LIMITE
                BuildMeshModel(item.baseItemData.Id, currentNumber);
                return true;
            }
        }

        return false;
    }

    private void BuildMeshModel(int id, int currentNumber) // DROPAR O ITEM
    {
        GameObject resultItemGo = itemListGo.Find(go => go.GetComponent<ItemView>().Item.Id == id);

        if (resultItemGo != null)
        {
            Vector3 placeLaunchPos = directionLaunchGo.transform.position; //LUGAR QUE SERA GERADO

            Vector3 directionPos = directionLaunchGo.transform.forward; // DIREÇAO QUE SERA GERADO, DITO PELA SETA AZUL

            Quaternion playerRotation = playerGo.transform.rotation; // QUAL A ROTAÇAO QUE O ITEM SERA GERADO

            Vector3 spawnPos = placeLaunchPos + directionPos; // POSIÇAO QUE SERA GERADO

            GameObject obj = Instantiate(resultItemGo, spawnPos, playerRotation); // INSTANCIA O GAMEOBJECT 
            //obj.name = obj.GetComponent<ItemView>().Item.name + "_" + Time.realtimeSinceStartup; //DIZ QUANDO O ITEM FOI DROPADO

            obj.GetComponent<ItemView>().Number = currentNumber; // QUANTIDADE DE ITENS QUE SERA DROPADO

            obj.transform.SetParent(placeToDrop.transform, true); // LUGAR AONDE O OBJETO SERA INSTANCIADO, QUE E UM GAMEOBJECT VAZIO NO CENARIO

            float yForce = Random.Range(-50, -250); //FORCA RANDOMICA APLICADA NO EIXO Y 
            float zForce = Random.Range(75, 100); //FORCA RANDOMICA APLICADA NO EIXO Z

            obj.GetComponentInChildren<Rigidbody>().AddRelativeForce(0, yForce, zForce); // ACRESCENTA UM FORCA NO RIGIDBODY AO DROPAR
        }
        else
        {
            Debug.LogWarning("NAO TEM ITEM DENTRO DA LISTA listItemGo COM ESTE ID ");
        }
    }

    public void ShowAndHide()
    {
        InventoryView.instance.ShowAndHide();
        ClothingWeaponView.instance.ShowAndHide();
        if (Player.instance.conteinerAberto == false)
        {
            NearbyItemsView.intance.ShowAndHide();
        }
        else
        {
            Player.instance.currentContainerAberto.ShowAndHide();
        }

        StartCoroutine(RefreshInventoryView());
        StartCoroutine(RefreshClothingWeaponView());

    }

    #endregion

    #region Scrol View React

    IEnumerator ScrollToTopNextFrame()
    {
        yield return null; // Espera 1 frame
        myScrollRect.verticalNormalizedPosition = 1f;
    }

    #endregion

    #region Multiplos Inventarios

    public List<GenericBagScriptable> TodasAsBags()
    {
        return new List<GenericBagScriptable> { vesteBag, blusaBag, calcaBag, mochilaBag };
    }

    public List<ItemComBag> GetAllItensComBag()
    {
        List<ItemComBag> allItens = new List<ItemComBag>();

        foreach (var bag in TodasAsBags()) // TodasAsBags() deve retornar mochila, colete, roupa, etc
        {
            foreach (var item in bag.ReturnFullListInventoryItem())
            {
                allItens.Add(new ItemComBag
                {
                    item = item,
                    bag = bag
                });
            }
        }

        return allItens;
    }

    private List<BagScriptable> TodasAsBagScriptable()
    {
        return TodasAsBags().OfType<BagScriptable>().ToList();
    }

    public int GetBagIndex(GenericBagScriptable bag)
    {
        for (int i = 0; i < TodasAsBags().Count; i++)
        {
            if (TodasAsBags()[i] == bag)
                return i;
        }
        return -1;
    }

    void IniciarSlotsBags()
    {

        if (vesteBag != null) { InventoryView.instance.Iniciate(vesteBag, 0); }
        if (blusaBag != null) { InventoryView.instance.Iniciate(blusaBag, 1); }
        if (calcaBag != null) { InventoryView.instance.Iniciate(calcaBag, 2); }
        if (mochilaBag != null) { InventoryView.instance.Iniciate(mochilaBag, 3); }
        InventoryView.instance.AtualizarReferenciasDasBags(TodasAsBags());
        StartCoroutine(ScrollToTopNextFrame());
    }

    // Açao para trocar as bags
    /*public void EquiparBag(GenericBagScriptable novaBag)
    {
        // Lógica de equipar bag
        personagem.bagEquipada = novaBag;

        // Atualiza os painéis visuais
        InventoryView.instance.AtualizarReferenciasDasBags(TodasAsBags());
        StartCoroutine(RefreshInventoryView());
    }*/

    public bool AdicionarItemEmAlgumaBag(GenericItemScriptable item, int quantidade, bool exceptionCase, WeaponItemScriptable armaData)
    {
        if (!InventoryView.instance.VisiblePanel || exceptionCase)
        {
            foreach (var bag in TodasAsBags())
            {
                if (bag.AddItem(item, quantidade, armaData))
                {
                    //Debug.Log($"Adicionado {item.name} na bag {bag.name}");
                    InventoryView.instance.UpdateAllItems(GetAllItensComBag()); // <-- Atualiza UI de TODAS as bags
                    return true;
                }
            }
            Debug.LogWarning("Nenhuma bag tinha espaço.");
        }
        UpdateCurrentWeigthChar();
        return false;
    }

    #endregion

    #region Methods Usar

    private void UseItem(GenericBagScriptable bag, int id, int value)
    {
        bool result = bag.UseItem(id, value);

        if (result)
        {
            InventoryItem itemResult = bag.FindInventoryItemByID(id);
            StartCoroutine(RefreshInventoryView());
            EsconderItemAction();

            if (itemResult.baseItemData.CurrentNumber <= 0 && itemResult.baseItemData.RemoveWhenNumberIsZero)
            {
                //DropItem(itemResult);
                bag.RemoverItem(itemResult.baseItemData.Id, itemResult.instanceID);
                StartCoroutine(RefreshInventoryView());
                EsconderItemAction();
            }
        }
    }

    public void UseAmmunition(TipoDeMunicao tipoDeMunicao, int qtdDeMunicao)
    {
        foreach (var bag in TodasAsBags())
        {
            InventoryItem itemResult = bag.FindItemByTipoDeMunicao(tipoDeMunicao);

            if (itemResult == null || itemResult.baseItemData == null)
                continue;

            bool result = bag.UseAmmunition(tipoDeMunicao, qtdDeMunicao);

            if (result)
            {
                if (itemResult.baseItemData.CurrentNumber <= 0 && itemResult.baseItemData.RemoveWhenNumberIsZero)
                {
                    bag.RemoveItem(itemResult);
                }
                //Debug.Log("Munição usada com sucesso.");
                break;
            }
        }
    }

    public void UseItemAction(int itemIndiceID, string idUnico)
    {
        foreach (var bag in TodasAsBags())
        {
            InventoryItem item = bag.FindInventoryItemByID(itemIndiceID);
            if (item == null) continue; // ← Pula bag se não achou item

            FireWeaponInstance instancia = bag.FindWeaponInstanceByStringID(item.instanceID);
            MostrarItemAction(itemIndiceID);

            if (item.baseItemData.TipoDeItem == ItemType.ARMA || item.baseItemData.TipoDeItem == ItemType.ROUPA)
            {
                if (instancia != null && instancia.equipado == false)
                {
                    AddAction("Equipar", () => PerformActionEquip(idUnico, bag));
                }
                else if (instancia != null && instancia.equipado == true)
                {
                    AddAction("Desequipar", () => PerformActionEquip(idUnico, bag));
                }
                if (item.baseItemData.IsDroppable)
                {
                    AddAction("Dropar", () => PerfomActionDropWeaponItem(idUnico, bag));
                }
            }
            else if (item.baseItemData.TipoDeItem == ItemType.COMIDA_BEBIDA || item.baseItemData.TipoDeItem == ItemType.ITEN_MEDICO)
            {

                AddAction("Usar", () => UseItem(bag, itemIndiceID, 1));
                if (item.baseItemData.IsDroppable)
                {
                    AddAction("Dropar", () => DropItem(item, bag));
                }
            }
            else
            {
                if (item.baseItemData.IsDroppable)
                {
                    AddAction("Dropar", () => DropItem(item, bag));
                }
            }

            break; // ← item encontrado e processado, pode sair do loop
        }
    }

    #endregion

    #region Item Action Panel

    public void PerformActionEquip(string itemIndiceID, GenericBagScriptable bag)
    {
        InventoryItem item = bag.FindInventoryItemByStringID(itemIndiceID);
        FireWeaponInstance fireWeaponInstance = bag.FindWeaponInstanceByStringID(item.instanceID);

        if (item.baseItemData is WeaponItemScriptable)
        {
            if (fireWeaponInstance.equipado == true) // AÇAO DE DESEQUIPAR O ITEM
            {
                //Debug.Log($"Desequipou: {fireWeaponInstance.weaponData.Label} Com ID: {fireWeaponInstance.instanceID}");
                DesequiparTodasAsArmas();
                PlayerBracos.instance.anim.SetTrigger("Desequipar");
                fireWeaponInstance.weaponData.ActionEquipAndUnequipListDispatch();
                Player.instance.trocaAnimator = true;
                EsconderItemAction();
                StartCoroutine(RefreshInventoryView());
                List<string> idListClothing = currentClothingWeapon.GetIdsFromItemsClothingWeaponDictionary();
                foreach (var idClothing in idListClothing)
                {
                    if (idClothing == fireWeaponInstance.instanceID)
                    {
                        currentClothingWeapon.RemoveItemById(fireWeaponInstance.instanceID);
                        StartCoroutine(RefreshClothingWeaponView());
                    }
                    else
                    {
                        Debug.Log($"este item nao esta dentro do clothingWeaponView ou esta com id diferente ID Clothing: {idClothing}, And ID Item: {fireWeaponInstance.instanceID}");
                    }
                }
            }
            // AÇAO DE EQUIPAR O ITEM
            else
            {
                if (fireWeaponInstance != null)
                {
                    //Debug.Log($"Equipou: {fireWeaponInstance.weaponData.Label} Com ID: {fireWeaponInstance.instanceID}");
                    Player.instance.trocaAnimator = true;
                    DesequiparTodasAsArmas();
                    equipedWeapon = fireWeaponInstance;
                    fireWeaponInstance.equipado = true;
                    fireWeaponInstance.weaponData.ActionEquipAndUnequipListDispatch();
                    EsconderItemAction();
                }
                else
                {
                    Debug.LogError("Instância da arma não encontrada para ID: " + fireWeaponInstance.instanceID);
                }
            }
        }
    }

    public bool PerfomActionDropWeaponItem(string itemIndiceID, GenericBagScriptable bag)
    {
        InventoryItem item = bag.FindInventoryItemByStringID(itemIndiceID);
        FireWeaponInstance fireWeaponInstance = bag.FindWeaponInstanceByStringID(item.instanceID);
        bool result = false;
        int currentNumber;

        if (item.baseItemData.Id >= 0)
        {
            currentNumber = item.baseItemData.CurrentNumber;
            result = bag.DropeWeaponInstanceItem(fireWeaponInstance.instanceID);
            if (result && fireWeaponInstance.equipado == false)
            {
                //Debug.Log($"Dropou: {fireWeaponInstance.weaponData.Label} Com ID: {fireWeaponInstance.instanceID}");
                EsconderItemAction();
                StartCoroutine(RefreshInventoryView());
                List<string> idListShortCut = TodasAsBagScriptable().Select(bag => bag.GetIdsFromItemsShortCutDictionary()).FirstOrDefault(item => item != null);
                foreach (var idShortCut in idListShortCut)
                {
                    if (idShortCut == fireWeaponInstance.instanceID)
                    {
                        RemoveItemFromShortCut(item.instanceID);
                    }
                    else
                    {
                        Debug.Log($"este item nao esta dentro do shortcut ou esta com id diferente ID Shortcut: {idShortCut}, And ID Item: {fireWeaponInstance.instanceID}");
                    }
                }
                UpdateCurrentWeigthChar(); //ATIVAR CASO QUEIRA QUE DIMINUA A VELOCIDADE DO PLAYER ANTES DE ULTRAPASSAR O LIMITE
                BuildMeshModel(item.baseItemData.Id, currentNumber);
                return true;
            }

            else if (result && fireWeaponInstance.equipado == true)
            {
                //Debug.Log($"Dropou: {fireWeaponInstance.weaponData.Label} Com ID: {fireWeaponInstance.instanceID}");
                DesequiparTodasAsArmas();
                fireWeaponInstance.weaponData.ActionEquipAndUnequipListDispatch();
                EsconderItemAction();
                StartCoroutine(RefreshInventoryView());
                List<string> idListClothing = currentClothingWeapon.GetIdsFromItemsClothingWeaponDictionary();
                List<string> idListShortCut = TodasAsBagScriptable().Select(bag => bag.GetIdsFromItemsShortCutDictionary()).FirstOrDefault(item => item != null);
                foreach (var idShortCut in idListShortCut)
                {
                    if (idShortCut == fireWeaponInstance.instanceID)
                    {
                        RemoveItemFromShortCut(item.instanceID);
                    }
                    else
                    {
                        Debug.Log($"este item nao esta dentro do shortcut ou esta com id diferente ID Shortcut: {idShortCut}, And ID Item: {fireWeaponInstance.instanceID}");
                    }
                }
                foreach (var idClothing in idListClothing)
                {
                    if (idClothing == fireWeaponInstance.instanceID)
                    {
                        currentClothingWeapon.RemoveItemById(fireWeaponInstance.instanceID);
                        StartCoroutine(RefreshClothingWeaponView());
                    }
                    else
                    {
                        Debug.Log($"este item nao esta dentro do clothingWeaponView ou esta com id diferente ID Clothing: {idClothing}, And ID Item: {fireWeaponInstance.instanceID}");
                    }
                }
                UpdateCurrentWeigthChar(); //ATIVAR CASO QUEIRA QUE DIMINUA A VELOCIDADE DO PLAYER ANTES DE ULTRAPASSAR O LIMITE
                BuildMeshModel(item.baseItemData.Id, currentNumber);
                return true;
            }
        }
        return false;
    }

    void DesequiparTodasAsArmas()
    {
        foreach (var bag in TodasAsBags())
        {
            equipedWeapon = null;
            foreach (var item in bag.weaponList)
            {
                item.equipado = false;
            }
        }
    }

    public void MostrarItemAction(int index)
    {
        foreach (var bag in TodasAsBags())
        {
            bag.FindCellById(index);
            itemActionPanelView.Toggle(true);
            itemActionPanelView.transform.position = Input.mousePosition;
        }
    }
    public void EsconderItemAction()
    {
        itemActionPanelView.Toggle(false);
    }

    public void AddAction(string actionName, Action performAction)
    {
        itemActionPanelView.AddButon(actionName, performAction);
    }

    #endregion

    #region Sistema De Peso

    public void UpdateCurrentWeigthChar() // ATUALIZA A VELOCIDADE DO PERSONAGEM DE ACORDO COM O PESO PEGO
    {
        foreach (var bag in TodasAsBags())
        {
            float x = bag.CurrentWeigthUse;
            float y = currentClothingWeapon.CurrentWeigUse;

            float rate = curveRate.Evaluate(x + y);
            if (bag.CurrentWeigthUse > bag.WeigthLimited)
            {
                GameController.instance.ChangeRate(rate);
            }
            if (bag.CurrentWeigthUse <= bag.WeigthLimited)
            {
                GameController.instance.ResetMoveSpeed();
            }
            //GameController.instance.ChangeRate(rate);
        }
    }

    #endregion

    #region IEnumerator

    public IEnumerator RefreshInventoryView()
    {
        yield return new WaitForSeconds(0.02f);
        List<ItemComBag> allItensComBag = new List<ItemComBag>();
        foreach (var bag in TodasAsBags())
        {
            foreach (var item in bag.ReturnFullListInventoryItem())
            {
                allItensComBag.Add(new ItemComBag { item = item, bag = bag });
            }
        }
        InventoryView.instance.UpdateAllItems(allItensComBag);
        NearbyItemsView.intance.AtualizarItensPertos();
        IniciarSlotsBags();
    }

    public IEnumerator RefreshShortCutView()
    {
        yield return new WaitForSeconds(0.02f);

        BagScriptable resultBag = CastGenericBagToBag();
        Dictionary<int, InventoryItem> resultDictionary = resultBag.ItemShortCutDictionary;
        ShortCutView.instance.RefreshSlotSystem(resultDictionary);
    }

    public IEnumerator RefreshClothingWeaponView()
    {
        yield return new WaitForSeconds(0.01f);
        Dictionary<int, InventoryItem> resultDictionary = currentClothingWeapon.ItemsDictionary;
        ClothingWeaponView.instance.RefreshSlotSystem(resultDictionary);

        //Update current weigthchar
    }

    #endregion

    #region Clothing Weapon Methods

    public bool TransferItemFromBagToClothingWeapon(int index, string id)
    {
        InventoryItem item = TodasAsBags().Select(bag => bag.FindInventoryItemByStringID(id)).FirstOrDefault(item => item != null);
        if (item == null)
        {
            Debug.LogWarning($"[ClothingWeapon] Nenhum item encontrado No inventario com ID: {id}");
            return false;
        }

        GenericBagScriptable origemBag = item.bagOwner;
        if (origemBag == null)
        {
            Debug.LogWarning("[ClothingWeapon] Item não está associado a nenhuma bag.");
            return false;
        }

        FireWeaponInstance fireWeapon = origemBag.FindWeaponInstanceByStringID(item.instanceID);
        if (fireWeapon == null)
        {
            Debug.LogWarning("[SHORTCUT] Instância da arma não encontrada.");
            return false;
        }

        bool result = currentClothingWeapon.AddItem(index, item);

        if (result)
        {
            StartCoroutine(RefreshClothingWeaponView());
            fireWeapon.weaponData.ActionEquipAndUnequipListDispatch(); //ITEM EQUIPAR
            equipedWeapon = fireWeapon;
            UpdateCurrentWeigthChar(); //ATIVAR CASO QUEIRA QUE DIMINUA A VELOCIDADE DO PLAYER ANTES DE ULTRAPASSAR O LIMITE
            return RemoveItem(origemBag, item.baseItemData.Id, item.instanceID);
        }

        return false;
    }

    public bool TransferItemFromClothingWeaponToBag(string id)
    {
        InventoryItem item = currentClothingWeapon.GetItemByStringId(id);
        if (item == null)
        {
            Debug.LogWarning($"[TransferItemFromClothingWeaponToBag] Item{item} com ID: {id} não foi encontrado.");
            return false;
        }

        bool result = currentClothingWeapon.RemoveItemById(item.instanceID);
        if (!result)
        {
            Debug.LogWarning($"[TransferItemFromClothingWeaponToBag] Falha ao remover item{item} Com ID: {id} do currentClothingWeapon.");
            return false;
        }

        FireWeaponInstance fireWeaponInstance = item.baseItemData is WeaponItemScriptable ? new FireWeaponInstance((WeaponItemScriptable)item.baseItemData, item.instanceID) : null;

        StartCoroutine(RefreshClothingWeaponView());
        fireWeaponInstance?.weaponData.ActionEquipAndUnequipListDispatch();
        DesequiparTodasAsArmas();
        PlayerBracos.instance.anim.SetTrigger("Desequipar");
        UpdateCurrentWeigthChar();

        return AdicionarItemEmAlgumaBag(item.baseItemData, 0, true, fireWeaponInstance.weaponData);
    }

    #endregion

    #region OnDropItem and OnPointerDown Methods

    public bool OnDropItem(InventoryItem itemDrop, GameObject origin, Vector2 coordinate, SlotPraceTo slotPraceTo)
    {
        //Debug.Log(itemDrop + "" + origin + "" + "" + coordinate + "" + slotPraceTo);
        int index = (int)coordinate.y;

        //BAG ->
        if (origin.transform.parent.parent.parent.name == "Content")
        {
            //SETUP OutraBag<-
            /*if (slotPraceTo == SlotPraceTo.BAG)
            {
                return true;
            }*/
            //SETUP SHORTCUT<-
            if (slotPraceTo == SlotPraceTo.SHORT_CUT && slotPraceTo != SlotPraceTo.BAG)
            {
                return AddItemToCurrentShortCut(index, itemDrop);
            }
            //SETUP CLOTHINGWEAPON <-
            if (slotPraceTo == SlotPraceTo.CLOTHING_WEAPON && slotPraceTo != SlotPraceTo.BAG)
            {
                return TransferItemFromBagToClothingWeapon(index, itemDrop.instanceID);
            }
        }

        //SHORTCUT -> CHANGE FOR ANOTHER SHORTCUT POSITION
        if (origin.transform.parent.parent.name == "SlotSpecialGroup" && slotPraceTo == SlotPraceTo.SHORT_CUT)
        {
            // CONSEGUE TROCAR OS ITENS DOS SHORTCUT DE LUGAR, ATIVE CASO QUEIRA USAR ISTO
            //return ChangeShortCutPosition(itemDrop, index);
        }
        return false;
    }

    public bool OnDropItem(string originTag, InventoryItem item)
    {
        GenericBagScriptable origemBag = item.bagOwner;
        if (origemBag == null)
        {
            Debug.LogWarning("[DROP] Item sem bagOwner associado.");
            return false;
        }

        InventoryItem inventoryItem = origemBag.inventoryItemsList.FirstOrDefault(i => i.instanceID == item.instanceID);
        if (inventoryItem == null)
        {
            Debug.LogWarning("[DROP] Item não encontrado na bag.");
            return false;
        }
        FireWeaponInstance itemDrop = origemBag.FindWeaponInstanceByStringID(item.instanceID);
        if (itemDrop == null && inventoryItem.baseItemData is WeaponItemScriptable)
        {
            Debug.LogWarning("[DROP] Item não encontrado na bag.");
            return false;
        }
        //ITEMDROP VINDO DO INVENTARIO
        if (originTag.Equals("ComplexSlot"))
        {
            //Debug.Log("[DROP] Slot do inventário detectado.");

            if (item.baseItemData is WeaponItemScriptable)
            {
                if (itemDrop.equipado)
                {
                    itemDrop.equipado = false;
                    currentClothingWeapon.RemoveItemById(itemDrop.instanceID);
                    itemDrop.weaponData.ActionEquipAndUnequipListDispatch();
                    StartCoroutine(RefreshClothingWeaponView());
                    PerfomActionDropWeaponItem(item.instanceID, origemBag);
                }
                else
                {
                    PerfomActionDropWeaponItem(item.instanceID, origemBag);
                }
            }
            else
            {
                return DropItem(item, origemBag);
            }
        }

        //ITEMDROP VINDO DO SHORTCUT
        if (originTag.Equals("SpecialSlot"))
        {
            if (!item.baseItemData.Equipado)
            {
                return RemoveItemFromShortCut(item.instanceID);
            }
        }

        //ITEMDROP VINDO DO CLOTHINGWEAPONS
        if (originTag.Equals("ClothingWeaponSlot"))
        {
            //Debug.Log($"[OnDropItem] Recebido drop de ClothingWeaponSlot. ID: {item.instanceID}");
            return TransferItemFromClothingWeaponToBag(item.instanceID);
        }

        return false;
    }

    public void OnPointerDownItem(InventoryItem itemPointerDown, GameObject origin)
    {
        if (origin.transform.parent.parent.name == "InventarioPainel")
        {
            InventoryView.instance.UpdateDiscriptionAndDetailPanel(itemPointerDown);
        }
    }

    #endregion

    #region ShortCut Methods

    private bool AddItemToCurrentShortCut(int index, InventoryItem item)
    {
        BagScriptable resultBag = CastGenericBagToBag();
        if (resultBag == null) return false;
        bool result = resultBag.AddItemToShortCut(index, item);
        if (result)
        {
            RefreshShortCutViewByItem(item);
            return result;
        }

        return false;
    }


    public bool ChangeShortCutPosition(InventoryItem itemChanged, int index)
    {
        BagScriptable resultBag = CastGenericBagToBag();
        if (resultBag == null) return false;
        bool result = resultBag.ChangeItemShortCutPosition(itemChanged, index);
        if (result)
        {
            StartCoroutine(RefreshShortCutView());
            return true;
        }

        return false;
    }

    public bool RemoveItemFromShortCut(string id)
    {
        BagScriptable resultBag = CastGenericBagToBag();
        if (resultBag == null) return false;
        bool result = resultBag.RemoveItemFromShortCutById(id);
        if (result)
        {
            StartCoroutine(RefreshShortCutView());
            return true;
        }

        return false;
    }

    /*private void UseShortCutToUseItem(int index)
    {
        if (!InventoryView.instance.VisiblePanel)
        {
            BagScriptable resultBag = CastGenericBagToBag();
            GenericItemScriptable resultItem = resultBag.GetItemByIndexPosition(index);

            if (resultItem != null)
            {
                UseItem(resultItem.Id, 1);
            }
            else
            {
                Debug.LogWarning("NAO TEM NENHUM ITEM DENTRO DESTE SHORTCUTSLOT");
            }

            StartCoroutine(RefresInventoryView());
        }
    }*/

    // Mudei para PUBLIC para você poder chamar esse método pelo botão da UI na Unity se necessário
    public void UseShortCutToEquipItem(int shortcutIndex, int equipSlotIndex)
    {
        if (InventoryView.instance.VisiblePanel) return;

        InventoryItem resultItem = null;
        GenericBagScriptable origemBag = null;

        foreach (var bag in TodasAsBagScriptable())
        {
            // Verifica se essa bag tem o item registrado no atalho solicitado
            if (bag.ItemShortCutDictionary != null && bag.ItemShortCutDictionary.ContainsKey(shortcutIndex))
            {
                resultItem = bag.ItemShortCutDictionary[shortcutIndex];
                origemBag = bag;
                break;
            }
        }

        // Se não achou nada no atalho, sai
        if (resultItem == null)
        {
            Debug.LogWarning($"[SHORTCUT] Nenhum item encontrado mapeado no atalho {shortcutIndex}");
            return;
        }

        // Busca a referência da arma
        FireWeaponInstance fireWeapon = origemBag.FindWeaponInstanceByStringID(resultItem.instanceID);
        
        if (fireWeapon == null)
        {
            Debug.LogWarning("[SHORTCUT] O item no atalho não é uma arma válida ou instância não encontrada.");
            return;
        }

        Player.instance.trocaAnimator = true;

        InventoryItem equippedItem = currentClothingWeapon.GetItemByIndex(equipSlotIndex);

        // LÓGICA DE TOGGLE (EQUIPAR / DESEQUIPAR)
        
        // Se o item clicado já é exatamente o que está na mão: Desequipa
        bool isAlreadyEquippedInSlot = equippedItem != null && equippedItem.instanceID == resultItem.instanceID;

        if (isAlreadyEquippedInSlot)
        {
            // DESEQUIPAR 
            DesequiparTodasAsArmas();
            
            fireWeapon.weaponData.ActionEquipAndUnequipListDispatch();
            PlayerBracos.instance.anim.SetTrigger("Desequipar");
            
            currentClothingWeapon.RemoveItemById(resultItem.instanceID);
        }
        else
        {
            // Equipar
            if (equippedItem != null)
            {
                InventoryItem itemAntigo = equippedItem;
                FireWeaponInstance armaAntiga = origemBag.FindWeaponInstanceByStringID(itemAntigo.instanceID);
                if(armaAntiga != null) armaAntiga.weaponData.ActionEquipAndUnequipListDispatch();
                
                currentClothingWeapon.RemoveItemById(equippedItem.instanceID);
            }
            DesequiparTodasAsArmas(); 
            fireWeapon.equipado = true;
            equipedWeapon = fireWeapon;
            currentClothingWeapon.AddItem(equipSlotIndex, resultItem);
            fireWeapon.weaponData.ActionEquipAndUnequipListDispatch();
        }

        StartCoroutine(RefreshInventoryView());
        StartCoroutine(RefreshClothingWeaponView());
    }

    private void RefreshShortCutViewByItem(InventoryItem itemUpdate)
    {
        BagScriptable resultBag = CastGenericBagToBag();
        List<string> idsResults = resultBag.GetIdsFromItemsShortCutDictionary();

        foreach (var id in idsResults)
        {
            if (id == itemUpdate.instanceID)
            {
                Dictionary<int, InventoryItem> resultDictionary = resultBag.ItemShortCutDictionary;
                List<int> KeyResults = resultBag.GetUsedKeysFromShortCutDictionary();
                ShortCutView.instance.UpdateSlot(resultDictionary, KeyResults);
            }
        }
    }

    #endregion

    #region Helpers Methods

    private BagScriptable CastGenericBagToBag()
    {
        BagScriptable resultBag = ScriptableObject.CreateInstance<BagScriptable>();
        foreach (var bags in GetAllItensComBag())
        {
            if (bags.bag is BagScriptable)
            {
                resultBag = (BagScriptable)bags.bag;
            }
        }

        return resultBag;
    }

    #endregion

}

[Serializable]
public class ItemComBag
{
    public InventoryItem item;
    public GenericBagScriptable bag;
}
