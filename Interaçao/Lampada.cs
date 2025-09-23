using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Lampada : MonoBehaviour, InterfaceInteracao
{
    [Serializable]
    public class LampConfigurations
    {
        public GameObject[] Lights;
        public Renderer[] EmissiveObjects;
        [HideInInspector] public Color[] EmissColors;
        public bool LightsON = true;
        public float _timer = 0.5f;
    }
    [Serializable]
    public class LampControls
    {
        public KeyCode switchButton = KeyCode.E; //Button on the keyboard to open the door
    }
    [Serializable]
    public class AnimNames //names of the animations, which you use for every action
    {
        public string AnimName = "LightSwitch";
    }
    [Serializable]
    public class LampSounds
    {
        public AudioClip SwitchSound;
        [Range(0, 1)]
        public float Volume = 1;

    }
    [Serializable]
    public class LampTexts
    {
        public bool enabled = true;
        public string ligarText = "Aperte [BUTTON] para ligar";
        public string desligarText = "Aperte [BUTTON] para desligar";
        public GameObject TextPrefab;
    }
    public LampConfigurations Configurations = new LampConfigurations();
    public LampControls Controls = new LampControls();
    public AnimNames animNames = new AnimNames();
    public LampSounds lampSounds = new LampSounds();
    public LampTexts lampTexts = new LampTexts();
    public KeyCode BotaoDeInteracao => Controls.switchButton;


    /* [Header("UI Botao de Interaçao")]
    public Transform btnInteracaoTransform;
    [SerializeField] private TextMeshProUGUI btnTxt;
    [SerializeField] private float baseScaleBtnInteracao;
    [SerializeField] private float scaleMultiplierBtnInteracao; */

    private AudioSource audioSource;
    private Animation Anim;
    private Canvas TextObj;
    private Text theText;

    #region Metodos iniciais

    void Awake()
    {
        AddAudioSource();
        Anim = GetComponent<Animation>();
    }
    void Start()
    {
        AddText();
        foreach (GameObject _light in Configurations.Lights)
        {
            if (_light)
            {
                if (Configurations.LightsON)
                {
                    _light.SetActive(true);

                }
                else
                {
                    _light.SetActive(false);

                }
            }
        }
        if (Configurations.LightsON)
        {
            if (Anim != null)
            {
                Anim[animNames.AnimName].normalizedTime = 0;
                Anim[animNames.AnimName].speed = -1;
                Anim.Play();
            }
        }
        else
        {
            if (Anim != null)
            {
                Anim[animNames.AnimName].normalizedTime = 1;
                Anim[animNames.AnimName].speed = 1;
                Anim.Play();
            }
        } 

        Configurations.EmissColors = new Color[Configurations.EmissiveObjects.Length];

        for (int i = 0; i < Configurations.EmissiveObjects.Length; i++)
        {
            Configurations.EmissColors[i] = Configurations.EmissiveObjects[i].material.GetColor("_EmissionColor");
        }

        if (!Configurations.LightsON)
        {
            DisableEmission();
        }
        else
        {
            StartCoroutine(EnableEmissionLate());
            //Debug.Log("Emission enabled");
        }
        
    }

    void Update()
    {
        // SeguirJogadorBotao();
    }

    #endregion

    /* void SeguirJogadorBotao()
    {
        if (!btnInteracaoTransform.gameObject.activeSelf) return;

        Transform cam = Camera.main.transform;

        // Faz a imagem olhar para a câmera (somente rotação)
        Vector3 lookDir = btnInteracaoTransform.position - cam.position;
        btnInteracaoTransform.rotation = Quaternion.LookRotation(lookDir);

        // Ajusta a escala com base na distância, se quiser manter legível
        float distance = Vector3.Distance(cam.position, btnInteracaoTransform.position);
        float finalScale = baseScaleBtnInteracao * (distance * scaleMultiplierBtnInteracao);
        btnInteracaoTransform.localScale = new Vector3(finalScale, finalScale, finalScale);
    } */

    #region Ligar/Desligar

    public void Interagir()
    {
        if (Configurations.LightsON)
        {
            Light_Off();
            DisableEmission();
            
        }

        else
        {
            Light_On();
            EnableEmission();
        }

    }

    void Light_Off()
    {
        foreach (GameObject _light in Configurations.Lights)
        {
            if (_light)
            {
                _light.SetActive(false);
            }
        }
        Configurations.LightsON = false;
        if (Anim != null)
        {
            Anim[animNames.AnimName].normalizedTime = 1;
            Anim[animNames.AnimName].speed = 1;
            Anim.Play();
        }
        if (lampSounds.SwitchSound != null)
        {
            audioSource.Play();
        }
    }

    void Light_On()
    {
        foreach (GameObject _light in Configurations.Lights)
        {
            if (_light)
            {
                _light.SetActive(true);
            }
        }
        Configurations.LightsON = true;
        if (Anim != null)
        {
            Anim[animNames.AnimName].normalizedTime = 0;
            Anim[animNames.AnimName].speed = -1;
            Anim.Play();
        }
        if (lampSounds.SwitchSound != null)
        {
            audioSource.Play();
        }
    }

    #region Texto

    public void MostarBotaoInteraçao(bool mostrar)
    {
        if (lampTexts.TextPrefab != null)
        {
            lampTexts.TextPrefab.SetActive(mostrar);
            if (mostrar == true)
            {
                ShowHint();
            }
            else
            {
                HideText();
            }
        }
    }

    void AddText()
    {
        if (lampTexts.enabled)
        {
            if (lampTexts.TextPrefab == null)
            {
                Debug.LogWarning(gameObject.name + ": Text prefab missing, if you want see the text, please, put the text prefab in Text Prefab slot");
                return;
            }
            GameObject go = Instantiate(lampTexts.TextPrefab, Vector3.zero, new Quaternion(0, 0, 0, 0)) as GameObject;
            go.transform.SetParent(gameObject.transform, false);

            TextObj = go.GetComponent<Canvas>();

            theText = TextObj.GetComponentInChildren<Text>();
        }
    }

    void ShowHint()
    {
        if (Configurations.LightsON)
        {
            DesligarText();
        }
        else
        {
            LigarText();
        }
    }

    void LigarText()
    {
        ShowText(lampTexts.ligarText);
    }

    void DesligarText()
    {
        ShowText(lampTexts.desligarText);
    }

    void ShowText(string txt)
    {
        if (!lampTexts.enabled || TextObj == null || theText == null)
            return;

        string tempTxt = txt;

        
        tempTxt = txt.Replace("[BUTTON]", "'" + Controls.switchButton.ToString() + "'");

        theText.text = tempTxt;
        TextObj.gameObject.SetActive(true);
    }

    void HideText()
    {
        if (!lampTexts.enabled)
            return;

        if (TextObj != null)
            TextObj.gameObject.SetActive(false);
    }

    #endregion

    #region Audio

    void AddAudioSource()
    {
        audioSource = GetComponentInChildren<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError($"Esta faltando o audio source:{audioSource}");
            return;
        }
        audioSource.volume = lampSounds.Volume;
        audioSource.spatialBlend = 1;
        audioSource.playOnAwake = false;
        audioSource.clip = lampSounds.SwitchSound;
    } 

    #endregion
    void DisableEmission()
    {

        for (int i = 0; i < Configurations.EmissiveObjects.Length; i++)
        {
            Configurations.EmissiveObjects[i].material.SetColor("_EmissionColor", Color.black);
            DynamicGI.SetEmissive(Configurations.EmissiveObjects[i], Color.black * 0);
        }

    }
    void EnableEmission()
    {
        for (int i = 0; i < Configurations.EmissiveObjects.Length; i++)
        {
            Configurations.EmissiveObjects[i].material.SetColor("_EmissionColor", Configurations.EmissColors[i]);
            DynamicGI.SetEmissive(Configurations.EmissiveObjects[i], Configurations.EmissColors[i] * 0.36f);
        }
    }
    
    IEnumerator EnableEmissionLate () {
		yield return null;
		for(int i = 0; i < Configurations.EmissiveObjects.Length; i++){
			Configurations.EmissiveObjects[i].material.SetColor ("_EmissionColor", Configurations.EmissColors[i]);
			DynamicGI.SetEmissive(Configurations.EmissiveObjects[i], Configurations.EmissColors[i] * 0.36f);
		}
	}
    
    #endregion
}
