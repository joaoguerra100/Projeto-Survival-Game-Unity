using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipeAction", menuName = "Action/NewEquipeAction")]
public class EquipeActionScriptable : GenericActionScriptable
{
    [SerializeField]private ItemEquip itemEquip;
    public override IEnumerator Execute()
    {
        yield return new WaitForSeconds(DelayToStart);
        GameController.instance.EquipChar(itemEquip);
    }
    
}
