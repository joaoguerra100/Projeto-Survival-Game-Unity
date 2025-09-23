using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    #region Declaration

    [Header("Script")]
    public static PlayerView instance;

    [Header("Weapon Icon")]
    [SerializeField] private Image currentWeaponIcon;
    [SerializeField] private List<Sprite> listIcon;

    [Header("Weapon Controller")]
    [SerializeField] private Animator currentRuntimeAnimatorController;
    [SerializeField] private List<RuntimeAnimatorController> RuntimeAnimatorController;

    [Header("List")]
    [SerializeField]public List<GameObject> weaponGoList;
    [SerializeField] List<GameObject> roupasGoList;

    private ItemEquip? currentEquippedWeapon = null; // Nullable enum


    #endregion

    #region Methods

    void Awake()
    {
        instance = this;
    }
    #endregion

    #region ARMAS
    public void Equip(ItemEquip typeWeapon, bool visible)
    {
        
        // Se a arma já estiver equipada e visível, desequipa
        if (currentEquippedWeapon == typeWeapon && StatusEquipment(typeWeapon))
        {
            SetWeaponActive(typeWeapon, false);
            currentEquippedWeapon = null;
            PlayerBracos.instance.Desarmar();
            ClearWeaponUI();
            return;
        }
        //Se nao tiver nenhuma arma ativa desarma o persongame
        if (typeWeapon == ItemEquip.NONE)
        {
            PlayerBracos.instance.Desarmar();
            ClearWeaponUI();
            currentEquippedWeapon = ItemEquip.NONE;
            return;
        }

        // Desativa qualquer arma atualmente equipada
        if (currentEquippedWeapon != null)
        {
            SetWeaponActive(currentEquippedWeapon.Value, false);
        }

        // Ativa a nova arma
        SetWeaponActive(typeWeapon, visible);
        currentEquippedWeapon = typeWeapon;
        CheckWeapon(typeWeapon);
        ProcurarScriptArma(typeWeapon);
    }

    void ProcurarScriptArma(ItemEquip typeWeapon)
    {
        // Procurar a Arma recém-ativada
        foreach (var item in weaponGoList)
        {
            if (item.name == typeWeapon.ToString())
            {
                Arma armaScript = item.GetComponent<Arma>();
                if (armaScript != null)
                {
                    PlayerBracos.instance.SetarArma(armaScript);
                }
                break;
            }
        }
    }
    private void SetWeaponActive(ItemEquip typeWeapon, bool active)
    {
        foreach (var item in weaponGoList)
        {
            if (item.name == typeWeapon.ToString())
            {
                item.SetActive(active);
                return;
            }
        }
    }

    private void ClearWeaponUI()
    {
        if (listIcon.Count > 0)
        {
            currentWeaponIcon.sprite = listIcon[0];
        }
        if (RuntimeAnimatorController.Count > 0)
        {
            currentRuntimeAnimatorController.runtimeAnimatorController = RuntimeAnimatorController[0];
        }
    }

    public bool StatusEquipment(ItemEquip typeWeapon)
    {
        foreach (var item in weaponGoList)
        {
            if (item.name == typeWeapon.ToString())
            {
                return item.activeSelf;
            }
        }
        return false;
    }

    private void CheckWeapon(ItemEquip typeWeapon)
    {
        switch (typeWeapon)
        {
            case ItemEquip.AK_47:
                ChangeWeapon(ItemEquip.AK_47);
                break;
            case ItemEquip.PISTOLA_SK:
                ChangeWeapon(ItemEquip.PISTOLA_SK);
                break;
            case ItemEquip.Mp5:
                ChangeWeapon(ItemEquip.Mp5);
                break;
            case ItemEquip.M4A1:
                ChangeWeapon(ItemEquip.M4A1);
                break;
            case ItemEquip.Ak_12:
                ChangeWeapon(ItemEquip.Ak_12);
                break;
            case ItemEquip.Scar_L:
                ChangeWeapon(ItemEquip.Scar_L);
                break;
            case ItemEquip.M1911:
                ChangeWeapon(ItemEquip.M1911);
                break;
            case ItemEquip.Walther_PP:
                ChangeWeapon(ItemEquip.Walther_PP);
                break;
            case ItemEquip.As_50:
                ChangeWeapon(ItemEquip.As_50);
                break;
            case ItemEquip.AWM:
                ChangeWeapon(ItemEquip.AWM);
                break;
            case ItemEquip.Mossberg590:
                ChangeWeapon(ItemEquip.Mossberg590);
                break;
            case ItemEquip.REVOLVER:
                ChangeWeapon(ItemEquip.REVOLVER);
                break;
            case ItemEquip.CROSSBOW:
                ChangeWeapon(ItemEquip.CROSSBOW);
                break;
            case ItemEquip.Minibea_Pm_9:
                ChangeWeapon(ItemEquip.Minibea_Pm_9);
                break;
        }
    }

    private void ChangeWeapon(ItemEquip typeWeapon)
    {
        int index = (int)typeWeapon;
        currentWeaponIcon.sprite = listIcon[index];
        currentRuntimeAnimatorController.runtimeAnimatorController = RuntimeAnimatorController[index];
    }

    #endregion

    #region ROUPAS
    public bool StatusEquipmentRoupa(RoupaEquip typeroupa)
    {
        foreach (var item in roupasGoList)
        {
            if (item.name == typeroupa.ToString())
            {
                return item.activeSelf;
            }
        }

        return false;
    }

    public void EquipRoupa(RoupaEquip typeroupa, bool visible)
    {
        foreach (var item in weaponGoList)
        {
            if (item.name == typeroupa.ToString())
            {
                item.SetActive(visible);
            }
        }
    }

    #endregion
    
}
