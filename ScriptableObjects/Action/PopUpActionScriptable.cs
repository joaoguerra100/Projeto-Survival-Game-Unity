using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPopUpAction", menuName = "Action/NewPopUpAction")]
public class PopUpActionScriptable : GenericActionScriptable
{
    [SerializeField][TextArea(5,8)]private string description;
    [SerializeField]private Sprite icon;
    [SerializeField]private Color textColor;
    [SerializeField]private Color backGroundColor;
    [SerializeField][Range(0,7)]float timeToClosePopup;

    public override IEnumerator Execute()
    {
        yield return new WaitForSeconds(DelayToStart);

        GameController.instance.ShowPopUp(description, icon, textColor, backGroundColor, timeToClosePopup);
    }


}
