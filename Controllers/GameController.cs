using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using System.Linq;


public class GameController : MonoBehaviour
{
    #region Variaveis

    [Header("Scripts")]
    public static GameController instance;
    [SerializeField] private Player playerSpt;

    [Header("Valores Default")]
    private float rateReduceMovements = 1;
    private float defaultMoveSpeed = 0;
    private float defaultJump = 0;

    [Header("PostProcessVolume")]
    [HideInInspector] public bool bloom, occlusion, reflection, motionBloor, deptthOfField;

    public ItemDatabase itemDatabase;



    #endregion

    #region Methods Iniciais

    void Awake()
    {
        instance = this;
        
    }

    void Start()
    {
        DontDestroyOnLoad(this);
        defaultMoveSpeed = playerSpt.stats.velAndar;
        defaultJump = playerSpt.alturaDoPulo;
    }

    void Update()
    {
        bool estaSeMovendo = Player.instance.movimentosJogador.magnitude > 0.1f; // exemplo para esforço físico
        DiminuiFomeSede(estaSeMovendo);
    }

    #endregion

    public void ChangeRate(float rate) //MUDA A VELOCIDADE DO PERSONAGEM A PARTIRAR DA TAXA RATE
    {
        rateReduceMovements = rate;
        playerSpt.stats.velAndar = defaultMoveSpeed * rateReduceMovements;
        playerSpt.alturaDoPulo = defaultJump * rateReduceMovements;
    }

    public void ResetMoveSpeed()
    {
        playerSpt.stats.velAndar = defaultMoveSpeed;
        playerSpt.alturaDoPulo = defaultJump;
    }

    #region Sede/Fome/Stam.Vida

    private void DiminuiFomeSede(bool estaSeMovendo)
    {
        float perdaFome = (estaSeMovendo ? 0.066f : 0.033f);
        float perdaSede = (estaSeMovendo ? 0.1f : 0.05f);

        // Reduz Fome e Sede
        Player.instance.stats.fomeAtual -= perdaFome * Time.deltaTime;
        Player.instance.stats.fomeAtual = Mathf.Clamp(Player.instance.stats.fomeAtual, 0, Player.instance.stats.fomeMaxima);

        Player.instance.stats.sedeAtual -= perdaSede * Time.deltaTime;
        Player.instance.stats.sedeAtual = Mathf.Clamp(Player.instance.stats.sedeAtual, 0, Player.instance.stats.sedeMaxima);

        // --- Penalidades por Fome ---
        if (Player.instance.stats.fomeAtual < 10f)
        {
            // Fome crítica: perde vida
            Player.instance.stats.vidaAtual -= 0.7f * Time.deltaTime;
            //Debug.Log("Fome crítica - perdendo vida");
        }
        else if (Player.instance.stats.fomeAtual < 30f)
        {
            // Fome moderada: regen reduzida
            Player.instance.RecuperarStamina(3f);
            //Debug.Log("Fome moderada - stamina reduzida");
        }

        // --- Penalidades por Sede ---
        if (Player.instance.stats.sedeAtual < 10f)
        {
            // Sede crítica: perde vida
            Player.instance.stats.vidaAtual -= 0.7f * Time.deltaTime;
            //Debug.Log("Sede crítica - perdendo vida");
        }
        else if (Player.instance.stats.sedeAtual < 30f)
        {
            // Sede moderada: regen reduzida
            Player.instance.RecuperarStamina(3.6f);
            //Debug.Log("Sede moderada - stamina reduzida");
        }

        // Se nenhum efeito negativo, regen normal
        if (Player.instance.stats.fomeAtual >= 30f && Player.instance.stats.sedeAtual >= 30f)
        {
            Player.instance.RecuperarStamina(6f);
            //Debug.Log("Stamina regenerando em 6");
        }

        // Garante que vida não passe do limite
        Player.instance.stats.vidaAtual = Mathf.Clamp(Player.instance.stats.vidaAtual, 0, Player.instance.stats.vidaMaxima);
    }

    public void ChangeVida(float value) // MUDA A VIDA ACRESCENTANDO MAIS VIDA
    {
        float vidaAtual = Player.instance.stats.vidaAtual;
        vidaAtual = Validate(value, vidaAtual);
        Player.instance.stats.vidaAtual = Mathf.RoundToInt(vidaAtual);
    }

    public void ChangeEstamina(float value)
    {

    }

    public void ChangeFome(float value) // MUDA A FOME ACRESCENTANDO COMIDA
    {
        Player.instance.stats.fomeAtual = Validate(value, Player.instance.stats.fomeAtual);
    }

    public void ChangeSede(float value) // MUDA A SEDE ACRESCENTANDO BEBIDA
    {
        Player.instance.stats.sedeAtual = Validate(value, Player.instance.stats.sedeAtual);
    }

    private float Validate(float value, float variable) // VALIDAÇAO PARA USAR ITENS DE AUMENTAR OS RECURSOS
    {
        if (variable + value >= 0 && variable + value <= 100)
        {
            variable += value;
        }
        else
        {
            if (variable + value > 0)
            {
                variable = 100;
            }
            else
            {
                variable = 0;
            }
        }

        return variable;
    }

    #endregion

    public void ShowPopUp(string texto, Sprite icon, Color textColor, Color backGroundColor, float timeToClosePopUp)
    {
        PopUpView.instance.PopUpUpdate(texto, icon, textColor, backGroundColor, timeToClosePopUp);
    }

    #region Equipar/Desequipar

    public void EquipChar(ItemEquip itemEquip)
    {
        switch (itemEquip)
        {
            case ItemEquip.AK_47:
                SetEquipamenteArma(itemEquip);
                break;
            case ItemEquip.PISTOLA_SK:
                SetEquipamenteArma(itemEquip);
                break;
            case ItemEquip.Mp5:
                SetEquipamenteArma(itemEquip);
                break;
            case ItemEquip.M4A1:
                SetEquipamenteArma(itemEquip);
                break;
            case ItemEquip.Ak_12:
                SetEquipamenteArma(itemEquip);
                break;
            case ItemEquip.Scar_L:
                SetEquipamenteArma(itemEquip);
                break;
            case ItemEquip.M1911:
                SetEquipamenteArma(itemEquip);
                break;
            case ItemEquip.Walther_PP:
                SetEquipamenteArma(itemEquip);
                break;
            case ItemEquip.As_50:
                SetEquipamenteArma(itemEquip);
                break;
            case ItemEquip.AWM:
                SetEquipamenteArma(itemEquip);
                break;
            case ItemEquip.Mossberg590:
                SetEquipamenteArma(itemEquip);
                break;
            case ItemEquip.REVOLVER:
                SetEquipamenteArma(itemEquip);
                break;
            case ItemEquip.CROSSBOW:
                SetEquipamenteArma(itemEquip);
                break;
            case ItemEquip.Minibea_Pm_9:
                SetEquipamenteArma(itemEquip);
                break;

        }
    }

    private void SetEquipamenteArma(ItemEquip itemEquip)
    {
        bool status = PlayerView.instance.StatusEquipment(itemEquip);
        PlayerView.instance.Equip(itemEquip, !status);
    }
    private void SetEquipamenteRoupa(RoupaEquip itemEquipRoupa)
    {
        bool status = PlayerView.instance.StatusEquipmentRoupa(itemEquipRoupa);
        PlayerView.instance.EquipRoupa(itemEquipRoupa, !status);
    }

    #endregion

    #region MetodosSave

    void SaveStats(Dados dados)
    {
        dados.dadosVida = Player.instance.stats.vidaAtual;
        dados.dadosFome = Player.instance.stats.fomeAtual;
        dados.dadosSede = Player.instance.stats.sedeAtual;
    }
    void SaveAudio(Dados dados)
    {
        dados.volumeFx = PauseController.instance.effectVol.value;
        dados.volumeMusica = PauseController.instance.musicsVol.value;
        
    }
    /*void SaveInventory(Dados dados)
    {
        foreach (GenericItemScriptable item in InventoryManagerController.instance.currentBag.itemList)
        {
            dados.itensSalvos.Add(new ItemSaveData
            {
                itemID = item.Id,
                quantidade = item.CurrentNumber // supondo que esse seja o campo da quantidade
            });
        }
    }*/
    /*void SaveShortCut(Dados dados)
    {
        BagScriptable resultBag = CastGenericBagToBag();
        var shortcutDict = resultBag.ItemShortCutDictionary;
        foreach (var kvp in shortcutDict)
        {
            dados.shortcutSalvos.Add(new ShortcutData
            {
                index = kvp.Key,
                itemId = kvp.Value.Id
            });
        }
    }*/
    /*void SaveClothingWeapon(Dados dados)
    {
        ClothingWeaponScriptable clothingWeapon = InventoryManagerController.instance.currentClothingWeapon;
        List<EquippedItemData> equippedList = new List<EquippedItemData>();

        foreach (var item in clothingWeapon.ItemsDictionary)
        {
            EquippedItemData data = new EquippedItemData
            {
                slotIndex = item.Key,
                itemID = item.Value.Id,
                equipado = item.Value.Equipado,

            };

            // Só se for uma arma, salva o tipo da arma
            if (item.Value is WeaponItemScriptable arma)
            {
                data.tipoEquip = arma.tipoEquip;
            }

            equippedList.Add(data);
        }
        dados.equippedItems = equippedList;
    }*/
    void SaveWeaponList(Dados dados)
    {
        foreach (var weaponGO in PlayerView.instance.weaponGoList)
        {
            if (weaponGO.TryGetComponent(out Arma arma))
            {
                dados.armasSalvas.Add(new ArmaSaveData
                {
                    weaponName = weaponGO.name,
                    municaoAtual = arma.equipedWeapon.currentAmmo,
                    maxMunicao = arma.weaponData.municaoMaxima,
                    municaoParaRecarregar = arma.weaponData.municaoParaRecarregar
                });
            }
        }
    }

    #endregion

    #region MetodosLoad

    void LoadStats(Dados dados)
    {
        Player.instance.stats.vidaAtual = dados.dadosVida;
        Player.instance.stats.fomeAtual = dados.dadosFome;
        Player.instance.stats.sedeAtual = dados.dadosSede;
    }
    void LoadAudio(Dados dados)
    {
        PauseController.instance.effectVol.value = dados.volumeFx;
        PauseController.instance.musicsVol.value = dados.volumeMusica;
    }
    /*void LoadInventory(Dados dados)
    {
        // Limpar a bag
        InventoryManagerController.instance.currentBag.ResetBag();
        // Carrega itens do inventario
        foreach (ItemSaveData itemData in dados.itensSalvos)
        {
            GenericItemScriptable item = itemDatabase.GetItemByID(itemData.itemID);
            if (item != null)
            {
                item.Reset(); // Zera a quantidade atual
                InventoryManagerController.instance.currentBag.AddItem(item, itemData.quantidade);
            }
            else
            {
                Debug.LogWarning("Item não encontrado no banco: " + itemData.itemID);
            }
        }
        StartCoroutine(InventoryManagerController.instance.RefresInventoryView());
        InventoryManagerController.instance.UpdateCurrentWeigthChar();
    }*/
    /*void LoadShortChut(Dados dados)
    {
        BagScriptable resultBag = CastGenericBagToBag();
        // Carrega itens do shortcut
        resultBag.ClearShortcutDictionary();

        foreach (ShortcutData shortcut in dados.shortcutSalvos)
        {
            GenericItemScriptable item = itemDatabase.GetItemByID(shortcut.itemId);
            if (item != null)
            {
                bool added = resultBag.AddItemToShortCut(shortcut.index, item);
                if (!added)
                    Debug.LogWarning($"Não foi possível adicionar o item {item.name} no slot {shortcut.index}");
            }
            else
            {
                Debug.LogWarning("Item do shortcut não encontrado no banco: " + shortcut.itemId);
            }
        }
        StartCoroutine(InventoryManagerController.instance.RefreshShortCutView());
    }*/
    /*void LoadClothingWeapon(Dados dados)
    {
        //  Carrega itens do Clothing Weapon View
        ClothingWeaponScriptable clothingWeapon = InventoryManagerController.instance.currentClothingWeapon;
        clothingWeapon.ItemsDictionary.Clear();
        foreach (var data in dados.equippedItems)
        {
            GenericItemScriptable item = itemDatabase.GetItemByID(data.itemID);
            if (item != null)
            {
                //clothingWeapon.AddItem(data.slotIndex, item);
                item.Equipado = data.equipado;

                if (data.equipado)
                {
                    PlayerView.instance.Equip(data.tipoEquip, true);
                }
            }
        }
    }*/
    void LoadWeaponList(Dados dados)
    {
        foreach (var weaponGO in PlayerView.instance.weaponGoList)
        {
            if (weaponGO.TryGetComponent(out Arma arma))
            {
                var match = dados.armasSalvas.Find(w => w.weaponName == weaponGO.name);
                if (match != null)
                {
                    arma.equipedWeapon.currentAmmo = match.municaoAtual;
                    arma.weaponData.municaoMaxima = match.maxMunicao;
                    arma.weaponData.municaoParaRecarregar = match.municaoParaRecarregar;
                }
            }
        }
    }

    #endregion

    #region Save e Load

    public void SaveGame()
    {
        Dados dados = new Dados();
        SaveStats(dados);
        SaveAudio(dados);
        //SaveInventory(dados);
        //SaveShortCut(dados);
        //SaveClothingWeapon(dados);
        SaveWeaponList(dados);

        string JsonData = JsonUtility.ToJson(dados);
        File.WriteAllText(Application.persistentDataPath + "Save.Json", JsonData); // Application.persistentDataPath e a pasta padrao da unity
    }

    public void LoadGame()
    {
        string JsonData = File.ReadAllText(Application.persistentDataPath + "Save.Json");
        Dados dados = JsonUtility.FromJson<Dados>(JsonData);

        LoadStats(dados);
        LoadAudio(dados);
        //LoadInventory(dados);
        //LoadShortChut(dados);
        //LoadClothingWeapon(dados);
        LoadWeaponList(dados);
    }

    #endregion
}

[Serializable]
class Dados
{
    public float dadosVida;
    public float dadosFome;
    public float dadosSede;
    public int dadosMunicao;
    public float volumeFx;
    public float volumeMusica;
    public List<ItemSaveData> itensSalvos = new List<ItemSaveData>(); // Itens do inventário
    public List<ShortcutData> shortcutSalvos = new List<ShortcutData>();  // Itens do shortcut
    public List<EquippedItemData> equippedItems = new List<EquippedItemData>(); // ITEMS CLOTHING WEAPON
    public List<ArmaSaveData> armasSalvas = new List<ArmaSaveData>(); // Lista de Armas
}

#region Serializable
// ITEMS INVENTARIO
[Serializable]
public class ItemSaveData
{
    public int itemID;
    public int quantidade;
}

// ITEMS SHORTCUT
[Serializable]
public class ShortcutData
{
    public int index;
    public int itemId;
}

//ITEMS CLOTHING WEAPON VIEW
[Serializable]
public class EquippedItemData
{
    public int slotIndex;
    public int itemID;
    public bool equipado;
    public ItemEquip tipoEquip;
}
[Serializable]
public class ArmaSaveData
{
    public string weaponName;
    public int municaoAtual;
    public int maxMunicao;
    public int municaoParaRecarregar;
}

#endregion