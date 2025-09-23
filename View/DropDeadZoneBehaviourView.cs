using UnityEngine;
using UnityEngine.EventSystems;

public class DropDeadZoneBehaviourView : MonoBehaviour, IDropHandler
{
    #region  Methods
    public void OnDrop(PointerEventData eventData)
    {
        RectTransform invPanel = transform as RectTransform;
        if (RectTransformUtility.RectangleContainsScreenPoint(invPanel, Input.mousePosition))
        {
            GameObject gameObjectResult = eventData.pointerDrag;
            ComplexSlotView complexSlot = gameObjectResult.GetComponent<ComplexSlotView>();
            if (complexSlot == null)
            {
                //Debug.LogWarning("O objeto arrastado não é um slot de item válido porque não contém o componente 'ComplexSlotView'.");
                return;
            }
            InventoryItem item = complexSlot.ItemView;
            if (item == null)
            {
                //Debug.LogWarning($"[OndDropZoneItem]Item Nao Existe ou e nulo");
                return;
            }
            string originTag = LookForTag(gameObjectResult);
            bool result = InventoryManagerController.instance.OnDropItem(originTag, item);
            if (item == null)
            {
                Debug.LogWarning($"[OndDropItem]Item Nao Existe{item.baseItemData.Label}");
                return;
            }
            if (result)
            {
                //Debug.Log($"[OndDropItem]Item dropado{item.baseItemData.Label}");
            }
            else
            {
                Debug.LogWarning($"[OndDropItem]Item Nao dropado{item.baseItemData.Label}");
                return;
            }
            StartCoroutine(InventoryManagerController.instance.RefreshInventoryView());
        }
    }

    private string LookForTag(GameObject obj)
    {
        //INVENTARIO
        if (obj.tag.Equals("ComplexSlot"))
        {
            return "ComplexSlot";
        }

        //SHORTCUT
        if (obj.transform.parent.tag.Equals("SpecialSlot"))
        {
            return "SpecialSlot";
        }

        //CLOTHING WEAPON SLOT
        if (obj.transform.parent.tag.Equals("ClothingWeaponSlot"))
        {
            return "ClothingWeaponSlot";
        }

        return "Untagged";
    }
    
    #endregion


}
