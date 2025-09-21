using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using DG.Tweening;

public class LobbyUiManager : MonoBehaviour
{
    private Main main;

    [Header("Tween Setting")]
    public float creditDuration = 0.4f;
    public float logoDuration = 0.4f;
    public float buttonsDuration = 0.25f;
    public Ease buttonsEase = Ease.OutBack;
    public float panelDuration = 1.2f;

    [Header("Canvas")]
    [SerializeField] private Canvas defaultCanvas;
    [SerializeField] private Canvas aboveCanvas;

    [Header("Images")]
    [SerializeField] private GameObject[] hideObjects;
    [SerializeField] private Image logo;
    [SerializeField] private RectTransform sky;

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

    // 뒤로가기 감지
    void Update()
    {
        MoveImage();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscapeButtonTapped ??= OpenExitPanel;
            OnEscapeButtonTapped?.Invoke();
        }
    }

    // 초기화
    // - - - - - - - - -
    public void Init(Main main)
    {
        this.main = main;

        InitSetting();
        InitCodex();
        InitCredit();
        InitEscape();

        startButton.onClick.AddListener(OnStartButtonTapped);
        main.Sound.PlayBGM(main.Sound.LobbyBGM);
    }

    public void OnEnterLobby(bool isFirst = false)
    {
        defaultCanvas.gameObject.SetActive(true);
        aboveCanvas.gameObject.SetActive(true);
        codexButton.gameObject.SetActive(true);
        logo.gameObject.SetActive(true);
        main.Sound.InitPanel(bgmSlider, sfxSlider);
        

        if (isFirst)
        {
            SetAllButton(false);

            var sequence = DOTween.Sequence();
            sequence.Append(creditButton.GetComponent<RectTransform>().DOAnchorPosX(0, creditDuration).SetEase(Ease.OutCubic));
            sequence.Append(logo.DOColor(Color.white, logoDuration).SetEase(Ease.OutCubic));

            FadeInButton(sequence, startButton, 550);
            FadeInButton(sequence, settingButton, 350);
            FadeInButton(sequence, escapeButton, 150);

            sequence.OnComplete(() => SetAllButton(true));
        }
    }

    void InitSetting()
    {
        settingButton.onClick.AddListener(OnOpenSettingButtonTapped);
        closeSettingButton.onClick.AddListener(OnCloseSettingButtonTapped);
        main.Sound.InitPanel(bgmSlider, sfxSlider);
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
        exitPanel.Find("Button Layout Group/Exit Button").GetComponent<Button>().onClick.AddListener(OnExitButtonTapped);
        exitPanel.Find("Button Layout Group/Cancle Button").GetComponent<Button>().onClick.AddListener(OnCancleExitButtonTapped);
    }


    // Lobby Buttons
    // - - - - - - - - -    
    private void OnStartButtonTapped()
    {
        var testStartIndex = int.TryParse(testStartIndexInputFied.text, out int inputIndex) ? inputIndex : 0;
        var stageIndex = Mathf.Clamp(testStartIndex, 0, 6);

        main.LoadToStage(stageIndex);
    }

    public void TurnOff()
    {
        defaultCanvas.gameObject.SetActive(false);
        aboveCanvas.gameObject.SetActive(false);
        codexButton.gameObject.SetActive(false);
        logo.gameObject.SetActive(false);
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
        OpenPanel(sequence, exitPanel);
    }

    private void OnCancleExitButtonTapped()
    {
        OnEscapeButtonTapped = null;

        var sequence = DOTween.Sequence();
        ClosePanel(sequence, exitPanel);

        sequence.OnComplete(() =>
        {
            exitPanel.gameObject.SetActive(false);
            SetAllButton(canInteractive: true);
            isEscapePopUp = false;
        });
    }

    private void OnExitButtonTapped()
    {
        main.Sound.SaveSetting();

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
        OpenPanel(sequence, creditPanel);
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
        ClosePanel(sequence, creditPanel);
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
        OpenPanel(sequence, settingPanel);
    }

    public void OnCloseSettingButtonTapped()
    {
        OnEscapeButtonTapped = null;

        var sequence = DOTween.Sequence();
        ClosePanel(sequence, settingPanel);
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
        codexPanel.gameObject.SetActive(true);
    }

    private void OnCloseCodexButtonTapped()
    {
        OnEscapeButtonTapped = null;

        SetAllButton(canInteractive: true);
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

    // DOTween Helper
    private void FadeInButton(Sequence sequence, Button button, float targetY)
    {
        if (button == null) return;
        var rect = button.transform as RectTransform;
        var image = button.image;
        if (rect == null || image == null) return;

        sequence.Append(rect.DOAnchorPosY(targetY, buttonsDuration)
            .SetEase(buttonsEase));
        sequence.Join(image.DOColor(Color.white, buttonsDuration)
            .SetEase(Ease.OutCubic));
    }

    /// <summary>투명색</summary>
    private Color Transparent => new(1, 1, 1, 0);

    private void OpenPanel(Sequence sequence, Transform panel)
    {
        if (panel == null) return;
        var rect = panel as RectTransform;
        if (rect == null) return;
        
        panel.gameObject.SetActive(true);

        sequence.Append(rect.DOAnchorPosY(-155, panelDuration)
            .SetEase(Ease.OutBack));
        sequence.Join(rect.DOScale(Vector3.one, panelDuration)
            .SetEase(Ease.OutBack));

        sequence.Join(startButton.GetComponent<Image>().DOColor(Transparent, panelDuration)
            .SetEase(Ease.OutQuart));
        sequence.Join(settingButton.GetComponent<Image>().DOColor(Transparent, panelDuration)
            .SetEase(Ease.OutQuart));
        sequence.Join(escapeButton.GetComponent<Image>().DOColor(Transparent, panelDuration)
            .SetEase(Ease.OutQuart));
        sequence.Join(logo.DOColor(Transparent, panelDuration)
            .SetEase(Ease.OutQuart));
    }

    private void ClosePanel(Sequence sequence, Transform panel)
    {
        if (panel == null) return;
        var rect = panel as RectTransform;
        if (rect == null) return;

        sequence.Append(rect.DOAnchorPosY(-1500, panelDuration)
            .SetEase(Ease.InBack));
        sequence.Join(rect.DOScale(Vector3.zero, panelDuration)
            .SetEase(Ease.InBack));


        sequence.Join(startButton.GetComponent<Image>().DOColor(Color.white, panelDuration).SetEase(Ease.InQuart));
        sequence.Join(settingButton.GetComponent<Image>().DOColor(Color.white, panelDuration).SetEase(Ease.InQuart));
        sequence.Join(escapeButton.GetComponent<Image>().DOColor(Color.white, panelDuration).SetEase(Ease.InQuart));
        sequence.Join(logo.DOColor(Color.white, panelDuration).SetEase(Ease.InQuart));
    }

}
