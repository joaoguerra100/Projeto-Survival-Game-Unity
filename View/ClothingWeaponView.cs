
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class ClothingWeaponView : MonoBehaviour
{
    #region Declaration

    [SerializeField] private GameObject clothingWeaponPainel;
    [SerializeField] private GameObject specialSlotGo;
    [SerializeField] private List<Transform> clothingWeaponSlotParents;
    [SerializeField] private GameObject auxCameraPreviwGo;
    //[SerializeField] private Transform player;
    [SerializeField] private ClothingWeaponScriptable currentClothingWeapon;
    private Dictionary<int, GameObject> specialSlotDictionary;
    public static ClothingWeaponView instance;

    #endregion

    #region Methods

    void Awake()
    {
        instance = this;
        clothingWeaponPainel.SetActive(true);
    }

    public void Iniciate(ClothingWeaponScriptable currentClothingWeapon)
    {
        this.currentClothingWeapon = currentClothingWeapon;
        IniciateClothingWeaponSlots(this.currentClothingWeapon.SlotNumber);
    }

    private void IniciateClothingWeaponSlots(int maxClothingWeaponSlot)
    {
        // Verifica se há painéis suficientes
        if (clothingWeaponSlotParents.Count < maxClothingWeaponSlot)
        {
            Debug.LogError("[ClothingWeaponView] Faltam painéis para instanciar todos os ClothingWeapon slots!");
            return;
        }

        specialSlotDictionary = new Dictionary<int, GameObject>();

        for (int index = 0; index < maxClothingWeaponSlot; index++)
        {
            GameObject resultGo = Instantiate(specialSlotGo);
            resultGo.tag = "ClothingWeaponSlot";

            // Define o parent correto
            resultGo.transform.SetParent(clothingWeaponSlotParents[index], false); // False preserva o layout do Grid

            // Define coordenadas internas (se necessário)
            var simpleSlot = resultGo.GetComponentInChildren<SimpleSlotView>();
            if (simpleSlot != null)
                simpleSlot.coodinate = new Vector2(0, index);

            specialSlotDictionary.Add(index, resultGo);
        }
    }

    public void RefreshSlotSystem(Dictionary<int, InventoryItem> itemsClothingWeaponsDictionary)
    {
        foreach (var item in specialSlotDictionary)
        {
            item.Value.GetComponent<DisplayItemBehaviorView>().TurnOff();
        }

        if (!itemsClothingWeaponsDictionary.Equals(null))
        {
            foreach (var element in itemsClothingWeaponsDictionary)
            {
                GameObject resultGo = SelectSpecialSlotByIndex(element.Key);
                resultGo.GetComponent<DisplayItemBehaviorView>().TurnOn();
                resultGo.GetComponentInChildren<ComplexSlotView>().ItemView = element.Value;
                resultGo.GetComponentInChildren<ComplexSlotView>().UpdateText();
                resultGo.GetComponentInChildren<ComplexSlotView>().UpdateIcon();
            }
        }
    }

    private GameObject SelectSpecialSlotByIndex(int index)
    {
        try
        {
            var result = specialSlotDictionary.First(element => element.Key == index);

            if (!(result.Equals(null)))
            {
                return result.Value;
            }
        }
        catch (System.Exception)
        {

            Debug.LogWarning("NAO TEM NENHUM SLOT COM ESTE ID");
        }

        return null;
    }

    public bool ShowAndHide()
    {
        clothingWeaponPainel.SetActive(!clothingWeaponPainel.activeSelf);
        //CallSpinRoutine();
        return clothingWeaponPainel.activeSelf;
    }

    private void CallSpinRoutine()
    {
        //StartCoroutine(SpinCamera());
    }

    /*IEnumerator SpinCamera()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.02f);
            //auxCameraPreviwGo.transform.RotateAround(player.position, Vector3.up, 200f * Time.deltaTime);
        }
    }*/


    #endregion
    

}
