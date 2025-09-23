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
        canvasIconeGo = GameObject.FindGameObjectWithTag("CanvasIcone");
    }

    void Start()
    {
        
    }


    void Update()
    {
        if (isDragging && Input.GetKeyDown(KeyCode.R))
        {
            RotateItem();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            Destroy(iconeGo);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        StarDrag();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (iconeGo != null)
        {
            iconeGo.transform.position = Input.mousePosition;
        }

        iconeGo.gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        StopDrag();
    }

    private void StarDrag()
    {
        iconeGo = new GameObject("icone", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
        iconRect = iconeGo.GetComponent<RectTransform>();
        iconRect.SetParent(canvasIconeGo.transform, false);
        iconRect.localScale = Vector3.one;
        iconRect.anchorMin = new Vector2(0.5f, 0.5f);
        iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);

        // Get item info
        InventoryItem item = GetComponent<ComplexSlotView>().ItemView;
        if (item == null) return;
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
        if (item.baseItemData.itemSize.x < item.baseItemData.itemSize.y)
        {
            if (!item.isRotated)
            {
                //img.sprite = item.baseItemData.IconVertical;
                iconRect.localRotation = Quaternion.Euler(0, 0, 90f);
                iconRect.sizeDelta = new Vector2(originalSize2.x, originalSize.y);
            }
            else
            {
                //img.sprite = item.baseItemData.IconHorizontal; // imagem deitada
                iconRect.localRotation = Quaternion.Euler(0, 0, 0);
                iconRect.sizeDelta = new Vector2(originalSize.y, originalSize.x);
            }
        }
        else
        {
            if (item.isRotated)
            {
                //img.sprite = item.baseItemData.IconVertical;
                iconRect.localRotation = Quaternion.Euler(0, 0, 90f);
                iconRect.sizeDelta = new Vector2(originalSize2.x, originalSize.y);
            }
            else
            {
                //img.sprite = item.baseItemData.IconHorizontal; // imagem deitada
                iconRect.localRotation = Quaternion.Euler(0, 0, 0);
                iconRect.sizeDelta = new Vector2(originalSize.y, originalSize.x);
            }
        }
            
    }

    private void RotateItem()
    {
        InventoryItem item = GetComponent<ComplexSlotView>().ItemView;
        Image img = iconeGo.GetComponent<Image>();
        if (iconeGo == null || item == null) return;

        item.isRotated = !item.isRotated;

        if (item.baseItemData.itemSize.x < item.baseItemData.itemSize.y)
        {
            if (!item.isRotated)
            {
                //img.sprite = item.baseItemData.IconVertical;   // imagem em pé
                iconRect.localRotation = Quaternion.Euler(0, 0, 90f);
                iconRect.sizeDelta = new Vector2(originalSize2.x, originalSize2.y);
            }
            else
            {
                //img.sprite = item.baseItemData.IconHorizontal; // imagem deitada
                iconRect.localRotation = Quaternion.Euler(0, 0, 0f);
                iconRect.sizeDelta = new Vector2(originalSize.y, originalSize.x);
            }
        }
        else
        {
            if (item.isRotated)
            {
                //img.sprite = item.baseItemData.IconVertical;   // imagem em pé
                iconRect.localRotation = Quaternion.Euler(0, 0, 90f);
                iconRect.sizeDelta = new Vector2(originalSize2.x, originalSize2.y);
            }
            else
            {
                //img.sprite = item.baseItemData.IconHorizontal; // imagem deitada
                iconRect.localRotation = Quaternion.Euler(0, 0, 0f);
                iconRect.sizeDelta = new Vector2(originalSize.y, originalSize.x);
            }
        }
    }



    private void StopDrag()
    {
        Destroy(iconeGo, 0.05f);
    }



    #endregion
}
