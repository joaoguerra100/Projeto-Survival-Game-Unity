using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragBehaviourView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{


    #region Declaration

    private CanvasGroup canvasGroupCg;
    private GameObject canvasIconeGo;
    private GameObject iconeGo;
    private bool isDragging = false;
    private Vector2 originalSize;
    private Vector2 originalSize2;
    private RectTransform iconRect;

    #endregion

    #region Methods

    void Awake()
    {
        canvasGroupCg = this.gameObject.GetComponent<CanvasGroup>();
        var canvasObj = GameObject.FindGameObjectWithTag("CanvasIcone");
        if (canvasObj != null)
        {
            canvasIconeGo = canvasObj;
        }
    }

    void OnDisable()
    {
        if (isDragging)
        {
            StopDrag();
        }

        if (iconeGo != null)
        {
            Destroy(iconeGo);
        }
        isDragging = false;
    }

    void Update()
    {
        if (isDragging && Input.GetKeyDown(KeyCode.R))
        {
            RotateItem();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasIconeGo == null) return;

        isDragging = true;
        StarDrag();

        if (canvasGroupCg != null) canvasGroupCg.alpha = 0.3f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (iconeGo != null)
        {
            iconeGo.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        StopDrag();

        if (canvasGroupCg != null) canvasGroupCg.alpha = 1f;
    }

    private void StarDrag()
    {
        iconeGo = new GameObject("icone", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
        iconRect = iconeGo.GetComponent<RectTransform>();
        if (canvasIconeGo != null)
        {
            iconRect.SetParent(canvasIconeGo.transform, false);
        }

        iconRect.localScale = Vector3.one;
        iconRect.anchorMin = new Vector2(0.5f, 0.5f);
        iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);

        InventoryItem item = GetComponent<ComplexSlotView>().ItemView;
        if (item == null)
        {
            StopDrag();
            return;
        }
        Vector2Int size = item.baseItemData.itemSize; // Ex: (1,2)
        GridLayoutGroup grid = InventoryView.instance.slotGroup[0].GetComponent<GridLayoutGroup>();
        Vector2 cellSize = grid.cellSize;

        originalSize = new Vector2(cellSize.x * size.x, cellSize.y * size.y);
        originalSize2 = new Vector2(cellSize.x * size.y, cellSize.y * size.x);
        iconRect.sizeDelta = originalSize;

        Image img = iconeGo.GetComponent<Image>();
        img.sprite = item.baseItemData.IconHorizontal;
        img.raycastTarget = false;
        img.preserveAspect = true;
        iconeGo.GetComponent<CanvasGroup>().alpha = 0.65f;
        ApplyRotationVisuals(item);
    }

    private void RotateItem()
    {
        InventoryItem item = GetComponent<ComplexSlotView>().ItemView;
        if (iconeGo == null || item == null) return;
        item.isRotated = !item.isRotated;
        ApplyRotationVisuals(item);
    }

    private void ApplyRotationVisuals(InventoryItem item)
    {
        if (iconRect == null) return;

        if (item.baseItemData.itemSize.x < item.baseItemData.itemSize.y)
        {
            if (!item.isRotated)
            {
                iconRect.localRotation = Quaternion.Euler(0, 0, 90f);
                iconRect.sizeDelta = new Vector2(originalSize2.x, originalSize.y); // Atenção aqui, ajustei para usar originalSize
            }
            else
            {
                iconRect.localRotation = Quaternion.Euler(0, 0, 0);
                iconRect.sizeDelta = new Vector2(originalSize.y, originalSize.x);
            }
        }
        else
        {
            // Logica para itens largos... (mantido igual ao seu)
            if (item.isRotated)
            {
                iconRect.localRotation = Quaternion.Euler(0, 0, 90f);
                iconRect.sizeDelta = new Vector2(originalSize2.x, originalSize2.y);
            }
            else
            {
                iconRect.localRotation = Quaternion.Euler(0, 0, 0f);
                iconRect.sizeDelta = new Vector2(originalSize.y, originalSize.x);
            }
        }
    }



    private void StopDrag()
    {
        if (iconeGo != null)
        {
            Destroy(iconeGo);
            iconeGo = null;
        }
    }
    #endregion
}
