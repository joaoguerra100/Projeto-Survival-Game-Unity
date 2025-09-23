using System.Collections.Generic;
using UnityEngine;

public class ActionManagerController : MonoBehaviour
{
    #region Declaration

    [SerializeField]private List<GenericActionScriptable> currentActionList;

    #endregion

    #region Methods

    private void OnEnable()
    {
        ActionManagerEvent.SendActionListEvent += ReceiverActionList;
    }

    private void OnDisable()
    {
        ActionManagerEvent.SendActionListEvent -= ReceiverActionList;
    }

    private void ReceiverActionList(List<GenericActionScriptable> actionListReceived)
    {
        if(actionListReceived.Count > 0)
        {
            currentActionList.AddRange(actionListReceived);
            try
            {
                while (currentActionList.Count > 0) // enquanto tiver açoes para ocorrer ele executara as açoes abaixo
                {
                    StopCoroutine(currentActionList[0].Execute()); // VAI PARAR A PRIMEIRA CORROTINA DE AÇOES
                    StartCoroutine(currentActionList[0].Execute()); //EXECULTARA A PRIMEIRA CORROTINA DE AÇOES(ESTE PROCESSIMENTO E FEITO PARA QUE NAO ACABE FAZENDO A MESMA AÇAO 2X)
                    currentActionList.Remove(currentActionList[0]); // REMOVE A PRIMEIRA CORROTINA DA LISTA DE AÇOES QUANDO ELA TERMINAR TODAS AS AÇOES
                }
            }
            catch (System.Exception)
            {
                Debug.LogError("A Action List esta vazia ou faltando alguma açao");
            }
        }
        else
        {
            Debug.LogWarning("Nao tem nenhuma açao dentro da Action List atual");
        }
    }

    #endregion
    
    
}
