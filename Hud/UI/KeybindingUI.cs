using UnityEngine;
using UnityEngine.UI; // Para usar Text
using System.Collections;
using TMPro; // Para usar Coroutines

public class KeybindingUI : MonoBehaviour
{
    [Header("Scroll")]
    public ScrollRect myScrollRect;
    public float scrollStep = 0.2f; // Velocidade da rolagem ao clicar na seta
    [Header("Controle Do jogador")]
    public TextMeshProUGUI forwardKeyText;
    public TextMeshProUGUI leftKeyText;
    public TextMeshProUGUI backKeyText;
    public TextMeshProUGUI rigthKeyText;
    public TextMeshProUGUI runKeyText;
    public TextMeshProUGUI interactKeyText;
    public TextMeshProUGUI collectKeyText;
    public TextMeshProUGUI agaichadoKeyText;
    public TextMeshProUGUI jumpKeyText;

    [Header("Combate")]
    public TextMeshProUGUI fireKeyText;
    public TextMeshProUGUI aimKeyText;
    public TextMeshProUGUI attackKeyText;
    public TextMeshProUGUI blockKeyText;
    public TextMeshProUGUI switchFireModeKeyText;
    public TextMeshProUGUI reloadKeyText;

    [Header("Interface")]
    public TextMeshProUGUI menuKeyText;
    public TextMeshProUGUI inventoryKeyText;

    [Header("Teclas De Atalho")]
    public TextMeshProUGUI teclaDeAtalho1KeyText;
    public TextMeshProUGUI teclaDeAtalho2KeyText;
    public TextMeshProUGUI teclaDeAtalho3KeyText;
    public TextMeshProUGUI teclaDeAtalho4KeyText;
    public TextMeshProUGUI teclaDeAtalho5KeyText;
    public TextMeshProUGUI ligarDesligarLanternaKeyText;

    [Header("Painel de Remapeamento")]
    public GameObject rebindPopupPanel;
    public TextMeshProUGUI rebindActionText;

    private string friendlyActionName = null;
    private string actionToRebind = null;

    void Start()
    {

        rebindPopupPanel.SetActive(false);
        UpdateKeyTexts();
    }

    public void UpdateKeyTexts()
    {
        /* Controle Do jogador */
        forwardKeyText.text = InputManager.instance.forwardKey.ToString();
        backKeyText.text = InputManager.instance.backKey.ToString();
        leftKeyText.text = InputManager.instance.leftKey.ToString();
        rigthKeyText.text = InputManager.instance.rigthKey.ToString();
        runKeyText.text = InputManager.instance.runKey.ToString();
        interactKeyText.text = InputManager.instance.interactKey.ToString();
        collectKeyText.text = InputManager.instance.collectKey.ToString();
        jumpKeyText.text = InputManager.instance.jumpKey.ToString();
        agaichadoKeyText.text = InputManager.instance.agaichadoKey.ToString();

        /* Combate */
        fireKeyText.text = InputManager.instance.fireKey.ToString();
        aimKeyText.text = InputManager.instance.aimKey.ToString();
        attackKeyText.text = InputManager.instance.attackKey.ToString();
        blockKeyText.text = InputManager.instance.blockKey.ToString();
        switchFireModeKeyText.text = InputManager.instance.switchFireModeKey.ToString();
        reloadKeyText.text = InputManager.instance.reloadKey.ToString();

        /* Interface */
        menuKeyText.text = InputManager.instance.menuKey.ToString();
        inventoryKeyText.text = InputManager.instance.inventoryKey.ToString();

        /* Teclas De Atalho */
        teclaDeAtalho1KeyText.text = InputManager.instance.teclaDeAtalho1Key.ToString();
        teclaDeAtalho2KeyText.text = InputManager.instance.teclaDeAtalho2Key.ToString();
        teclaDeAtalho3KeyText.text = InputManager.instance.teclaDeAtalho3Key.ToString();
        teclaDeAtalho4KeyText.text = InputManager.instance.teclaDeAtalho4Key.ToString();
        teclaDeAtalho5KeyText.text = InputManager.instance.teclaDeAtalho5Key.ToString();
        ligarDesligarLanternaKeyText.text = InputManager.instance.ligarDesligarLanternaKey.ToString();

    }

    private void OnGUI()
    {
        // Se actionToRebind NÃO for nulo, significa que estamos esperando uma tecla.
        if (actionToRebind != null)
        {
            // Verifica se o evento atual é um evento de teclado
            if (Event.current.isKey && Event.current.keyCode != KeyCode.None)
            {
                KeyCode newKey = Event.current.keyCode;

                //Debug.Log("Tecla Pressionada Detectada: " + newKey);

                // Diz ao InputManager para definir a nova tecla
                InputManager.instance.SetKeybinding(actionToRebind, newKey);

                CloseRebindPanel();
            }
        }
    }

    private void CloseRebindPanel()
    {
        rebindPopupPanel.SetActive(false);
        actionToRebind = null; // Limpa a flag para parar de ouvir
        friendlyActionName = null;
        UpdateKeyTexts(); // Atualiza a lista principal de teclas
    }

    #region OnClick

    // Este método é chamado pelo OnClick do botão "Mudar Ataque"
    public void StartRebinding(string actionName)
    {
        actionToRebind = actionName;
        //friendlyActionName = friendlyName;
        rebindActionText.text = $"Pressione qualquer tecla para atribuir a '{actionName}', ou pressione um desses botões";
        rebindPopupPanel.SetActive(true);
        //Debug.Log("Esperando por uma tecla para a ação: " + actionName);
    }

    public void AssignNoneKey()
    {
        InputManager.instance.SetKeybinding(actionToRebind, KeyCode.None);
        CloseRebindPanel();
    }

    public void AssignDefaultKey()
    {
        InputManager.instance.ResetKeyToAction(actionToRebind);
        CloseRebindPanel();
    }

    public void CancelRebind()
    {
        CloseRebindPanel();
    }
    
    #endregion
    
    #region Scroll

    public void ScrollUp()
    {
        if (myScrollRect == null) return;

        // Aumenta a posição normalizada para mover para cima
        myScrollRect.verticalNormalizedPosition += scrollStep;

        // Garante que o valor não passe de 1 (topo)
        myScrollRect.verticalNormalizedPosition = Mathf.Clamp01(myScrollRect.verticalNormalizedPosition);
    }

    public void ScrollDown()
    {
        if (myScrollRect == null) return;

        // Diminui a posição normalizada para mover para baixo
        myScrollRect.verticalNormalizedPosition -= scrollStep;

        // Garante que o valor não passe de 0 (base)
        myScrollRect.verticalNormalizedPosition = Mathf.Clamp01(myScrollRect.verticalNormalizedPosition);
    }

    #endregion
}