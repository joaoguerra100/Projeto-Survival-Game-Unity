using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComplexSlotView : MonoBehaviour
{

    #region Declaration
    [SerializeField]private InventoryItem itemView;
    [SerializeField] public Image icon;
    [SerializeField]public TextMeshProUGUI currentNumberText;
    private CanvasGroup canvasGroup;
    public GenericBagScriptable bagOrigem;

    public InventoryItem ItemData { get; private set; }
    public int inventoryIndex;

    public void Setup(InventoryItem item, int index, GenericBagScriptable origem)
    {
        this.ItemData = item;
        this.inventoryIndex = index;
        this.bagOrigem = origem;
    }

    public void Setup2(InventoryItem item, int index)
    {
        this.ItemData = item;
        this.inventoryIndex = index;
    }

    #endregion

    #region  Getting Setting
    public InventoryItem ItemView { get => itemView; set => itemView = value; }

    #endregion

    void Awake()
    {
        canvasGroup = this.gameObject.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = this.gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void UpdateText()
    {
        currentNumberText.text = ItemView.quantidade.ToString();
        EnableAndDisableIcon(ItemView.quantidade);
    }

    public void UpdateIcon()
    {
        icon.sprite = ItemView.baseItemData.IconHorizontal;
    }

    private void EnableAndDisableIcon(int value)
    {
        if (canvasGroup == null) return;
        
        if(value > 0)
        {
            canvasGroup.alpha = 1;
        }
        else
        {
            canvasGroup.alpha = 0.3f;
        }
    }
}
