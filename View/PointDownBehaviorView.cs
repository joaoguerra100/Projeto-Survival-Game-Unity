using UnityEngine;
using UnityEngine.EventSystems;

public class PointDownBehaviorView : MonoBehaviour, IPointerDownHandler
{
    private ComplexSlotView complexSlotSpt;

    void Awake()
    {
        complexSlotSpt = this.gameObject.GetComponent<ComplexSlotView>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            InventoryItem item = complexSlotSpt.ItemView;
            InventoryManagerController.instance.OnPointerDownItem(item, this.gameObject);
            ItemActionPanelView.instance.Toggle(false);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            int id = complexSlotSpt.ItemView.baseItemData.Id;
            string idUnico = complexSlotSpt.ItemView.instanceID;
            InventoryManagerController.instance.UseItemAction(id,idUnico);
        }
    }
}
