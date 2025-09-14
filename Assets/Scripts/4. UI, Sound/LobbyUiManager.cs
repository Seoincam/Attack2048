using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;
using DG.Tweening;

public class LobbyUiManager : MonoBehaviour
{
    [SerializeField] private LoadingSO loadingSO;
    [SerializeField] private GameObject[] hideObjects;
    [SerializeField] private Image logo;
    [SerializeField] private RectTransform sky;

    [Header("Tween Setting")]
    [SerializeField] float creditDuration = 0.4f;
    [SerializeField] float logoDuration = 0.4f;
    [SerializeField] float buttonsDuration = 0.25f;
    [SerializeField] Ease buttonsEase = Ease.OutBack;
    [SerializeField] float panelDuration = 0.4f;

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
    [SerializeField] private Transform exitPanel;
    private bool isEscapePopUp = false;

    [Header("Test")]
    [SerializeField] private InputField testStartIndexInputFied;

    private Action OnEscapeButtonTapped;

    private float creditPanelY;
    private float settingPanelY;
    private float exitPanelY;

    // 뒤로가기 감지
    void Update()
    {
        MoveImage();

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

        SetAllButton(false);

        var sequence = DOTween.Sequence();
        sequence.Append(creditButton.GetComponent<RectTransform>().DOAnchorPosX(0, creditDuration).SetEase(Ease.OutCubic));
        sequence.Append(logo.DOColor(Color.white, logoDuration).SetEase(Ease.OutCubic));

        sequence.Append(startButton.GetComponent<RectTransform>().DOAnchorPosY(550, buttonsDuration).SetEase(buttonsEase));
        sequence.Join(startButton.GetComponent<Image>().DOColor(Color.white, buttonsDuration).SetEase(Ease.OutCubic));

        sequence.Append(settingButton.GetComponent<RectTransform>().DOAnchorPosY(350, buttonsDuration).SetEase(buttonsEase));
        sequence.Join(settingButton.GetComponent<Image>().DOColor(Color.white, buttonsDuration).SetEase(Ease.OutCubic));

        sequence.Append(escapeButton.GetComponent<RectTransform>().DOAnchorPosY(150, buttonsDuration).SetEase(buttonsEase));
        sequence.Join(escapeButton.GetComponent<Image>().DOColor(Color.white, buttonsDuration).SetEase(Ease.OutCubic));

        SetAllButton(true);
    }

    void InitSetting()
    {
        settingButton.onClick.AddListener(OnOpenSettingButtonTapped);
        closeSettingButton.onClick.AddListener(OnCloseSettingButtonTapped);
        SoundManager.Instance.InitPanel(bgmSlider, sfxSlider);
        settingPanelY = settingPanel.GetComponent<RectTransform>().localPosition.y;
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
        creditPanelY = creditPanel.GetComponent<RectTransform>().localPosition.y;
    }

    void InitEscape()
    {
        escapeButton.onClick.AddListener(OpenExitPanel);
        exitPanel.Find("Button Layout Group/Exit Button").GetComponent<Button>().onClick.AddListener(OnExitButtonTapped);
        exitPanel.Find("Button Layout Group/Cancle Button").GetComponent<Button>().onClick.AddListener(OnCancleExitButtonTapped);
        exitPanelY = exitPanel.GetComponent<RectTransform>().localPosition.y;
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
        isEscapePopUp = true;
        exitPanel.gameObject.SetActive(true);

        var sequence = DOTween.Sequence();
        sequence.Append(exitPanel.GetComponent<RectTransform>().DOAnchorPosY(-153, panelDuration).SetEase(Ease.OutQuart));
        sequence.Join(exitPanel.GetComponent<Image>().DOColor(Color.white, panelDuration).SetEase(Ease.OutQuart));
        sequence.Join(exitPanel.GetComponent<RectTransform>().DOScale(Vector3.one, panelDuration).SetEase(Ease.OutQuart));

        sequence.Join(startButton.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), panelDuration).SetEase(Ease.OutQuart));
        sequence.Join(settingButton.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), panelDuration).SetEase(Ease.OutQuart));
        sequence.Join(escapeButton.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), panelDuration).SetEase(Ease.OutQuart));
        sequence.Join(logo.DOColor(new Color(1, 1, 1, 0), panelDuration).SetEase(Ease.OutQuart));
    }

    private void OnCancleExitButtonTapped()
    {
        OnEscapeButtonTapped = null;

        var sequence = DOTween.Sequence();
        sequence.Append(exitPanel.GetComponent<RectTransform>().DOAnchorPosY(exitPanelY, panelDuration).SetEase(Ease.InQuart));
        sequence.Join(exitPanel.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), panelDuration + .1f).SetEase(Ease.InQuart));
        sequence.Join(exitPanel.GetComponent<RectTransform>().DOScale(Vector3.zero, panelDuration).SetEase(Ease.InQuart));

        sequence.Join(startButton.GetComponent<Image>().DOColor(Color.white, panelDuration).SetEase(Ease.InQuart));
        sequence.Join(settingButton.GetComponent<Image>().DOColor(Color.white, panelDuration).SetEase(Ease.InQuart));
        sequence.Join(escapeButton.GetComponent<Image>().DOColor(Color.white, panelDuration).SetEase(Ease.InQuart));
        sequence.Join(logo.DOColor(Color.white, panelDuration).SetEase(Ease.InQuart));

        sequence.OnComplete(() =>
        {
            exitPanel.gameObject.SetActive(false);
            SetAllButton(canInteractive: true);
            isEscapePopUp = false;
        });
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
        creditPanel.gameObject.SetActive(true);

        var sequence = DOTween.Sequence();
        sequence.Append(creditPanel.GetComponent<RectTransform>().DOAnchorPosY(-153, panelDuration).SetEase(Ease.OutQuart));
        sequence.Join(creditPanel.GetComponent<Image>().DOColor(Color.white, panelDuration).SetEase(Ease.OutQuart));
        sequence.Join(creditPanel.GetComponent<RectTransform>().DOScale(Vector3.one, panelDuration).SetEase(Ease.OutQuart));

        sequence.Join(startButton.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), panelDuration).SetEase(Ease.OutQuart));
        sequence.Join(settingButton.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), panelDuration).SetEase(Ease.OutQuart));
        sequence.Join(escapeButton.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), panelDuration).SetEase(Ease.OutQuart));
        sequence.Join(logo.DOColor(new Color(1, 1, 1, 0), panelDuration).SetEase(Ease.OutQuart));
        sequence.OnComplete(() =>
        {
            creditPanel.GetComponentInChildren<ScrollRect>().enabled = true;
        });
    }

    private void OnCloseCreditButtonTapped()
    {
        OnEscapeButtonTapped = OnCloseSettingButtonTapped;

        creditPanel.GetComponentInChildren<ScrollRect>().enabled = false;
        var sequence = DOTween.Sequence();
        sequence.Append(creditPanel.GetComponent<RectTransform>().DOAnchorPosY(creditPanelY, panelDuration).SetEase(Ease.InQuart));
        sequence.Join(creditPanel.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), panelDuration + .1f).SetEase(Ease.InQuart));
        sequence.Join(creditPanel.GetComponent<RectTransform>().DOScale(Vector3.zero, panelDuration).SetEase(Ease.InQuart));

        sequence.Join(startButton.GetComponent<Image>().DOColor(Color.white, panelDuration).SetEase(Ease.InQuart));
        sequence.Join(settingButton.GetComponent<Image>().DOColor(Color.white, panelDuration).SetEase(Ease.InQuart));
        sequence.Join(escapeButton.GetComponent<Image>().DOColor(Color.white, panelDuration).SetEase(Ease.InQuart));
        sequence.Join(logo.DOColor(Color.white, panelDuration).SetEase(Ease.InQuart));

        sequence.OnComplete(() =>
        {
            creditPanel.gameObject.SetActive(false);
            SetAllButton(canInteractive: true);
        });
    }


    // Setting Panel
    // - - - - - - - - -
    public void OnOpenSettingButtonTapped()
    {
        OnEscapeButtonTapped = OnCloseSettingButtonTapped;
        SetAllButton(canInteractive: false);
        settingPanel.gameObject.SetActive(true);

        var sequence = DOTween.Sequence();
        sequence.Append(settingPanel.GetComponent<RectTransform>().DOAnchorPosY(0, panelDuration).SetEase(Ease.OutQuart));
        sequence.Join(settingPanel.Find("Setting Panel")?.GetComponent<Image>().DOColor(Color.white, panelDuration).SetEase(Ease.OutQuart));
        sequence.Join(settingPanel.GetComponent<RectTransform>().DOScale(Vector3.one, panelDuration).SetEase(Ease.OutQuart));

        sequence.Join(startButton.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), panelDuration).SetEase(Ease.OutQuart));
        sequence.Join(settingButton.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), panelDuration).SetEase(Ease.OutQuart));
        sequence.Join(escapeButton.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), panelDuration).SetEase(Ease.OutQuart));
        sequence.Join(logo.DOColor(new Color(1, 1, 1, 0), panelDuration).SetEase(Ease.OutQuart));
    }

    public void OnCloseSettingButtonTapped()
    {
        OnEscapeButtonTapped = null;

        var sequence = DOTween.Sequence();
        sequence.Append(settingPanel.GetComponent<RectTransform>().DOAnchorPosY(settingPanelY, panelDuration).SetEase(Ease.InQuart));
        sequence.Join(settingPanel.Find("Setting Panel")?.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), panelDuration + .1f).SetEase(Ease.InQuart));
        sequence.Join(settingPanel.GetComponent<RectTransform>().DOScale(Vector3.zero, panelDuration).SetEase(Ease.InQuart));

        sequence.Join(startButton.GetComponent<Image>().DOColor(Color.white, panelDuration).SetEase(Ease.InQuart));
        sequence.Join(settingButton.GetComponent<Image>().DOColor(Color.white, panelDuration).SetEase(Ease.InQuart));
        sequence.Join(escapeButton.GetComponent<Image>().DOColor(Color.white, panelDuration).SetEase(Ease.InQuart));
        sequence.Join(logo.DOColor(Color.white, panelDuration).SetEase(Ease.InQuart));

        sequence.OnComplete(() =>
        {
            settingPanel.gameObject.SetActive(false);
            SetAllButton(canInteractive: true);
        });
    }


    // Codex Panel
    // - - - - - - - - -
    private void OnOpenCodexButtonTapped()
    {
        OnEscapeButtonTapped = OnCloseCodexButtonTapped;

        SetAllButton(canInteractive: false);
        // SetActiveHideObjects(false);
        codexPanel.gameObject.SetActive(true);
    }

    private void OnCloseCodexButtonTapped()
    {
        OnEscapeButtonTapped = null;

        SetAllButton(canInteractive: true);
        // SetActiveHideObjects(true);
        codexPanel.gameObject.SetActive(false);
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

    private void MoveImage()
    {
        var tiltSpeed = 7;
        float lerpX = Mathf.LerpAngle(codexButton.GetComponent<RectTransform>().eulerAngles.x, Mathf.Sin(Time.time) * 6, tiltSpeed * Time.deltaTime);
        float lerpY = Mathf.LerpAngle(codexButton.GetComponent<RectTransform>().eulerAngles.y, Mathf.Cos(Time.time) * 6, tiltSpeed * Time.deltaTime);
        float y = Mathf.Lerp(codexButton.GetComponent<RectTransform>().anchoredPosition.y, 600 + Mathf.Sin(Time.time) * 10, Time.deltaTime);
        codexButton.GetComponent<RectTransform>().eulerAngles = new Vector3(lerpX, lerpY, 0);
        codexButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y);

        var skyLerpX = Mathf.LerpAngle(codexButton.GetComponent<RectTransform>().eulerAngles.x, Mathf.Sin(Time.time) * 10, tiltSpeed * Time.deltaTime);
        var skyLerpY = Mathf.LerpAngle(codexButton.GetComponent<RectTransform>().eulerAngles.y, Mathf.Cos(Time.time) * 10, tiltSpeed * Time.deltaTime);
        sky.eulerAngles = new Vector3(skyLerpX, skyLerpY, 0);
    }
}
