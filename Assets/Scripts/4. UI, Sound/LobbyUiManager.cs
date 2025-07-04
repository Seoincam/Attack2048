using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class LobbyUiManager : MonoBehaviour
{
    [SerializeField] private LoadingSO loadingSO;

    [Header("Default Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button creditButton;

    [Header("Setting")]
    [SerializeField] private Button settingButton;
    [SerializeField] private Transform settingPanel;

    [Header("Codex")]
    [SerializeField] private Button codexButton;
    [SerializeField] private Transform codexPanel;
    [SerializeField] private CodexSO[] codexSO;
    private int currentCodexIndex;

    [Header("Exit")]
    [SerializeField] private Button escapeButton;
    [SerializeField] private Transform escapePanel;
    private bool isEscapePopUp = false;

    [Header("etc")]
    [SerializeField] private Transform creditPanel;
    [SerializeField] private InputField testStartIndexInputFied;

    // 뒤로가기 감지
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isEscapePopUp)
                OnCancleExitButtonTapped();
            else
                OnEscapeButtonTapped();
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
        settingPanel.Find("Close Button").GetComponent<Button>().onClick.AddListener(OnCloseSettingButtonTapped);

        SoundManager.Instance.SetPanel
        (
            settingPanel.Find("BGM Layout Group/BGM Slider").GetComponent<Slider>(),
            settingPanel.Find("SFX Layout Group/SFX Slider").GetComponent<Slider>()
        );
    }

    void InitCodex()
    {
        codexButton.onClick.AddListener(OnOpenCodexButtonTapped);
        codexPanel.Find("Close Button").GetComponent<Button>().onClick.AddListener(OnCloseCodexButtonTapped);
        codexPanel.Find("Prev Button").GetComponent<Button>().onClick.AddListener(OnPreviousCodexButtonTapped);
        codexPanel.Find("Next Button").GetComponent<Button>().onClick.AddListener(OnNextCodexButtonTapped);
        currentCodexIndex = 0;

        UpdateCodexUI();
    }

    void InitCredit()
    {
        creditButton.onClick.AddListener(OnOpenCreditButtonTapped);
        creditPanel.Find("Close Button").GetComponent<Button>().onClick.AddListener(OnCloseCreditButtonTapped);
    }

    void InitEscape()
    {
        escapeButton.onClick.AddListener(OnEscapeButtonTapped);
        escapePanel.Find("Button Layout Group/Exit Button").GetComponent<Button>().onClick.AddListener(OnExitButtonTapped);
        escapePanel.Find("Button Layout Group/Cancle Button").GetComponent<Button>().onClick.AddListener(OnCancleExitButtonTapped);
    }


    // Lobby Buttons
    // - - - - - - - - -    
    private void OnStartButtonTapped()
    {
        GameSetting.Instance.testStartIndex = int.TryParse(testStartIndexInputFied.text, out int inputIndex) ? inputIndex : 0;
        loadingSO.SceneName = "2048Game";
        SceneManager.LoadScene("Loading");
    }

    // Exit Panel
    // - - - - - - - - 
    private void OnEscapeButtonTapped()
    {
        SetAllButton(canInteractive: false);

        codexPanel.gameObject.SetActive(false);
        creditPanel.gameObject.SetActive(false);
        settingPanel.gameObject.SetActive(false);

        isEscapePopUp = true;
        escapePanel.gameObject.SetActive(true);
    }

    private void OnCancleExitButtonTapped()
    {
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
        SetAllButton(canInteractive: false);
        creditPanel.gameObject.SetActive(true);
    }

    private void OnCloseCreditButtonTapped()
    {
        SetAllButton(canInteractive: true);
        creditPanel.gameObject.SetActive(false);
    }


    // Setting Panel
    // - - - - - - - - -
    public void OnOpenSettingButtonTapped()
    {
        SetAllButton(canInteractive: false);
        settingPanel.gameObject.SetActive(true);
    }

    public void OnCloseSettingButtonTapped()
    {
        SetAllButton(canInteractive: true);
        settingPanel.gameObject.SetActive(false);
    }


    // Codex Panel
    // - - - - - - - - -
    private void OnOpenCodexButtonTapped()
    {
        SetAllButton(canInteractive: false);
        codexPanel.gameObject.SetActive(true);
    }

    private void OnCloseCodexButtonTapped()
    {
        SetAllButton(canInteractive: true);
        codexPanel.gameObject.SetActive(false);
    }

    private void OnPreviousCodexButtonTapped()
    {
        currentCodexIndex--;
        Mathf.Clamp(currentCodexIndex, min: 0, max: codexSO.Length - 1);
        UpdateCodexUI();
    }

    private void OnNextCodexButtonTapped()
    {
        currentCodexIndex++;
        Mathf.Clamp(currentCodexIndex, min: 0, max: codexSO.Length - 1);
        UpdateCodexUI();
    }

    void UpdateCodexUI()
    {
        codexPanel.Find("Prev Button").GetComponent<Button>().interactable = currentCodexIndex > 0;
        codexPanel.Find("Next Button").GetComponent<Button>().interactable = currentCodexIndex < codexSO.Length - 1;
        codexPanel.Find("Index Text").GetComponent<Text>().text = $"{currentCodexIndex + 1} / {codexSO.Length}";

        codexPanel.Find("Stage Codex/Name Text").GetComponent<Text>().text = codexSO[currentCodexIndex].slimeName ?? "이름이 없음";
        codexPanel.Find("Stage Codex/Description Text").GetComponent<Text>().text = codexSO[currentCodexIndex].slimeDescription ?? "설명이 없음";
    }


    // All Buttons
    private void SetAllButton(bool canInteractive)
    {
        startButton.interactable = canInteractive;
        creditButton.interactable = canInteractive;
        escapeButton.interactable = canInteractive;

        settingButton.interactable = canInteractive;
        codexButton.interactable = canInteractive;
    }
}
