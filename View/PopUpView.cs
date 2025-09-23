using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpView : MonoBehaviour
{
    #region Delcaration

    [SerializeField] private GameObject PopUpGo;
    private float delayToClose = 0.05f;

    public static PopUpView instance;

    #endregion

    #region Methods

    void Awake()
    {
        instance = this;
        PopUpGo.SetActive(false);
    }

    public void PopUpUpdate(string text, Sprite icon, Color textColor, Color backGroundColor, float timeToClosePopUp)
    {
        PopUpGo.SetActive(true);

        PopUpGo.GetComponentInChildren<TextMeshProUGUI>().text = text;
        PopUpGo.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = icon;
        PopUpGo.GetComponentInChildren<TextMeshProUGUI>().color = textColor;
        PopUpGo.GetComponent<Image>().color = backGroundColor;

        if (timeToClosePopUp != 0)
        {
            delayToClose = timeToClosePopUp;
            StartCoroutine(Close());
        }
    }

    IEnumerator Close()
    {
        yield return new WaitForSeconds(delayToClose);
        PopUpGo.SetActive(false);
    }
    
    #endregion
}
