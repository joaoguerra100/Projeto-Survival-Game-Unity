using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemActionPanelView : MonoBehaviour
{
    public static ItemActionPanelView instance;
    [SerializeField] private GameObject buttonPrefab;

    void Awake()
    {
        instance = this;
    }

    public void AddButon(string name, Action onClickAction)
    {
        GameObject button = Instantiate(buttonPrefab, transform);
        button.GetComponent<Button>().onClick.AddListener(() => onClickAction());
        button.GetComponentInChildren<TMPro.TMP_Text>().text = name;
    }

    public void Toggle(bool val)
    {
        if (val == true)
        {
            RemoveOldButtons();
        }
        gameObject.SetActive(val);
    }

    public void RemoveOldButtons()
    {
        foreach (Transform transformChildObjects in transform)
        {
            Destroy(transformChildObjects.gameObject);
        }
    }
    
    
}
