using System;
using UnityEngine;
using UnityEngine.UI;

public class Porta : MonoBehaviour, InterfaceInteracao
{
    public enum OpenStyle
    {
        BUTTON,
        AUTOMATIC
    }

    [Serializable]
    public class DoorControls
    {
        public float openingSpeed = 1;
        public float closingSpeed = 1.3f;
        [Range(0, 1)]
        public float closeStartFrom = 0.6f;
        public OpenStyle openMethod; //Open by button or automatically?
        public bool autoClose = false; //Automatically close the door. Forced to true when in AUTOMATIC mode.
    }
    [Serializable]
    public class AnimNames //names of the animations, which you use for every action
    {
        public string OpeningAnim = "Door_open";
        public string LockedAnim = "Door_locked";
    }
    [Serializable]
    public class DoorSounds
    {
        public bool enabled = true;
        public AudioSource audioSource;
        public AudioClip open;
        public AudioClip close;
        public AudioClip closed;
        [Range(0, 1.0f)]
        public float volume = 1.0f;
        [Range(0, 0.4f)]
        public float pitchRandom = 0.2f;
    }
    [Serializable]
    public class DoorTexts
    {
        public bool enabled = false;
        public string openingText = "Aperte [BUTTON] para abir";
        public string closingText = "Aperte [BUTTON] para fechar";
        public string lockText = "voce precisa de uma chave!";
        public GameObject TextPrefab;
    }
    [Serializable]
    public class KeySystem
    {
        public bool enabled = false;
        [HideInInspector]
        public bool isUnlock = false;
        [Tooltip("If you have a padlock model, you can put the prefab here.")]
        public GameObject LockPrefab;
    }

    public string playerTag = "Player";
    //public Transform knob; //Empty GO in the door knobs area.

    public DoorControls Controls = new DoorControls();
    public AnimNames AnimationNames = new AnimNames();
    public DoorSounds doorSounds = new DoorSounds();
    public DoorTexts doorTexts = new DoorTexts();
    public KeySystem keySystem = new KeySystem();
    public KeyCode BotaoDeInteracao => InputManager.instance.interactKey;


    Transform player;
    public bool Opened = false;
    bool inZone = false;
    Canvas TextObj;
    Text theText;
    AudioSource SoundFX;
    Animation doorAnimation;
    Animation LockAnim;

    void Awake()
    {
        AddText();
        AddLock();
        AddAudioSource();
        doorAnimation = GetComponent<Animation>();
    }

    void Start()
    {
        if (Controls.openMethod == OpenStyle.AUTOMATIC)
            Controls.autoClose = true;

        if (playerTag == "")
            Debug.LogError("You need to set a tag!");

        if (GameObject.FindWithTag(playerTag) != null)
        {
            player = GameObject.FindWithTag(playerTag).transform;
        }
        else
        {
            Debug.LogWarning(gameObject.name + ": You need to set your player's tag to " + "'" + playerTag + "'." + " The " + "'" + gameObject.name + "'" + " can't open/close if you don't set this tag");
        }

    }


    void Update()
    {
        if (!doorAnimation.isPlaying && SoundFX.isPlaying) {
			SoundFX.Stop();
		}
		/* if(!inZone)
		{
			HideHint();
			return;
		} */

		if(Controls.openMethod == OpenStyle.AUTOMATIC && !Opened)
			OpenDoor();


        /* if(!Input.GetKeyDown(Controls.openButton) || Controls.openMethod != OpenStyle.BUTTON)
			return; */
        if (keySystem.enabled)
        {
            if (Opened)
            {
                if (!Controls.autoClose)
                    CloseDoor();
            }
            else
            {
                if (keySystem.enabled)
                {
                    if (keySystem.isUnlock)
                        OpenLockDoor();
                    else
                        PlayClosedFXs();
                }
                else
                {
                    OpenDoor();
                }
            }
        }
    }

    #region TEXT

    public void MostarBotaoInteraçao(bool mostrar)
    {
        if (doorTexts.TextPrefab != null)
        {
            doorTexts.TextPrefab.SetActive(mostrar);
            if (mostrar == true)
            {
                ShowHint();
            }
            else
            {
                HideHint();
            }
        }
    }

    void AddText()
    {
        if (doorTexts.enabled)
        {
            if (doorTexts.TextPrefab == null)
            {
                Debug.LogWarning(gameObject.name + ": Text prefab missing, if you want see the text, please, put the text prefab in Text Prefab slot");
                return;
            }
            GameObject go = Instantiate(doorTexts.TextPrefab, Vector3.zero, new Quaternion(0, 0, 0, 0)) as GameObject;
            go.transform.SetParent(gameObject.transform, false);

            TextObj = go.GetComponent<Canvas>();

            theText = TextObj.GetComponentInChildren<Text>();
        }
    }

    void HideText()
    {
        if (!doorTexts.enabled)
            return;
        
       if (TextObj != null)
            TextObj.gameObject.SetActive(false);
    }

    void ShowHint()
    {
        if (Opened)
        {
            if (!Controls.autoClose)
                CloseText();
        }
        else
        {
            if (keySystem.enabled && !keySystem.isUnlock)
                LockText();
            else
                OpenText();
        }
    }

    void HideHint()
    {
        if (Controls.openMethod == OpenStyle.BUTTON)
            HideText();
    }

    void OpenText()
    {
        ShowText(doorTexts.openingText);
    }

    void LockText()
    {
        ShowText(doorTexts.lockText);
    }

    void CloseText()
    {
        ShowText(doorTexts.closingText);
    }

    void ShowText(string txt)
    {
        if (!doorTexts.enabled || TextObj == null || theText == null)
            return;

        string tempTxt = txt;

        if (Controls.openMethod == OpenStyle.BUTTON)
            tempTxt = txt.Replace("[BUTTON]", "'" + BotaoDeInteracao.ToString() + "'");

        theText.text = tempTxt;
        TextObj.gameObject.SetActive(true);
    }

    #endregion

    void AddLock()
    {
        if (!keySystem.enabled)
            return;

        if (keySystem.LockPrefab == null)
        {
            Debug.LogWarning(gameObject.name + ": you can set a padlock prefab if you want.");
        }
        else
        {
            LockAnim = keySystem.LockPrefab.GetComponent<Animation>();
            keySystem.enabled = true;
        }
    }

    #region Abrir/Fechar
    public void Interagir()
    {
        if (Opened)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }
    void CloseDoor()
    {
        if (doorAnimation[AnimationNames.OpeningAnim].normalizedTime < 0.98f && doorAnimation[AnimationNames.OpeningAnim].normalizedTime > 0)
        {
            doorAnimation[AnimationNames.OpeningAnim].speed = -Controls.closingSpeed;
            doorAnimation[AnimationNames.OpeningAnim].normalizedTime = doorAnimation[AnimationNames.OpeningAnim].normalizedTime;
            doorAnimation.Play(AnimationNames.OpeningAnim);
        }
        else
        {
            doorAnimation[AnimationNames.OpeningAnim].speed = -Controls.closingSpeed;
            doorAnimation[AnimationNames.OpeningAnim].normalizedTime = Controls.closeStartFrom;
            doorAnimation.Play(AnimationNames.OpeningAnim);
        }
        if (doorAnimation[AnimationNames.OpeningAnim].normalizedTime > Controls.closeStartFrom)
        {
            doorAnimation[AnimationNames.OpeningAnim].speed = -Controls.closingSpeed;
            doorAnimation[AnimationNames.OpeningAnim].normalizedTime = Controls.closeStartFrom;
            doorAnimation.Play(AnimationNames.OpeningAnim);
        }
        Opened = false;

        if (Controls.openMethod == OpenStyle.BUTTON && !Controls.autoClose)
            HideText();
    }

    void OpenDoor()
    {
        doorAnimation.Play(AnimationNames.OpeningAnim);
        doorAnimation[AnimationNames.OpeningAnim].speed = Controls.openingSpeed;
        doorAnimation[AnimationNames.OpeningAnim].normalizedTime = doorAnimation[AnimationNames.OpeningAnim].normalizedTime;

        if (doorSounds.open != null)
            PlaySFX(doorSounds.open);

        Opened = true;
        if (Controls.openMethod == OpenStyle.BUTTON)
            HideText();

        keySystem.enabled = false;
    }

    void OpenLockDoor()
    {
        if (keySystem.LockPrefab != null)
        {
            LockAnim.Play("Lock_open");
            Invoke("OpenDoor", 1);
        }
        else
        {
            OpenDoor();
        }
    }
    #endregion

    #region AUDIO

    void PlaySFX(AudioClip clip)
    {
        if (!doorSounds.enabled)
            return;

        SoundFX.pitch = UnityEngine.Random.Range(1 - doorSounds.pitchRandom, 1 + doorSounds.pitchRandom);
        SoundFX.clip = clip;
        SoundFX.Play();
    }

    void PlayClosedFXs()
    {
        if (doorSounds.closed != null)
        {
            SoundFX.clip = doorSounds.closed;
            SoundFX.Play();
            if (doorAnimation[AnimationNames.LockedAnim] != null)
            {
                doorAnimation.Play(AnimationNames.LockedAnim);
                doorAnimation[AnimationNames.LockedAnim].speed = 1;
                doorAnimation[AnimationNames.LockedAnim].normalizedTime = 0;
            }


        }
    }

    void AddAudioSource()
    {
        //GameObject go = new GameObject("SoundFX");
        //go.transform.position = transform.position;
        //go.transform.rotation = transform.rotation;
        //go.transform.parent = transform;
        SoundFX = GetComponent<AudioSource>();
        SoundFX.volume = doorSounds.volume;
        SoundFX.spatialBlend = 1;
        SoundFX.playOnAwake = false;
        SoundFX.clip = doorSounds.open;
    }

    void CloseSound()
    {
        if (doorAnimation[AnimationNames.OpeningAnim].speed < 0 && doorSounds.close != null)
            PlaySFX(doorSounds.close);
    }
    #endregion

    #region Triggers

    void OnTriggerEnter(Collider other)
	{
		if(other.tag != playerTag)
			return;
		
		inZone = true;
	}
	
	void OnTriggerExit(Collider other)
	{
		if (other.tag != playerTag) 
			return;


		if(Opened && Controls.autoClose)
			CloseDoor();
		
		inZone = false;
	}
    
    #endregion
}
