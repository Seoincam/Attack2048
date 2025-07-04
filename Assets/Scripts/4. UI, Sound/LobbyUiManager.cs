using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mono.Cecil.Cil;

public class LobbyUiManager : MonoBehaviour
{
    [SerializeField] private LoadingSO loadingSO;

    [Header("Default Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button creditButton;
    [SerializeField] private Button exitButton;

    [Header("Setting")]
    [SerializeField] private Button settingButton;
    [SerializeField] private Transform settingPanel;

    [Header("Codex")]
    [SerializeField] private Button codexButton;
    [SerializeField] private Transform codexPanel;
    [SerializeField] private CodexSO[] codexSO;
    private int currentCodexIndex;

    [Header("etc")]
    [SerializeField] private Transform creditPanel;
    [SerializeField] private InputField testStartIndexInputFied;

    private bool isPanelPopOver = false;



    // 초기화
    // - - - - - - - - -
    void Start()
    {
        InitSetting();
        InitCodex();
        InitCredit();

        startButton.onClick.AddListener(GameStart);
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


    // Lobby Buttons
    // - - - - - - - - -    
    public void GameStart()
    {
        if (isPanelPopOver)
            return;

        GameSetting.Instance.testStartIndex = int.TryParse(testStartIndexInputFied.text, out int inputIndex) ? inputIndex : 0;
        loadingSO.SceneName = "2048Game";
        SceneManager.LoadScene("Loading");
    }

    public void Exit()
    {
        SoundManager.Instance?.SaveSetting();
        Application.Quit();
    }


    // Credit Panel
    // - - - - - - - - -
    private void OnOpenCreditButtonTapped()
    {
        if (isPanelPopOver)
            return;

        isPanelPopOver = true;
        creditPanel.gameObject.SetActive(true);
    }

    private void OnCloseCreditButtonTapped()
    {
        isPanelPopOver = false;
        creditPanel.gameObject.SetActive(false);
    }
    

    // Setting Panel
    // - - - - - - - - -
    public void OnOpenSettingButtonTapped()
    {
        if (isPanelPopOver)
            return;

        isPanelPopOver = true;
        settingPanel.gameObject.SetActive(true);
    }

    public void OnCloseSettingButtonTapped()
    {
        isPanelPopOver = false;
        settingPanel.gameObject.SetActive(false);
    }


    // Codex Panel
    // - - - - - - - - -
    private void OnOpenCodexButtonTapped()
    {
        if (isPanelPopOver)
            return;

        isPanelPopOver = true;
        codexPanel.gameObject.SetActive(true);
    }

    private void OnCloseCodexButtonTapped()
    {
        isPanelPopOver = false;
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
}
