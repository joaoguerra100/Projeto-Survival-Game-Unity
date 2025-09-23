using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSlotFilter", menuName = "Inventory/Filters/NewSlotFilter", order = 1)]
public class SlotFilterScriptable : ScriptableObject
{

    #region Declaration

    [SerializeField][Range(0, 10)]private int indexFrom; // DAONDE ELE VEM
    [SerializeField][Range(0, 10)]private int indexTo; //DAONDE ELE VAI

    #endregion

    #region Methods

    void OnEnable()
    {
        if (indexFrom > indexTo)
        {
            indexFrom = indexTo;
        }
    }

    public List<int> GetAllIndex()
    {
        List<int> indexRange = new List<int>();

        if (indexFrom - indexTo != 0)
        {
            for (int i = indexFrom; i < indexTo; i++)
            {
                indexRange.Add(i);
            }
        }
        else
        {
            indexRange.Add(indexFrom);
        }

        return indexRange;
    }
    #endregion

}
