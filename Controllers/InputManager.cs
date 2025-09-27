using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region Variaveis
    [Header("Referencias")]
    public static InputManager instance;

    [Header("Controle Do jogador")]
    public KeyCode forwardKey { get; private set; } = KeyCode.W;
    public KeyCode leftKey { get; private set; } = KeyCode.A;
    public KeyCode backKey { get; private set; } = KeyCode.S;
    public KeyCode rigthKey { get; private set; } = KeyCode.D;
    public KeyCode runKey { get; private set; } = KeyCode.LeftShift;
    public KeyCode interactKey { get; private set; } = KeyCode.E;
    public KeyCode collectKey { get; private set; } = KeyCode.E;
    public KeyCode jumpKey { get; private set; } = KeyCode.Space;
    public KeyCode agaichadoKey { get; private set; } = KeyCode.C;

    [Header("Combate")]
    public KeyCode fireKey { get; private set; } = KeyCode.Mouse0;
    public KeyCode aimKey { get; private set; } = KeyCode.Mouse1;
    public KeyCode attackKey { get; private set; } = KeyCode.Mouse0;
    public KeyCode blockKey { get; private set; } = KeyCode.Mouse1;
    public KeyCode switchFireModeKey { get; private set; } = KeyCode.B;
    public KeyCode reloadKey { get; private set; } = KeyCode.R;

    [Header("Interface")]
    public KeyCode menuKey { get; private set; } = KeyCode.Escape;
    public KeyCode inventoryKey { get; private set; } = KeyCode.Tab;

    [Header("Teclas De Atalho")]
    public KeyCode teclaDeAtalho1Key { get; private set; } = KeyCode.Alpha1;
    public KeyCode teclaDeAtalho2Key { get; private set; } = KeyCode.Alpha2;
    public KeyCode teclaDeAtalho3Key { get; private set; } = KeyCode.Alpha3;
    public KeyCode teclaDeAtalho4Key { get; private set; } = KeyCode.Alpha4;
    public KeyCode teclaDeAtalho5Key { get; private set; } = KeyCode.Alpha5;
    public KeyCode ligarDesligarLanternaKey { get; private set; } = KeyCode.F;

    [Header("Atalhos de movimento")]
    public float VerticalAxis { get; private set; }
    public float HorizontalAxis { get; private set; }
    private Dictionary<string, KeyCode> defaultKeys;

    /* Actions */
    public event Action OnInventoryKeyPressed;
    public event Action OnShortcut1Pressed;
    public event Action OnShortcut2Pressed;
    public event Action OnShortcut3Pressed;
    public event Action OnShortcut4Pressed;
    public event Action OnShortcut5Pressed;

    #endregion


    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        InitializeDefaultKeys();
        LoadKeys();
    }


    void Update()
    {
        if (Input.GetKeyDown(inventoryKey))
        {
            OnInventoryKeyPressed?.Invoke();
        }

        // Inputs de Atalho
        if (Input.GetKeyDown(teclaDeAtalho1Key)) OnShortcut1Pressed?.Invoke();
        if (Input.GetKeyDown(teclaDeAtalho2Key)) OnShortcut2Pressed?.Invoke();
        if (Input.GetKeyDown(teclaDeAtalho3Key)) OnShortcut3Pressed?.Invoke();
        if (Input.GetKeyDown(teclaDeAtalho4Key)) OnShortcut4Pressed?.Invoke();
        if (Input.GetKeyDown(teclaDeAtalho5Key)) OnShortcut5Pressed?.Invoke();

        // Eixo Horizontal (Esquerda/Direita)
        HorizontalAxis = 0f; // Reseta o eixo
        if (Input.GetKey(rigthKey))
        {
            HorizontalAxis += 1f;
        }
        if (Input.GetKey(leftKey))
        {
            HorizontalAxis -= 1f;
        }

        // Eixo Vertical (Frente/Trás)
        VerticalAxis = 0f; // Reseta o eixo
        if (Input.GetKey(forwardKey))
        {
            VerticalAxis += 1f;
        }
        if (Input.GetKey(backKey))
        {
            VerticalAxis -= 1f;
        }
    }

    private void InitializeDefaultKeys()
    {
        defaultKeys = new Dictionary<string, KeyCode>
        {
            /*Controle Do jogador  */
            { "Forward", KeyCode.W },
            { "Back", KeyCode.S },
            { "Left", KeyCode.A },
            { "Rigth", KeyCode.D },
            { "Run", KeyCode.LeftShift },
            { "Interact", KeyCode.E },
            { "Collect", KeyCode.E },
            { "Jump", KeyCode.Space },
            { "Agaichado", KeyCode.C },
            /*Combate  */
            { "Fire", KeyCode.Mouse0 },
            { "Aim", KeyCode.Mouse1 },
            { "Attack", KeyCode.Mouse0 },
            { "Block", KeyCode.Mouse1 },
            { "SwitchFireMode", KeyCode.B },
            { "Reload", KeyCode.R },
            /* Interface */
            { "Menu", KeyCode.Escape },
            { "Inventory", KeyCode.Tab },
            /* Teclas De Atalho */
            { "Atalho1", KeyCode.Alpha1 },
            { "Atalho2", KeyCode.Alpha2 },
            { "Atalho3", KeyCode.Alpha3 },
            { "Atalho4", KeyCode.Alpha4 },
            { "Atalho5", KeyCode.Alpha5 },
            { "LigarDesligarLanterna", KeyCode.F }
        };
    }

    public void ResetKeyToAction(string actionName)
    {
        if (defaultKeys.ContainsKey(actionName))
        {
            KeyCode defaultKey = defaultKeys[actionName];
            SetKeybinding(actionName, defaultKey);
        }
    }

    public void LoadKeys()
    {
        /* Controle Do jogador */
        string forwardKeyString = PlayerPrefs.GetString("ForwardKey", "W");
        forwardKey = (KeyCode)Enum.Parse(typeof(KeyCode), forwardKeyString);

        string backKeyKeyString = PlayerPrefs.GetString("BackKey", "S");
        backKey = (KeyCode)Enum.Parse(typeof(KeyCode), backKeyKeyString);

        string leftKeyKeyString = PlayerPrefs.GetString("LeftKeyKey", "A");
        leftKey = (KeyCode)Enum.Parse(typeof(KeyCode), leftKeyKeyString);

        string rigthKeyKeyString = PlayerPrefs.GetString("RigthKeyKey", "D");
        rigthKey = (KeyCode)Enum.Parse(typeof(KeyCode), rigthKeyKeyString);

        string runKeyString = PlayerPrefs.GetString("RunKey", "LeftShift");
        runKey = (KeyCode)Enum.Parse(typeof(KeyCode), runKeyString);

        string interactKeyString = PlayerPrefs.GetString("InteractKey", "E");
        interactKey = (KeyCode)Enum.Parse(typeof(KeyCode), interactKeyString);

        string collectKeyString = PlayerPrefs.GetString("CollectKey", "E");
        collectKey = (KeyCode)Enum.Parse(typeof(KeyCode), collectKeyString);

        string jumpKeyString = PlayerPrefs.GetString("JumpKey", "Space");
        jumpKey = (KeyCode)Enum.Parse(typeof(KeyCode), jumpKeyString);

        string agaichadoKeyString = PlayerPrefs.GetString("AgaichadoKey", "C");
        agaichadoKey = (KeyCode)Enum.Parse(typeof(KeyCode), agaichadoKeyString);

        /*Combate  */
        string fireKeyString = PlayerPrefs.GetString("FireKey", "Mouse0");
        fireKey = (KeyCode)Enum.Parse(typeof(KeyCode), fireKeyString);

        string aimKeyString = PlayerPrefs.GetString("AimKey", "Mouse1");
        aimKey = (KeyCode)Enum.Parse(typeof(KeyCode), aimKeyString);

        string attackKeyString = PlayerPrefs.GetString("AttackKey", "Mouse0");
        attackKey = (KeyCode)Enum.Parse(typeof(KeyCode), attackKeyString);

        string blockKeyString = PlayerPrefs.GetString("BlockKey", "Mouse1");
        blockKey = (KeyCode)Enum.Parse(typeof(KeyCode), blockKeyString);

        string switchFireModeKeyString = PlayerPrefs.GetString("SwitchFireModeKey", "B");
        switchFireModeKey = (KeyCode)Enum.Parse(typeof(KeyCode), switchFireModeKeyString);

        string reloadKeyString = PlayerPrefs.GetString("ReloadKey", "R");
        reloadKey = (KeyCode)Enum.Parse(typeof(KeyCode), reloadKeyString);


        /* Interface */
        string menuKeyString = PlayerPrefs.GetString("MenuKey", "Escape");
        menuKey = (KeyCode)Enum.Parse(typeof(KeyCode), menuKeyString);

        string inventoryKeyString = PlayerPrefs.GetString("inventoryKey", "Tab");
        inventoryKey = (KeyCode)Enum.Parse(typeof(KeyCode), inventoryKeyString);

        /* Teclas De Atalho */
        string atalho1KeyString = PlayerPrefs.GetString("TeclaDeAtalho1Key", "Alpha1");
        teclaDeAtalho1Key = (KeyCode)Enum.Parse(typeof(KeyCode), atalho1KeyString);

        string atalho2KeyString = PlayerPrefs.GetString("TeclaDeAtalho2Key", "Alpha2");
        teclaDeAtalho2Key = (KeyCode)Enum.Parse(typeof(KeyCode), atalho2KeyString);

        string atalho3KeyString = PlayerPrefs.GetString("TeclaDeAtalho3Key", "Alpha3");
        teclaDeAtalho3Key = (KeyCode)Enum.Parse(typeof(KeyCode), atalho3KeyString);

        string atalho4KeyString = PlayerPrefs.GetString("TeclaDeAtalho4Key", "Alpha4");
        teclaDeAtalho4Key = (KeyCode)Enum.Parse(typeof(KeyCode), atalho4KeyString);

        string atalho5KeyString = PlayerPrefs.GetString("TeclaDeAtalho5Key", "Alpha5");
        teclaDeAtalho5Key = (KeyCode)Enum.Parse(typeof(KeyCode), atalho5KeyString);

        string ligarDesligarLanternaKeyString = PlayerPrefs.GetString("ligarDesligarLanternaKey", "F");
        ligarDesligarLanternaKey = (KeyCode)Enum.Parse(typeof(KeyCode), ligarDesligarLanternaKeyString);
    }

    public void SaveKeys()
    {
        /* Controle Do jogador */
        PlayerPrefs.SetString("ForwardKey", forwardKey.ToString());
        PlayerPrefs.SetString("BackKey", backKey.ToString());
        PlayerPrefs.SetString("LeftKeyKey", leftKey.ToString());
        PlayerPrefs.SetString("RigthKeyKey", rigthKey.ToString());
        PlayerPrefs.SetString("RunKey", runKey.ToString());
        PlayerPrefs.SetString("InteractKey", interactKey.ToString());
        PlayerPrefs.SetString("CollectKey", collectKey.ToString());
        PlayerPrefs.SetString("JumpKey", jumpKey.ToString());
        PlayerPrefs.SetString("AgaichadoKey", agaichadoKey.ToString());

        /* Combate */
        PlayerPrefs.SetString("FireKey", fireKey.ToString());
        PlayerPrefs.SetString("AimKey", aimKey.ToString());
        PlayerPrefs.SetString("BlockKey", blockKey.ToString());
        PlayerPrefs.SetString("AttackKey", attackKey.ToString());
        PlayerPrefs.SetString("SwitchFireModeKey", switchFireModeKey.ToString());
        PlayerPrefs.SetString("ReloadKey", reloadKey.ToString());

        /* Interface */
        PlayerPrefs.SetString("MenuKey", menuKey.ToString());
        PlayerPrefs.SetString("inventoryKey", inventoryKey.ToString());

        /* Teclas De Atalho */
        PlayerPrefs.SetString("TeclaDeAtalho1Key", teclaDeAtalho1Key.ToString());
        PlayerPrefs.SetString("TeclaDeAtalho2Key", teclaDeAtalho2Key.ToString());
        PlayerPrefs.SetString("TeclaDeAtalho3Key", teclaDeAtalho3Key.ToString());
        PlayerPrefs.SetString("TeclaDeAtalho4Key", teclaDeAtalho4Key.ToString());
        PlayerPrefs.SetString("TeclaDeAtalho5Key", teclaDeAtalho5Key.ToString());
        PlayerPrefs.SetString("ligarDesligarLanternaKey", ligarDesligarLanternaKey.ToString());

        PlayerPrefs.Save();
    }

    public void SetKeybinding(string action, KeyCode newKey)
    {
        switch (action)
        {
            /* Controle Do jogador */
            case "Forward":
                forwardKey = newKey;
                break;
            case "Back":
                backKey = newKey;
                break;
            case "Left":
                leftKey = newKey;
                break;
            case "Rigth":
                rigthKey = newKey;
                break;
            case "Run":
                runKey = newKey;
                break;
            case "Interact":
                interactKey = newKey;
                break;
            case "Collect":
                collectKey = newKey;
                break;
            case "Jump":
                jumpKey = newKey;
                break;
            case "Agaichado":
                agaichadoKey = newKey;
                break;
            /* Combate */
            case "Fire":
                fireKey = newKey;
                break;
            case "Aim":
                aimKey = newKey;
                break;
            case "Attack":
                attackKey = newKey;
                break;
            case "Block":
                blockKey = newKey;
                break;
            case "SwitchFireMode":
                switchFireModeKey = newKey;
                break;
            case "Reload":
                reloadKey = newKey;
                break;
            /* Interface */
            case "Menu":
                menuKey = newKey;
                break;
            case "Inventory":
                inventoryKey = newKey;
                break;
            /* Teclas De Atalho */
            case "Atalho1":
                teclaDeAtalho1Key = newKey;
                break;
            case "Atalho2":
                teclaDeAtalho2Key = newKey;
                break;
            case "Atalho3":
                teclaDeAtalho3Key = newKey;
                break;
            case "Atalho4":
                teclaDeAtalho4Key = newKey;
                break;
            case "Atalho5":
                teclaDeAtalho5Key = newKey;
                break;
            case "LigarDesligarLanterna":
                ligarDesligarLanternaKey = newKey;
                break;
        }
        SaveKeys();
    }
}
