using UnityEngine;
using UnityEngine.EventSystems;

public class DropZoneBehaviorView : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        RectTransform invPanel = transform as RectTransform;

        if (RectTransformUtility.RectangleContainsScreenPoint(invPanel, Input.mousePosition))
        {
            GameObject gameObjectResult = eventData.pointerDrag;
            GameObject tempGameobject = eventData.pointerEnter;

            ComplexSlotView complexSlot = gameObjectResult.GetComponent<ComplexSlotView>();
            if (complexSlot == null)
            {
                //Debug.LogWarning("O objeto arrastado não é um slot de item válido porque não contém o componente 'ComplexSlotView'.");
                return;
            }
            InventoryItem itemResult = complexSlot.ItemView;
            if (itemResult == null)
            {
                //Debug.LogWarning($"[OndDropZoneItem]Item Nao Existe ou e nulo");
                return;
            }
            
            Vector2 coordinate = this.gameObject.GetComponent<SimpleSlotView>().coodinate;
            SlotPraceTo slotPraceTo = this.gameObject.GetComponent<SimpleSlotView>().slotPlaceteTo;

            var origemBag = gameObjectResult.GetComponent<ComplexSlotView>().bagOrigem;
            ComplexSlotGroupIdentifier identifier = tempGameobject.GetComponentInParent<ComplexSlotGroupIdentifier>();

            bool result = InventoryManagerController.instance.OnDropItem(itemResult,gameObjectResult,coordinate,slotPraceTo);
            if (result)
            {
                //Debug.Log($"o item {itemResult.baseItemData.Label} Foi aceito");
            }

            if (identifier != null)
            {
                var destinoBag = identifier.bag;

                if (destinoBag == null || itemResult == null)
                    return;

                // Dimensões do item (considerando se foi rotacionado ou não)
                int width = itemResult.baseItemData.itemSize.x;
                int height = itemResult.baseItemData.itemSize.y;

                // Verifica se foi rotacionado visualmente durante o drag
                bool isRotated = itemResult.isRotated;
                if (isRotated)
                {
                    int temp = width;
                    width = height;
                    height = temp;
                }
                // Verifica se a área está livre na bag de destino
                bool isFree = destinoBag.matrix.IsAreaFree(coordinate, width, height);

                if (isFree)
                {
                    if (origemBag != null)
                    {
                        origemBag.RemoverItem(itemResult.baseItemData.Id, itemResult.instanceID);
                    }
                    else
                    {
                        if (gameObjectResult.transform.parent.parent.name == "LootTablePainel")
                        {
                            bool removido  = Player.instance.currentContainerAberto.RemoverItemDoContainer(itemResult.instanceID,gameObjectResult);
                            
                            if (removido)
                            {
                                //Debug.Log($"Item Removido{itemResult.baseItemData.Label}");
                            }
                            else
                            {
                                Debug.LogWarning($"Item Nao Removido{itemResult.baseItemData.Label}");
                                return;
                            }
                            
                        }
                        else
                        {
                            NearbyItemsView.intance.RemoverItemLooteado(itemResult.instanceID);
                        }
                    }

                    // Coloca manualmente na nova posição
                    destinoBag.matrix.SetItemFromStart(coordinate, width, height, itemResult.baseItemData.Id);
                    destinoBag.inventoryItemsList.Add(itemResult);
                    FireWeaponInstance fireWeaponInstance = itemResult.baseItemData is WeaponItemScriptable ? new FireWeaponInstance((WeaponItemScriptable)itemResult.baseItemData, itemResult.instanceID) : null;
                    if (fireWeaponInstance != null)
                    {
                        destinoBag.weaponList.Add(fireWeaponInstance);
                    }
                    itemResult.bagOwner = destinoBag;

                    // Atualiza pesos e slots
                    destinoBag.UsedOrganizeBtSizePryority = false;
                    destinoBag.UpdateSizeAndWeigth();

                    InventoryManagerController.instance.UpdateCurrentWeigthChar();
                    StartCoroutine(InventoryManagerController.instance.RefreshInventoryView());

                    //Debug.Log($"Item {itemResult.baseItemData.Label} movido com sucesso para bag {destinoBag.name} na posição {coordinate}.");
                }
                else
                {
                    Debug.LogWarning("Espaço ocupado. Não foi possível soltar o item aqui.");
                }
            }
        }
    }
}
