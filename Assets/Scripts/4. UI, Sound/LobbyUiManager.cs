using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;

public class LobbyUiManager : MonoBehaviour
{
    [SerializeField] private LoadingSO loadingSO;

    [SerializeField] private GameObject[] hideObjects;

    [Header("Default Buttons")]
    [SerializeField] private Button startButton;

    [Header("Setting")]
    [SerializeField] private Button settingButton;
    [SerializeField] private Transform settingPanel;
    [SerializeField] private Button closeSettingButton;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Credit")]
    [SerializeField] private Button creditButton;
    [SerializeField] private Transform creditPanel;
    [SerializeField] private Button creditCloseButton;

    [Header("Codex")]
    [SerializeField] private Button codexButton;
    [SerializeField] private Transform codexPanel;
    [SerializeField] private Button codexCloseButton;

    [Header("Exit")]
    [SerializeField] private Button escapeButton;
    [SerializeField] private Transform escapePanel;
    private bool isEscapePopUp = false;

    [Header("Test")]
    [SerializeField] private InputField testStartIndexInputFied;

    private Action OnEscapeButtonTapped;

    // 뒤로가기 감지
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // if (isEscapePopUp)
            //     OnCancleExitButtonTapped();
            // else
            //     OpenExitPannel();
            // OnEscapeBu

            OnEscapeButtonTapped = OnEscapeButtonTapped != null ? OnEscapeButtonTapped : OpenExitPanel;
            OnEscapeButtonTapped?.Invoke();
        }
    }



    // 초기화
    // - - - - - - - - -
    void Start()
    {
        InitSetting();
        InitCodex();
        InitCredit();
        InitEscape();

        startButton.onClick.AddListener(OnStartButtonTapped);
        SoundManager.Instance.PlayBGM(SoundManager.Instance.LobbyBGM);
    }

    void InitSetting()
    {
        settingButton.onClick.AddListener(OnOpenSettingButtonTapped);
        closeSettingButton.onClick.AddListener(OnCloseSettingButtonTapped);
        SoundManager.Instance.InitPanel(bgmSlider, sfxSlider);
    }

    void InitCodex()
    {
        codexButton.onClick.AddListener(OnOpenCodexButtonTapped);
        codexCloseButton.onClick.AddListener(OnCloseCodexButtonTapped);
    }

    void InitCredit()
    {
        creditButton.onClick.AddListener(OnOpenCreditButtonTapped);
        creditPanel.Find("Close Button").GetComponent<Button>().onClick.AddListener(OnCloseCreditButtonTapped);
    }

    void InitEscape()
    {
        escapeButton.onClick.AddListener(OpenExitPanel);
        escapePanel.Find("Button Layout Group/Exit Button").GetComponent<Button>().onClick.AddListener(OnExitButtonTapped);
        escapePanel.Find("Button Layout Group/Cancle Button").GetComponent<Button>().onClick.AddListener(OnCancleExitButtonTapped);
    }


    // Lobby Buttons
    // - - - - - - - - -    
    private void OnStartButtonTapped()
    {
        var testStartIndex = int.TryParse(testStartIndexInputFied.text, out int inputIndex) ? inputIndex : 0;
        GameSetting.Instance.testStartIndex = Mathf.Clamp(testStartIndex, 0, 6);
        loadingSO.SceneName = "2048Game";
        SceneManager.LoadScene("Loading");
    }

    // Exit Panel
    // - - - - - - - - 
    private void OpenExitPanel()
    {
        OnEscapeButtonTapped = OnCancleExitButtonTapped;

        SetAllButton(canInteractive: false);

        codexPanel.gameObject.SetActive(false);
        creditPanel.gameObject.SetActive(false);
        settingPanel.gameObject.SetActive(false);

        isEscapePopUp = true;
        escapePanel.gameObject.SetActive(true);
    }

    private void OnCancleExitButtonTapped()
    {
        OnEscapeButtonTapped = null;

        SetAllButton(canInteractive: true);

        isEscapePopUp = false;
        escapePanel.gameObject.SetActive(false);
    }

    private void OnExitButtonTapped()
    {
        SoundManager.Instance?.SaveSetting();

#if UNITY_ANDROID && !UNITY_EDITOR
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            activity.Call<bool>("moveTaskToBack", true);
        }
#elif UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }


    // Credit Panel
    // - - - - - - - - -
    private void OnOpenCreditButtonTapped()
    {
        OnEscapeButtonTapped = OnCloseCreditButtonTapped;

        SetAllButton(canInteractive: false);
        SetActiveHideObjects(false);
        creditPanel.gameObject.SetActive(true);
    }

    private void OnCloseCreditButtonTapped()
    {
        OnEscapeButtonTapped = OnCloseSettingButtonTapped;

        SetAllButton(canInteractive: true);
        SetActiveHideObjects(true);
        creditPanel.gameObject.SetActive(false);
    }


    // Setting Panel
    // - - - - - - - - -
    public void OnOpenSettingButtonTapped()
    {
        OnEscapeButtonTapped = OnCloseSettingButtonTapped;

        SetAllButton(canInteractive: false);
        settingPanel.gameObject.SetActive(true);
        SetActiveHideObjects(false);
    }

    public void OnCloseSettingButtonTapped()
    {
        OnEscapeButtonTapped = null;

        SetAllButton(canInteractive: true);
        settingPanel.gameObject.SetActive(false);
        SetActiveHideObjects(true);
    }


    // Codex Panel
    // - - - - - - - - -
    private void OnOpenCodexButtonTapped()
    {
        OnEscapeButtonTapped = OnCloseCodexButtonTapped;

        SetAllButton(canInteractive: false);
        SetActiveHideObjects(false);
        codexPanel.gameObject.SetActive(true);
    }

    private void OnCloseCodexButtonTapped()
    {
        OnEscapeButtonTapped = null;

        SetAllButton(canInteractive: true);
        SetActiveHideObjects(true);
        codexPanel.gameObject.SetActive(false);
    }


    // All Buttons
    private void SetAllButton(bool canInteractive)
    {
        startButton.interactable = canInteractive;
        // creditButton.interactable = canInteractive;
        escapeButton.interactable = canInteractive;

        settingButton.interactable = canInteractive;
        codexButton.interactable = canInteractive;
    }

    private void SetActiveHideObjects(bool value)
    {
        foreach (var obj in hideObjects)
            obj.SetActive(value);
    }
}
