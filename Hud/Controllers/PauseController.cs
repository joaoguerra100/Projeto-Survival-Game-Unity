using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Audio;

public class PauseController : MonoBehaviour
{
    #region Variaveis

    [Header("Scripts")]
    public static PauseController instance;

    [Header("PauseMenu")]
    [SerializeField] private GameObject pausePainel;
    [HideInInspector] public bool visiblePanel;
    [HideInInspector] public bool pause;
    [SerializeField] private GameObject btnPause;

    [Header("ConficuraçoesMenu")]
    public GameObject configuracoesPainel;
    [SerializeField] private GameObject btnConfig;

    [Header("Configuraçoes")]
    public Dropdown quality;
    public Toggle bloom, occlusion, reflection, motionBloor, deptthOfField;
    public Slider motionBloorScale;

    [Header("Video")]
    public Dropdown resolution;
    public Toggle limitFPS, windowMode, vSinc;
    public InputField textFPS;

    [Header("Audio")]
    public Slider musicsVol, effectVol;
    public AudioMixer mixer;

    [Header("Teclas")]

    [Header("Multiplayer")]

    [Header("Opçoes")]
    public Toggle autosave;

    #endregion

    #region Methods Iniciais

    void Awake()
    {
        instance = this;
    }
    void OnEnable()
    {
        visiblePanel = pausePainel.activeSelf;
    }

    void Start()
    {
        ShowAndHide();
        ApplyConfigs();
        configuracoesPainel.SetActive(false);
        pausePainel.SetActive(false);
        visiblePanel = false;
    }

    void Update()
    {
        Pause();
        //ProcurarProfile();
    }

    #endregion

    #region Opçoes

    /* private void ProcurarProfile()
    {
        if (postProcessVolume.profile.TryGetSettings(out Bloom bloom))
        {
            bloom.active = GameController.instance.bloom;
        }
        if (postProcessVolume.profile.TryGetSettings(out AmbientOcclusion occlusion))
        {
            occlusion.active = GameController.instance.occlusion;
        }
        if (postProcessVolume.profile.TryGetSettings(out ScreenSpaceReflections reflection))
        {
            reflection.active = GameController.instance.reflection;
        }
        if (postProcessVolume.profile.TryGetSettings(out MotionBlur motionBloor))
        {
            motionBloor.active = GameController.instance.motionBloor;
        }
        if (postProcessVolume.profile.TryGetSettings(out DepthOfField deptthOfField))
        {
            deptthOfField.active = GameController.instance.deptthOfField;
        }
    } */

    public void Salvar()
    {
        SaveConfigs();
        ApplyConfigs();
        RetornarPausePainel();
    }

    public void Configuracoes()
    {
        configuracoesPainel.SetActive(true);
        pausePainel.SetActive(false);
    }

    private void ApplyConfigs()
    {
        var configs = LoadConfigs();
        if (configs == null)
        {
            return;
        }

        //APLICA RESOLUÇAO E MODO JANELA
        Screen.SetResolution(configs.resolution.width, configs.resolution.width, !configs.windowMode);

        //APLICA O PRESET DE QUALIDAED DA UNITY
        QualitySettings.SetQualityLevel((int)configs.quality);

        //APLCIA O LIMITE DE FPS
        Application.targetFrameRate = configs.limitFps.limit ? configs.limitFps.fps : -1;

        //APLICA O VSINC
        QualitySettings.vSyncCount = configs.vSinc ? 1 : 0;

        //autosave = configs.autoSave;

        //APLICA O PostProcessVolume
        GameController.instance.bloom = configs.bloom;
        GameController.instance.occlusion = configs.ambientOcclusioon;
        GameController.instance.reflection = configs.reflection;
        GameController.instance.motionBloor = configs.motionBloor;
        GameController.instance.deptthOfField = configs.deptthOfField;
    }

    private ConfigModel LoadConfigs()
    {
        try
        {
            //var fileDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/ConfigData.Save";
            var fileDirectory = Application.persistentDataPath + "save.dat";
            if (!File.Exists(fileDirectory))
            {
                return null;
            }
            var binaryFormater = new BinaryFormatter();
            var file = File.OpenRead(fileDirectory);

            var configs = (ConfigModel)binaryFormater.Deserialize(file);
            file.Close();

            if (configs != null)
            {
                var option = resolution.options.Where(x => x.text == $"{configs.resolution.width}x {configs.resolution.height}").FirstOrDefault();
                resolution.value = resolution.options.IndexOf(option);
                quality.value = (int)configs.quality;
                textFPS.text = configs.limitFps.fps.ToString();
                limitFPS.isOn = configs.limitFps.limit;
                windowMode.isOn = configs.windowMode;
                vSinc.isOn = configs.vSinc;
                bloom.isOn = configs.bloom;
                occlusion.isOn = configs.ambientOcclusioon;
                reflection.isOn = configs.reflection;
                motionBloor.isOn = configs.motionBloor;
                deptthOfField.isOn = configs.deptthOfField;
                autosave.isOn = configs.autoSave;
            }

            return configs;
        }
        catch (System.Exception)
        {

            return null;
        }
    }

    private void SaveConfigs()
    {
        var resolutuinModel = new ConfigModel.Resolution();
        switch (resolution.value)
        {
            case 0:
                resolutuinModel.width = 800;
                resolutuinModel.height = 600;
                break;
            case 1:
                resolutuinModel.width = 1280;
                resolutuinModel.height = 720;
                break;
            case 2:
                resolutuinModel.width = 1920;
                resolutuinModel.height = 1080;
                break;
            case 3:
                resolutuinModel.width = 2560;
                resolutuinModel.height = 1440;
                break;
            case 4:
                resolutuinModel.width = 3840;
                resolutuinModel.height = 2160;
                break;
        }

        var configs = new ConfigModel()
        {
            bloom = bloom.isOn,
            ambientOcclusioon = occlusion.isOn,
            reflection = reflection.isOn,
            motionBloor = motionBloor.isOn,
            deptthOfField = deptthOfField.isOn,
            autoSave = autosave.isOn,
            vSinc = vSinc.isOn,
            windowMode = windowMode.isOn,
            limitFps = new ConfigModel.LimitFps()
            {
                fps = int.Parse(textFPS.text),
                limit = limitFPS.isOn
            },
            quality = (ConfigModel.Quality)quality.value,
            resolution = resolutuinModel
        };

        string caminho = Application.persistentDataPath + "save.dat";
        //var caminho = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Saves/";

        var binaryFormater = new BinaryFormatter();
        var file = File.Create(caminho);
        //var file = File.Create(caminho + "ConfigData.save");

        binaryFormater.Serialize(file, configs);
        file.Close();
    }

    #endregion

    #region SOM

    public void SomFx(float volumeFx)
    {
        mixer.SetFloat("Fx", effectVol.value);
    }
    public void SomMusica(float MusicaFx)
    {
        mixer.SetFloat("Musica", musicsVol.value);
    }

    #endregion

    #region PauseMenu

    public void Pause()
    {
        if (Input.GetKeyDown(KeyCode.Escape) & Time.timeScale == 1)
        {
            pause = true;
            ShowAndHide();
            Time.timeScale = 0;
            PlayerBracos.instance.crosshairPanel.SetActive(false);
            AudioManager.instance.MetodoSomFxHud(AudioManager.instance.fxPause);
            EventSystem.current.SetSelectedGameObject(btnPause);
            if (configuracoesPainel.activeSelf == true)
            {
                configuracoesPainel.SetActive(false);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape) & Time.timeScale == 0)
        {
            pause = false;
            ShowAndHide();
            Time.timeScale = 1;
            PlayerBracos.instance.crosshairPanel.SetActive(true);
            AudioManager.instance.MetodoSomFxHud(AudioManager.instance.fxPause);
            if (configuracoesPainel.activeSelf == true)
            {
                configuracoesPainel.SetActive(false);
            }
        }
    }

    public void BotaoPause()
    {
        EventSystem.current.SetSelectedGameObject(btnPause);
    }

    public void RetornarPausePainel()
    {
        configuracoesPainel.SetActive(false);
        pausePainel.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    void ShowAndHide()
    {
        visiblePanel = !visiblePanel;
        pausePainel.SetActive(visiblePanel);
    }

    #endregion

    #region OpçoesMenu

    public void FirstBtnConfig()
    {
        EventSystem.current.SetSelectedGameObject(btnPause);
    }


    #endregion
}
