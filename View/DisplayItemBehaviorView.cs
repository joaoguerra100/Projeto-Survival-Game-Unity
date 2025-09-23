using UnityEngine;

public class DisplayItemBehaviorView : MonoBehaviour
{

    #region Declaration

    [SerializeField]private GameObject simpleSlotGo;
    [SerializeField]private GameObject complexSlotGo;

    #endregion

    #region Methods

    void OnEnable()
    {
        TurnOff();
    }

    public void TurnOn()
    {
        complexSlotGo.SetActive(true);
    }

    public void TurnOff()
    {
        complexSlotGo.SetActive(false);
    }

    #endregion
    
}
